using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.BarrierClasses
{
    public class Moneys
    {
        public Moneys(string levelName, int velocity, int phoneWidth, int phoneHeight)
        {
            this.LevelName = string.Format(@"Levels\{0}.level", levelName);
            this.Velocity = velocity;
            this.PhoneWidth = phoneWidth;
            this.PhoneHeight = phoneHeight;
            ProcLevelFile();

            _horizontalStep = this.PhoneWidth / Consts.PhoneWidthRate;
            _verticalStep = this.PhoneHeight / Consts.PhoneHeightRate;
        }

        public string LevelName { get; set; }
        public List<Money> MoneyList { get; set; }
        public int Velocity { get; set; }
        public int PhoneHeight { get; set; }
        public int PhoneWidth { get; set; }

        public void Load(Game1 game) { foreach (var item in MoneyList) { item.Load(game); } }

        public void RePosition() { foreach (var item in MoneyList) { item.Position = item.StartPosition; } }

        private void ProcLevelFile()
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
                            case "a": MoneyList.Add(new Money("Barriers/Money", new MoneyIndex(j, i, id), PhoneWidth, PhoneHeight));
                                break;
                            case "!": MoneyList.Add(new Money("Barriers/Barrier2", new MoneyIndex(j, i, id), PhoneWidth, PhoneHeight));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private int _horizontalStep;
        private int _verticalStep;
        public void Scroll(Game1 game)
        {
            foreach (var moneyItem in MoneyList)
            {
                if (moneyItem.Position.X >= -moneyItem.Texture.Width)
                {
                    moneyItem.Position = new Vector2(moneyItem.Position.X - Velocity, moneyItem.Position.Y);
                    moneyItem.UpdateRectangle();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var moneyItem in MoneyList)
            {
                if (moneyItem.Position.X >= -moneyItem.Texture.Width && moneyItem.Position.X <= PhoneWidth)
                {
                    spriteBatch.Draw(moneyItem.Texture, moneyItem.Position, Color.White);
                }
            }
        }
    }
}
