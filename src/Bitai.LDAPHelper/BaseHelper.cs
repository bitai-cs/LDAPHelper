using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bitai.LDAPHelper.Extensions;
using Bitai.LDAPHelper.LdapAdapters;

namespace Bitai.LDAPHelper
{
	/// <summary>
	/// Base class for LDAP helper services, providing shared connection and mapping utilities.
	/// </summary>
	public abstract partial class BaseHelper
	{
		#region Properties
		/// <summary>
		/// Gets or sets LDAP server connection settings.
		/// </summary>
		public ConnectionInfo ConnectionInfo { get; set; }

		/// <summary>
		/// Gets or sets the credential used for LDAP search/management operations.
		/// </summary>
		public DTO.LDAPDomainAccountCredential DomainAccountCredential { get; set; }

		/// <summary>
		/// Gets or sets default LDAP search limits.
		/// </summary>
		public SearchLimits SearchLimits { get; set; }

		/// <summary>
		/// Gets or sets the LDAP connection factory abstraction.
		/// </summary>
        protected ILdapConnectionFactoryAdapter ConnectionFactory { get; set; }
        #endregion


        #region Protected constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="BaseHelper"/> class.
		/// </summary>
		/// <param name="clientConfiguration">Client configuration containing connection, credential, and search settings.</param>
		/// <param name="connectionFactory">LDAP connection factory abstraction.</param>
        protected BaseHelper(ClientConfiguration clientConfiguration, ILdapConnectionFactoryAdapter connectionFactory)
		{
			ConnectionInfo = clientConfiguration.ServerSettings;
			DomainAccountCredential = clientConfiguration.DomainAccountCredential;
			SearchLimits = clientConfiguration.SearchLimits;
			ConnectionFactory = connectionFactory;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseHelper"/> class.
		/// </summary>
		/// <param name="connectionInfo">LDAP server connection settings.</param>
		/// <param name="searchLimits">LDAP search limits.</param>
		/// <param name="domainAccountCredential">Credential used by LDAP operations.</param>
		/// <param name="connectionFactory">LDAP connection factory abstraction.</param>
		protected BaseHelper(ConnectionInfo connectionInfo, SearchLimits searchLimits, DTO.LDAPDomainAccountCredential domainAccountCredential, ILdapConnectionFactoryAdapter connectionFactory)
		{
			ConnectionInfo = connectionInfo;
			SearchLimits = searchLimits;
			DomainAccountCredential = domainAccountCredential;
			ConnectionFactory = connectionFactory;

        }

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseHelper"/> class used by authentication-only services.
		/// </summary>
		/// <param name="connectionInfo">LDAP server connection settings.</param>
		/// <param name="connectionFactory">LDAP connection factory abstraction.</param>
		protected BaseHelper(ConnectionInfo connectionInfo, ILdapConnectionFactoryAdapter connectionFactory)
		{
			ConnectionInfo = connectionInfo;
            ConnectionFactory = connectionFactory;
        }
		#endregion


		#region Protected methods
		/// <summary>
		/// Converts a SAM account-type numeric code to a symbolic name.
		/// </summary>
		/// <param name="index">String representation of the SAM account type numeric value.</param>
		/// <returns>A symbolic account type name when known; otherwise the original value.</returns>
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
		/// Creates and optionally binds an LDAP connection using domain-account credentials.
		/// </summary>
		/// <param name="connectionInfo">LDAP server connection settings.</param>
		/// <param name="credential">Domain-account credential used for bind.</param>
		/// <param name="bindRequired">Whether bind/authentication is required.</param>
		/// <returns>A task with an initialized LDAP connection adapter.</returns>
		protected async Task<ILdapConnectionAdapter> GetLdapConnection(ConnectionInfo connectionInfo, DTO.LDAPDomainAccountCredential credential, bool bindRequired = true)
		{
            //return getLdapConnection(connectionInfo, credential.DomainAccountName, credential.DomainAccountPassword, bindRequired);

            return await ConnectionFactory.CreateConnectionAsync(
                connectionInfo,
                credential.DomainAccountName,
                credential.DomainAccountPassword,
                bindRequired);
        }

		/// <summary>
		/// Creates and optionally binds an LDAP connection using distinguished-name credentials.
		/// </summary>
		/// <param name="connectionInfo">LDAP server connection settings.</param>
		/// <param name="credential">Distinguished-name credential used for bind.</param>
		/// <param name="bindRequired">Whether bind/authentication is required.</param>
		/// <returns>A task with an initialized LDAP connection adapter.</returns>
		protected async Task<ILdapConnectionAdapter> GetLdapConnection(ConnectionInfo connectionInfo, DTO.LDAPDistinguishedNameCredential credential, bool bindRequired = true)
		{
            //return getLdapConnection(connectionInfo, credential.DistinguishedName, credential.Password, bindRequired);

            return await ConnectionFactory.CreateConnectionAsync(
                connectionInfo,
                credential.DistinguishedName,
                credential.Password,
                bindRequired);
        }

		/// <summary>
		/// Converts a binary SID value into its canonical string representation.
		/// </summary>
		/// <param name="sidBytes">Binary SID value.</param>
		/// <returns>Canonical SID string (for example, S-1-5-21-...).</returns>
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

		/// <summary>
		/// Resolves the LDAP attribute names to request based on a predefined attribute-set preset.
		/// </summary>
		/// <param name="requiredEntryAttributes">Preset indicating which attributes should be loaded.</param>
		/// <returns>A sequence of attribute names to request in LDAP search operations.</returns>
		protected IEnumerable<string> GetRequiredAttributeNames(DTO.RequiredEntryAttributes requiredEntryAttributes)
		{
			switch (requiredEntryAttributes)
			{
				case DTO.RequiredEntryAttributes.Minimun:
					return BaseHelper.MinimunAttributeNames;
				case DTO.RequiredEntryAttributes.MinimunWithMember:
					return BaseHelper.MinimunWithMemberAttributeNames;
				case DTO.RequiredEntryAttributes.MinimunWithMemberOf:
					return BaseHelper.MinimunWithMemberOfAttributeNames;
				case DTO.RequiredEntryAttributes.MinimunWithMemberAndMemberOf:
					return BaseHelper.MinimunWithMemberAndMemberOfAttributeNames;
				case DTO.RequiredEntryAttributes.Few:
					return BaseHelper.FewAttributeNames;
				case DTO.RequiredEntryAttributes.FewWithMember:
					return BaseHelper.FewWithMemberAttributeNames;
				case DTO.RequiredEntryAttributes.FewWithMemberOf:
					return BaseHelper.FewWithMemberOfAttributeNames;
				case DTO.RequiredEntryAttributes.FewWithMemberAndMemberOf:
					return BaseHelper.FewWithMemberAndMemberOfAttributeNames;
				case DTO.RequiredEntryAttributes.All:
					return BaseHelper.AllAttributeNames;
				case DTO.RequiredEntryAttributes.AllWithMember:
					return BaseHelper.AllWithMemberAttributeNames;
				case DTO.RequiredEntryAttributes.AllWithMemberOf:
					return BaseHelper.AllWithMemberOfAttributeNames;
				case DTO.RequiredEntryAttributes.AllWithMemberAndMemberOf:
					return BaseHelper.AllWithMemberAndMemberOfAttributeNames;
				case DTO.RequiredEntryAttributes.OnlyMember:
					return BaseHelper.MemberAttributeName;
				case DTO.RequiredEntryAttributes.OnlyMemberOf:
					return BaseHelper.MemberOfAttributeName;
				case DTO.RequiredEntryAttributes.MemberAndMemberOf:
					return BaseHelper.MemberAndMemberOfAttributeNames;
				case DTO.RequiredEntryAttributes.OnlyCN:
					return BaseHelper.CNAttributeName;
				case DTO.RequiredEntryAttributes.OnlyObjectSid:
					return BaseHelper.ObjectSidAttributeName;
				case DTO.RequiredEntryAttributes.ObjectSidAndSAMAccountName:
					return BaseHelper.ObjectSidAndSAMAccountNameAttributeNames;
				default:
					throw new ArgumentOutOfRangeException("requiredEntryAttributes");
			}
		}        
        #endregion
    }
}
