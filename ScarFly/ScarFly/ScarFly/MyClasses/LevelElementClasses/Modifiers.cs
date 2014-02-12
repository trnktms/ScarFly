using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.LevelElementClasses
{
    public class Modifiers : LevelElements
    {
        public Modifiers(string levelName, int velocity)
            : base(levelName, velocity) { }

        public List<Modifier> ModifierList { get; set; }

        public override void Load(Game1 game) { foreach (var item in ModifierList) { item.Load(game); } }

        public override void RePosition(Game1 game) { ProcLevelFile(); Load(game); }

        public override void ProcLevelFile()
        {
            ModifierList = new List<Modifier>();
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
                            case "*": ModifierList.Add(new Modifier("LevelElements/Modifier", new ModifierIndex(j, i, id), 1));
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
            foreach (var moneyItem in ModifierList.Where(p => p.Position.X >= -p.Texture.Width && !p.IsCatched))
            {
                moneyItem.Position = new Vector2(moneyItem.Position.X - Velocity, moneyItem.Position.Y);
                moneyItem.UpdateRectangle();
            }
        }

        public List<Modifier> GetActualMoneyList()
        {
            List<Modifier> result = new List<Modifier>();
            result = ModifierList.Where(p => p.Position.X >= -p.Texture.Width && p.Position.X <= Consts.PhoneWidth && !p.IsCatched).ToList();
            return result;
        }

        public override void Draw(SpriteBatch spriteBatch, Color color)
        {
            foreach (var moneyItem in ModifierList.Where(p => p.Position.X >= -p.Texture.Width && p.Position.X <= Consts.PhoneWidth && !p.IsCatched))
            {
                moneyItem.Draw(spriteBatch, color);
            }
        }
    }
}
