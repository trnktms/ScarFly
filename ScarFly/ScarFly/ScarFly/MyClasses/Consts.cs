using System;
using System.Collections.Generic;
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
        public static int PhoneWidthRate = 8;
        public static int PhoneHeightRate = 5;

        public static string sf_GameScore = "Fonts/GameScore";
        public static string p_MoneyIcon = "Barriers/Money";
        public static string p_Pixel = "Player/Pixel";
    }
}
