using System;
using System.IO;
using Magneto;
using Magneto.Configuration;
using Magneto.Microsoft;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;
using Samples.Domain;
using Samples.Infrastructure;

namespace Samples
{
	public class Startup
	{
		public Startup(IWebHostEnvironment environment, IConfiguration configuration)
		{
			Environment = environment;
			Configuration = configuration;
			InitializeAlbums();
		}

		void InitializeAlbums()
		{
			using var streamReader = new StreamReader(Environment.ContentRootFileProvider.GetFileInfo(Album.AllAlbumsFilename).CreateReadStream());
			File.WriteAllText(Path.Combine(Environment.WebRootPath, Album.AllAlbumsFilename), streamReader.ReadToEnd());
		}

		public IConfiguration Configuration { get; }

		protected IWebHostEnvironment Environment { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddApplicationInsightsTelemetry(Configuration);

			// Here we add the Microsoft memory cache and our associated cache store.
			services.AddMemoryCache();
			services.AddSingleton<ICacheStore<MemoryCacheEntryOptions>, MemoryCacheStore>();

			// Here we add the Microsoft distributed cache and our associated cache store with it's associated serializer. Normally
			// we'd only have one type of cache in an application, but this is a sample application so we've got both here as examples.
			services.AddDistributedMemoryCache();
			services.AddSingleton<IStringSerializer, JsonConvertStringSerializer>();
			services.AddSingleton<ICacheStore<DistributedCacheEntryOptions>, DistributedCacheStore>();

			// Here we add a decorator object which performs exception logging and timing telemetry for all our Magneto operations.
			services.AddSingleton<IDecorator, ApplicationInsightsDecorator>();

			// Here we add the three context objects with which our queries and commands are executed. The first two are actually
			// used together in a ValueTuple and are resolved as such by a special wrapper around IServiceProvider.
			services.AddSingleton(Environment.WebRootFileProvider);
			services.AddSingleton(new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver(),
				Formatting = Formatting.Indented
			});
			services.AddHttpClient<JsonPlaceHolderHttpClient>()
				.AddHttpMessageHandler(() => new EnsureSuccessHandler())
				.AddTransientHttpErrorPolicy(x => x.WaitAndRetryAsync(new[]
				{
					TimeSpan.FromSeconds(1),
					TimeSpan.FromSeconds(5),
					TimeSpan.FromSeconds(10)
				}));
			
			// Here we add Magneto.IMagneto as the main entry point for consumers, because it can do everything. We could also add any of
			// the interfaces which Magneto.IMagneto is comprised of, to enable exposing limited functionality to some consumers.
			// Internally, Magneto.Magneto relies on Magneto.IMediary to do it's work, so we could also add that or any of the interfaces
			// it's comprised of in order to take control of passing the context when executing queries or commands.
			services.AddTransient<IMagneto, Magneto.Magneto>();

			// Here we specify how cache keys are created. This is optional as there is already a default built-in method,
			// but consumers may want to use their own method instead.
			CachedQuery.UseKeyCreator((prefix, varyBy) => $"{prefix}.{JsonConvert.SerializeObject(varyBy)}");

			services.AddControllersWithViews().SetCompatibilityVersion(CompatibilityVersion.Latest);
		}

		public void Configure(IApplicationBuilder app)
		{
			if (Environment.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();

			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
