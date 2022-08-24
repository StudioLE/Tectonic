using System.Diagnostics;
using Lineweights.Dashboard.Hubs;
using Lineweights.Dashboard.Scripts;
using Lineweights.Dashboard.States;
using Lineweights.Workflows.Execution;
using Lineweights.Workflows.Results;
using Microsoft.AspNetCore.ResponseCompression;

namespace Lineweights.Dashboard;

public class Program
{
    private readonly WebApplicationBuilder _builder;
    private Process? _azurite;

    private Program(string[] args)
    {
        _builder = WebApplication.CreateBuilder(args);
        InjectServices();
        StartAzurite();
        RunApp();
        StopAzurite();
    }

   private void InjectServices()
    {
        // Add Blazor services to the container.
        _builder.Services.AddRazorPages();
        _builder.Services.AddServerSideBlazor();

        // Inject runner
        _builder.Services.AddScoped<IActivityRunner, ActivityRunner>();

        // Inject
        _builder.Services.AddScoped<ResultsState>();
        _builder.Services.AddScoped<ModelViewerFacade>();
        _builder.Services.AddScoped<SignalRState>();
        _builder.Services.AddScoped<ActivityRunnerState>();

        // Add SignalR services
        // https://docs.microsoft.com/en-us/aspnet/core/blazor/tutorials/signalr-blazor?view=aspnetcore-6.0&tabs=visual-studio&pivots=server
        _builder.Services.AddResponseCompression(opts => opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            new[] { "application/octet-stream" }));

        // Use Newtonsoft's Json.NET for signal serialisation
        // https://docs.microsoft.com/en-us/aspnet/core/migration/22-to-30?view=aspnetcore-6.0&tabs=visual-studio#use-newtonsoftjson-in-an-aspnet-core-30-signalr-project
        _builder.Services.AddSignalR().AddNewtonsoftJsonProtocol();
    }

   private void StartAzurite()
   {
       _azurite = Process.Start(new ProcessStartInfo
       {
           FileName = "npm",
           Arguments = "run azurite",
           UseShellExecute = true
       });
       // TODO: Azurite console to logger.
   }

   private void StopAzurite()
   {
       // TODO: Stop azurite
       if (_azurite is null)
           return;
       _azurite.Kill();
       _azurite.WaitForExit();
       _azurite.Dispose();
   }

   private void RunApp()
    {
        WebApplication app = _builder.Build();
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
            app.UseExceptionHandler("/Error");
        app.UseStaticFiles();
        //app.UsePathBase();
        app.UseRouting();
        app.MapBlazorHub();
        app.MapHub<SignalRHub>(SendToDashboard.HubPath);
        app.MapFallbackToPage("/_Host");
        app.Run();
    }

   public static void Main(string[] args)
   {
       Program program = new(args);
   }
}
