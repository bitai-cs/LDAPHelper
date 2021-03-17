using LdapHelperDTO;
using Novell.Directory.Ldap;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LdapHelperLib
{
	public abstract class BaseHelper
	{
		#region Protected fields
		public static readonly IEnumerable<string> ObjectSidAndSAMAccountNameAttributeNames;
		public static readonly IEnumerable<string> ObjectSidAttributeName;
		public static readonly IEnumerable<string> CNAttributeName;
		public static readonly IEnumerable<string> MemberAttributeName;
		public static readonly IEnumerable<string> MemberOfAttributeName;
		public static readonly IEnumerable<string> MemberAndMemberOfAttributeNames;

		public static readonly IEnumerable<string> MinimunAttributeNames;
		public static readonly IEnumerable<string> MinimunWithMemberAttributeNames;
		public static readonly IEnumerable<string> MinimunWithMemberOfAttributeNames;
		public static readonly IEnumerable<string> MinimunWithMemberAndMemberOfAttributeNames;

		public static readonly IEnumerable<string> FewAttributeNames;
		public static readonly IEnumerable<string> FewWithMemberAttributeNames;
		public static readonly IEnumerable<string> FewWithMemberOfAttributeNames;
		public static readonly IEnumerable<string> FewWithMemberAndMemberOfAttributeNames;

		public static readonly IEnumerable<string> AllAttributeNames;
		public static readonly IEnumerable<string> AllWithMemberAttributeNames;
		public static readonly IEnumerable<string> AllWithMemberOfAttributeNames;
		public static readonly IEnumerable<string> AllWithMemberAndMemberOfAttributeNames;
		#endregion


		#region Static methods
		internal static IEnumerable<string> GetMinimunAttributeNames()
		{
			return new string[] { LdapHelperDTO.EntryAttribute.objectSid.ToString(), LdapHelperDTO.EntryAttribute.distinguishedName.ToString(), LdapHelperDTO.EntryAttribute.sAMAccountName.ToString(), LdapHelperDTO.EntryAttribute.cn.ToString(), LdapHelperDTO.EntryAttribute.displayName.ToString(), LdapHelperDTO.EntryAttribute.objectClass.ToString() };
		}

		internal static IEnumerable<string> GetMinimunWithMemberAttributeNames()
		{
			return GetMinimunAttributeNames().Concat(MemberAttributeName);
		}

		internal static IEnumerable<string> GetMinimunWithMemberOfAttributeNames()
		{
			return GetMinimunAttributeNames().Concat(MemberOfAttributeName);
		}

		internal static IEnumerable<string> GetMinimunWithMemberAndMemberOfAttributeNames()
		{
			return GetMinimunAttributeNames().Concat(MemberAndMemberOfAttributeNames);
		}

		internal static IEnumerable<string> GetFewAttributeNames()
		{
			return new string[] { LdapHelperDTO.EntryAttribute.objectSid.ToString(), LdapHelperDTO.EntryAttribute.objectGuid.ToString(), LdapHelperDTO.EntryAttribute.distinguishedName.ToString(), LdapHelperDTO.EntryAttribute.sAMAccountName.ToString(), LdapHelperDTO.EntryAttribute.cn.ToString(), LdapHelperDTO.EntryAttribute.name.ToString(), LdapHelperDTO.EntryAttribute.displayName.ToString(), LdapHelperDTO.EntryAttribute.objectClass.ToString(), LdapHelperDTO.EntryAttribute.objectCategory.ToString() };
		}

		internal static IEnumerable<string> GetFewWithMemberAttributeNames()
		{
			return GetFewAttributeNames().Concat(new string[1] { "member" });
		}

		internal static IEnumerable<string> GetFewWithMemberOfAttributeNames()
		{
			return GetFewAttributeNames().Concat(new string[1] { "memberOf" });
		}

		internal static IEnumerable<string> GetFewWithMemberAndMemberOfAttributeNames()
		{
			return GetFewAttributeNames().Concat(new string[2] { "member", "memberOf" });
		}

		internal static IEnumerable<string> GetAllAttributeNames()
		{
			return Enum.GetNames(typeof(EntryAttribute)).Except(MemberAndMemberOfAttributeNames);
		}

		internal static IEnumerable<string> GetAllWithMemberAttributeNames()
		{
			return GetAllAttributeNames().Concat(MemberAttributeName);
		}

		internal static IEnumerable<string> GetAllWithMemberOfAttributeNames()
		{
			return GetAllAttributeNames().Concat(MemberOfAttributeName);
		}

		internal static IEnumerable<string> GetAllWithMemberAndMemberOfAttributeNames()
		{
			return GetAllAttributeNames().Concat(MemberAndMemberOfAttributeNames);
		}
		#endregion


		#region Static constructor
		static BaseHelper()
		{
			ObjectSidAndSAMAccountNameAttributeNames = new string[2] { LdapHelperDTO.EntryAttribute.objectSid.ToString(), LdapHelperDTO.EntryAttribute.sAMAccountName.ToString() };
			ObjectSidAttributeName = new string[1] { LdapHelperDTO.EntryAttribute.objectSid.ToString() };
			CNAttributeName = new string[1] { LdapHelperDTO.EntryAttribute.cn.ToString() };
			MemberAttributeName = new string[1] { LdapHelperDTO.EntryAttribute.member.ToString() };
			MemberOfAttributeName = new string[1] { LdapHelperDTO.EntryAttribute.memberOf.ToString() };
			MemberAndMemberOfAttributeNames = MemberAttributeName.Concat(MemberOfAttributeName);

			MinimunAttributeNames = GetMinimunAttributeNames().ToArray();
			MinimunWithMemberAttributeNames = GetMinimunWithMemberAttributeNames().ToArray();
			MinimunWithMemberOfAttributeNames = GetMinimunWithMemberOfAttributeNames().ToArray();
			MinimunWithMemberAndMemberOfAttributeNames = GetMinimunWithMemberAndMemberOfAttributeNames().ToArray();

			FewAttributeNames = GetFewAttributeNames().ToArray();
			FewWithMemberAttributeNames = GetFewWithMemberAttributeNames().ToArray();
			FewWithMemberOfAttributeNames = GetFewWithMemberOfAttributeNames().ToArray();
			FewWithMemberAndMemberOfAttributeNames = GetFewWithMemberAndMemberOfAttributeNames().ToArray();

			AllAttributeNames = GetAllAttributeNames().ToArray();
			AllWithMemberAttributeNames = GetAllWithMemberAttributeNames().ToArray();
			AllWithMemberOfAttributeNames = GetAllWithMemberOfAttributeNames().ToArray();
			AllWithMemberAndMemberOfAttributeNames = GetAllWithMemberAndMemberOfAttributeNames().ToArray();
		}
		#endregion


		#region Properties
		public LdapConnectionInfo ConnectionInfo { get; set; }
		public LdapUserCredentials UserCredentials { get; set; }
		public LdapSearchLimits SearchLimits { get; set; }
		#endregion


		#region Constructor
		protected BaseHelper(LdapClientConfiguration clientConfiguration)
		{
			ConnectionInfo = clientConfiguration.ServerSettings;
			UserCredentials = clientConfiguration.UserCredentials;
			SearchLimits = clientConfiguration.SearchLimits;
		}

		protected BaseHelper(LdapConnectionInfo connectionInfo, LdapSearchLimits searchLimits, LdapUserCredentials userCredentials)
		{
			ConnectionInfo = connectionInfo;
			SearchLimits = searchLimits;
			UserCredentials = userCredentials;
		}

		/// <summary>
		/// Constructor used by <see cref="LdapAuthenticator"/>
		/// </summary>
		/// <param name="connectionInfo"><see cref="LdapConnectionInfo"/></param>
		protected BaseHelper(LdapConnectionInfo connectionInfo)
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

		protected async Task<LdapConnection> GetLdapConnection(LdapConnectionInfo server, LdapUserCredentials credentials, bool bindRequired = true)
		{
			if (ConnectionInfo.UseSSL)
				throw new NotImplementedException("SSL connection not implemented.");

			var _connection = new LdapConnection
			{
				ConnectionTimeout = ConnectionInfo.ConnectionTimeout * 1000,
			};

			await Task.Run(() =>
			{
				_connection.Connect(server.ServerName, server.ServerPort);

				try
				{
					_connection.Bind(credentials.Username, credentials.Password);
				}
				catch (LdapException ex)
				{
					if (bindRequired)
						throw ex;
				}
				catch (Exception ex)
				{
					throw ex;
				}
			});

			return _connection;
		}

		protected string ConvertByteToStringSid(Byte[] sidBytes)
		{
			short sSubAuthorityCount = 0;
			StringBuilder strSid = new StringBuilder();
			strSid.Append("S-");
			try
			{
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
			}
			catch (Exception ex)
			{
				throw ex;
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
	}
}