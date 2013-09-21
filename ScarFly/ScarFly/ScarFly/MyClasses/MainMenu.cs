using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses
{
    public class MainMenu
    {
        public MainMenu(List<MenuButton> buttonList)
        {
            this.ButtonList = buttonList;
        }
        public List<MenuButton> ButtonList { get; set; }

        public void LoadButtonList(Game1 game)
        {
            foreach (MenuButton buttonItem in ButtonList) { buttonItem.Load(game); }
        }

        public void DrawButtonList(SpriteBatch spriteBatch)
        {
            foreach (MenuButton buttonItem in ButtonList) { spriteBatch.Draw(buttonItem.Texture, buttonItem.Position, Color.White); }
        }

        public void IsTouched(Game1 game, TouchCollection touchCollection)
        {
            foreach (TouchLocation touchLocItem in touchCollection)
            {
                foreach (MenuButton buttonItem in ButtonList)
                {
                    if ((touchLocItem.State == TouchLocationState.Pressed) 
                    && (touchLocItem.Position.X >= buttonItem.Position.X 
                    && touchLocItem.Position.Y >=buttonItem. Position.Y 
                    && touchLocItem.Position.X <= buttonItem.Texture.Height + buttonItem.Position.X 
                    && touchLocItem.Position.Y <= buttonItem.Texture.Width + buttonItem.Position.Y))
                    {
                        switch (buttonItem.Name)
                        {
                            case "Start": buttonItem.Position = new Vector2(buttonItem.Position.X + 10, buttonItem.Position.Y);
                                break;
                            case "Scores": buttonItem.Position = new Vector2(buttonItem.Position.X, buttonItem.Position.Y + 10);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
