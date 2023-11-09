/**
 * UIManager.cs
 * 
 * Manages UI elements for network connection and user interactions within a Unity game.
 * Handles displaying connection UI, responding to connection events, and processing user inputs.
 *
 * Authors: Santi Rijal, Adam Sarty
 * Course: CSCI4126
 * Assignment: 3
 */

using System;
using Riptide;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour {
    private static UIManager _singleton;

    [SerializeField] private GameObject connectUI;
    [SerializeField] private GameObject controlsUI;
    [SerializeField] private TextMeshProUGUI error;
    [SerializeField] private TMP_InputField inputField;

    private string _userIP;

    public static UIManager Singleton {
        get => _singleton;
        private set {
            if (_singleton == null) {
                _singleton = value;
            }
            else if (_singleton != value) {
                Debug.Log($"{nameof(UIManager)} instance already exists, destroying object.");
                Destroy(value.gameObject);
            }
        }
    }

    private void Awake() {
        Singleton = this;
    }

    private void Start() {
        // Subscribe to network events
        SubscribeToNetworkEvents();
    }

    private void SubscribeToNetworkEvents() {
        if (NetworkManager.Singleton.Client != null) {
            NetworkManager.Singleton.Client.Disconnected += ClientOnDisconnected;
            NetworkManager.Singleton.Client.ConnectionFailed += ClientOnConnectionFailed;
            NetworkManager.Singleton.Client.Connected += ClientOnConnected;
        } else {
            Debug.LogError("NetworkManager.Singleton.Client is not initialized!");
        }
    }

    // Called when the client successfully connects
    private void ClientOnConnected(object sender, EventArgs e) {
        SwitchToControlsUI();
    }

    // Called when a connection attempt fails
    private void ClientOnConnectionFailed(object sender, ConnectionFailedEventArgs e) {
        error.text = "Connection failed";
    }

    // Called when the client gets disconnected
    private void ClientOnDisconnected(object sender, DisconnectedEventArgs e) {
        error.text = "Disconnected";
        BackToMain();
    }

    // Invoked when the connect button is clicked
    public void ClickedConnect() {
        AttemptConnection();
    }

    // Handles the attempt to connect to the server
    private void AttemptConnection() {
        _userIP = inputField.text;

        if (!string.IsNullOrEmpty(_userIP)) {
            NetworkManager.Singleton.Connect(_userIP);
            error.text = "Connecting...";
        } else {
            error.text = "Please enter a valid IP address.";
        }
    }

    // Returns the UI to the main connection screen
    public void BackToMain() {
        connectUI.SetActive(true);
        controlsUI.SetActive(false);
    }

    // Switches the UI to the control panel after connection
    private void SwitchToControlsUI() {
        connectUI.SetActive(false);
        controlsUI.SetActive(true);
    }

    // Retrieves the user-entered IP address
    public string GetUserIP() {
        return _userIP;
    }
}
