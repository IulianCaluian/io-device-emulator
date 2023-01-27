using ioDeviceEmulator.Server.GrpcServices;
using ioDeviceEmulator.Server.Repo;
using Microsoft.AspNetCore.ResponseCompression;
using MudBlazor.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();
        builder.Services.AddGrpc();
        builder.Services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                new[] { "application/octet-stream" });
        });
        builder.Services.AddSingleton<WeatherForecastService>();
        builder.Services.AddSingleton<DeviceState>();
        builder.Services.AddSingleton<DeviceStateService>();
        builder.Services.AddMudServices();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();


        app.MapRazorPages();
        app.MapControllers();
        app.MapFallbackToFile("index.html");

        app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<WeatherForecastService>();
            endpoints.MapGrpcService<DeviceStateService>();
        });

        app.Run();
    }
}