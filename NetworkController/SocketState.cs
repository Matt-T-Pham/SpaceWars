using System;
using System.Net.Sockets;
using System.Text;

namespace NetworkController
{
    /// <summary>
    /// Pulled from Lab11
    /// </summary>
    public class SocketState
    {
        public Socket theSocket;

        public int ID;

        // This is the buffer where we will receive data from the socket
        public byte[] messageBuffer = new byte[1024];

        // This is a larger (growable) buffer, in case a single receive does not contain the full message.
        public StringBuilder sb = new StringBuilder();

        // This is how the networking library will "notify" users when a connection is made, or when data is received.
        public NetworkAction callMe;

        //public Action<SocketState> callMe;
        public SocketState(Socket s, int id)
        {
            theSocket = s;
            ID = id;
        }
    }
}