﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PSI.EpicsClient2
{
    /// <summary>
    /// extended epics type <br/> serves severity, status and value
    /// </summary>
    /// <typeparam name="TType">generic datatype for value</typeparam>
    public class ExtType<TType> : Decodable
    {
        internal ExtType()
        {
        }

        /// <summary>
        /// Severity of the channel serving this value
        /// </summary>
        public Severity Severity { get; internal set; }
        /// <summary>
        /// Status of the channel serving this value
        /// </summary>
        public Status Status { get; internal set; }
        /// <summary>
        /// current value, type transformation made by epics not c#
        /// </summary>
        public TType Value { get; set; }

        internal override void Decode(EpicsChannel channel, uint nbElements)
        {
            Status = (Status)channel.DecodeData<ushort>(1, 0);
            Severity = (Severity)channel.DecodeData<ushort>(1, 2);
            Value = channel.DecodeData<TType>(nbElements, 4);
        }

        /// <summary>
        /// builds a string line of all properties
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Value:{0},Status:{1},Severity:{2}", Value, Status, Severity);
        }
    }
}
