using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.LevelElementClasses
{
    public class Barrier
    {
        public Barrier(string assetName, BarrierIndex index, int moveCount)
        {
            this.AssetName = assetName;
            this.Index = index;
            Position = new Vector2(this.Index.Column * (Consts.PhoneWidth / Consts.PhoneWidthRate), (this.Index.Row) * (Consts.PhoneHeight / Consts.PhoneHeightRate));
            StartPosition = Position;
            this.MoveCount = moveCount;
        }

        public BarrierIndex Index { get; set; }
        public string AssetName { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle Bound { get; set; }
        public Color[] ColorData { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 StartPosition { get; set; }
        public int MoveWidth { get; set; }
        public int MoveCount { get; set; }

        public void Load(Game1 game) 
        { 
            Texture = game.Content.Load<Texture2D>(AssetName);
            ColorData = new Color[Texture.Width * Texture.Height];
            Texture.GetData(ColorData);
            this.MoveWidth = Texture.Width / MoveCount;
            UpdateRectangle();
        }

        public void UpdateRectangle()
        {
            Bound = new Rectangle((int)Position.X, (int)Position.Y, MoveWidth, Texture.Height);
        }

        private int _animateCount = 0;
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, new Rectangle((int)(MoveWidth * _animateCount), 0, (int)MoveWidth, (int)Texture.Height), Color.White);
            _animateCount++;
            if (_animateCount == MoveCount) { _animateCount = 0; }
        }
    }
}
