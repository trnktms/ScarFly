using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.LevelElementClasses
{
    public abstract class LevelElement
    {
        public LevelElement(string assetName, int moveCount)
        {
            this.AssetName = assetName;
            this.MoveCount = moveCount;
        }
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
            animateCount++;
            if (animateCount == MoveCount) { animateCount = 0; }
        }

        protected int animateCount = -1;
        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(Texture, Position, new Rectangle((int)(MoveWidth * animateCount), 0, (int)MoveWidth, (int)Texture.Height), color);
        }
    }
}
