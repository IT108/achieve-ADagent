using achieve_ADagent.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace achieve_ADagent.Hubs
{
	public static class Edge
	{
		private static string edgeAddress = null;

		private static string EDGE_ADDRESS_PROPERTY = "EDGE_ADDRESS";
		private static HubConnection connection;

		public static string EDGE_ADDRESS
		{
			get {
				return edgeAddress;
			}
			set {
				if (value is null)
					throw new NotImplementedException("Master key not defined");
				if (!(edgeAddress is null))
					throw new MemberAccessException("Double key declaration");
				edgeAddress = value;
			}
		}

		public static void ConfigureEdge(IConfiguration config)
		{
			EDGE_ADDRESS = config[EDGE_ADDRESS_PROPERTY];
			RegisterService();
		}

		public async static void RegisterService()
		{
			connection = new HubConnectionBuilder()
				.WithUrl($"{edgeAddress}internal")
				.Build();

			connection.Closed += async (error) =>
			{
				await Task.Delay(new Random().Next(0, 5) * 1000);
				await connection.StartAsync();
			};

			connection.On<EdgeResponse>("RegisterResponse", new Action<EdgeResponse>(OnRegister));

			await connection.StartAsync();
			await connection.InvokeAsync("Register", Auth.KEY, AD.Manage.DOMAIN);
		}
		private static void OnRegister(EdgeResponse response)
		{
			if (response.status != StatusCodes.Status200OK)
				throw new ApplicationException(response.message);
			Console.WriteLine(response.message);
		}
	}
}
