using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.BarrierClasses
{
    public class BarrierTexture
    {
        public BarrierTexture(int id, string assetName)
        {
            this.ID = id;
            this.AssetName = assetName;
        }

        public int ID { get; set; }
        public string AssetName { get; set; }
        public Texture2D Texture { get; set; }

        public void Load(Game1 game) { Texture = game.Content.Load<Texture2D>(AssetName); }
    }
}
