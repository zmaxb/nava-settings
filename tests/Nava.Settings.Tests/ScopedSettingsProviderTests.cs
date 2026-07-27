using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Nava.Settings.Abstractions;
using Nava.Settings.Infrastructure;

namespace Nava.Settings.Tests;

public class ScopedSettingsProviderTests
{
    [Fact]
    public async Task GetAsync_LoadsSettingsForSpecifiedScope()
    {
        const string scopeId = "user-42";

        var settings = new TestSettings
        {
            Value = "scoped"
        };

        var store = new Mock<ISettingsStore>();

        store.Setup(x => x.GetAsync<TestSettings>(scopeId))
            .ReturnsAsync(settings);

        var provider = CreateProvider(store.Object);

        var result = await provider.GetAsync(scopeId);

        result.Should().NotBeNull();
        result!.Value.Should().Be("scoped");

        store.Verify(
            x => x.GetAsync<TestSettings>(scopeId),
            Times.Once);
    }

    [Fact]
    public async Task GetAsync_ReturnsNull_WhenScopeDoesNotExist()
    {
        const string scopeId = "missing-user";

        var store = new Mock<ISettingsStore>();

        store.Setup(x => x.GetAsync<TestSettings>(scopeId))
            .ReturnsAsync((TestSettings?)null);

        var provider = CreateProvider(store.Object);

        var result = await provider.GetAsync(scopeId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_SavesSettingsForSpecifiedScope()
    {
        const string scopeId = "user-42";

        var settings = new TestSettings
        {
            Value = "updated"
        };

        var store = new Mock<ISettingsStore>();
        var provider = CreateProvider(store.Object);

        await provider.UpdateAsync(settings, scopeId);

        store.Verify(
            x => x.SaveAsync(settings, scopeId),
            Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_RemovesSettingsForSpecifiedScope()
    {
        const string scopeId = "user-42";

        var store = new Mock<ISettingsStore>();
        var provider = CreateProvider(store.Object);

        await provider.RemoveAsync(scopeId);

        store.Verify(
            x => x.RemoveAsync<TestSettings>(scopeId),
            Times.Once);
    }

    private static ScopedSettingsProvider<TestSettings> CreateProvider(
        ISettingsStore store)
    {
        var serviceProvider = new ServiceCollection()
            .AddScoped(_ => store)
            .BuildServiceProvider();

        return new ScopedSettingsProvider<TestSettings>(
            serviceProvider.GetRequiredService<IServiceScopeFactory>());
    }
}