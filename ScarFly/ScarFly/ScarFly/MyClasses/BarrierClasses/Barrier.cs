using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.BarrierClasses
{
    public class Barrier
    {
        public Barrier(string assetName, BarrierIndex index, int phoneWidth, int phoneHeight)
        {
            this.AssetName = assetName;
            this.Index = index;
            Position = new Vector2(this.Index.Column * (phoneWidth / 4), (this.Index.Row) * (phoneHeight / 5));
            StartPosition = Position;
        }

        public BarrierIndex Index { get; set; }
        public string AssetName { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 StartPosition { get; set; }
    }
}
