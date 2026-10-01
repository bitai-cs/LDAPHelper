using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.DTO
{
    /// <summary>
    /// User account for Microsoft Active Directory (immutable record version)
    ///
    /// Resources of interest:
    /// - https://www.rlmueller.net/Name_Attributes.htm
    /// </summary>
    public record LDAPMsADUserAccount : ISecureCloningCredential<LDAPMsADUserAccount>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public LDAPMsADUserAccount()
        {
            UserAccountControl =
                $"{UserAccountControlFlagsForMsAD.NORMAL_ACCOUNT},{UserAccountControlFlagsForMsAD.DONT_EXPIRE_PASSWORD}";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="distinguishedNameOfContainer">Container distinguished name of user account.</param>
        public LDAPMsADUserAccount(string distinguishedNameOfContainer) : this()
        {
            DistinguishedNameOfContainer = distinguishedNameOfContainer
                ?? throw new ArgumentNullException(nameof(distinguishedNameOfContainer));
        }

        /// <summary>
        /// Constructor that initializes all properties at once. Any parameter left at its
        /// default keeps the same behavior as the parameterless/single-arg constructors above
        /// (e.g. <paramref name="userAccountControl"/> still defaults to NORMAL_ACCOUNT + DONT_EXPIRE_PASSWORD).
        /// Calls with just <paramref name="distinguishedNameOfContainer"/> still resolve to the
        /// simpler constructor above, so existing call sites are unaffected.
        /// </summary>
        public LDAPMsADUserAccount(
            string distinguishedNameOfContainer,
            string? givenName = null,
            string? sn = null,
            string? cn = null,
            string? name = null,
            string? displayName = null,
            string? description = null,
            string? distinguishedName = null,
            string[]? objectClass = null,
            string? samAccountName = null,
            string? userPrincipalName = null,
            string? userAccountControl = null,
            string? department = null,
            string? telephoneNumber = null,
            string? mail = null,
            string? password = null) : this(distinguishedNameOfContainer)
        {
            GivenName = givenName;
            Sn = sn;
            Cn = cn;
            Name = name;
            DisplayName = displayName;
            Description = description;
            DistinguishedName = distinguishedName;
            ObjectClass = objectClass;
            SAMAccountName = samAccountName;
            UserPrincipalName = userPrincipalName;
            Department = department;
            TelephoneNumber = telephoneNumber;
            Mail = mail;
            Password = password;

            // Only overrides the default UAC set by the parameterless constructor
            // when the caller explicitly passed one — preserves current default behavior.
            if (userAccountControl != null)
                UserAccountControl = userAccountControl;
        }

        /// <summary>
        /// Gets the distinguished name of the container where the user account should be created.
        /// </summary>
        public string? DistinguishedNameOfContainer { get; init; }

        /// <summary>
        /// Gets the user's given name.
        /// </summary>
        public string? GivenName { get; init; }

        /// <summary>
        /// Gets the user's surname.
        /// </summary>
        public string? Sn { get; init; }

        /// <summary>
        /// Gets the user common name (CN).
        /// </summary>
        public string? Cn { get; init; }

        /// <summary>
        /// Gets the LDAP <c>name</c> attribute.
        /// </summary>
        public string? Name { get; init; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        public string? DisplayName { get; init; }

        /// <summary>
        /// Gets the account description.
        /// </summary>
        public string? Description { get; init; }

        /// <summary>
        /// Gets the full distinguished name of the user account.
        /// </summary>
        public string? DistinguishedName { get; init; }

        /// <summary>
        /// Gets LDAP object classes for this account.
        /// </summary>
        public string[]? ObjectClass { get; init; }

        /// <summary>
        /// Gets the sAMAccountName value.
        /// </summary>
        public string? SAMAccountName { get; init; }

        /// <summary>
        /// Gets the user principal name (UPN).
        /// </summary>
        public string? UserPrincipalName { get; init; }

        /// <summary>
        /// Gets user-account-control flags as a comma-separated list of
        /// <see cref="UserAccountControlFlagsForMsAD"/> names.
        /// Validated eagerly via the <c>init</c> accessor so an invalid value still fails fast,
        /// the same way the original setter did.
        /// </summary>
        public string? UserAccountControl
        {
            get => userAccountControl;
            init
            {
                userAccountControl = value;
                userAccountControlFlags = ParseFlags(value);
            }
        }
        private readonly string? userAccountControl;

        /// <summary>
        /// Gets the department.
        /// </summary>
        public string? Department { get; init; }

        /// <summary>
        /// Gets the phone number.
        /// </summary>
        public string? TelephoneNumber { get; init; }

        /// <summary>
        /// Gets the email address.
        /// </summary>
        public string? Mail { get; init; }

        /// <summary>
        /// Gets the account password.
        /// </summary>
        public string? Password { get; init; }

        /// <summary>
        /// Gets parsed account-control flags derived from <see cref="UserAccountControl"/>.
        /// </summary>
        public UserAccountControlFlagsForMsAD? UserAccountControlFlags => userAccountControlFlags;
        private readonly UserAccountControlFlagsForMsAD? userAccountControlFlags;

        private static UserAccountControlFlagsForMsAD? ParseFlags(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            int totalFlagValue = 0;
            foreach (var flagName in value.Split(','))
            {
                if (!Enum.TryParse<UserAccountControlFlagsForMsAD>(flagName.Trim(), out var parsedFlag))
                    throw new InvalidCastException(
                        $"Unable to assign property {nameof(UserAccountControl)}. Can not parse {flagName} to {nameof(UserAccountControlFlagsForMsAD)}");

                totalFlagValue += (int)parsedFlag;
            }

            return (UserAccountControlFlagsForMsAD)Enum.ToObject(typeof(UserAccountControlFlagsForMsAD), totalFlagValue);
        }

        /// <summary>
        /// Creates a secure clone with password masked.
        /// </summary>
        /// <returns>A cloned account suitable for logging/transport.</returns>
        public LDAPMsADUserAccount SecureClone() =>
            this with
            {
                Password = "*****",
                ObjectClass = (string[]?)ObjectClass?.Clone()
            };
    }


    ///// <summary>
    ///// User account for Microsoft Active Directory (immutable record version)
    /////
    ///// Resources of interest:
    ///// - https://www.rlmueller.net/Name_Attributes.htm
    ///// </summary>
    //public record LDAPMsADUserAccount : ISecureCloningCredential<LDAPMsADUserAccount>
    //{
    //    /// <summary>
    //    /// Default constructor
    //    /// </summary>
    //    public LDAPMsADUserAccount()
    //    {
    //        UserAccountControl =
    //            $"{UserAccountControlFlagsForMsAD.NORMAL_ACCOUNT},{UserAccountControlFlagsForMsAD.DONT_EXPIRE_PASSWORD}";
    //    }

    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    /// <param name="distinguishedNameOfContainer">Container distinguished name of user account.</param>
    //    public LDAPMsADUserAccount(string distinguishedNameOfContainer) : this()
    //    {
    //        DistinguishedNameOfContainer = distinguishedNameOfContainer
    //            ?? throw new ArgumentNullException(nameof(distinguishedNameOfContainer));
    //    }

    //    /// <summary>
    //    /// Gets the distinguished name of the container where the user account should be created.
    //    /// </summary>
    //    public string? DistinguishedNameOfContainer { get; init; }

    //    /// <summary>
    //    /// Gets the user's given name.
    //    /// </summary>
    //    public string? GivenName { get; init; }

    //    /// <summary>
    //    /// Gets the user's surname.
    //    /// </summary>
    //    public string? Sn { get; init; }

    //    /// <summary>
    //    /// Gets the user common name (CN).
    //    /// </summary>
    //    public string? Cn { get; init; }

    //    /// <summary>
    //    /// Gets the LDAP <c>name</c> attribute.
    //    /// </summary>
    //    public string? Name { get; init; }

    //    /// <summary>
    //    /// Gets the display name.
    //    /// </summary>
    //    public string? DisplayName { get; init; }

    //    /// <summary>
    //    /// Gets the account description.
    //    /// </summary>
    //    public string? Description { get; init; }

    //    /// <summary>
    //    /// Gets the full distinguished name of the user account.
    //    /// </summary>
    //    public string? DistinguishedName { get; init; }

    //    /// <summary>
    //    /// Gets LDAP object classes for this account.
    //    /// </summary>
    //    public string[]? ObjectClass { get; init; }

    //    /// <summary>
    //    /// Gets the sAMAccountName value.
    //    /// </summary>
    //    public string? SAMAccountName { get; init; }

    //    /// <summary>
    //    /// Gets the user principal name (UPN).
    //    /// </summary>
    //    public string? UserPrincipalName { get; init; }

    //    /// <summary>
    //    /// Gets user-account-control flags as a comma-separated list of
    //    /// <see cref="UserAccountControlFlagsForMsAD"/> names.
    //    /// Validated eagerly via the <c>init</c> accessor so an invalid value still fails fast,
    //    /// the same way the original setter did.
    //    /// </summary>
    //    public string? UserAccountControl
    //    {
    //        get => userAccountControl;
    //        init
    //        {
    //            userAccountControl = value;
    //            userAccountControlFlags = ParseFlags(value);
    //        }
    //    }
    //    private readonly string? userAccountControl;

    //    /// <summary>
    //    /// Gets the department.
    //    /// </summary>
    //    public string? Department { get; init; }

    //    /// <summary>
    //    /// Gets the phone number.
    //    /// </summary>
    //    public string? TelephoneNumber { get; init; }

    //    /// <summary>
    //    /// Gets the email address.
    //    /// </summary>
    //    public string? Mail { get; init; }

    //    /// <summary>
    //    /// Gets the account password.
    //    /// </summary>
    //    public string? Password { get; init; }

    //    /// <summary>
    //    /// Gets parsed account-control flags derived from <see cref="UserAccountControl"/>.
    //    /// </summary>
    //    public UserAccountControlFlagsForMsAD? UserAccountControlFlags => userAccountControlFlags;
    //    private readonly UserAccountControlFlagsForMsAD? userAccountControlFlags;

    //    private static UserAccountControlFlagsForMsAD? ParseFlags(string? value)
    //    {
    //        if (string.IsNullOrEmpty(value))
    //            return null;

    //        int totalFlagValue = 0;
    //        foreach (var flagName in value.Split(','))
    //        {
    //            if (!Enum.TryParse<UserAccountControlFlagsForMsAD>(flagName.Trim(), out var parsedFlag))
    //                throw new InvalidCastException(
    //                    $"Unable to assign property {nameof(UserAccountControl)}. Can not parse {flagName} to {nameof(UserAccountControlFlagsForMsAD)}");

    //            totalFlagValue += (int)parsedFlag;
    //        }

    //        return (UserAccountControlFlagsForMsAD)Enum.ToObject(typeof(UserAccountControlFlagsForMsAD), totalFlagValue);
    //    }

    //    /// <summary>
    //    /// Creates a secure clone with password masked.
    //    /// </summary>
    //    /// <returns>A cloned account suitable for logging/transport.</returns>
    //    public LDAPMsADUserAccount SecureClone() =>
    //        this with
    //        {
    //            Password = "*****",
    //            ObjectClass = (string[]?)ObjectClass?.Clone()
    //        };
    //}    
}
