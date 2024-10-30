using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Sockets;
using Opc.Ua;
using Middleware.controller;
using Middleware.model;
using Middleware.TemporaryDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Middleware
{

    public class MyBackgroundService : BackgroundService
    {
        private const int Port = 4080;
        private static readonly IPAddress AllowedIPAddress = IPAddress.Parse("127.0.0.1");
        private static List<TcpClient> clients = new List<TcpClient>();
        private static CancellationTokenSource cts = new CancellationTokenSource();
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            _ = MonitorClientAsync(cts.Token);

            while (true)
            {
                try
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    //var clientEndpoint = client.Client.RemoteEndPoint as IPEndPoint;
                    lock(clients) clients.Add(client);
                    _= HandleClient(client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private static async Task HandleClient(TcpClient client)
        {
            using (client)
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                try
                {

                    while (true)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        Logger.LogMessage("Receive : " + ByteArrayToDecimal(buffer),"TCP");
                        if (bytesRead == 0)
                        {
                            break;
                        }
                        Console.WriteLine(Encoding.ASCII.GetString(buffer,0,7) == "REQUEST");
                        if (Encoding.ASCII.GetString(buffer, 0, 7) == "REQUEST")
                        {
                            bool isRequestPCBInList = false;
                            foreach (Pcb pcb in PCBData.pcb)
                            {
                                Console.WriteLine(buffer[7]);
                                if (buffer[7] == pcb.number)
                                {
                                    isRequestPCBInList = true;
                                    string requestPrefix = "REQUEST";
                                    byte[] requestBytes = Encoding.UTF8.GetBytes(requestPrefix);
                                    byte[] responseData = new byte[requestBytes.Length + 2];
                                    Buffer.BlockCopy(requestBytes, 0, responseData, 0, requestBytes.Length);
                                    responseData[requestBytes.Length] = buffer[7];
                                    if (!pcb.isDefected)
                                    {
                                        responseData[requestBytes.Length + 1] = 1;
                                    }
                                    else
                                    {
                                        responseData[requestBytes.Length + 1] = 0;
                                    }
                                    await stream.WriteAsync(responseData, 0, responseData.Length);
                                    Logger.LogMessage("Send : " + ByteArrayToDecimal(responseData), "TCP");
                                    pcb.requested = true;
                                    break;
                                }
                            }
                            if (!isRequestPCBInList)
                            {
                                byte[] responseData = Encoding.UTF8.GetBytes("NOTEXIST");
                                await stream.WriteAsync(responseData, 0, responseData.Length);
                            }
                        }
                        else if (Encoding.ASCII.GetString(buffer, 0, 7) == "JOBDONE")
                        {
                            bool isRequestPCBInList = false;
                            foreach (Pcb pcb in PCBData.pcb)
                            {
                                if (buffer[7] == pcb.number)
                                {
                                    isRequestPCBInList = true;
                                    string requestPrefix;
                                    if (pcb.requested)
                                    {
                                        PCBData.pcb.Remove(pcb);
                                        requestPrefix = "SUCCESS";
                                        byte[] requestBytes = Encoding.UTF8.GetBytes(requestPrefix);
                                        byte[] responseData = new byte[requestBytes.Length + 2];
                                        Buffer.BlockCopy(requestBytes, 0, responseData, 0, requestBytes.Length);
                                        responseData[requestBytes.Length] = buffer[7];
                                        await stream.WriteAsync(responseData, 0, responseData.Length);
                                        Logger.LogMessage("Send : " + ByteArrayToDecimal(responseData), "TCP");
                                    }
                                    else
                                    {
                                        requestPrefix = "NOTRQST";
                                        byte[] requestBytes = Encoding.UTF8.GetBytes(requestPrefix);
                                        byte[] responseData = new byte[requestBytes.Length + 2];
                                        Buffer.BlockCopy(requestBytes, 0, responseData, 0, requestBytes.Length);
                                        responseData[requestBytes.Length] = buffer[7];
                                        await stream.WriteAsync(responseData, 0, responseData.Length);
                                        Logger.LogMessage("Send : " + ByteArrayToDecimal(responseData), "TCP");
                                    }
                                    break;
                                }
                            }
                            if (!isRequestPCBInList)
                            {
                                byte[] responseData = Encoding.UTF8.GetBytes("NOTEXIST");
                                await stream.WriteAsync(responseData, 0, responseData.Length);
                            }

                        }
                        else
                        {
                            byte[] responseData = Encoding.UTF8.GetBytes("RCVERROR");
                            await stream.WriteAsync(responseData, 0, responseData.Length);
                            Logger.LogMessage("Send : " + ByteArrayToDecimal(responseData), "TCP");
                        }
                        Console.WriteLine("not found");

                    }
                }
                catch (Exception ex)
                {
                    Logger.LogMessage($"Error : {ex.Message}", "Error");
                }
                finally
                {
                    lock (clients) clients.Remove(client);
                }
            }
        }

        

        private static async Task MonitorClientAsync (CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested) 
            {
                await Task.Delay(5000); // Check every 5 seconds

                lock (clients)
                {
                    for (int i = clients.Count - 1; i >= 0; i--)
                    {
                        TcpClient client = clients[i];
                        if (!IsClientConnected(client))
                        {
                            Console.WriteLine("Client is no longer connected.");
                            clients.RemoveAt(i); // Remove disconnected client
                        }
                    }
                }
            }
        }

        private static bool IsClientConnected(TcpClient client)
        {
            try
            {
                if (client.Client.Poll(0, SelectMode.SelectRead) && client.Client.Available == 0)
                {
                    return false; // Client has disconnected
                }
                return true; // Client is still connected
            }
            catch (SocketException)
            {
                return false; // Client connection failed
            }
        }

        public static string ByteArrayToDecimal(byte[] bytes)
        {
            string separator = " ";
            StringBuilder decimalString = new StringBuilder(bytes.Length * (3 + separator.Length));

            for (int i = 0; i < bytes.Length; i++)
            {
                if (i > 0)
                {
                    decimalString.Append(separator);
                }
                decimalString.Append(bytes[i].ToString()); // Convert to decimal
            }

            return decimalString.ToString();
        }
    }
}
