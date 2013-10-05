using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ScarFly.MyClasses
{
    public enum PlayerStates
    {
        Running,
        Flying,
        Falling
    }

    public class Player
    {
        public Player(string name, float positionX, float positionY, string runAssetName, string flyAssetName, string fallAssetName)
        {
            this.Name = name;
            this.RunAssetName = runAssetName;
            this.FlyAssetName = flyAssetName;
            this.FallAssetName = fallAssetName;
            this.Position = new Vector2(positionX, positionY);
            this.ZeroPositionY = (int)positionY;
            PlayerState = PlayerStates.Running;
        }

        public string Name { get; set; }
        public int GameScore { get; set; }
        public int TotalScore { get; set; }
        public List<Dress> DressList { get; set; }
        public List<InjuryStopper> InjuryStopperList { get; set; }

        public PlayerStates PlayerState { get; set; }

        private int ZeroPositionY { get; set; }

        public Vector2 Position { get; set; }
        public string RunAssetName { get; set; }
        public Texture2D RunTexture { get; set; }
        public string FlyAssetName { get; set; }
        public Texture2D FlyTexture { get; set; }
        public string FallAssetName { get; set; }
        public Texture2D FallTexture { get; set; }

        public void Load(Game1 game) 
        { 
            RunTexture = game.Content.Load<Texture2D>(RunAssetName);
            FlyTexture = game.Content.Load<Texture2D>(FlyAssetName);
            FallTexture = game.Content.Load<Texture2D>(FallAssetName); 
        }

        public void Run(SpriteBatch spriteBatch, int moveCount)
        {
            Position = new Vector2(Position.X, Position.Y);
            Animate(spriteBatch, moveCount);
        }

        private int _vy, _sy;
        private int _ay = 1;
        public void Fall(SpriteBatch spriteBatch, int moveCount)
        {
            _vy = _vy + _ay;
            _sy = _sy + _vy;
            Position = new Vector2(Position.X, _sy);
            Animate(spriteBatch, moveCount);
            if (_sy > ZeroPositionY || _sy > ZeroPositionY - 12)
            {
                Position = new Vector2(Position.X, ZeroPositionY);
                PlayerState = PlayerStates.Running;
            }
        }

        public void Fly(SpriteBatch spriteBatch, int moveCount)
        {
            
            if (Position.Y > 0)
            {
                Position = new Vector2(Position.X, Position.Y - 9);
                _sy = (int)Position.Y;
                _vy = 0;
                _ay = 1;
            }
            Animate(spriteBatch, moveCount);
        }


        private int _animateCount = 0;
        public void Animate(SpriteBatch spriteBatch, int moveCount)
        {
            float moveWidth = RunTexture.Width / moveCount;
            if (_animateCount < moveCount - 1)
            {
                spriteBatch.Draw(RunTexture, Position, new Rectangle((int)(moveWidth * _animateCount), 0, (int)moveWidth, (int)RunTexture.Height), Color.White);
                ++_animateCount;
            }
            else
            {
                spriteBatch.Draw(RunTexture, Position, new Rectangle((int)(moveWidth * (moveCount - 1)), 0, (int)moveWidth, (int)RunTexture.Height), Color.White);
                _animateCount = 0;
            }
        }
    }
}
