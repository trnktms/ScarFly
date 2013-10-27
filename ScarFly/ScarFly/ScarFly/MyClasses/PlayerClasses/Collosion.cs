using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScarFly.MyClasses.BarrierClasses;
using ScarFly.MyClasses.PlayerClasses;

namespace ScarFly.MyClasses.PlayerClasses
{
    public class Collosion
    {
        public Collosion(Barriers barriers, Player player, Moneys moneys)
        {
            this.Barriers = barriers;
            this.Player = player;
            this.Moneys = moneys;
        }

        public Moneys Moneys { get; set; }
        public Barriers Barriers { get; set; }
        public Player Player { get; set; }

        public bool CollosionDetectionWithBarrier()
        {
            bool result = false;
            foreach (var barrierItem in Barriers.BarrierList)
            {
                switch (Player.PlayerState)
                {
                    case PlayerStates.Running:
                        result = IntersectsPixel(barrierItem.Bound, barrierItem.ColorData, Player.RunBound, Player.RunColorData);
                        break;
                    case PlayerStates.Flying:
                        result = IntersectsPixel(barrierItem.Bound, barrierItem.ColorData, Player.FlyBound, Player.FlyColorData);
                        break;
                    case PlayerStates.Falling:
                        result = IntersectsPixel(barrierItem.Bound, barrierItem.ColorData, Player.FallBound, Player.FallColorData);
                        break;
                    default:
                        break;
                }

                if (result) { return true; }
            }
            return result;
        }

        public bool CollosionDetectionWithMoney()
        {
            bool result = false;
            int i = 0;
            foreach (var moneyItem in Moneys.MoneyList)
            {
                switch (Player.PlayerState)
                {
                    case PlayerStates.Running:
                        result = IntersectsPixel(moneyItem.Bound, moneyItem.ColorData, Player.RunBound, Player.RunColorData);
                        break;
                    case PlayerStates.Flying:
                        result = IntersectsPixel(moneyItem.Bound, moneyItem.ColorData, Player.FlyBound, Player.FlyColorData);
                        break;
                    case PlayerStates.Falling:
                        result = IntersectsPixel(moneyItem.Bound, moneyItem.ColorData, Player.FallBound, Player.FallColorData);
                        break;
                    default:
                        break;
                }

                if (result) { 
                    break; }
                i++;
            }

            if (result) { Moneys.MoneyList.RemoveAt(i); }
            return result;
        }

        public void Update()
        {
            Player.isDead = CollosionDetectionWithBarrier();
            Player.isEatMoney = CollosionDetectionWithMoney();
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
    }
}
