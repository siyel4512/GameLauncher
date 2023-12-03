using System;
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

    public bool isRunning;

    public void Update()
    {
        if (GameManager.instance.isLogin && !isRunning)
        {
            StartServer();
        }
    }

    public void StartServer()
    {
        ListenThread = new Thread(new ThreadStart(Listen));
        ListenThread.Start();
        isRunning = true;
    }

    public void StopServer()
    {
        if (Server != null)
        {
            Server.Stop();
            Server = null;
        }

        if (Client != null)
        {
            Client.Close();
            Client = null;
        }

        if (ListenThread != null)
        {
            ListenThread.Abort();
            ListenThread = null;
        }

        isRunning = false;
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

                using (NetworkStream stream = Client.GetStream())
                {
                    string msg;

                    if (DEV.instance.isTEST_Login)
                    {
                        msg = "Test";
                    }
                    else
                    {
                        msg = Login.PID;
                    }

                    byte[] sendBuffer = Encoding.UTF8.GetBytes(msg);
                    stream.Write(sendBuffer, 0, sendBuffer.Length);

                    // read msg
                    stream.ReadTimeout = 5000;
                    byte[] recevBuffer = new byte[1024];
                    int bytesRead = stream.Read(recevBuffer, 0, recevBuffer.Length);

                    //string receiveMsg = Encoding.UTF8.GetString(recevBuffer, 0, recevBuffer.Length); // 사용할 Nickname를 읽어옴
                    string receiveMsg = Encoding.UTF8.GetString(recevBuffer, 0, bytesRead); // 사용할 Nickname를 읽어옴
                    receiveMsg = receiveMsg.Trim('\0');

                    if (receiveMsg != "" && receiveMsg == "Need NickName")
                    {
                        // send msg
                        msg = Login.nickname;
                        byte[] sendBuffer_2 = Encoding.UTF8.GetBytes(msg);
                        stream.Write(sendBuffer_2, 0, sendBuffer_2.Length);
                    }
                }

                Client.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            if (Server != null)
            {
                Server.Stop();
                Server = null;
            }

            isRunning = false;
        }
    }
}
