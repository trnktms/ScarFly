using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ScarFly.MyClasses.PlayerClasses;

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
        public Player(string name, int velocity, float positionX, float positionY, string runAssetName, string flyAssetName, string fallAssetName, int runMoveCount, int flyMoveCount, int fallMoveCount)
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
            isDead = false;
            isEatMoney = false;
            Score = new Score(Consts.p_MoneyIcon, Consts.p_MoneyIcon, Consts.sf_GameScore, Consts.sf_GameScore);
            this.Velocity = velocity;
            PositionHistory = new Queue<Vector2>(20);
            _fly_sy = (int)Position.Y;
        }

        public string Name { get; set; }
        public PlayerStates PlayerState { get; set; }
        public bool isDead { get; set; }
        public bool isEatMoney { get; set; }

        Queue<Vector2> PositionHistory;
        Texture2D _line;
        public Score Score { get; set; }
        public int Velocity { get; set; }

        private int ZeroPositionY { get; set; }
        public Vector2 Position { get; set; }

        public string RunAssetName { get; set; }
        public string FlyAssetName { get; set; }
        public string FallAssetName { get; set; }
        public Texture2D RunTexture { get; set; }
        public Texture2D FlyTexture { get; set; }
        public Texture2D FallTexture { get; set; }
        public Rectangle RunBound { get; set; }
        public Rectangle FlyBound { get; set; }
        public Rectangle FallBound { get; set; }
        public Color[] RunColorData { get; set; }
        public Color[] FlyColorData { get; set; }
        public Color[] FallColorData { get; set; }
        public int RunMoveCount { get; set; }
        public int FlyMoveCount { get; set; }
        public int FallMoveCount { get; set; }
        public int RunMoveWidth { get; set; }
        public int FlyMoveWidth { get; set; }
        public int FallMoveWidth { get; set; }

        public void Load(Game1 game) 
        { 
            RunTexture = game.Content.Load<Texture2D>(RunAssetName);
            FlyTexture = game.Content.Load<Texture2D>(FlyAssetName);
            FallTexture = game.Content.Load<Texture2D>(FallAssetName);

            RunColorData = new Color[RunTexture.Width * RunTexture.Height];
            FlyColorData = new Color[FlyTexture.Width * FlyTexture.Height];
            FallColorData = new Color[FallTexture.Width * FallTexture.Height];

            RunTexture.GetData(RunColorData);
            FlyTexture.GetData(FlyColorData);
            FallTexture.GetData(FallColorData);

            RunMoveWidth = RunTexture.Width / this.RunMoveCount;
            FlyMoveWidth = FlyTexture.Width / this.FlyMoveCount;
            FallMoveWidth = FallTexture.Width / this.FallMoveCount;

            UpdateRectangle();
            Score.Load(game);

            _line = game.Content.Load<Texture2D>(Consts.p_Pixel);
        }

        public void UpdateRectangle()
        {
            RunBound = new Rectangle((int)Position.X, (int)Position.Y, RunMoveWidth, RunTexture.Height);
            FlyBound = new Rectangle((int)Position.X, (int)Position.Y, FlyMoveWidth, FlyTexture.Height);
            FallBound = new Rectangle((int)Position.X, (int)Position.Y, FallMoveWidth, FallTexture.Height);
        }

        public void Run(SpriteBatch spriteBatch)
        {
            Position = new Vector2(Position.X, Position.Y);
            UpdateRectangle();
            Animate(spriteBatch, RunMoveCount);
        }

        private int _fall_vy, _fall_sy;
        private int _fall_ay = 1;
        public void Fall(SpriteBatch spriteBatch)
        {
            _fall_vy += _fall_ay;
            _fall_sy += _fall_vy;

            Position = new Vector2(Position.X, _fall_sy);

            _fly_sy = (int)Position.Y;
            _fly_vy = 0;

            UpdateRectangle();
            Animate(spriteBatch, FallMoveCount);
            if (_fall_sy > ZeroPositionY || _fall_sy > ZeroPositionY - 12)
            {
                //Position = new Vector2(Position.X, ZeroPositionY);
                PlayerState = PlayerStates.Running;
            }
        }

        private int _fly_vy, _fly_sy;
        private int _fly_ay = 1;
        public void Fly(SpriteBatch spriteBatch)
        {
            if (Position.Y > 0)
            {
                if (_fly_vy < 10) { _fly_vy += _fly_ay; }
                _fly_sy -= _fly_vy;
                Position = new Vector2(Position.X, _fly_sy);
                UpdateRectangle();
                _fall_sy = (int)Position.Y;
                _fall_vy = 0;
            }
            Animate(spriteBatch, FlyMoveCount);
        }


        private int _animateCount = 0;
        public void Animate(SpriteBatch spriteBatch, int moveCount)
        {
            Color overlayer = Color.White;
            if (isDead) { overlayer = Color.Red; Score.GameScore--;  }
            else if (isEatMoney) { overlayer = Color.Green; Score.GameScore++; }

            DrawHistory(spriteBatch);

            if (_animateCount < moveCount - 1)
            {
                switch (PlayerState)
                {
                    case PlayerStates.Running:
                        spriteBatch.Draw(RunTexture, Position, new Rectangle((int)(RunMoveWidth * _animateCount), 0, (int)RunMoveWidth, (int)RunTexture.Height), overlayer);
                        break;
                    case PlayerStates.Flying:
                        spriteBatch.Draw(FlyTexture, Position, new Rectangle((int)(FlyMoveWidth * _animateCount), 0, (int)FlyMoveWidth, (int)FlyTexture.Height), overlayer);
                        break;
                    case PlayerStates.Falling:
                        spriteBatch.Draw(FallTexture, Position, new Rectangle((int)(FallMoveWidth * _animateCount), 0, (int)FallMoveWidth, (int)FallTexture.Height), overlayer);
                        break;
                    default:
                        break;
                }
                _animateCount++;
            }
            else
            {
                switch (PlayerState)
                {
                    case PlayerStates.Running:
                        spriteBatch.Draw(RunTexture, Position, new Rectangle((int)(RunMoveWidth * (moveCount - 1)), 0, (int)RunMoveWidth, (int)RunTexture.Height), overlayer);
                        break;
                    case PlayerStates.Flying:
                        spriteBatch.Draw(FlyTexture, Position, new Rectangle((int)(FlyMoveWidth * (moveCount - 1)), 0, (int)FlyMoveWidth, (int)FlyTexture.Height), overlayer);
                        break;
                    case PlayerStates.Falling:
                        spriteBatch.Draw(FallTexture, Position, new Rectangle((int)(FallMoveWidth * (moveCount - 1)), 0, (int)FallMoveWidth, (int)FallTexture.Height), overlayer);
                        break;
                    default:
                        break;
                }
                _animateCount = 0;
            }
        }

        private void DrawHistory(SpriteBatch spriteBatch)
        {
            PositionHistory.Enqueue(Position);
            int i = 20;
            foreach (var item in PositionHistory)
            {
                spriteBatch.Draw(_line, new Vector2(item.X - i * Velocity, item.Y + RunTexture.Height / 2), Color.White);
                i--;
            }

            if (PositionHistory.Count == 20) { PositionHistory.Dequeue(); }
        }

        public void RePosition()
        {
            Position = new Vector2(Position.X, ZeroPositionY);
            PlayerState = PlayerStates.Running;
            Score.GameScore = 0;
            PositionHistory.Clear();
        }
    }
}
