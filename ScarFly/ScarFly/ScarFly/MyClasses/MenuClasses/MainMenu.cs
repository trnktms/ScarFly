﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses
{
    public enum GameState
    {
        Gaming,
        InMainMenu,
        InScoreMenu,
        InPauseMenu,
        Invalid
    }

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

        public GameState IsTouched(Game1 game, TouchCollection touchCollection, GameState currentGameState)
        {
            GameState result = currentGameState;
            if (ButtonList != null)
            {
                foreach (TouchLocation touchLocItem in touchCollection)
                {
                    foreach (MenuButton buttonItem in ButtonList)
                    {
                        if ((touchLocItem.State == TouchLocationState.Pressed)
                        && (touchLocItem.Position.X >= buttonItem.Position.X
                        && touchLocItem.Position.Y >= buttonItem.Position.Y
                        && touchLocItem.Position.X <= buttonItem.Texture.Width + buttonItem.Position.X
                        && touchLocItem.Position.Y <= buttonItem.Texture.Height + buttonItem.Position.Y))
                        {
                            switch (buttonItem.Name)
                            {
                                case "Start": result = GameState.Gaming;
                                    break;
                                case "Scores": result = GameState.InScoreMenu;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
