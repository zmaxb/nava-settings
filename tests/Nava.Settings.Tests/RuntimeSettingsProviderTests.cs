using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Nava.Settings.Abstractions;
using Nava.Settings.Infrastructure;

namespace Nava.Settings.Tests;

public class RuntimeSettingsProviderTests
{
    [Fact]
    public async Task InitializeAsync_LoadsSettingsFromStore()
    {
        var settings = new TestSettings { Value = "from_db" };

        var store = new Mock<ISettingsStore>();
        store.Setup(x => x.GetAsync<TestSettings>())
            .ReturnsAsync(settings);

        var sp = new ServiceCollection()
            .AddScoped(_ => store.Object)
            .BuildServiceProvider();

        var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();

        var provider = new RuntimeSettingsProvider<TestSettings>(
            Options.Create(new TestSettings { Value = "default" }),
            scopeFactory,
            Mock.Of<ILogger<RuntimeSettingsProvider<TestSettings>>>()
        );

        await provider.InitializeAsync();

        provider.Settings.Value.Should().Be("from_db");
    }

    [Fact]
    public async Task UpdateAsync_UpdatesSettings_AndFiresEvent()
    {
        var store = new Mock<ISettingsStore>();

        var sp = new ServiceCollection()
            .AddScoped(_ => store.Object)
            .BuildServiceProvider();

        var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();

        var provider = new RuntimeSettingsProvider<TestSettings>(
            Options.Create(new TestSettings { Value = "default" }),
            scopeFactory,
            Mock.Of<ILogger<RuntimeSettingsProvider<TestSettings>>>()
        );

        TestSettings? changed = null;

        provider.SettingsChanged += s =>
        {
            changed = s;
            return Task.CompletedTask;
        };

        var newSettings = new TestSettings { Value = "updated" };

        await provider.UpdateAsync(newSettings);

        provider.Settings.Value.Should().Be("updated");
        changed.Should().NotBeNull();
        changed!.Value.Should().Be("updated");
    }

    [Fact]
    public async Task UpdateAsync_AwaitsSettingsChangedHandlers()
    {
        var store = new Mock<ISettingsStore>();

        var sp = new ServiceCollection()
            .AddScoped(_ => store.Object)
            .BuildServiceProvider();

        var provider = new RuntimeSettingsProvider<TestSettings>(
            Options.Create(new TestSettings { Value = "default" }),
            sp.GetRequiredService<IServiceScopeFactory>(),
            Mock.Of<ILogger<RuntimeSettingsProvider<TestSettings>>>()
        );

        var firstCompleted = false;
        var secondCompleted = false;

        provider.SettingsChanged += async _ =>
        {
            await Task.Delay(10);
            firstCompleted = true;
        };

        provider.SettingsChanged += async _ =>
        {
            await Task.Delay(10);
            secondCompleted = true;
        };

        await provider.UpdateAsync(new TestSettings { Value = "updated" });

        firstCompleted.Should().BeTrue();
        secondCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task JsonSettingsStore_SaveAndLoad_ShouldRoundtrip()
    {
        var repo = new Mock<IConfigurationRepository>();

        string? saved = null;

        repo.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string>((_, v) => saved = v);

        repo.Setup(x => x.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(() => saved);

        var store = new JsonSettingsStore(
            repo.Object,
            new JsonSerializerOptions(),
            Mock.Of<ILogger<JsonSettingsStore>>()
        );

        var settings = new TestSettings { Value = "hello" };

        await store.SaveAsync(settings);
        var loaded = await store.GetAsync<TestSettings>();

        loaded!.Value.Should().Be("hello");
    }

    [Fact]
    public async Task InitializeAsync_UsesDefault_WhenStoreEmpty()
    {
        var store = new Mock<ISettingsStore>();
        store.Setup(x => x.GetAsync<TestSettings>())
            .ReturnsAsync((TestSettings?)null);

        var sp = new ServiceCollection()
            .AddScoped(_ => store.Object)
            .BuildServiceProvider();

        var provider = new RuntimeSettingsProvider<TestSettings>(
            Options.Create(new TestSettings { Value = "default" }),
            sp.GetRequiredService<IServiceScopeFactory>(),
            Mock.Of<ILogger<RuntimeSettingsProvider<TestSettings>>>()
        );

        await provider.InitializeAsync();

        provider.Settings.Value.Should().Be("default");
    }
}