using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using ScarFly.MyClasses;
using ScarFly.MyClasses.PlayerClasses;
using ScarFly.MyClasses.LevelElementClasses;
using ScarFly.MyClasses.MenuClasses;

namespace ScarFly
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Menu mainMenu;
        Menu pauseMenu;
        Menu endGameMenu;
        List<MenuButton> mainButtons = new List<MenuButton>();
        List<MenuButton> pauseButtons = new List<MenuButton>();
        List<MenuButton> endGameButtons = new List<MenuButton>();

        PlayerBackground backBackground;
        PlayerBackground foreBackground;
        PlayerBackground walkPlace;
        Player player;
        Barriers barriers;
        Moneys moneys;
        Collosion collosion;
        GameState gameState;

        bool firstEntry;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);
            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferHeight = 480;
            graphics.PreferredBackBufferWidth = 800;
            Consts.PhoneWidth = graphics.PreferredBackBufferWidth;
            Consts.PhoneHeight = graphics.PreferredBackBufferHeight;

            firstEntry = true;
            gameState = GameState.InMainMenu;

            mainButtons = new List<MenuButton>();
            mainButtons.Add(new MenuButton("Main_Start", "Buttons/StartButton", (Consts.PhoneWidth / 2) - 124, (Consts.PhoneHeight / 2) - 128));
            mainMenu = new Menu(mainButtons);

            pauseButtons = new List<MenuButton>();
            pauseButtons.Add(new MenuButton("Pause_Resume", "Buttons/StartButton", (Consts.PhoneWidth / 2) - 124, (Consts.PhoneHeight / 2) - 128));
            pauseMenu = new Menu(pauseButtons);

            endGameButtons = new List<MenuButton>();
            endGameButtons.Add(new MenuButton("EndGame_Start", "Buttons/StartButton", (Consts.PhoneWidth / 2) - 124, (Consts.PhoneHeight / 2) - 128));
            endGameMenu = new Menu(endGameButtons);

            player = new Player("Player1", 4, 100, 370, "Player/PaperPlane_v2", "Player/PaperPlane_v2", "Player/PaperPlane_v2", 1, 1, 1);
            backBackground = new PlayerBackground("Background/Forest", 1);
            foreBackground = new PlayerBackground("Background/ForestFore", 2);
            walkPlace = new PlayerBackground("Background/WalkPlace", 4);

            string level = LevelSelector.Select();
            barriers = new Barriers(level, 4);
            moneys = new Moneys(level, 4);

            collosion = new Collosion(barriers, player, moneys);
        }

        protected override void Initialize()
        {
            base.Initialize();
            TouchPanel.EnabledGestures = GestureType.Tap | GestureType.None;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mainMenu.LoadButtonList(this);
            pauseMenu.LoadButtonList(this);
            endGameMenu.LoadButtonList(this);
            player.Load(this);
            backBackground.Load(this);
            foreBackground.Load(this);
            walkPlace.Load(this);
            barriers.Load(this);
            moneys.Load(this);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            //NOTE: MAIN MENU
            if (gameState == GameState.InMainMenu)
            {
                if (firstEntry)
                {
                    mainMenu = new Menu(mainButtons);

                    firstEntry = false;
                }
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) this.Exit();
                backBackground.Scroll(this);
                foreBackground.Scroll(this);
                walkPlace.Scroll(this);
                gameState = mainMenu.IsTouched(this, TouchPanel.GetState(), gameState, ref firstEntry);
            }
            //NOTE: GAMING
            else if (gameState == GameState.Gaming)
            {
                if (firstEntry)
                {
                    string level = LevelSelector.Select();
                    barriers = new Barriers(level, 4);
                    barriers.Load(this);
                    moneys = new Moneys(level, 4);
                    moneys.Load(this);
                    player.RePosition();
                    collosion = new Collosion(barriers, player, moneys);

                    firstEntry = false;
                }
                player.Update();
                if (!(moneys.GetActualMoneyList().Count != 0 && moneys.GetActualMoneyList().LastOrDefault().Index.ID == "!"))
                {
                    backBackground.Scroll(this);
                    foreBackground.Scroll(this);
                    walkPlace.Scroll(this);
                    barriers.Scroll(this);
                    moneys.Scroll(this);
                    collosion.Update();
                }
                else
                {
                    player.isEnd = true;
                    collosion.Update();
                    if (player.Position.X >= Consts.PhoneWidth)
                    {
                        gameState = GameState.InEndGameMenu;
                        firstEntry = true;
                    }
                }

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                    gameState = GameState.InPauseMenu;
                    firstEntry = true;
                }
            }
            //NOTE: PAUSE MENU
            else if (gameState == GameState.InPauseMenu)
            {
                if (firstEntry)
                {
                    pauseMenu = new Menu(pauseButtons);
                    firstEntry = false;
                }
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                    gameState = GameState.InMainMenu;
                    firstEntry = true;
                }
                gameState = pauseMenu.IsTouched(this, TouchPanel.GetState(), gameState, ref firstEntry);
            }
            //NOTE: END GAME MENU
            else if (gameState == GameState.InEndGameMenu)
            {
                if (firstEntry)
                {
                    endGameMenu = new Menu(endGameButtons);
                    firstEntry = false;
                }
                backBackground.Scroll(this);
                foreBackground.Scroll(this);
                walkPlace.Scroll(this);
                gameState = endGameMenu.IsTouched(this, TouchPanel.GetState(), gameState, ref firstEntry);

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                    gameState = GameState.InMainMenu;
                }
            }
            //NOTE: SCORE MENU
            else if (gameState == GameState.InScoreMenu)
            { }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            //NOTE: MAIN MENU
            if (gameState == GameState.InMainMenu)
            {
                backBackground.Draw(spriteBatch);
                foreBackground.Draw(spriteBatch);
                walkPlace.Draw(spriteBatch);
                mainMenu.DrawButtonList(spriteBatch);
            }
            //NOTE: GAMING
            else if (gameState == GameState.Gaming)
            {
                backBackground.Draw(spriteBatch);
                foreBackground.Draw(spriteBatch);
                walkPlace.Draw(spriteBatch);
                player.Draw(spriteBatch);
                barriers.Draw(spriteBatch);
                moneys.Draw(spriteBatch);
                player.Score.DrawGameScore(spriteBatch);
            }
            //NOTE: PAUSE MENU
            else if (gameState == GameState.InPauseMenu)
            {
                backBackground.Draw(spriteBatch);
                foreBackground.Draw(spriteBatch);
                walkPlace.Draw(spriteBatch);
                barriers.Draw(spriteBatch);
                moneys.Draw(spriteBatch);
                pauseMenu.DrawButtonList(spriteBatch);
            }
            //NOTE: END GAME MENU
            else if (gameState == GameState.InEndGameMenu)
            {
                backBackground.Draw(spriteBatch);
                foreBackground.Draw(spriteBatch);
                walkPlace.Draw(spriteBatch);
                endGameMenu.DrawButtonList(spriteBatch);
                spriteBatch.DrawString(player.Score.GameScoreFont, "" + player.Score.GameScore, new Vector2(10, 10), Color.White);
                spriteBatch.DrawString(player.Score.TotalScoreFont, "" + player.Score.TotalScore, new Vector2(10, 80), Color.White);
            }
            //NOTE: SCORE MENU
            else if (gameState == GameState.InScoreMenu)
            { }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
