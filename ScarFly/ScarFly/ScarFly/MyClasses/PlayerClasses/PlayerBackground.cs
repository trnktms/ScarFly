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
        public PlayerBackground(string assetName, float positionX, float positionY)
        {
            this.AssetName = assetName;
            Position = new Vector2(positionX, positionY);
        }

        public Vector2 Position { get; set; }
        public string AssetName { get; set; }
        public Texture2D Texture { get; set; }

        public void Load(Game1 game) { Texture = game.Content.Load<Texture2D>(AssetName); }

        private int _scrollCount = 0;
        public void Scroll(Game1 game, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, new Rectangle((int)Position.X + _scrollCount, (int)Position.Y, (int)Texture.Width, (int)Texture.Height), Color.White);
            _scrollCount++;

            if (_scrollCount >= Texture.Width - game.GraphicsDevice.Viewport.Width)
            {
                spriteBatch.Draw(Texture, Position, new Rectangle((int)Position.X + game.GraphicsDevice.Viewport.Width + _scrollCount, (int)Position.Y, (int)Texture.Width, (int)Texture.Height), Color.White);
            }

            if (_scrollCount >= Texture.Width)
            {
                _scrollCount = 0;
            }
        }
    }
}
