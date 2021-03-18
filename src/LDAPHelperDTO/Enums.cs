using System;
using System.Collections.Generic;
using System.Text;

namespace LdapHelperDTO
{
	public enum EntryAttribute
	{
		/// <summary>
		/// country abrev.
		/// </summary>
		c,
		cn,
		company,
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
		userPrincipalName,
		whenCreated,
		objectGuid,
		objectSid
	}

	public enum EntryKeyAttribute
	{
		objectSid,
		distinguishedName,
		sAMAccountName
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
}
