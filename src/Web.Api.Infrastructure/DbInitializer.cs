using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Migrations;
namespace Web.Api.Infrastructure;

public class DbInitializer
{
    private readonly AppDbContext _ctx;
    public DbInitializer(AppDbContext ctx) => _ctx = ctx;
    public void DumpPendingChanges()
    {
        // 1. Get EF Core’s internal service provider
        var internalServices = ((IInfrastructure<IServiceProvider>)_ctx).Instance;

        // 2. Resolve the services we need
        var modelInitializer = internalServices.GetRequiredService<IModelRuntimeInitializer>();
        var differ = internalServices.GetRequiredService<IMigrationsModelDiffer>();
        var designTimeModel = internalServices.GetRequiredService<IDesignTimeModel>().Model;

        // 3. Finalize the design-time model & your snapshot
        var finalizedLive = modelInitializer.Initialize(designTimeModel);
        var snapshotRaw = new AppDbContextModelSnapshot().Model;
        var finalizedSnapshot = modelInitializer.Initialize(snapshotRaw);

        // 4. Build the relational models
        var relationalLive = finalizedLive.GetRelationalModel();
        var relationalSnapshot = finalizedSnapshot.GetRelationalModel();

        // 5. Compute the diff
        IReadOnlyList<MigrationOperation> ops = differ.GetDifferences(relationalSnapshot, relationalLive);

        // 6. Dump to Debug
        Debug.WriteLine("=== EF Core PendingModelChanges Diff ===");
        Console.WriteLine("=== EF Core PendingModelChanges Diff ===");
        if (ops.Count == 0)
        {
            Debug.WriteLine("✅ No pending operations — snapshot and model match.");
            Console.WriteLine("✅ No pending operations — snapshot and model match.");
        }
        else
        {
            foreach (var op in ops)
            {
                Debug.WriteLine($"• {op.GetType().Name}: {op}");
                Console.WriteLine($"• {op.GetType().Name}: {op}");
            }
        }
    }
}