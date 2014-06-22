using Microsoft.Xna.Framework;
using ScarFly.MyClasses.LevelElementClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.Common
{
    public static class Consts
    {
        public static bool IsVibrate = true;

        public static int PhoneWidth;
        public static int PhoneHeight;

        public static int PhoneWidthRate = 8;
        public static int PhoneHeightRate = 5;

        public static string SF_BaseFont = "Fonts/BaseFont";
        public static string SF_BaseFontBig = "Fonts/BaseFontBig";
        public static string SF_ModifierNotification = "Fonts/ModifierNotification";
        public static string SF_EndGameScore = "Fonts/EndGameScore";

        public static string P_MoneyIcon = "Icons/MoneyIcon";
        public static string P_RankIcon = "Icons/RankIcon";
        public static string P_BestIcon = "Icons/BestIcon";
        public static string P_Pixel = "Player/Pixel";
        public static string P_StartButton = "Buttons/StartButton";
        public static string P_HelpButton = "Buttons/HelpButton";
        public static string P_AboutButton = "Buttons/AboutButton";
        public static string P_NetworkButton = "Buttons/NetworkButton";
        public static string P_ResumeButton = "Buttons/ResumeButton";
        public static string P_VibrateButton = "Buttons/VibrateButton";
        public static string P_Tutorial = "Buttons/Tutorial";
        public static string P_RateButton = "Buttons/RateButton";
        public static string P_Player = "Player/PaperPlane_v2";
        public static string P_Backgrond = "Background/Forest";
        public static string P_ForeBackground = "Background/ForestFore";
        public static string P_Walkplace = "Background/WalkPlace";

        public static Color PastelGreen = new Color(119, 221, 119);
        public static Color PastelRed = new Color(255, 105, 97);
    }
}
