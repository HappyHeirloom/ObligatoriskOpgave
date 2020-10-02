using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Fan;
using FanREST.Controllers;
using Newtonsoft.Json;

namespace FanServer
{
    class Server
    {
        //Output objects
        private static List<FanOutput> FanServerList = GetAllFanOutputs();
        private static readonly FanOutput FanFromId = GetFanFromID(0);

        //JSON lists
        private static readonly List<string> JsonList = new List<string>();


        
        //private static string urlLinkToServer = "http://localhost:57982/fanoutput";

        private static int _clientNr = 0;

        private static string serverOutput = "Enter an input.. I.e HentAlle, Hent, Gem. To exit the program, please write exit";


        public static void StartServer()
        {
            int port = 4646;
            TcpListener listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            Console.WriteLine("Server started...");

            while (true)
            {
                TcpClient socket = listener.AcceptTcpClient();
                _clientNr++;
                Console.WriteLine("User connected");
                Console.WriteLine($"Number of users online {_clientNr}");

                Task.Run(() =>
                    {
                        TcpClient tempSocket = socket;
                        DoServer(tempSocket);
                    }
                );
            }
        }


        public static void DoServer(TcpClient socket)
        {
            NetworkStream ns = socket.GetStream();
            StreamReader reader = new StreamReader(ns);
            StreamWriter writer = new StreamWriter(ns) { AutoFlush = true };

            writer.WriteLine("Server started...");

            try
            {
                Console.WriteLine($"Sending '{serverOutput}' to client.");
                writer.WriteLine(serverOutput);
                bool correctInput = false;
                bool exit = false;
                var rightId = false;

                while (exit == false)
                {
                    while (correctInput == false)
                    {
                        var userInput = reader.ReadLine();

                        if (userInput != null)
                        {
                            switch (userInput.ToString().ToLower())
                            {
                                case "hentalle":
                                    UpdateList();
                                    foreach (var fan in FanServerList)
                                    {
                                        var json = JsonConvert.SerializeObject(fan);
                                        serverOutput = json;
                                        Console.WriteLine($"Sending '{serverOutput}' to client.");
                                        writer.WriteLine(serverOutput + ",");
                                    }
                                    correctInput = true;
                                    OutputStandardMessage(writer);
                                    break;
                                case "hent":
                                    UpdateList();
                                    while (rightId == false)
                                    {
                                        serverOutput = $"Enter an id between 1 and {FanServerList.Count}";
                                        Console.WriteLine($"Sending '{serverOutput}' to client.");
                                        writer.WriteLine(serverOutput);
                                        userInput = reader.ReadLine();
                                        if (userInput != null)
                                        {
                                            int userInputAsInt = Convert.ToInt32(userInput);
                                            if (userInputAsInt >= 1 && userInputAsInt <= FanServerList.Count)
                                            {
                                                var json = JsonConvert.SerializeObject(
                                                    GetFanFromID(userInputAsInt));
                                                serverOutput = json;
                                                Console.WriteLine($"Sending '{serverOutput}' to client.");
                                                writer.WriteLine(serverOutput);
                                                rightId = true;
                                            }
                                        }
                                    }
                                    rightId = false;
                                    correctInput = true;
                                    OutputStandardMessage(writer);
                                    break;
                                case "gem":
                                    UpdateList();
                                    bool rightName = false;
                                    bool rightTemp = false;
                                    bool rightHumid = false;

                                    serverOutput = "Do you want to input as text or json?";
                                    Console.WriteLine($"Sending '{serverOutput}' to client.");
                                    writer.WriteLine(serverOutput);

                                    userInput = reader.ReadLine();

                                    switch (userInput.ToLower())
                                    {
                                        case "text":
                                            int FanId = FanServerList.Count + 1;
                                            string name = "";
                                            int temp = 15;
                                            int humid = 30;
                                            while (rightHumid == false)
                                            {
                                                while (rightTemp == false)
                                                {
                                                    while (rightName == false)
                                                    {
                                                        serverOutput = "Enter a fan name: (min 2. characters)";
                                                        Console.WriteLine($"Sending '{serverOutput}' to client.");
                                                        writer.WriteLine(serverOutput);

                                                        userInput = reader.ReadLine();
                                                        if (userInput != null && userInput.Length >= 2)
                                                        {
                                                            name = userInput;
                                                            rightName = true;
                                                        }
                                                    }
                                                    serverOutput = "Enter the temperature: (Must be between 15 and 25)";
                                                    Console.WriteLine($"Sending '{serverOutput}' to client.");
                                                    writer.WriteLine(serverOutput);

                                                    userInput = reader.ReadLine();
                                                    int userInputAsIntTemp = Convert.ToInt32(userInput);
                                                    if (userInputAsIntTemp >= 15 && userInputAsIntTemp <= 25)
                                                    {
                                                        temp = userInputAsIntTemp;
                                                        rightTemp = true;
                                                    }
                                                }
                                                serverOutput = "Enter the temperature: (Must be between 30 and 80)";
                                                Console.WriteLine($"Sending '{serverOutput}' to client.");
                                                writer.WriteLine(serverOutput);

                                                userInput = reader.ReadLine();
                                                int userInputAsIntHumid = Convert.ToInt32(userInput);
                                                if (userInputAsIntHumid >= 30 && userInputAsIntHumid <= 80)
                                                {
                                                    humid = userInputAsIntHumid;
                                                    rightHumid = true;
                                                }
                                            }
                                            SaveFan(FanId, name, temp, humid);
                                            serverOutput =
                                                "Created fan with " +
                                                $"id = {FanId} and a " +
                                                $"name = {name} with a " +
                                                $"temperature of {temp} and a " +
                                                $"humidity of {humid}";
                                            Console.WriteLine($"Sending '{serverOutput}' to client.");
                                            writer.WriteLine(serverOutput);
                                            break;
                                        case "json":
                                            serverOutput = "Enter a json string";
                                            Console.WriteLine($"Sending '{serverOutput}' to client.");
                                            writer.WriteLine(serverOutput);

                                            serverOutput = $"e.g. {GetFanFromID(3)} but without id. ";

                                            userInput = reader.ReadLine();

                                            var userInputAsObj = JsonConvert.DeserializeObject<FanOutput>(userInput);

                                            userInputAsObj.Id = FanServerList.Count + 1;

                                            SaveFanJson(userInputAsObj);

                                            serverOutput = userInputAsObj.ToString();
                                            Console.WriteLine($"Sending '{serverOutput}' to client.");
                                            writer.WriteLine(serverOutput);

                                            break;

                                        default:
                                            serverOutput = "Enter a valid input. 'text' or 'json'";
                                            Console.WriteLine($"Sending '{serverOutput}' to client.");
                                            writer.WriteLine(serverOutput);
                                            break;
                                    }


                                    rightName = false;
                                    rightTemp = false;
                                    rightHumid = false;
                                    correctInput = true;
                                    OutputStandardMessage(writer);
                                    break;
                                case "exit":
                                    Console.WriteLine("Exit");
                                    correctInput = true;
                                    exit = true;
                                    break;

                                default:
                                    Console.WriteLine($"Sending '{serverOutput}' to client.");
                                    writer.WriteLine(serverOutput);
                                    correctInput = false;
                                    break;
                            }
                        }
                    }
                    correctInput = false;
                }

            }
            catch (Exception e)
            {
                if (e.Message == 
                    "Unable to read data from the transport connection: En eksisterende forbindelse blev tvangsafbrudt af en ekstern vært.."
                    || e.Message == "Unable to read data from the transport connection: An existing connection was forcibly closed by the remote host.."
                    )
                {
                    ns.Close();
                    _clientNr--;
                    Console.WriteLine($"User disconnected... current number of users: {_clientNr}");
                }

                Console.WriteLine(e.Message);
                throw;
            }

            ns.Close();
            _clientNr--;
            Console.WriteLine($"User disconnected... current number of users: {_clientNr}");

        }

