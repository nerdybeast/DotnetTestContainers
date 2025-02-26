using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Testcontainers.Azurite;

namespace FunctionalTests;

[TestFixture]
public class Tests
{
    private IContainer AzuriteContainerResource { get; set; } = null!;

    [OneTimeSetUp]
    public async Task Setup()
    {
        //AzuriteContainerResource = new ContainerBuilder()
        //        .WithImage("mcr.microsoft.com/azure-storage/azurite:latest")
        //        .WithName("azurite")
        //        .WithEntrypoint("azurite")
        //        .WithCommand("--blobHost", "0.0.0.0", "--queueHost", "0.0.0.0", "--tableHost", "0.0.0.0")
        //        .WithPortBinding(10000, 10000)
        //        .WithPortBinding(10001, 10001)
        //        .WithPortBinding(10002, 10002)
        //        .WithWaitStrategy(Wait.ForUnixContainer()
        //            .UntilMessageIsLogged("Blob service is successfully listening")
        //            .UntilMessageIsLogged("Queue service is successfully listening")
        //            .UntilMessageIsLogged("Table service is successfully listening")
        //        )
        //        .Build();

        AzuriteContainerResource = new AzuriteBuilder()
            .WithName("azurite")
            .WithPortBinding(10000, 10000)
            .WithPortBinding(10001, 10001)
            .WithPortBinding(10002, 10002)
            .Build();

        await AzuriteContainerResource.StartAsync();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await AzuriteContainerResource.DisposeAsync();
    }

    [Test]
    public async Task Test1()
    {
        await Task.Delay(2000);
        Assert.Pass();
    }
}