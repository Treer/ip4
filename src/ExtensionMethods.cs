namespace ip4 {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Text;

    using System.Runtime.CompilerServices;


    public static class ExtensionMethods {
        /// <summary>
        /// The .Net Framework has a bug where UnicastIPAddressInformation.IPv4Mask() returns
        /// null when networks are down, this method is a version of IPv4Mask() that returns
        /// IPAddress.None instead of null.
        /// 
        /// The bug is fixed in version 4.0 of the framework, so targetting both 2.0 and 4.0 
        /// with <SupportedRuntime> could be the best solution:
        /// http://msdn.microsoft.com/en-us/library/vstudio/ff602939%28v=vs.100%29.aspx
        /// </summary>
        /// <returns>The IPv4 mask, or IPAddress.None if the mask is unavailable</returns>
        public static IPAddress IPv4MaskSafe(this UnicastIPAddressInformation addressInfo) {

            IPAddress result = addressInfo.IPv4Mask;

            if (result == null) {
                if (addressInfo.Address.IsLinkLocalAddress()) {
                    result = IPAddress.Parse("255.255.0.0");
                } else {
                    // Perhaps can get it the same way as this:
                    // http://stackoverflow.com/questions/11834004/how-to-retrieve-ip-v6-subnet-mask-length
                    result = IPAddress.None;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns true if the address is a link-local address.
        /// https://en.wikipedia.org/wiki/Link-local_address
        /// </summary>
        public static bool IsLinkLocalAddress(this IPAddress address) {

            bool result = false;

            switch (address.AddressFamily) {
                case AddressFamily.InterNetwork:
                    // IPv4 address
                    // Link-local addresses for IPv4 are defined in the address block 169.254.0.0/16
                    byte[] octets = address.GetAddressBytes();
                    result = octets[0] == 169 && octets[1] == 254;
                    break;

                case AddressFamily.InterNetworkV6:
                    // IPv6 address
                    // Link-local addresses for IPv6 are defined with the address prefix fe80::/64
                    result = address.IsIPv6LinkLocal;
                    break;

                default:
                    Debug.Fail("Unknown address type");
                    break;
            }

            return result;
        }
    }
}

namespace System.Runtime.CompilerServices {

    #if Framework4
    #else
        // Hack to make extension methods work in .Net 2.0
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        public class ExtensionAttribute : Attribute {
        }
    #endif
}
