using Moniter.Agent;
var builder = Host.CreateApplicationBuilder(args);
builder.Logging.ClearProviders(); builder.Logging.AddJsonConsole();
builder.Services.AddSingleton(AgentOptions.Load(builder.Configuration));
builder.Services.AddHostedService<AgentWorker>();
if (OperatingSystem.IsWindows()) builder.Services.AddWindowsService(options => options.ServiceName = "Moniter Endpoint Agent");
await builder.Build().RunAsync();
