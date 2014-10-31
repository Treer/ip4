using indoo.tools;
using Microsoft.VisualBasic.CompilerServices;
using System;
namespace indoo
{
	[StandardModule]
	internal sealed class Module1
	{
		private static externalIP externalIP = new externalIP();
		[STAThread]
		public static void Main(string[] args)
		{
			Module1.externalIP.execute(args);
		}
	}
}
