﻿using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using PBCaGw.Workers;
using PBCaGw.Services;

namespace PBCaGw
{
    /// <summary>
    /// Monitor a TCP port and creates a new worker chain for each incoming connection.
    /// </summary>
    public class GwTcpListener : IDisposable
    {
        readonly TcpListener tcpListener;
        bool disposed = false;
        readonly IPEndPoint ipSource;
        readonly ChainSide side = ChainSide.SIDE_A;
        readonly Gateway gateway;

        public GwTcpListener(Gateway gateway, ChainSide side, IPEndPoint ipSource)
        {
            this.gateway = gateway;
            this.ipSource = ipSource;
            this.side = side;
            tcpListener = new TcpListener(ipSource);
            tcpListener.Start(10);
            tcpListener.BeginAcceptSocket(ReceiveConn, tcpListener);
            if (Log.WillDisplay(System.Diagnostics.TraceEventType.Start))
                Log.TraceEvent(System.Diagnostics.TraceEventType.Start, -1, "TCP Listener " + side.ToString() + " on " + ipSource);
        }

        void ReceiveConn(IAsyncResult result)
        {
            DiagnosticServer.NbTcpCreated++;
            TcpListener listener = null;
            Socket client = null;

            try
            {
                listener = (TcpListener)result.AsyncState;
                client = listener.EndAcceptSocket(result);

                client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                //client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 0);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception ex)
            {
                if (Log.WillDisplay(System.Diagnostics.TraceEventType.Critical))
                    Log.TraceEvent(System.Diagnostics.TraceEventType.Critical, -1, "Error: " + ex.Message);
            }

            if (disposed)
                return;

            if (client == null)
                return;

            // Create the client chain and register the client in the Tcp Manager
            try
            {
                // Send version
                DataPacket packet = DataPacket.Create(16);
                packet.Sender = ipSource;
                packet.Destination = (IPEndPoint)client.RemoteEndPoint;
                packet.Command = 0;
                packet.DataType = 1;
                packet.DataCount = 11;
                packet.Parameter1 = 0;
                packet.Parameter2 = 0;
                packet.PayloadSize = 0;
                client.Send(packet.Data, packet.Offset, packet.BufferSize, SocketFlags.None);

                WorkerChain chain = WorkerChain.TcpChain(this.gateway, this.side, (IPEndPoint)client.RemoteEndPoint, ipSource);
                if (Log.WillDisplay(System.Diagnostics.TraceEventType.Start))
                    Log.TraceEvent(System.Diagnostics.TraceEventType.Start, chain.ChainId, "New client connection: " + client.RemoteEndPoint);
                TcpReceiver receiver = (TcpReceiver)chain[0];
                receiver.Socket = client;
                TcpManager.RegisterClient((IPEndPoint)client.RemoteEndPoint, chain);
            }
            catch (Exception ex)
            {
                if (Log.WillDisplay(System.Diagnostics.TraceEventType.Error))
                    Log.TraceEvent(System.Diagnostics.TraceEventType.Error, -1, "Error: " + ex.Message);
            }

            // Wait for the next one
            try
            {
                Debug.Assert(listener != null, "listener != null");
                listener.BeginAcceptSocket(new AsyncCallback(ReceiveConn), listener);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception ex)
            {
                if (Log.WillDisplay(System.Diagnostics.TraceEventType.Critical))
                    Log.TraceEvent(System.Diagnostics.TraceEventType.Critical, -1, "Error: " + ex.Message);
            }
        }

        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            tcpListener.Server.Close();
        }
    }
}
