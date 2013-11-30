using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ScarFly.MyClasses.NetworkClasses
{
    public class OtherPlayer
    {
        public OtherPlayer(string assetName)
        {
            this.AssetName = assetName;
            this.Score = 0;
        }

        public string AssetName { get; set; }
        public SpriteFont Font { get; set; }
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public int Score { get; set; }

        public void Load(Game1 game)
        {
            Texture = game.Content.Load<Texture2D>(AssetName);
            Font = game.Content.Load<SpriteFont>(Consts.SF_GameScore);
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(Texture, Position, color);
            spriteBatch.DrawString(Font, Score.ToString(), new Vector2(0, 80), color);
        }
    }
}
