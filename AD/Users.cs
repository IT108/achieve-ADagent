using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Windows;
using achieve_ADagent.Models;
using Novell.Directory.Ldap;

namespace achieve_ADagent.AD
{
	public static class Users
	{



		public static bool AuthenticateUser(string userName, string password)
		{
			bool ret = false;

			try
			{
				getUserInfo(userName, password);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				ret = false;
			}

			return ret;
		}

		public static ADUserModel getUserInfo(string userName, string password)
		{
			DirectorySearcher ds = null;

			DirectoryEntry de = GetDirectoryEntry(userName, password);
			DirectorySearcher dsearch = new DirectorySearcher(de);
			SearchResult result = null;

			ds = new DirectorySearcher(de);
			// Full Name
			ds.PropertiesToLoad.Add("displayName");
			// First Name
			ds.PropertiesToLoad.Add("givenname");
			// Last Name (Surname)
			ds.PropertiesToLoad.Add("sn");
			// Login Name
			ds.PropertiesToLoad.Add("userPrincipalName");
			//
			ds.PropertiesToLoad.Add("sAMAccountName");
			// Distinguished Name
			ds.PropertiesToLoad.Add("distinguishedName");
			
			//connect string
			ds.PropertiesToLoad.Add("userPrincipalName");
			//grups
			ds.PropertiesToLoad.Add("memberOf");

			ds.Filter = $"(&(objectClass=user)(sAMAccountName={userName}))";

			result = ds.FindOne();

			if (!(result is null))
			{
				return new ADUserModel(result);
			}
			throw new NotImplementedException("User not found or some parameter not exists");
		}

		public static DirectoryEntry GetDirectoryEntry(string userName, string password)
		{
			DirectoryEntry de = new DirectoryEntry();
			de.Path = $"LDAP://{Manage.DOMAIN}/{Manage.DOMAIN_PATH}";
			de.Username = $"{Manage.DOMAIN}\\{userName}";
			de.Password = password;
			return de;
		}
	}
}
