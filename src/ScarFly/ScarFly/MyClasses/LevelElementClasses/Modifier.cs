using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScarFly.MyClasses.Common;
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
            this.Position = new Vector2(this.Index.Column * (Consts.PhoneWidth / Consts.PhoneWidthRate), (this.Index.Row) * (Consts.PhoneHeight / Consts.PhoneHeightRate));
            this.StartPosition = Position;
        }

        public ModifierIndex Index { get; set; }
        public bool IsCatched { get; set; }
        public bool IsHidden { get; set; }
        private float _catchCounter = 1;
        public void DrawCatched(SpriteBatch spriteBatch, Color color)
        {
            this._catchCounter -= 0.1f;
            if (this._catchCounter > 0)
            {
                spriteBatch.Draw(this.Texture, this.Position, new Rectangle((int)(this.MoveWidth * this.animateCount), 0, (int)(this.MoveWidth), (int)(this.Texture.Height)), color * this._catchCounter);
            }
            else
            {
                this.IsHidden = true;
            }
        }
    }
}
