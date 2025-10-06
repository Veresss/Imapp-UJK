using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace EmptyMauiApp.Services
{
    public static class ThemeManager
    {
        public static event EventHandler<int> ThemeChanged;

        public static int GetCurrentPreset()
        {
            return Preferences.Get("CurrentPreset", 0); //default 0
        }

        public static void SetPreset(int presetId)
        {
            var res = Application.Current.Resources;
            Color primary, secondary;

            switch (presetId)
            {
                case 0: // Alapértelmezett
                    primary = Color.Parse("#FFFFFF");
                    secondary = Color.Parse("#DEA111");
                    res["TitleColor"] = Color.Parse("#DEA111");
                    res["ButtonTextColor"] = Color.Parse("#FFFFFF");
                    res["ButtonColor"] = Color.Parse("#DEA111");

                    res["PrimaryColor"] = Color.Parse("#FFFFFF");
                    res["SecondaryColor"] = Color.Parse("#DEA111");
                    float alpha0 = 96f / 255f;
                    res["PrimaryTransparent"] = new Color(primary.Red, primary.Green, primary.Blue, alpha0);
                    res["SecondaryTransparent"] = new Color(secondary.Red, secondary.Green, secondary.Blue, alpha0);
                    ThemeChanged?.Invoke(null, presetId);
                    break;
                case 1: // Világos
                    primary = Colors.White; 
                    secondary = Colors.Black;
                    res["TitleColor"] = Colors.Black;
                    res["ButtonTextColor"] = Colors.White;
                    res["ButtonColor"] = Colors.Black;

                    res["PrimaryColor"] = Colors.White;
                    res["SecondaryColor"] = Colors.Black;
                    float alpha1 = 96f / 255f;
                    res["PrimaryTransparent"] = new Color(primary.Red, primary.Green, primary.Blue, alpha1);
                    res["SecondaryTransparent"] = new Color(secondary.Red, secondary.Green, secondary.Blue, alpha1);
                    ThemeChanged?.Invoke(null, presetId);
                    return;
                case 2: // Sötét
                    primary = Colors.Black; 
                    secondary = Colors.White;
                    res["TitleColor"] = Colors.White;
                    res["ButtonTextColor"] = Colors.Black;
                    res["ButtonColor"] = Colors.White;

                    res["PrimaryColor"] = Colors.Black;
                    res["SecondaryColor"] = Colors.White;
                    float alphaDark = 96f / 255f;
                    res["PrimaryTransparent"] = new Color(primary.Red, primary.Green, primary.Blue, alphaDark);
                    res["SecondaryTransparent"] = new Color(secondary.Red, secondary.Green, secondary.Blue, alphaDark);
                    ThemeChanged?.Invoke(null, presetId);
                    return;
                default:
                    throw new ArgumentException("Érvénytelen preset ID.");
            }

            //gradient-ekhez
            res["PrimaryColor"] = primary;
            res["SecondaryColor"] = secondary;
            float alphaGrad = 96f / 255f;
            res["PrimaryTransparent"] = new Color(primary.Red, primary.Green, primary.Blue, alphaGrad);
            res["SecondaryTransparent"] = new Color(secondary.Red, secondary.Green, secondary.Blue, alphaGrad);
            res["AccentTextColor"] = secondary;
            res["InsideTextColor"] = primary;
            res["ButtonTextColor"] = Colors.White;

            ThemeChanged?.Invoke(null, presetId); //esemény trigger minden preset-hez
        }
    }
}
