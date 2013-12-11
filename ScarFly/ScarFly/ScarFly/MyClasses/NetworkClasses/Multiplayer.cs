using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ScarFly.MyClasses.PlayerClasses;

namespace ScarFly.MyClasses.NetworkClasses
{
    public class Multiplayer
    {
        public Multiplayer(Player player)
        {
            OtherPlayer = new OtherPlayer(Consts.P_Player);
            Player = player;
            RecievedData = "";
            SendedData = "";
        }

        public UdpAnySourceMulticastChannel Channel { get; set; }
        public string RecievedData { get; set; }
        public string SendedData { get; set; }
        public Player Player { get; set; }
        public OtherPlayer OtherPlayer { get; set; }
        public string Level { get; set; }

        public void InitializeSockets()
        {
            Channel = new UdpAnySourceMulticastChannel();
            Channel.GroupPacketReceived += new EventHandler<UdpPacketReceivedEventArgs>(Channel_GroupPacketReceived);
            Channel.GroupOpen();
        }

        public void SendData()
        {
            Channel.GroupSend(SendedData);
        }

        void Channel_GroupPacketReceived(object sender, UdpPacketReceivedEventArgs e)
        {
            RecievedData = e.Message.Trim('\0');
            string[] recievedDataArray = RecievedData.Split(',');
            if (OtherPlayer.Id == Guid.Empty && recievedDataArray[1] == "0")
            {
                OtherPlayer.Id = Guid.Parse(recievedDataArray[0]);
                if (Player.Id.CompareTo(OtherPlayer.Id) == 1)
                {
                    Level = recievedDataArray[2];
                }
                else
                {
                    Level = SendedData.Split(',')[2];
                }
            }
            //NOTE: if received data source is correct
            else if (recievedDataArray[0] == OtherPlayer.Id.ToString() && recievedDataArray[1] == "1")
            {
                if (recievedDataArray.Length == 4)
                {
                    OtherPlayer.Distance = int.Parse(recievedDataArray[2]);
                    OtherPlayer.Score = int.Parse(recievedDataArray[3]);
                }
            }
        }

        public void Update()
        {
            SendedData = string.Format("{0},{1},{2},{3}", Player.Id, "1", Player.Distance, Player.Score.GameScore);
            SendData();
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            OtherPlayer.Draw(spriteBatch, color);
        }
    }
}
