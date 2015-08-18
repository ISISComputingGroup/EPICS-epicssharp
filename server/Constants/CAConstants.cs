﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaSharpServer.Constants
{
    internal enum CAConstants : ushort
    {
        DO_REPLY = 10,
        DONT_REPLY = 5,
        /// <summary>
        /// Minor revision of channel access protocol implemented in this library
        /// </summary>
        CA_MINOR_PROTOCOL_REVISION = 11
    }
}
