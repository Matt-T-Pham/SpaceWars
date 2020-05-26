using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkController
{
    public class NewConnectionState
    {  
        public NetworkAction callMe;
        public TcpListener listener;

        public NewConnectionState(TcpListener t , NetworkAction a)
        {

            listener = t;

            callMe = a;

        }

    }
}
