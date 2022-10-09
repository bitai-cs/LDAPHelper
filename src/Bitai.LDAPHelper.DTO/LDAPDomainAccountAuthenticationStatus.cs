using System;

namespace Bitai.LDAPHelper.DTO;

/// <summary>
/// Represents the status of the authentication process for the 
/// credentials of a user account.
/// </summary>
public class LDAPDomainAccountAuthenticationResult: LDAPOperationResult
{
	public LDAPDomainAccountAuthenticationResult(LDAPDomainAccountCredential credential, bool isAuthenticated = true, string requestTag = null, bool isSuccessfulOperation = true) :base(requestTag, isSuccessfulOperation)
	{
		Credential = credential;
		IsAuthenticated = isAuthenticated;
	}

	public LDAPDomainAccountAuthenticationResult(LDAPDomainAccountCredential credential, string operationMessage, Exception exception, string requestTag = null) : base(operationMessage, exception, requestTag)
	{
		Credential = credential;
		IsAuthenticated = false;
	}



	public LDAPDomainAccountCredential Credential { get; }

	/// <summary>
	/// Value indicating whether the account was able to authenticate 
	/// to the LDAP server.
	/// </summary>
	public bool IsAuthenticated { get; set; }
	
}