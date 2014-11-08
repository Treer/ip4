namespace indoo.tools {

    using System;
    using System.Collections.Generic;
    using System.Text;

    public class LookupEventArgs : EventArgs {

        string _ipAddress;
        string _consoleOutput;
        bool _timedOut;
        bool _skippedExternalIP;
        bool _alwaysSkip;

        public string IpAddress { get { return _ipAddress; } }
        public string ConsoleOutput { get { return _consoleOutput; } }
        public bool TimedOut { get { return _timedOut; } }
        public bool SkippedExternalIP { get { return _skippedExternalIP; } }
        /// <summary>
        /// True if the ini file is indicating to always skip the external ip
        /// </summary>
        public bool AlwaysSkip { get { return _alwaysSkip; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress">quad-dotted external ip address, if lookup was successful</param>
        /// <param name="consoleOutput">Text such as errors and status that externalIP would have written to the console</param>
        /// <param name="timedOut">true if the lookup failed</param>
        /// <param name="skipExternalIP">true if the lookup was skipped (whether due to -s command, or settings in the .ini file)</param>
        /// <param name="alwaysSkip">true if the .ini file currently indicates to always skip the lookup</param>
        public LookupEventArgs(string ipAddress, string consoleOutput, bool timedOut, bool skipExternalIP, bool alwaysSkip) {
            _ipAddress = ipAddress;
            _timedOut = timedOut;
            _skippedExternalIP = skipExternalIP;
            _alwaysSkip = alwaysSkip;
            _consoleOutput = consoleOutput;
        }
    }
}
