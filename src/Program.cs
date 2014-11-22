using indoo.tools;

// This is a bit of a frankenstein program because it's built on top of another program
// (outerIP.exe - see externalIP.cs) and is using the persisted settings system from 
// that program.
namespace ip4 {

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Reflection;
    using ip4.formatters;
    using System.Net.Sockets;
    using System.Threading;
    using System.Diagnostics;
    using System.IO;


    public class Program {

        internal const string cProgramName = "ip4";
        internal const string cProgramDesc = "List all the IPs!";
        internal const string cCopyright = "Copyright 2014 primoz@licen.net and Glenn Fisher";
        readonly TimeSpan     cTimeout     = TimeSpan.FromMilliseconds(10000);

        internal const int cReturnSuccess = 0;
        internal const int cReturnError = 1;

        readonly string _programVersion;

        ManualResetEvent _externalOperationComplete;
        LookupEventArgs _operationCompletedArgs = null;

        externalIP outerIP;

        #region Help text
        public const string cHelp = @"Displays the IP address of each network adapter, as well as the external-
IP address if the Internet is accessible.

USAGE:
    ip4 [options]
    
    Most useful options:
        
        -o      Optimizes the order of external-IP URLs based on response
                times. It is recommended that you run ip4 with this option
                at least once. The optimized order is saved and will be 
                used each time ip4 is run.

        -s      Skip the external-IP lookup. When the external IP address 
                doesn't matter, execution time and service bandwidth can  
                be saved by skipping unnecessary lookups.
              
        -s:on   Persists the -s option so that external-IP lookups are 
                always skipped when ip4 is run (until ip4 is run with the 
                -s:off option)

                
    All options:
      
        -o  Optimizes the order of external-IP services based on response.
        -s  Skip the external-IP lookup. Shows only adapter IP addresses.
        -q  Quiet. Returns only IP addresses. 
        -v  Verbose. Full details are returned.        
        -c  Creates a new .ini file with default values (overwrites old 
            one if it exists).
        -p  Lists the URLs used for external-IP lookup in the order they 
            are tried, and the regular expression applied to each URL.
        -k  Keep URL. Adds an external-IP lookup service to the URL list.
            Use -w:<webpage> to specify the URL and -e:<regularExpression> 
            to specify the regular expression for the URL.
        -fp Format-Plain. No colour will be used in the output.

            
        -p:<positionIndex>        Perform the external-IP lookup using the 
                                  URL at the specific position (for random 
                                  page use 0).
       
        -w:<webPage>              Web page url. Works with -e switch, and 
                                  -k switch.
        -t:<miliseconds>          Download timeout. Default value is 10000
        -e:<regularExpression>    Regular expression. Works with -w 
                                  switch, and -k switch.
        -s:on                     Makes -s the default behaviour.
        -s:off                    Makes external-IP lookup the default 
                                  behaviour.

For more details, see the content of the .ini file located with the ip4
executable, or sometimes located in %appdata%\ip4\ if the executable's 
directory isn't writable.


OUTPUT:
    By default, six columns of information are displayed for each IPv4
    interface. Quiet (-q) and verbose (-v) options can adjust how much
    of each column is shown.

     * IP address
     
     * Mask 
     
     * Status - this consists of one of the following:
         · Up        - The interface can transmit data packets. 
                       (When the interface is ""Up"" the output text for the 
                       interface is colored green)
         · Down      - The interface cannot transmit data packets.
         · Testing   - The interface is running tests.
         · Unknown   - The network interface status is not known.
         · Dormant   - The interface is not in a condition to transmit 
                       data packets, it is waiting for an external event.
         · Missing   - The network interface is unable to transmit data 
                       packets because of a missing component, typically a
                       hardware component.
         · LayerDown - The network interface is unable to transmit data 
                       packets because it runs on top of one or more other 
                       interfaces, and at least one of these ""lower layer""
                       interfaces is down.
                     
     * ""via DHCP"" - this column is used to indicate which interfaces have 
         had their IP addresses assigned to them by a DHCP server. 
         When an IP address is marked as ""via DHCP"", it means the interface
         is configured to obtain its IP address using DHCP, and the current 
         IP address is not a ""fallback"" IP address such as would be used 
         when DHCP failed (aka Automatic Private IP Addresses). Note: There
         may be edge cases where both of these conditions are satisfied while 
         the current IP address did not come from a DHCP server.
         
     * Network adapter name
     
     * Network adapter description


ip4 is free and open source. It uses routines from outerIP, a command-line
utility by primoz@licen.net which which provides more functions for 
external IP addresses, such as launching a script when the address 
changes. 
OuterIP can be found at http://goo.gl/s6W5wn
";
        #endregion Help text

        #region App.config text
        const string cAppConfig = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
    <startup>
      <!-- 
      This file is automatically created when you run ip4.
      
      ip4 is compatible with .Net v2.0, but v4.0 can provide 
      the mask for an adapter when it's down. 
      -->
      <supportedRuntime version=""v4.0""/>
      <supportedRuntime version=""v2.0.50727""/>
    </startup>
</configuration>";
        #endregion App.config text

