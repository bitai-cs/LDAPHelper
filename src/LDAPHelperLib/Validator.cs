using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using LdapHelperDTO;

namespace LdapHelperLib
{
	public class Validator : BaseHelper
	{
		#region Constructors
		public Validator(LdapClientConfiguration clientConfiguration) : base(clientConfiguration)
		{
		}

		public Validator(LdapConnectionInfo serverSettings, string baseDN, LdapUserCredentials userCredentials) : base(serverSettings, baseDN, userCredentials) { }
		#endregion



		/// <summary>
		/// Verifica para una cuenta, si en su lista de grupos, existe un determinado grupo
		/// </summary>
		/// <param name="samAccountName">Cuenta que se evaluará. No se debe usar comodines para lograr la búsqueda exacta</param>
		/// <param name="groupName">Grupo que se verificará</param>
		/// <returns></returns>
		public async Task<bool> CheckGroupMembershipForSAMAccountName(string samAccountName, string groupName)
		{
			if (string.IsNullOrEmpty(samAccountName))
				throw new ArgumentException("Debe de proporcionar el parametro samAccountName.");
			if (samAccountName.Contains("*"))
				throw new ArgumentException("El parametro samAccountName no debe de contener caracteres comodines *.");

			var _searcher = new Searcher(this.ConnectionInfo, this.BaseDN, this.UserCredentials);

			var _ldapEntries = await _searcher.SearchUsersAndGroupsByAttributeAsync(EntryAttribute.sAMAccountName, samAccountName, RequiredEntryAttributes.OnlyMemberOf);

			if (_ldapEntries.Count().Equals(0))
				throw new LdapHelperLib.LdapEntryNotFoundException($"No se encontró la cuenta {samAccountName}. No se puede realizar la operación.");

			var _entry = _ldapEntries.First();
			var _found = _entry.memberOf.Where(s => s.Equals(groupName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
			if (string.IsNullOrEmpty(_found))
				return false;
			else
				return true;
		}
	}
}