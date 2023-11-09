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

public class NetworkManager : MonoBehaviour {
    
    public GameObject playerPrefab;
    public GameObject timer;
    public Transform spawnPoint;

    [SerializeField] private Flipper flipper;
    [SerializeField] private Flipper spoon;
    [SerializeField] private SwingHammer hammer;

    // Singleton pattern for NetworkManager. Ensures only one instance exists.
    private static NetworkManager _singleton;
    public static NetworkManager Singleton {
        get => _singleton;
        
        private set {
            if (_singleton == null) {
                _singleton = value;
            }
            else if (_singleton != value) {
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
    private void Awake() {
        Singleton = this;
        DeactivateHammerSwing();
    }

    // Initialization method called on Start
    private void Start() {
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
        Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
    }
    
    [MessageHandler((ushort)1)]
    private static void ServerOnMessageReceived(ushort fromClientID, Message message) {
        var interaction = message.GetString();

        if (interaction.Equals("Thump called.")) {
            // Simulate thump action
            Debug.Log("Thump called.");
            SimulateThump();
        } 
        else if (interaction.Equals("Reset called.")) {
            // Simulate reset action
            Debug.Log("Reset Button was pressed.");
            SimulateReset();
        } 
        else if (interaction.Equals("Hammer Swing called.")) {
            // Simulate Hammer Swing 
            Debug.Log("Hammer Swing called.");
            ActivateHammerSwing();
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
    
    // A static method that calls a method to activate the hammer if the hammer ref isn't null.
    private static void ActivateHammerSwing() {
        if (Singleton.hammer != null) {
            ActivateHammer(Singleton.hammer);
        }
        else {
            Debug.LogError("Hammer reference not set in NetworkManager.");
        }
    }

    // A method that calls a method to deactivate the hammer if the hammer ref isn't null.
    private void DeactivateHammerSwing() {
        if (Singleton.hammer != null) {
            DeactivateHammer(Singleton.hammer);
        }
        else {
            Debug.LogError("Hammer reference not set in NetworkManager.");
        }
    }

    // A static method to activate the hammer.
    private static void ActivateHammer(SwingHammer objectToActivate) {
        objectToActivate.Activate();
    }
    
    // A method to deactivate the hammer.
    private void DeactivateHammer(SwingHammer objectToActivate) {
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