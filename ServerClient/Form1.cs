using NetworkController;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheShip;
using TheWorld;
using System.Timers;
using DrawingPanel;
using Projectile;
using Newtonsoft.Json.Linq;
using Vector;
using TheStar;
using ProcessMessage;

namespace ServerClient
{
    public partial class Form1 : Form
    {

        private Socket theServer;

        private World theWorld;

        private DrawPanel drawingPanel;

        private Score sc;

        const int WorldSize = 750;

        private ProcMessage proc;

        private int WS;

        private bool leftPressed = false;

        private bool rightPressed = false;

        private bool upPressed = false;

        private bool spacePressed = false;

        private int PlayerID;

        const int scoreboardSize = 300; 

        public Form1()
        {

            InitializeComponent();

            theWorld = new World();

            SeverInput.Text = "localhost";

            ClientSize = new Size(WorldSize, WorldSize);

            drawingPanel = new DrawPanel(theWorld);
            drawingPanel.Location = new Point(0, 0);
            drawingPanel.Size = new Size(this.ClientSize.Width, this.ClientSize.Height);
            drawingPanel.BackColor = Color.Black;
            

            sc = new Score(theWorld);
            sc.Location = new Point(ClientSize.Width, 0);
            sc.Size = new Size(300, this.ClientSize.Height);
            sc.BackColor = Color.WhiteSmoke;
            ClientSize = new Size(WorldSize+sc.Size.Width, WorldSize);

            this.Controls.Add(sc);
            this.Controls.Add(drawingPanel);
        }
        // Redraw the game.
        public void Redraw()
        {
            try
            {
                MethodInvoker m = new MethodInvoker(() => this.Invalidate(true));
                Invoke(m);
            }
            catch (Exception e)
            {

            }
        }

         
        private void Connect_Click(object sender, EventArgs e)
        {
            if (SeverInput.Text == "" || messages.Text =="")
            {
                MessageBox.Show("please enter server address or name");
                return;
            }
            //Disable button after connecting to server
            Connect.Enabled = false;
            SeverInput.Enabled = false;
            Connect.Visible = false;
            SeverInput.Visible = false;
            messages.Visible = false;

            theServer = Networking.ConnectToServer(FirstContact, SeverInput.Text);

            string message = messages.Text;

            //append a newLine since that is our protocal'p terminating characters for a message

            byte[] messageBytes = Encoding.UTF8.GetBytes(message + "\n");
            //messages.Text = "";
            try
            {
                theServer.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, sendCallBack, theServer);
                this.Focus();
                messages.Enabled = false;
            }catch(Exception )
            {

            }

        }


        private void FirstContact(SocketState state)
        {       
            //state.callMe = proc.ReceiveWorld;
            state.callMe = RecieveStartup;

            Networking.GetData(state);
        }

        private void RecieveStartup(SocketState state)
        {
            string totalData = state.sb.ToString();

            string[] c = totalData.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

            if (c.Length < 3)
            {
                for(int x = 0; x < c.Length; x++)
                {
                    int.TryParse(c[x], out int i);
                    if (i < 100)
                    {
                        i = PlayerID;
                    }
                    else
                    {                      
                        MethodInvoker m = new MethodInvoker(() =>
                        {
                            ClientSize = new Size(i, i);

                            drawingPanel.Size = new Size(this.ClientSize.Width, this.ClientSize.Height);

                            sc.Location = new Point(this.ClientSize.Width, 0);
                            sc.Size = new Size(300,this.ClientSize.Height);
                            ClientSize = new Size(i+sc.Size.Width, i);

                        });
                        Invoke(m);
                    }
                }
            }
            state.callMe = ReceiveWorld;
            Networking.GetData(state);
        }

        private void ReceiveWorld(SocketState state)
        {
            string totalData = state.sb.ToString();
            
            // Messages are separated by newline
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
                        {
                            theWorld.GetDead()[ship.GetID()] = ship;

                            if (theWorld.Score.ContainsKey(ship.GetID()))
                            {
                                theWorld.Score[ship.GetID()] = (ship.GetScore());
                            }
                            else
                            {
                                theWorld.Score.Add(ship.GetID(), ship.GetScore());
                            }

                            theWorld.GetShips().Remove(ship.GetID());
                        }

                        else
                        {
                            theWorld.GetShips()[ship.GetID()] = ship;

                            if (theWorld.Score.ContainsKey(ship.GetID()))
                            {
                                theWorld.Score[ship.GetID()]=(ship.GetScore());
                            }
                            else
                            {
                                theWorld.Score.Add(ship.GetID(), ship.GetScore());
                            }
                            
                            theWorld.GetDead().Remove(ship.GetID());
                        }

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
                    // Checks for input from the controller to set flags for controlling the ship
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
            Redraw();
            Networking.GetData(state);
        }



        private void sendCallBack(IAsyncResult ar)
        {
            Socket s = (Socket)ar.AsyncState;
            // Nothing much to do here, just conclude the send operation so the socket is happy.
            s.EndSend(ar);

            // We don't want to start an event loop for sending.
        }

        /// <summary>
        /// Checks for key pressed to set flags for controls
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.Left)
            {
                leftPressed = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                rightPressed = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                upPressed = false;
            }
            if (e.KeyCode == Keys.Space)
            {
                spacePressed = false;
            }

        }
        /// <summary>
        /// Checks for key pressed to set flags for controls
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Left)
            {
                leftPressed = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                rightPressed = true;
            }
            if (e.KeyCode == Keys.Up)
            {
                upPressed = true;
            }
            if (e.KeyCode == Keys.Space)
            {
                spacePressed = true;
            }
        }
    }
}
