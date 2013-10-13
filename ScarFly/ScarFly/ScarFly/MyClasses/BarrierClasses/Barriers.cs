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

            _horizontalStep = this.PhoneWidth / 4;
            _verticalStep = this.PhoneHeight / 5;
        }

        public string LevelName { get; set; }
        public List<Barrier> BarrierList { get; set; }
        public List<BarrierTexture> BarrierTextures { get; set; }

        public int Velocity { get; set; }

        public int PhoneHeight { get; set; }
        public int PhoneWidth { get; set; }

        public void Load(Game1 game) 
        {
            BarrierTextures = BarrierList
                .GroupBy(p => new { ID = p.Index.ID, AssetName = p.AssetName })
                .Select(p => new BarrierTexture((int)p.Key.ID, p.Key.AssetName.ToString()))
                .ToList();

            foreach (var item in BarrierTextures) { item.Load(game); }
        }

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
                            case 1: BarrierList.Add(new Barrier("Barriers/fire", new BarrierIndex(j, i, id), PhoneWidth, PhoneHeight));
                                break;
                            case 2: BarrierList.Add(new Barrier("Barriers/fire", new BarrierIndex(j, i, id), PhoneWidth, PhoneHeight));
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
            foreach (var textureItem in BarrierTextures)
            {
                foreach (var barrierItem in BarrierList)
                {
                    if (barrierItem.Position.X >= -textureItem.Texture.Width)
                    {
                        barrierItem.Position = new Vector2(barrierItem.Position.X - Velocity, barrierItem.Position.Y);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var textureItem in BarrierTextures)
            {
                foreach (var barrierItem in BarrierList)
                {
                    if (barrierItem.Position.X >= -textureItem.Texture.Width && barrierItem.Position.X <= PhoneWidth)
                    {
                        spriteBatch.Draw(textureItem.Texture, barrierItem.Position, Color.White);
                    }
                }
            }
        }

        public void RePosition()
        {
            foreach (var item in BarrierList)
            {
                item.Position = item.StartPosition;
            }
        }
    }
}
