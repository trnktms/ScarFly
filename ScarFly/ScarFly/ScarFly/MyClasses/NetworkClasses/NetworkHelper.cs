using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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
            if (Channel.SingleIsJoined)
            {
                SendedData = "Connected!";
                Channel.SingleSendToSource(SendedData);
            }
            else if (Channel.GroupIsJoined)
            {
                if (RecievedData == "I am here!")
                {
                    SendedData = "I found you!";
                    Channel.GroupSend(SendedData);
                    Channel.GroupClose();
                    Channel.SingleOpen();
                    SendedData = "Connected!";
                    Channel.SingleSendToSource(SendedData);
                }
                else if (RecievedData == "I found you!")
                {
                    SendedData = "I found you!";
                    Channel.GroupSend(SendedData);
                    Channel.GroupClose();
                    Channel.SingleOpen();
                    SendedData = "Connected!";
                    Channel.SingleSendToSource(SendedData);
                }
                else
                {
                    SendedData = "I am here!";
                    Channel.GroupSend(SendedData);
                }
            }
        }

        int i = 0;
        void Channel_SinglePacketReceived(object sender, UdpPacketReceivedEventArgs e)
        {
            RecievedData = e.Message.Trim('\0');
            RecievedData += " " + i++;
            //if (!Channel.SingleIsJoining && Channel.SingleIsJoined)
            //{
            //    string[] recievedArray = RecievedData.Split(',');
            //    OtherPlayer.Position = new Vector2(float.Parse(recievedArray[0]), float.Parse(recievedArray[1]));
            //    OtherPlayer.Score = int.Parse(recievedArray[3]);
            //    Channel.SingleSourceEndPoint = e.Source;
            //}
        }

        void Channel_GroupPacketReceived(object sender, UdpPacketReceivedEventArgs e)
        {
            RecievedData = e.Message.Trim('\0');
            Channel.SingleSourceEndPoint = e.Source;
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            OtherPlayer.Draw(spriteBatch, color);
        }
    }
}
