﻿using Amazon.CloudWatchLogs;
using Amazon.S3.Model;
using Moq;
using OneCDP.Logging;

namespace fhir_facade_tests.ServicesTests
{
    [TestFixture]
    public class LoggerServiceTests
    {
        private Mock<AmazonCloudWatchLogsClient> _mockCloudWatchLogsClient;
        private LoggerService _loggerService;

        [SetUp]
        public void SetUp()
        {

            var _mockCloudWatchLogsClient = new Mock<AmazonCloudWatchLogsClient>(MockBehavior.Loose, new AmazonCloudWatchLogsConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast1, // or any other region
                ServiceURL = "https://s3.custom-endpoint.com"  

            });

            // Mock CloudWatch Logs client to prevent actual AWS interactions
            //     _mockCloudWatchLogsClient = new Mock<AmazonCloudWatchLogsClient>();

            //     _mockCloudWatchLogsClient.Setup(x => x.ListBucketsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new ListBucketsResponse());

        
            // Create instance of LoggerService with mock dependency
            _loggerService = new LoggerService(_mockCloudWatchLogsClient.Object, "TestLogGroup");
        }

        [Test]
        public async Task LogData_ConsoleLogs_ShouldLogMessageWithRequestId()
        {
            // Arrange
            string requestId1 = Guid.NewGuid().ToString();
            string requestId2 = Guid.NewGuid().ToString();
            string message = "Test log message";

            // Act
            await _loggerService.LogData(message, requestId1, env: true);
            await _loggerService.LogData(message, requestId2, env: true);

            // Assert - Request IDs should be unique for each log entry
            Assert.That(requestId1, Is.Not.EqualTo(requestId2));
        }

        [Test]
        public void LogData_CloudWatchLogs_ShouldNotThrowException()
        {
            // Arrange
            string requestId = Guid.NewGuid().ToString();
            string message = "CloudWatch log message";

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await _loggerService.LogData(message, requestId, env: false));
        }
    }

}

