using System.Reflection;
namespace Moniter.Api.Infrastructure;
public static class AppVersion { public static string Value => Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3) ?? "0.1.0"; }
