using Moq;
using nbm_message_processing.Business_Logic.Message_Processors;

namespace nbm_message_processing_tests;

public class TextMessageProcessorTests
{
    [Fact]
    public void Test_Process_ValidTextMessageWithTextSpeak()
    {
        // Arrange
        var header = "TestHeader";
        var body = "+44 1234 567 890 LMAO!";

        var mockSplitterService = new Mock<IMessageSplitterService>();
        var mockTextSpeakReplacer = new Mock<ITextSpeakReplacer>();

        mockSplitterService.Setup(m => m.ExtractSender(body)).Returns("+44 1234 567 890");
        mockSplitterService.Setup(m => m.ExtractMessageText(body)).Returns("LMAO!");
        mockTextSpeakReplacer.Setup(m => m.ReplaceTextSpeak("LMAO!")).Returns("LMAO <Laughing My A** Off>!");

        var processor = new TextMessageProcessor(mockSplitterService.Object, mockTextSpeakReplacer.Object);

        // Act
        var result = processor.Process(header, body);

        // Assert
        Assert.Equal("SMS", result.Item1);
        Assert.Equal("LMAO <Laughing My A** Off>!", result.Item2);
    }

    [Fact]
    public void Test_Process_InvalidSenderNumber()
    {
        // Arrange
        var header = "TestHeader";
        var body = "InvalidNumber LMAO!";

        var mockSplitterService = new Mock<IMessageSplitterService>();
        var mockTextSpeakReplacer = new Mock<ITextSpeakReplacer>();

        mockSplitterService.Setup(m => m.ExtractSender(body)).Returns("InvalidNumber");

        var processor = new TextMessageProcessor(mockSplitterService.Object, mockTextSpeakReplacer.Object);

        // Act and Assert
        Assert.Throws<ArgumentException>(() => processor.Process(header, body));
    }

    [Fact]
    public void Test_Process_InvalidMessageLength()
    {
        // Arrange
        var header = "TestHeader";
        var body = "+44 1234 567 890 " + new string('A', 2000); // 2001 characters

        var mockSplitterService = new Mock<IMessageSplitterService>();
        var mockTextSpeakReplacer = new Mock<ITextSpeakReplacer>();

        mockSplitterService.Setup(m => m.ExtractSender(body)).Returns("+44 1234 567 890");
        mockSplitterService.Setup(m => m.ExtractMessageText(body)).Returns(new string('A', 2000));

        var processor = new TextMessageProcessor(mockSplitterService.Object, mockTextSpeakReplacer.Object);

        // Act and Assert
        Assert.Throws<ArgumentException>(() => processor.Process(header, body));
    }
}