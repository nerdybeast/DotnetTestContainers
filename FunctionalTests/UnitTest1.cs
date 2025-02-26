namespace FunctionalTests;

public class Tests
{
    [OneTimeSetUp]
    public void Setup()
    {
    }

    [OneTimeTearDown]
    public void TearDown()
    {
    }

    [Test]
    public async Task Test1()
    {
        await Task.Delay(2000);
        Assert.Pass();
    }
}