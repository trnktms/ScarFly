using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ScarFly.MyClasses.PlayerClasses;
using Microsoft.Devices;
namespace ScarFly.MyClasses.PlayerClasses
{
    public enum PlayerStates
    {
        InOneAltitude,
        Flying,
        Falling
    }

    public class Player
    {
        public Player(string name, int velocity, float positionX, float positionY, string runAssetName, int runMoveCount)
        {
            this.Name = name;
            this.AssetName = runAssetName;
            this.Position = new Vector2(positionX, positionY);
            this.ZeroPosition = new Vector2(positionX, positionY);
            this.PlayerState = PlayerStates.InOneAltitude;
            this.MoveCount = runMoveCount;
            this.isDead = false;
            this.isEatMoney = false;
            this.isEnd = false;
            this.Score = new Score(Consts.P_MoneyIcon, Consts.P_MoneyIcon, Consts.P_RankIcon, Consts.SF_GameScore);
            this.Velocity = velocity;
            this.StartVelocity = velocity;
            this.PositionHistory = new Queue<Vector2>(20);
            this._fly_sy = (int)Position.Y;
            this.Id = Guid.NewGuid();
        }

        public string Name { get; set; }
        public PlayerStates PlayerState { get; set; }
        public bool isDead { get; set; }
        public bool isEatMoney { get; set; }
        public bool isEatModifier { get; set; }
        public bool isEnd { get; set; }
        public Guid Id { get; set; }

        Queue<Vector2> PositionHistory;
        private Texture2D _line;
        public Score Score { get; set; }
        public int Velocity { get; set; }
        public int StartVelocity { get; set; }
        public int Distance { get; set; }
        public Color Overlayer { get; set; }
        public double Time { get; set; }

        private Vector2 ZeroPosition { get; set; }
        public Vector2 Position { get; set; }

        public string AssetName { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle RunBound { get; set; }
        public Color[] ColorData { get; set; }
        public int MoveCount { get; set; }
        public int MoveWidth { get; set; }

        private const int _max_vy = 8;
        private int _fall_ay = 1;
        private int _fly_ay = 1;
        private int _fall_vy, _fall_sy;
        private int _fly_vy, _fly_sy;
        private int _animateCount = 0;
        private int _positionHistoryMax = 20;
        private VibrateController vibrateController = VibrateController.Default;

        public void Load(Game1 game) 
        { 
            Texture = game.Content.Load<Texture2D>(AssetName);
            ColorData = new Color[Texture.Width * Texture.Height];
            Texture.GetData(ColorData);
            MoveWidth = Texture.Width / this.MoveCount;
            UpdateRectangle();
            Score.Load(game);
            _line = game.Content.Load<Texture2D>(Consts.P_Pixel);
        }

        public void UpdateRectangle()
        {
            RunBound = new Rectangle((int)Position.X, (int)Position.Y, MoveWidth, Texture.Height);
        }

        public void Update()
        {
            if (isDead) { Overlayer = new Color(255, 105, 97); Score.GameScore--; vibrateController.Start(TimeSpan.FromMilliseconds(50)); }
            else if (isEatMoney) { Score.GameScore += 10; }

            if (isEnd) { Position = new Vector2(Position.X + Velocity, Position.Y); }

            TouchCollection touches = TouchPanel.GetState();
            foreach (var touch in touches)
            {
                if (touch.State == TouchLocationState.Pressed)
                {
                    PlayerState = PlayerStates.Flying;
                }
                else if (touch.State == TouchLocationState.Released)
                {
                    PlayerState = PlayerStates.Falling;
                }
            }

            Distance += Velocity;
        }

        public void InOneAltitude(SpriteBatch spriteBatch, Color color)
        {
            Position = new Vector2(Position.X, Position.Y);
            Animate(spriteBatch, MoveCount, color);
        }

        public void Fall(SpriteBatch spriteBatch, Color color)
        {
            if (_fall_vy < _max_vy) { _fall_vy += _fall_ay; }
            _fall_sy += _fall_vy;
            Position = new Vector2(Position.X, _fall_sy);
            _fly_sy = (int)Position.Y;
            _fly_vy = 0;
            if (_fall_sy > ZeroPosition.Y || _fall_sy > ZeroPosition.Y - 12) 
            { 
                PlayerState = PlayerStates.InOneAltitude; 
            }
            Animate(spriteBatch, MoveCount, color);
        }

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
            Animate(spriteBatch, MoveCount, color);
        }

        public void Animate(SpriteBatch spriteBatch, int moveCount, Color color)
        {
            UpdateRectangle();
            DrawHistory(spriteBatch, color);

            spriteBatch.Draw(
                Texture,
                Position,
                new Rectangle((int)(MoveWidth * _animateCount), 0, (int)MoveWidth, (int)Texture.Height),
                color);

            if (_animateCount < moveCount - 1) { _animateCount++; }
            else { _animateCount = 0; }
        }

        private void DrawHistory(SpriteBatch spriteBatch, Color color)
        {
            PositionHistory.Enqueue(Position); int i = _positionHistoryMax;
            foreach (var item in PositionHistory)
            {
                spriteBatch.Draw(_line, new Vector2(item.X - i * StartVelocity, item.Y + Texture.Height / 2), color);
                i--;
            }
            if (PositionHistory.Count == _positionHistoryMax) { PositionHistory.Dequeue(); }
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            switch (PlayerState)
            {
                case PlayerStates.InOneAltitude: InOneAltitude(spriteBatch, isDead ? Overlayer : color);
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
            PlayerState = PlayerStates.InOneAltitude;
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
            Distance = 0;
        }
    }
}