        public static void OutputStandardMessage(StreamWriter writer)
        {
            // Rewrite users option to client
            serverOutput = "Enter an input.. I.e HentAlle, Hent, Gem. To exit the program, please write exit";
            Console.WriteLine("");
            Console.WriteLine($"Sending '{serverOutput}' to client.");
            writer.WriteLine("");
            writer.WriteLine(serverOutput);
        }

        public static List<FanOutput> GetAllFanOutputs()
        {
            List<FanOutput> AllFansList = new List<FanOutput>();

            FanOutPutController fanRestController = new FanOutPutController();

            var fanRestList = fanRestController.Get();

            foreach (var fan in fanRestList)
            {
                AllFansList.Add(fan);
            }

            return AllFansList;
        }

        public static FanOutput GetFanFromID(int id)
        {
            FanOutput FanFromInput = null;

            foreach (var fan in FanServerList)
            {
                if (fan.Id == id)
                {
                    FanFromInput = fan;
                }
            }

            return FanFromInput;
        }

        public static void SaveFan(int id, string name, int temp, int humid)
        {
            FanOutput savedFan = new FanOutput(id, name,temp,humid);
            FanOutPutController con = new FanOutPutController();

            con.Post(savedFan);

            //FanServerList.Add(savedFan);
        }

        public static void SaveFanJson(FanOutput el)
        {
            FanOutPutController con = new FanOutPutController();

            con.Post(el);
        }

        public static void UpdateList()
        {
            var updatedList = GetAllFanOutputs();

            FanServerList = updatedList;
        }

    }
}
