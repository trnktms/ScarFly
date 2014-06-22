using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.PlayerClasses
{
    public class PlayerBackground
    {
        public PlayerBackground(string assetName, int velocity)
        {
            this.AssetName = assetName;
            this.Velocity = velocity;
            this.StartVelocity = velocity;
        }

        public string AssetName { get; set; }
        public int Velocity { get; set; }
        public int StartVelocity { get; set; }
        public Rectangle Rectangle1 { get; set; }
        public Rectangle Rectangle2 { get; set; }
        public Texture2D Texture1 { get; set; }
        public Texture2D Texture2 { get; set; }

        public void Load(Game1 game) 
        {
            Texture1 = game.Content.Load<Texture2D>(AssetName);
            Texture2 = game.Content.Load<Texture2D>(AssetName);
            Rectangle1 = new Rectangle(0, 0, Texture1.Width, Texture1.Height);
            Rectangle2 = new Rectangle(Texture1.Width, 0, Texture1.Width, Texture1.Height);
        }

        public void Scroll(Game1 game)
        {
            if (Rectangle1.X + Texture1.Width <= 0)
            {
                Rectangle1 = new Rectangle(Rectangle2.X + Texture1.Width - Velocity, Rectangle1.Y, Rectangle1.Width, Rectangle1.Height);
            }
            else
            {
                Rectangle1 = new Rectangle(Rectangle1.X - Velocity, Rectangle1.Y, Rectangle1.Width, Rectangle1.Height);
                //Rectangle2 = new Rectangle(Rectangle2.X - Step, Rectangle2.Y, Rectangle2.Width, Rectangle2.Height);
            }
            if (Rectangle2.X + Texture1.Width <= 0)
            {
                Rectangle2 = new Rectangle(Rectangle1.X + Texture1.Width - Velocity, Rectangle2.Y, Rectangle2.Width, Rectangle2.Height);
            }
            else
            {
                //Rectangle1 = new Rectangle(Rectangle1.X - Step, Rectangle1.Y, Rectangle1.Width, Rectangle1.Height);
                Rectangle2 = new Rectangle(Rectangle2.X - Velocity, Rectangle2.Y, Rectangle2.Width, Rectangle2.Height);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(Texture1, Rectangle1, color);
            spriteBatch.Draw(Texture2, Rectangle2, color);
        }

        public void RePosition()
        {
            Velocity = StartVelocity;
        }
    }
}
