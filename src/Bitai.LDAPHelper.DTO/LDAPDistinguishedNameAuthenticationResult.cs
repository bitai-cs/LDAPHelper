using System.Net;
using System;

namespace Bitai.LDAPHelper.DTO;

/// <summary>
/// Represents the status of the authentication process for the 
/// credentials of a user account.
/// </summary>
public class LDAPDistinguishedNameAuthenticationResult: LDAPOperationResult
{
	/// <summary>
	/// Gets or sets the credential associated with this authentication attempt.
	/// </summary>
	public LDAPDistinguishedNameCredential Credential { get; set; }

	/// <summary>
	/// Value indicating whether the account was able to authenticate 
	/// to the LDAP server.
	/// </summary>
	public bool IsAuthenticated { get; set; }




	/// <summary>
	/// Default constructor.
	/// </summary>
	public LDAPDistinguishedNameAuthenticationResult()
	{
		//Do not remove this constructor, it is required to deserialize data.
	}

	/// <summary>
	/// Initializes an authentication result.
	/// </summary>
	/// <param name="credential">Credential associated with the authentication attempt.</param>
	/// <param name="isAuthenticated">Authentication status.</param>
	/// <param name="requestLabel">Optional request label.</param>
	/// <param name="isSuccessfulOperation">Whether the operation is marked successful.</param>
	public LDAPDistinguishedNameAuthenticationResult(LDAPDistinguishedNameCredential credential, bool isAuthenticated = true, string requestLabel = null, bool isSuccessfulOperation = true) : base(requestLabel, isSuccessfulOperation)
	{
		Credential = credential;
		IsAuthenticated = isAuthenticated;
	}

	/// <summary>
	/// Initializes an unsuccessful authentication result from an exception.
	/// </summary>
	/// <param name="credential">Credential associated with the authentication attempt.</param>
	/// <param name="operationMessage">Operation message.</param>
	/// <param name="exception">Underlying error.</param>
	/// <param name="requestLabel">Optional request label.</param>
	public LDAPDistinguishedNameAuthenticationResult(LDAPDistinguishedNameCredential credential, string operationMessage, Exception exception, string requestLabel = null) : base(operationMessage, exception, requestLabel)
	{
		Credential = credential;
		IsAuthenticated = false;
	}
}
