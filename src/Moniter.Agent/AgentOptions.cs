namespace Moniter.Agent;
public sealed record AgentOptions(string EnvironmentName, string DiagnosticLevel)
{
 public static AgentOptions Load(IConfiguration configuration) {
  var name = configuration["Moniter:Environment"]?.Trim() ?? "Development";
  var level = configuration["Moniter:DiagnosticLevel"]?.Trim() ?? "Information";
  if (name.Length is < 1 or > 40) throw new InvalidOperationException("Moniter:Environment must contain between 1 and 40 characters.");
  var allowed = new[] { "Trace", "Debug", "Information", "Warning", "Error", "Critical", "None" };
  if (!allowed.Contains(level, StringComparer.OrdinalIgnoreCase)) throw new InvalidOperationException("Moniter:DiagnosticLevel must be a valid logging level.");
  return new AgentOptions(name, level);
 }
}
