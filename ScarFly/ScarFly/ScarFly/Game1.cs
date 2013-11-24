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
        UdpAnySourceMulticastChannel Channel;
        Guid guid = Guid.NewGuid();
        string recievedData;

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

            #region MENU
            mainButtons = new List<MenuButton>();
            mainButtons.Add(new MenuButton("Main_Start", "Buttons/StartButton", (Consts.PhoneWidth / 2) + 20, (Consts.PhoneHeight / 2) - 220));
            mainButtons.Add(new MenuButton("Main_Help", "Buttons/HelpButton", (Consts.PhoneWidth / 2) + 20, (Consts.PhoneHeight / 2) - 220 + 200));
            mainButtons.Add(new MenuButton("Main_About", "Buttons/AboutButton", (Consts.PhoneWidth / 2) + 20, (Consts.PhoneHeight / 2) - 220 + 300));
            mainButtons.Add(new MenuButton("Main_Network", "Buttons/NetworkButton", (Consts.PhoneWidth / 2) + 20, (Consts.PhoneHeight / 2) - 220 + 100));
            mainMenu = new Menu(mainButtons);

            pauseButtons = new List<MenuButton>();
            pauseButtons.Add(new MenuButton("Pause_Resume", "Buttons/ResumeButton", (Consts.PhoneWidth / 2) - 166, (Consts.PhoneHeight / 2) - 200));
            pauseMenu = new Menu(pauseButtons);

            tutorialButtons = new List<MenuButton>();
            tutorialButtons.Add(new MenuButton("Tutorial", "Buttons/Tutorial", 0, 0));
            tutorialMenu = new Menu(tutorialButtons);
            #endregion MENU

            #region GAMING
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
            #endregion GAMING

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

        #region Sockets Helper Methods

        void InitializeSockets()
        {
            Channel = new UdpAnySourceMulticastChannel();
            Channel.GroupPacketReceived += new EventHandler<UdpPacketReceivedEventArgs>(Channel_GroupPacketReceived);
            Channel.SinglePacketReceived += new EventHandler<UdpPacketReceivedEventArgs>(Channel_SinglePacketReceived);
            Channel.GroupOpen();
        }

        void Channel_SinglePacketReceived(object sender, UdpPacketReceivedEventArgs e)
        {
            recievedData = e.Message.Trim('\0');
            if (recievedData.Contains(" found you!"))
            {
                Channel.GroupClose();
                Channel.SingleOpen();
                Channel.SingleSourceEndPoint = e.Source;
            }
        }

        void Channel_GroupPacketReceived(object sender, UdpPacketReceivedEventArgs e)
        {
            recievedData = e.Message.Trim('\0');
            Channel.SingleSourceEndPoint = e.Source;
            if (Channel.SingleSourceEndPoint != null)
            {
                Channel.SingleOpen();
                Channel.GroupClose();
            }
        }

        void SendPosition(string data)
        {
            if (Channel.SingleSourceEndPoint != null && UdpAnySourceMulticastChannel.SingleIsJoined)
            {
                Channel.SingleSendToSource(guid + " found you!");
            }
            else
            {
                Channel.GroupSend(guid + " is searching pair...");
            }
        } 

        #endregion

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            baseFont = this.Content.Load<SpriteFont>("Fonts/ModifierNotification");
            mainMenu.LoadButtonList(this);
            pauseMenu.LoadButtonList(this);
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
            switch (gameState)
            {
                case GameState.Gaming:
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
                                Transitions.ChangeGameState(ref firstEntry);
                                gameState = GameState.InEndGameMenu;
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
                    break;
                case GameState.NetworkGaming:
                    if (firstEntry)
                    {
                        InitializeSockets();
                        firstEntry = false;
                    }
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) gameState = GameState.InMainMenu;
                    SendPosition(guid.ToString());
                    break;
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
            //NOTE: NETWORK GAMING
            else if (gameState == GameState.NetworkGaming)
            {
                spriteBatch.DrawString(baseFont, "My Guid: " + guid.ToString(), new Vector2(0, 0), Color.White);
                spriteBatch.DrawString(baseFont, "Other: " + recievedData, new Vector2(0, 30), Color.White);
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
