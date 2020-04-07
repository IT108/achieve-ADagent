using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace achieve_ADagent
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers().AddNewtonsoftJson(options => options.UseMemberCasing());
			ConfigureAD();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}

		private void ConfigureAD()
		{
			AD.Manage.DOMAIN = Configuration["DOMAIN_NAME"];
			AD.Manage.ADMIN_PASSWORD = Configuration["ADMIN_PASSWORD"];
			AD.Manage.ADMIN_USERNAME = Configuration["ADMIN_USERNAME"];
			AD.Manage.DOMAIN_PATH = Configuration["DOMAIN_PATH"];
			Auth.KEY = Configuration["AUTH_KEY"];
			Auth.MASTER_KEY = Configuration["MASTER_KEY"];
			if (AD.Manage.DOMAIN == "None" || AD.Manage.ADMIN_PASSWORD == "None" ||
				AD.Manage.ADMIN_USERNAME == "None" || AD.Manage.DOMAIN_PATH == "None")
			{
				throw new NotImplementedException("Domain settings not defined");
			}
		}
	}
}
