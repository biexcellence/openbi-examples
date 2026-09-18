using BiExcellence.OpenBi.Server.Database.Abstractions;
using Ibssolution.biox.Repositoryserver;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CustomDatabaseExample;

// An IHostedService is the place to create or update own tables, because it runs once while the
// open bi server starts.
public sealed class CustomDatabaseMigration : IHostedService
{
    private readonly IEntityDatabaseFactory _entityDatabaseFactory;
    private readonly ILogger<CustomDatabaseMigration> _logger;

    public CustomDatabaseMigration(IEntityDatabaseFactory entityDatabaseFactory, ILogger<CustomDatabaseMigration> logger)
    {
        _entityDatabaseFactory = entityDatabaseFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var database = _entityDatabaseFactory.GetDatabase();

        // Creates the table, and adds the columns which do not exist yet
        await database.MigrateAsync<CustomEntity>(cancellationToken);

        _logger.LogInformation("CUSTOM_ENTITY migrated");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

// Endpoints which read and write the own table
public sealed class CustomDatabaseEndpoints : IOpenBiStartup
{
    private readonly IEntityDatabaseFactory _entityDatabaseFactory;

    public CustomDatabaseEndpoints(IEntityDatabaseFactory entityDatabaseFactory)
    {
        _entityDatabaseFactory = entityDatabaseFactory;
    }

    public void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/customdatabase");

        group.Map("/list", async context =>
        {
            var cancellationToken = context.RequestAborted;

            await using var database = _entityDatabaseFactory.GetDatabase();

            var selectOptions = new SelectOptions();
            selectOptions.OrderBy.Add(new OrderByField(Identifier.Entity<CustomEntity>.Column(e => e.Created), OrderByDirection.Desc));
            selectOptions.Count = 50;

            // Filter with the "name" query parameter, if it is present
            if (context.Request.Query.TryGetValue("name", out var name))
            {
                selectOptions.Where = Where.AndGroup().Equal(Identifier.Entity<CustomEntity>.Column(e => e.Name), name.ToString());
            }

            var result = await database.SelectAsync<CustomEntity>(selectOptions, cancellationToken);

            foreach (var entity in result)
            {
                await context.Response.WriteAsync($"{entity.Created:s} {entity.Id} {entity.Name}\n", cancellationToken);
            }
        });

        group.Map("/add", async context =>
        {
            var cancellationToken = context.RequestAborted;

            var entity = new CustomEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = context.Request.Query["name"].ToString(),
                Created = DateTimeOffset.UtcNow,
            };

            await using var database = _entityDatabaseFactory.GetDatabase();

            // Use a transaction when several statements have to succeed together
            await using var transaction = await database.TransactionAsync(cancellationToken);

            // Inserts the entity, or updates it when a row with the same key already exists
            await transaction.SaveAsync(entity, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            await context.Response.WriteAsync($"Saved {entity.Id}", cancellationToken);
        });

        group.Map("/delete", async context =>
        {
            var cancellationToken = context.RequestAborted;

            await using var database = _entityDatabaseFactory.GetDatabase();

            var where = Where.AndGroup().Equal(Identifier.Entity<CustomEntity>.Column(e => e.Id), context.Request.Query["id"].ToString());

            var deleted = await database.DeleteAsync<CustomEntity>(where, cancellationToken);

            await context.Response.WriteAsync($"Deleted {deleted}", cancellationToken);
        });
    }
}
