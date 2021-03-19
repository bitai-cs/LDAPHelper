using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace LDAPHelper.DTO
{
	public class LdhSearchResult
	{
		public LdhSearchResult(string customTag)
		{
			CustomTag = customTag;
		}


		public string CustomTag { get; private set; }

		public IEnumerable<LDAPHelper.DTO.LdhEntry> Entries { get; set; }

		[IgnoreDataMember]
		public Exception ErrorObject { get; private set; }

		public string ErrorType { get; private set; }

		public string ErrorMessage { get; private set; }

		public bool HasErrorInfo { get => ErrorObject != null; }


		public void SetError(Exception exception)
		{
			ErrorObject = exception;
			ErrorType = exception.GetType().FullName;
			ErrorMessage = exception.Message;
		}
	}
}