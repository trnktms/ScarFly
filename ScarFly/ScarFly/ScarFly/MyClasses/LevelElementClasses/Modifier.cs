using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.LevelElementClasses
{
    public class Modifier : LevelElement
    {
        public Modifier(string assetName, ModifierIndex index, int moveCount)
            : base(assetName, moveCount)
        {
            this.Index = index;
            Position = new Vector2(this.Index.Column * (Consts.PhoneWidth / Consts.PhoneWidthRate), (this.Index.Row) * (Consts.PhoneHeight / Consts.PhoneHeightRate));
            StartPosition = Position;
        }

        public ModifierIndex Index { get; set; }
        public bool IsCatched { get; set; }
        public bool IsHidden { get; set; }
        private float _catchCounter = 1;
        public void DrawCatched(SpriteBatch spriteBatch, Color color)
        {
            _catchCounter -= 0.1f;
            if (_catchCounter > 0)
            {
                spriteBatch.Draw(Texture, Position, new Rectangle((int)(MoveWidth * animateCount), 0, (int)(MoveWidth * _catchCounter), (int)(Texture.Height * _catchCounter)), color);
            }
            else
            {
                IsHidden = true;
            }
        }
    }
}
