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
            Font = game.Content.Load<SpriteFont>(FontAssetName);
            GameScoreIcon = game.Content.Load<Texture2D>(GameScoreIconAssetName);
            TotalScoreIcon = game.Content.Load<Texture2D>(TotalScoreIconAssetName);
            HighScoreIcon = game.Content.Load<Texture2D>(HighScoreIconAssetName);
            RankIcon = game.Content.Load<Texture2D>(RankIconAssetName);
        }

        public void DrawGameScore(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(GameScoreIcon, new Vector2(5, 5), color);
            spriteBatch.DrawString(Font, GameScore.ToString(), new Vector2(GameScoreIcon.Width + 8, 8), color);
        }

        public void DrawMainMenuScores(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(RankIcon, new Vector2(5, 5), color);
            spriteBatch.DrawString(Font, Rank.ToString(), new Vector2(RankIcon.Width + 8, 8), color);
            spriteBatch.Draw(HighScoreIcon, new Vector2(5, 85), color * 0.8f);
            spriteBatch.DrawString(Font, HighScore.ToString() + (IsNewHighScore ? " - Hit!" : ""), new Vector2(HighScoreIcon.Width + 8, 88), color);
            spriteBatch.Draw(GameScoreIcon, new Vector2(5, 165), color);
            spriteBatch.DrawString(Font, TotalScore.ToString(), new Vector2(GameScoreIcon.Width + 8, 168), color);
            spriteBatch.Draw(GameScoreIcon, new Vector2(5, 245), color);
            spriteBatch.DrawString(Font, GameScore.ToString(), new Vector2(GameScoreIcon.Width + 8, 248), color);
        }

        public void DrawEndGameScores(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(GameScoreIcon, new Vector2(5, 5), color);
            spriteBatch.DrawString(Font, GameScore.ToString(), new Vector2(GameScoreIcon.Width + 8, 8), color);
        }

        public void SaveTotalScore()
        {
            LoadTotalScore();
            TotalScore += GameScore;
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
                            TotalScore = int.Parse(temp);
                        }
                    }
                }
            }

            if (TotalScore > 50000) { Rank = PlayerRank.Impendent; }
            else if (TotalScore > 25000) { Rank = PlayerRank.Hardcore; }
            else if (TotalScore > 15000) { Rank = PlayerRank.Professional; }
            else if (TotalScore > 7500) { Rank = PlayerRank.Amateur; }
            else { Rank = PlayerRank.Beginner; }
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
                            HighScore = int.Parse(temp);
                        }
                    }
                }
            }
        }

        public void SaveHighScore()
        {
            LoadHighScore();
            if (GameScore > HighScore)
            {
                IsNewHighScore = true;
                HighScore = GameScore;
                IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
                IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile("HighScore.txt", FileMode.OpenOrCreate, FileAccess.Write);
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    writer.Write(HighScore);
                    writer.Close();
                }
            }
            else
            {
                IsNewHighScore = false;
            }
        }
    }
}
