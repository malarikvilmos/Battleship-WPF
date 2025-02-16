using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship_WPF.ViewModel.Base;

namespace Battleship_WPF.ViewModel
{
    internal class InfoPanelGUIViewModel : BaseViewModel
    {
        private bool greenArrowVisibility = true;

        public bool GreenArrowVisibility { get => greenArrowVisibility; set { greenArrowVisibility = value; OnPropertyChanged(); } }

        private bool redArrowVisibility;

        public bool RedArrowVisibility { get => redArrowVisibility; set { redArrowVisibility = value; OnPropertyChanged(); } }

        public void changeVisibility(bool enabled)
        {
            GreenArrowVisibility = enabled;
            RedArrowVisibility = !enabled;
        }
    }
}
