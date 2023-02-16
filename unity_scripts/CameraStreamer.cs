using System;
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Networking;

public class CameraStreamer : MonoBehaviour
{
    private Camera mainCamera;
    private string boundary = "AaB03x";
    private string contentType = "multipart/x-mixed-replace; boundary=" + "AaB03x";
    private bool isStreaming = false;

    void Start()
    {
        mainCamera = Camera.main;

        isStreaming = true;

        StartCoroutine(StreamCamera());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && isStreaming)
        {
            Debug.Log("Stream stopped");
            OnDisable();
        } 
        else if (Input.GetKeyDown(KeyCode.R) && isStreaming == false)
        {
            Debug.Log("Stream resumed");
            OnResume();
        }
    }

    IEnumerator StreamCamera()
    {
        while (isStreaming)
        {
            yield return new WaitForEndOfFrame();

            Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            texture.Apply();

            byte[] bytes = texture.EncodeToJPG(50);

            WWWForm form = new WWWForm();
            form.AddField("Content-Type", contentType);
            form.AddField("msg", "Hello Python");
            form.AddBinaryData("image", bytes, "image.jpg", "image/jpeg");

            using (UnityWebRequest www = UnityWebRequest.Post("http://localhost:8080/stream", form))
            {

                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Debug.Log("Server Response: " + www.downloadHandler.text);
                }

                yield return null;
            }
        }
    }

    void OnDisable()
    {
        isStreaming = false;
    }
    
    void OnResume()
    {
        isStreaming = true;
        StartCoroutine(StreamCamera());
    }
}