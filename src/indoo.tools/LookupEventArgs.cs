namespace indoo.tools {

    using System;
    using System.Collections.Generic;
    using System.Text;

    public class LookupEventArgs : EventArgs {

        string _ipAddress;
        bool _timedOut;
        bool _skippedExternalIP;
        bool _alwaysSkip;

        public string IpAddress { get { return _ipAddress; } }
        public bool TimedOut { get { return _timedOut; } }
        public bool SkippedExternalIP { get { return _skippedExternalIP; } }
        public bool AlwaysSkip { get { return _alwaysSkip; } }

        public LookupEventArgs(string ipAddress, bool timedOut, bool skipExternalIP, bool alwaysSkip) {
            _ipAddress = ipAddress;
            _timedOut = timedOut;
            _skippedExternalIP = skipExternalIP;
            _alwaysSkip = alwaysSkip;
        }
    }
}
