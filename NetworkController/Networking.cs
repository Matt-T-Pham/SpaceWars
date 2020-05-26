using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace NetworkController
{
    public delegate void NetworkAction(SocketState state);

    /// <summary>
    /// This class holds all the necessary state to represent a socket connection
    /// Note that all of its fields are public because we are using it like a "struct"
    /// It is a simple collection of fields
    /// </summary>

    public class Networking
    {

        public const int DEFAULT_PORT = 11000;
        public const int HTTP_PORT = 80;
        static TcpListener lstn;
       

        static Socket socket;

        private static int id = 1;

        #region //client region


        /// <summary>
        /// Creates a Socket object for the given host string
        /// </summary>
        /// <param name="hostName">The host name or IP address</param>
        /// <param name="socket">The created Socket</param>
        /// <param name="ipAddress">The created IPAddress</param>
        public static void MakeSocket(string hostName, out Socket socket, out IPAddress ipAddress)
        {
            ipAddress = IPAddress.None;
            socket = null;
            try
            {
                // Establish the remote endpoint for the socket.
                IPHostEntry ipHostInfo;

                // Determine if the server address is a URL or an IP
                try
                {
                    ipHostInfo = Dns.GetHostEntry(hostName);
                    bool foundIPV4 = false;
                    foreach (IPAddress addr in ipHostInfo.AddressList)
                        if (addr.AddressFamily != AddressFamily.InterNetworkV6)
                        {
                            foundIPV4 = true;
                            ipAddress = addr;
                            break;
                        }
                    // Didn't find any IPV4 addresses
                    if (!foundIPV4)
                    {
                        System.Diagnostics.Debug.WriteLine("Invalid addres: " + hostName);
                        throw new ArgumentException("Invalid address");
                    }
                }
                catch (Exception)
                {
                    // see if host name is actually an ipaddress, i.e., 155.99.123.456
                    System.Diagnostics.Debug.WriteLine("using IP");
                    ipAddress = IPAddress.Parse(hostName);
                }

                // Create a TCP/IP socket.
                socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

                // Disable Nagle's algorithm - can speed things up for tiny messages, 
                // such as for a game
                socket.NoDelay = true;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Unable to create socket. Error occured: " + e);
                throw new ArgumentException("Invalid address");
            }
        }


        // TODO: Move all networking code to this class. Left as an exercise.
        // It should contain no references to a specific project.
        // We moved most of it out of ChatClient during class, except for the parts related to sending data.
        // We moved the receive and connect parts.


        /// <summary>
        /// Start attempting to connect to the server
        /// </summary>
        /// <param name="hostName"> server to connect to </param>
        /// <returns></returns>
        public static Socket ConnectToServer(NetworkAction callMe, string hostName)
        {
            System.Diagnostics.Debug.WriteLine("connecting  to " + hostName);

            // Create a TCP/IP socket.
            Socket socket;
            IPAddress ipAddress;

            Networking.MakeSocket(hostName, out socket, out ipAddress);

            SocketState state = new SocketState(socket, -1);

            state.callMe = callMe;

            state.theSocket.BeginConnect(ipAddress, Networking.DEFAULT_PORT, ConnectedCallback, state);

            return state.theSocket;

        }

        /// <summary>
        /// This function is "called" by the operating system when the remote site acknowledges connect request
        /// </summary>
        /// <param name="ar"></param>
        private static void ConnectedCallback(IAsyncResult ar)
        {
            SocketState state = (SocketState)ar.AsyncState;

            try
            {
                // Complete the connection.
                state.theSocket.EndConnect(ar);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Unable to connect to server. Error occured: " + e);
                return;
            }

            // Don't start an event loop to receive data from the server. The client might not want to do that.
            //state.theSocket.BeginReceive(state.messageBuffer, 0, state.messageBuffer.Length, SocketFlags.None, ReceiveCallback, state);

            // Instead, just invoke the client's delegate so it can take whatever action it desires.
            state.callMe(state);

        }


        /// <summary>
        /// This function is "called" by the operating system when data is available on the socket
        /// </summary>
        /// <param name="ar"></param>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            SocketState state = (SocketState)ar.AsyncState;

            try
            {
                int bytesRead = state.theSocket.EndReceive(ar);


                // If the socket is still open
                if (bytesRead > 0)
                {
                    string theMessage = Encoding.UTF8.GetString(state.messageBuffer, 0, bytesRead);
                    // Append the received data to the growable buffer.
                    // It may be an incomplete message, so we need to start building it up piece by piece
                    state.sb.Append(theMessage);

                    // We can't process the message directly, because different users of this library might have different
                    // processing needs.
                    //ProcessMessage(state);

                    // Instead, just invoke the client's delegate, so it can take whatever action it desires.
                    state.callMe(state);
                }
            }
            catch
            {
                // remove socket from clients
            }

        }

        /// <summary>
        /// GetData is just a wrapper for BeginReceive.
        /// This is the public entry point for asking for data.
        /// Necessary so that we can separate networking concerns from client concerns.
        /// </summary>
        /// <param name="state"></param>
        public static void GetData(SocketState state)
        {           
            state.theSocket.BeginReceive(state.messageBuffer, 0, state.messageBuffer.Length, SocketFlags.None, ReceiveCallback, state);                      
        }



#endregion



        //TODO error handling
        public static void ServerAwaitingClientLoop(NetworkAction callMe)
        {

            lstn = new TcpListener(IPAddress.Any, Networking.DEFAULT_PORT);
   
            lstn.Start();
           
            NewConnectionState cs = new NewConnectionState(lstn, callMe);
           
            cs.listener = lstn;
           
            cs.callMe = callMe;
       
            lstn.BeginAcceptSocket(AcceptNewClient, cs);
                            

        }
        /// <summary>
        /// Moded method to also take an HTTP port
        /// </summary>
        /// <param name="callMe"></param>
        /// <param name="port"></param>
        public static void ServerAwaitingClientLoop(NetworkAction callMe,int port)
        {

            lstn = new TcpListener(IPAddress.Any, Networking.HTTP_PORT);

            lstn.Start();

            NewConnectionState cs = new NewConnectionState(lstn, callMe);

            cs.listener = lstn;

            cs.callMe = callMe;

            lstn.BeginAcceptSocket(AcceptNewClient, cs);


        }

        public static void AcceptNewClient(IAsyncResult ar)
        {
            NewConnectionState cs = (NewConnectionState)ar.AsyncState;

            socket = cs.listener.EndAcceptSocket(ar);

            SocketState ss = new SocketState(socket, AssignID());

            ss.theSocket = socket;

            ss.callMe = cs.callMe;

            ss.callMe(ss);

            Console.WriteLine("Connection Made!");

            cs.listener.BeginAcceptSocket(AcceptNewClient, cs);


        }
        public static void SendData(Socket socket, string str)
        {
            try
            {
                int offset = 0;

                byte[] bytes = Encoding.UTF8.GetBytes(str + "\n");

                socket.BeginSend(bytes, offset, bytes.Length, SocketFlags.None, new AsyncCallback(Networking.SendCallBack), socket);
            }
            catch(Exception e)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch
                {

                }
            }
        }
        /// <summary>
        /// Sends data and then closes the socket after the send
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="str"></param>
        public static void SendAndClose(Socket socket, string str)
        {
            try
            {
                int offset = 0;

                byte[] bytes = Encoding.UTF8.GetBytes(str + "\r\n");

                socket.BeginSend(bytes, offset, bytes.Length, SocketFlags.None, new AsyncCallback(Networking.SendCallBack), socket);
              
           
            }
            catch (Exception e)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch
                {

                }
            }
        }

        private static void SendCallBack(IAsyncResult ar)
        {

            try
            {
                Socket state = (Socket)ar.AsyncState;
                state.EndSend(ar);
            }
            catch
            {

            }
           
        }
        public static int AssignID()
        {
            return id++;
        }
    }

}