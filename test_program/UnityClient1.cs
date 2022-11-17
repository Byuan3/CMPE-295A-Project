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
using UnityEngine.UI;


public class UnityClient1 : MonoBehaviour
{
    private Texture2D tex;
    public RawImage rawImage;
    static byte[] imageData = null;
    static byte[] imageData2 = null;


    // Start is called before the first frame update
    async void Start()
    {
        tex = new Texture2D(1, 1);
        print("tex in start: " + tex != null);
        rawImage = GetComponent<RawImage>();
        print("STart rawImage " + rawImage);

        await Task.Run(() => StartClient());
    }

    // Update is called once per frame
    void Update()
    {
        print("Update Called\n");
        // a
        if (imageData != null)
        {
            //await WaitOneSecondAsync();
            tex.LoadImage(imageData);
            print("tex Inside Update: " + tex);
            print("rawImage" + rawImage);
            rawImage.texture = tex;
        }

        if (imageData2 != null)
        {
            //await WaitOneSecondAsync();
            tex.LoadImage(imageData2);
            print("tex Inside Update: " + tex);
            print("rawImage" + rawImage);
            rawImage.texture = tex;
        }
        print("Update Ended");
    }

    static async Task WaitOneSecondAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(5));
        Debug.Log("Finished waiting.");
    }

    private static void StartClient()
    {

        try
        {

            // Establish the remote endpoint
            // for the socket. This example
            // uses port 11111 on the local
            // computer.
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 9997);

            // Creation TCP/IP Socket using
            // Socket Class Constructor
            Socket sender = new Socket(ipAddr.AddressFamily,
                       SocketType.Stream, ProtocolType.Tcp);

            try
            {

                // Connect Socket to the remote
                // endpoint using method Connect()
                sender.Connect(localEndPoint);

                // We print EndPoint information
                // that we are connected
                Console.WriteLine("Socket connected to -> {0} ",
                              sender.RemoteEndPoint.ToString());

                // Creation of message that
                // we will send to Server
                byte[] messageSent = Encoding.ASCII.GetBytes("Test Client<EOF>");
                int byteSent = sender.Send(messageSent);
                String data = null;

                var bytes = new byte[1024];
                var numByte = sender.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, numByte);

                print(data);

                // SCREENSHOT //

                messageSent = Encoding.ASCII.GetBytes("Capture Screen");
                byteSent = sender.Send(messageSent);


                bytes = new byte[1024];
                data = null;

                numByte = sender.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, numByte);
                print("File Size - > { " + data + " }");
                var fileSize = int.Parse(data);

                string strLength = fileSize.ToString();
                sender.Send(Encoding.ASCII.GetBytes(strLength));

                var RecvBytes = new byte[fileSize];


                Int32 bytesImage = 0;
                //Int32 readcount = 0;
                Int32 ip = 0;

                while (bytesImage < fileSize)
                {
                    bytesImage += sender.Receive(RecvBytes, bytesImage, fileSize - bytesImage, SocketFlags.None);


                    ip++;
                    print("Received: " + bytesImage + "  " + ip);
                }

                imageData = RecvBytes;

                print("Image Data length in client: " + imageData.Length);

                // SCREENSHOT END//

                // IMAGE FILE //

                messageSent = Encoding.ASCII.GetBytes("SavedScreen2.png");
                byteSent = sender.Send(messageSent);


                bytes = new byte[1024];
                data = null;

                numByte = sender.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, numByte);
                print("Data first received in IMAGE FILE: " + data);
                if (data != "File Not Found.")
                {
                    print("File Size - > { " + data + " }");
                    fileSize = int.Parse(data);

                    strLength = fileSize.ToString();
                    sender.Send(Encoding.ASCII.GetBytes(strLength));

                    RecvBytes = new byte[fileSize];


                    bytesImage = 0;
                    // Int32 readcount = 0;
                    ip = 0;

                    while (bytesImage < fileSize)
                    {
                        bytesImage += sender.Receive(RecvBytes, bytesImage, fileSize - bytesImage, SocketFlags.None);


                        ip++;
                        print("Received: " + bytesImage + "  " + ip);
                    }

                    imageData2 = RecvBytes;

                    print("Image Data length in client: " + imageData.Length);
                }




                //IMAGE FILE END//

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }

            // Manage of Socket's Exceptions
            catch (ArgumentNullException ane)
            {

                print(ane.ToString());
            }

            catch (SocketException se)
            {

                print(se.ToString());
            }

            catch (Exception e)
            {
                print(e.ToString());
            }
        }

        catch (Exception e)
        {

            print(e.ToString());
        }
    }
}