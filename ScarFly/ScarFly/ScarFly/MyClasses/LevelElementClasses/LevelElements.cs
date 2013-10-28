using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.LevelElementClasses
{
    public abstract class LevelElements
    {
        public LevelElements(string levelName, int velocity)
        {
            this.LevelName = string.Format(@"Levels\{0}.level", levelName);
            this.Velocity = velocity;
            _horizontalStep = Consts.PhoneWidth / Consts.PhoneWidthRate;
            _verticalStep = Consts.PhoneHeight / Consts.PhoneHeightRate;
            ProcLevelFile();
        }

        private int _horizontalStep;
        private int _verticalStep;

        public string LevelName { get; set; }
        public int Velocity { get; set; }

        public abstract void ProcLevelFile();

        public abstract void RePosition(Game1 game);
        public abstract void Load(Game1 game);
        public abstract void Scroll(Game1 game);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
