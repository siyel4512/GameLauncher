using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class TCP_Server
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

    public void StopServer()
    {
        if (Server != null)
        {
            Server.Stop();
            Server = null;
        }

        if (ListenThread != null)
        {
            ListenThread.Abort();
            ListenThread = null;
        }
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

                string msg;

                if (DEV.instance.isTEST_Login)
                {
                    msg = "Test";
                }
                else
                {
                    msg = Login.PID;
                }


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
        //catch (ThreadAbortException)
        //{
        //     스레드가 중지되었을 때 발생하는 예외 처리
        //    Console.WriteLine("stop thread");
        //}
        //finally
        //{
        //    if (Server != null)
        //    {
        //        Server.Stop();
        //        Server = null;
        //    }
        //}
    }
}
