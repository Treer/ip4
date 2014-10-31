namespace ip4 {

    using System;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;

    using indoo.tools;
    using System.Collections.Generic;

    class Program {

        private static externalIP externalIP = new externalIP();

        static void Main(string[] args) {

            /*
             * 
             * How to merge ip4 options with outerip.exe
             * 
             * -fx format as xml
             * -fj format as json
             * -fp format as plain (no colour)
             * -s skip external ip (show internal ips only)
             * -s:off turns skip off
             * -s:on  turns skip on
             * 
             * 
             * 
              -?  or   -h    --help.
              -c  create new ini file with default values (overrides old one if exists).
              -o  order list of public web pages based on speed for acquiring IP  address.
              -q  quiet, returns only ip addresses and name (no mask, dhcp or description).
              -v  verbose. Reply with detailed info.
              -p  shows positioned URLs for web pages with external IP and parsing info.
              -p:<positionIndex>        acquire IP using url at specific position (for

              -k  keep URL. Save URL using -w:<webpage> prameter and regular expression
                  using -s:<regularExpression> to list of URLs in ini file.
              -w:<webPage>              web page url. Works with -s switch.
              -t:<miliseconds>          download timeout. Default value is 10000.
              -s:<regularExpression>    regular expression. Works with -w switch.
              -r:<count>[,<seconds>]    use -r:0 or just -r for infinite repetition.


             * 
             * 
             * 
              -2  program acquires IP address from two different sources.
              -a  all. Verbose. Process all pages using default order.
              -x  program executes <exeFile> when current IP is also included in list of
                  IP addresses <ipList>.
              -l  executes program <exeFile> when external IP changes, first parameter
                  of execution is new IP address.
              -e:<exeFile>              path to exe file. Works with -x or -l switch.
              -i:<ipList>               comma separated ipList (without spaces).
  
                        */
            //todo: parse commandline
            Run(null);

            //externalIP.execute(args);
        }

        static void Run(object options) {

            IEnumerable<InterfaceInfo> interfaces = InterfaceInfo.GetAll(AddressFamily.InterNetwork);

            int maxAddressLength = 0;
            int maxMaskLength = 0;
            int maxStatusLength = 0;
            int maxNameLength = 0;
            bool dhcpAddressesFound = false;

            foreach (InterfaceInfo info in interfaces) {

                maxAddressLength = Math.Max(maxAddressLength, info.Address.Length);
                maxMaskLength    = Math.Max(maxMaskLength, info.Mask.Length);
                maxStatusLength  = Math.Max(maxStatusLength, info.StateAsString.Length);
                maxNameLength    = Math.Max(maxNameLength, info.AdapterName.Length);

                dhcpAddressesFound |= info.AddressAssignedByDhcp;
            }


            Console.WriteLine();
            Console.WriteLine("Interfaces:");

            ConsoleColor textColor = new ConsoleColor();
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

                    // make sure we don't take up more than one line per ip

                    if (isOperational) {
                        textColor.TextColor((int)(ConsoleForeground.Green | ConsoleForeground.Intensity));
                        //textColor.TextColor(((int)ConsoleBackground.Green) + (int)(ConsoleForeground.Intensity));
                    } else {
                        textColor.TextColor((int)(ConsoleForeground.Red));
                        //textColor.TextColor(((int)ConsoleBackground.Red)   + (int)(ConsoleForeground.Intensity));
                    }

                    Console.Write(output);
                    int lineChars = output.Length;

                    output = info.AdapterName.PadRight(maxNameLength);
                    int remainingChars = Console.WindowWidth - (1 + lineChars + output.Length);
                    if (remainingChars < 0) output = output.Substring(0, Math.Max(0, output.Length + remainingChars));

                    textColor.TextColor((int)(ConsoleForeground.Red | ConsoleForeground.Green | ConsoleForeground.Blue | ConsoleForeground.Intensity));
                    Console.Write(output);
                    lineChars += output.Length;

                    output = " " + info.AdapterDescription;
                    remainingChars = Console.WindowWidth - (1 + lineChars + output.Length);
                    if (remainingChars < 0) output = output.Substring(0, Math.Max(0, output.Length + remainingChars));

                    //textColor.TextColor((int)(ConsoleForeground.Blue)); // Blue is the darkest colour
                    //textColor.TextColor((int)(ConsoleForeground.Red | ConsoleForeground.Green | ConsoleForeground.Blue));
                    textColor.TextColor((int)(ConsoleForeground.Intensity));
                    Console.WriteLine(output);
                }

            } finally {
                textColor.ResetColor();
            }
            Console.WriteLine();
            Console.WriteLine("External IP address: ");
            Console.WriteLine(" Not implemented yet.");

            Console.WriteLine();
        }
    }
}
