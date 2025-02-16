using System;
using System.Windows.Media;
using Battleship_WPF.EventArguments.Chat;
using Battleship_WPF.Utils;
using Battleship_WPF.ViewModel.Base;

namespace Battleship_WPF.ViewModel
{
    public class ChatViewModel : BaseViewModel
    {
        string chatMessages, chatInput;
        private SolidColorBrush foregroundColor;
        public SolidColorBrush ForegroundColor { get => foregroundColor; set { foregroundColor = value; OnPropertyChanged(); } }
        public RelayCommand SendCommand { get; }
        public string ChatMessages { get => chatMessages; set { chatMessages = value; OnPropertyChanged(); } }
        public string ChatInput { get => chatInput; set { chatInput = value; OnPropertyChanged(); } }
        public event EventHandler<SendMessageEventArgs> OnSendMessage;
        public ChatViewModel()
        {
            SendCommand = new RelayCommand(send);
            if (Settings.getBackgroundColor() == Color.FromRgb(37, 57, 66))
                foregroundColor = new SolidColorBrush(Color.FromRgb(52, 235, 174));
            else
                foregroundColor = new SolidColorBrush(Colors.Black);
        }

        private void send()
        {
            if (ChatInput != String.Empty)
            {
                OnSendMessage?.Invoke(null, new SendMessageEventArgs(ChatInput));
                ChatInput = String.Empty;
            }
        }

        internal void addMessage(string sender, string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            ChatMessages += "[" + time + "] [" + sender + "] : " + message + "\n";
        }
    }
}
