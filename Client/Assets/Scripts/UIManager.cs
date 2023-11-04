using System;
using Riptide;
using TMPro;
using UnityEngine;

/**
 * Class to handle connect button click and player spawn.
 */
public class UIManager : MonoBehaviour {
    
    private static UIManager _singleton;    // Singleton of this class.
    
    [SerializeField] private GameObject connectUI;  // Connection UI.
    [SerializeField] private GameObject controlsUI;  // Controls UI.
    [SerializeField] private TextMeshProUGUI error;  // Error text.
    [SerializeField] private TMP_InputField inputField; // Input field for IP address.

    private string _userIP;
    
    // Create a new singleton if it already doesn't exists else destroy it.
    public static UIManager Singleton {
        get => _singleton;

        private set {
            if (_singleton == null) {
                _singleton = value;
            }
            else if (_singleton != value) {
                Debug.Log($"{nameof(UIManager)} instance already exists, destroying duplicate.");
                Destroy(value);
            }
        }
    }
    
    // Set the singleton.
    private void Awake() {
        _singleton = this;
    }

    private void Start() {
        NetworkManager.Singleton.Client.Disconnected += ClientOnDisconnected;
        NetworkManager.Singleton.Client.ConnectionFailed += ClientOnConnectionFailed;
        NetworkManager.Singleton.Client.Connected += ClientOnConnected;
    }

    // A subscription method for connected event.
    private void ClientOnConnected(object sender, EventArgs e) {
        
        // Send a message indicating the users IP to the server.
        // Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.name);
        // message.Add(iP);
        // NetworkManager.Singleton.Client.Send(message);
        
        connectUI.SetActive(false); // Hide connect UI screen.
        controlsUI.SetActive(true); // Show controls UI screen.
    }

    // A subscription method for failed to connect event.
    private void ClientOnConnectionFailed(object sender, ConnectionFailedEventArgs e) {
        error.text = "Connection failed";
    }

    // A subscription method for disconnect event.
    private void ClientOnDisconnected(object sender, DisconnectedEventArgs e) {
        error.text = "Disconnected";
        BackToMain();
    }

    // Method to handle connect button click.
    public void ClickedConnect() {
        _userIP = inputField.text;
        
        NetworkManager.Singleton.Connect(); // Try to connect to server.
        error.text = "Connecting....";
    }
    
    // If failed to connect or disconnected, show the connect screen UI.
    public void BackToMain() {
        connectUI.SetActive(true);  // Show connect screen.
        controlsUI.SetActive(false); // hide controls UI screen.
    }

    // Get method for user IP.
    public string GetUserIP() {
        return _userIP;
    }
}
