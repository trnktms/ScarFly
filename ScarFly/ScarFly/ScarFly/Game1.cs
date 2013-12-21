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
using ScarFly.MyClasses.NetworkClasses;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ScarFly
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont baseFont;

        //NOTE: Menu
        Menu mainMenu;
        Menu pauseMenu;
        //Menu endGameMenu;
        Menu tutorialMenu;
        List<MenuButton> mainButtons = new List<MenuButton>();
        List<MenuButton> pauseButtons = new List<MenuButton>();
        List<MenuButton> endGameButtons = new List<MenuButton>();
        List<MenuButton> tutorialButtons = new List<MenuButton>();
        bool firstEntry;

        //NOTE: Gaming
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

        //NOTE: Network
        Multiplayer multiPlayer;
        string selectedLevel;

        public Game1()
        {
            //NOTE: Init
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

            //NOTE: MENU
            mainButtons = new List<MenuButton>();
            mainButtons.Add(new MenuButton("Main_Start", Consts.P_StartButton, (Consts.PhoneWidth / 2) + 20, (Consts.PhoneHeight / 2) - 220));
            mainButtons.Add(new MenuButton("Main_Help", Consts.P_HelpButton, (Consts.PhoneWidth / 2) + 20, (Consts.PhoneHeight / 2) - 220 + 200));
            mainButtons.Add(new MenuButton("Main_About", Consts.P_AboutButton, (Consts.PhoneWidth / 2) + 20, (Consts.PhoneHeight / 2) - 220 + 300));
            mainButtons.Add(new MenuButton("Main_Network", Consts.P_NetworkButton, (Consts.PhoneWidth / 2) + 20, (Consts.PhoneHeight / 2) - 220 + 100));
            mainMenu = new Menu(mainButtons);

            pauseButtons = new List<MenuButton>();
            pauseButtons.Add(new MenuButton("Pause_Resume", Consts.P_ResumeButton, (Consts.PhoneWidth / 2) - 166, (Consts.PhoneHeight / 2) - 200));
            pauseMenu = new Menu(pauseButtons);

            tutorialButtons = new List<MenuButton>();
            tutorialButtons.Add(new MenuButton("Tutorial", Consts.P_Tutorial, 0, 0));
            tutorialMenu = new Menu(tutorialButtons);

            //NOTE: GAMING
            player = new Player("Player1", 4, 100, 350, Consts.P_Player, 1);
            backBackground = new PlayerBackground(Consts.P_Backgrond, 1);
            foreBackground = new PlayerBackground(Consts.P_ForeBackground, 2);
            walkPlace = new PlayerBackground(Consts.P_Walkplace, 4);

            string level = LevelSelector.Select();
            barriers = new Barriers(level, 4);
            moneys = new Moneys(level, 4);
            modifiers = new Modifiers(level, 4);

            backgroundList = new List<PlayerBackground>();
            backgroundList.Add(backBackground);
            backgroundList.Add(foreBackground);
            backgroundList.Add(walkPlace);

            collosion = new Collosion(barriers, player, moneys, modifiers, backgroundList);

            multiPlayer = new Multiplayer(player);

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
            baseFont = this.Content.Load<SpriteFont>(Consts.SF_GameScore);
            mainMenu.LoadButtonList(this);
            pauseMenu.LoadButtonList(this);
            tutorialMenu.LoadButtonList(this);
            player.Load(this);
            multiPlayer.OtherPlayer.Load(this);
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
            switch (gameState)
            {
                //NOTE: LOAD LEVEL
                case GameState.LoadLevel:
                    if (firstEntry)
                    {
                        firstEntry = false;
                        Thread loadThread = new Thread(() =>
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
                                gameState = GameState.Gaming;
                                Transitions.ChangeGameState(ref firstEntry);
                            });
                        loadThread.Start();
                    }
                    backBackground.Scroll(this);
                    foreBackground.Scroll(this);
                    walkPlace.Scroll(this);
                    break;
                //NOTE: GAMING
                case GameState.Gaming:
                    if (firstEntry)
                    {
                        firstEntry = false;
                    }
                    else
                    {
                        if (Helper.IsLevelEnd(moneys))
                        {
                            player.isEnd = true;
                            if (player.Position.X >= Consts.PhoneWidth)
                            {
                                Transitions.ChangeGameState(ref firstEntry);
                                gameState = GameState.InEndGameMenu;
                            }
                        }
                        player.Update();
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
                    break;
                //NOTE: NETWORK SEARCH
                case GameState.NetworkSearch:
                    if (firstEntry)
                    {
                        selectedLevel = LevelSelector.Select();
                        multiPlayer.InitializeSockets();
                        firstEntry = false;
                    }
                    else
                    {
                        multiPlayer.SendedData = string.Format("{0};{1};{2}", player.Id, "0", selectedLevel);
                        multiPlayer.SendData();
                        if (multiPlayer.OtherPlayer.Id != Guid.Empty && !string.IsNullOrWhiteSpace(multiPlayer.Level))
                        {
                            gameState = GameState.NetworkGaming;
                            Transitions.ChangeGameState(ref firstEntry);
                        }
                        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                        {
                            gameState = GameState.InMainMenu;
                            multiPlayer.Channel.GroupClose();
                            Transitions.ChangeGameState(ref firstEntry);
                        }
                    }
                    break;
                //NOTE: NETWORK GAMING
                case GameState.NetworkGaming:
                    if (firstEntry)
                    {
                        foreach (PlayerBackground item in backgroundList) { item.RePosition(); }
                        string level = multiPlayer.Level;
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
                        if (Helper.IsLevelEnd(moneys))
                        {
                            player.isEnd = true;
                            if (player.Position.X >= Consts.PhoneWidth)
                            {
                                Transitions.ChangeGameState(ref firstEntry);
                                gameState = GameState.NetworkEndGame;
                            }
                        }
                        multiPlayer.Update(gameTime);
                        player.Update();
                        backBackground.Scroll(this);
                        foreBackground.Scroll(this);
                        walkPlace.Scroll(this);
                        barriers.Scroll(this);
                        moneys.Scroll(this);
                        modifiers.Scroll(this);
                        collosion.Update();
                    }
                    break;
                //NOTE: NETWORK END GAME
                case GameState.NetworkEndGame:
                    multiPlayer.Update(gameTime);
                    backBackground.Scroll(this);
                    foreBackground.Scroll(this);
                    walkPlace.Scroll(this);
                    foreach (TouchLocation item in TouchPanel.GetState())
                    {
                        if (item.State == TouchLocationState.Released)
                        {
                            multiPlayer.Channel.GroupClose();
                            multiPlayer.Reset();
                            gameState = GameState.InMainMenu;
                        }
                    }
                    break;
                //NOTE: MAIN MENU
                case GameState.InMainMenu:
                    if (firstEntry)
                    {
                        player.Score.LoadTotalScore();
                        player.Score.LoadHighScore();
                        mainMenu = new Menu(mainButtons);
                        player.isDead = false;
                        player.isEatMoney = false;
                        player.isEatModifier = false;
                        collosion.Reset();
                        firstEntry = false;
                    }
                    backBackground.Scroll(this);
                    foreBackground.Scroll(this);
                    walkPlace.Scroll(this);
                    gameState = mainMenu.IsTouched(this, TouchPanel.GetState(), gameState, ref firstEntry, spriteBatch);
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) this.Exit();
                    break;
                //NOTE: PAUSE MENU
                case GameState.InPauseMenu:
                    if (firstEntry)
                    {
                        pauseMenu = new Menu(pauseButtons);
                        firstEntry = false;
                    }
                    gameState = pauseMenu.IsTouched(this, TouchPanel.GetState(), gameState, ref firstEntry, spriteBatch);
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    {
                        Transitions.ChangeGameState(ref firstEntry);
                        player.Score.GameScore = 0;
                        gameState = GameState.InMainMenu;
                    }
                    break;
                //NOTE: TUTORIAL
                case GameState.InTutorial:
                    backBackground.Scroll(this);
                    foreBackground.Scroll(this);
                    walkPlace.Scroll(this);
                    gameState = tutorialMenu.IsTouched(this, TouchPanel.GetState(), gameState, ref firstEntry, spriteBatch);
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    {
                        Transitions.ChangeGameState(ref firstEntry);
                        gameState = GameState.InMainMenu;
                    }
                    break;
                //NOTE: ENDGAME MENU
                case GameState.InEndGameMenu:
                    player.Score.SaveTotalScore();
                    player.Score.SaveHighScore();
                    gameState = GameState.InMainMenu;
                    break;
                default:
                    break;
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
            //NOTE: LOAD LEVEL
            else if (gameState == GameState.LoadLevel)
            {
                Color color = Color.White;
                Transitions.Transition(ref color);
                backBackground.Draw(spriteBatch, color);
                foreBackground.Draw(spriteBatch, color);
                walkPlace.Draw(spriteBatch, color);
                spriteBatch.DrawString(baseFont, "Loading...", new Vector2(10, 10), color);
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
            //NOTE: NETWORK SEARCH
            else if (gameState == GameState.NetworkSearch)
            {
                Color color = Color.White;
                Transitions.Transition(ref color);
                backBackground.Draw(spriteBatch, color);
                foreBackground.Draw(spriteBatch, color);
                spriteBatch.DrawString(baseFont, "Searching player...", new Vector2(0, 0), color);
            }
            //NOTE: NETWORK GAMING
            else if (gameState == GameState.NetworkGaming)
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
                multiPlayer.Draw(spriteBatch, color);
            }
            //NOTE: NETWORK END GAME
            else if (gameState == GameState.NetworkEndGame)
            {
                Color color = Color.White;
                Transitions.Transition(ref color);
                backBackground.Draw(spriteBatch, color);
                foreBackground.Draw(spriteBatch, color);
                walkPlace.Draw(spriteBatch, color);
                multiPlayer.Draw(spriteBatch, color);
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
            //NOTE: TUTORIAL
            else if (gameState == GameState.InTutorial)
            {
                Color color = Color.White;
                Transitions.Transition(ref color);
                backBackground.Draw(spriteBatch, color);
                foreBackground.Draw(spriteBatch, color);
                walkPlace.Draw(spriteBatch, color);
                tutorialMenu.DrawButtonList(spriteBatch, color);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