        /// <summary>
        /// Update the application.config file if it's missing
        /// </summary>
        void WriteAppConfig() {
            
			string assemblyName = Assembly.GetExecutingAssembly().Location;
            if (assemblyName.ToLowerInvariant().EndsWith(".exe")) {
                string appConfigPath = assemblyName + ".config";

                if (!File.Exists(appConfigPath)) {
                    try {
                        File.WriteAllText(appConfigPath, cAppConfig, Encoding.UTF8);
                    } catch {
                        // oh well, it wasn't that important anyway.
                        // It just gives us the ability to correctly list masks when the 
                        // adapter is down.
                    }
                }
            }
        }

        void Handle_OperationComplete(object sender, LookupEventArgs args) {

            _operationCompletedArgs = args;

            if (_externalOperationComplete != null) {
                _externalOperationComplete.Set();
            } else {
                Debug.Fail("OperationCompleted without constructing _externalOperationComplete?");
            }
        }

        int ListIPs(ProgramOptions options) {

            int result = cReturnSuccess;

            // Start the externalIP threads
            _externalOperationComplete = new ManualResetEvent(false);
            outerIP.processIP_async(
                options.SaveSkipValue,
                options.SkipExternalIP,
                new string[] { "-q" } // supress externalIP from outputting a program title/header;
            );

            // Now's a good time to create the application.config file if it's missing, because
            // external IP lookup is happening in another thread so we probably won't be holding
            // anything up.
            WriteAppConfig();

            // create the output-formatter
            FormatterBase formatter = null;
            switch (options.OutputFormat) {
                case OutputFormat.Color:
                    formatter = new Formatter_Color(options.Verbosity, true);
                    break;
                case OutputFormat.Json:
                    // I don't think anyone is ever going to use this feature
                    break;
                case OutputFormat.Xml:
                    // I don't think anyone is ever going to use this feature
                    break;
                case OutputFormat.Plain:
                    formatter = new Formatter_Color(options.Verbosity, false);
                    break;
            }

            if (formatter != null) {

                formatter.WriteTitle(
                    cProgramName,
                    _programVersion,
                    cProgramDesc,
                    cCopyright
                );


                // Output the adapter IPs
                IEnumerable<InterfaceInfo> interfaces = InterfaceInfo.GetAll(AddressFamily.InterNetwork);
                formatter.WriteInterfaces(interfaces);

                // Wait for the externalIP results
                if (_externalOperationComplete.WaitOne(cTimeout)) {

                    if (_operationCompletedArgs.SkippedExternalIP) {
                        formatter.WriteExternalIPSkipped(_operationCompletedArgs.AlwaysSkip);
                    } else if (_operationCompletedArgs.TimedOut) {
                        formatter.WriteExternalIPTimedOut();
                    } else if (!String.IsNullOrEmpty(_operationCompletedArgs.IpAddress)) {
                        formatter.WriteExternalIP(_operationCompletedArgs.IpAddress);
                    } else {
                        Debug.Fail("invalid LookupEventArgs");
                    }

                    // Just incase ExternalIP tried to tell the user something
                    formatter.WriteExternalIPConsoleOutput(_operationCompletedArgs.ConsoleOutput);

                } else {
                    formatter.WriteExternalIPTimedOut();
                }
            }

            return result;
        }

        /// <summary>
        /// Pass the command-line arguments through to externalIP and 
        /// let it do its thing.
        /// </summary>
        int RunExternalIPs(ProgramOptions options) {

            int result = cReturnSuccess;            
            outerIP.execute(options.PassthroughArgs);

            return result;
        }

        void ShowHelp() {
            Console.WriteLine(cProgramName + " v" + _programVersion + " - " + cProgramDesc);
            Console.WriteLine(cCopyright);

            Console.WriteLine();
            Console.WriteLine(cHelp);
        }


        void ShowVersion() {
            Console.Write("Version ");
            Console.WriteLine(_programVersion);
        }


        public int Run(string[] args) {

            int result = 0;
            ProgramOptions options = new ProgramOptions(args);

            switch (options.RunMode) {
                case RunMode.ListIPs:
                    result = ListIPs(options);
                    break;
                case RunMode.ConfigureExternalIP:
                    result = RunExternalIPs(options);
                    break;
                case RunMode.ShowHelp:
                    ShowHelp();
                    break;
                case RunMode.ShowVersion:
                    ShowVersion();
                    break;
            }
            return result;
        }


        public Program() {

            outerIP = new externalIP();
            outerIP.OperationComplete += Handle_OperationComplete;
            outerIP.txt_copyr = "{p}" + cProgramName + " v{0} - " + cProgramDesc + ".{p}" + cCopyright + "{p}";
            outerIP.MainProgramHelp = cHelp;

			Version assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            _programVersion = assemblyVersion.Major + "." + assemblyVersion.Minor + "." + assemblyVersion.Revision;
        }
    }
}
