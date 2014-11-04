using indoo.tools;

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
        internal const string cCopyright   = "Copyright 2014 primoz@licen.net, glenn@fisher.bio";
        readonly TimeSpan     cTimeout     = TimeSpan.FromMilliseconds(10000);

        internal const int cReturnSuccess = 0;
        internal const int cReturnError = 1;

        readonly string _programVersion;

        ManualResetEvent _externalOperationComplete;
        LookupEventArgs _operationCompletedArgs = null;

        externalIP outerIP;


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
                new string[] { }
            );

            // create the output-formatter
            FormatterBase formatter = null;
            switch (options.OutputFormat) {
                case OutputFormat.Color:
                    formatter = new Formatter_Color(options.Verbosity, true);
                    break;
                case OutputFormat.Json:
                    break;
                case OutputFormat.Xml:
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
            Console.WriteLine("Help goes here");
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

			Version assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            _programVersion = assemblyVersion.Major + "." + assemblyVersion.Minor + "." + assemblyVersion.Revision;
        }
    }
}
