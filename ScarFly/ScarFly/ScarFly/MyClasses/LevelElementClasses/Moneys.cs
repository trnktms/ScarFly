using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public override void Load(Game1 game) { foreach (var item in MoneyList) { item.Load(game); } }

        public override void RePosition(Game1 game) { ProcLevelFile(); Load(game); }

        public override void ProcLevelFile()
        {
            MoneyList = new List<Money>();
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
                            case "a": MoneyList.Add(new Money("Barriers/Money", new MoneyIndex(j, i, id), 1));
                                break;
                            case "!": MoneyList.Add(new Money("Barriers/Money", new MoneyIndex(j, i, id), 1));
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
            foreach (var moneyItem in MoneyList.Where(p => p.Position.X >= -p.Texture.Width))
            {
                moneyItem.Position = new Vector2(moneyItem.Position.X - Velocity, moneyItem.Position.Y);
                moneyItem.UpdateRectangle();
            }
        }

        public List<Money> GetActualMoneyList()
        {
            return MoneyList.Where(p => p.Position.X >= -p.Texture.Width && p.Position.X <= Consts.PhoneWidth).ToList();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var moneyItem in MoneyList.Where(p => p.Position.X >= -p.Texture.Width && p.Position.X <= Consts.PhoneWidth))
            {
                moneyItem.Draw(spriteBatch);
            }
        }
    }
}
