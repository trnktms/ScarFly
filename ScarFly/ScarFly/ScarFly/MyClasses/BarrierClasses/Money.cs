﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.BarrierClasses
{
    public class Money
    {
        public Money(string assetName, MoneyIndex index, int phoneWidth, int phoneHeight)
        {
            this.AssetName = assetName;
            this.Index = index;
            Position = new Vector2(this.Index.Column * (phoneWidth / Consts.PhoneWidthRate), (this.Index.Row) * (phoneHeight / Consts.PhoneHeightRate));
            StartPosition = Position;
        }

        public MoneyIndex Index { get; set; }
        public string AssetName { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle Bound { get; set; }
        public Color[] ColorData { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 StartPosition { get; set; }

        public void Load(Game1 game)
        {
            Texture = game.Content.Load<Texture2D>(AssetName);
            UpdateRectangle();
            ColorData = new Color[Texture.Width * Texture.Height];
            Texture.GetData(ColorData);
        }

        public void UpdateRectangle()
        {
            Bound = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        }
    }
}