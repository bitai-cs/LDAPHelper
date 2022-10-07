using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.DTO
{
	public class DomainAccountCredential
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="domainName">Domain name.</param>
		/// <param name="accountName">Account name.</param>
		/// <param name="domainAccountPassword">Account password.</param>
		/// <exception cref="InvalidOperationException">Constructor exception.</exception>
		public DomainAccountCredential(string domainName, string accountName, string domainAccountPassword)
		{
			if (string.IsNullOrEmpty(domainName))
				throw new InvalidOperationException("The domain name must be specified.");

			if (string.IsNullOrEmpty(accountName))
				throw new InvalidOperationException("The account name must be specified.");


			DomainName = domainName;
			AccountName = accountName;
			DomainAccountPassword = domainAccountPassword;
		}


		public string DomainName { get; }
		public string AccountName { get; }
		public string DomainAccountName { get => $"{DomainName}\\{AccountName}"; }
		public string DomainAccountPassword { get; }
	}
}
