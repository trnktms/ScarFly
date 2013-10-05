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
        List<MenuButton> buttons = new List<MenuButton>();

        Player player;
        PlayerBackground backBackground;
        PlayerBackground foreBackground;
        Barriers barriers;

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
            phoneHeight = graphics.PreferredBackBufferHeight;
            phoneWidth = graphics.PreferredBackBufferWidth;

            gameState = GameState.InMainMenu;
            buttons = new List<MenuButton>();
            buttons.Add(new MenuButton("Start","Buttons/testButton", 10, 10));
            mainMenu = new MainMenu(buttons);

            player = new Player("Player1", 100, 390, "Player/circle", "Player/circle", "Player/circle");
            backBackground = new PlayerBackground("Background/Forest", 1);
            foreBackground = new PlayerBackground("Background/ForestFore", 3);
            barriers = new Barriers("level_1", 5, phoneWidth, phoneHeight);
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
            player.Load(this);
            backBackground.Load(this);
            foreBackground.Load(this);
            barriers.Load(this);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {            
            if (gameState == GameState.Gaming)
            {
                mainMenu = new MainMenu(null);
                backBackground.Scroll(this);
                foreBackground.Scroll(this);
                barriers.Scroll(this);

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
                    gameState = GameState.InMainMenu;
                    mainMenu = new MainMenu(buttons);
                }
            }
            else if (gameState == GameState.InMainMenu)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();
                backBackground.Scroll(this);
                foreBackground.Scroll(this);
                gameState = mainMenu.IsTouched(this, TouchPanel.GetState(), gameState);
            }
            else if (gameState == GameState.InScoreMenu)
            {
                    
            }
            else if (gameState == GameState.Invalid)
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
                barriers.Draw(spriteBatch);
                if (player.PlayerState == PlayerStates.Running)
                {
                    player.Run(spriteBatch, 1);
                }
                else if (player.PlayerState == PlayerStates.Flying)
                {
                    player.Fly(spriteBatch, 1);
                }
                else if (player.PlayerState == PlayerStates.Falling)
                {
                    player.Fall(spriteBatch, 1);
                }
            }
            else if (gameState == GameState.InScoreMenu)
            {
                GraphicsDevice.Clear(Color.Red);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
