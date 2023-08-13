using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            string returnedMessage = String.Empty;
            
            try
            {
                returnedMessage = _messageHandlerFacade.AddMessage(header, message);
            }
            catch (ArgumentException exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            

            MessageOutputTextBox.Text = returnedMessage;
        }

        private void EndSessionButton_Click(object sender, RoutedEventArgs e)
        {
            var sirDtoList = _messageHandlerFacade.GetSirList();
            if (sirDtoList.Count > 0)
            {
                MessageBox.Show(ConvertSirDtoListToReadableString(sirDtoList), "SIR List", MessageBoxButton.OK);
            }
            
            var mentions = _messageHandlerFacade.GetMentions();
            if (mentions.Count > 0)
            {
                MessageBox.Show(ConvertTupleListToReadableString(mentions), "Tweet Mentions", MessageBoxButton.OK);
            }
            
            var hashtags = _messageHandlerFacade.GetHashtags();
            if (hashtags.Count > 0)
            {
                MessageBox.Show(ConvertTupleListToReadableString(hashtags), "Tweet Hashtags", MessageBoxButton.OK);
            }

            _messageHandlerFacade.WriteToJsonOnSessionFinish();
            Application.Current.Shutdown();
        }

        
        private string ConvertSirDtoListToReadableString(List<SirDto> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var dto in list)
            {
                sb.AppendLine($"Date: {dto.Date}");
                sb.AppendLine($"Sort Code: {dto.SortCode}");
                sb.AppendLine($"Incident Nature: {dto.IncidentNature}");
                sb.AppendLine("-------------------------");
            }
            return sb.ToString().Trim();
        }

        private string ConvertTupleListToReadableString(List<Tuple<string, int>> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var tuple in list)
            {
                sb.AppendLine($"{tuple.Item1}: {tuple.Item2} mentions");
            }
            return sb.ToString();
        }

    }
}