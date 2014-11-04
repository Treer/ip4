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

        public void processIP_async(bool save_SkipIPLookupArg, bool skipIPLookupArg, string[] args) {

            Thread thread = new Thread(
                (ThreadStart)delegate {

                    // Perform the threaded operation using a seperate externalIP
                    // instance to ensure there is only ever one thread acting on 
                    // a class instance.
                    externalIP workerExternalIP = new externalIP();

                    workerExternalIP.initValues();
                    workerExternalIP.setArguments(args);
                    workerExternalIP.isQuiet = true; // supress externalIP from outputting its program title;
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
