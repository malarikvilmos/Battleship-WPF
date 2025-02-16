using System.Windows.Media;

namespace Battleship_WPF.Utils
{
    public static class ColorChanger
    {
        public static Color DarkeningColor(Color current, int darkeningLevel)
        {
            int R = current.R;
            int G = current.G;
            int B = current.B;

            if (darkeningLevel > 0)
            {
                R = R + darkeningLevel > 255 ? 255 : R + darkeningLevel;
                G = G + darkeningLevel > 255 ? 255 : G + darkeningLevel;
                B = B + darkeningLevel > 255 ? 255 : B + darkeningLevel;
            }
            else
            {
                R = R + darkeningLevel < 0 ? 0 : R + darkeningLevel;
                G = G + darkeningLevel < 0 ? 0 : G + darkeningLevel;
                B = B + darkeningLevel < 0 ? 0 : B + darkeningLevel;
            }

            return Color.FromRgb((byte)R, (byte)G, (byte)B);
        }
    }
}
