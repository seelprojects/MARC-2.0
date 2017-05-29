using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MARC2.Enums;

namespace MARC2.Extensions
{
    public static class HexToColor
    {
        public static Brush Hex2Color(AppColors color)
        {
            string hexValue = "#FFFFFF";
            switch (color)
            {
                case AppColors.Teal:
                    hexValue = "#FF30B89F";
                    break;
                case AppColors.Magenta:
                    hexValue = "#FFB83061";
                    break;
                case AppColors.Peach:
                    hexValue = "#FFDC7C1B";
                    break;
                default:
                    break;
            }
            var converter = new System.Windows.Media.BrushConverter();
            var brush = (Brush)converter.ConvertFromString(hexValue);
            return brush;
        }
    }
}
