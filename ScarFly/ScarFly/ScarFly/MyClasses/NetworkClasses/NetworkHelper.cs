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
            if (OtherPlayer.Id == Guid.Empty)
            {
                OtherPlayer.Id = Guid.Parse(recievedDataArray[0]);
            }
            //NOTE: if received data source is correct
            else if (recievedDataArray[0] == OtherPlayer.Id.ToString())
            {
                if (recievedDataArray.Length == 4)
                {
                    OtherPlayer.Position = new Vector2(int.Parse(recievedDataArray[1]), int.Parse(recievedDataArray[2]));
                    OtherPlayer.Score = int.Parse(recievedDataArray[3]);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            OtherPlayer.Draw(spriteBatch, color);
        }
    }
}
