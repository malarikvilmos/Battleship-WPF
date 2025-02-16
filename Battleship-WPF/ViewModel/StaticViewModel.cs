using System;
using System.ComponentModel;
using System.Windows.Media;
using Battleship_WPF.Network;
using Battleship_WPF.Utils;

namespace Battleship_WPF.ViewModel
{
    public static class StaticViewModel
    {
        public static event EventHandler SelectedColorChanged;

        private static Color selectedColor = Settings.getBackgroundColor();
        public static Color SelectedColor 
        { 
            get => selectedColor;
            set 
            {
                selectedColor = value;
                Settings.setBackgroundColor(selectedColor);
                Settings.Save();
                if (SelectedColorChanged != null)
                    SelectedColorChanged(null, EventArgs.Empty);
            }
        }

    }
}
