﻿using System.Diagnostics;
using PBCaGw.Services;

namespace PBCaGw.Handlers
{
    /// <summary>
    /// 11 (0x0C) CA_PROTO_ERROR
    /// </summary>
    class ProtoError : CommandHandler
    {
        public override void DoRequest(DataPacket packet, Workers.WorkerChain chain, DataPacketDelegate sendData)
        {
        }

        public override void DoResponse(DataPacket packet, Workers.WorkerChain chain, DataPacketDelegate sendData)
        {
            Record record = InfoService.ChannelCid[packet.Parameter1];
            if (Log.WillDisplay(TraceEventType.Critical))
            {
                if (record != null)
                    Log.TraceEvent(TraceEventType.Critical, chain.ChainId, "Proto Error (" + packet.Parameter2 + ") on CID: " + packet.Parameter1 + " (" + record.Channel + "), SID = " + record.SID);
                else
                    Log.TraceEvent(TraceEventType.Critical, chain.ChainId, "Proto Error (" + packet.Parameter2 + ") on CID: " + packet.Parameter1);
            }
        }
    }
}
