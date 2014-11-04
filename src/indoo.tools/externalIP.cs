// This partial class-file contains my c# additions to externalIP.
using System.Collections.Generic;
namespace indoo.tools
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Microsoft.VisualBasic.CompilerServices;
    using System.Threading;
    using Microsoft.VisualBasic;

    /// <summary>
    /// externalIP is the main class of outerIP.exe, and is providing 
    /// ip4 with external ip functionality.
    /// 
    /// Originally the code was decompiled from outerIP.exe, written 
    /// by primoz@licen.net
    /// outerIP can be obtained from http://primocode.blogspot.com.au/2013/12/i-spent-couple-of-hours-searching-for.html
    ///
    /// This version has been heavily modified and cut down for use
    /// use as part of ip4, with permission from primoz@licen.net.
    /// 
    /// This version is released under the MIT Licence (see LICENCE file)
    /// </summary>
	public partial class externalIP
	{
        /// <summary>
        /// Some strings have been changed to better fit ip4
        /// </summary>
        private void setText() {
            this.txtP = "{p}";
            this.txtC = "' ";
            this.txt_site = "Site: ";
            this.txt_time1 = "  time: ";
            this.txt_invalid1 = "  - INVALID SITE AND/OR SETTINGS, URL IS DISABLED";
            this.txt_ipBracket = " ({0})";
            this.txt_result = "Computer's external IP: {0}";
            this.txt_dlFail = "Download failed: {0}";
            this.txt_regexFail = "Malformed regex: {0}";
            this.txt_resultVerbose = "External IP={0}, time={1}ms, url={2}";
            this.txt_timeoutVerbose = "Timeout, time>{0}ms, url={1}";
            this.txt_timeout = "Timeout > {0}ms.";
            if (String.IsNullOrEmpty(this.txt_copyr)) {
                this.txt_copyr = "{p}OuterIP v{0} - Acquires external IP from public web pages.{p}Copyright (c) 2013 primoz@licen.net{p}";
            }
            this.txt_errDesc = "Current IP address missing. ;Exe file path is invalid or missing. ;There is no valid provided url(s) and/or extraction regular expression.{p}If you delete current ini file then valid example with urls will be {p}automatically written into ini file.";
            
            this.txt_err1 = "Program also cannot write to:{p}{0}{p}Enable writing to this location and/or set correct permissions. {p}";
            
            this.txt_err2 = "";
            this.txt_err2 += "{p}****  ERROR !!! ****{p}{p}";
            this.txt_err2 += "Input parameters are invalid. Two parameters and list of pages that {p}";
            this.txt_err2 += "provide public IP address with regular expression for extraction of {p}";
            this.txt_err2 += "IP addresses are required.{p}";
            this.txt_err2 += "Check contents of ini file for additional instructions. {p}";
            this.txt_err2 += "Missing parameters are: {p}";
            this.txt_err2 += "{0}{p}";
            
            this.txt_err3 = "{p}ERROR!!!{p}{p}Cannot get external IP.{p}";
            
            // changed -s to -e, since this is remapped by ProgramOptions
            this.txt_err4 = "{p}ERROR!!!{p}{p}Both parameters -w and -e should be used at same time.{p} URL should starts with http. See help /? for details.{p}";
            
            this.txt_err5 = "{p}ERROR!!!{p}{p}Cannot acquire webpage:{p}{0}{p}";
            
            this.txt_err6 = "{p}ERROR!!!{p}{p}Cannot create regular expresion object. Wrong regex syntax. Error description:{p}{0}{p}";
            
            this.txt_err7 = "{p}ERROR!!!{p}{p}Cannot create regular expresion match object. Wrong syntax. Error description:{p}{0}{p}";
            
            this.txt_err8 = "{p}ERROR!!!{p}{p}Regular expression error. See description for details:{p}{0}{p}";
            
            this.txt_err9 = "Regular expression cannot match any string. Regular expression string is:{p}{0}{p}Content of downloaded web page is:{p}{1}{p}";
            
            this.txt_err10 = "{p}ERROR!!!{p}{p}No valid file <exeFile> for execution{p}";            
            this.txt_err10 = "{p}ERROR!!!{p}{p}No valid URL index.{p}";
            
            this.txt_infoRegex = "Regular expression successfully parsees this text:{p}{2}{p}from folowing web page in {1} seconds:{p}{0}{p}";

            this.txt_infoTiming = "Timing and ordering the web pages...";

            this.txt_info1 = "New ini file is written to:{p}    {0}{p}{p}See file contents for future instruction and change data if necessary.{p}";
            
            // txt_info2 is displayed when the -p option is used
            this.txt_info2 = "External IP is parsed from public web page. This list contains URL {p}";
            this.txt_info2 += "addresses for web pages with written external IP and appropriate {p}";
            this.txt_info2 += "regular expression with word 'content' as matching group for each page. {p}";
            this.txt_info2 += "External IP is parsed from first (or specific) position. Initial order is {p}";
            this.txt_info2 += "set according to predefined internal data but (highly recommended) use  {p}";
            this.txt_info2 += "of -o switch sets new position for each URL according to current speed {p}";
            this.txt_info2 += "and location. {p}{p}";
            this.txt_info2 += "List contains position, followed with URL address and regular expression  {p}";
            this.txt_info2 += "for each page in next line:{p}{p}{0}{p}";
            this.txt_info2 += "";

            // Default contents for ini file (appended to bottom of commented-out this.txt_helpExtended)
            this.txt_ini = "";
            this.txt_ini += "' {p}";
            this.txt_ini += "' All default settings in this ini file should be valid for successful {p}";
            this.txt_ini += "' determination of current public IP address of this computer. Even if this file {p}";
            this.txt_ini += "' is not in use program uses same data as is written in default ini file to {p}";
            this.txt_ini += "' acquire IP address. Web pages are downloaded using GET method, in the case {p}";
            this.txt_ini += "' of problems alternative POST method can be used with -m switch.{p}";
            this.txt_ini += "' {p}";
            this.txt_ini += "' Instructions for configuration used in this file:{p}";
            this.txt_ini += "' Write webpage url (link should not contains semicolon ;){p}";
            this.txt_ini += "' Url should always start with http. In next line add regular expression {p}";
            this.txt_ini += "' that extract IP from page, see examples at end of this text. Use {p}";
            this.txt_ini += "' parameter 'content' for extracting string with IP address as is shown in {p}";
            this.txt_ini += "' example. Parser uses .net dialect, word 'content' is used for regular {p}";
            this.txt_ini += "' expression's group. Each line with url should be followed with line that {p}";
            this.txt_ini += "' contains regular expression for this url. {p}";
            this.txt_ini += "' Each line that starts with apostrophe or # will be ignored. Eventual empty {p}";
            this.txt_ini += "' lines will be ignored also. Url can be optionally followed with IP address {p}";
            this.txt_ini += "' within same line for that page in form: {p}";
            this.txt_ini += "' \"<url>; XXX.XXX.XXX.XXX\" {p}";
            this.txt_ini += "' without quotes. IP can be add manually or automatically with command -o. IP is {p}";
            this.txt_ini += "' added only when output is same as output for page with normal URL. In this case {p}";
            this.txt_ini += "' page is acquired with IP address - in this case DNS query is eliminated and {p}";
            this.txt_ini += "' answer is much faster. This also eliminate potent ional DNS problems. Order for {p}";
            this.txt_ini += "' URL and parsing information is also important, faster pages should be listed {p}";
            this.txt_ini += "' first.{p}";
            this.txt_ini += "' Line that starts with checkIPs = <ipList> that contains comma separated list {p}";
            this.txt_ini += "' of actual public IP addresses that should not be same as extracted from listed {p}";
            this.txt_ini += "' pages. If this happens then exe file is executed. Exe file can be also provided {p}";
            this.txt_ini += "' in line that starts with runFile = <exeFile>. This functionality can help {p}";
            this.txt_ini += "' protect your privacy - in the case when your actual IP is revealed you can {p}";
            this.txt_ini += "' block all internet traffic, for example. {p}";
            this.txt_ini += "' {p}";
            this.txt_ini += "' {p}";
            this.txt_ini += "' Configuration:{p}";
            // lastIp, checkIPs, and runFile are all configuration settings for features of
            // outerIP.exe that ip4.exe doesn't provide, so they are commented out:
            //    this.txt_ini += "lastIP = 127.0.0.1{p}";
            //    this.txt_ini += "checkIPs = 127.0.0.1{p}";
            //    this.txt_ini += "runFile = c:\\something.bat{p}";
            this.txt_ini += "skipExternalIPLookup = false{p}";
            this.txt_ini += "http://api.externalip.net/ip{p}";
            this.txt_ini += "(?<content>.*){p}";
            this.txt_ini += "http://www.whatsmyip.us/{p}";
            this.txt_ini += "copyClip\\(\\)\"\\>\\s(?<content>.*?)\\<\\/textarea\\>{p}";
            this.txt_ini += "http://2ip.ru/{p}";
            this.txt_ini += "\\<big\\ id=\"d_clip_button\"\\>(?<content>.*?)\\<\\/big\\>{p}";
            this.txt_ini += "http://whatsmyip.net/{p}";
            this.txt_ini += "\\<input\\ type\\=\"text\"\\ value\\=\"(?<content>.*?)\"\\ \\/\\>\\<\\/h1\\>{p}";
            this.txt_ini += "http://whatismyipaddress.com/{p}";
            this.txt_ini += "\\<a\\ href\\=\"\\/ip\\/(?<content>.*?)\"\\>\\<img\\ src\\=\"{p}";
            this.txt_ini += "http://www.myip.ru/{p}";
            this.txt_ini += "\\<TD\\ bgcolor\\=white\\ align\\=center\\ valign\\=middle\\>(?<content>.*?)\\<\\/TD\\>{p}";

            // The help strings should not be used, as ip4 provides its own
            this.txt_help = "";
            this.txt_help += "Program provides external (public) IP for current computer. External IP is {p}";
            this.txt_help += "acquired using public web pages that provide current IP address. {p}";
            this.txt_help += "{p}";
            this.txt_help += "Usage: program returns current public IP address. {p}";
            this.txt_help += "{p}";
            this.txt_help += "Options:{p}";
            this.txt_help += "  -q    quiet, returns only IP address. {p}";
            this.txt_help += "  -c    create new ini file with default values.{p}";
            this.txt_help += "  -o    order list of public web pages based on speed for acquiring IP address.{p}";
            this.txt_help += "  -??   or   -hh   additional commands and help. {p}";
            this.txt_help += "  -?    or   -h    help. {p}";
            this.txt_help += "Switch -o is recommended at first use. This requires some time but usually {p}";
            this.txt_help += "less than minute. Command creates ini file with proper order of URLs. See ini {p}";
            this.txt_help += "file contents for details and examples.{p}";

            // The extended help strings are included in the ini file, so have been rewritten for ip4
            this.txt_helpExtended = "";
            this.txt_helpExtended += "Program provides external (public) IP for current computer. External IP is {p}";
            this.txt_helpExtended += "acquired using public web pages that provide IP. Information is parsed {p}";
            this.txt_helpExtended += "utilizing internal data and/or data from ini file. For details see content {p}";
            this.txt_helpExtended += "of ini file at same location as this executable. {p}";
            this.txt_helpExtended += "{p}";
            this.txt_helpExtended += "Usage: program returns current public IP address. {p}";
            this.txt_helpExtended += "{p}";
            this.txt_helpExtended += "Options:{p}";
            this.txt_helpExtended += "  -c  create new ini file with default values (overrides old one if exists).{p}";
            this.txt_helpExtended += "  -o  order list of public web pages based on speed for acquiring IP  address.{p}";
            this.txt_helpExtended += "  -q  quiet, returns only ip address. {p}";
            this.txt_helpExtended += "  -v  verbose. Reply with detailed info.{p}";
            this.txt_helpExtended += "  -2  program acquires IP address from two different sources.{p}";
            this.txt_helpExtended += "  -a  all. Verbose. Process all pages using default order.{p}";
            this.txt_helpExtended += "  -p  shows positioned URLs for web pages with external IP and parsing info. {p}";
            this.txt_helpExtended += "  -x  program executes <exeFile> when current IP is also included in list of {p}";
            this.txt_helpExtended += "      IP addresses <ipList>.{p}";
            this.txt_helpExtended += "  -k  keep URL. Save URL using -w:<webpage> prameter and regular expression {p}";
            this.txt_helpExtended += "      using -s:<regularExpression> to list of URLs in ini file.{p}";
            this.txt_helpExtended += "  -l  executes program <exeFile> when external IP changes, first parameter {p}";
            this.txt_helpExtended += "      of execution is new IP address.{p}";
            this.txt_helpExtended += "  -??  or  -hh    additional help. {p}";
            this.txt_helpExtended += "  -?  or   -h    help. {p}";
            this.txt_helpExtended += "  -e:<exeFile>              path to exe file. Works with -x or -l switch.{p}";
            this.txt_helpExtended += "  -i:<ipList>               comma separated ipList (without spaces).{p}";
            this.txt_helpExtended += "  -w:<webPage>              web page url. Works with -s switch.{p}";
            this.txt_helpExtended += "  -t:<miliseconds>          download timeout. Default value is 10000.{p}";
            this.txt_helpExtended += "  -s:<regularExpression>    regular expression. Works with -w switch.{p}";
            this.txt_helpExtended += "  -r:<count>[,<seconds>]    use -r:0 or just -r for infinite repetition.{p}";
            this.txt_helpExtended += "  -p:<positionIndex>        acquire IP using url at specific position (for {p}";
            this.txt_helpExtended += "                            random page use 0).{p}";
            this.txt_helpExtended += "Each command that writes to ini file requires permission for writing at same {p}";
            this.txt_helpExtended += "location as this executable. Functionality of all switches are described in {p}";
            this.txt_helpExtended += "details in ini file.{p}";
            this.txt_helpExtended += "{p}DOWNLOAD SPEED{p}";
            this.txt_helpExtended += "At first use switch -o is recommended because web pages load with different {p}";
            this.txt_helpExtended += "speeds from different locations (globally). This command will optimize order {p}";
            this.txt_helpExtended += "of URLs used for acquiring IP address from your location and also optionally {p}";
            this.txt_helpExtended += "eliminates (comment) URLs for pages that are not valid any more. Command {p}";
            this.txt_helpExtended += "requires some time to measure average accessing time for each page. {p}";
            this.txt_helpExtended += "Command also adds IP address where is possible.{p}";
            this.txt_helpExtended += "{p}EXTERNAL IP CHANGE{p}";
            this.txt_helpExtended += "When dynamic IP is used then sometimes some action is required. Program can {p}";
            this.txt_helpExtended += "execute specific exe file <exeFile> (defined in ini file or as parameter) {p}";
            this.txt_helpExtended += "each time when external IP is changed. Programs can also wait for change with {p}";
            this.txt_helpExtended += "-r switch. Default value for each repetition is 60 miliseconds (owners of web {p}";
            this.txt_helpExtended += "pages do not like too much traffic, especially if this is automated), but {p}";
            this.txt_helpExtended += "this can be changed, example for 30 seconds: -r:0,30. {p}";
            this.txt_helpExtended += "{p}ANONYMIZE REAL IP ADDRESS{p}";
            this.txt_helpExtended += "When real IP should be hidden (using virtual machine with TOR, for example) {p}";
            this.txt_helpExtended += "then -x switch is used. Program compares your public IP with provided list {p}";
            this.txt_helpExtended += "of IP addresses <ipList>. If any address match then specific program <exeFile> {p}";
            this.txt_helpExtended += "is executed. When proxy is used for anonymizing, some pages that provide {p}";
            this.txt_helpExtended += "public IP show your IP address although connection through proxy but due {p}";
            this.txt_helpExtended += "to proxy settings and/or role real IP address is not necessary hidden. In {p}";
            this.txt_helpExtended += "this case some pages (whatsmyip.net for example) show your real IP if {p}";
            this.txt_helpExtended += "available. Even if real IP is hidden some proxies keep log files which {p}";
            this.txt_helpExtended += "can reveal your real address. {p}";
            this.txt_helpExtended += "{p}ADD OR CHANGE SITES (with testing) {p}";
            this.txt_helpExtended += "Information with external IP is available in public sites. Program {p}";
            this.txt_helpExtended += "downloads site and parse appropriates string. Sites can be add or {p}";
            this.txt_helpExtended += "changed in ini file. Correct parsing can be tested using these switches:{p}";
            this.txt_helpExtended += "-w and -s switches always should be used together. Both are for testing {p}";
            this.txt_helpExtended += "parsed IP address from specific web page using regular expression. Parser {p}";
            this.txt_helpExtended += "uses .net dialect with word 'content' as matching group. When both {p}";
            this.txt_helpExtended += "parameters are provided then program returns parsed contents (this should {p}";
            this.txt_helpExtended += "be IP address only) with time required for operation. Both parameters, url {p}";
            this.txt_helpExtended += "and regular expression are writen to URLs list in ini file with -k switch. {p}";
            this.txt_helpExtended += "{p}NOTES{p}";
            this.txt_helpExtended += "When parameter value contains spaces then put whole string within quotation. {p}";
            this.txt_helpExtended += "When quotation is needed inside such parameter then double existing quotation.{p}";
            this.txt_helpExtended += "Exmple: \"-w:http://mywebpage.com/sp ace\" {p}";
            this.txt_helpExtended += "Program can be used as .net library also. Root namespace is indoo. {p}";
            this.txt_helpExtended += "See ini file contents for details and examples.{p}";
        }

        public void processIP_async(bool save_SkipIPLookupArg, bool skipIPLookupArg, string[] args) {

            Thread thread = new Thread(
                (ThreadStart)delegate {

                // Perform the threaded operation using a seperate externalIP
                // instance to ensure there is only ever one thread acting on 
                // a class instance.
                externalIP workerExternalIP = new externalIP();

                workerExternalIP.initValues();
                workerExternalIP.setArguments(args);
                if (save_SkipIPLookupArg) {
                    // Inject this new value directly (immediately after the call 
                    // to setArguments()), rather than do it by adding argument 
                    // parsing to externalIP for skipIPLookup.
                    workerExternalIP.skipIPLookup = skipIPLookupArg;
                }

                workerExternalIP.setVariables();
                workerExternalIP.fillIniFileParameters(args);

                if (save_SkipIPLookupArg) {
                    workerExternalIP.configValue(workerExternalIP.paramSkipIP, skipIPLookupArg ? "true" : "false");
                }


                // workerExternalIP.skipIPLookup will now have been set from the ini file value
                // if save_SkipIPLookupArg didn't cause us to already inject a value into it.
                // (if we did inject a value then it will have been saved to the ini file)
                bool alwaysSkipIPLookup = workerExternalIP.skipIPLookup == true;
                bool skipIPLookup = skipIPLookupArg || alwaysSkipIPLookup;

                string ipResult = null;
                if (!skipIPLookup) {
                    ipResult = workerExternalIP.processIP();
                }

                LookupEventArgs lookupResult = new LookupEventArgs(
                    ipResult,
                    String.IsNullOrEmpty(ipResult) && workerExternalIP.isTimeOut, // the way externalIP.getPage() is written, any failure to get iniFileText from a website is considered a timeout, and isTimeOut only gives the result of the last attempt.
                    skipIPLookup,
                    alwaysSkipIPLookup
                );

                if (OperationComplete != null) {
                    OperationComplete(this, lookupResult);
                }
            }
            );

            thread.Start();
        }


        /// <summary>
        /// Invoked on a pool thread when the external address is found, or
        /// skipped, or timed out.
        /// </summary>
        public EventHandler<LookupEventArgs> OperationComplete;

	}
}
