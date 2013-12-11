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
        public int Distance { get; set; }
        public Texture2D Texture { get; set; }
        public Texture2D MoneyIconTexture { get; set; }
        public int Score { get; set; }
        public int Velocity { get; set; }
        public Guid Id { get; set; }

        public void Load(Game1 game)
        {
            Texture = game.Content.Load<Texture2D>(AssetName);
            MoneyIconTexture = game.Content.Load<Texture2D>(Consts.P_MoneyIcon);
            Font = game.Content.Load<SpriteFont>(Consts.SF_GameScore);
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            //spriteBatch.Draw(Texture, new Vector2(0, Consts.PhoneWidth / Distance), color);
            spriteBatch.Draw(MoneyIconTexture, new Vector2(0, Consts.PhoneHeight - 100), color);
            spriteBatch.DrawString(Font, Score.ToString(), new Vector2(MoneyIconTexture.Width + 3, Consts.PhoneHeight - 100 + 3), color);
            spriteBatch.DrawString(Font, Distance.ToString(), new Vector2(0, 80), color);
        }
    }
}
