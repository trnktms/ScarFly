using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses
{
    public enum GameState
    {
        Gaming,
        InMainMenu,
        InPauseMenu,
        InEndGameMenu,
        InTutorial
    }

    public static class Consts
    {
        public static int PhoneWidth;
        public static int PhoneHeight;

        public static int PhoneWidthRate = 8;
        public static int PhoneHeightRate = 5;

        public static string SF_GameScore = "Fonts/GameScore";
        public static string SF_ModifierNotification = "Fonts/ModifierNotification";
        public static string P_MoneyIcon = "Icons/MoneyIcon";
        public static string P_RankIcon = "Icons/RankIcon";
        public static string P_Pixel = "Player/Pixel";

        public static Color PastelGreen = new Color(119, 221, 119);
        public static Color PastelRed = new Color(255, 105, 97);
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

    public static class Transitions
    {
        public static int TransitionCounter = 0;
        public static bool IsTransition = true;
        public static void Transition(ref Color color)
        {
            if (IsTransition)
            {
                if (TransitionCounter >= 254) { IsTransition = false; TransitionCounter = 0; return; }
                color = new Color(TransitionCounter, TransitionCounter, TransitionCounter);
                TransitionCounter += 5;
            }
        }
    }

    public static class Tutorial
    {
        public static bool FirstStart()
        {
            bool result;
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                result = myIsolatedStorage.FileExists("TotalScore.txt");
                if (!result) { myIsolatedStorage.CreateFile("TotalScore.txt"); }
            }
            return result;
        }
    }
}
