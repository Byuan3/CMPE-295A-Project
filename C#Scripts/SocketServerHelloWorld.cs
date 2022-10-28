using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static System.Net.Sockets.Socket;


public class SocketServer : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        await Task.Run(() => StartServer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private static void StartServer()
    {
        var ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
        var ipAddress = ipHostEntry.AddressList[0];
        var localEndPoint = new IPEndPoint(ipAddress, 9998);

        var listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(10);

            while (true)
            {
                print("Waiting connection ... ");
                var clientSocket = listener.Accept();

                var bytes = new byte[1024];
                string data = null;


                var numByte = clientSocket.Receive(bytes);

                data += Encoding.ASCII.GetString(bytes, 0, numByte);

                print("Text received - > { " + data +" }");

                var message = Encoding.ASCII.GetBytes("Test Server");

                clientSocket.Send(message);
                
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            
        }
        catch (Exception exception)
        {
            print(exception.ToString());
        }
    }
}
