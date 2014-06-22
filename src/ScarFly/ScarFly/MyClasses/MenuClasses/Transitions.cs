using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.MenuClasses
{
    public static class Transitions
    {
        public static int TransitionCounter = 50;
        public static bool IsTransition = true;

        public static void Transition(ref Color color)
        {
            if (IsTransition)
            {
                if (TransitionCounter >= 254) { IsTransition = false; TransitionCounter = 50; return; }
                color = new Color(TransitionCounter, TransitionCounter, TransitionCounter);
                TransitionCounter += 5;
            }
        }

        public static void ChangeGameState(ref bool firstEntry)
        {
            TransitionCounter = 50;
            IsTransition = true;
            firstEntry = true;
        }
    }
}
