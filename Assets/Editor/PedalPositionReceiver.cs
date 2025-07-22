using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

// Fait en sorte que le script s'exécute aussi en mode éditeur (pas seulement en Play Mode)
[ExecuteInEditMode]
public class PedalPositionReceiver : MonoBehaviour
{
    private UdpClient listener;
    private Thread listenerThread;
    private int port = 11000;

    // Volatile pour être sûr que le thread y accède correctement
    private volatile bool dataReceived = false;
    private volatile string receivedData;

    void OnEnable()
    {
        // Ne lance le récepteur que si on est dans l'éditeur
        if (!Application.isEditor) return;

        try
        {
            listener = new UdpClient(port);
            listenerThread = new Thread(new ThreadStart(ListenForData));
            listenerThread.IsBackground = true;
            listenerThread.Start();
            Debug.Log("Récepteur démarré sur le port " + port);
        }
        catch (Exception e)
        {
            Debug.LogError("Impossible de démarrer le récepteur : " + e.Message);
        }
    }

    void Update()
    {
        // Ne s'applique que dans l'éditeur
        if (!Application.isEditor) return;

        // Si de nouvelles données ont été reçues par le thread
        if (dataReceived)
        {
            // Copie les données pour éviter les problèmes de thread
            string data = receivedData;
            dataReceived = false; // On réinitialise

            // Sépare les données de position et de rotation
            string[] parts = data.Split('|');
            string[] posParts = parts[0].Split(',');
            string[] rotParts = parts[1].Split(',');

            // Parse et applique la position et la rotation
            Vector3 pos = new Vector3(float.Parse(posParts[0]), float.Parse(posParts[1]), float.Parse(posParts[2]));
            Quaternion rot = new Quaternion(float.Parse(rotParts[0]), float.Parse(rotParts[1]), float.Parse(rotParts[2]), float.Parse(rotParts[3]));

            transform.position = pos;
            transform.rotation = rot;
        }
    }

    private void ListenForData()
    {
        IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, port);
        try
        {
            while (true)
            {
                byte[] data = listener.Receive(ref groupEP);
                receivedData = Encoding.UTF8.GetString(data);
                dataReceived = true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    // S'assure que le thread et le client sont bien fermés
    void OnDisable()
    {
        if (listener != null)
        {
            listener.Close();
            listener = null;
        }
        if (listenerThread != null && listenerThread.IsAlive)
        {
            listenerThread.Abort();
        }
    }
}