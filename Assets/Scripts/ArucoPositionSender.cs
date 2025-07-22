using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class ArucoPositionSender : MonoBehaviour
{
    private string pcIpAddress = "10.10.10.187"; // Votre IP
    private int port = 11000;
    private UdpClient client;
    private IPEndPoint endPoint;

    void Start()
    {
        // !! LIGNE AJOUTÉE !! : Ne fait rien si on est dans l'éditeur
        if (Application.isEditor) return; 

        client = new UdpClient();
        endPoint = new IPEndPoint(IPAddress.Parse(pcIpAddress), port);
    }

    void Update()
    {
        // !! LIGNE AJOUTÉE !! : Ne fait rien si on est dans l'éditeur
        if (Application.isEditor) return;

        try
        {
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;
            string message = $"{pos.x},{pos.y},{pos.z}|{rot.x},{rot.y},{rot.z},{rot.w}";
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, endPoint);
        }
        catch (System.Exception err) { }
    }
}