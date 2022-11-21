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
using System.IO;

public class CommuniServer : MonoBehaviour
{

    public static bool isConnected = false;
    public static string data = null;
    public static Socket clientSocket;
    static int nB;


    public Renderer screenGrabRenderer;

    static Texture2D destinationTexture;
    static bool isPerformingScreenGrab;

    public static byte[] bytes;

    public static Texture2D tex;
    public static RawImage rawImage;

    static bool captureScreen = false;

    static bool sendImage = false;
    static byte[] imageBytes;
    private static byte[] imageData;

    static byte[] fileBytes = null;
    static int numBytes = 0;

    // Start is called before the first frame update
    async void Start()
    {
        tex = new Texture2D(1, 1);
        print("tex in start: " + tex != null);
        rawImage = GetComponent<RawImage>();
        print("STart rawImage " + rawImage);

        await Task.Run(() => RecvImage());
    }
    void Update()
    {
        print("Inside update method!");
          if (imageData != null)
        {
            //await WaitOneSecondAsync();
            tex.LoadImage(imageData);
            print("tex Inside Update: " + tex);
            print("rawImage" + rawImage);
            rawImage.texture = tex;
        }
        print("Exiting update method!");
    }

    public static void RecvImage()
    {
        var ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
        var ipAddress = ipHostEntry.AddressList[0];
        var localEndPoint = new IPEndPoint(ipAddress, 20202);

        var listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        listener.Bind(localEndPoint);
            listener.Listen(10);
            bool _continue = true;

            while (true)
            {
                print("Waiting connection ... ");
                clientSocket = listener.Accept();

                bytes = new byte[1024];
                //data = null;


                var numByte = clientSocket.Receive(bytes);

                data += Encoding.ASCII.GetString(bytes, 0, numByte);

                print("Command received - > { " + data +" }");
                

                var message = Encoding.ASCII.GetBytes("Command received - > { "+ data +" }");

                clientSocket.Send(message);
                //bytes = null;

                if (data == "2.Image")
                {
                    print("Inside if statement");
                    var ByteArr = new byte[1024];
                    data = null;
                    
                    var NumsByte = clientSocket.Receive(ByteArr);
                    data += Encoding.ASCII.GetString(ByteArr, 0, NumsByte);
                    print("Command received: File Size - > { " + data +" }");
                    int imageBytesLength = int.Parse(data);
                    print(imageBytesLength);
                

                    imageBytes = new byte[imageBytesLength];
                    print(imageBytes.Length);

                    var n = imageBytesLength / 1024;
                    var last = imageBytesLength % 1024;
                    var j = 1;
                    var i = 0;
                    for (i = 0, j = 1; j <= n; j++, i += 1024)
                    {
                        print("Inside for loop!");
                        clientSocket.Receive(imageBytes, i, 1024, SocketFlags.None);
                        print("received: " + (i + 1024) + " ,j: " + j);
                        print("Exiting for loop!");
 
                    }
                    print("In the middle!");
                    if (last != 0)
                    {
                        clientSocket.Receive(imageBytes, imageBytesLength - last, last, SocketFlags.None);
                        print("i: " + (imageBytesLength - last) + " ,j-last: " + last);
                    }  

                    byte[] imageData2 = imageBytes;

                    print("Image Data length in Server: " + imageData2.Length);

                    if(imageData2.Length == imageBytesLength)
                    {
                        var message1 = Encoding.ASCII.GetBytes("Server: All bytes received! { "+ imageData2.Length +" }");
                        clientSocket.Send(message1);

                        bytes = new byte[1024];

                        numByte = clientSocket.Receive(bytes);

                        data += Encoding.ASCII.GetString(bytes, 0, numByte);

                        print("Command received - > { " + data +" }");

                        if(data == "0.Close")
                        {
                            Close();
                            print("Server connection closed!");
                        }
                    }
                    
                }
                _continue = false;
            }
    }
    public static void Close()
    {
        if (isConnected)
        {
            try
            {
                print("Server: Connection closed!");
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();

                isConnected = false;


            }
            catch (Exception e)
            {
                print(e.ToString());
            }
        }
        else
        {
            print("Server: 0 connections available to close!!!");
        }
    }
  
}
