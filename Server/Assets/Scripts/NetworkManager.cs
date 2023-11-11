using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
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

public class NetworkManager : MonoBehaviour 
{
    public GameObject playerPrefab; // Prefab for the player object
    public Transform spawnPoint; // Spawn point for player instantiation

    // Serialized fields for various game components
    [SerializeField] private TimingRecording timer;
    [SerializeField] private Flipper flipper;
    [SerializeField] private Flipper spoon;
    [SerializeField] private SwingHammer hammer;

    // Singleton instance for managing network operations
    private static NetworkManager _singleton;
    public static NetworkManager Singleton {
        get => _singleton;
        private set {
            if (_singleton == null) 
            {
                _singleton = value;
            }
            else if (_singleton != value) 
            {
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }

    // Network configuration settings
    [SerializeField] private ushort port;             
    [SerializeField] private ushort maxClientCount;   
    public Server Server { get; private set; }        

    // Reference to the current player instance
    private GameObject currentPlayerInstance;         

    // Reference to the Cinemachine camera target group
    [SerializeField] private CinemachineTargetGroup cinemachineTargetGroup;

    private void Awake() 
    {
        // Singleton pattern with DontDestroyOnLoad to persist across scene changes
        if (_singleton == null) 
        {
            _singleton = this;
            DontDestroyOnLoad(gameObject);
            DeactivateHammerSwing();
        }
        else if (_singleton != this) 
        {
            Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying object!");
            Destroy(gameObject);
        }
    }

    private void Start() 
    {
        // Initial setup for the server and networking
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        Console.Title = "Server";
        Console.Clear();
        Application.SetStackTraceLogType(UnityEngine.LogType.Log, StackTraceLogType.None);
        RiptideLogger.Initialize(Debug.Log, true);

        Server = new Server();
        Server.Start(port, maxClientCount);
        Server.ClientConnected += ServerOnClientConnected;
    }

    // Called when a client is connected to the server
    public void ServerOnClientConnected(object sender, ServerConnectedEventArgs e) 
    {
        CreatePlayerInstance();
    }

    // Handles different network messages received from clients
    [MessageHandler((ushort)1)]
    private static void ServerOnMessageReceived(ushort fromClientID, Message message) 
    {
        var interaction = message.GetString();

        if (interaction.Equals("Thump called.")) 
        {
            Debug.Log("Thump called.");
            SimulateThump();
        }
        else if (interaction.Equals("Hammer Swing called.")) 
        {
            Debug.Log("Hammer Swing called.");
            ActivateHammerSwing();
        }
    }

    private void FixedUpdate() 
    {
        // Regular update call for server operations
        Server.Update();
    }

    private void OnApplicationQuit() 
    {
        // Stops the server when the application is quitting
        Server.Stop();
    }

    // Simulates the 'Thump' action in the game
    private static void SimulateThump() 
    {
        if (Singleton.flipper != null) 
        {
            Singleton.StartCoroutine(Singleton.ActivateFlipperTemporarily(Singleton.flipper, 1f));
        } else {
            Debug.LogError("Flipper reference not set in NetworkManager.");
        }

        if (Singleton.spoon != null) 
        {
            Singleton.StartCoroutine(Singleton.ActivateFlipperTemporarily(Singleton.spoon, 1f));
        } else {
            Debug.LogError("Spoon reference not set in NetworkManager.");
        }
    }

    // Activates a flipper for a temporary duration
    private IEnumerator ActivateFlipperTemporarily(Flipper objectToActivate, float time) 
    {
        objectToActivate.Activate();
        yield return new WaitForSeconds(time);
        objectToActivate.Deactivate();
    }

    // Activates the hammer swing action
    private static void ActivateHammerSwing() 
    {
        if (Singleton.hammer != null) 
        {
            ActivateHammer(Singleton.hammer);
        } else 
        {
            Debug.LogError("Hammer reference not set in NetworkManager.");
        }
    }

    // Deactivates the hammer swing action
    private void DeactivateHammerSwing() 
    {
        if (Singleton.hammer != null) 
        {
            DeactivateHammer(Singleton.hammer);
        } else 
        {
            Debug.LogError("Hammer reference not set in NetworkManager.");
        }
    }

    // Activates the specified hammer
    private static void ActivateHammer(SwingHammer objectToActivate) 
    {
        objectToActivate.Activate();
    }
    
    // Deactivates the specified hammer
    private void DeactivateHammer(SwingHammer objectToActivate) 
    {
        objectToActivate.Deactivate();
    }

    // Creates a new player instance in the game
    public void CreatePlayerInstance() 
    {
        CreatePlayer(this);
    }

    // Public method to get the current player instance
    public GameObject GetCurrentPlayerInstance() 
    {
        return currentPlayerInstance;
    }

    // Static method for creating a player instance
    public static void CreatePlayer(NetworkManager instance) 
    {
        if (instance.currentPlayerInstance != null) 
        {
            instance.RemoveFromCinemachineGroup(instance.currentPlayerInstance);
            Destroy(instance.currentPlayerInstance);
        }
        instance.currentPlayerInstance = Instantiate(instance.playerPrefab, instance.spawnPoint.position, instance.spawnPoint.rotation);
        instance.currentPlayerInstance.tag = "Player";

        instance.AddToCinemachineGroup(instance.currentPlayerInstance, 1, 2); // Adjust weight and radius as needed
    }


    // Adds a new target to the Cinemachine camera group
    public void AddToCinemachineGroup(GameObject newTarget, float weight, float radius) 
    {
        if (cinemachineTargetGroup != null && newTarget != null)
        {
            CinemachineTargetGroup.Target target = new CinemachineTargetGroup.Target
            {
                target = newTarget.transform,
                weight = weight,
                radius = radius
            };

            List<CinemachineTargetGroup.Target> targetsList = new List<CinemachineTargetGroup.Target>(cinemachineTargetGroup.m_Targets);
            targetsList.Add(target);
            cinemachineTargetGroup.m_Targets = targetsList.ToArray();
        }
        else
        {
            Debug.LogError("CinemachineTargetGroup or target is null.");
        }
    }

    // Removes a target from the Cinemachine camera group
    public void RemoveFromCinemachineGroup(GameObject targetToRemove) 
    {
        if (cinemachineTargetGroup != null && targetToRemove != null)
        {
            List<CinemachineTargetGroup.Target> targetsList = new List<CinemachineTargetGroup.Target>(cinemachineTargetGroup.m_Targets);
            targetsList.RemoveAll(target => target.target == targetToRemove.transform);
            cinemachineTargetGroup.m_Targets = targetsList.ToArray();
        }
        else
        {
            Debug.LogError("CinemachineTargetGroup or target is null.");
        }
    }
}
