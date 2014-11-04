namespace ip4 {

    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;

    /// <summary>
    /// Provides all the information we intend to display about an interface.
    /// </summary>
    public class InterfaceInfo {

        readonly string _adapterName;
        readonly string _adapterDescription;
        readonly bool _addressAssignedByDhcp;
        readonly OperationalStatus _state;
        readonly IPAddress _address;
        readonly IPAddress _mask;

        string _stateAsString   = null;
        string _addressAsString = null;
        string _maskAsString    = null;

        public string AdapterName           { get { return _adapterName; } }
        public string AdapterDescription    { get { return _adapterDescription; } }
        public bool   AddressAssignedByDhcp { get { return _addressAssignedByDhcp; } }
        public OperationalStatus State      { get { return _state; } }

        public string StateAsString {
            get {
                if (_stateAsString == null) {
                    // Keep the status short - even some of the enum names are too long. 
                    _stateAsString = String.Empty;
                    switch (_state) {
                        case OperationalStatus.Up:             _stateAsString = "Up";        break;
                        case OperationalStatus.Down:           _stateAsString = "Down";      break;
                        case OperationalStatus.Testing:        _stateAsString = "Testing";   break;
                        case OperationalStatus.Unknown:        _stateAsString = "Unknown";   break;
                        case OperationalStatus.Dormant:        _stateAsString = "Dormant";   break;
                        case OperationalStatus.NotPresent:     _stateAsString = "Missing";   break;
                        case OperationalStatus.LowerLayerDown: _stateAsString = "LayerDown"; break;
                    }
                }
                return _stateAsString;
            }
        }

        public string Address {
            get {
                if (_addressAsString == null) {
                    _addressAsString = _address.ToString();
                }
                return _addressAsString;
            }
        }

        public string Mask {
            get {
                if (_maskAsString == null) {
                    _maskAsString = _mask.ToString();
                }
                return _maskAsString;
            }
        }


        /// <summary>
        /// Returns a collection of InterfaceInfo instances, representing all the
        /// unicast network adapters that can be found
        /// </summary>
        /// <param name="desiredFamily">MUST be either InterNetwork (IPv4) or InterNetworkV6 (IPv6)</param>
        public static IEnumerable<InterfaceInfo> GetAll(AddressFamily desiredFamily) {

            List<InterfaceInfo> result = new List<InterfaceInfo>();

            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters) {
                foreach (UnicastIPAddressInformation addressInfo in adapter.GetIPProperties().UnicastAddresses) {

                    IPAddress address = addressInfo.Address;

                    //do not include the loopback address in the list of available interfaces!
                    if (!IPAddress.IsLoopback(address) && adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback && (address.AddressFamily == desiredFamily)) {

                        bool fromDhcp = false;

                        IPv4InterfaceProperties ipv4Properties = adapter.GetIPProperties().GetIPv4Properties();
                        if (ipv4Properties != null) {

                            fromDhcp = ipv4Properties.IsDhcpEnabled && !ipv4Properties.IsAutomaticPrivateAddressingActive;

                            // Test for a link-local address.
                            // http://sourcedaddy.com/networking/checking-tcpip-configuration-with-ipconfig.html
                            // In IPV4, an IP address starting with 169.254 in a network with DCHP means 
                            // client was unable to get a DHCP address and has an Automatic Private
                            // IP Address (APIPA). Regardless of whether ipv4Properties.IsAutomaticPrivateAddressingActive
                            // returned true
                            if (address.IsLinkLocalAddress()) fromDhcp = false;
                        }

                        result.Add(
                            new InterfaceInfo(
                                adapter.Name.Trim(),
                                adapter.Description.Trim(),
                                fromDhcp,
                                adapter.OperationalStatus,
                                address,
                                addressInfo.IPv4MaskSafe()
                            )
                        );
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Private constructor - use GetAll() instead
        /// </summary>
        private InterfaceInfo(string adapterName, string adapterDescription, bool addressAssignedByDhcp, OperationalStatus state, IPAddress address, IPAddress mask) {

            _adapterName = adapterName;
            _adapterDescription = adapterDescription;
            _addressAssignedByDhcp = addressAssignedByDhcp;
            _state = state;
            _address = address;
            _mask = mask;
        }
    }
}
