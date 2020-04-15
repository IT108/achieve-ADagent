using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace achieve_ADagent.AD
{
	public static class Manage
	{
		private static string ADMIN_USERNAME_PROPERTY = "ADMIN_USERNAME";
		private static string ADMIN_PASSWORD_PROPERTY = "ADMIN_PASSWORD";
		private static string DOMAIN_PROPERTY = "DOMAIN_NAME";
		private static string DOMAIN_PATH_PROPERTY = "DOMAIN_PATH";
		private static string DOMAIN_SERVER_PROPERTY = "SERVER_ADDRESS";

		private static string adminUsername = null;
		private static string adminPassword = null;
		private static string serverAddress = null;
		private static string domain = null;
		private static string domainPath = null;

		public static string ADMIN_USERNAME
		{
			get {
				return adminUsername;
			}
			set {
				setValue(ref adminUsername, value);
			}
		}
		public static string ADMIN_PASSWORD
		{
			get {
				return adminPassword;
			}
			set {
				setValue(ref adminPassword, value);
			}
		}
		public static string DOMAIN
		{
			get {
				return domain;
			}
			set {
				setValue(ref domain, value);
			}
		}
		public static string DOMAIN_PATH
		{
			get {
				return domainPath;
			}
			set {
				setValue(ref domainPath, value);
			}
		}

		public static string DOMAIN_SERVER_ADDRESS
		{
			get {
				return serverAddress;
			}
			set {
				setValue(ref serverAddress, value);
			}
		}

		private static IConfiguration configuration;

		public static void ConfigureDomain(IConfiguration config)
		{
			configuration = config;
			ADMIN_USERNAME = ADMIN_USERNAME_PROPERTY;
			ADMIN_PASSWORD = ADMIN_PASSWORD_PROPERTY;
			DOMAIN = DOMAIN_PROPERTY;
			DOMAIN_PATH = DOMAIN_PATH_PROPERTY;
			DOMAIN_SERVER_ADDRESS = DOMAIN_SERVER_PROPERTY;
		}

		private static void setValue(ref string dest, string valueName)
		{
			string value = configuration[valueName];

			if (value is null)
				throw new NotImplementedException("Auth key not defined");
			if (!(dest is null))
				throw new MemberAccessException("Double key declaration");

			dest = value;
		}
	}
}
