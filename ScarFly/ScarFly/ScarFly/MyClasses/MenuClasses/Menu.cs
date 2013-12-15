using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.MenuClasses
{
    public class Menu
    {
        public Menu(List<MenuButton> buttonList)
        {
            this.ButtonList = buttonList;
        }

        public List<MenuButton> ButtonList { get; set; }

        public void LoadButtonList(Game1 game)
        {
            foreach (MenuButton buttonItem in ButtonList) { buttonItem.Load(game); }
        }

        public void DrawButtonList(SpriteBatch spriteBatch, Color color)
        {
            foreach (MenuButton buttonItem in ButtonList) { buttonItem.Draw(spriteBatch, color); }
        }

        public GameState IsTouched(Game1 game, TouchCollection touchCollection, GameState currentGameState, ref bool firstEntry, SpriteBatch spriteBatch)
        {
            GameState result = currentGameState;
            if (ButtonList != null)
            {
                foreach (TouchLocation touchLocItem in touchCollection)
                {
                    foreach (MenuButton buttonItem in ButtonList)
                    {
                        buttonItem.IsPressed = buttonItem.IsTouched(touchLocItem);
                        if (buttonItem.IsReleased(touchLocItem))
                        {
                            switch (buttonItem.Name)
                            {
                                case "Main_Start": 
                                    result = GameState.Gaming; Transitions.IsTransition = true; firstEntry = true; Transitions.TransitionCounter = 0;
                                    break;
                                case "Main_Help":
                                    result = GameState.InTutorial; Transitions.IsTransition = true; firstEntry = true; Transitions.TransitionCounter = 0;
                                    break;
                                case "Main_Network":
                                    result = GameState.NetworkSearch; Transitions.IsTransition = true; firstEntry = true; Transitions.TransitionCounter = 0;
                                    break;
                                case "Pause_Resume": 
                                    result = GameState.Gaming;
                                    break;
                                //case "EndGame_Start": 
                                //    result = GameState.Gaming; Transitions.IsTransition = true; firstEntry = true; Transitions.TransitionCounter = 0;
                                //    break;
                                case "Tutorial":
                                    result = GameState.InMainMenu; Transitions.IsTransition = true; firstEntry = true; Transitions.TransitionCounter = 0;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        }
                    }
                }
            }
            return result;
        }
    }
}
