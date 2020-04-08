using Newtonsoft.Json;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices;
using System.Linq;
using System.Threading.Tasks;

namespace achieve_ADagent.Models
{
	public class ADUserModel
	{
		public ADUserModel(string username, string displayName, string name, string surname, string principalName)
		{
			Username = username;
			DisplayName = displayName;
			FirstName = name;
			Surname = surname;
			PrincipalName = principalName;
		}

		public ADUserModel(SearchResult user)
		{
			Groups = new List<string>();
			DisplayName = user.Properties["displayName"][0].ToString();
			Username = user.Properties["sAMAccountName"][0].ToString();
			FirstName = user.Properties["givenname"][0].ToString();
			Surname = user.Properties["sn"][0].ToString();
			PrincipalName = user.Properties["userPrincipalName"][0].ToString();
			foreach (var group in user.Properties["memberOf"])
			{
				Groups.Add(group.ToString());
			}

		}

		[Required]
		[JsonRequired]
		[JsonProperty("username")]
		public string Username { get; set; }

		[Required]
		[JsonRequired]
		[JsonProperty("display_name")]
		string DisplayName { get; set; }

		[Required]
		[JsonRequired]
		[JsonProperty("givenname")]
		string FirstName { get; set; }

		[Required]
		[JsonRequired]
		[JsonProperty("surname")]
		string Surname { get; set; }

		[Required]
		[JsonRequired]
		[JsonProperty("principal_name")]
		string PrincipalName { get; set; }

		[Required]
		[JsonRequired]
		[JsonProperty("groups")]
		List<string> Groups { get; set; }
	}
}
