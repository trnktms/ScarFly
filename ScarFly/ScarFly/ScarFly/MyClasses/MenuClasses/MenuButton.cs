using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses
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

        public bool IsTouched(Game1 game, TouchCollection touchCollection) 
        {
            foreach (TouchLocation touchLocItem in touchCollection)
            {
                if ((touchLocItem.State == TouchLocationState.Pressed) 
                    && (touchLocItem.Position.X >= Position.X 
                    && touchLocItem.Position.Y >= Position.Y 
                    && touchLocItem.Position.X <= Texture.Width + Position.X 
                    && touchLocItem.Position.Y <= Texture.Height + Position.Y))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
