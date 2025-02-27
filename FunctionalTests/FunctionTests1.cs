using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using System.Text;
using Testcontainers.Azurite;

namespace FunctionalTests;

[TestFixture]
public class FunctionalTests1
{
    private INetwork NetworkResource { get; set; } = null!;
    private AzuriteContainer AzuriteContainer { get; set; } = null!;
    private IContainer MainContainer { get; set; } = null!;
    private QueueClient SampleQueueClient { get; set; } = null!;

    [OneTimeSetUp]
    public async Task Setup()
    {
        NetworkResource = new NetworkBuilder().Build();
        await NetworkResource.CreateAsync();

        const string azuriteContainerName = "azurite";
        const string accountName = AzuriteBuilder.AccountName;
        const string accountKey = AzuriteBuilder.AccountKey;

        AzuriteContainer = new AzuriteBuilder()
            .WithImage("mcr.microsoft.com/azure-storage/azurite:3.33.0")
            .WithName(azuriteContainerName)
            .WithNetwork(NetworkResource)
            .WithPortBinding(10000, 10000)
            .WithPortBinding(10001, 10001)
            .WithPortBinding(10002, 10002)
            .Build();

        await AzuriteContainer.StartAsync();
        await Task.Delay(1000 * 5); // Wait for Azurite to start up and be ready to accept requests.

        // Only good in the unit test context, cannot be passed in and used in another container.
        string localAzuriteConnectionString = AzuriteContainer.GetConnectionString();
        //string localAzuriteConnectionString = "UseDevelopmentStorage=true";

        SampleQueueClient = new(localAzuriteConnectionString, "samplequeueitems");
        Azure.Response createQueueResponse = await SampleQueueClient.CreateIfNotExistsAsync();
        Console.WriteLine($"Queue creation response code: {createQueueResponse?.Status}");

        static string generateEndpoint(int port) => $"http://{azuriteContainerName}:{port}/{accountName}";
        string publicAzuriteConnectionString = $"DefaultEndpointsProtocol=http;AccountName={accountName};AccountKey={accountKey};BlobEndpoint={generateEndpoint(10000)};QueueEndpoint={generateEndpoint(10001)};TableEndpoint={generateEndpoint(10002)};"; 

        MainContainer = new ContainerBuilder()
            .WithImage("dotnet-test-containers")
            .WithNetwork(NetworkResource)
            .WithEnvironment("AzureWebJobsStorage", publicAzuriteConnectionString)
            .WithEnvironment("FUNCTIONS_WORKER_RUNTIME", "dotnet-isolated")
            .Build();

        await MainContainer.StartAsync();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await AzuriteContainer.DisposeAsync();
        await MainContainer.DisposeAsync();
        await NetworkResource.DisposeAsync();
    }

    [Test]
    public async Task Test1()
    {
        Guid claimId = Guid.NewGuid();

        string queueMessage = @$"{{
            ""ClaimId"": ""{claimId}""
        }}";

        byte[] queueMessageBytes = Encoding.UTF8.GetBytes(queueMessage);
        string encodedQueueMessage = Convert.ToBase64String(queueMessageBytes);

        Azure.Response<SendReceipt> sendMessageResponse = await SampleQueueClient.SendMessageAsync(encodedQueueMessage);
        Console.WriteLine($"Created message Id: {sendMessageResponse.Value.MessageId}");

        await Task.Delay(1000 * 15);
    }
}