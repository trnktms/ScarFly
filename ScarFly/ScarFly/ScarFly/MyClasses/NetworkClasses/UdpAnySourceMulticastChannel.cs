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
        private static IPAddress GROUP_ADDRESS = IPAddress.Parse("224.109.108.107");
        private static int GROUP_PORT = 3007;

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

        public event EventHandler<UdpPacketReceivedEventArgs> GroupPacketReceived;
        public bool GroupIsDisposed { get; private set; }
        public static bool GroupIsJoined;
        private UdpAnySourceMulticastClient GroupClient { get; set; }

        public event EventHandler<UdpPacketReceivedEventArgs> SinglePacketReceived;
        public UdpSingleSourceMulticastClient SingleClient { get; set; }
        public IPEndPoint SingleSourceEndPoint { get; set; }
        public static bool SingleIsJoined;
        public bool SingleIsDisposed { get; private set; }

        public void GroupDispose()
        {
            if (!GroupIsDisposed)
            {
                this.GroupIsDisposed = true;

                if (this.GroupClient != null)
                    this.GroupClient.Dispose();
            }
        }

        public void SingleDispose()
        {
            if (!SingleIsDisposed)
            {
                this.SingleIsDisposed = true;

                if (this.SingleClient != null)
                    this.SingleClient.Dispose();
            }
        }

        public void GroupOpen()
        {
            if (!GroupIsJoined)
            {
                this.GroupClient.BeginJoinGroup(
                    result =>
                    {
                        this.GroupClient.EndJoinGroup(result);
                        GroupIsJoined = true;
                        this.GroupClient.MulticastLoopback = false;
                        this.GroupReceive();
                    }, null);
            }
        }

        public void SingleOpen()
        {
            if (!SingleIsJoined)
            {
                SingleClient = new UdpSingleSourceMulticastClient(SingleSourceEndPoint.Address, GROUP_ADDRESS, GROUP_PORT);
                this.SingleClient.BeginJoinGroup(
                    result =>
                    {
                        this.SingleClient.EndJoinGroup(result);
                        SingleIsJoined = true;
                        this.SingleClientReceive();
                    }, null);
            }
        }

        public void GroupClose()
        {
            GroupIsJoined = false;
            this.GroupDispose();
        }

        public void SingleClose()
        {
            SingleIsJoined = false;
            this.SingleDispose();
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

        public void SingleSendToSource(string format, params object[] args)
        {
            if (SingleIsJoined)
            {
                byte[] data = Encoding.UTF8.GetBytes(string.Format(format, args));
                this.SingleClient.BeginSendToSource(data, 0, data.Length, SingleSourceEndPoint.Port,
                    result =>
                    {
                        this.SingleClient.EndSendToSource(result);
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

        private void SingleClientReceive()
        {
            if (SingleIsJoined)
            {
                Array.Clear(this.ReceiveBuffer, 0, this.ReceiveBuffer.Length);
                this.SingleClient.BeginReceiveFromSource(this.ReceiveBuffer, 0, this.ReceiveBuffer.Length,
                    result =>
                    {
                        if (!SingleIsDisposed)
                        {
                            int source;
                            try
                            {
                                this.SingleClient.EndReceiveFromSource(result, out source);
                                this.SingleOnReceive(source, this.ReceiveBuffer);
                                this.SingleClientReceive();
                            }
                            catch
                            {
                                SingleIsJoined = false;
                                this.SingleOpen();
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

        private void SingleOnReceive(int source, byte[] data)
        {
            EventHandler<UdpPacketReceivedEventArgs> handler = this.SinglePacketReceived;
            if (handler != null)
                handler(this, new UdpPacketReceivedEventArgs(data, SingleSourceEndPoint));
        }
    }
}