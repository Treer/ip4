using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
namespace indoo.tools
{
	public class externalIP
	{
		public enum modes
		{
			returnExternalIP,
			runExeWhenExternalIPmatch,
			createIniFile,
			showHelpExtended,
			addIPsToIni,
			showHelp,
			orderURLs,
			runExeWhenExternalIpChange,
			saveToUrlList,
			showUrlList
		}
		private struct order
		{
			public string urlLine;
			public string regexLine;
			public string regexLineTrim;
			public string urlLineTrim;
			public int orgIdxUrl;
			public int orgIdxRegex;
			public double[] time;
			public double avgTime;
			public string ipAddress;
		}
		private class getPage2Thread
		{
			public string url;
			public int timeOut;
			public bool isCachePreventAborted;
			public string urlUniqueAdd;
			public string result;
			[DebuggerNonUserCode]
			public getPage2Thread()
			{
			}
			public string getPage()
			{
				string text;
				try
				{
					externalIP.myWebClient myWebClient = new externalIP.myWebClient(this.timeOut);
					if (!this.isCachePreventAborted)
					{
						try
						{
							myWebClient.Headers.Add("lastCached", DateTime.Now.AddYears(-1).ToString());
							myWebClient.Headers.Add("Expires", DateTime.Now.AddYears(-1).ToString());
							myWebClient.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate");
							myWebClient.Headers.Add("Cache-Control", "post-check=0, pre-check=0");
							myWebClient.Headers.Add("Pragma", "no-cache");
							myWebClient.Headers.Add("lastCached", DateTime.Now.AddDays(-1.0).ToString());
						}
						catch (Exception expr_D8)
						{
							ProjectData.SetProjectError(expr_D8);
							ProjectData.ClearProjectError();
						}
					}
					byte[] bytes = myWebClient.DownloadData(this.url + this.urlUniqueAdd);
					string @string = Encoding.UTF8.GetString(bytes);
					myWebClient.Dispose();
					myWebClient = null;
					this.result = @string;
					text = @string;
				}
				catch (Exception expr_124)
				{
					ProjectData.SetProjectError(expr_124);
					text = "";
					ProjectData.ClearProjectError();
				}
				return text;
			}
		}
		private class getPage3Thread
		{
			public string url;
			public int timeOut;
			public bool isCachePreventAborted;
			public string urlUniqueAdd;
			public string result;
			[DebuggerNonUserCode]
			public getPage3Thread()
			{
			}
			public string getPage()
			{
				string text2;
				try
				{
					byte[] bytes = Encoding.ASCII.GetBytes("test");
					HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this.url + this.urlUniqueAdd);
					httpWebRequest.ProtocolVersion = HttpVersion.Version10;
					httpWebRequest.KeepAlive = false;
					httpWebRequest.Method = "POST";
					httpWebRequest.Headers.Add("lastCached", DateTime.Now.ToString());
					if (!this.isCachePreventAborted)
					{
						try
						{
							httpWebRequest.Headers.Add("lastCached", DateTime.Now.AddYears(-1).ToString());
							httpWebRequest.Headers.Add("Expires", DateTime.Now.AddYears(-1).ToString());
							httpWebRequest.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate");
							httpWebRequest.Headers.Add("Cache-Control", "post-check=0, pre-check=0");
							httpWebRequest.Headers.Add("Pragma", "no-cache");
							httpWebRequest.Headers.Add("lastCached", DateTime.Now.AddDays(-1.0).ToString());
						}
						catch (Exception expr_13E)
						{
							ProjectData.SetProjectError(expr_13E);
							ProjectData.ClearProjectError();
						}
					}
					httpWebRequest.ContentType = "text/xml;charset=\\\"utf-8\\\"";
					Stream requestStream = httpWebRequest.GetRequestStream();
					requestStream.Write(bytes, 0, 1);
					requestStream.Flush();
					requestStream.Close();
					WebResponse response = httpWebRequest.GetResponse();
					StreamReader streamReader = new StreamReader(response.GetResponseStream());
					string text = streamReader.ReadToEnd();
					streamReader.Close();
					this.result = text;
					text2 = text;
				}
				catch (Exception expr_1A8)
				{
					ProjectData.SetProjectError(expr_1A8);
					text2 = "";
					ProjectData.ClearProjectError();
				}
				return text2;
			}
		}
		private class myWebClient : WebClient
		{
			private int _TimeoutMS;
			private WebRequest w;
			public int setTimeout
			{
				set
				{
					this._TimeoutMS = value;
				}
			}
			public myWebClient()
			{
				this._TimeoutMS = 0;
			}
			public myWebClient(int TimeoutMS)
			{
				this._TimeoutMS = 0;
				this._TimeoutMS = TimeoutMS;
			}
			private static void TimeoutCallback(object state, bool timedOut)
			{
				if (timedOut)
				{
					HttpWebRequest httpWebRequest = (HttpWebRequest)state;
				}
			}
			private static void RespCallback(IAsyncResult asynchronousResult)
			{
			}
			protected override WebRequest GetWebRequest(Uri address)
			{
				this.w = base.GetWebRequest(address);
				if (this._TimeoutMS != 0)
				{
					this.w.Timeout = this._TimeoutMS;
				}
				return this.w;
			}
		}
		[CompilerGenerated]
		internal class _Closure$__1
		{
			public externalIP.getPage2Thread $VB$Local_VB$t_ref$S6;
			[DebuggerNonUserCode]
			public _Closure$__1()
			{
			}
			[DebuggerStepThrough, CompilerGenerated]
			public void _Lambda$__2()
			{
				this.$VB$Local_VB$t_ref$S6.getPage();
			}
		}
		[CompilerGenerated]
		internal class _Closure$__2
		{
			public externalIP.getPage3Thread $VB$Local_VB$t_ref$S7;
			[DebuggerNonUserCode]
			public _Closure$__2()
			{
			}
			[DebuggerStepThrough, CompilerGenerated]
			public void _Lambda$__3()
			{
				this.$VB$Local_VB$t_ref$S7.getPage();
			}
		}
		private List<string> urls;
		private List<string> ips;
		private List<string> regExs;
		public string iPList;
		public string exeFile;
		private string paramIPs;
		private string paramFile;
		private string paramLastIP;
		private string missingParams;
		private string httpStart;
		private string httpStart2;
		private string regexGroupname;
		public string myIniFile;
		private bool isPublic;
		public bool isIpFromTwoSources;
		private bool exitApp;
		private bool isForceNewIps;
		private bool isUrlSave;
		public bool isQuiet;
		private bool isWait;
		public int timeOut;
		public string paramUrl;
		public string paramRegex;
		public bool isVerbose;
		private DateTime timeStart;
		private DateTime timeStop;
		private int positionIndex;
		public int repeatNumber;
		public int repeatPause;
		public bool isAllURLs;
		private externalIP.modes _mode;
		private bool isWarnWritten;
		public string urlResult;
		public string urlResult2;
		private string lastIp;
		private bool isCachePreventAborted;
		private int x;
		private int y;
		private bool isTimeOut;
		private string timeoutUrl;
		private bool addNewLine;
		private bool writeToOrderOnly;
		private List<externalIP.order> urlOrder;
		public bool isPOSTMethod;
		public bool isWritenToConsole;
		public string output;
		public string[] arguments;
		public string txtP;
		public string txtC;
		public string txt_result;
		public string txt_resultVerbose;
		public string txt_copyr;
		public string txt_errDesc;
		public string txt_time1;
		public string txt_invalid1;
		public string txt_ipBracket;
		public string txt_dlFail;
		public string txt_err1;
		public string txt_err2;
		public string txt_err3;
		public string txt_err4;
		public string txt_err5;
		public string txt_err6;
		public string txt_err7;
		public string txt_err8;
		public string txt_err9;
		public string txt_err10;
		public string txt_infoRegex;
		public string txt_info1;
		public string txt_info2;
		public string txt_helpExtended;
		public string txt_help;
		public string txt_site;
		public string txt_ini;
		public string txt_timeoutVerbose;
		public string txt_timeout;
		public externalIP.modes mode
		{
			get
			{
				return this._mode;
			}
			set
			{
				this._mode = value;
			}
		}
		private void waitOnExit(object sender, EventArgs e)
		{
			Debugger.Break();
		}
		private void setText()
		{
			this.txtP = "{p}";
			this.txtC = "' ";
			this.txt_site = "Site: ";
			this.txt_time1 = "  time: ";
			this.txt_invalid1 = "  - INVALID SITE AND/OR SETTINGS, URL IS DISABLED";
			this.txt_ipBracket = " ({0})";
			this.txt_result = "Computer's external IP: {0}";
			this.txt_dlFail = "Download failed: {0}";
			this.txt_resultVerbose = "External IP={0}, time={1}ms, url={2}";
			this.txt_timeoutVerbose = "Timeout, time>{0}ms, url={1}";
			this.txt_timeout = "Timeout > {0}ms.";
			this.txt_copyr = "{p}OuterIP v{0} - Acquires external IP from public web pages.{p}Copyright (c) 2013 primoz@licen.net{p}";
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
			this.txt_err4 = "{p}ERROR!!!{p}{p}Both parameters -w and -s should be used at same time.{p} URL should starts with http. See help /? for details.{p}";
			this.txt_err5 = "{p}ERROR!!!{p}{p}Cannot acquire webpage:{p}{0}{p}";
			this.txt_err6 = "{p}ERROR!!!{p}{p}Cannot create regular expresion object. Wrong regex syntax. Error description:{p}{0}{p}";
			this.txt_err7 = "{p}ERROR!!!{p}{p}Cannot create regular expresion match object. Wrong syntax. Error description:{p}{0}{p}";
			this.txt_err8 = "{p}ERROR!!!{p}{p}Regular expression error. See description for details:{p}{0}{p}";
			this.txt_err9 = "Regular expression cannot match any string. Regular expression string is:{p}{0}{p}Content of downloaded web page is:{p}{1}{p}";
			this.txt_err10 = "{p}ERROR!!!{p}{p}No valid file <exeFile> for execution{p}";
			this.txt_err10 = "{p}ERROR!!!{p}{p}No valid URL index.{p}";
			this.txt_infoRegex = "Regular expression successfully parsees this text:{p}{2}{p}from folowing web page in {1} seconds:{p}{0}{p}";
			this.txt_info1 = "New ini file is written to:{p}    {0}{p}{p}See file contents for future instruction and change data if necessary.{p}";
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
			this.txt_ini += "lastIP = 127.0.0.1{p}";
			this.txt_ini += "checkIPs = 127.0.0.1{p}";
			this.txt_ini += "runFile = c:\\something.bat{p}";
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
		}
		private void setTextVars()
		{
			string text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			text = Strings.Left(text, Strings.InStrRev(text, ".", -1, CompareMethod.Binary));
			this.txt_copyr = Strings.Replace(this.txt_copyr, "{0}", text, 1, -1, CompareMethod.Binary);
		}
		public void setArguments(string[] args)
		{
			bool flag = false;
			int arg_0A_0 = 0;
			checked
			{
				int num = args.Length - 1;
				for (int i = arg_0A_0; i <= num; i++)
				{
					string text = Strings.Trim(this.cString(args[i], false));
					if (text.StartsWith("-i:", StringComparison.InvariantCultureIgnoreCase) & Strings.Len(text) > 3)
					{
						this.iPList = Strings.Mid(text, 4);
					}
					else
					{
						if (text.StartsWith("-e:", StringComparison.InvariantCultureIgnoreCase) & Strings.Len(text) > 3)
						{
							this.exeFile = Strings.Mid(text, 4);
						}
						else
						{
							if (text.StartsWith("-w:", StringComparison.InvariantCultureIgnoreCase) & Strings.Len(text) > 3)
							{
								this.paramUrl = Strings.Mid(text, 4);
							}
							else
							{
								if (text.StartsWith("-s:", StringComparison.InvariantCultureIgnoreCase) & Strings.Len(text) > 3)
								{
									this.paramRegex = Strings.Mid(text, 4);
								}
								else
								{
									if (text.StartsWith("-p:", StringComparison.InvariantCultureIgnoreCase) & Strings.Len(text) > 3)
									{
										try
										{
											this.positionIndex = Conversions.ToInteger(Strings.Mid(text, 4));
											if (Operators.CompareString(Strings.Mid(text, 4), "0", false) == 0)
											{
												this.positionIndex = 0;
											}
											goto IL_49C;
										}
										catch (Exception expr_113)
										{
											ProjectData.SetProjectError(expr_113);
											ProjectData.ClearProjectError();
											goto IL_49C;
										}
									}
									if (text.StartsWith("-r:", StringComparison.InvariantCultureIgnoreCase) & Strings.Len(text) > 3)
									{
										string text2 = Strings.Mid(text, 4);
										try
										{
											this.repeatNumber = Conversions.ToInteger(Strings.Split(text2, ",", -1, CompareMethod.Binary)[0]);
											if (this.repeatNumber < 1)
											{
												this.repeatNumber = 999999999;
											}
										}
										catch (Exception expr_179)
										{
											ProjectData.SetProjectError(expr_179);
											ProjectData.ClearProjectError();
										}
										if (text2.Contains(","))
										{
											try
											{
												this.repeatPause = Conversions.ToInteger(Strings.Split(text2, ",", -1, CompareMethod.Binary)[1]);
											}
											catch (Exception expr_1B3)
											{
												ProjectData.SetProjectError(expr_1B3);
												ProjectData.ClearProjectError();
											}
											if (this.repeatPause < 0)
											{
												this.repeatPause = 0;
											}
										}
										if (this.repeatNumber < 1)
										{
											this.repeatNumber = 1;
										}
									}
									else
									{
										if (text.StartsWith("-t:", StringComparison.InvariantCultureIgnoreCase) & Strings.Len(text) > 3)
										{
											int num2 = 0;
											try
											{
												num2 = Conversions.ToInteger(Strings.Mid(text, 4));
											}
											catch (Exception expr_215)
											{
												ProjectData.SetProjectError(expr_215);
												ProjectData.ClearProjectError();
											}
											if (num2 > 0 & num2 < 999999)
											{
												this.timeOut = num2;
											}
										}
										else
										{
											if (Operators.CompareString(Strings.LCase(text), "-h", false) == 0 | Operators.CompareString(Strings.LCase(text), "-?", false) == 0)
											{
												this.mode = externalIP.modes.showHelp;
											}
											else
											{
												if (Operators.CompareString(Strings.LCase(text), "-hh", false) == 0 | Operators.CompareString(Strings.LCase(text), "-??", false) == 0)
												{
													this.mode = externalIP.modes.showHelpExtended;
												}
												else
												{
													if (Operators.CompareString(Strings.LCase(text), "-x", false) == 0)
													{
														this.mode = externalIP.modes.runExeWhenExternalIPmatch;
													}
													else
													{
														if (Operators.CompareString(Strings.LCase(text), "-2", false) == 0)
														{
															this.isIpFromTwoSources = true;
														}
														else
														{
															if (Operators.CompareString(Strings.LCase(text), "-c", false) == 0)
															{
																this.mode = externalIP.modes.createIniFile;
															}
															else
															{
																if (Operators.CompareString(Strings.LCase(text), "-f", false) == 0)
																{
																	this.mode = externalIP.modes.addIPsToIni;
																}
																else
																{
																	if (Operators.CompareString(Strings.LCase(text), "-o", false) == 0)
																	{
																		this.mode = externalIP.modes.orderURLs;
																	}
																	else
																	{
																		if (Operators.CompareString(Strings.LCase(text), "-p", false) == 0)
																		{
																			this.mode = externalIP.modes.showUrlList;
																		}
																		else
																		{
																			if (Operators.CompareString(Strings.LCase(text), "-m", false) == 0)
																			{
																				this.isPOSTMethod = true;
																			}
																			else
																			{
																				if (Operators.CompareString(Strings.LCase(text), "-q", false) == 0)
																				{
																					this.isQuiet = true;
																				}
																				else
																				{
																					if (Operators.CompareString(Strings.LCase(text), "-k", false) == 0)
																					{
																						this.mode = externalIP.modes.saveToUrlList;
																					}
																					else
																					{
																						if (Operators.CompareString(Strings.LCase(text), "-d", false) == 0)
																						{
																							this.isWait = true;
																						}
																						else
																						{
																							if (Operators.CompareString(Strings.LCase(text), "-v", false) == 0)
																							{
																								this.isVerbose = true;
																							}
																							else
																							{
																								if (Operators.CompareString(Strings.LCase(text), "-l", false) == 0)
																								{
																									this.mode = externalIP.modes.runExeWhenExternalIpChange;
																								}
																								else
																								{
																									if (Operators.CompareString(Strings.LCase(text), "-r", false) == 0)
																									{
																										this.repeatNumber = 99999999;
																									}
																									else
																									{
																										if (Operators.CompareString(Strings.LCase(text), "-n", false) == 0)
																										{
																											this.isCachePreventAborted = true;
																										}
																										else
																										{
																											if (Operators.CompareString(Strings.LCase(text), "-a", false) == 0)
																											{
																												this.isVerbose = true;
																												this.isAllURLs = true;
																												this.isIpFromTwoSources = false;
																											}
																											else
																											{
																												flag = true;
																											}
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
					IL_49C:;
				}
				if (flag & this.mode != externalIP.modes.showHelpExtended)
				{
					this.mode = externalIP.modes.showHelp;
				}
			}
		}
		private void consoleWrite(string Text)
		{
			if (this.isWritenToConsole)
			{
				Console.Write(Text);
			}
			else
			{
				this.output += Text;
			}
		}
		private void consoleWriteLine(string Text)
		{
			if (this.isWritenToConsole)
			{
				Console.WriteLine(Text);
			}
			else
			{
				this.output = this.output + Text + "\r\n";
			}
		}
		public void setVariables()
		{
			this.setText();
			this.setTextVars();
			if (this.isWait)
			{
				AppDomain.CurrentDomain.ProcessExit += new EventHandler(this.waitOnExit);
			}
			if (!this.isQuiet)
			{
				this.consoleWriteLine(this.changeP(this.txt_copyr));
			}
		}
		private void setDefaultValues()
		{
			this.urls = new List<string>();
			this.ips = new List<string>();
			this.regExs = new List<string>();
			this.iPList = "";
			this.exeFile = "";
			this.paramIPs = "checkIPs";
			this.paramFile = "runFile";
			this.paramLastIP = "lastIP";
			this.httpStart = "http";
			this.httpStart2 = "http://";
			this.regexGroupname = "content";
			this.isPublic = false;
			this.isIpFromTwoSources = false;
			this.exitApp = false;
			this.isForceNewIps = false;
			this.isQuiet = false;
			this.isWait = false;
			this.timeOut = 10000;
			this.paramUrl = "";
			this.paramRegex = "";
			this.isVerbose = false;
			this.positionIndex = -1;
			this.repeatNumber = 1;
			this.repeatPause = 60;
			this.isAllURLs = false;
			this.mode = externalIP.modes.returnExternalIP;
			this.urlResult = "";
			this.urlResult2 = "";
			this.lastIp = "";
			this.isCachePreventAborted = false;
			this.isWritenToConsole = true;
			this.output = "";
		}
		private void exec()
		{
			checked
			{
				if (!this.checkExeFile2())
				{
					this.exitApp = true;
				}
				else
				{
					int arg_1C_0 = 1;
					int num = this.repeatNumber;
					for (int i = arg_1C_0; i <= num; i++)
					{
						if (this.repeatNumber > 1 & !this.isAllURLs)
						{
							this.y++;
							this.consoleWrite(Conversions.ToString(this.y) + ". ");
						}
						if ((!this.isEmpty(this.paramUrl) | !this.isEmpty(this.paramRegex)) & this.mode != externalIP.modes.saveToUrlList)
						{
							this.testWebpage();
							this.exitApp = true;
						}
						else
						{
							switch (this.mode)
							{
							case externalIP.modes.returnExternalIP:
							{
								string text = this.processIP();
								if (!this.isAllURLs)
								{
									if (!this.isEmpty(text))
									{
										this.writeIpAddress(text);
										this.exitApp = true;
									}
									else
									{
										if (this.isTimeOut)
										{
											this.writeTimeOut();
										}
									}
								}
								else
								{
									this.exitApp = true;
								}
								break;
							}
							case externalIP.modes.runExeWhenExternalIPmatch:
								if (!this.checkExeParams())
								{
									this.exitApp = true;
								}
								else
								{
									this.processIPinfoExe();
									this.ifRunExe();
									this.addIPsToIniFile();
								}
								break;
							case externalIP.modes.createIniFile:
								this.createIni();
								break;
							case externalIP.modes.showHelpExtended:
								this.showHelpExtended();
								this.exitApp = true;
								break;
							case externalIP.modes.addIPsToIni:
								if (!File.Exists(this.myIniFile))
								{
									this.createIni();
								}
								this.isForceNewIps = true;
								this.addIPsToIniFile();
								break;
							case externalIP.modes.showHelp:
								this.showHelp();
								this.exitApp = true;
								break;
							case externalIP.modes.orderURLs:
								this.setUrlList("", "");
								this.exitApp = true;
								break;
							case externalIP.modes.runExeWhenExternalIpChange:
							{
								string text2 = this.processIP();
								if (!this.isAllURLs)
								{
									if (!this.isEmpty(text2))
									{
										this.writeIpAddress(text2);
										this.exitApp = true;
									}
									else
									{
										if (this.isTimeOut)
										{
											this.writeTimeOut();
										}
									}
								}
								else
								{
									this.exitApp = true;
								}
								break;
							}
							case externalIP.modes.saveToUrlList:
								if (this.testUrlRegexPrameters())
								{
									this.setUrlList(this.paramUrl, this.paramRegex);
								}
								this.exitApp = true;
								break;
							case externalIP.modes.showUrlList:
								this.showUrlList();
								this.exitApp = true;
								break;
							}
						}
						if (this.repeatNumber > 1 & i < this.repeatNumber)
						{
							Thread.Sleep(this.repeatPause * 1000);
						}
					}
				}
				if (this.exitApp)
				{
					return;
				}
				if (this.mode == externalIP.modes.returnExternalIP | this.mode == externalIP.modes.runExeWhenExternalIpChange | (this.mode == externalIP.modes.runExeWhenExternalIPmatch & this.isEmpty(this.lastIp)))
				{
					this.consoleWriteLine(this.changeP(this.txt_err3));
				}
			}
		}
		public void execute(string[] args)
		{
			this.arguments = args;
			this.execute();
		}
		public void execute()
		{
			this.initValues();
			this.setArguments(this.arguments);
			this.setVariables();
			this.fillIniFileParameters(this.arguments);
			this.exec();
		}
		private void writeIpAddress(string ipAddress)
		{
			int num = checked((int)Math.Round(unchecked(DateTime.Now.Subtract(this.timeStart).TotalSeconds * 1000.0)));
			string text = this.urlResult;
			if (!this.isEmpty(this.urlResult2))
			{
				text = this.urlResult2 + ", " + text;
			}
			if (this.isQuiet)
			{
				if (this.addNewLine)
				{
					this.consoleWrite("\r\n");
				}
				this.consoleWrite(ipAddress);
				this.addNewLine = true;
			}
			else
			{
				if (this.isVerbose)
				{
					this.consoleWriteLine(string.Format(this.txt_resultVerbose, ipAddress, num, text));
				}
				else
				{
					this.consoleWriteLine(string.Format(this.txt_result, ipAddress));
				}
			}
		}
		private void writeTimeOut()
		{
			if (!this.isQuiet)
			{
				if (this.isVerbose)
				{
					this.consoleWriteLine(string.Format(this.txt_timeoutVerbose, this.timeOut, this.timeoutUrl));
				}
				else
				{
					this.consoleWriteLine(string.Format(this.txt_timeout, this.timeOut));
				}
			}
		}
		private bool testUrlRegexPrameters()
		{
			if (this.isEmpty(this.paramUrl) | this.isEmpty(this.paramRegex))
			{
				this.consoleWriteLine(this.changeP(this.txt_err4));
				return false;
			}
			return true;
		}
		public string testWebpage()
		{
			string text = "";
			if (this.testUrlRegexPrameters())
			{
				DateTime now = DateTime.Now;
				string text2 = "";
				try
				{
					text2 = this.getPage(this.paramUrl);
				}
				catch (Exception expr_2C)
				{
					ProjectData.SetProjectError(expr_2C);
					ProjectData.ClearProjectError();
				}
				if (!this.isEmpty(text2))
				{
					string text3 = this.cString(text2, false);
					Regex regex;
					string result;
					try
					{
						regex = new Regex(this.paramRegex);
					}
					catch (Exception expr_82)
					{
						ProjectData.SetProjectError(expr_82);
						Exception ex = expr_82;
						this.consoleWriteLine(string.Format(this.changeP(this.txt_err6), ex.Message));
						ProjectData.ClearProjectError();
						return result;
					}
					Match match;
					try
					{
						match = regex.Match(text3);
					}
					catch (Exception expr_BF)
					{
						ProjectData.SetProjectError(expr_BF);
						Exception ex2 = expr_BF;
						this.consoleWriteLine(string.Format(this.changeP(this.txt_err7), ex2.Message));
						ProjectData.ClearProjectError();
						return result;
					}
					try
					{
						if (match.Success)
						{
							string value = match.Groups[this.regexGroupname].Value;
							double totalSeconds = DateTime.Now.Subtract(now).TotalSeconds;
							text = value;
							this.consoleWriteLine(string.Format(this.changeP(this.txt_infoRegex), this.paramUrl, totalSeconds, value));
							result = text;
							return result;
						}
						this.consoleWriteLine(string.Format(this.changeP(this.txt_err9), Strings.Replace(this.paramRegex, "\"", "\"\"", 1, -1, CompareMethod.Binary), text3));
						return result;
					}
					catch (Exception expr_18B)
					{
						ProjectData.SetProjectError(expr_18B);
						Exception ex3 = expr_18B;
						this.consoleWriteLine(string.Format(this.changeP(this.txt_err8), ex3.Message));
						ProjectData.ClearProjectError();
						return result;
					}
					return text;
				}
				this.consoleWriteLine(string.Format(this.changeP(this.txt_err5), this.paramUrl));
			}
			return text;
		}
		private bool checkExeParams()
		{
			this.checkExeFile();
			if (this.isEmpty(this.exeFile) | this.isEmpty(this.iPList) | this.urls.Count < 1)
			{
				this.writeParamErr();
				return false;
			}
			return true;
		}
		private bool checkExeFile2()
		{
			this.checkExeFile();
			if (this.mode == externalIP.modes.runExeWhenExternalIpChange & this.isEmpty(this.exeFile))
			{
				string text = this.changeP(this.txt_err10);
				this.consoleWrite(text);
				return false;
			}
			return true;
		}
		private string readFileText(string FullPath)
		{
			StreamReader streamReader = new StreamReader(FullPath, true);
			string result = streamReader.ReadToEnd();
			streamReader.Close();
			return result;
		}
		private void writeFileContent(string filename, string content, int codepage)
		{
			Encoding encoding;
			if (codepage == 0)
			{
				encoding = Encoding.GetEncoding(Encoding.Default.WindowsCodePage);
			}
			else
			{
				encoding = Encoding.GetEncoding(codepage);
			}
			StreamWriter streamWriter = new StreamWriter(filename, false, encoding);
			streamWriter.Write(content);
			streamWriter.Close();
		}
		public string configValue(string name, string value)
		{
			string text = null;
			name = Strings.Trim(Strings.LCase(this.cString(name, false)));
			string text2 = "";
			if (!this.setIniFile(ref text2))
			{
				return "";
			}
			text2 = this.setLineEndToCr(text2);
			string[] array = Strings.Split(text2, "\r", -1, CompareMethod.Binary);
			string text3 = "";
			int arg_51_0 = 0;
			checked
			{
				int num = array.Length - 1;
				for (int i = arg_51_0; i <= num; i++)
				{
					string text4 = Strings.Trim(this.cString(array[i], false));
					bool flag = false;
					if (text4.StartsWith(name, StringComparison.InvariantCultureIgnoreCase))
					{
						int num2 = Strings.InStr(text4, "=", CompareMethod.Binary);
						if (Operators.CompareString(Strings.LCase(Strings.Trim(Strings.Left(text4, num2 - 1))), name, false) == 0 && Strings.Len(text4) > num2 + 1)
						{
							text = Strings.Trim(Strings.Mid(text4, num2 + 1));
							flag = true;
						}
					}
					if (flag & !Information.IsNothing(value))
					{
						text3 = string.Concat(new string[]
						{
							text3,
							name,
							" = ",
							value,
							"\r\n"
						});
					}
					else
					{
						text3 = text3 + array[i] + "\r\n";
					}
				}
				while (text3.EndsWith("\r\n\r\n"))
				{
					text3 = Strings.Left(text3, Strings.Len(text3) - Strings.Len("\r\n"));
				}
				if (!Information.IsNothing(value))
				{
					if (Information.IsNothing(text))
					{
						text3 = string.Concat(new string[]
						{
							text3,
							name,
							" = ",
							value,
							"\r\n"
						});
					}
					this.writeFile(this.myIniFile, text3);
				}
				return this.cString(text, false);
			}
		}
		private string changeP(string text)
		{
			return Strings.Replace(text, this.txtP, "\r\n", 1, -1, CompareMethod.Binary);
		}
		private bool writeFile(string path, string text)
		{
			try
			{
				text = this.changeP(text);
				this.writeFileContent(path, text, 850);
			}
			catch (Exception expr_18)
			{
				ProjectData.SetProjectError(expr_18);
				if (!this.isWarnWritten)
				{
					this.consoleWrite(string.Format(this.changeP(this.txt_err1) + "\r\n", this.myIniFile));
					this.isWarnWritten = true;
				}
				ProjectData.ClearProjectError();
			}
			return !this.isWarnWritten;
		}
		private void fillIniFileParameters(string[] args)
		{
			string text = "";
			try
			{
				if (File.Exists(this.myIniFile))
				{
					text = this.readFileText(this.myIniFile);
				}
			}
			catch (Exception expr_22)
			{
				ProjectData.SetProjectError(expr_22);
				ProjectData.ClearProjectError();
			}
			if (this.isEmpty(text))
			{
				text = this.getDefaultIniFileText();
			}
			text = this.setLineEndToCr(text);
			string[] array = Strings.Split(text, "\r", -1, CompareMethod.Binary);
			int arg_5E_0 = 0;
			checked
			{
				int num = array.Length - 1;
				for (int i = arg_5E_0; i <= num; i++)
				{
					array[i] = Strings.Trim(this.cString(array[i], false));
					if (array[i].StartsWith("'") | array[i].StartsWith("#"))
					{
						array[i] = "";
					}
					if (this.isEmpty(this.iPList))
					{
						array[i] = this.changeParamStart(array[i], this.paramIPs);
						this.fillParam(this.paramIPs, array[i], ref this.iPList);
					}
					if (this.isEmpty(this.exeFile))
					{
						array[i] = this.changeParamStart(array[i], this.paramFile);
						this.fillParam(this.paramFile, array[i], ref this.exeFile);
					}
				}
				int arg_119_0 = 0;
				int num2 = array.Length - 1;
				for (int j = arg_119_0; j <= num2; j++)
				{
					if ((array[j].StartsWith(this.httpStart) & array.Length > j) && Conversions.ToDouble(Strings.Trim(Conversions.ToString(Strings.Len(array[j + 1])))) > 1.0)
					{
						string text2 = this.cString(array[j], false);
						this.urls.Add(Strings.Split(text2, ";", -1, CompareMethod.Binary)[0]);
						if (text2.Contains(";"))
						{
							this.ips.Add(Strings.Split(text2, ";", -1, CompareMethod.Binary)[0]);
						}
						else
						{
							this.ips.Add("");
						}
						this.regExs.Add(array[j + 1]);
						j++;
					}
				}
			}
		}
		private string setLineEndToCr(string file)
		{
			file = Strings.Replace(file, "\r\n", "\r", 1, -1, CompareMethod.Binary);
			file = Strings.Replace(file, "\n", "\r", 1, -1, CompareMethod.Binary);
			file = Strings.Replace(file, "\r\r", "\r", 1, -1, CompareMethod.Binary);
			file = Strings.Replace(file, "\r\r", "\r", 1, -1, CompareMethod.Binary);
			file = Strings.Replace(file, "\r\r", "\r", 1, -1, CompareMethod.Binary);
			return file;
		}
		private bool setIniFile(ref string file)
		{
			if (!File.Exists(this.myIniFile) && !this.writeFile(this.myIniFile, this.getDefaultIniFileText()))
			{
				return false;
			}
			file = this.readFileText(this.myIniFile);
			return this.writeFile(this.myIniFile, file);
		}
		private void setUrlList(string addUrl, string addRegex)
		{
			string text = "";
			bool flag = false;
			if (this.isEmpty(addUrl) | this.isEmpty(addRegex))
			{
				flag = true;
			}
			if (!this.setIniFile(ref text))
			{
				return;
			}
			text = this.setLineEndToCr(text);
			string[] array = Strings.Split(text, "\r", -1, CompareMethod.Binary);
			List<externalIP.order> list = new List<externalIP.order>();
			int num = -1;
			List<string> list2 = new List<string>();
			int arg_57_0 = 0;
			checked
			{
				int num2 = array.Length - 1;
				for (int i = arg_57_0; i <= num2; i++)
				{
					string text2 = array[i];
					if (Strings.LCase(Strings.Trim(this.cString(text2, false))).StartsWith(this.httpStart))
					{
						if (num < 0)
						{
							num = i;
						}
						externalIP.order item = default(externalIP.order);
						item.urlLineTrim = Strings.Split(Strings.LCase(Strings.Trim(this.cString(text2, false))), ";", -1, CompareMethod.Binary)[0];
						item.regexLineTrim = Strings.Trim(this.cString(array[i + 1], false));
						item.urlLine = text2;
						item.orgIdxUrl = i;
						item.orgIdxRegex = i + 1;
						item.regexLine = array[i + 1];
						double[] time = new double[4];
						item.time = time;
						list.Add(item);
					}
					else
					{
						if (num > -1 && Operators.CompareString(list[list.Count - 1].regexLine, text2, false) != 0)
						{
							list2.Add(text2);
						}
					}
				}
				if (this.writeToOrderOnly)
				{
					this.urlOrder = list;
					return;
				}
				if (flag)
				{
					this.measureTimes(ref list);
					list.Sort((externalIP.order x, externalIP.order y) => x.avgTime.CompareTo(y.avgTime));
				}
				string text3 = "";
				if (num > -1)
				{
					int arg_1A2_0 = 0;
					int num3 = num - 1;
					for (int j = arg_1A2_0; j <= num3; j++)
					{
						text3 = text3 + array[j] + "\r\n";
					}
					int arg_1CF_0 = 0;
					int num4 = list.Count - 1;
					for (int k = arg_1CF_0; k <= num4; k++)
					{
						if (list[k].avgTime > 0.0 | !flag)
						{
							text3 += array[list[k].orgIdxUrl];
							if (!this.isEmpty(list[k].ipAddress))
							{
								text3 = text3 + "; " + list[k].ipAddress;
							}
							text3 = text3 + "\r\n" + array[list[k].orgIdxRegex] + "\r\n";
						}
					}
					if (!flag)
					{
						text3 = string.Concat(new string[]
						{
							text3,
							addUrl,
							"\r\n",
							addRegex,
							"\r\n"
						});
					}
					else
					{
						int arg_2CA_0 = 0;
						int num5 = list.Count - 1;
						for (int l = arg_2CA_0; l <= num5; l++)
						{
							if (list[l].avgTime == 0.0 & flag)
							{
								text3 = string.Concat(new string[]
								{
									text3,
									this.txtC,
									array[list[l].orgIdxUrl],
									"\r\n",
									this.txtC,
									array[list[l].orgIdxRegex],
									"\r\n"
								});
							}
						}
					}
					int arg_37A_0 = 0;
					int num6 = list2.Count - 1;
					for (int m = arg_37A_0; m <= num6; m++)
					{
						text3 = text3 + list2[m] + "\r\n";
					}
				}
				this.writeFile(this.myIniFile, text3);
			}
		}
		private void measureTimes(ref List<externalIP.order> ordList)
		{
			this.consoleWriteLine("");
			int arg_17_0 = 0;
			checked
			{
				int num = ordList.Count - 1;
				for (int i = arg_17_0; i <= num; i++)
				{
					externalIP.order value = ordList[i];
					bool flag = false;
					this.consoleWrite(this.txt_site + value.urlLineTrim);
					int num2 = 0;
					do
					{
						string text = "";
						string text2 = "";
						try
						{
							if (!flag)
							{
								flag = true;
								text = this.getIpFromUrl(value.urlLineTrim);
								string input = this.cString(this.getPage(this.httpStart2 + text), false);
								Regex regex = new Regex(value.regexLineTrim);
								Match match = regex.Match(input);
								if (match.Success)
								{
									text2 = Strings.Trim(this.cString(match.Groups[this.regexGroupname].Value, false));
									if (!this.IsAddressValid(text2))
									{
										text2 = "";
									}
								}
							}
						}
						catch (Exception expr_E5)
						{
							ProjectData.SetProjectError(expr_E5);
							ProjectData.ClearProjectError();
						}
						string url = value.urlLineTrim;
						if (!this.isEmpty(value.ipAddress))
						{
							url = this.httpStart2 + value.ipAddress;
						}
						DateTime now = DateTime.Now;
						string input2 = this.cString(this.getPage(url), false);
						Regex regex2 = new Regex(value.regexLineTrim);
						Match match2 = regex2.Match(input2);
						if (match2.Success)
						{
							string text3 = Strings.Trim(this.cString(match2.Groups[this.regexGroupname].Value, false));
							if (this.IsAddressValid(text3))
							{
								if (Operators.CompareString(text3, text2, false) == 0)
								{
									value.ipAddress = text;
								}
								double totalSeconds = DateTime.Now.Subtract(now).TotalSeconds;
								value.time[num2] = totalSeconds;
							}
						}
						else
						{
							this.consoleWriteLine("");
							this.consoleWriteLine(string.Format(this.txt_dlFail, value.urlLineTrim));
						}
						num2++;
					}
					while (num2 <= 3);
					double num3 = 0.0;
					int num4 = 1;
					do
					{
						unchecked
						{
							num3 += value.time[num4];
						}
						num4++;
					}
					while (num4 <= 3);
					if (!this.isEmpty(value.ipAddress))
					{
						this.consoleWrite(string.Format(this.txt_ipBracket, value.ipAddress));
					}
					if (num3 > 0.0)
					{
						value.avgTime = num3 / 4.0;
						this.consoleWriteLine(this.txt_time1 + Conversions.ToString(num3));
					}
					else
					{
						this.consoleWriteLine(this.txt_invalid1);
					}
					ordList[i] = value;
				}
			}
		}
		private void cWrite(string text)
		{
			this.consoleWrite("\r");
			this.consoleWrite(Strings.StrDup(checked(Console.WindowWidth - 1), " "));
			this.consoleWrite("\r");
			this.consoleWrite(text);
		}
		private void addIPsToIniFile()
		{
			checked
			{
				if (File.Exists(this.myIniFile))
				{
					try
					{
						int arg_20_0 = 0;
						int num = this.urls.Count - 1;
						for (int i = arg_20_0; i <= num; i++)
						{
							if (!this.isEmpty(this.urls[i]) & (this.isEmpty(this.ips[i]) | this.isForceNewIps))
							{
								string text = "";
								try
								{
									text = this.getIpFromUrl(this.urls[i]);
								}
								catch (Exception expr_75)
								{
									ProjectData.SetProjectError(expr_75);
									ProjectData.ClearProjectError();
								}
								if (!this.isEmpty(text))
								{
									string text2 = this.readFileText(this.myIniFile);
									string[] array = Strings.Split(text2, "\r\n", -1, CompareMethod.Binary);
									bool flag = false;
									int arg_B7_0 = 0;
									int num2 = array.Length - 1;
									for (int j = arg_B7_0; j <= num2; j++)
									{
										if (Strings.Trim(this.cString(array[j], false)).StartsWith(this.urls[i]))
										{
											array[j] = Strings.Split(array[j], ";", -1, CompareMethod.Binary)[0] + ";" + text;
											flag = true;
										}
									}
									if (flag)
									{
										text2 = Strings.Join(array, "\r\n");
										if (!this.writeFile(this.myIniFile, text2))
										{
											break;
										}
										this.ips[i] = text;
									}
								}
							}
						}
					}
					catch (Exception expr_14E)
					{
						ProjectData.SetProjectError(expr_14E);
						ProjectData.ClearProjectError();
					}
				}
			}
		}
		private void writeParamErr()
		{
			string format = this.changeP(this.txt_err2);
			string text = "";
			if (this.isEmpty(this.exeFile))
			{
				text = text + "- " + Strings.Split(this.missingParams, ";", -1, CompareMethod.Binary)[1] + "\r\n";
			}
			if (this.isEmpty(this.iPList))
			{
				text = text + "- " + Strings.Split(this.missingParams, ";", -1, CompareMethod.Binary)[0] + "\r\n";
			}
			if (this.urls.Count < 1)
			{
				text = text + "- " + Strings.Split(this.missingParams, ";", -1, CompareMethod.Binary)[2] + "\r\n";
			}
			this.consoleWrite(string.Format(format, text));
		}
		private void checkExeFile()
		{
			if (Strings.Len(this.exeFile) > 0 && !File.Exists(this.exeFile))
			{
				this.exeFile = "";
			}
		}
		public void createIni()
		{
			try
			{
				File.Delete(this.myIniFile);
			}
			catch (Exception expr_0D)
			{
				ProjectData.SetProjectError(expr_0D);
				ProjectData.ClearProjectError();
			}
			if (this.writeFile(this.myIniFile, this.getDefaultIniFileText()))
			{
				this.consoleWrite(string.Format(this.changeP(this.txt_info1) + "\r\n", this.myIniFile));
			}
		}
		private void showHelpExtended()
		{
			string text = this.txt_helpExtended;
			text = this.changeP(text);
			this.consoleWriteLine(text);
		}
		private void showUrlList()
		{
			this.writeToOrderOnly = true;
			this.setUrlList(null, null);
			string text = this.txt_info2;
			text = this.changeP(text);
			string text2 = "";
			int arg_33_0 = 0;
			checked
			{
				int num = this.urlOrder.Count - 1;
				for (int i = arg_33_0; i <= num; i++)
				{
					if (i > 0)
					{
						text2 += "\r\n";
					}
					text2 = text2 + Conversions.ToString(i + 1) + ". ";
					text2 = text2 + this.urlOrder[i].urlLineTrim + "\r\n";
					text2 = text2 + Strings.StrDup(Strings.Len((i + 1).ToString()) + 2, " ") + this.urlOrder[i].regexLineTrim;
				}
				text = string.Format(text, text2);
				this.consoleWriteLine(text);
			}
		}
		private void showHelp()
		{
			string text = this.txt_help;
			text = this.changeP(text);
			this.consoleWriteLine(text);
		}
		private string getDefaultIniFileText()
		{
			string text = this.txt_copyr + this.txtP + this.txt_helpExtended;
			text = this.txtC + Strings.Replace(text, this.txtP, this.txtP + this.txtC, 1, -1, CompareMethod.Binary) + this.txtP;
			text += this.txt_ini;
			return this.changeP(text);
		}
		private void initValues()
		{
			this.myIniFile = Assembly.GetExecutingAssembly().Location;
			if (Strings.LCase(this.myIniFile).EndsWith(".exe"))
			{
				this.myIniFile = Strings.Left(this.myIniFile, checked(Strings.Len(this.myIniFile) - 4));
			}
			this.myIniFile += ".ini";
			this.missingParams = this.changeP(this.txt_errDesc);
			if (Information.IsNothing(this.arguments))
			{
				this.arguments = new string[0];
			}
		}
		private string changeParamStart(string line, string param)
		{
			line = this.cString(line, false);
			checked
			{
				while (line.StartsWith(param + " ", StringComparison.InvariantCultureIgnoreCase) & Strings.Len(line) > Strings.Len(param) + 1)
				{
					line = param + Strings.Trim(Strings.Mid(line, Strings.Len(param) + 1));
				}
				return line;
			}
		}
		private string cString(object textIn, bool isNewInstance = false)
		{
			if (this.isEmpty(RuntimeHelpers.GetObjectValue(textIn)))
			{
				return "";
			}
			if (isNewInstance)
			{
				return string.Copy(textIn.ToString());
			}
			return textIn.ToString();
		}
		private void ifRunExe()
		{
			if (this.isPublic)
			{
				this.runExe(this.exeFile, null);
			}
		}
		private void runExe(string file, string param)
		{
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.FileName = file;
			if (!Information.IsNothing(param))
			{
				processStartInfo.Arguments = param;
			}
			Process process = Process.Start(processStartInfo);
			Thread.Sleep(200);
		}
		private void ifRunExeBat()
		{
			if (this.isPublic)
			{
				string text = this.myIniFile + ".bat";
				this.writeFile(text, "\"" + this.exeFile + "\"");
				Process process = Process.Start(new ProcessStartInfo
				{
					FileName = text
				});
				Thread.Sleep(100);
				File.Delete(text);
			}
		}
		public void processIPinfoExe()
		{
			this.lastIp = "";
			string[] ipsCheck = Strings.Split(this.iPList, ",", -1, CompareMethod.Binary);
			List<string> list = new List<string>();
			int num = 0;
			checked
			{
				int num2 = this.urls.Count - 1;
				if (this.positionIndex > 0)
				{
					num = this.positionIndex - 1;
					num2 = num;
				}
				else
				{
					if (this.positionIndex == 0)
					{
						Random random = new Random();
						num = random.Next(this.urls.Count - 1);
						num2 = num;
					}
				}
				int arg_75_0 = num;
				int num3 = num2;
				for (int i = arg_75_0; i <= num3; i++)
				{
					string var = "";
					if (!this.isEmpty(this.ips[i]))
					{
						string url = this.httpStart2 + this.ips[i];
						string input = this.cString(this.getPage(url), false);
						Regex regex = new Regex(this.regExs[i]);
						Match match = regex.Match(input);
						if (match.Success)
						{
							var = Strings.Trim(this.cString(match.Groups[this.regexGroupname].Value, false));
							this.lastIp = var;
							if (this.setResult(ref var, ref this.isIpFromTwoSources, ref list, ipsCheck, url))
							{
								return;
							}
						}
					}
					if (this.isEmpty(var))
					{
						string url = this.urls[i];
						string input2 = this.cString(this.getPage(url), false);
						Regex regex2 = new Regex(this.regExs[i]);
						Match match2 = regex2.Match(input2);
						if (match2.Success)
						{
							var = Strings.Trim(this.cString(match2.Groups[this.regexGroupname].Value, false));
							this.lastIp = var;
							if (this.setResult(ref var, ref this.isIpFromTwoSources, ref list, ipsCheck, url))
							{
								return;
							}
						}
					}
				}
			}
		}
		public bool IsAddressValid(string addrString)
		{
			IPAddress iPAddress = null;
			return !this.isEmpty(addrString) && IPAddress.TryParse(addrString, out iPAddress);
		}
		private bool setResult(ref string currentIp, ref bool checkTwoSurces, ref List<string> result, string[] ipsCheck, string url)
		{
			if (this.IsAddressValid(currentIp))
			{
				if (Array.IndexOf<string>(ipsCheck, currentIp) > -1)
				{
					if (!checkTwoSurces)
					{
						this.isPublic = true;
						this.urlResult = url;
						return true;
					}
					if (result.Contains(currentIp))
					{
						this.isPublic = true;
						this.urlResult = url;
						return true;
					}
					this.urlResult2 = url;
				}
				if (!result.Contains(currentIp))
				{
					result.Add(currentIp);
				}
			}
			return false;
		}
		public string processIP()
		{
			List<string> list = new List<string>();
			int num = 0;
			checked
			{
				int num2 = this.urls.Count - 1;
				if (this.positionIndex > 0)
				{
					num = this.positionIndex - 1;
					num2 = num;
				}
				else
				{
					if (this.positionIndex == 0)
					{
						Random random = new Random();
						num = random.Next(this.urls.Count - 1);
						num2 = num;
					}
				}
				int arg_57_0 = num;
				int num3 = num2;
				for (int i = arg_57_0; i <= num3; i++)
				{
					this.timeStart = DateTime.Now;
					try
					{
						string text = "";
						string url = "";
						if (!this.isEmpty(this.ips[i]))
						{
							url = this.httpStart2 + this.ips[i];
							string input = this.cString(this.getPage(url), false);
							Regex regex = new Regex(this.regExs[i]);
							Match match = regex.Match(input);
							if (match.Success)
							{
								text = Strings.Trim(this.cString(match.Groups[this.regexGroupname].Value, false));
								if (this.setResultIp(ref text, ref list, ref this.isIpFromTwoSources, url) && !this.isAllURLs)
								{
									this.checkLastIp(text);
									string result = text;
									return result;
								}
							}
						}
						if (this.isEmpty(text))
						{
							url = this.urls[i];
							string input2 = this.cString(this.getPage(url), false);
							Regex regex2 = new Regex(this.regExs[i]);
							Match match2 = regex2.Match(input2);
							if (match2.Success)
							{
								text = Strings.Trim(this.cString(match2.Groups[this.regexGroupname].Value, false));
								if (this.setResultIp(ref text, ref list, ref this.isIpFromTwoSources, url) && !this.isAllURLs)
								{
									this.checkLastIp(text);
									string result = text;
									return result;
								}
							}
						}
						if (this.isAllURLs)
						{
							this.urlResult = url;
							this.x++;
							this.consoleWrite(Conversions.ToString(this.x) + ". ");
							this.checkLastIp(text);
							this.writeIpAddress(text);
						}
					}
					catch (Exception expr_221)
					{
						ProjectData.SetProjectError(expr_221);
						ProjectData.ClearProjectError();
					}
				}
				return "";
			}
		}
		private void checkLastIp(string newIp)
		{
			if (this.mode == externalIP.modes.runExeWhenExternalIpChange && this.IsAddressValid(newIp))
			{
				this.lastIp = this.configValue(this.paramLastIP, null);
				if (this.IsAddressValid(this.lastIp))
				{
					if (Operators.CompareString(this.lastIp, newIp, false) != 0)
					{
						this.configValue(this.paramLastIP, newIp);
						this.runExe(this.exeFile, newIp);
					}
				}
				else
				{
					this.configValue(this.paramLastIP, newIp);
				}
			}
		}
		private bool setResultIp(ref string currentIp, ref List<string> result, ref bool checkTwoSurces, string url)
		{
			if (this.IsAddressValid(currentIp))
			{
				if (!checkTwoSurces)
				{
					this.urlResult = url;
					return true;
				}
				if (result.Contains(currentIp))
				{
					this.urlResult = url;
					return true;
				}
				if (!result.Contains(currentIp))
				{
					this.urlResult2 = url;
					result.Add(currentIp);
				}
			}
			return false;
		}
		public string getPage(string url)
		{
			string result;
			try
			{
				DateTime now = DateTime.Now;
				this.timeStart = DateTime.Now;
				externalIP.getPage2Thread getPage2Thread;
				Thread thread;
				externalIP.getPage3Thread getPage3Thread;
				if (!this.isPOSTMethod)
				{
					getPage2Thread = new externalIP.getPage2Thread();
					getPage2Thread.isCachePreventAborted = this.isCachePreventAborted;
					getPage2Thread.timeOut = this.timeOut;
					getPage2Thread.url = url;
					getPage2Thread.urlUniqueAdd = this.urlUniqueAdd();
					externalIP.getPage2Thread VB$t_ref$S6 = getPage2Thread;
					thread = new Thread(delegate
					{
						VB$t_ref$S6.getPage();
					});
				}
				else
				{
					getPage3Thread = new externalIP.getPage3Thread();
					getPage3Thread.isCachePreventAborted = this.isCachePreventAborted;
					getPage3Thread.timeOut = this.timeOut;
					getPage3Thread.url = url;
					getPage3Thread.urlUniqueAdd = this.urlUniqueAdd();
					externalIP.getPage3Thread VB$t_ref$S7 = getPage3Thread;
					thread = new Thread(delegate
					{
						VB$t_ref$S7.getPage();
					});
				}
				thread.Start();
				this.isTimeOut = false;
				this.timeoutUrl = "";
				double num;
				do
				{
					Thread.Sleep(10);
					num = DateTime.Now.Subtract(this.timeStart).TotalSeconds * 1000.0;
				}
				while (!(num > (double)this.timeOut | !thread.IsAlive));
				string text;
				if (!this.isPOSTMethod)
				{
					text = this.cString(getPage2Thread.result, true);
				}
				else
				{
					text = this.cString(getPage3Thread.result, true);
				}
				if (this.isEmpty(text))
				{
					this.isTimeOut = true;
					this.timeoutUrl = url;
				}
				thread.Abort();
				result = text;
			}
			catch (Exception expr_175)
			{
				ProjectData.SetProjectError(expr_175);
				result = "";
				ProjectData.ClearProjectError();
			}
			return result;
		}
		private string getPage5(string url)
		{
			string result;
			return result;
		}
		private string getPage4(string url)
		{
			byte[] bytes = Encoding.ASCII.GetBytes("test");
			WebRequest webRequest = WebRequest.Create(url);
			webRequest.Timeout = this.timeOut;
			webRequest.Method = "POST";
			webRequest.Timeout = -1;
			webRequest.Headers.Add("lastCached", DateTime.Now.ToString());
			webRequest.ContentType = "text/xml;charset=\\\"utf-8\\\"";
			Stream requestStream = webRequest.GetRequestStream();
			requestStream.Write(bytes, 0, 1);
			requestStream.Flush();
			requestStream.Close();
			WebResponse response = webRequest.GetResponse();
			StreamReader streamReader = new StreamReader(response.GetResponseStream());
			string result = streamReader.ReadToEnd();
			streamReader.Close();
			return result;
		}
		public bool isEmpty(object var)
		{
			if (Information.IsNothing(RuntimeHelpers.GetObjectValue(var)))
			{
				return true;
			}
			try
			{
				if (Strings.Len(var.ToString()) == 0)
				{
					return true;
				}
			}
			catch (Exception arg_23_0)
			{
				ProjectData.SetProjectError(arg_23_0);
				ProjectData.ClearProjectError();
			}
			return false;
		}
		private void fillParam(string paramName, string line, ref string paramValue)
		{
			paramValue = this.cString(paramValue, false);
			checked
			{
				if (line.StartsWith(paramName + "=", StringComparison.InvariantCultureIgnoreCase) & Strings.Len(line) > Strings.Len(paramName) + 1)
				{
					paramValue = Strings.Trim(Strings.Mid(line, Strings.Len(paramName) + 2));
				}
			}
		}
		public string getIpFromUrl(string url)
		{
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			Uri uri = new Uri(url);
			processStartInfo.UseShellExecute = false;
			processStartInfo.RedirectStandardInput = true;
			processStartInfo.RedirectStandardOutput = true;
			processStartInfo.FileName = "ping";
			processStartInfo.Arguments = uri.Authority + " -n 1 -i 1";
			Process process = Process.Start(processStartInfo);
			string text = "server";
			StreamReader standardOutput = process.StandardOutput;
			Regex regex = new Regex(string.Format("\\[(?<{0}>.*?)\\]", text));
			int num = 0;
			string result = "";
			checked
			{
				while (standardOutput.Peek() > -1)
				{
					string input = standardOutput.ReadLine();
					num++;
					Match match = regex.Match(input);
					if (match.Success | num > 3)
					{
						result = match.Groups[text].Value;
						break;
					}
				}
				return result;
			}
		}
		private string urlUniqueAdd()
		{
			if (this.isCachePreventAborted)
			{
				return "";
			}
			return DateAndTime.Now.ToString("?yyMMddHHmmssfffffff");
		}
		public externalIP()
		{
			this.writeToOrderOnly = false;
			this.setDefaultValues();
		}
		[CompilerGenerated]
		private static int _Lambda$__1(externalIP.order x, externalIP.order y)
		{
			return x.avgTime.CompareTo(y.avgTime);
		}
	}
}
