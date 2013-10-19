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
using ScarFly.MyClasses.BarrierClasses;

namespace ScarFly
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        int phoneWidth;
        int phoneHeight;

        MainMenu mainMenu;
        MainMenu pauseMenu;
        List<MenuButton> mainButtons = new List<MenuButton>();
        List<MenuButton> pauseButtons = new List<MenuButton>();

        Player player;
        PlayerBackground backBackground;
        PlayerBackground foreBackground;
        Barriers barriers;
        Moneys moneys;
        Collosion collosion;

        GameState gameState;

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
            phoneHeight = graphics.PreferredBackBufferHeight;
            phoneWidth = graphics.PreferredBackBufferWidth;

            gameState = GameState.InMainMenu;

            mainButtons = new List<MenuButton>();
            mainButtons.Add(new MenuButton("Main_Start", "Buttons/StartButton", (phoneWidth / 2) - 124, (phoneHeight / 2) - 128));
            mainMenu = new MainMenu(mainButtons);

            pauseButtons = new List<MenuButton>();
            pauseButtons.Add(new MenuButton("Pause_Resume", "Buttons/StartButton", 10, 10));
            pauseMenu = new MainMenu(pauseButtons);


            player = new Player("Player1", 100, 370, "Player/AnimatedCircle", "Player/AnimatedCircle", "Player/AnimatedCircle", 16, 16, 16);
            backBackground = new PlayerBackground("Background/Forest3", 1);
            foreBackground = new PlayerBackground("Background/ForestFore", 3);

            barriers = new Barriers("level_1", 3, phoneWidth, phoneHeight);
            moneys = new Moneys("level_1", 3, phoneWidth, phoneHeight);

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

            player.Load(this);

            backBackground.Load(this);
            foreBackground.Load(this);

            barriers.Load(this);
            moneys.Load(this);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (gameState == GameState.InMainMenu)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();
                backBackground.Scroll(this);
                foreBackground.Scroll(this);

                gameState = mainMenu.IsTouched(this, TouchPanel.GetState(), gameState);
            }
            else if (gameState == GameState.Gaming)
            {
                backBackground.Scroll(this);
                foreBackground.Scroll(this);
                barriers.Scroll(this);
                moneys.Scroll(this);
                collosion.Update();
                while (TouchPanel.IsGestureAvailable)
                {
                    var gesture = TouchPanel.ReadGesture();
                    if (gesture.GestureType == GestureType.Tap && (player.PlayerState == PlayerStates.Falling || player.PlayerState == PlayerStates.Running))
                    {
                        player.PlayerState = PlayerStates.Flying;
                    }
                    else if (gesture.GestureType == GestureType.Tap && player.PlayerState == PlayerStates.Flying)
                    {
                        player.PlayerState = PlayerStates.Falling;
                    }
                }

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                    gameState = GameState.InPauseMenu;
                    pauseMenu = new MainMenu(pauseButtons);
                }
            }
            else if (gameState == GameState.InPauseMenu)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                    gameState = GameState.InMainMenu;
                    mainMenu = new MainMenu(mainButtons);
                    barriers.RePosition();
                    moneys.RePosition();
                    player.RePosition();
                }
                gameState = pauseMenu.IsTouched(this, TouchPanel.GetState(), gameState);
            }
            
            else if (gameState == GameState.InScoreMenu)
            {

            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            if (gameState == GameState.InMainMenu)
            {
                backBackground.Draw(spriteBatch);
                foreBackground.Draw(spriteBatch);
                mainMenu.DrawButtonList(spriteBatch);
            }
            else if (gameState == GameState.Gaming)
            {
                backBackground.Draw(spriteBatch);
                foreBackground.Draw(spriteBatch);
                switch (player.PlayerState)
                {
                    case PlayerStates.Running: player.Run(spriteBatch);
                        break;
                    case PlayerStates.Flying: player.Fly(spriteBatch);
                        break;
                    case PlayerStates.Falling: player.Fall(spriteBatch);
                        break;
                    default:
                        break;
                }
                barriers.Draw(spriteBatch);
                moneys.Draw(spriteBatch);
            }
            else if (gameState == GameState.InPauseMenu)
            {
                backBackground.Draw(spriteBatch);
                foreBackground.Draw(spriteBatch);
                barriers.Draw(spriteBatch);
                moneys.Draw(spriteBatch);
                pauseMenu.DrawButtonList(spriteBatch);
            }
            else if (gameState == GameState.InScoreMenu)
            {

            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
