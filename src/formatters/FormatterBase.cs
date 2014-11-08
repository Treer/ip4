namespace ip4.formatters {

    using System;
    using System.Collections.Generic;
    using System.Text;

    public abstract class FormatterBase {

        protected Verbosity Verbosity;

        protected void Write(Verbosity requiredLevel, string text) {
            if (Verbosity >= requiredLevel) Console.Write(text);
        }

        protected void WriteLine(Verbosity requiredLevel, string text) {
            if (Verbosity >= requiredLevel) Console.WriteLine(text);
        }

        protected void WriteLine(Verbosity requiredLevel) {
            if (Verbosity >= requiredLevel) Console.WriteLine();
        }


        public abstract void WriteTitle(string name, string version, string description, string copyright);
        public abstract void WriteInterfaces(IEnumerable<InterfaceInfo> interfaces);
        public abstract void WriteExternalIP(string ipAddress);
        public abstract void WriteExternalIPTimedOut();
        public abstract void WriteExternalIPSkipped(bool alwaysSkip);
        public abstract void WriteExternalIPConsoleOutput(string consoleOutput);

        public FormatterBase(Verbosity verbosity) {
            Verbosity = verbosity;
        }
    }
}
