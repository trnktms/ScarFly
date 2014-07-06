using ScarFly.MyClasses.LevelElementClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.Helpers
{
    public static class LevelHelper
    {
        public static bool IsLevelEnd(Moneys moneys)
        {
            return moneys.GetActualMoneyList().Count != 0 && moneys.GetActualMoneyList().LastOrDefault().Index.ID == "!";
        }

        public static string SelectLevel()
        {
            string[] levels = { "level_1", "level_2", "level_3", "level_4", "level_5", "level_6" };
            Random R = new Random();
            return levels[R.Next(0, levels.Length - 1)];
        }
    }
}
