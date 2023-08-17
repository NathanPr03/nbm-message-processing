using Moq;
using nbm_message_processing.Business_Logic.Message_Processors;

namespace nbm_message_processing_tests;

public class EmailMessageProcessorTests
{
    private readonly Mock<IMessageSplitterService> _mockMessageSplitterService;
    private readonly EmailMessageProcessor _processor;

    public EmailMessageProcessorTests()
    {
        _mockMessageSplitterService = new Mock<IMessageSplitterService>();
        _processor = new EmailMessageProcessor(_mockMessageSplitterService.Object);
    }

    [Fact]
    public void Process_WithSirSubject_ShouldAddToSirList()
    {
        // Arrange
        string header = "Test Header";
        string body = "test body with http://test.com link";
        _mockMessageSplitterService.Setup(m => m.ExtractSender(It.IsAny<string>())).Returns("test@test.com");
        _mockMessageSplitterService.Setup(m => m.ExtractSubject(It.IsAny<string>())).Returns("SIR 17/08/23");
        _mockMessageSplitterService.Setup(m => m.ExtractMessageText(It.IsAny<string>())).Returns(body);

        // Act
        var result = _processor.Process(header, body);

        // Assert
        Assert.Single(_processor.GetSirList());
    }

    [Fact]
    public void Process_WithURL_ShouldQuarantineUrl()
    {
        // Arrange
        string header = "Test Header";
        string body = "test body with http://test.com link";
        _mockMessageSplitterService.Setup(m => m.ExtractSender(It.IsAny<string>())).Returns("test@test.com");
        _mockMessageSplitterService.Setup(m => m.ExtractSubject(It.IsAny<string>())).Returns("Test Subject");
        _mockMessageSplitterService.Setup(m => m.ExtractMessageText(It.IsAny<string>())).Returns(body);

        // Act
        var result = _processor.Process(header, body);

        // Assert
        Assert.Contains("<URL Quarantined>", result.Item2);
    }

    [Fact]
    public void Process_WithLongSubject_ShouldThrowArgumentException()
    {
        // Arrange
        string header = "Test Header";
        string body = "test body";
        _mockMessageSplitterService.Setup(m => m.ExtractSender(It.IsAny<string>())).Returns("test@test.com");
        _mockMessageSplitterService.Setup(m => m.ExtractSubject(It.IsAny<string>())).Returns(new string('a', 21)); // length > 20
        _mockMessageSplitterService.Setup(m => m.ExtractMessageText(It.IsAny<string>())).Returns(body);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _processor.Process(header, body));
    }
}