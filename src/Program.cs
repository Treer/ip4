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


    public class Program {

        internal const string cProgramName = "ip4";
        internal const string cProgramDesc = "List all the IPs!";
        internal const string cCopyright = "Copyright 2014 primoz@licen.net and glenn@fisher.bio";
        readonly TimeSpan     cTimeout     = TimeSpan.FromMilliseconds(10000);

        internal const int cReturnSuccess = 0;
        internal const int cReturnError = 1;

        readonly string _programVersion;

        ManualResetEvent _externalOperationComplete;
        LookupEventArgs _operationCompletedArgs = null;

        externalIP outerIP;

        public const string cHelp = @"Displays the IP address of each network adapter, as well as the external-
IP address if the Internet is accessible.

USAGE:
    ip4 [options]
    
    Most useful options:
        
        -o      Optimizes the order of external-IP services based on 
                response times. It is recommended that you run ip4 with 
                this option at least once. The optimized order is saved 
                and will be used each time ip4 is run.

        -s      Skip the external-IP lookup. When the external IP address 
                doesn't matter, execution time and service bandwidth can  
                be saved by skipping unnecessary lookups.
              
        -s:on   Persists the -s option so that the external-IP lookups are 
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
executable. 

ip4 uses routines from outerIP, a utility by primoz@licen.net which 
provides external IP address functions, such as launching a script when 
the external IP address changes. 
OuterIP can be found at http://goo.gl/s6W5wn
";


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
