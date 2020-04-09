using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace achieve_ADagent
{
	public static class Auth
	{
		private static string key = null;
		private static string masterKey = null;
		private const string AUTH_KEY_PROPERTY_NAME = "AUTH_KEY";
		private const string MASTER_KEY_PROPERTY_NAME = "MASTER_KEY";

		public static void ConfigureAuth(IConfiguration config)
		{
			KEY = config[AUTH_KEY_PROPERTY_NAME];
			MASTER_KEY = config[MASTER_KEY_PROPERTY_NAME];
		}

		public static string KEY
		{
			get {
				return key;
			}
			set {
				if (value is null)
					throw new NotImplementedException("Auth key not defined");
				if (!(key is null))
					throw new MemberAccessException("Double key declaration");
				key = value;
			}
		}

		public static string MASTER_KEY
		{
			get {
				return masterKey;
			}
			set {
				if (value is null)
					throw new NotImplementedException("Master key not defined");
				if (!(masterKey is null))
					throw new MemberAccessException("Double key declaration");
				masterKey = value;
			}
		}

		public static bool AuthenticateClient(string clientKey)
		{
			return key == clientKey || masterKey == clientKey;
		}
	}
}
