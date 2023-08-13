using System.Windows;
using nbm_message_processing.Business_Logic;

namespace nbm_message_processing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MessageHandlerFacade _messageHandlerFacade = new();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string header = MessageHeaderTextBox.Text;
            string message = MessageTextTextBox.Text;
            string returned_message = _messageHandlerFacade.AddMessage(header, message);
        }

        private void EndSessionButton_Click(object sender, RoutedEventArgs e)
        {
            _messageHandlerFacade.WriteToJsonOnSessionFinish();
            Application.Current.Shutdown();
        }
    }
}