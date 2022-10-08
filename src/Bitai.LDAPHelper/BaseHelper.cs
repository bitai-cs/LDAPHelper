using Bitai.LDAPHelper.DTO;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper
{
	public abstract partial class BaseHelper
	{
		#region Properties
		public ConnectionInfo ConnectionInfo { get; set; }
		public LDAPDomainAccountCredential DomainAccountCredential { get; set; }
		public SearchLimits SearchLimits { get; set; }
		#endregion


		#region Protected constructors
		protected BaseHelper(ClientConfiguration clientConfiguration)
		{
			ConnectionInfo = clientConfiguration.ServerSettings;
			DomainAccountCredential = clientConfiguration.DomainAccountCredential;
			SearchLimits = clientConfiguration.SearchLimits;
		}

		protected BaseHelper(ConnectionInfo connectionInfo, SearchLimits searchLimits, LDAPDomainAccountCredential domainAccountCredential)
		{
			ConnectionInfo = connectionInfo;
			SearchLimits = searchLimits;
			DomainAccountCredential = domainAccountCredential;
		}

		/// <summary>
		/// Constructor used by <see cref="Authenticator"/>
		/// </summary>
		/// <param name="connectionInfo"><see cref="LDAPHelper.ConnectionInfo"/></param>
		protected BaseHelper(ConnectionInfo connectionInfo)
		{
			ConnectionInfo = connectionInfo;
		}
		#endregion


		#region Protected methods
		protected string GetSAMAccountTypeName(string index)
		{
			switch (index)
			{
				case "268435456":
					return "SAM_GROUP_OBJECT";
				case "268435457":
					return "SAM_NON_SECURITY_GROUP_OBJECT";
				case "536870912":
					return "SAM_ALIAS_OBJECT";
				case "536870913":
					return "SAM_NON_SECURITY_ALIAS_OBJECT";
				case "805306368":
					return "SAM_NORMAL_USER_ACCOUNT";
				case "805306369":
					return "SAM_MACHINE_ACCOUNT";
				case "805306370":
					return "SAM_TRUST_ACCOUNT";
				case "1073741824":
					return "SAM_APP_BASIC_GROUP";
				case "1073741825":
					return "SAM_APP_QUERY_GROUP";
				case "2147483647":
					return "SAM_ACCOUNT_TYPE_MAX";
				default:
					return index;
			}
		}

		/// <summary>
		/// Get <see cref="LdapConnection"/>
		/// </summary>
		/// <param name="connectionInfo"><see cref="LDAPHelper.ConnectionInfo"/> to connect to the LDAP Server</param>
		/// <param name="credential"><see cref="DomainAccountCredential"/>  to connect to the LDAP Server</param>
		/// <param name="bindRequired">If <see cref="DomainAccountCredential"/> are required to be mandatorily authenticated on the LDAP Server</param>
		/// <returns>Task of <see cref="LdapConnection"/></returns>
		protected Task<LdapConnection> GetLdapConnection(ConnectionInfo connectionInfo, LDAPDomainAccountCredential credential, bool bindRequired = true)
		{
			return getLdapConnection(connectionInfo, credential.DomainAccountName, credential.DomainAccountPassword, bindRequired);
		}

		protected Task<LdapConnection> GetLdapConnection(ConnectionInfo connectionInfo, string distinguishedName, string password, bool bindRequired = true)
		{
			return getLdapConnection(connectionInfo, distinguishedName, password, bindRequired);
		}

		protected string ConvertByteToStringSid(byte[] sidBytes)
		{
			short sSubAuthorityCount = 0;
			StringBuilder strSid = new StringBuilder();
			strSid.Append("S-");

			// Add SID revision.
			strSid.Append(sidBytes[0].ToString());

			sSubAuthorityCount = Convert.ToInt16(sidBytes[1]);

			// Next six bytes are SID authority value.
			if (sidBytes[2] != 0 || sidBytes[3] != 0)
			{
				string strAuth = String.Format("0x{0:2x}{1:2x}{2:2x}{3:2x}{4:2x}{5:2x}",
									(Int16)sidBytes[2],
									(Int16)sidBytes[3],
									(Int16)sidBytes[4],
									(Int16)sidBytes[5],
									(Int16)sidBytes[6],
									(Int16)sidBytes[7]);
				strSid.Append("-");
				strSid.Append(strAuth);
			}
			else
			{
				Int64 iVal = sidBytes[7] +
					  (sidBytes[6] << 8) +
					  (sidBytes[5] << 16) +
					  (sidBytes[4] << 24);
				strSid.Append("-");
				strSid.Append(iVal.ToString());
			}

			// Get sub authority count...
			int idxAuth = 0;
			for (int i = 0; i < sSubAuthorityCount; i++)
			{
				idxAuth = 8 + i * 4;
				UInt32 iSubAuth = BitConverter.ToUInt32(sidBytes, idxAuth);
				strSid.Append("-");
				strSid.Append(iSubAuth.ToString());
			}

			return strSid.ToString();
		}

		protected IEnumerable<string> GetRequiredAttributeNames(RequiredEntryAttributes requiredEntryAttributes)
		{
			switch (requiredEntryAttributes)
			{
				case RequiredEntryAttributes.Minimun:
					return BaseHelper.MinimunAttributeNames;
				case RequiredEntryAttributes.MinimunWithMember:
					return BaseHelper.MinimunWithMemberAttributeNames;
				case RequiredEntryAttributes.MinimunWithMemberOf:
					return BaseHelper.MinimunWithMemberOfAttributeNames;
				case RequiredEntryAttributes.MinimunWithMemberAndMemberOf:
					return BaseHelper.MinimunWithMemberAndMemberOfAttributeNames;
				case RequiredEntryAttributes.Few:
					return BaseHelper.FewAttributeNames;
				case RequiredEntryAttributes.FewWithMember:
					return BaseHelper.FewWithMemberAttributeNames;
				case RequiredEntryAttributes.FewWithMemberOf:
					return BaseHelper.FewWithMemberOfAttributeNames;
				case RequiredEntryAttributes.FewWithMemberAndMemberOf:
					return BaseHelper.FewWithMemberAndMemberOfAttributeNames;
				case RequiredEntryAttributes.All:
					return BaseHelper.AllAttributeNames;
				case RequiredEntryAttributes.AllWithMember:
					return BaseHelper.AllWithMemberAttributeNames;
				case RequiredEntryAttributes.AllWithMemberOf:
					return BaseHelper.AllWithMemberOfAttributeNames;
				case RequiredEntryAttributes.AllWithMemberAndMemberOf:
					return BaseHelper.AllWithMemberAndMemberOfAttributeNames;
				case RequiredEntryAttributes.OnlyMember:
					return BaseHelper.MemberAttributeName;
				case RequiredEntryAttributes.OnlyMemberOf:
					return BaseHelper.MemberOfAttributeName;
				case RequiredEntryAttributes.MemberAndMemberOf:
					return BaseHelper.MemberAndMemberOfAttributeNames;
				case RequiredEntryAttributes.OnlyCN:
					return BaseHelper.CNAttributeName;
				case RequiredEntryAttributes.OnlyObjectSid:
					return BaseHelper.ObjectSidAttributeName;
				case RequiredEntryAttributes.ObjectSidAndSAMAccountName:
					return BaseHelper.ObjectSidAndSAMAccountNameAttributeNames;
				default:
					throw new ArgumentOutOfRangeException("requiredEntryAttributes");
			}
		}
		#endregion


		#region Private methods
		private Task<LdapConnection> getLdapConnection(ConnectionInfo connectionInfo, string dn, string password, bool bindRequired = true)
		{
			return Task.Run(() =>
			{
				var ldapConnection = new LdapConnection
				{
					ConnectionTimeout = ConnectionInfo.ConnectionTimeout * 1000
				};

				if (connectionInfo.UseSSL)
				{
					ldapConnection.SecureSocketLayer = true;
					ldapConnection.UserDefinedServerCertValidationDelegate += (sender, certificate, chain, sslPolicyErrors) => true;
				}

				ldapConnection.Connect(connectionInfo.Server, connectionInfo.ServerPort);

				try
				{
					ldapConnection.Bind(dn, password);
				}
				catch (LdapException)
				{
					if (bindRequired)
						throw;
				}
				catch (Exception)
				{
					throw;
				}

				return ldapConnection;
			});
		}
		#endregion
	}
}