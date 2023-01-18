using System;
using System.Collections.Generic;
using System.Text;

namespace Bitai.LDAPHelper.DTO
{
	public enum EntryAttribute
	{
		/// <summary>
		/// country abrev.
		/// </summary>
		c,
		cn,
		company,
		co,
		department,
		description,
		displayName,
		distinguishedName,
		givenName,
		l,
		lastLogonTimestamp,
		mail,
		manager,
		member,
		memberOf,
		name,
		objectCategory,
		objectClass,
		sAMAccountName,
		sAMAccountType,
		sn,
		telephoneNumber,
		title,
		userPassword,
		unicodePwd,
		userPrincipalName,
		whenCreated,
		objectGuid,
		objectSid,
		userAccountControl
	}

	public enum RequiredEntryAttributes
	{
		Minimun,
		MinimunWithMember,
		MinimunWithMemberOf,
		MinimunWithMemberAndMemberOf,
		Few,
		FewWithMember,
		FewWithMemberOf,
		FewWithMemberAndMemberOf,
		All,
		AllWithMember,
		AllWithMemberOf,
		AllWithMemberAndMemberOf,
		MemberAndMemberOf,
		ObjectSidAndSAMAccountName,
		OnlyMember,
		OnlyMemberOf,
		OnlyCN,
		OnlyObjectSid
	}

	/// <summary>
	/// MS Active Directory user account control flags
	/// 
	/// Links of interest:
	/// * http://www.selfadsi.org/ads-attributes/user-userAccountControl.htm
	/// * https://learn.microsoft.com/en-us/troubleshoot/windows-server/identity/useraccountcontrol-manipulate-account-properties
	/// </summary>
	[Flags]
	public enum UserAccountControlFlagsForMsAD
	{
		SCRIPT = 0b_0000_0001 /*1*/,
		ACCOUNTDISABLE = 0b_0000_0010 /*2*/,
		HOMEDIR_REQUIRED = 0b_0000_1000 /*8*/,
		LOCKOUT = 0b_0001_0000 /*16*/,
		PASSWD_NOTREQD = 0b_0010_0000 /*32*/,
		PASSWD_CANT_CHANGE = 0b_0100_0000,
		ENCRYPTED_TEXT_PWD_ALLOWED = 0b_1000_0000,
		TEMP_DUPLICATE_ACCOUNT = 0b_0001_0000_0000,
		NORMAL_ACCOUNT = 0b_0010_0000_0000,
		INTERDOMAIN_TRUST_ACCOUNT = 0b_1000_0000_0000,
		WORKSTATION_TRUST_ACCOUNT = 0b_0001_0000_0000_0000,
		SERVER_TRUST_ACCOUNT = 0b_0010_0000_0000_0000,
		DONT_EXPIRE_PASSWORD = 0b_0001_0000_0000_0000_0000,
		MNS_LOGON_ACCOUNT = 0b_0010_0000_0000_0000_0000,
		SMARTCARD_REQUIRED = 0b_0100_0000_0000_0000_0000,
		TRUSTED_FOR_DELEGATION = 0b_1000_0000_0000_0000_0000,
		NOT_DELEGATED = 0b_0001_0000_0000_0000_0000_0000,
		USE_DES_KEY_ONLY = 0b_0010_0000_0000_0000_0000_0000,
		DONT_REQ_PREAUTH = 0b_0100_0000_0000_0000_0000_0000,
		PASSWORD_EXPIRED = 0b_1000_0000_0000_0000_0000_0000,
		TRUSTED_TO_AUTH_FOR_DELEGATION = 0b_0001_0000_0000_0000_0000_0000_0000,
		PARTIAL_SECRETS_ACCOUNT = 0b_0100_0000_0000_0000_0000_0000_0000
	}
}
