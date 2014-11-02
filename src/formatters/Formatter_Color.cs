namespace ip4.formatters {

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Net.NetworkInformation;

    class Formatter_Color: FormatterBase {

        bool _color;
        ConsoleColor _orginalColor;

        void SetColor(ConsoleColor textColor) {
            if (_color) {
                Console.ForegroundColor = textColor;
            }
        }
        void ResetColor() {
            SetColor(_orginalColor);
        }

        public override void WriteTitle(string name, string version, string description, string copyright) {
            Console.WriteLine(name + " v" + version + " - " + description);
            //Console.WriteLine(copyright);
            Console.WriteLine();
        }


        public override void WriteInterfaces(IEnumerable<InterfaceInfo> interfaces) {

            int maxAddressLength = 0;
            int maxMaskLength    = 0;
            int maxStatusLength  = 0;
            int maxNameLength    = 0;
            bool dhcpAddressesFound = false;

            foreach (InterfaceInfo info in interfaces) {

                maxAddressLength = Math.Max(maxAddressLength, info.Address.Length);
                maxMaskLength = Math.Max(maxMaskLength, info.Mask.Length);
                maxStatusLength = Math.Max(maxStatusLength, info.StateAsString.Length);
                maxNameLength = Math.Max(maxNameLength, info.AdapterName.Length);

                dhcpAddressesFound |= info.AddressAssignedByDhcp;
            }


            Console.WriteLine("Interfaces:");
            try {
                foreach (InterfaceInfo info in interfaces) {

                    bool isOperational = info.State == OperationalStatus.Up;

                    string output = String.Format(
                        " {0} {1} {2} {3} ",
                        info.Address.PadRight(maxAddressLength),
                        info.Mask.PadRight(maxMaskLength),
                        ("[" + info.StateAsString + "]").PadRight(maxStatusLength + 2),
                        info.AddressAssignedByDhcp ? "DHCP" : (dhcpAddressesFound ? "    " : "   ") // if none of the interfaces has DHCP then can afford to make the column slightly narrowed than would otherwise be needed.
                    );

                    SetColor(isOperational ? ConsoleColor.Green : ConsoleColor.DarkRed);

                    Console.Write(output);

                    // make sure we don't take up more than one line per ip
                    int lineChars = output.Length;

                    output = info.AdapterName.PadRight(maxNameLength);
                    int remainingChars = Console.WindowWidth - (1 + lineChars + output.Length);
                    if (remainingChars < 0) {
                        output = output.Substring(0, Math.Max(0, output.Length + remainingChars));
                    }

                    SetColor(ConsoleColor.White);
                    Console.Write(output);
                    lineChars += output.Length;

                    output = " " + info.AdapterDescription;
                    remainingChars = Console.WindowWidth - (1 + lineChars + output.Length);
                    if (remainingChars < 0) {
                        output = output.Substring(0, Math.Max(0, output.Length + remainingChars));
                    }

                    SetColor(ConsoleColor.DarkGray);
                    Console.WriteLine(output);
                }

            } finally {
                ResetColor();
            }

        }


        public override void WriteExternalIP(string ipAddress) {
            Console.WriteLine();
            Console.WriteLine("External IP address: ");
            
            SetColor(ConsoleColor.Green);
            Console.WriteLine(" " + ipAddress);
            ResetColor();
        }
        

        public override void WriteExternalIPTimedOut() {
            Console.WriteLine();
            Console.WriteLine("External IP address: ");

            if (_color) Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" No internet connection (operation timed out).");
            if (_color) Console.ForegroundColor = _orginalColor;
        }


        public override void WriteExternalIPSkipped(bool alwaysSkip) {

            Console.WriteLine();

            if (alwaysSkip) {
                Console.Write("Skipped external IP address. To stop skipping, run: ip4 -s:off");
            } else {
                Console.Write("Skipped external IP address");
            }
        }


        public Formatter_Color(Verbosity verbosity, bool color): base(verbosity) {
            
            _color = color;
            _orginalColor = Console.ForegroundColor;
        }
    }
}
