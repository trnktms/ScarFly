using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses
{
    public enum GameState
    {
        Gaming,
        InMainMenu,
        InScoreMenu,
        InPauseMenu,
        InEndGameMenu
    }

    public static class Consts
    {
        public static int PhoneWidth;
        public static int PhoneHeight;

        public static int PhoneWidthRate = 8;
        public static int PhoneHeightRate = 5;

        public static string SF_GameScore = "Fonts/GameScore";
        public static string P_MoneyIcon = "Barriers/Money";
        public static string P_Pixel = "Player/Pixel";
    }

    public static class LevelSelector
    {
        public static string Select()
        {
            string[] levels = { "level_1", "level_2", "level_3", "level_4", "level_5" };
            Random R = new Random();
            return levels[R.Next(0, levels.Length - 1)];
        }
    }
}
