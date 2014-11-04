// This partial class-file contains my c# additions to externalIP.
using System.Collections.Generic;
namespace indoo.tools
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Microsoft.VisualBasic.CompilerServices;
    using System.Threading;

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
        public void processIP_async(bool saveSkipIPValue, bool skipIPValue, string[] args) {

            this.initValues();
            this.setArguments(args);
            this.setVariables();
            this.fillIniFileParameters(args);

			List<string> list = new List<string>();
			int firstUrl_index = 0;
            int lastUrl_index = this.urls.Count - 1;

			if (this.positionIndex > 0) {
			    // only use one url
				firstUrl_index = this.positionIndex - 1;
				lastUrl_index = firstUrl_index;

			} else if (this.positionIndex == 0) {
			    // use a single random url
				Random random = new Random();
				firstUrl_index = random.Next(this.urls.Count - 1);
				lastUrl_index = firstUrl_index;
            }

			for (int i = firstUrl_index; i <= lastUrl_index; i++)
			{
				this.timeStart = DateTime.Now;
				try	{
					string text = "";
					if (!this.isEmpty(this.ips[i]))	{

                        string url = this.httpStart2 + this.ips[i];
                        Regex regex = new Regex(this.regExs[i]);

                        getPage_ip4(url, regex);
                        /*
						string input = this.cString(this.getPage(url), false);

						Regex regex = new Regex(this.regExs[i]);
						Match match = regex.Match(input);
						if (match.Success) {
							text = Strings.Trim(this.cString(match.Groups[this.regexGroupname].Value, false));

							if (this.setResultIp(ref text, ref list, ref this.isIpFromTwoSources, url) && !this.isAllURLs)
							{
								this.checkLastIp(text);
								string result = text;
								return result;
							}
						}*/
					}
					if (this.isEmpty(text))
					{
						string url = this.urls[i];
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
								//return result;
							}
						}
					}
                    /*
					if (this.isAllURLs)
					{
						this.urlResult = url;
						this.x++;
						this.consoleWrite(Conversions.ToString(this.x) + ". ");
						this.checkLastIp(text);
						this.writeIpAddress(text);
					}*/
				}
				catch (Exception expr_221)
				{
					ProjectData.SetProjectError(expr_221);
					ProjectData.ClearProjectError();
				}
			}
			//return "";
		}

        void LookupCompleted(string ipResult, Regex extractionRegex) {

            if (OperationComplete != null) {

                Match match = extractionRegex.Match(ipResult);
                if (match.Success) {

                    OperationComplete(
                        this,
                        new LookupEventArgs(
                            ipResult,
                            false,
                            false,
                            false
                        )
                    );
                }
            }
        }

        public void getPage_ip4(string url, Regex extractionRegex) {

            try {
                DateTime now = DateTime.Now;
                this.timeStart = DateTime.Now;

                Thread thread;

                if (this.isPOSTMethod) {
                    externalIP.getPage3Thread postMethodRequest = new externalIP.getPage3Thread();
                    postMethodRequest.isCachePreventAborted = this.isCachePreventAborted;
                    postMethodRequest.timeOut = this.timeOut;
                    postMethodRequest.url = url;
                    postMethodRequest.urlUniqueAdd = this.urlUniqueAdd();
                    thread = new Thread(
                        (ThreadStart)delegate {
                            postMethodRequest.getPage();
                            LookupCompleted(postMethodRequest.result, extractionRegex);
                        }
                    );
                } else {
                    externalIP.getPage2Thread getMethodRequest = new externalIP.getPage2Thread();
                    getMethodRequest.isCachePreventAborted = this.isCachePreventAborted;
                    getMethodRequest.timeOut = this.timeOut;
                    getMethodRequest.url = url;
                    getMethodRequest.urlUniqueAdd = this.urlUniqueAdd();
                    thread = new Thread(
                        (ThreadStart)delegate {
                            getMethodRequest.getPage();
                            LookupCompleted(getMethodRequest.result, extractionRegex);
                        }
                    );
                }
                /*, 
                thread.Start();
                this.isTimeOut = false;
                this.timeoutUrl = "";
                double num;
                do {
                    Thread.Sleep(10);
                    num = DateTime.Now.Subtract(this.timeStart).TotalSeconds * 1000.0;
                }
                while (!(num > (double)this.timeOut | !thread.IsAlive));
                string text;
                if (!this.isPOSTMethod) {
                    text = this.cString(getPage2Thread.result, true);
                } else {
                    text = this.cString(getPage3Thread.result, true);
                }
                if (this.isEmpty(text)) {
                    this.isTimeOut = true;
                    this.timeoutUrl = url;
                }
                thread.Abort();
                result = text;
                 * */
            } catch (Exception ex) {
                ProjectData.SetProjectError(ex);
                ProjectData.ClearProjectError();
            }
        }



        /// <summary>
        /// Raised on a pool thread when the external address is found, or
        /// skipped, or timed out.
        /// </summary>
        public EventHandler<LookupEventArgs> OperationComplete;

	}
}
