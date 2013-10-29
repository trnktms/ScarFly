using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            
            BarrierList = new List<Barrier>();
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
                        switch (id)
                        {
                            case 1: BarrierList.Add(new Barrier("LevelElements/PinWheelMini", new BarrierIndex(j, i, id), 20));
                                break;
                            case 2: BarrierList.Add(new Barrier("LevelElements/PinWheelBig", new BarrierIndex(j, i, id), 20));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        public override void Load(Game1 game) { foreach (var item in BarrierList) { item.Load(game); } }

        public override void RePosition(Game1 game) { foreach (var item in BarrierList) { item.Position = item.StartPosition; } }

        public override void Scroll(Game1 game)
        {
            foreach (var barrierItem in BarrierList.Where(p => p.Position.X >= -p.Texture.Width))
            {
                barrierItem.Position = new Vector2(barrierItem.Position.X - Velocity, barrierItem.Position.Y);
                barrierItem.UpdateRectangle();
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Color color)
        {
            foreach (var barrierItem in BarrierList.Where(p => p.Position.X >= -p.Texture.Width && p.Position.X <= Consts.PhoneWidth))
            {
                barrierItem.Draw(spriteBatch, color);
            }
        } 
    }
}
