using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace ScarFly.MyClasses.NetworkClasses
{
    public class UdpAnySourceMulticastChannel
    {
        private static readonly IPAddress GROUP_ADDRESS = IPAddress.Parse("224.109.108.107");
        private static readonly int GROUP_PORT = 3007;

        public UdpAnySourceMulticastChannel()
        {
            this.GroupClient = new UdpAnySourceMulticastClient(GROUP_ADDRESS, GROUP_PORT);
            this.ReceiveBuffer = new byte[1024];
        }

        public UdpAnySourceMulticastChannel(int maxMessageSize)
        {
            this.ReceiveBuffer = new byte[maxMessageSize];
            this.GroupClient = new UdpAnySourceMulticastClient(GROUP_ADDRESS, GROUP_PORT);
        }

        private byte[] ReceiveBuffer { get; set; }
        private UdpAnySourceMulticastClient GroupClient { get; set; }

        public event EventHandler<UdpPacketReceivedEventArgs> GroupPacketReceived;
        public bool GroupIsDisposed { get; private set; }
        public bool GroupIsJoined { get; private set; }

        public void GroupDispose()
        {
            if (!GroupIsDisposed)
            {
                this.GroupIsDisposed = true;

                if (this.GroupClient != null)
                    this.GroupClient.Dispose();
            }
        }

        public void GroupOpen()
        {
            if (!GroupIsJoined)
            {
                this.GroupClient.BeginJoinGroup(
                    result =>
                    {
                        try
                        {
                            this.GroupClient.EndJoinGroup(result);
                            GroupIsJoined = true;
                            this.GroupClient.MulticastLoopback = false;
                            this.GroupReceive();
                        }
                        catch { }
                    }, null);
            }
        }

        public void GroupClose()
        {
            GroupIsJoined = false;
            this.GroupDispose();
        }

        public void GroupSend(string format, params object[] args)
        {
            if (GroupIsJoined)
            {
                byte[] data = Encoding.UTF8.GetBytes(string.Format(format, args));
                this.GroupClient.BeginSendToGroup(data, 0, data.Length,
                    result =>
                    {
                        this.GroupClient.EndSendToGroup(result);
                    }, null);
            }
        }

        private void GroupReceive()
        {
            if (GroupIsJoined)
            {
                Array.Clear(this.ReceiveBuffer, 0, this.ReceiveBuffer.Length);
                this.GroupClient.BeginReceiveFromGroup(this.ReceiveBuffer, 0, this.ReceiveBuffer.Length,
                    result =>
                    {
                        if (!GroupIsDisposed)
                        {
                            IPEndPoint source;
                            try
                            {
                                this.GroupClient.EndReceiveFromGroup(result, out source);
                                this.GroupOnReceive(source, this.ReceiveBuffer);
                                this.GroupReceive();
                            }
                            catch
                            {
                                GroupIsJoined = false;
                                this.GroupOpen();
                            }
                        }
                    }, null);
            }
        }

        private void GroupOnReceive(IPEndPoint source, byte[] data)
        {
            EventHandler<UdpPacketReceivedEventArgs> handler = this.GroupPacketReceived;
            if (handler != null)
                handler(this, new UdpPacketReceivedEventArgs(data, source));
        }
    }
}