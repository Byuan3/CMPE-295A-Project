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
using UnityEngine.Networking;
using System.IO;


public class UnityServer : MonoBehaviour
{
    // Start is called before the first frame update

    public Renderer screenGrabRenderer;

    static Texture2D destinationTexture;
    static bool isPerformingScreenGrab;
    static bool captureScreen = false;

    static bool sendImage = false;
    static byte[] imageBytes;

    async void Start()
    {

        // Create a new Texture2D with the width and height of the screen, and cache it for reuse
        destinationTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        await Task.Run(() => StartServer());
    }

    // Update is called once per frame
    void Update()
    {
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

                File.WriteAllBytes("ScreenCapture.png", bytes);

                imageBytes = bytes;
                print("Bytes Length: " + imageBytes.Length);
                print("Image bytes: " + bytes);
                sendImage = true;
                print("Send Image: " + sendImage);
                captureScreen = false;
                // Reset the isPerformingScreenGrab state
                isPerformingScreenGrab = false;
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


    async private static void StartServer()
    {
        var ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
        var ipAddress = ipHostEntry.AddressList[0];
        var localEndPoint = new IPEndPoint(ipAddress, 9997);

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

                print("Text received - > { " + data + " }");

                var message = Encoding.ASCII.GetBytes("Test Server");

                clientSocket.Send(message);

                data = null;

                numByte = clientSocket.Receive(bytes);

                data += Encoding.ASCII.GetString(bytes, 0, numByte);
                if (data == "Capture Screen")
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

                    numByte = clientSocket.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, numByte);
                    print("File Size - > { " + data + " }");
                    var fileSize = int.Parse(data);

                    //check if client received proper file size sent from server
                    if (fileSize == imageBytes.Length)
                    {
                        print("Client recived the exact filsize sent by server.");
                        var n = imageBytes.Length / 1024;
                        var last = imageBytes.Length % 1024;
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
                            clientSocket.Send(imageBytes, fileSize - last, last, SocketFlags.None);
                            print("i: " + (fileSize - last) + " ,j-last: " + last);
                        }
                    }

                    // IMAGE FILE //

                    data = null;
                    numByte = clientSocket.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, numByte);

                    print("FIle asked for is: " + data);
                    byte[] fileBytes = null;
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

                        var n = fileBytes.Length / 1024;
                        var last = fileBytes.Length % 1024;
                        var j = 1;
                        var i = 0;
                        for (i = 0, j = 1; j <= n; j++, i += 1024)
                        {
                            clientSocket.Send(fileBytes, i, 1024, SocketFlags.None);
                            print("sent: " + (i + 1024) + " ,j: " + j);

                        }
                        print("In between");

                        if (last != 0)
                        {
                            var lastOffset = (fileBytes.Length) - last;
                            clientSocket.Send(fileBytes, lastOffset, last, SocketFlags.None);
                            print("i: " + lastOffset + " ,j-last: " + last);
                        }
                    }
                    else
                    {
                        var messageSent = Encoding.ASCII.GetBytes("File Not Found.");
                        clientSocket.Send(messageSent);
                    }


                    //clientSocket.send();





                    //clientSocket.Send(imageBytes);
                }

                // print("Text received again- > { " + data + " }");





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
