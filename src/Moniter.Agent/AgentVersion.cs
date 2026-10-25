using System.Reflection;

namespace Moniter.Agent;

public static class AgentVersion
{
    public static string Value =>
        Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3) ?? "0.1.0";
}
