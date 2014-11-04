namespace ip4 {

    using System;
    using System.Collections.Generic;
    using System.Text;


    public enum RunMode {
        ListIPs,
        ShowHelp,
        ShowVersion,
        ConfigureExternalIP
    }

    public enum OutputFormat {
        Color,
        Plain,
        Xml,
        Json
    }

    public enum Verbosity: int {
        Quiet   = -1,
        Default =  0,
        Verbose =  1
    }

    public class ProgramOptions {

        RunMode      _runMode        = RunMode.ListIPs;
        OutputFormat _outputFormat   = OutputFormat.Color;
        bool         _skipExternalIP = false;
        bool         _saveSkipValue  = false;
        Verbosity    _verbosity      = Verbosity.Default;
        

        /// <summary>
        /// If these are not empty then ip4 should run externalIP with them
        /// instead of doing anything itself
        /// </summary>
        List<string> _passthroughArgs;

        private string AsExternalIPArg(string arg) {

            string result = null;

            // The following commands are can be passed through to externalIP,
            // anything else we can ignore.
            // Reformat to always start with a hyphen.
            //
            //  -c  create new ini file with default values (overrides old one if exists).
            //  -o  order list of public web pages based on speed for acquiring IP  address.
            //  -p  shows positioned URLs for web pages with external IP and parsing info.
            //  -p:<positionIndex>        acquire IP using url at specific position (for random page use 0)
            //
            //  -k  keep URL. Save URL using -w:<webpage> prameter and regular expression
            //      using -s:<regularExpression> to list of URLs in ini file.
            //  -w:<webPage>              web page url. Works with -s switch.
            //  -t:<miliseconds>          download timeout. Default value is 10000.
            //  -s:<regularExpression>    regular expression. Works with -w switch.
            //  -r:<count>[,<seconds>]    use -r:0 or just -r for infinite repetition.

            if (!String.IsNullOrEmpty(arg) && arg.Length > 1) {

                if (arg[0] == '/' || arg[0] == '-') {

                    string operation = arg.Substring(1).ToLowerInvariant();

                    if (operation.Length == 1) {
                        switch (operation) {
                            case "c":
                            case "o":
                            case "p":
                            case "k":
                                result = '-' + operation;
                                break;
                        }
                    } else if (operation.Length > 2 && operation[1] == ':') {

                        switch (operation) {
                            case "p":
                            case "w":
                            case "s":
                            case "t":
                            case "r":
                                result = '-' + arg.Substring(1);
                                break;
                        }
                    }
                }
            }
            return result;
        }

        public RunMode RunMode { get { return _runMode; } }
        public OutputFormat OutputFormat { get { return _outputFormat; } }
        public bool SkipExternalIP { get { return _skipExternalIP; } }
        public bool SaveSkipValue { get { return _saveSkipValue; } }
        public string[] PassthroughArgs { get { return _passthroughArgs.ToArray(); } }
        public Verbosity Verbosity { get { return _verbosity; } }

        public ProgramOptions(string[] args) {

            _passthroughArgs = new List<string>();

            bool ignoreExternalIpArgs = false; // true if ip4 instruction args were found, so skip any config args for externalIP

            for (int i = 0; i < args.Length; i++) {
                string arg = args[i].ToLower();
                switch (arg) {
                    case "--help":
                    case "-h":
                    case "/h":
                    case "-?":
                    case "/?":
                        _runMode = RunMode.ShowHelp;
                        ignoreExternalIpArgs = true;
                        break;

                    case "--version":
                        _runMode = RunMode.ShowVersion;
                        ignoreExternalIpArgs = true;
                        break;

                    case "--formatxml":
                    case "-fx":
                    case "/fx":
                        _outputFormat = OutputFormat.Xml;
                        ignoreExternalIpArgs = true;
                        break;

                    case "--formatjson":
                    case "-fj":
                    case "/fj":
                        _outputFormat = OutputFormat.Json;
                        ignoreExternalIpArgs = true;
                        break;

                    case "--formatplain":
                    case "-fp":
                    case "/fp":
                        _outputFormat = OutputFormat.Plain;
                        ignoreExternalIpArgs = true;
                        break;

                    case "--skipexternalip":
                    case "-s":
                    case "/s":
                        _skipExternalIP = true;
                        ignoreExternalIpArgs = true;
                        break;

                    case "--skipexternalipon":
                    case "-s:on":
                    case "/s:on":
                        _skipExternalIP = true;
                        _saveSkipValue = true;
                        ignoreExternalIpArgs = true;
                        break;

                    case "--skipexternalipoff":
                    case "-s:off":
                    case "/s:off":
                        _skipExternalIP = false;
                        _saveSkipValue = true;
                        ignoreExternalIpArgs = true;
                        break;


                    case "--quiet":
                    case "-q":
                    case "/q":
                        _verbosity = Verbosity.Quiet;
                        ignoreExternalIpArgs = true;
                        break;

                    case "--verbose":
                    case "-v":
                    case "/v":
                        _verbosity = Verbosity.Verbose;
                        ignoreExternalIpArgs = true;
                        break;


                    default:

                        string externalIPArg = AsExternalIPArg(args[i]);

                        if (!String.IsNullOrEmpty(externalIPArg)) {
                            _passthroughArgs.Add(externalIPArg);                        
                        }
                        break;
                }

                if (!ignoreExternalIpArgs && _passthroughArgs.Count > 0) {
                    // args for configuring externalIP were found, and there were 
                    // no commands for ip4, so configure externalIP
                    _runMode = RunMode.ConfigureExternalIP;
                }
            }
        }
    }
}
