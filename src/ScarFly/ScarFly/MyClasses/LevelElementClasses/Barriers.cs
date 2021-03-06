﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScarFly.MyClasses.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.LevelElementClasses
{
    public class Barriers : LevelElements
    {
        public Barriers(string levelName, int velocity)
            : base(levelName, velocity) { }

        public List<Barrier> BarrierList { get; set; }

        public override void ProcLevelFile()
        {
            this.BarrierList = new List<Barrier>();
            List<string> rows = new List<string>();

            using (var stream = TitleContainer.OpenStream(LevelName))
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null) { rows.Add(line); }
                }
            }

            for (int i = 0; i < rows.Count; i++)
            {
                for (int j = 0; j < rows[i].Length; j++)
                {
                    int id = 0;
                    if (int.TryParse(rows[i][j].ToString(), out id))
                    {
                        var barrierAssetName = string.Empty;
                        switch (id)
                        {
                            case 1:
                                barrierAssetName = "LevelElements/PinWheelMini";
                                break;
                            case 2:
                                barrierAssetName = "LevelElements/PinWheelBig";
                                break;
                            case 3:
                                barrierAssetName = "LevelElements/PinWheelMiniDown";
                                break;
                            case 4:
                                barrierAssetName = "LevelElements/PinWheelBigDown";
                                break;
                            default:
                                break;
                        }

                        this.BarrierList.Add(new Barrier(barrierAssetName,
                            new BarrierIndex(j, i, id),
                            20));
                    }
                }
            }
        }

        public override void Load(Game1 game)
        {
            var barriers = new List<Barrier>();
            foreach (var item in this.BarrierList)
            {
                var barrier = barriers.SingleOrDefault(b => b.AssetName == item.AssetName);
                if (barrier != null)
                {
                    item.Load(game, barrier);
                }
                else
                {
                    barriers.Add(item);
                    item.Load(game);
                }
            }
        }

        public override void RePosition(Game1 game)
        {
            foreach (var item in this.BarrierList)
            {
                item.Position = item.StartPosition;
            }
        }

        public override void Scroll(Game1 game)
        {
            foreach (var barrierItem in this.BarrierList.Where(p => p.Position.X >= -p.Texture.Width))
            {
                barrierItem.Position = new Vector2(barrierItem.Position.X - Velocity, barrierItem.Position.Y);
                barrierItem.UpdateRectangle();
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Color color)
        {
            foreach (var barrierItem in this.BarrierList.Where(p => p.Position.X >= -p.Texture.Width && p.Position.X <= Consts.PhoneWidth))
            {
                barrierItem.Draw(spriteBatch, color);
            }
        }
    }
}
