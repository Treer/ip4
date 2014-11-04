namespace ip4.formatters {

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Net.NetworkInformation;

    class Formatter_Color: FormatterBase {

        bool _color;
        ConsoleColor _orginalColor;
        string _ipIndent;

        void SetColor(ConsoleColor textColor) {
            if (_color) {
                Console.ForegroundColor = textColor;
            }
        }
        void ResetColor() {
            SetColor(_orginalColor);
        }


        public override void WriteTitle(string name, string version, string description, string copyright) {
            WriteLine(Verbosity.Default);
            WriteLine(Verbosity.Default, name + " v" + version + " - " + description);
            WriteLine(Verbosity.Default, copyright);
            WriteLine(Verbosity.Default);
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


            WriteLine(Verbosity.Default, "Interfaces:");
            try {
                foreach (InterfaceInfo info in interfaces) {

                    bool isOperational = info.State == OperationalStatus.Up;

                    string output = _ipIndent + info.Address.PadRight(maxAddressLength);

                    SetColor(isOperational ? ConsoleColor.Green : ConsoleColor.DarkRed);
                    Console.Write(output);

                    // make sure we don't take up more than one line per ip
                    int lineChars = output.Length;

                    if (Verbosity >= Verbosity.Default) {
                        // Show the mask
                        output = " " + info.Mask.PadRight(maxMaskLength);

                        SetColor(isOperational ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);
                        Console.Write(output);
                        lineChars += output.Length;


                        // Show whether network is up/down and whether the address came from dchp
                        output = String.Format(
                            " {0} {1} ",
                            ("[" + info.StateAsString + "]").PadRight(maxStatusLength + 2),
                            info.AddressAssignedByDhcp ? "via DHCP" : (dhcpAddressesFound ? "        " : "     ") // if none of the interfaces has DHCP then can afford to make the column slightly narrowed than would otherwise be needed.
                        );

                        SetColor(isOperational ? ConsoleColor.Green : ConsoleColor.DarkRed);
                        Console.Write(output);
                        lineChars += output.Length;
                    }

                    // Show the adapter name
                    output = info.AdapterName.PadRight(maxNameLength);
                    int remainingChars = Console.WindowWidth - (1 + lineChars + output.Length);
                    if (remainingChars < 0) {
                        output = output.Substring(0, Math.Max(0, output.Length + remainingChars));
                    }

                    if (Verbosity >= Verbosity.Default) {

                        // show the adapter description
                        SetColor(ConsoleColor.White);
                        Console.Write(output);
                        lineChars += output.Length;

                        output = " " + info.AdapterDescription;

                        if (Verbosity < Verbosity.Verbose) {
                            // Trim the description to ensure the adpater info fits on one line
                            remainingChars = Console.WindowWidth - (1 + lineChars + output.Length);
                            if (remainingChars < 0) {
                                output = output.Substring(0, Math.Max(0, output.Length + remainingChars));
                            }
                        }

                        SetColor(ConsoleColor.DarkGray);
                        Console.WriteLine(output);
                    } else {
                        Console.WriteLine();
                    }
                }

            } finally {
                ResetColor();
            }

        }


        public override void WriteExternalIP(string ipAddress) {
            WriteLine(Verbosity.Quiet);
            WriteLine(Verbosity.Default, "External IP address: ");
            
            SetColor(ConsoleColor.Green);
            WriteLine(Verbosity.Quiet, _ipIndent + ipAddress);
            ResetColor();
        }
        

        public override void WriteExternalIPTimedOut() {
            WriteLine(Verbosity.Default);
            WriteLine(Verbosity.Default, "External IP address: ");

            if (_color) Console.ForegroundColor = ConsoleColor.Red;
            WriteLine(Verbosity.Default, " No internet connection found.");
            if (_color) Console.ForegroundColor = _orginalColor;
        }


        public override void WriteExternalIPSkipped(bool alwaysSkip) {

            if (alwaysSkip) {
                WriteLine(Verbosity.Default);
                WriteLine(Verbosity.Default, "Skipped external IP address. To stop skipping, run: ip4 -s:off");
            } else {
                // If skipping isn't permanently on then the user must have typed -s, so no need
                // to fill lines informing them that external IP is being skipped
                //Console.Write("Skipped external IP address");
            }
        }

        public Formatter_Color(Verbosity verbosity, bool color): base(verbosity) {
            
            _color = color;
            _orginalColor = Console.ForegroundColor;
            _ipIndent = (Verbosity > Verbosity.Quiet) ? " " : "";
        }
    }
}
