using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.Helpers
{
    public static class MenuHelper
    {
        public static bool FirstStart()
        {
            bool result;
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                result = myIsolatedStorage.FileExists("TotalScore.txt");
                if (!result) { myIsolatedStorage.CreateFile("TotalScore.txt"); myIsolatedStorage.CreateFile("HighScore.txt"); }
            }
            return result;
        }
    }
}
