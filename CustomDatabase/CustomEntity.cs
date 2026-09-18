using BiExcellence.OpenBi.Server.Database.Abstractions;

namespace CustomDatabaseExample;

// An own table in the open bi database. The table is created and updated by MigrateAsync,
// see CustomDatabaseMigration.
[TableName("CUSTOM_ENTITY")]
[Index(nameof(Name))]
public sealed class CustomEntity : Entity
{
    [Column("ID", Size = 36), Key, DefaultValue(DefaultValue.Guid)]
    public string Id { get => GetValue<string>()!; set => SetValue(value); }

    [Column("NAME"), DefaultValue]
    public string? Name { get => GetValue<string>(); set => SetValue(value); }

    [Column("CREATED"), DefaultValue(DefaultValue.CurrentTimestamp)]
    public DateTimeOffset Created { get => GetValue<DateTimeOffset>(); set => SetValue(value); }
}
