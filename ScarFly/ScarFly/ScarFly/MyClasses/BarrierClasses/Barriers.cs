using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.BarrierClasses
{
    public class Barriers
    {
        public Barriers(string levelName, int velocity, int phoneWidth, int phoneHeight)
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
        public List<Barrier> BarrierList { get; set; }
        public int Velocity { get; set; }
        public int PhoneHeight { get; set; }
        public int PhoneWidth { get; set; }

        public void Load(Game1 game) { foreach (var item in BarrierList) { item.Load(game); } }

        public void RePosition() { foreach (var item in BarrierList) { item.Position = item.StartPosition; } }

        private void ProcLevelFile()
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
                            case 1: BarrierList.Add(new Barrier("Barriers/PinWheelMini", new BarrierIndex(j, i, id), PhoneWidth, PhoneHeight, 20));
                                break;
                            case 2: BarrierList.Add(new Barrier("Barriers/PinWheelBig", new BarrierIndex(j, i, id), PhoneWidth, PhoneHeight, 20));
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
            foreach (var barrierItem in BarrierList.Where(p => p.Position.X >= -p.Texture.Width))
            {
                barrierItem.Position = new Vector2(barrierItem.Position.X - Velocity, barrierItem.Position.Y);
                barrierItem.UpdateRectangle();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var barrierItem in BarrierList.Where(p => p.Position.X >= -p.Texture.Width && p.Position.X <= PhoneWidth))
            {
                barrierItem.Draw(spriteBatch);
            }
        }
    }
}
