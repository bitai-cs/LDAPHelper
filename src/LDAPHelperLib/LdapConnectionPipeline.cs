using System;
using System.Collections.Generic;
using System.Text;

namespace LdapHelperLib
{
	public class LdapConnectionPipeline
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="useSSL"></param>
		/// <param name="connectionTimeOut"></param>
		public LdapConnectionPipeline(bool useSSL, short connectionTimeOut)
		{
			UseSSL = useSSL;
			ConnectionTimeOut = connectionTimeOut;
		}

		/// <summary>
		/// Flag to set an SSL mode connection
		/// </summary>
		public bool UseSSL { get; }

		/// <summary>
		/// Connection timeout in seconds.
		/// </summary>
		public short ConnectionTimeOut { get; }
	}
}
