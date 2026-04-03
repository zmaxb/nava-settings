using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nava.Settings.Infrastructure;

public class ConfigurationEntry
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public sealed class ConfigurationEntryConfiguration
    : IEntityTypeConfiguration<ConfigurationEntry>
{
    public void Configure(EntityTypeBuilder<ConfigurationEntry> builder)
    {
        builder.ToTable("ConfigurationEntries");

        builder.HasKey(x => x.Key);

        builder.Property(x => x.Key)
            .IsRequired();

        builder.Property(x => x.Value)
            .IsRequired();
    }
}