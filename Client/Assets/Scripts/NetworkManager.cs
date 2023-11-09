/**
 * NetworkManager.cs
 * 
 * Handles the client-side network connection in a Unity game, managing the client instance and its events.
 *
 * Authors: Santi Rijal, Adam Sarty
 * Course: CSCI4126
 * Assignment: 3
 */

using System;
using Riptide;
using Riptide.Utils;
using UnityEngine;

public class NetworkManager : MonoBehaviour {
    private static NetworkManager _singleton;
    public Client Client { get; private set; }

    [SerializeField] private ushort port;

    public static NetworkManager Singleton {
        get => _singleton;
        private set {
            if (_singleton == null) {
                _singleton = value;
            } else if (_singleton != value) {
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying duplicate.");
                Destroy(value.gameObject);
            }
        }
    }

    private void Awake() {
        Singleton = this;
        InitializeClient();
    }

    private void InitializeClient() {
        Client = new Client();
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
    }

    private void Start() {
        // Subscribe to client events
        Client.ConnectionFailed += FailedToConnect;
    }

    private void FixedUpdate() {
        // Update the client
        Client.Update();
    }

    private void OnApplicationQuit() {
        // Ensure the client disconnects on application quit
        Client.Disconnect();
    }

    // Initiates a connection to the server with the provided IP
    public void Connect(string ip) {
        Client.Connect($"{ip}:{port}");
    }

    // Callback for when the connection attempt fails
    private void FailedToConnect(object sender, EventArgs e) {
        NotifyConnectionFailure();
    }

    // Notifies the UI manager of connection failure and logs the event
    private void NotifyConnectionFailure() {
        SendConnectionFailureMessage();
        UIManager.Singleton.BackToMain();
    }

    // Sends a connection failure message to the server
    private void SendConnectionFailureMessage() {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.player);
        message.Add("Failed to connect");
        Client.Send(message);
    }
}

public enum ClientToServerId : ushort {
    player = 1,
}
