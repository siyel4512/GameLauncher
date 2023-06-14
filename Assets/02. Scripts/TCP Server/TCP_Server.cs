using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCP_Server : MonoBehaviour
{
    public TcpListener Server;
    public TcpClient Client;

    public Thread ListenThread;
    private int port = 8000;

    public void StartServer()
    {
        ListenThread = new Thread(new ThreadStart(Listen));
        ListenThread.Start();
    }

    private void Listen()
    {
        Server = new TcpListener(IPAddress.Any, port);

        try
        {
            Server.Start(); // TCP Server start
            Console.WriteLine("Start Server");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        try
        {
            while (true)
            {
                Client = Server.AcceptTcpClient();

                NetworkStream stream = Client.GetStream();

                string msg = MainWindow.token;
                int byteCount = Encoding.UTF8.GetByteCount(msg);

                byte[] sendBuffer = new byte[byteCount];
                sendBuffer = Encoding.UTF8.GetBytes(msg);

                stream.Write(sendBuffer, 0, sendBuffer.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
