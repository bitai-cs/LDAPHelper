﻿using System.Net;
using System;

namespace Bitai.LDAPHelper.DTO;

/// <summary>
/// Represents the status of the authentication process for the 
/// credentials of a user account.
/// </summary>
public class LDAPDistinguishedNameAuthenticationResult: LDAPOperationResult
{
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

	public LDAPDistinguishedNameAuthenticationResult(LDAPDistinguishedNameCredential credential, bool isAuthenticated = true, string requestLabel = null, bool isSuccessfulOperation = true) : base(requestLabel, isSuccessfulOperation)
	{
		Credential = credential;
		IsAuthenticated = isAuthenticated;
	}

	public LDAPDistinguishedNameAuthenticationResult(LDAPDistinguishedNameCredential credential, string operationMessage, Exception exception, string requestLabel = null) : base(operationMessage, exception, requestLabel)
	{
		Credential = credential;
		IsAuthenticated = false;
	}
}