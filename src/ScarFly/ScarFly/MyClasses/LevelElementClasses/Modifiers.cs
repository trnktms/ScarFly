using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScarFly.MyClasses.Common;
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
            foreach (var modifierItem in ModifierList.Where(p => p.Position.X >= -p.Texture.Width && !p.IsHidden))
            {
                modifierItem.Position = new Vector2(modifierItem.Position.X - Velocity, modifierItem.Position.Y);
                modifierItem.UpdateRectangle();
            }
        }

        public List<Modifier> GetActualMoneyList()
        {
            return ModifierList.Where(p => p.Position.X >= -p.Texture.Width && p.Position.X <= Consts.PhoneWidth && !p.IsHidden).ToList();
        }

        public override void Draw(SpriteBatch spriteBatch, Color color)
        {
            foreach (var modifierItem in ModifierList.Where(p => p.Position.X >= -p.Texture.Width && p.Position.X <= Consts.PhoneWidth && !p.IsHidden))
            {
                if (modifierItem.IsCatched)
                {
                    modifierItem.DrawCatched(spriteBatch, color);
                }
                else
                {
                    modifierItem.Draw(spriteBatch, color);
                }
            }
        }
    }
}
