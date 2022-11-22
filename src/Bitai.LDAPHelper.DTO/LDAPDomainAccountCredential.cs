using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.DTO
{
	public class LDAPDomainAccountCredential : ISecureCloningCredential<LDAPDomainAccountCredential>
	{
		public string DomainName { get; set; }
		public string AccountName { get; set; }
		[IgnoreDataMember]
		public string DomainAccountName { get => $"{DomainName}\\{AccountName}"; }
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
		/// <param name="domainName">Domain name.</param>
		/// <param name="accountName">Account name.</param>
		/// <param name="domainAccountPassword">Account password.</param>
		/// <exception cref="InvalidOperationException">Constructor exception.</exception>
		public LDAPDomainAccountCredential(string domainName, string accountName, string domainAccountPassword)
		{
			if (string.IsNullOrEmpty(domainName))
				throw new InvalidOperationException("The domain name must be specified.");

			if (string.IsNullOrEmpty(accountName))
				throw new InvalidOperationException("The account name must be specified.");


			DomainName = domainName;
			AccountName = accountName;
			DomainAccountPassword = domainAccountPassword;
		}



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
