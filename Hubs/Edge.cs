using achieve_ADagent.AD;
using achieve_lib.AD;
using achieve_lib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
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

			connection = new HubConnectionBuilder()
				.WithUrl($"{edgeAddress}internal", HttpTransportType.WebSockets)
				.WithAutomaticReconnect()
				.ConfigureLogging(logging =>
				{
					logging.SetMinimumLevel(LogLevel.Information);
					logging.AddConsole();
				})
				.Build();

			RegisterHandlers();
			RegisterService();
		}

		public async static void RegisterService()
		{
			await connection.StartAsync();
			await connection.InvokeAsync("Register", Auth.KEY, AD.Manage.DOMAIN);
		}

		private static void RegisterHandlers()
		{
			connection.Closed += async (error) =>
			{
				await Task.Delay(new Random().Next(0, 5) * 1000);
				await connection.StartAsync();
				await connection.InvokeAsync("Register", Auth.KEY, AD.Manage.DOMAIN);
			};

			connection.Reconnecting += error =>
			{
				Debug.Assert(connection.State == HubConnectionState.Reconnecting);


				return Task.CompletedTask;
			};

			connection.Reconnected += async (connectionId) =>
			{
				Debug.Assert(connection.State == HubConnectionState.Connected);

				await connection.InvokeAsync("Register", Auth.KEY, AD.Manage.DOMAIN);
			};

			connection.On<EdgeResponse>("RegisterResponse", new Action<EdgeResponse>(OnRegister));
			connection.On<ADAuthRequest>("GetUserInfo", new Action<ADAuthRequest>(OnADRequest));

		}

		private static void OnRegister(EdgeResponse response)
		{
			Console.WriteLine(response.message);
			if (response.status != StatusCodes.Status200OK)
			{
				Environment.Exit(1);
			}
		}

		private async static void OnADRequest(ADAuthRequest request)
		{
			try
			{
				ADUser user = Users.getUserInfo(request.Username, request.Password);
				request.Answer = user;
				request.IsSuccess = true;
			} catch (Exception ex)
			{
				request.IsSuccess = false;
				request.Error = ex.Message;
			}
			await connection.InvokeAsync("UserInfo", request);

		}
	}
}
