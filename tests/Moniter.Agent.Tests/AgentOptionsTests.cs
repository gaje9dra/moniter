using Microsoft.Extensions.Configuration;
using Moniter.Agent;
namespace Moniter.Agent.Tests;
public sealed class AgentOptionsTests {
 [Fact] public void Uses_safe_development_defaults() { var o=AgentOptions.Load(new ConfigurationBuilder().Build()); Assert.Equal("Development",o.EnvironmentName); Assert.Equal("Information",o.DiagnosticLevel); }
 [Fact] public void Rejects_invalid_diagnostic_level() { var c=new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?> { ["Moniter:DiagnosticLevel"]="Verboseish" }).Build(); Assert.Throws<InvalidOperationException>(()=>AgentOptions.Load(c)); }
 [Fact] public void Rejects_overlong_environment_name() { var c=new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?> { ["Moniter:Environment"]=new string('x',41) }).Build(); Assert.Throws<InvalidOperationException>(()=>AgentOptions.Load(c)); }
 [Fact] public void Reports_a_version()=>Assert.False(string.IsNullOrWhiteSpace(AgentVersion.Value));
}
