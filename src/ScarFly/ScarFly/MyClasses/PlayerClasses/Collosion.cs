using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScarFly.MyClasses.LevelElementClasses;
using ScarFly.MyClasses.PlayerClasses;
using ScarFly.MyClasses.Common;

namespace ScarFly.MyClasses.PlayerClasses
{
    public class Collosion
    {
        public Collosion(Barriers barriers, Player player, Moneys moneys, Modifiers modifiers, List<PlayerBackground> backgrounds)
        {
            this.Barriers = barriers;
            this.Player = player;
            this.Moneys = moneys;
            this.Modifiers = modifiers;
            this.Backgrounds = backgrounds;
            this.R = new Random();
        }

        public SpriteFont Font { get; set; }
        public Moneys Moneys { get; set; }
        public Barriers Barriers { get; set; }
        public Player Player { get; set; }
        public Modifiers Modifiers { get; set; }
        public List<PlayerBackground> Backgrounds { get; set; }

        private int selectedModify = -1;
        private int modifyCounter = 0;
        private int modifyCounterMax = 400;
        private bool isModify { get; set; }
        private Random R;

        public void Load(Game1 game) { Font = game.Content.Load<SpriteFont>(Consts.SF_ModifierNotification); }

        private bool CollosionDetectionWithBarrier()
        {
            bool result = false;
            foreach (var barrierItem in this.Barriers.BarrierList)
            {
                result = this.IntersectsPixel(barrierItem.Bound, barrierItem.ColorData, this.Player.RunBound, this.Player.ColorData);
                if (result)
                {
                    return true;
                }
            }
            return result;
        }

        private bool CollosionDetectionWithMoney()
        {
            foreach (var moneyItem in this.Moneys.MoneyList.Where(p => p.Index.ID != "!" && !p.IsCatched))
            {
                if (this.IntersectsPixel(moneyItem.Bound, moneyItem.ColorData, this.Player.RunBound, this.Player.ColorData))
                {
                    moneyItem.IsCatched = true;
                    return true;
                }
            }
            return false;
        }

        private bool CollosionDetectionWithModifier()
        {
            foreach (var modifierItem in this.Modifiers.ModifierList.Where(p => p.Index.ID != "!" && !p.IsCatched))
            {
                if (this.IntersectsPixel(modifierItem.Bound, modifierItem.ColorData, Player.RunBound, Player.ColorData))
                {
                    modifierItem.IsCatched = true;
                    return true;
                }
            }
            return false;
        }

        public void Update()
        {
            this.Player.isDead = this.CollosionDetectionWithBarrier();
            this.Player.isEatMoney = this.CollosionDetectionWithMoney();
            this.Player.isEatModifier = this.CollosionDetectionWithModifier();
            this.ModifyGame();
        }

        private bool IntersectsPixel(Rectangle rectangle1, Color[] data1, Rectangle rectangle2, Color[] data2)
        {
            int top = Math.Max(rectangle1.Top, rectangle2.Top);
            int bottom = Math.Min(rectangle1.Bottom, rectangle2.Bottom);
            int left = Math.Max(rectangle1.Left, rectangle2.Left);
            int right = Math.Min(rectangle1.Right, rectangle2.Right);

            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    Color color1 = data1[(x - rectangle1.Left) + (y - rectangle1.Top) * rectangle1.Width];
                    Color color2 = data2[(x - rectangle2.Left) + (y - rectangle2.Top) * rectangle2.Width];
                    if (color1.A != 0 && color2.A != 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void ModifyGame()
        {
            if (selectedModify == -1)
            {
                selectedModify = R.Next(0, 3);
            }

            switch (selectedModify)
            {
                case 0: this.IncreaseVelocity();
                    break;
                case 1: this.DecreaseVelocity();
                    break;
                case 2: this.EatMore();
                    break;
                default:
                    break;
            }
        }

        private void IncreaseVelocity()
        {
            if (this.Player.isEatModifier)
            {
                this.isModify = true;
                foreach (var item in Backgrounds) { item.Velocity *= 2; }
                this.Player.Velocity *= 2;
                this.Barriers.Velocity *= 2;
                this.Moneys.Velocity *= 2;
                this.Modifiers.Velocity *= 2;
            }
            if (this.isModify)
            {
                this.modifyCounter++;
                if (modifyCounter >= modifyCounterMax || Player.isEnd)
                {
                    this.isModify = false;
                    this.modifyCounter = 0;
                    this.selectedModify = -1;
                    foreach (var item in this.Backgrounds) { item.Velocity /= 2; }
                    this.Player.Velocity /= 2;
                    this.Barriers.Velocity /= 2;
                    this.Moneys.Velocity /= 2;
                    this.Modifiers.Velocity /= 2;
                }
            }
        }

        private void DecreaseVelocity()
        {
            if (this.Player.isEatModifier)
            {
                this.isModify = true;
                foreach (var item in this.Backgrounds) { if (item.Velocity != 1) { item.Velocity /= 2; } }
                this.Player.Velocity /= 2;
                this.Barriers.Velocity /= 2;
                this.Moneys.Velocity /= 2;
                this.Modifiers.Velocity /= 2;
            }
            if (this.isModify)
            {
                this.modifyCounter++;
                if (this.modifyCounter >= this.modifyCounterMax || this.Player.isEnd)
                {
                    this.isModify = false;
                    this.modifyCounter = 0;
                    this.selectedModify = -1;
                    foreach (var item in this.Backgrounds) { if (item.StartVelocity != 1) { item.Velocity *= 2; } }
                    this.Player.Velocity *= 2;
                    this.Barriers.Velocity *= 2;
                    this.Moneys.Velocity *= 2;
                    this.Modifiers.Velocity *= 2;
                }
            }
        }

        private void EatMore()
        {
            if (this.Player.isEatModifier)
            {
                this.isModify = true;
            }

            if (this.isModify)
            {
                this.modifyCounter++;
                if (this.Player.isEatMoney)
                {
                    this.Player.Score.GameScore += 10;
                }

                if (this.modifyCounter >= this.modifyCounterMax || this.Player.isEnd)
                {
                    this.isModify = false;
                    this.modifyCounter = 0;
                    this.selectedModify = -1;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.isModify)
            {
                switch (this.selectedModify)
                {
                    case 0: spriteBatch.DrawString(this.Font, "Faster!", new Vector2(10, Consts.PhoneHeight - 40), Color.Gray);
                        break;
                    case 1: spriteBatch.DrawString(this.Font, "Slower!", new Vector2(10, Consts.PhoneHeight - 40), Color.Gray);
                        break;
                    case 2: spriteBatch.DrawString(this.Font, "Eat more!", new Vector2(10, Consts.PhoneHeight - 40), Color.Gray);
                        break;
                    default:
                        break;
                }
            }
        }

        public void Reset()
        {
            this.selectedModify = -1;
            this.modifyCounter = 0;
            this.isModify = false;
        }
    }
}
