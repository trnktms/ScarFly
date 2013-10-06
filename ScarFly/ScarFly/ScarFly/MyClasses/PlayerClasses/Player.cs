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
        public Player(string name, float positionX, float positionY, string runAssetName, string flyAssetName, string fallAssetName, int runMoveCount, int flyMoveCount, int fallMoveCount)
        {
            this.Name = name;
            this.RunAssetName = runAssetName;
            this.FlyAssetName = flyAssetName;
            this.FallAssetName = fallAssetName;
            this.Position = new Vector2(positionX, positionY);
            this.ZeroPositionY = (int)positionY;
            PlayerState = PlayerStates.Running;

            this.RunMoveCount = runMoveCount;
            this.FlyMoveCount = flyMoveCount;
            this.FallMoveCount = fallMoveCount;
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
        public int RunMoveCount { get; set; }
        public string FlyAssetName { get; set; }
        public Texture2D FlyTexture { get; set; }
        public int FlyMoveCount { get; set; }
        public string FallAssetName { get; set; }
        public Texture2D FallTexture { get; set; }
        public int FallMoveCount { get; set; }

        private int _runMoveWidth { get; set; }
        private int _flyMoveWidth { get; set; }
        private int _fallMoveWidth { get; set; }

        public void Load(Game1 game) 
        { 
            RunTexture = game.Content.Load<Texture2D>(RunAssetName);
            FlyTexture = game.Content.Load<Texture2D>(FlyAssetName);
            FallTexture = game.Content.Load<Texture2D>(FallAssetName);

            _runMoveWidth = RunTexture.Width / this.RunMoveCount;
            _flyMoveWidth = FlyTexture.Width / this.FlyMoveCount;
            _fallMoveWidth = FallTexture.Width / this.FallMoveCount;
        }

        public void Run(SpriteBatch spriteBatch)
        {
            Position = new Vector2(Position.X, Position.Y);
            Animate(spriteBatch, RunMoveCount);
        }

        private int _vy, _sy;
        private int _ay = 1;
        public void Fall(SpriteBatch spriteBatch)
        {
            _vy = _vy + _ay;
            _sy = _sy + _vy;
            Position = new Vector2(Position.X, _sy);
            Animate(spriteBatch, FallMoveCount);
            if (_sy > ZeroPositionY || _sy > ZeroPositionY - 12)
            {
                Position = new Vector2(Position.X, ZeroPositionY);
                PlayerState = PlayerStates.Running;
            }
        }

        public void Fly(SpriteBatch spriteBatch)
        {
            if (Position.Y > 0)
            {
                Position = new Vector2(Position.X, Position.Y - 7);
                _sy = (int)Position.Y;
                _vy = 0;
                _ay = 1;
            }
            Animate(spriteBatch, FlyMoveCount);
        }


        private int _animateCount = 0;
        public void Animate(SpriteBatch spriteBatch, int moveCount)
        {
            if (_animateCount < moveCount - 1)
            {
                switch (PlayerState)
                {
                    case PlayerStates.Running: 
                        spriteBatch.Draw(RunTexture, Position, new Rectangle((int)(_runMoveWidth * _animateCount), 0, (int)_runMoveWidth, (int)RunTexture.Height), Color.White);
                        break;
                    case PlayerStates.Flying:
                        spriteBatch.Draw(FlyTexture, Position, new Rectangle((int)(_flyMoveWidth * _animateCount), 0, (int)_flyMoveWidth, (int)FlyTexture.Height), Color.White);
                        break;
                    case PlayerStates.Falling:
                        spriteBatch.Draw(FallTexture, Position, new Rectangle((int)(_fallMoveWidth * _animateCount), 0, (int)_fallMoveWidth, (int)FallTexture.Height), Color.White);
                        break;
                    default:
                        break;
                }
                ++_animateCount;
            }
            else
            {
                switch (PlayerState)
                {
                    case PlayerStates.Running:
                        spriteBatch.Draw(RunTexture, Position, new Rectangle((int)(_runMoveWidth * (moveCount - 1)), 0, (int)_runMoveWidth, (int)RunTexture.Height), Color.White);
                        break;
                    case PlayerStates.Flying:
                        spriteBatch.Draw(FlyTexture, Position, new Rectangle((int)(_flyMoveWidth * (moveCount - 1)), 0, (int)_flyMoveWidth, (int)FlyTexture.Height), Color.White);
                        break;
                    case PlayerStates.Falling:
                        spriteBatch.Draw(FallTexture, Position, new Rectangle((int)(_fallMoveWidth * (moveCount - 1)), 0, (int)_fallMoveWidth, (int)FallTexture.Height), Color.White);
                        break;
                    default:
                        break;
                }
                _animateCount = 0;
            }
        }
    }
}
