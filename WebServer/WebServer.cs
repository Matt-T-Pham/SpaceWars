using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkController;
using System.Net.Sockets;
using System.Net;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace WebServer
{
    class WebServers
    {
        // This is the header to use when the server recognizes the browser's request
        private const string httpOkHeader =
         "HTTP/1.1 200 OK\r\n" +
         "Connection: close\r\n" +
        "Content-Type: text/html; charset=UTF-8\r\n" +
         "\r\n";
        // This is the header to use when the server does not recognize the browser's request
        private const string httpBadHeader =
          "HTTP/1.1 404 Not Found\r\n" +
          "Connection: close\r\n" +
          "Content-Type: text/html; charset=UTF-8\r\n" +
          "\r\n";


        public const string connectionString = "server=atr.eng.utah.edu;" +
      "database=cs3500_u0952138;" +
      "uid=cs3500_u0952138;" +
      "password=t!ct@c";


        static private TcpListener lstn;
        static void Main(string[] args)
        {
            // Start an event loop that listens for socket connections on port 80
            // This requires a slight modification to the networking library to take the port argument
            Networking.ServerAwaitingClientLoop(HandleHttpConnection, Networking.HTTP_PORT);
            Console.WriteLine("Waiting connection");
            Console.Read();
        }
        /// <summary>
        /// This is the delegate for when a new socket is accepted
        /// The networking library will invoke this method when a browser connects
        /// </summary>
        /// <param name="state"></param>
        public static void HandleHttpConnection(SocketState state)
        {
            // Before receiving data from the browser, we need to change what we do when network activity occurs.
            state.callMe = ServeHttpRequest;

            Networking.GetData(state);
        }
        /// <summary>
        /// This method parses the HTTP request sent by the broswer, and serves the appropriate web page.
        /// </summary>
        /// <param name="state">The socket state representing the connection with the client</param>
        private static void ServeHttpRequest(SocketState state)
        {
            string request = state.sb.ToString();

            // Print it for debugging/examining
            Console.WriteLine("received http request: " + request);

            // If the browser requested the home page (e.g., localhost/)
            if (request.Contains("GET / HTTP/1.1"))
                Networking.SendAndClose(state.theSocket, httpOkHeader + "<h2>here is a web page!</h2>");
            // Otherwise, our very simple web server doesn't recognize any other URL
            else if (request.Contains("GET /scores HTTP/1.1"))
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        // Open a connection
                        conn.Open();

                        // Create a command
                        MySqlCommand command = conn.CreateCommand();
                        command.CommandText = "select PlayerName from Players;";

                        // Execute the command and cycle through the DataReader object
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read()==true)
                            {
                                Networking.SendAndClose(state.theSocket, httpOkHeader + "<h2>" + reader["ID"] + "</h2>");
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            else if (request.Contains("player="))
            {
                //string[] test = Regex.Split(request, "\\=(.*)");
                string test = "joe";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        // Open a connection
                        conn.Open();


                        string com = "select PlayerName from Players where PlayerName = @parm1 ;";

                        // Create a command
   
                        MySqlCommand cmd2 = new MySqlCommand(com, conn);

                        cmd2.Parameters.AddWithValue("@parm1", test);
                        // Execute the command and cycle through the DataReader object
                        using (MySqlDataReader reader = cmd2.ExecuteReader())
                        {
                            while (reader.Read() == true)
                            {
                                Networking.SendAndClose(state.theSocket, httpOkHeader + "<h2>" + reader["PlayerName"] + "</h2>");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            else if (request.Contains("id="))
            {
                string[] test = Regex.Split(request, "\\=[1-9]*");
                
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        // Open a connection
                        conn.Open();


                        string com = "select ID from Players where Players.ID = @parm1 ;";

                        // Create a command

                        MySqlCommand cmd2 = new MySqlCommand(com, conn);

                        cmd2.Parameters.AddWithValue("@parm1", test);
                        // Execute the command and cycle through the DataReader object
                        using (MySqlDataReader reader = cmd2.ExecuteReader())
                        {
                            while (reader.Read() == true)
                            {
                                Networking.SendAndClose(state.theSocket, httpOkHeader + "<h2>" + reader["ID3"] + "</h2>");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            else
                Networking.SendAndClose(state.theSocket, httpBadHeader + "<h2>page not found</h2>");
            // NOTE: The above SendAndClose calls are an addition to the Networking library.
            //       The only difference from the basic Send method is that this method uses a callback
            //     
        }
    }
}
