using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.DTO
{
	/// <summary>
	/// Represents domain-account credentials in <c>Domain\\Account</c> format.
	/// </summary>
	public class LDAPDomainAccountCredential : ISecureCloningCredential<LDAPDomainAccountCredential>
	{
		/// <summary>
		/// Gets or sets the domain name.
		/// </summary>
		public string DomainName { get; set; }

		/// <summary>
		/// Gets or sets the account (user) name.
		/// </summary>
		public string AccountName { get; set; }

		/// <summary>
		/// Gets the composite domain-account name in <c>Domain\\Account</c> format.
		/// </summary>
		[IgnoreDataMember]
		public string DomainAccountName { get => $"{DomainName}\\{AccountName}"; }

		/// <summary>
		/// Gets or sets the account password.
		/// </summary>
		public string DomainAccountPassword { get; set; }



		/// <summary>
		/// Default constructor
		/// </summary>
		public LDAPDomainAccountCredential()
		{
			//Do not remove this constructor, it is required to deserialize data.
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="domainName">Domain name</param>
		/// <param name="username">Username</param>
		/// <param name="domainAccountPassword">Account password.</param>
		/// <exception cref="InvalidOperationException">Constructor exception.</exception>
		public LDAPDomainAccountCredential(string domainName, string username, string domainAccountPassword)
		{
			if (string.IsNullOrEmpty(domainName))
				throw new InvalidOperationException("The domain name must be specified.");

			if (string.IsNullOrEmpty(username))
				throw new InvalidOperationException("The username must be specified.");


			DomainName = domainName;
			AccountName = username;
			DomainAccountPassword = domainAccountPassword;
		}



		/// <summary>
		/// Creates a clone with password removed.
		/// </summary>
		/// <returns>A cloned credential with sensitive information sanitized.</returns>
		public LDAPDomainAccountCredential SecureClone()
		{
			return new LDAPDomainAccountCredential {
				DomainName = this.DomainName, 
				AccountName = this.AccountName,
				DomainAccountPassword =  null
			};
		}
	}
}
