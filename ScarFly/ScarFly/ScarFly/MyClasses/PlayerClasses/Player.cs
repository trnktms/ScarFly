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
        public Player(string name, int velocity, float positionX, float positionY, string runAssetName, int runMoveCount)
        {
            this.Name = name;
            this.RunAssetName = runAssetName;
            this.Position = new Vector2(positionX, positionY);
            this.ZeroPosition = new Vector2(positionX, positionY);
            this.PlayerState = PlayerStates.Running;
            this.RunMoveCount = runMoveCount;
            this.isDead = false;
            this.isEatMoney = false;
            this.isEnd = false;
            this.Score = new Score(Consts.P_MoneyIcon, Consts.P_MoneyIcon, Consts.P_RankIcon, Consts.SF_GameScore);
            this.Velocity = velocity;
            this.StartVelocity = velocity;
            this.PositionHistory = new Queue<Vector2>(20);
            this._fly_sy = (int)Position.Y;
        }

        public string Name { get; set; }
        public PlayerStates PlayerState { get; set; }
        public bool isDead { get; set; }
        public bool isEatMoney { get; set; }
        public bool isEatModifier { get; set; }
        public bool isEnd { get; set; }

        Queue<Vector2> PositionHistory;
        private Texture2D _line;
        public Score Score { get; set; }
        public int Velocity { get; set; }
        public int StartVelocity { get; set; }
        public Color Overlayer { get; set; }

        private Vector2 ZeroPosition { get; set; }
        public Vector2 Position { get; set; }

        public string RunAssetName { get; set; }
        public Texture2D RunTexture { get; set; }
        public Rectangle RunBound { get; set; }
        public Color[] RunColorData { get; set; }
        public int RunMoveCount { get; set; }
        public int RunMoveWidth { get; set; }

        public void Load(Game1 game) 
        { 
            RunTexture = game.Content.Load<Texture2D>(RunAssetName);
            RunColorData = new Color[RunTexture.Width * RunTexture.Height];
            RunTexture.GetData(RunColorData);
            RunMoveWidth = RunTexture.Width / this.RunMoveCount;
            UpdateRectangle();
            Score.Load(game);
            _line = game.Content.Load<Texture2D>(Consts.P_Pixel);
        }

        public void UpdateRectangle()
        {
            RunBound = new Rectangle((int)Position.X, (int)Position.Y, RunMoveWidth, RunTexture.Height);
        }

        public void Update()
        {
            if (isDead) { Overlayer = new Color(255, 105, 97); Score.GameScore--; }
            else if (isEatMoney) { Score.GameScore += 10; }

            if (isEnd) { Position = new Vector2(Position.X + Velocity, Position.Y); }

            while (TouchPanel.IsGestureAvailable)
            {
                var gesture = TouchPanel.ReadGesture();
                if (gesture.GestureType == GestureType.Tap && (PlayerState == PlayerStates.Falling || PlayerState == PlayerStates.Running))
                    PlayerState = PlayerStates.Flying;
                else if (gesture.GestureType == GestureType.Tap && PlayerState == PlayerStates.Flying)
                    PlayerState = PlayerStates.Falling;
            }
        }

        public void Run(SpriteBatch spriteBatch, Color color)
        {
            Position = new Vector2(Position.X, Position.Y);
            Animate(spriteBatch, RunMoveCount, color);
        }

        private const int _max_vy = 8;
        private int _fall_vy, _fall_sy;
        private int _fall_ay = 1;
        public void Fall(SpriteBatch spriteBatch, Color color)
        {
            if (_fall_vy < _max_vy) { _fall_vy += _fall_ay; }
            _fall_sy += _fall_vy;
            Position = new Vector2(Position.X, _fall_sy);
            _fly_sy = (int)Position.Y;
            _fly_vy = 0;
            if (_fall_sy > ZeroPosition.Y || _fall_sy > ZeroPosition.Y - 12) { PlayerState = PlayerStates.Flying; }
            Animate(spriteBatch, RunMoveCount, color);
        }

        private int _fly_vy, _fly_sy;
        private int _fly_ay = 1;
        public void Fly(SpriteBatch spriteBatch, Color color)
        {
            if (Position.Y > 5)
            {
                if (_fly_vy < _max_vy) { _fly_vy += _fly_ay; }
                _fly_sy -= _fly_vy;
                Position = new Vector2(Position.X, _fly_sy);
                _fall_sy = (int)Position.Y;
                _fall_vy = 0;
            }
            else { PlayerState = PlayerStates.Falling; }
            Animate(spriteBatch, RunMoveCount, color);
        }

        private int _animateCount = 0;
        public void Animate(SpriteBatch spriteBatch, int moveCount, Color color)
        {
            UpdateRectangle();
            DrawHistory(spriteBatch, color);

            if (_animateCount < moveCount - 1)
            {
                spriteBatch.Draw(
                            RunTexture,
                            Position,
                            new Rectangle((int)(RunMoveWidth * _animateCount), 0, (int)RunMoveWidth, (int)RunTexture.Height),
                            color);
                _animateCount++;
            }
            else
            {
                spriteBatch.Draw(
                            RunTexture,
                            Position,
                            new Rectangle((int)(RunMoveWidth * (moveCount - 1)), 0, (int)RunMoveWidth, (int)RunTexture.Height),
                            color);
                _animateCount = 0;
            }
        }

        private int _positionHistoryMax = 20;
        private void DrawHistory(SpriteBatch spriteBatch, Color color)
        {
            PositionHistory.Enqueue(Position); int i = _positionHistoryMax;
            foreach (var item in PositionHistory)
            {
                spriteBatch.Draw(_line, new Vector2(item.X - i * StartVelocity, item.Y + RunTexture.Height / 2), color);
                i--;
            }
            if (PositionHistory.Count == _positionHistoryMax) { PositionHistory.Dequeue(); }
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            switch (PlayerState)
            {
                case PlayerStates.Running: Run(spriteBatch, isDead ? Overlayer : color);
                    break;
                case PlayerStates.Flying: Fly(spriteBatch, isDead ? Overlayer : color);
                    break;
                case PlayerStates.Falling: Fall(spriteBatch, isDead ? Overlayer : color);
                    break;
                default:
                    break;
            }
        }

        public void RePosition()
        {
            Position = ZeroPosition;
            PlayerState = PlayerStates.Running;
            Score.TotalScore += Score.GameScore;
            Score.GameScore = 0;
            PositionHistory.Clear();
            _fly_sy = (int)Position.Y;
            _fall_sy = 0;
            isEnd = false;
            isDead = false;
            isEatMoney = false;
            isEatModifier = false;
            Velocity = StartVelocity;
        }
    }
}
