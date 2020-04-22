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
using System.Threading;

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

		private static async Task<bool> ConnectToSignalRServer()
		{
			Console.WriteLine("Trying to connect");
			bool connected = false;
			try
			{
				await connection.StartAsync();

				if (connection.State == HubConnectionState.Connected)
				{
					connected = true;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
			}
			return connected;
		}

		public async static Task TryConnect()
		{
			while (true)
			{
				bool connected = await ConnectToSignalRServer();
				if (connected)
					return;
				Thread.Sleep(10000);
			}
		}

		public async static void ConfigureEdge(IConfiguration config)
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
			await TryConnect();
			RegisterService();
		}

		public async static Task RegisterService()
		{
			await connection.InvokeAsync("Register", Auth.KEY, AD.Manage.DOMAIN);
		}

		private static void RegisterHandlers()
		{

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

			connection.Closed += error =>
			{
				Debug.Assert(connection.State == HubConnectionState.Disconnected);

				TryConnect().Wait();
				RegisterService().Wait();
				return Task.CompletedTask;
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

		private async static Task OnConnClosed(Exception error)
		{
			await TryConnect();
			await connection.InvokeAsync("Register", Auth.KEY, AD.Manage.DOMAIN);
		}

		private async static void OnADRequest(ADAuthRequest request)
		{
			try
			{
				ADUser user = Users.getUserInfo(request.Username, request.Password);
				request.Result = user;
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
