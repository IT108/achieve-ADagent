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

		public static string KEY
		{
			get {
				return key;
			}
			set {
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
