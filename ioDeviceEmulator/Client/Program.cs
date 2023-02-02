using Grpc.Net.Client.Web;
using Grpc.Net.Client;
using ioDeviceEmulator.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ioDeviceEmulator.Shared;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.VisibleStateDuration = 4000;
    config.SnackbarConfiguration.HideTransitionDuration = 200;
    config.SnackbarConfiguration.ShowTransitionDuration = 200;
});

builder.Services.AddSingleton(services =>
{
    var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
    var backendUrl = services.GetRequiredService<NavigationManager>().BaseUri;
    var channel = GrpcChannel.ForAddress(backendUrl, new GrpcChannelOptions { HttpClient = httpClient });

    return new ProtoDeviceStateService.ProtoDeviceStateServiceClient(channel);
});

builder.Services.AddSingleton(services =>
{
    var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
    httpClient.Timeout = Timeout.InfiniteTimeSpan;
    var backendUrl = services.GetRequiredService<NavigationManager>().BaseUri;
    var channel = GrpcChannel.ForAddress(backendUrl, new GrpcChannelOptions { HttpClient = httpClient });

    return new ProtoIOEventsStreamService.ProtoIOEventsStreamServiceClient(channel);
});

builder.Services.AddSingleton(services =>
{
    var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
    var backendUrl = services.GetRequiredService<NavigationManager>().BaseUri;
    var channel = GrpcChannel.ForAddress(backendUrl, new GrpcChannelOptions { HttpClient = httpClient });

    return new ProtoChangeDeviceIOStatusService.ProtoChangeDeviceIOStatusServiceClient(channel);
});

await builder.Build().RunAsync();

