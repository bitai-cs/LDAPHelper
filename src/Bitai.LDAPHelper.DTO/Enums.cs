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
        objectSid
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
