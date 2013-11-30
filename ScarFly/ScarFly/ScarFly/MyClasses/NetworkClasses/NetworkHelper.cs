using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.NetworkClasses
{
    public class NetworkHelper
    {
        public NetworkHelper()
        {
            Channel = new UdpAnySourceMulticastChannel();
            OtherPlayer = new OtherPlayer("Player/PaperPlane_v2");
            RecievedData = "";
            SendedData = "";
        }

        public UdpAnySourceMulticastChannel Channel { get; set; }
        public string RecievedData { get; set; }
        public string SendedData { get; set; }
        public OtherPlayer OtherPlayer { get; set; }

        public void InitializeSockets()
        {
            Channel = new UdpAnySourceMulticastChannel();
            Channel.GroupPacketReceived += new EventHandler<UdpPacketReceivedEventArgs>(Channel_GroupPacketReceived);
            Channel.SinglePacketReceived += new EventHandler<UdpPacketReceivedEventArgs>(Channel_SinglePacketReceived);
            Channel.GroupOpen();
        }

        public void SendData()
        {
            if (Channel.SingleIsJoined && !Channel.SingleIsDisposed)
            {
                Channel.SingleSendToSource(SendedData);
            }
            else if (Channel.GroupIsJoined && !Channel.GroupIsDisposed)
            {
                Channel.GroupSend(SendedData);
            }
        }

        void Channel_SinglePacketReceived(object sender, UdpPacketReceivedEventArgs e)
        {
            RecievedData = e.Message.Trim('\0');
            if (RecievedData != "0")
            {
                string[] recievedArray = RecievedData.Split(',');
                OtherPlayer.Position = new Vector2(float.Parse(recievedArray[0]), float.Parse(recievedArray[1]));
                OtherPlayer.Score = int.Parse(recievedArray[3]);
                Channel.SingleSourceEndPoint = e.Source;
            }
        }

        void Channel_GroupPacketReceived(object sender, UdpPacketReceivedEventArgs e)
        {
            RecievedData = e.Message.Trim('\0');
            Channel.SingleSourceEndPoint = e.Source;
            if (Channel.SingleSourceEndPoint != null)
            {
                Channel.GroupClose();
                Channel.SingleOpen();
            }
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            OtherPlayer.Draw(spriteBatch, color);
        }
    }
}
