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
        Menu tutorialMenu;
        List<MenuButton> mainButtons = new List<MenuButton>();
        List<MenuButton> pauseButtons = new List<MenuButton>();
        List<MenuButton> endGameButtons = new List<MenuButton>();
        List<MenuButton> tutorialButtons = new List<MenuButton>();

        List<PlayerBackground> backgroundList;
        PlayerBackground backBackground;
        PlayerBackground foreBackground;
        PlayerBackground walkPlace;
        Player player;
        Barriers barriers;
        Moneys moneys;
        Modifiers modifiers;
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
            this.Deactivated += Game1_Deactivated;
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

            tutorialButtons = new List<MenuButton>();
            tutorialButtons.Add(new MenuButton("Tutorial", "Buttons/Tutorial", 0, 0));
            tutorialMenu = new Menu(tutorialButtons);

            //player = new Player("Player1", 4, 100, 370, "Player/PaperPlane_v2", "Player/PaperPlane_v2", "Player/PaperPlane_v2", 1, 1, 1);
            player = new Player("Player1", 4, 100, 370, "Player/PaperPlane_v2", 1);
            backBackground = new PlayerBackground("Background/Forest", 1);
            foreBackground = new PlayerBackground("Background/ForestFore", 2);
            walkPlace = new PlayerBackground("Background/WalkPlace", 4);

            string level = LevelSelector.Select();
            barriers = new Barriers(level, 4);
            moneys = new Moneys(level, 4);
            modifiers = new Modifiers(level, 4);

            backgroundList = new List<PlayerBackground>();
            backgroundList.Add(backBackground);
            backgroundList.Add(foreBackground);
            backgroundList.Add(walkPlace);

            collosion = new Collosion(barriers, player, moneys, modifiers, backgroundList);

            if (!Tutorial.FirstStart()) { gameState = GameState.InTutorial; }
        }

        private void Game1_Deactivated(object sender, EventArgs e)
        {
            if (gameState == GameState.Gaming) { gameState = GameState.InPauseMenu; } 
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
            tutorialMenu.LoadButtonList(this);
            player.Load(this);
            backBackground.Load(this);
            foreBackground.Load(this);
            walkPlace.Load(this);
            barriers.Load(this);
            moneys.Load(this);
            modifiers.Load(this);
            collosion.Load(this);
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
                    player.Score.LoadTotalScore();
                    mainMenu = new Menu(mainButtons);
                    firstEntry = false;
                }
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) this.Exit();
                backBackground.Scroll(this);
                foreBackground.Scroll(this);
                walkPlace.Scroll(this);
                gameState = mainMenu.IsTouched(this, TouchPanel.GetState(), gameState, ref firstEntry, spriteBatch);
            }
            //NOTE: GAMING
            else if (gameState == GameState.Gaming)
            {
                if (firstEntry)
                {
                    foreach (PlayerBackground item in backgroundList) { item.RePosition(); }
                    string level = LevelSelector.Select();
                    barriers = new Barriers(level, 4);
                    barriers.Load(this);
                    moneys = new Moneys(level, 4);
                    moneys.Load(this);
                    modifiers = new Modifiers(level, 4);
                    modifiers.Load(this);
                    player.RePosition();
                    collosion = new Collosion(barriers, player, moneys, modifiers, backgroundList);
                    collosion.Load(this);

                    firstEntry = false;
                }
                else
                {
                    player.Update();
                    if (moneys.GetActualMoneyList().Count != 0 && moneys.GetActualMoneyList().LastOrDefault().Index.ID == "!")
                    {
                        player.isEnd = true;
                        if (player.Position.X >= Consts.PhoneWidth)
                        {
                            Transitions.TransitionCounter = 0;
                            Transitions.IsTransition = true;
                            gameState = GameState.InEndGameMenu;
                            firstEntry = true;
                        }
                    }
                    backBackground.Scroll(this);
                    foreBackground.Scroll(this);
                    walkPlace.Scroll(this);
                    barriers.Scroll(this);
                    moneys.Scroll(this);
                    modifiers.Scroll(this);
                    collosion.Update();
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    {
                        gameState = GameState.InPauseMenu;
                        firstEntry = true;
                    }
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
                    Transitions.TransitionCounter = 0;
                    Transitions.IsTransition = true;
                    gameState = GameState.InMainMenu;
                    firstEntry = true;
                }
                gameState = pauseMenu.IsTouched(this, TouchPanel.GetState(), gameState, ref firstEntry, spriteBatch);
            }
            //NOTE: END GAME MENU
            else if (gameState == GameState.InEndGameMenu)
            {
                if (firstEntry)
                {
                    endGameMenu = new Menu(endGameButtons);
                    player.Score.SaveTotalScore();
                    firstEntry = false;
                }
                backBackground.Scroll(this);
                foreBackground.Scroll(this);
                walkPlace.Scroll(this);
                gameState = endGameMenu.IsTouched(this, TouchPanel.GetState(), gameState, ref firstEntry, spriteBatch);

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                    Transitions.TransitionCounter = 0;
                    Transitions.IsTransition = true;
                    gameState = GameState.InMainMenu;
                    firstEntry = true;
                }
            }
            //NOTE: TUTORIAL
            else if (gameState == GameState.InTutorial)
            {
                gameState = tutorialMenu.IsTouched(this, TouchPanel.GetState(), gameState, ref firstEntry, spriteBatch);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            //NOTE: MAIN MENU
            if (gameState == GameState.InMainMenu)
            {
                Color color = Color.White;
                Transitions.Transition(ref color);
                backBackground.Draw(spriteBatch, color);
                foreBackground.Draw(spriteBatch, color);
                walkPlace.Draw(spriteBatch, color);
                mainMenu.DrawButtonList(spriteBatch, color);
                player.Score.DrawMainMenuScores(spriteBatch, color);
            }
            //NOTE: GAMING
            else if (gameState == GameState.Gaming)
            {
                Color color = Color.White;
                Transitions.Transition(ref color);
                backBackground.Draw(spriteBatch, color);
                foreBackground.Draw(spriteBatch, color);
                player.Draw(spriteBatch, color);
                barriers.Draw(spriteBatch, color);
                moneys.Draw(spriteBatch, color);
                modifiers.Draw(spriteBatch, color);
                walkPlace.Draw(spriteBatch, color);
                player.Score.DrawGameScore(spriteBatch, color);
                collosion.Draw(spriteBatch);
            }
            //NOTE: PAUSE MENU
            else if (gameState == GameState.InPauseMenu)
            {
                Color color = Color.White;
                Transitions.Transition(ref color);
                backBackground.Draw(spriteBatch, color);
                foreBackground.Draw(spriteBatch, color);
                barriers.Draw(spriteBatch, color);
                moneys.Draw(spriteBatch, color);
                modifiers.Draw(spriteBatch, color);
                walkPlace.Draw(spriteBatch, color);
                player.Score.DrawGameScore(spriteBatch, color);
                pauseMenu.DrawButtonList(spriteBatch, color);
            }
            //NOTE: END GAME MENU
            else if (gameState == GameState.InEndGameMenu)
            {
                Color color = Color.White;
                Transitions.Transition(ref color);
                backBackground.Draw(spriteBatch, color);
                foreBackground.Draw(spriteBatch, color);
                walkPlace.Draw(spriteBatch, color);
                endGameMenu.DrawButtonList(spriteBatch, color);
                player.Score.DrawEndGameScores(spriteBatch, color);
            }
            //NOTE: TUTORIAL
            else if (gameState == GameState.InTutorial)
            {
                Color color = Color.White;
                Transitions.Transition(ref color);
                tutorialMenu.DrawButtonList(spriteBatch, color);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
