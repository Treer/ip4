# ip4

A Windows command line utility to list the IP address of every network adapter, as well as the external IP address if the Internet is accessible.

 * [Executable](https://github.com/Treer/ip4/releases/latest)
 * [Source code](https://github.com/Treer/ip4)
 
<img src="https://treer.github.io/images/ip4screenshot.png" width="677" height="343" />

Saves hunting through pages of noise from `ipconfig /all`

### Usage:
    
        ip4 [options]
    
    Most useful options:
        
        -o      Optimizes the order of external-IP URLs based on response
                times. It is recommended that you run ip4 with this option
                at least once. The optimized order is saved and will be 
                used each time ip4 is run.

        -s      Skip the external-IP lookup. When the external IP address 
                doesn't matter, execution time and service bandwidth can  
                be saved by skipping unnecessary lookups.
              
        -s:on   Persists the -s option so that external-IP lookups are 
                always skipped when ip4 is run (until ip4 is run with the 
                -s:off option)

                
    All options:
      
        -o  Optimizes the order of external-IP services based on response.
        -s  Skip the external-IP lookup. Shows only adapter IP addresses.
        -q  Quiet. Returns only IP addresses. 
        -v  Verbose. Full details are returned.        
        -c  Creates a new .ini file with default values (overwrites old 
            one if it exists).
        -p  Lists the URLs used for external-IP lookup in the order they 
            are tried, and the regular expression applied to each URL.
        -k  Keep URL. Adds an external-IP lookup service to the URL list.
            Use -w:<webpage> to specify the URL and -e:<regularExpression> 
            to specify the regular expression for the URL.
        -fp Format-Plain. No colour will be used in the output.

            
        -p:<positionIndex>        Perform the external-IP lookup using the 
                                  URL at the specific position (for random 
                                  page use 0).
       
        -w:<webPage>              Web page url. Works with -e switch, and 
                                  -k switch.
        -t:<miliseconds>          Download timeout. Default value is 10000
        -e:<regularExpression>    Regular expression. Works with -w 
                                  switch, and -k switch.
        -s:on                     Makes -s the default behaviour.
        -s:off                    Makes external-IP lookup the default 
                                  behaviour.


For more details, see the content of the .ini file located with the ip4
executable, or sometimes located in `%appdata%\ip4\` if the executable's 
directory isn't writable.

### Output:
By default, six columns of information are displayed for each IPv4 interface. Quiet (-q) and verbose (-v) options can adjust how much of each column is shown.

* IP address
* Mask 
* Status - this consists of one of the following:
    * Up - The interface can transmit data packets. (When the interface is "Up" the output text for the interface is colored green)
    * Down      - The interface cannot transmit data packets.
    * Testing   - The interface is running tests.
    * Unknown   - The network interface status is not known.
    * Dormant   - The interface is not in a condition to transmit data packets, it is waiting for an external event.
    * Missing   - The network interface is unable to transmit data packets because of a missing component, typically a hardware component.
    * LayerDown - The network interface is unable to transmit data packets because it runs on top of one or more other  interfaces, and at least one of these "lower layer" interfaces is down.
* "via DHCP" - this column is used to indicate which interfaces have had their IP addresses assigned to them by a DHCP server. When an IP address is marked as "via DHCP", it means the interface is configured to obtain its IP address using DHCP, and the current IP address is not a "fallback" IP address such as would be used when DHCP failed (aka Automatic Private IP Addresses). Note: There may be edge cases where both of these conditions are satisfied while the current IP address did not come from a DHCP server.     
* Network adapter name
* Network adapter description

### Credits:
ip4 is free and open source. It uses routines from [outerIP](http://primocode.blogspot.com.au/2013/12/i-spent-couple-of-hours-searching-for.html), a command-line utility by primoz@licen.net which provides more functions related to the external IP addresses, such as launching a script if the external IP address changes.
