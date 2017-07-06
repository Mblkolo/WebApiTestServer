using NUnit.Framework;

namespace WebAPI.Tests.Infrastructure
{
 
    public class InMemoryTestBase
    {
        protected ApiServer ApiServer;
        protected ApiClient ApiClient;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ApiServer = new ApiServer();
            var requester = ApiServer.GetRequester();
            ApiClient = new ApiClient(requester);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            ApiServer.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            ApiServer.Reset();
        }
    }
}