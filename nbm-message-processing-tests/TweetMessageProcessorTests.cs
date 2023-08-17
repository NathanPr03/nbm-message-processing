using Moq;
using nbm_message_processing.Business_Logic.Message_Processors;

namespace nbm_message_processing_tests;

public class TweetMessageProcessorTests
{
    private readonly Mock<IMessageSplitterService> _mockMessageSplitterService;
    private readonly Mock<ITextSpeakReplacer> _mockTextSpeakReplacer;
    private readonly TweetMessageProcessor _processor;
    
    public TweetMessageProcessorTests()
    {
        _mockMessageSplitterService = new Mock<IMessageSplitterService>();
        _mockTextSpeakReplacer = new Mock<ITextSpeakReplacer>();
        _processor = new TweetMessageProcessor(_mockMessageSplitterService.Object, _mockTextSpeakReplacer.Object);
    }

    [Fact]
    public void Process_WithMentions_ShouldCountMentions()
    {
        // Arrange
        string header = "Test Header";
        string body = "Sender: @Nathan Hello @user1, I hope @user2 will also see this tweet.";
        _mockMessageSplitterService.Setup(m => m.ExtractSender(It.IsAny<string>())).Returns("@test");
        _mockMessageSplitterService.Setup(m => m.ExtractMessageText(It.IsAny<string>())).Returns(body);

        // Act
        var result = _processor.Process(header, body);

        // Assert
        Assert.Equal(1, _processor.GetMentions()["@user1"]);
        Assert.Equal(1, _processor.GetMentions()["@user2"]);
    }

    [Fact]
    public void Process_WithHashtags_ShouldCountHashtags()
    {
        // Arrange
        string header = "Test Header";
        string body = "#coding is fun, especially #csharp coding!";
        _mockMessageSplitterService.Setup(m => m.ExtractSender(It.IsAny<string>())).Returns("@test");
        _mockMessageSplitterService.Setup(m => m.ExtractMessageText(It.IsAny<string>())).Returns(body);

        // Act
        var result = _processor.Process(header, body);

        // Assert
        Assert.Equal(1, _processor.GetHashtags()["#coding"]);
        Assert.Equal(1, _processor.GetHashtags()["#csharp"]);
    }

    [Fact]
    public void Process_WithInvalidSender_ShouldThrowArgumentException()
    {
        // Arrange
        string header = "Test Header";
        string body = "Hello World!";
        _mockMessageSplitterService.Setup(m => m.ExtractSender(It.IsAny<string>())).Returns("test"); // no '@'
        _mockMessageSplitterService.Setup(m => m.ExtractMessageText(It.IsAny<string>())).Returns(body);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _processor.Process(header, body));
    }

    [Fact]
    public void Process_WithLongSender_ShouldThrowArgumentException()
    {
        // Arrange
        string header = "Test Header";
        string body = "Hello World!";
        _mockMessageSplitterService.Setup(m => m.ExtractSender(It.IsAny<string>())).Returns("@verylongusernamehere"); // > 16 characters
        _mockMessageSplitterService.Setup(m => m.ExtractMessageText(It.IsAny<string>())).Returns(body);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _processor.Process(header, body));
    }

    [Fact]
    public void Process_WithLongMessage_ShouldThrowArgumentException()
    {
        // Arrange
        string header = "Test Header";
        string body = new string('a', 1029); // > 1028 characters
        _mockMessageSplitterService.Setup(m => m.ExtractSender(It.IsAny<string>())).Returns("@test");
        _mockMessageSplitterService.Setup(m => m.ExtractMessageText(It.IsAny<string>())).Returns(body);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _processor.Process(header, body));
    }
}