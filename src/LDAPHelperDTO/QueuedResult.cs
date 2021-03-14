using System;
using System.Collections.Generic;
using System.Text;

namespace LdapHelperDTO
{
	public class QueuedResult
	{
		public QueuedResult(string requestTag)
		{
			RequestTag = requestTag;
		}




		public string RequestTag { get; }
		public IEnumerable<LdapHelperDTO.LdapEntry> Entries { get; set; }
		public string ErrorType { get; set; }
		public string ErrorMessage { get; set; }

		public bool HasErrorInfo()
		{
			return (!string.IsNullOrEmpty(ErrorType) && !string.IsNullOrEmpty(ErrorMessage));
		}
	}
}