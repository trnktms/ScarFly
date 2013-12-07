using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.MenuClasses
{
    public class MenuButton
    {
        public MenuButton(string name, string assetName, float positionX, float positionY) 
        {
            this.Name = name;
            this.AssetName = assetName;
            Position = new Vector2(positionX, positionY);
        }

        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public string AssetName { get; set; }
        public Texture2D Texture { get; set; }

        public void Load(Game1 game) { Texture = game.Content.Load<Texture2D>(AssetName); }

        public bool IsTouched(TouchLocation touchLocation) 
        {
            return (touchLocation.State == TouchLocationState.Pressed)
                    && (touchLocation.Position.X >= this.Position.X
                    && touchLocation.Position.Y >= this.Position.Y
                    && touchLocation.Position.X <= this.Texture.Width + this.Position.X
                    && touchLocation.Position.Y <= this.Texture.Height + this.Position.Y);
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(Texture, Position, color);
        }
    }
}
