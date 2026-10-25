using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moniter.Api.Data;

namespace Moniter.Api.Data.Migrations;

[DbContext(typeof(MoniterDbContext))]
public sealed class MoniterDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.13");
        modelBuilder.HasAnnotation("Relational:MaxIdentifierLength", 63);
        MoniterModelConfiguration.Configure(modelBuilder);
    }
}
