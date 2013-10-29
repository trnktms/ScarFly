using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.IsolatedStorage;
using System.IO;

namespace ScarFly.MyClasses.PlayerClasses
{
    public class Score
    {
        public Score(string gameScoreIconAssetName, string totalScoreIconAssetName, string gameScoreFontName, string totalGameScoreFontName)
        {
            this.GameScoreIconAssetName = gameScoreIconAssetName;
            this.TotalScoreIconAssetName = totalScoreIconAssetName;
            this.GameScoreFontAssetName = gameScoreFontName;
            this.TotalScoreFontAssetName = totalGameScoreFontName;
            LoadTotalScore();
        }

        public int GameScore { get; set; }
        public int TotalScore { get; set; }

        public SpriteFont GameScoreFont { get; set; }
        public SpriteFont TotalScoreFont { get; set; }

        public Texture2D GameScoreIcon { get; set; }
        public Texture2D TotalScoreIcon { get; set; }

        public string GameScoreIconAssetName { get; set; }
        public string TotalScoreIconAssetName { get; set; }

        public string GameScoreFontAssetName { get; set; }
        public string TotalScoreFontAssetName { get; set; }

        public void Load(Game1 game)
        {
            GameScoreFont = game.Content.Load<SpriteFont>(GameScoreFontAssetName);
            TotalScoreFont = game.Content.Load<SpriteFont>(TotalScoreFontAssetName);
            GameScoreIcon = game.Content.Load<Texture2D>(GameScoreIconAssetName);
            TotalScoreIcon = game.Content.Load<Texture2D>(TotalScoreIconAssetName);
        }

        public void DrawGameScore(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(GameScoreIcon, new Vector2(0, 0), color);
            spriteBatch.DrawString(GameScoreFont, GameScore.ToString(), new Vector2(GameScoreIcon.Width, 0), color);
        }

        public void DrawTotalScore(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(TotalScoreIcon, new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(TotalScoreFont, TotalScore.ToString(), new Vector2(TotalScoreIcon.Width, 0), color);
        }

        public void SaveTotalScore()
        {
            IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile("TotalScore.txt", FileMode.OpenOrCreate, FileAccess.Write);
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.Write(TotalScore);
            }
        }

        public void LoadTotalScore()
        {
            IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
            if (myIsolatedStorage.FileExists("TotalScore.txt"))
            {
                using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile("TotalScore.txt", FileMode.Open, FileAccess.ReadWrite))
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        TotalScore = int.Parse(reader.ReadLine());
                    }
                }
            }
        }
    }
}
