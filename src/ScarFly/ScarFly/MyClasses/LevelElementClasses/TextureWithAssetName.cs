using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.LevelElementClasses
{
    public class TextureWithAssetName
    {
        public TextureWithAssetName(Texture2D texture, string assetName)
        {
            this.Texture = texture;
            this.AssetName = assetName;
        }

        public Texture2D Texture { get; set; }
        public string AssetName { get; set; }
    }
}
