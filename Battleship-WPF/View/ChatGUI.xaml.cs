using System;
using System.Windows;
using System.Windows.Controls;
using Battleship_WPF.EventArguments.Chat;
using Battleship_WPF.Interfaces;
using Battleship_WPF.ViewModel;

namespace Battleship_WPF.View
{
    /// <summary>
    /// Interaction logic for ChatGUI.xaml
    /// </summary>
    public partial class ChatGUI : UserControl
    {
        public ChatViewModel ChatViewModel;
        public ChatGUI()
        {
            InitializeComponent();
            ChatViewModel = new ChatViewModel();
            DataContext = ChatViewModel;
            chatLog.TextChanged += (sender, args) => chatLog.ScrollToEnd();
        }
    }
}
