using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.LevelElementClasses
{
    public class Money : LevelElement
    {
        public Money(string assetName, MoneyIndex index, int moveCount)
            :base(assetName, moveCount)
        {
            this.Index = index;
            Position = new Vector2(this.Index.Column * (Consts.PhoneWidth / Consts.PhoneWidthRate), (this.Index.Row) * (Consts.PhoneHeight / Consts.PhoneHeightRate));
            StartPosition = Position;
        }

        public MoneyIndex Index { get; set; }
        public bool IsCatched { get; set; }
    }
}
