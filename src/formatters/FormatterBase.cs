namespace ip4.formatters {

    using System;
    using System.Collections.Generic;
    using System.Text;

    public abstract class FormatterBase {

        protected Verbosity Verbosity;

        public abstract void WriteTitle(string name, string version, string description, string copyright);
        public abstract void WriteInterfaces(IEnumerable<InterfaceInfo> interfaces);
        public abstract void WriteExternalIP(string ipAddress);
        public abstract void WriteExternalIPTimedOut();
        public abstract void WriteExternalIPSkipped(bool alwaysSkip);

        public FormatterBase(Verbosity verbosity) {
            Verbosity = verbosity;
        }
    }
}
