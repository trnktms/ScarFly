using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses
{
    public class Player
    {
        public string Name { get; set; }
        public int GameScore { get; set; }
        public int TotalScore { get; set; }
        public List<Dress> DressList { get; set; }
        public List<InjuryStopper> InjuryStopperList { get; set; }
    }
}
