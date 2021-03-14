using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LdapHelperLib
{
	public class LdapServerProfileConfig
	{
		public string Profile { get; set; }
		public string Server { get; set; }
		public string Port { get; set; }
		public string Port_GC { get; set; }
		public string BaseDN { get; set; }
		public string BaseDN_GC { get; set; }
		public string ConnectionTimeOut { get; set; }
		public string UseSSL { get; set; }
		public string User { get; set; }
		public string Password { get; set; }

		public int GetPort(bool forGC)
		{
			if (forGC)
			{
				if (Port_GC.ToLower().Equals("default"))
					return (int)LdapHelperLib.DefaultServerPorts.GlobalCatalogPort;

				return Convert.ToInt32(Port_GC);
			}
			else
			{
				if (Port.ToLower().Equals("default"))
					return (int)LdapHelperLib.DefaultServerPorts.DefaultPort;

				return Convert.ToInt32(Port);
			}
		}
		////VBG: Old version
		//public int GetPortGC()
		//{
		//	if (Port_GC.ToLower().Equals("default"))
		//		return (int)LdapHelperLib.DefaultServerPorts.GlobalCatalogPort;

		//	return Convert.ToInt32(Port_GC);
		//}

		public string GetBaseDN(bool forGC)
		{
			return (forGC ? BaseDN_GC : BaseDN);
		}
	}
}
