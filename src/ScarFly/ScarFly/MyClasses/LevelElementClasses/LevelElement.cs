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
            this.Texture = game.Content.Load<Texture2D>(AssetName);
            this.ColorData = new Color[Texture.Width * Texture.Height];
            this.Texture.GetData(ColorData);
            this.MoveWidth = Texture.Width / MoveCount;
            this.UpdateRectangle();
        }

        public void Load(Game1 game, LevelElement levelElement)
        {
            if (levelElement == null)
            {
                this.Texture = game.Content.Load<Texture2D>(AssetName);
                this.ColorData = new Color[Texture.Width * Texture.Height];
                this.Texture.GetData(ColorData);
                this.MoveWidth = Texture.Width / MoveCount;
            }
            else 
            {
                this.Texture = levelElement.Texture;
                this.ColorData = levelElement.ColorData;
                this.MoveWidth = levelElement.MoveWidth;
            }

            this.UpdateRectangle();
        }

        public void UpdateRectangle()
        {
            this.Bound = new Rectangle((int)Position.X, (int)Position.Y, MoveWidth, Texture.Height);
            this.animateCount++;
            if (this.animateCount == this.MoveCount) { this.animateCount = 0; }
        }

        protected int animateCount = -1;
        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(this.Texture, this.Position, new Rectangle((int)(this.MoveWidth * this.animateCount), 0, (int)this.MoveWidth, (int)this.Texture.Height), color);
        }
    }
}
