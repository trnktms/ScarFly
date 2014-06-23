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
using ScarFly.MyClasses.Helpers;
using ScarFly.MyClasses.MenuClasses;
using ScarFly.MyClasses.Common;
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
            this.Score = new Score(Consts.P_MoneyIcon, Consts.P_MoneyIcon, Consts.P_RankIcon, Consts.P_BestIcon, Consts.SF_BaseFont);
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
            this.Texture = game.Content.Load<Texture2D>(AssetName);
            this.ColorData = new Color[Texture.Width * Texture.Height];
            this.Texture.GetData(ColorData);
            this.MoveWidth = this.Texture.Width / this.MoveCount;
            this.UpdateRectangle();
            this.Score.Load(game);
            this._line = game.Content.Load<Texture2D>(Consts.P_Pixel);
        }

        public void UpdateRectangle()
        {
            this.RunBound = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.MoveWidth, this.Texture.Height);
        }

        public void Update(Game1 game, ref bool firstEntry)
        {
            if (this.isDead)
            {
                this.Overlayer = new Color(255, 105, 97);
                //Score.GameScore--;
                if (Consts.IsVibrate)
                {
                    this.vibrateController.Start(TimeSpan.FromMilliseconds(50));
                }

                game.gameState = GameState.InEndGameMenu;
                Transitions.ChangeGameState(ref firstEntry);

            }
            else if (this.isEatMoney)
            {
                this.Score.GameScore += 10;
            }

            if (this.isEnd)
            {
                this.Position = new Vector2(this.Position.X + this.Velocity, this.Position.Y);
            }

            TouchCollection touches = TouchPanel.GetState();
            foreach (var touch in touches)
            {
                if (touch.State == TouchLocationState.Pressed)
                {
                    this.PlayerState = PlayerStates.Flying;
                }
                else if (touch.State == TouchLocationState.Released)
                {
                    this.PlayerState = PlayerStates.Falling;
                }
            }

            this.Distance += this.Velocity;
        }

        public void InOneAltitude(SpriteBatch spriteBatch, Color color)
        {
            this.Position = new Vector2(this.Position.X, this.Position.Y);
            this.Animate(spriteBatch, this.MoveCount, color);
        }

        public void Fall(SpriteBatch spriteBatch, Color color)
        {
            if (this._fall_vy < _max_vy)
            {
                this._fall_vy += this._fall_ay;
            }

            this._fall_sy += this._fall_vy;
            this.Position = new Vector2(this.Position.X, this._fall_sy);
            this._fly_sy = (int)this.Position.Y;
            this._fly_vy = 0;
            if (this._fall_sy > this.ZeroPosition.Y || this._fall_sy > this.ZeroPosition.Y - 15)
            {
                this.PlayerState = PlayerStates.InOneAltitude;
            }

            this.Animate(spriteBatch, this.MoveCount, color);
        }

        public void Fly(SpriteBatch spriteBatch, Color color)
        {
            if (this.Position.Y > 5)
            {
                if (this._fly_vy < _max_vy)
                {
                    this._fly_vy += _fly_ay;
                }

                this._fly_sy -= this._fly_vy;
                this.Position = new Vector2(this.Position.X, this._fly_sy);
                this._fall_sy = (int)this.Position.Y;
                this._fall_vy = 0;
            }
            else
            {
                this.PlayerState = PlayerStates.Falling;
            }

            this.Animate(spriteBatch, MoveCount, color);
        }

        public void Animate(SpriteBatch spriteBatch, int moveCount, Color color)
        {
            this.UpdateRectangle();
            this.DrawHistory(spriteBatch, color);

            spriteBatch.Draw(
                Texture,
                Position,
                new Rectangle((int)(this.MoveWidth * this._animateCount), 0, (int)this.MoveWidth, (int)this.Texture.Height),
                color);

            if (this._animateCount < moveCount - 1)
            {
                this._animateCount++;
            }
            else
            {
                this._animateCount = 0;
            }
        }

        private void DrawHistory(SpriteBatch spriteBatch, Color color)
        {
            this.PositionHistory.Enqueue(Position); int i = _positionHistoryMax;
            foreach (var item in this.PositionHistory)
            {
                spriteBatch.Draw(this._line, new Vector2(item.X - i * this.StartVelocity, item.Y + this.Texture.Height / 2), color);
                i--;
            }

            if (this.PositionHistory.Count == this._positionHistoryMax)
            {
                this.PositionHistory.Dequeue();
            }
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
            this.Position = ZeroPosition;
            this.PlayerState = PlayerStates.InOneAltitude;
            this.Score.TotalScore += Score.GameScore;
            this.Score.GameScore = 0;
            this.PositionHistory.Clear();
            this._fly_sy = (int)Position.Y;
            this._fall_sy = 0;
            this.isEnd = false;
            this.isDead = false;
            this.isEatMoney = false;
            this.isEatModifier = false;
            this.Velocity = StartVelocity;
            this.Distance = 0;
        }
    }
}
