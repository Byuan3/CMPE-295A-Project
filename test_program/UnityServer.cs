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

public class UnityServer : MonoBehaviour
{

    public static bool isConnected = false;
    public static string data = null;
    public static Socket clientSocket;


    public Renderer screenGrabRenderer;

    static Texture2D destinationTexture;
    static bool isPerformingScreenGrab;

    public static byte[] bytes;

    private static Texture2D tex;
    private static RawImage rawImage;

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
        destinationTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        rawImage = GetComponent<RawImage>();
        await Task.Run(() => StartServer());
    }

    // Update is called once per frame
    void Update()
    {
        tex.LoadImage(imageData);
        rawImage.texture = tex;
    }

    static void takePic()
    {
        print("Inside takepic.");
        if (captureScreen)
        {
            isPerformingScreenGrab = true;
            Camera.onPostRender += OnPostRenderCallback;
        }
    }
    static void OnPostRenderCallback(Camera cam)
    {
        if (isPerformingScreenGrab)
        {
            print("Inside OnPostRenderCallback.");
            // Check whether the Camera that has just finished rendering is the one you want to take a screen grab from
            if (cam == Camera.main)
            {
                try
                {
                    // Define the parameters for the ReadPixels operation
                    Rect regionToReadFrom = new Rect(0, 0, Screen.width, Screen.height);
                    int xPosToWriteTo = 0;
                    int yPosToWriteTo = 0;
                    bool updateMipMapsAutomatically = false;

                    // Copy the pixels from the Camera's render target to the texture
                    destinationTexture.ReadPixels(regionToReadFrom, xPosToWriteTo, yPosToWriteTo, updateMipMapsAutomatically);

                    // Upload texture data to the GPU, so the GPU renders the updated texture
                    // Note: This method is costly, and you should call it only when you need to
                    // If you do not intend to render the updated texture, there is no need to call this method at this point
                    destinationTexture.Apply();

                    byte[] bytes = destinationTexture.EncodeToPNG();
                    //         //Object.Destroy(tex);

                    File.WriteAllBytes("ScreenShot.png", bytes);

                    imageBytes = bytes;
                    print("Bytes Length: " + imageBytes.Length);
                    print("Image bytes: " + bytes);
                    sendImage = true;
                    print("Send Image: " + sendImage);
                    captureScreen = false;
                    // Reset the isPerformingScreenGrab state
                    isPerformingScreenGrab = false;
                }
                catch (Exception e)
                {
                    print("Exception inside OnPostRenderCallback():  " + e);
                }
            }
        }
    }

    // Remove the onPostRender callback
    void OnDestroy()
    {
        Camera.onPostRender -= OnPostRenderCallback;
    }

    static async Task WaitOneSecondAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
        Debug.Log("Finished waiting.");
    }

    static void reset()
    {

        captureScreen = false;
        data = null;
        sendImage = false;
        imageBytes = null;
        fileBytes = null;
        numBytes = 0;

    }

    private static string getIPAddress()
    {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
            }

        }
        return localIP;
    }
    public static void StartServer()
    {
        if (isConnected)
        {
            print("A Client is already connected!!!");
        }

        else
        {

            var ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostEntry.AddressList[0];
            var localEndPoint = new IPEndPoint(ipAddress, 20202);
            print("Ip " + getIPAddress().ToString());
            //Security.PrefetchSocketPolicy(ipAddress.AddressFamily.ToString(), 10100);
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


                    var numBytes = clientSocket.Receive(bytes);

                    data += Encoding.ASCII.GetString(bytes, 0, numBytes);

                    print("Command received - > { " + data + " }");
                    clientSocket.Send(Encoding.ASCII.GetBytes(data));


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

                        case "3.imread_screen":
                            ReadScreen();
                            break;

                        case "4.imread_file":
                            ReadFile();
                            break;

                        default:
                            print("Unknown command request!!!: " + data);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                print(e.ToString());
            }

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

    public static void RecvMessage()
    {
        if (isConnected)
        {
            try
            {

                bytes = null;
                data = null;

                bytes = new byte[1024];

                numBytes = clientSocket.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, numBytes);
                print("Message received - > { " + data + " }");

                var messageOne = Encoding.ASCII.GetBytes("Server: Message received - > { " + data + " }");
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
                clientSocket.Send(Encoding.ASCII.GetBytes(data));

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

    public static void ReadFile()
    {
        if (isConnected)
        {
            try
            {

                data = null;
                numBytes = clientSocket.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, numBytes);

                print("File asked for is: " + data);

                try
                {
                    fileBytes = File.ReadAllBytes(data);
                }
                catch (Exception e)
                {
                    print(e);
                }

                // if (!(string.IsNullOrEmpty(fileLength)))
                if (fileBytes != null)
                {
                    print("FIleBytes of asked file: " + fileBytes);
                    string fileLength = (fileBytes.Length).ToString();
                    print("FIlesize of asked file: " + fileLength);
                    clientSocket.Send(Encoding.ASCII.GetBytes(fileLength));

                    //Send File Bytes
                    sendFileBytes(fileBytes.Length, fileBytes);
                }
                else
                {
                    var messageSent = Encoding.ASCII.GetBytes("File Not Found.");
                    clientSocket.Send(messageSent);
                }
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


    async public static void ReadScreen()
    {
        if (isConnected)
        {
            try
            {

                captureScreen = true;
                takePic();
                await WaitOneSecondAsync();
                print("Image bytes Length again: " + imageBytes.Length);

                //Send Filesize to client

                string strLength = (imageBytes.Length).ToString();
                clientSocket.Send(Encoding.ASCII.GetBytes(strLength));


                // Receive filesize ACK from client

                bytes = new byte[1024];
                data = null;

                numBytes = clientSocket.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, numBytes);
                print("File Size - > { " + data + " }");
                var fileSize = int.Parse(data);

                //check if client received proper file size sent from server
                if (fileSize == imageBytes.Length)
                {
                    //Send File Bytes
                    sendFileBytes(imageBytes.Length, imageBytes);

                }

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

    public static void sendFileBytes(int imageBytesLength, byte[] imageBytes)
    {
        if (isConnected)
        {
            try
            {

                print("Client recived the exact filsize sent by server.");
                var n = imageBytesLength / 1024;
                var last = imageBytesLength % 1024;
                var j = 1;
                var i = 0;
                for (i = 0, j = 1; j <= n; j++, i += 1024)
                {
                    clientSocket.Send(imageBytes, i, 1024, SocketFlags.None);
                    print("sent: " + (i + 1024) + " ,j: " + j);

                }
                print("In between");

                if (last != 0)
                {
                    clientSocket.Send(imageBytes, imageBytesLength - last, last, SocketFlags.None);
                    print("i: " + (imageBytesLength - last) + " ,j-last: " + last);
                }
                reset();
            }
            catch (Exception e)
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

