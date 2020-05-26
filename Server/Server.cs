using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheShip;
using TheStar;
using TheWorld;
using Projectile;
using Vector;
using System.Diagnostics;
using System.Xml;
using NetworkController;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;

namespace Server
{

    class Servers
    {
        private static int MSPerframe;
        private static int UniverseSize;
        private static int FramesPerShot;
        public static int RespawnRate;

        private static List<Star> StarList;
        public static List<SocketState> Clients;

        private static int ID = 0;
        private static int StarID = 0;

        static private TcpListener lstn;
        private static World world;


        private static string filename = @"../../../Server/settings.xml";

        public const string connectionString = "server=atr.eng.utah.edu;" +
              "database=cs3500_u0952138;" +
              "uid=cs3500_u0952138;" +
              "password=t!ct@c";



        static void Main(string[] args)
        {

   
            ReadXML(filename);


            Servers ser = new Servers();

            ser.StartServer();
            /// 1. read XML file for server settings X
            /// 2. Run an event-loop that listens for TCP socket connections from clients. implement
            /// server side handshake
            /// 3.start thread with infinite loop and update the world every time a new frame must be computed
            /// 


            while (true)
            {
            
                update();
            }
        }
        //implements MySQL server Code
        static void OnPressExit(object sender, EventArgs e)
        {
            string time = DateTime.Now.ToString();

            Dictionary<int, Ship> Server = new Dictionary<int, Ship>();

            world.GetShips().ToList().ForEach(x => Server.Add(x.Value.GetID(), x.Value));
            world.GetGraveyard().ToList().ForEach(x => Server.Add(x.Value.GetID(), x.Value));
            world.GetDead().ToList().ForEach(x => Server.Add(x.Value.GetID(), x.Value));

            Random rnd = new Random();
            int GameID = rnd.Next(0, 9999);

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    string q1 = "INSERT INTO `cs3500_u0952138`.`SpaceWarGames` (`GameID`,`Date`) VALUES(@GameID, @parTime);";

                    MySqlCommand cmd1 = new MySqlCommand(q1, conn);

                    cmd1.Parameters.AddWithValue("@GameID", GameID);
                    cmd1.Parameters.AddWithValue("@parTime", time);

                    MySqlDataReader reader = cmd1.ExecuteReader();

                    reader.Close();

                    Console.WriteLine("Success adding game");

                    
                    foreach (KeyValuePair<int, Ship> entry in Server)
                    { 
                        string Query = @"INSERT INTO `cs3500_u0952138`.`Players` (`GameID`, `ID`,`PlayerName`) VALUES (@GameID, @par1, @par2);
                                        INSERT INTO `cs3500_u0952138`.`Stats` (`ID`, `Score`,`Accuracy`) VALUES(@par1, @par3, @par4);";
                                        

                        MySqlCommand cmd2 = new MySqlCommand(Query, conn);
                        cmd2.Parameters.AddWithValue("@GameID", GameID);
                        cmd2.Parameters.AddWithValue("@par1", entry.Value.GetID());
                        cmd2.Parameters.AddWithValue("@par2", entry.Value.GetName());
                        cmd2.Parameters.AddWithValue("@par3", entry.Value.GetScore());
                        cmd2.Parameters.AddWithValue("@par4", entry.Value.shotPercentage());
                        MySqlDataReader reader1 = cmd2.ExecuteReader();
                        reader1.Close();
                        Console.WriteLine("Success adding ships");
                    }
                }
                catch (Exception c)
                {
                    Console.WriteLine(c.Message);
                }

            }
        }

        public Servers()
        {
            //ctrl + c to update server when tabbed into the console
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnPressExit);

            lstn = new TcpListener(IPAddress.Any, Networking.DEFAULT_PORT);

            Clients = new List<SocketState>();

            world = new World();

            world.SetRespawn(RespawnRate);
            world.SetWorldSize(UniverseSize);
            world.SetFireRate(FramesPerShot);
           
            foreach (Star s in StarList)
            {
                world.GetStar().Add(s.GetID(), s);
            }

        }
        private void StartServer()
        {
            Console.WriteLine("Server waiting for client");
            Console.WriteLine("Press ctrl + c to send data to server!");
            // This begins an "event loop".
            // ConnectionRequested will be invoked when the first connection arrives.

            Networking.ServerAwaitingClientLoop(HandleNewClients);

        }


        public static void HandleNewClients(SocketState s)
        {

            s.callMe = ReceiveName;

            Networking.GetData(s);

        }

        private static void ReceiveName(SocketState state)
        {

            StringBuilder sb = new StringBuilder();

            string playerName = state.sb.ToString();// Get playername

            string playerName2 = Regex.Replace(playerName.Trim(), @"\t|\n|\r", "");


            //the if else statments are just to show that the controls are working 

            GetInputs(state);
            state.sb.Clear();

            if (playerName2.Length > 1 && !playerName.Contains("("))
            {
                lock (Clients) // Adding clients to list
                {
                    Clients.Add(state);
                }
                //Calls for Assign ID and allows asssignID to increment to give a unique ID 
                AssignID();
                //create a random ship for player with the ID and name
                Ship s = RandomShipGen(ID, playerName2);
                //add it to the world dictionry for ships 

                lock (world.GetShips())
                {
                    world.GetShips().Add(s.GetID(), s);
                }
                sb.Append(ID + "\n" + UniverseSize);

                //seeing tokens 

                string final = sb.ToString();

                //sends the intial startup 

                Networking.SendData(state.theSocket, final);
            }

            //upon recieving the name, send the startup data
            //the server must not send any wold data to a client before sending the startup data.

        }

        public static int AssignID()
        {
            return ID++;
        }


        public static Ship RandomShipGen(int ID, string Name)
        {

            Ship s = new Ship();
            s.SetShipID(ID);
            s.SetShipName(Name);
            s.setFramesPerShot(FramesPerShot);
            Random random = new Random();

            double j = random.Next(-UniverseSize / 2, UniverseSize / 2);

            double k = random.Next(-UniverseSize / 2, UniverseSize / 2);

            s.SetShipLocation(j, k);

            return s;
        }

        private static void update()
        {

            StringBuilder sb = new StringBuilder();

            Stopwatch watch = new Stopwatch();

            watch.Start();

            while (watch.ElapsedMilliseconds < MSPerframe)
            {
                /*do nothing */
            }
            watch.Reset();
            //update stuff

            lock (Servers.world)
            {
                world.Update();

                foreach (KeyValuePair<int, Star> entrys in world.GetStar().ToList())
                {
                    string message = JsonConvert.SerializeObject(entrys.Value);

                    sb.Append(message + "\n");
                }
                foreach (KeyValuePair<int, Ship> entrys in world.GetShips().ToList())
                {
                    string message = JsonConvert.SerializeObject(entrys.Value);

                    sb.Append(message + "\n");
                }
                foreach (KeyValuePair<int, Proj> entrys in world.GetProj().ToList())
                {
                    string message = JsonConvert.SerializeObject(entrys.Value);

                    sb.Append(message + "\n");
                }
                world.CleanUp();
            }
            lock (Clients)
            {
                foreach (SocketState j in Clients)
                {
                    Networking.SendData(j.theSocket, sb.ToString());
                }
            }
        }
        public static void GetInputs(SocketState sock)

        { 

            char[] x = new char[1] { '\n' };

            HashSet<string[]> test = new HashSet<string[]>();

            //this regex gets everything inbetween parenthesis
            string pattern = Regex.Escape("(") + "(.*?)";

            Ship s = new Ship();

            try
            {
                int playerID = sock.ID;
                s = world.ship[playerID];
            }
            catch
            {

            }

            string[] strs = sock.sb.ToString().Split(x, StringSplitOptions.RemoveEmptyEntries);

            
            foreach (string str in strs)
            {

                if (str.Length > 1 && Regex.IsMatch(str, "^[(]") && str[str.Length-1] == 41)
                {
                    {
                        if (str.Contains("F"))
                            s.FlagShoot();
                        if (str.Contains("R"))
                            s.FlagRight();
                        if (str.Contains("L"))
                            s.FlagLeft();
                        if (str.Contains("T"))
                            s.FlagThrust();
                    }

                }
                sock.sb.Remove(0, str.Length-1);
            }
            Networking.GetData(sock);
        }

        public static int AssignIDStar()
        {
            return StarID++;
        }
        //read xml server file 
        public static void ReadXML(string filename)
        {

            Star s = null;
            Vector2D loc;
            double mass;

            StarList = new List<Star>();

            using (XmlReader reader = XmlReader.Create(filename))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "SpaceSettings":

                                break;
                            case "UniverseSize":
                                reader.Read();
                                int.TryParse(reader.Value, out int i);
                                UniverseSize = i;
                                break;
                            case "MSPerFrame":
                                reader.Read();
                                int.TryParse(reader.Value, out int j);
                                MSPerframe = j;
                                break;

                            case "FramesPerShot":
                                reader.Read();
                                int.TryParse(reader.Value, out int k);
                                FramesPerShot = k;
                                break;

                            case "RespawnRate":
                                reader.Read();
                                int.TryParse(reader.Value, out int l);
                                RespawnRate = l;
                                break;

                            case "Star":
                                reader.Read();//read x
                                reader.Read();
                                reader.Read();
                                double.TryParse(reader.Value, out double x);
                                reader.Read();//read y
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                double.TryParse(reader.Value, out double y);
                                loc = new Vector2D(x, y);
                                reader.Read();//mass
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                double.TryParse(reader.Value, out double m);
                                mass = m;
                                s = new Star(AssignIDStar(), loc, mass);
                                StarList.Add(s);
                                break;
                        }

                    }

                }

            }

        }

    }
}

