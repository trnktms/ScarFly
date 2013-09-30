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

namespace ScarFly
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        MainMenu mainMenu;
        List<MenuButton> buttons = new List<MenuButton>();

        Player player;
        PlayerBackground background;

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

            gameState = GameState.InMainMenu;
            buttons = new List<MenuButton>();
            buttons.Add(new MenuButton("Start","Buttons/testButton", 10, 10));
            buttons.Add(new MenuButton("Scores", "Buttons/testButton", 400, 10));
            mainMenu = new MainMenu(buttons);
            player = new Player("Player1", 100, 390, "Player/circle", "Player/circle", "Player/circle");
            background = new PlayerBackground("Background/testBackground", 0, 0);
        }

        protected override void Initialize()
        {
            base.Initialize();
            TouchPanel.EnabledGestures = GestureType.Hold | GestureType.Tap | GestureType.None;
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            mainMenu.LoadButtonList(this);
            player.Load(this);
            background.Load(this);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            
            // TODO: Add your update logic here

            if (gameState == GameState.Gaming)
            {
                mainMenu = new MainMenu(null);
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
                    gameState = GameState.InMainMenu;
            }
            else if (gameState == GameState.InMainMenu)
            {
                mainMenu = new MainMenu(buttons);
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();
            }
            else if (gameState == GameState.InScoreMenu)
            {
                    
            }
            else if (gameState == GameState.Invalid)
            {
                
            }

            gameState = mainMenu.IsTouched(this, TouchPanel.GetState(), gameState);

            base.Update(gameTime);
        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            spriteBatch.Begin();
            if (gameState == GameState.InMainMenu)
            {
                mainMenu.DrawButtonList(spriteBatch);
            }
            else if (gameState == GameState.Gaming)
            {
                background.Scroll(this, spriteBatch);
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
