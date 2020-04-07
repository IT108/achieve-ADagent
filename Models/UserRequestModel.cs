using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;


namespace achieve_ADagent.Models
{
	public class UserRequestModel
	{
		
		[Required]
		[JsonRequired]
		[JsonProperty("username")]
		public string Username { get; set; }

		[Required]
		[JsonRequired]
		[JsonProperty("password")]
		public string Password { get; set; }
	}
}
