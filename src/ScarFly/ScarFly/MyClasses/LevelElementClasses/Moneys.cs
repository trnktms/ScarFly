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
    public class Moneys : LevelElements
    {
        public Moneys(string levelName, int velocity)
            : base(levelName, velocity) { }

        public List<Money> MoneyList { get; set; }

        public override void Load(Game1 game)
        {
            var moneys = new List<Money>();
            foreach (var item in MoneyList)
            {
                var money = moneys.SingleOrDefault(b => b.AssetName == item.AssetName);
                if (money != null)
                {
                    item.Load(game, money);
                }
                else
                {
                    moneys.Add(item);
                    item.Load(game);
                }
            }
        }

        public override void RePosition(Game1 game)
        {
            this.ProcLevelFile();
            this.Load(game);
        }

        public override void ProcLevelFile()
        {
            this.MoneyList = new List<Money>();
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
                    string id = rows[i][j].ToString();
                    if (!string.IsNullOrEmpty(id))
                    {
                        switch (id)
                        {
                            case "a":
                                this.MoneyList.Add(new Money("LevelElements/Money", new MoneyIndex(j, i, id), 1));
                                break;
                            case "!":
                                this.MoneyList.Add(new Money("LevelElements/Money", new MoneyIndex(j, i, id), 1));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        public override void Scroll(Game1 game)
        {
            foreach (var moneyItem in this.MoneyList.Where(p => p.Position.X >= -p.Texture.Width && !p.IsHidden))
            {
                moneyItem.Position = new Vector2(moneyItem.Position.X - Velocity, moneyItem.Position.Y);
                moneyItem.UpdateRectangle();
            }
        }

        public List<Money> GetActualMoneyList()
        {
            return this.MoneyList.Where(p => p.Position.X >= -p.Texture.Width && p.Position.X <= Consts.PhoneWidth && !p.IsHidden).ToList();
        }

        public override void Draw(SpriteBatch spriteBatch, Color color)
        {
            foreach (var moneyItem in this.MoneyList.Where(p => p.Position.X >= -p.Texture.Width && p.Position.X <= Consts.PhoneWidth && p.Index.ID != "!" && !p.IsHidden))
            {
                if (moneyItem.IsCatched)
                {
                    moneyItem.DrawCatched(spriteBatch, color);
                }
                else
                {
                    moneyItem.Draw(spriteBatch, color);
                }
            }
        }
    }
}
