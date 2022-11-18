using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Sockets.Socket;

public class Unity_server : MonoBehaviour
{

    public static bool isConnected = false;
    public static string data = null;
    public static Socket clientSocket;
    public static byte[] bytes;

    private Texture2D tex;
    private RawImage rawImage;
    private static byte[] imageData;

    // Start is called before the first frame update
    async void Start()
    {
        tex = new Texture2D(1, 1);
        rawImage = GetComponent<RawImage>();
        await Task.Run(() => StartServer());
    }

    // Update is called once per frame
    void Update()
    {
        tex.LoadImage(imageData);
        rawImage.texture = tex;
    }

    public static void StartServer()
    {
        if (isConnected)
        {
            print("A Client is already connected!!!");
        }

        else{

            var ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostEntry.AddressList[0];
            var localEndPoint = new IPEndPoint(ipAddress, 20202);

            var listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
               {
                 listener.Bind(localEndPoint);
                 listener.Listen(10);

                 while (true)
                 {
                     print("Waiting connection ... ");
                     clientSocket = listener.Accept();

                     bytes = new byte[1024];
                     //string data = null;


                     var numByte = clientSocket.Receive(bytes);

                     data += Encoding.ASCII.GetString(bytes, 0, numByte);

                     print("Command received - > { " + data +" }");

                     var message = Encoding.ASCII.GetBytes("Command received - > { "+ data +" }");

                     clientSocket.Send(message);

                     isConnected = true;


                     switch (data)
                     {
                        case "0.Close":
                                Close();
                                break;

                        case "1.Message":
                                RecvMessage();
                                break;

                        case "2.Image":
                                RecvImage();
                                break;

                        case "3.im_screen":
                                //RecvMessage();
                                break;

                        case "4.im_read":
                                //RecvMessage();
                                break;
                        
                        default:
                                print("Unknown command request!!!");
                                break;
                     }
                 }
               }
               catch(Exception e)
               {
                print(e.ToString());
               }
        
        }
    }

    public static void Close()
    {
        if (isConnected)
        {
            try{
                print("Server: Connection closed!");
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();

                isConnected = false;
                

            }
            catch(Exception e)
            {
                print(e.ToString());
            }
        }
        else
        {
            print("Server: 0 connections available to close!!!");
        }
    }

    public static void RecvMessage()
    {
        if (isConnected)
        {
            try{

                bytes = null;
                data = null;

                bytes = new byte[1024];
                    
                var NumByte = clientSocket.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, NumByte);
                print("Message received - > { " + data +" }");

                var messageOne = Encoding.ASCII.GetBytes("Server: Message received - > { "+ data +" }");
                clientSocket.Send(messageOne);
                data = null;
            }

            catch (Exception e)
            {
                print(e.ToString());
            }
        }
        else
        {
            print("Server: No client connected to receive message!!!!!");
        }
    }

    public static void RecvImage()
    {
        if(isConnected)
        {
            try{

                bytes = null;
                data = null;

                bytes = new byte[1024];
                    
                var NumsByte = clientSocket.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, NumsByte);
                print("Image received: File Size - > { " + data +" }");
                var fileSize = int.Parse(data);

                bytes = new byte[fileSize];
                NumsByte = clientSocket.Receive(bytes);

                imageData = bytes;

                data = null;
                 data += Encoding.ASCII.GetString(bytes, 0, NumsByte); 

                var messageTwo = Encoding.ASCII.GetBytes("Server: Image Bytes received - > { "+ data +" }");
                clientSocket.Send(messageTwo);
            }
            catch(Exception e)
            {
                print(e.ToString());
            }
        }
    }

   /* static public void Main(String[] args)
    {
  
        UnityServer.StartServer();
    }*/
}
