using System;
using Riptide;
using Riptide.Utils;
using UnityEngine;

/**
 * A enum client to server id.
 */
public enum ClientToServerId : ushort {
    name = 1,
}

/**
 * Class to manage the connection on the client side.
 */
public class NetworkManager : MonoBehaviour {

    private static NetworkManager _singleton;   // Singleton of this class.

    // Create a new singleton if it already doesn't exists else destroy it.
    public static NetworkManager Singleton {
        get => _singleton;

        private set {
            if (_singleton == null) {
                _singleton = value;
            }
            else if (_singleton != value) {
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying duplicate.");
                Destroy(value);
            }
        }
    }

    public Client Client { get; private set; }  // Client get and set methods.

    [SerializeField] private ushort port;   // Port to run on.

    // Set singleton.
    private void Awake() {
        _singleton = this;
    }

    private void Start() {
        // Initialize riptide logs.
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Client = new Client();  // Create a new client.

        Client.ConnectionFailed += FailedToConnect; // Subscribe to connect to failed event.
    }

    private void FixedUpdate() {
        Client.Update();    // Call the client's update method at a fixed update.
    }

    // Method to disconnect client when application quits.
    private void OnApplicationQuit() {
        Client.Disconnect();
    }

    // Method to connect client.
    public void Connect() {
        var ip = UIManager.Singleton.GetUserIP(); // Get user ip from UI.
        
        Client.Connect($"{ip}:{port}"); // Call the connect method of client with the ip and port.
    }

    // A subscription method for failed to connect event.
    private void FailedToConnect(object sender, EventArgs e) {
        // Send failed to connect message to server.
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.name);
        message.Add("Failed to connect");
        Client.Send(message);
        
        UIManager.Singleton.BackToMain();   // Display the connect screen.
    }
    
}
