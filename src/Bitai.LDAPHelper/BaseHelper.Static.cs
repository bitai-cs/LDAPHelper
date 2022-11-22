using System;
using System.Collections.Generic;
using System.Linq;

namespace Bitai.LDAPHelper
{
    public abstract partial class BaseHelper
    {
        #region Private static fields
        private static readonly IEnumerable<string> ObjectSidAndSAMAccountNameAttributeNames;
        private static readonly IEnumerable<string> ObjectSidAttributeName;
        private static readonly IEnumerable<string> CNAttributeName;
        private static readonly IEnumerable<string> MemberAttributeName;
        private static readonly IEnumerable<string> MemberOfAttributeName;
        private static readonly IEnumerable<string> MemberAndMemberOfAttributeNames;

        private static readonly IEnumerable<string> MinimunAttributeNames;
        private static readonly IEnumerable<string> MinimunWithMemberAttributeNames;
        private static readonly IEnumerable<string> MinimunWithMemberOfAttributeNames;
        private static readonly IEnumerable<string> MinimunWithMemberAndMemberOfAttributeNames;

        private static readonly IEnumerable<string> FewAttributeNames;
        private static readonly IEnumerable<string> FewWithMemberAttributeNames;
        private static readonly IEnumerable<string> FewWithMemberOfAttributeNames;
        private static readonly IEnumerable<string> FewWithMemberAndMemberOfAttributeNames;

        private static readonly IEnumerable<string> AllAttributeNames;
        private static readonly IEnumerable<string> AllWithMemberAttributeNames;
        private static readonly IEnumerable<string> AllWithMemberOfAttributeNames;
        private static readonly IEnumerable<string> AllWithMemberAndMemberOfAttributeNames;
        #endregion

        #region Static constructor
        static BaseHelper()
        {
            ObjectSidAndSAMAccountNameAttributeNames = new string[3] { DTO.EntryAttribute.objectSid.ToString(), DTO.EntryAttribute.sAMAccountName.ToString(),
            DTO.EntryAttribute.distinguishedName.ToString()};

            ObjectSidAttributeName = new string[2] { DTO.EntryAttribute.objectSid.ToString(),
            DTO.EntryAttribute.distinguishedName.ToString() };

            CNAttributeName = new string[2] { DTO.EntryAttribute.cn.ToString(),
            DTO.EntryAttribute.distinguishedName.ToString() };

            MemberAttributeName = new string[2] { DTO.EntryAttribute.member.ToString() ,
            DTO.EntryAttribute.distinguishedName.ToString()};

            MemberOfAttributeName = new string[2] { DTO.EntryAttribute.memberOf.ToString() ,
            DTO.EntryAttribute.distinguishedName.ToString()};

            MemberAndMemberOfAttributeNames = MemberAttributeName.Concat(MemberOfAttributeName).Distinct();

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

        #region Private static methods
        private static IEnumerable<string> GetMinimunAttributeNames()
        {
            return new string[] { DTO.EntryAttribute.objectSid.ToString(), DTO.EntryAttribute.distinguishedName.ToString(), DTO.EntryAttribute.sAMAccountName.ToString(), DTO.EntryAttribute.cn.ToString(), DTO.EntryAttribute.displayName.ToString(), DTO.EntryAttribute.objectClass.ToString() };
        }

        private static IEnumerable<string> GetMinimunWithMemberAttributeNames()
        {
            return GetMinimunAttributeNames().Concat(MemberAttributeName).Distinct();
        }

        private static IEnumerable<string> GetMinimunWithMemberOfAttributeNames()
        {
            return GetMinimunAttributeNames().Concat(MemberOfAttributeName).Distinct();
        }

        private static IEnumerable<string> GetMinimunWithMemberAndMemberOfAttributeNames()
        {
            return GetMinimunAttributeNames().Concat(MemberAndMemberOfAttributeNames).Distinct();
        }

        private static IEnumerable<string> GetFewAttributeNames()
        {
            return new string[] { DTO.EntryAttribute.objectSid.ToString(), DTO.EntryAttribute.objectGuid.ToString(), DTO.EntryAttribute.distinguishedName.ToString(), DTO.EntryAttribute.sAMAccountName.ToString(), DTO.EntryAttribute.cn.ToString(), DTO.EntryAttribute.name.ToString(), DTO.EntryAttribute.displayName.ToString(), DTO.EntryAttribute.objectClass.ToString(), DTO.EntryAttribute.objectCategory.ToString() };
        }

        private static IEnumerable<string> GetFewWithMemberAttributeNames()
        {
            return GetFewAttributeNames().Concat(MemberAttributeName).Distinct();
        }

        private static IEnumerable<string> GetFewWithMemberOfAttributeNames()
        {
            return GetFewAttributeNames().Concat(MemberOfAttributeName).Distinct();
        }

        private static IEnumerable<string> GetFewWithMemberAndMemberOfAttributeNames()
        {
            return GetFewAttributeNames().Concat(MemberAttributeName).Concat(MemberOfAttributeName).Distinct();
        }

        private static IEnumerable<string> GetAllAttributeNames()
        {
            return Enum.GetNames(typeof(DTO.EntryAttribute)).Except(new string[] { DTO.EntryAttribute.member.ToString(), DTO.EntryAttribute.memberOf.ToString() });
        }

        private static IEnumerable<string> GetAllWithMemberAttributeNames()
        {
            return GetAllAttributeNames().Concat(MemberAttributeName).Distinct();
        }

        private static IEnumerable<string> GetAllWithMemberOfAttributeNames()
        {
            return GetAllAttributeNames().Concat(MemberOfAttributeName).Distinct();
        }

        private static IEnumerable<string> GetAllWithMemberAndMemberOfAttributeNames()
        {
            return GetAllAttributeNames().Concat(MemberAndMemberOfAttributeNames).Distinct();
        }
        #endregion
    }
}
