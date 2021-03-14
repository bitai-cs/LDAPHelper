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
		public Validator(string requestTag, LdapClientConfiguration clientConfiguration) : base(requestTag, clientConfiguration)
		{
		}

		public Validator(string requestTag, LdapConnectionPipeline connectionPipeline, LdapServerSettings serverSettings, string baseDN, bool useGC, LdapUserCredentials userCredentials) : base(requestTag, connectionPipeline, serverSettings, baseDN, useGC, userCredentials) { }
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

			var _searcher = new Searcher(this.RequestTag, this.ConnectionPipeline, this.ServerSettings, this.BaseDN, this.UseGC, this.UserCredentials);
			////VBG: A parte del constructor, se vuelven asignar las propiedades
			//var _searcher = new Searcher(this.DomainProfile, this.ConnectionPipeline, this.ServerSettings, this.BaseDN, this.UserCredentials) {
			//	ServerSettings = this.ServerSettings,
			//	UserCredentials = this.UserCredentials,
			//	BaseDN = this.BaseDN
			//};
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