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
    public enum PlayerRank
    {
        Beginner,
        Amateur,
        Professional,
        Hardcore,
        Impendent
    }
    public class Score
    {
        public Score(string gameScoreIconAssetName, string totalScoreIconAssetName, string rankIconAssetName, string highScoreIconAssetName, string fontName)
        {
            this.GameScoreIconAssetName = gameScoreIconAssetName;
            this.TotalScoreIconAssetName = totalScoreIconAssetName;
            this.HighScoreIconAssetName = highScoreIconAssetName;
            this.RankIconAssetName = rankIconAssetName;
            this.FontAssetName = fontName;
        }
        public PlayerRank Rank { get; set; }
        public int GameScore { get; set; }
        public int TotalScore { get; set; }
        public int HighScore { get; set; }
        public bool IsNewHighScore { get; set; }

        public SpriteFont Font { get; set; }

        public Texture2D GameScoreIcon { get; set; }
        public Texture2D TotalScoreIcon { get; set; }
        public Texture2D HighScoreIcon { get; set; }
        public Texture2D RankIcon { get; set; }

        public string GameScoreIconAssetName { get; set; }
        public string TotalScoreIconAssetName { get; set; }
        public string HighScoreIconAssetName { get; set; }
        public string RankIconAssetName { get; set; }

        public string FontAssetName { get; set; }

        public void Load(Game1 game)
        {
            this.Font = game.Content.Load<SpriteFont>(FontAssetName);
            this.GameScoreIcon = game.Content.Load<Texture2D>(GameScoreIconAssetName);
            this.TotalScoreIcon = game.Content.Load<Texture2D>(TotalScoreIconAssetName);
            this.HighScoreIcon = game.Content.Load<Texture2D>(HighScoreIconAssetName);
            this.RankIcon = game.Content.Load<Texture2D>(RankIconAssetName);
        }

        public void DrawGameScore(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(this.GameScoreIcon, new Vector2(5, 5), color);
            spriteBatch.DrawString(this.Font, this.GameScore.ToString(), new Vector2(this.GameScoreIcon.Width + 8, 8), color);
        }

        public void DrawMainMenuScores(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(this.RankIcon, new Vector2(5, 5), color);
            spriteBatch.DrawString(this.Font, this.Rank.ToString(), new Vector2(this.RankIcon.Width + 8, 8), color);
            spriteBatch.Draw(this.HighScoreIcon, new Vector2(5, 85), color * 0.8f);
            spriteBatch.DrawString(this.Font, this.HighScore.ToString() + (this.IsNewHighScore ? " - Hit!" : ""), new Vector2(this.HighScoreIcon.Width + 8, 88), color);
            spriteBatch.Draw(this.GameScoreIcon, new Vector2(5, 165), color);
            spriteBatch.DrawString(this.Font, this.TotalScore.ToString(), new Vector2(this.GameScoreIcon.Width + 8, 168), color);
            spriteBatch.Draw(this.GameScoreIcon, new Vector2(5, 245), color);
            spriteBatch.DrawString(this.Font, this.GameScore.ToString(), new Vector2(this.GameScoreIcon.Width + 8, 248), color);
        }

        public void DrawEndGameScores(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(this.GameScoreIcon, new Vector2(5, 5), color);
            spriteBatch.DrawString(this.Font, this.GameScore.ToString(), new Vector2(this.GameScoreIcon.Width + 8, 8), color);
        }

        public void SaveTotalScore()
        {
            this.LoadTotalScore();
            this.TotalScore += this.GameScore;
            IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile("TotalScore.txt", FileMode.OpenOrCreate, FileAccess.Write);
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.Write(TotalScore);
                writer.Close();
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
                        string temp = reader.ReadToEnd();
                        if (temp != null && temp != "")
                        {
                            this.TotalScore = int.Parse(temp);
                        }
                    }
                }
            }

            if (this.TotalScore > 50000)
            {
                this.Rank = PlayerRank.Impendent;
            }
            else if (this.TotalScore > 25000)
            {
                this.Rank = PlayerRank.Hardcore;
            }
            else if (this.TotalScore > 15000)
            {
                this.Rank = PlayerRank.Professional;
            }
            else if (this.TotalScore > 7500)
            {
                this.Rank = PlayerRank.Amateur;
            }
            else
            {
                this.Rank = PlayerRank.Beginner;
            }
        }

        public void LoadHighScore()
        {
            IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
            if (myIsolatedStorage.FileExists("HighScore.txt"))
            {
                using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile("HighScore.txt", FileMode.Open, FileAccess.ReadWrite))
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        string temp = reader.ReadToEnd();
                        if (temp != null && temp != "")
                        {
                            this.HighScore = int.Parse(temp);
                        }
                    }
                }
            }
        }

        public void SaveHighScore()
        {
            this.LoadHighScore();
            if (this.GameScore > this.HighScore)
            {
                this.IsNewHighScore = true;
                this.HighScore = GameScore;
                IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
                IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile("HighScore.txt", FileMode.OpenOrCreate, FileAccess.Write);
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    writer.Write(this.HighScore);
                    writer.Close();
                }
            }
            else
            {
                this.IsNewHighScore = false;
            }
        }
    }
}
