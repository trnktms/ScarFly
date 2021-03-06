﻿using System;
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
using ScarFly.MyClasses.Helpers;
using System.Text;
using System.Threading;
using ScarFly.MyClasses.Common;

namespace ScarFly
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //NOTE: Common
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont baseFont;
        SpriteFont baseFontBig;

        //NOTE: Menu
        Menu mainMenu;
        Menu pauseMenu;
        Menu tutorialMenu;
        Menu aboutMenu;
        List<MenuButton> mainButtons = new List<MenuButton>();
        List<MenuButton> pauseButtons = new List<MenuButton>();
        List<MenuButton> endGameButtons = new List<MenuButton>();
        List<MenuButton> tutorialButtons = new List<MenuButton>();
        List<MenuButton> aboutButtons = new List<MenuButton>();
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
        public GameState gameState { get; set; }

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
            mainButtons.Add(new MenuButton("Main_Help", Consts.P_HelpButton, (Consts.PhoneWidth / 2) + 20, (Consts.PhoneHeight / 2) - 220 + 100));
            mainButtons.Add(new MenuButton("Main_About", Consts.P_AboutButton, (Consts.PhoneWidth / 2) + 20, (Consts.PhoneHeight / 2) - 220 + 200));
            mainButtons.Add(new MenuButton("Main_Vibrate", Consts.P_VibrateButton, (Consts.PhoneWidth) -100, (Consts.PhoneHeight)-100));
            mainMenu = new Menu(mainButtons);

            pauseButtons = new List<MenuButton>();
            pauseButtons.Add(new MenuButton("Pause_Resume", Consts.P_ResumeButton, (Consts.PhoneWidth / 2) - 166, (Consts.PhoneHeight / 2) - 200));
            pauseMenu = new Menu(pauseButtons);

            tutorialButtons = new List<MenuButton>();
            tutorialButtons.Add(new MenuButton("Tutorial", Consts.P_Tutorial, 0, 0));
            tutorialMenu = new Menu(tutorialButtons);

            aboutButtons = new List<MenuButton>();
            aboutButtons.Add(new MenuButton("About_Rate", Consts.P_RateButton, (Consts.PhoneWidth / 2) - 166, (Consts.PhoneHeight / 2) - 50));
            aboutMenu = new Menu(aboutButtons);

            //NOTE: GAMING
            player = new Player("Player1", 4, 100, 340, Consts.P_Player, 1);
            backBackground = new PlayerBackground(Consts.P_Backgrond, 1);
            foreBackground = new PlayerBackground(Consts.P_ForeBackground, 2);
            walkPlace = new PlayerBackground(Consts.P_Walkplace, 4);

            string level = LevelHelper.SelectLevel();
            barriers = new Barriers(level, 4);
            moneys = new Moneys(level, 4);
            modifiers = new Modifiers(level, 4);

            backgroundList = new List<PlayerBackground>();
            backgroundList.Add(backBackground);
            backgroundList.Add(foreBackground);
            backgroundList.Add(walkPlace);

            collosion = new Collosion(barriers, player, moneys, modifiers, backgroundList);

            if (!MenuHelper.FirstStart()) { gameState = GameState.InTutorial; }
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
            baseFont = this.Content.Load<SpriteFont>(Consts.SF_BaseFont);
            baseFontBig = this.Content.Load<SpriteFont>(Consts.SF_BaseFontBig);
            mainMenu.LoadButtonList(this);
            pauseMenu.LoadButtonList(this);
            tutorialMenu.LoadButtonList(this);
            aboutMenu.LoadButtonList(this);
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
            switch (gameState)
            {
                //NOTE: LOAD LEVEL
                case GameState.LoadLevel:
                    if (firstEntry)
                    {
                        player.RePosition();
                        firstEntry = false;
                        Thread loadThread = new Thread(() =>
                            {
                                foreach (PlayerBackground item in backgroundList) { item.RePosition(); }
                                string level = LevelHelper.SelectLevel();
                                barriers = new Barriers(level, 4);
                                barriers.Load(this);
                                moneys = new Moneys(level, 4);
                                moneys.Load(this);
                                modifiers = new Modifiers(level, 4);
                                modifiers.Load(this);
                                
                                collosion = new Collosion(barriers, player, moneys, modifiers, backgroundList);
                                collosion.Load(this);
                                gameState = GameState.Gaming;
                                //Transitions.ChangeGameState(ref firstEntry);
                            });
                        loadThread.Start();
                    }
                    player.Update(this, ref firstEntry);
                    backBackground.Scroll(this);
                    foreBackground.Scroll(this);
                    walkPlace.Scroll(this);
                    break;
                //NOTE: GAMING
                case GameState.Gaming:
                    if (firstEntry) { firstEntry = false; }
                    else
                    {
                        if (LevelHelper.IsLevelEnd(moneys))
                        {
                            player.isEnd = true;
                            if (player.Position.X >= Consts.PhoneWidth)
                            {
                                Transitions.ChangeGameState(ref firstEntry);
                                gameState = GameState.InEndGameMenu;
                            }
                        }
                        player.Update(this, ref firstEntry);
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
                        backBackground.RePosition();
                        foreBackground.RePosition();
                        walkPlace.RePosition();
                        collosion.Reset();
                        firstEntry = false;
                    }
                    backBackground.Scroll(this);
                    foreBackground.Scroll(this);
                    walkPlace.Scroll(this);
                    gameState = mainMenu.IsTouched(this, TouchPanel.GetState(), gameState, ref firstEntry);
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) this.Exit();
                    break;
                //NOTE: PAUSE MENU
                case GameState.InPauseMenu:
                    if (firstEntry)
                    {
                        pauseMenu = new Menu(pauseButtons);
                        firstEntry = false;
                    }
                    gameState = pauseMenu.IsTouched(this, TouchPanel.GetState(), gameState, ref firstEntry);
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
                    gameState = tutorialMenu.IsTouched(this, TouchPanel.GetState(), gameState, ref firstEntry);
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    {
                        Transitions.ChangeGameState(ref firstEntry);
                        gameState = GameState.InMainMenu;
                    }
                    break;
                //NOTE: ABOUT
                case GameState.About:
                    backBackground.Scroll(this);
                    foreBackground.Scroll(this);
                    walkPlace.Scroll(this);
                    gameState = aboutMenu.IsTouched(this, TouchPanel.GetState(), gameState, ref firstEntry);
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
                player.Draw(spriteBatch, color);
                walkPlace.Draw(spriteBatch, color);
                player.Score.DrawGameScore(spriteBatch, color);
                spriteBatch.DrawString(baseFontBig, "Get ready!", new Vector2(Consts.PhoneWidth / 2, Consts.PhoneHeight / 2), color * 0.7f, 0.0f, baseFontBig.MeasureString("Get ready!") / 2, 1.0f, SpriteEffects.None, 0.0f);
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
            //NOTE: ABOUT
            else if (gameState == GameState.About)
            {
                Color color = Color.White;
                Transitions.Transition(ref color);
                backBackground.Draw(spriteBatch, color);
                foreBackground.Draw(spriteBatch, color);
                walkPlace.Draw(spriteBatch, color);
                aboutMenu.DrawButtonList(spriteBatch, color);
                spriteBatch.DrawString(baseFontBig, "Developed by Tamas Tarnok", new Vector2(Consts.PhoneWidth / 2, 100), color, 0.0f, baseFontBig.MeasureString("Developed by Tamas Tarnok") / 2, 1.0f, SpriteEffects.None, 0.0f);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
