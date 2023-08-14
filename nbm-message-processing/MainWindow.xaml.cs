using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using nbm_message_processing.Business_Logic;
using YamlDotNet.Serialization;

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
            List<SirDto> sirDtoList = _messageHandlerFacade.GetSirList();
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
        
        private void SubmitFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "YML files (*.yml)|*.yml",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string filePath = openFileDialog.FileName;
                
                SubmitFilePath(filePath);
                _messageHandlerFacade.ConvertYmlToJson(filePath);
            }
        }

        private async Task SubmitFilePath(string filePath)
        {
            var deserializer = new DeserializerBuilder().Build();
            using (var reader = new StreamReader(filePath))
            {
                RootDto rootDto = deserializer.Deserialize<RootDto>(reader);
                
                foreach (var message in rootDto.Messages)
                {
                    MessageOutputTextBox.Text = message.MessageText;
                    await Task.Delay(TimeSpan.FromSeconds(3));
                }
                
                StringBuilder sirDtoStringBuilder = new StringBuilder();
                foreach(var dto in rootDto.SirDtos)
                {
                    sirDtoStringBuilder.AppendLine($"Date: {dto.Date}");
                    sirDtoStringBuilder.AppendLine($"Sort Code: {dto.SortCode}");
                    sirDtoStringBuilder.AppendLine($"Incident Nature: {dto.IncidentNature}");
                    sirDtoStringBuilder.AppendLine("-------------------------");
                }
                MessageBox.Show(sirDtoStringBuilder.ToString(), "SIR List", MessageBoxButton.OK);
                
                StringBuilder mentionsStringBuilder = new StringBuilder();
                foreach(var mentions in rootDto.Mentions)
                {
                    mentionsStringBuilder.AppendLine($"{mentions.Key}: {mentions.Value} mentions");
                }
                MessageBox.Show(mentionsStringBuilder.ToString(), "Tweet Mentions", MessageBoxButton.OK);
                
                StringBuilder hashtagsStringBuilder = new StringBuilder();
                foreach(var hashtags in rootDto.Hashtags)
                {
                    hashtagsStringBuilder.AppendLine($"{hashtags.Key}: {hashtags.Value} hashtags");
                }
                MessageBox.Show(hashtagsStringBuilder.ToString(), "Tweet Hashtags", MessageBoxButton.OK);
            }
        }

    }
}