using System;
using Moq;
using nbm_message_processing.Business_Logic.Message_Processors;
using Xunit;
using Assert = Xunit.Assert;

namespace nbm_message_processing.Tests;

public class EmailMessageProcessorTests
{
    private readonly Mock<MessageSplitterService> _mockSplitterService;

    public EmailMessageProcessorTests()
    {
        _mockSplitterService = new Mock<MessageSplitterService>();
    }

    [Fact]
    public void Process_ShouldQuarantineUrls()
    {
        string body = "Hello, check out http://example.com";
        string expectedMessage = "Hello, check out <URL Quarantined>";
            
        // Setup mock
        _mockSplitterService.Setup(m => m.ExtractSender(body)).Returns("test@example.com");
        _mockSplitterService.Setup(m => m.ExtractSubject(body)).Returns("Test Subject");
        _mockSplitterService.Setup(m => m.ExtractMessageText(body)).Returns(body);
            
        var processor = new EmailMessageProcessor(_mockSplitterService.Object);
        var result = processor.Process("Header Test", body);

        Assert.Equal("Email", result.Item1);
        Assert.Equal(expectedMessage, result.Item2);
    }

    [Fact]
    public void Process_WithSirSubject_ShouldAddToSirList()
    {
        string body = "SIR 01/01/2023\n12345678 Fraud";
            
        // Setup mock
        _mockSplitterService.Setup(m => m.ExtractSender(body)).Returns("test@example.com");
        _mockSplitterService.Setup(m => m.ExtractSubject(body)).Returns("SIR 01/01/2023");
        _mockSplitterService.Setup(m => m.ExtractMessageText(body)).Returns(body);

        var processor = new EmailMessageProcessor(_mockSplitterService.Object);
        processor.Process("Header Test", body);

        var sirList = processor.GetSirList();
        Assert.Single(sirList);
        Assert.Equal("01/01/2023", sirList[0].Date);
        Assert.Equal("12345678", sirList[0].SortCode);
        Assert.Equal("Fraud", sirList[0].IncidentNature);
    }

    [Fact]
    public void Process_WithLongSubject_ShouldThrowArgumentException()
    {
        string body = "Hello, this is a test.";
            
        // Setup mock
        _mockSplitterService.Setup(m => m.ExtractSender(body)).Returns("test@example.com");
        _mockSplitterService.Setup(m => m.ExtractSubject(body)).Returns("This is a very long subject which exceeds the limit");
        _mockSplitterService.Setup(m => m.ExtractMessageText(body)).Returns(body);

        var processor = new EmailMessageProcessor(_mockSplitterService.Object);

        Assert.Throws<ArgumentException>(() => processor.Process("Header Test", body));
    }
}