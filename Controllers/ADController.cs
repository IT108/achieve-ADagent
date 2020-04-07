using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using achieve_ADagent.AD;
using achieve_ADagent.Models;
using System.ComponentModel.DataAnnotations;
using System;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace achieve_ADagent.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class ADController : ControllerBase
	{
		[HttpGet]
		public IActionResult Get([FromQuery] [Required] string agent_key,
			[FromQuery] UserRequestModel user)
		{
			if (! Auth.AuthenticateClient(agent_key))
				return StatusCode(StatusCodes.Status401Unauthorized, "Invalid agent key");
			try
			{
				ADUserModel res = Users.getUserInfo(user.Username, user.Password);
				return Ok(res);
			}
			catch
			{
				return StatusCode(StatusCodes.Status204NoContent);
			}
		}
	}
}
