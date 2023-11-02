using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;
using Riptide.Utils;

/*
 * -----------------------------------
 *  Created by:  Adam Sarty
 *  Student ID:   B00794681
 *  Assignment:   3
 *  Course Code:  CSCI4126
 * -----------------------------------
 */

// Enumeration of message IDs sent from server to client
public enum ServerToClientId : ushort
{
    SpawnPlayer = 1,
}

// Enumeration of message IDs sent from client to server
public enum ClientToServerId : ushort
{
    PlayerName = 1,
}

public class NetworkManager : MonoBehaviour
{
    // Singleton pattern for NetworkManager. Ensures only one instance exists.
    private static NetworkManager _singleton;
    public static NetworkManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }

    // Serialized fields for Unity's Inspector
    [SerializeField] private ushort port;               // Port number the server listens to
    [SerializeField] private ushort maxClientCount;     // Maximum number of clients server can handle

    // Property to get the Server instance
    public Server Server { get; private set; }

    // Set Singleton instance on Awake (before Start method)
    private void Awake()
    {
        Singleton = this;
    }

    // Initialization method called on Start
    private void Start()
    {
        // Disable vertical sync and set target frame rate to 30fps
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        // Initialize Riptide Logger with various Debug log methods
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        Console.Title = "Server";
        Console.Clear();
        Application.SetStackTraceLogType(UnityEngine.LogType.Log, StackTraceLogType.None);
        RiptideLogger.Initialize(Debug.Log, true);

        // Instantiate and start the server
        Server = new Server();
        Server.Start(port, maxClientCount);
    }

    // Update server logic on FixedUpdate
    private void FixedUpdate()
    {
        Server.Update();
    }

    // Cleanup when the application quits
    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    // Message handler for type of interaction message from client
    [MessageHandler((ushort)1)]
    private static void HandleTypeMessageFromServer(ushort fromClientID, Message message)
    {
        // Extracting message content
        int type = message.GetInt();
    }
}