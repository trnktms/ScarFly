﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScarFly.MyClasses.LevelElementClasses;
using ScarFly.MyClasses.PlayerClasses;

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
        }

        public SpriteFont Font { get; set; }
        public Moneys Moneys { get; set; }
        public Barriers Barriers { get; set; }
        public Player Player { get; set; }
        public Modifiers Modifiers { get; set; }
        public List<PlayerBackground> Backgrounds { get; set; }

        public void Load(Game1 game) { Font = game.Content.Load<SpriteFont>(Consts.SF_ModifierNotification); }

        public bool CollosionDetectionWithBarrier()
        {
            bool result = false;
            foreach (var barrierItem in Barriers.BarrierList)
            {
                result = IntersectsPixel(barrierItem.Bound, barrierItem.ColorData, Player.RunBound, Player.RunColorData);
                if (result) { return true; }
            }
            return result;
        }

        public bool CollosionDetectionWithMoney()
        {
            bool result = false;
            int i = 0;
            foreach (var moneyItem in Moneys.MoneyList.Where(p => p.Index.ID != "!"))
            {
                result = IntersectsPixel(moneyItem.Bound, moneyItem.ColorData, Player.RunBound, Player.RunColorData);
                if (result) { break; }
                i++;
            }

            if (result) { Moneys.MoneyList.RemoveAt(i); }
            return result;
        }

        public bool CollosionDetectionWithModifier()
        {
            bool result = false;
            int i = 0;
            foreach (var modifierItem in Modifiers.ModifierList.Where(p => p.Index.ID != "!"))
            {
                result = IntersectsPixel(modifierItem.Bound, modifierItem.ColorData, Player.RunBound, Player.RunColorData);
                if (result) { break; }
                i++;
            }

            if (result) { Modifiers.ModifierList.RemoveAt(i); }
            return result;
        }

        public void Update()
        {
            Player.isDead = CollosionDetectionWithBarrier();
            Player.isEatMoney = CollosionDetectionWithMoney();
            Player.isEatModifier = CollosionDetectionWithModifier();
            ModifyGame();
        }

        public bool IntersectsPixel(Rectangle rectangle1, Color[] data1, Rectangle rectangle2, Color[] data2)
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
                    if (color1.A != 0 && color2.A != 0) { return true; }
                }
            }

            return false;
        }

        private int selectedModify = -1;
        private int modifyCounter = 0;
        private int modifyCounterMax = 400;
        private bool isModify { get; set; }
        public void ModifyGame()
        {
            if (selectedModify == -1)
            {
                Random R = new Random();
                selectedModify = R.Next(0, 3);
            }

            switch (selectedModify)
            {
                case 0: IncreaseVelocity();
                    break;
                case 1: DecreaseVelocity();
                    break;
                case 2: EatMore();
                    break;
                default:
                    break;
            }
        }

        public void IncreaseVelocity()
        {
            if (Player.isEatModifier)
            {
                isModify = true;
                foreach (var item in Backgrounds) { item.Velocity *= 2; }
                Player.Velocity *= 2;
                Barriers.Velocity *= 2;
                Moneys.Velocity *= 2;
                Modifiers.Velocity *= 2;
            }
            if (isModify)
            {
                modifyCounter++;
                if (modifyCounter >= modifyCounterMax || Player.isEnd)
                {
                    isModify = false;
                    modifyCounter = 0;
                    selectedModify = -1;
                    foreach (var item in Backgrounds) { item.Velocity /= 2; }
                    Player.Velocity /= 2;
                    Barriers.Velocity /= 2;
                    Moneys.Velocity /= 2;
                    Modifiers.Velocity /= 2;
                }
            }
        }

        public void DecreaseVelocity()
        {
            if (Player.isEatModifier)
            {
                isModify = true;
                foreach (var item in Backgrounds) { if (item.Velocity != 1) { item.Velocity /= 2; } }
                Player.Velocity /= 2;
                Barriers.Velocity /= 2;
                Moneys.Velocity /= 2;
                Modifiers.Velocity /= 2;
            }
            if (isModify)
            {
                modifyCounter++;
                if (modifyCounter >= modifyCounterMax || Player.isEnd)
                {
                    isModify = false;
                    modifyCounter = 0;
                    selectedModify = -1;
                    foreach (var item in Backgrounds) { if (item.Velocity != 1) { item.Velocity *= 2; } }
                    Player.Velocity *= 2;
                    Barriers.Velocity *= 2;
                    Moneys.Velocity *= 2;
                    Modifiers.Velocity *= 2;
                }
            }
        }

        public void EatMore()
        {
            if (Player.isEatModifier) { isModify = true; }
            if (isModify)
            {
                modifyCounter++;
                if (Player.isEatMoney) { Player.Score.GameScore += 10; }
                if (modifyCounter >= modifyCounterMax || Player.isEnd)
                {
                    isModify = false;
                    modifyCounter = 0;
                    selectedModify = -1;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isModify)
            {
                switch (selectedModify)
                {
                    case 0: spriteBatch.DrawString(Font, "Faster!", new Vector2(10, Consts.PhoneHeight - 40), Consts.PastelGreen);
                        break;
                    case 1: spriteBatch.DrawString(Font, "Slower!", new Vector2(10, Consts.PhoneHeight - 40), Consts.PastelRed);
                        break;
                    case 2: spriteBatch.DrawString(Font, "Eat more!", new Vector2(10, Consts.PhoneHeight - 40), Consts.PastelGreen);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
