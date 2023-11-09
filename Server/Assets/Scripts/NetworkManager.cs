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
    public GameObject playerPrefab;
    public GameObject timer;

    [SerializeField] private Flipper flipper;
    [SerializeField] private Flipper spoon;
    [SerializeField] private SwingHammer hammer;

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
        
        Server.ClientConnected += ServerOnClientConnected;
    }

    private void ServerOnClientConnected(object sender, ServerConnectedEventArgs e) {
        // Instantiate or enable the player character
        GameObject player = Instantiate(playerPrefab);
        SimulateReset();
    }

    [MessageHandler((ushort)1)]
    private static void ServerOnMessageReceived(ushort fromClientID, Message message) 
    {
        var interaction = message.GetString();

        if (interaction.Equals("Thump called.")) 
        {
            // Simulate thump action
            Debug.Log("Thump called.");
            SimulateThump();
        } else if (interaction.Equals("Reset called.")) 
        {
            // Simulate reset action
            Debug.Log("Reset Button was pressed.");
            SimulateReset();
        } else if (interaction.Equals("Hammer Swing called."))
        {
            // Simulate Hammer Swing 
            Debug.Log("Hammer Swing called.");
            SimulateHammerSwing();
        }
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

    private static void SimulateThump() {
        if (Singleton.flipper != null)
        {
            Singleton.StartCoroutine(Singleton.ActivateFlipperTemporarily(Singleton.flipper, 1f));
        }
        else
        {
            Debug.LogError("Flipper reference not set in NetworkManager.");
        }

        if (Singleton.spoon != null)
        {
            Singleton.StartCoroutine(Singleton.ActivateFlipperTemporarily(Singleton.spoon, 1f));
        }
        else
        {
            Debug.LogError("Spoon reference not set in NetworkManager.");
        }
    }

    private IEnumerator ActivateFlipperTemporarily(Flipper objectToActivate, float time)
    {
        objectToActivate.Activate();

        // Wait for duration
        yield return new WaitForSeconds(time);

        objectToActivate.Deactivate();
    }

    private static void SimulateHammerSwing() {
        if (Singleton.hammer != null)
        {
            Singleton.StartCoroutine(Singleton.ActivateHammerTemporarily(Singleton.hammer, 1.5f));
        }
        else
        {
            Debug.LogError("Hammer reference not set in NetworkManager.");
        }
    }

    private IEnumerator ActivateHammerTemporarily(SwingHammer objectToActivate, float time)
    {
        objectToActivate.Activate();

        // Wait for duration
        yield return new WaitForSeconds(time);

        objectToActivate.Deactivate();
    }

    private static void SimulateReset() {
        if (Singleton.timer != null)
        {
            TimingRecording timingRecording = Singleton.timer.GetComponent<TimingRecording>();
            if (timingRecording != null)
            {
                timingRecording.Reset();
            }
            else
            {
                Debug.LogError("TimingRecording component not found on the timer GameObject.");
            }
        }
        else
        {
            Debug.LogError("Timer GameObject is not set in the NetworkManager.");
        }
    }
}