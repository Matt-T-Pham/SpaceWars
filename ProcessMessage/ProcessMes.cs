using NetworkController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using TheShip;
using Projectile;
using TheStar;
using System.Windows.Forms;
using TheWorld;
using DrawingPanel;
using System.Net.Sockets;


namespace ProcessMessage
{
    public class ProcMessage
    {
        private DrawPanel drawingPanel;

        private World theWorld;

        private bool leftPressed = false;

        private bool rightPressed = false;

        private bool upPressed = false;

        private bool spacePressed = false;

        private Socket theServer;
        /// <summary>
        /// Receiving the JSON objects and parsing them 
        /// </summary>
        /// <param name="state"></param>
        public void ReceiveWorld(SocketState state)
        {
            string totalData = state.sb.ToString();

            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            Proj proj;
            Ship ship;
            Star star;

            // Loop until we have processed all messages.
            // We may have received more than one.
            lock (drawingPanel)
            {
                foreach (string p in parts)
                {
                    // Ignore empty strings added by the regex splitter
                    if (p.Length == 0)
                        continue;
                    // The regex splitter will include the last string even if it doesn't end with a '\n',
                    // So we need to ignore it if this happens. 
                    if (p[p.Length - 1] != '\n')
                        break;

                    //if string has ship in it create a new ship
                    if (p.Contains("ship"))
                    {
                        ship = JsonConvert.DeserializeObject<Ship>(p);

                        if (!ship.GetActive())
                            theWorld.GetShips().Remove(ship.GetID());
                        else
                            theWorld.GetShips()[ship.GetID()] = ship;
                    }

                    else if (p.Contains("star"))
                    {

                        star = JsonConvert.DeserializeObject<Star>(p);
                        theWorld.GetStar()[star.GetID()] = star;

                    }
                    //if string has projectile in create a new projectile 
                    else if (p.Contains("proj"))
                    {

                        proj = JsonConvert.DeserializeObject<Proj>(p);

                        if (!proj.GetActive())
                            theWorld.GetProj().Remove(proj.GetID());
                        else
                            theWorld.GetProj()[proj.GetID()] = proj;

                    }
                    //Used to detect input from the controlle
                    if (leftPressed == true)
                    {
                        string left = "(L)";
                        byte[] messageBytes = Encoding.UTF8.GetBytes(left + "\n");
                        theServer.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, sendCallBack, theServer);
                    }
                    if (rightPressed == true)
                    {
                        string right = "(R)";
                        byte[] messageBytes = Encoding.UTF8.GetBytes(right + "\n");
                        theServer.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, sendCallBack, theServer);
                    }
                    if (upPressed == true)
                    {
                        string thrust = "(T)";
                        byte[] messageBytes = Encoding.UTF8.GetBytes(thrust + "\n");
                        theServer.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, sendCallBack, theServer);
                    }
                    if (spacePressed == true)
                    {
                        string fire = "(F)";
                        byte[] messageBytes = Encoding.UTF8.GetBytes(fire + "\n");
                        theServer.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, sendCallBack, theServer);
                    }

                    // Then remove the processed message from the SocketState'p growable buffer
                    state.sb.Remove(0, p.Length);
                }

            }

            // Now ask for more data. This will start an event loop.
            //Redraw();
            Networking.GetData(state);
        }

        private void sendCallBack(IAsyncResult ar)
        {
            Socket s = (Socket)ar.AsyncState;
            // Nothing much to do here, just conclude the send operation so the socket is happy.
            s.EndSend(ar);
            // We don't want to start an event loop for sending.
        }
    }
}
