using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TimingRecording : MonoBehaviour
{
    public KeyCode resetKeyCode = KeyCode.R;
    public Rigidbody startingMarble;
    public SceneCompletion sceneCompletion;
    public TextMeshProUGUI textMesh;
    public Action enableControlAction;
    [HideInInspector]
    public float timer;

    bool m_IsTiming;
    BaseInteractivePuzzlePiece[] m_PuzzlePieces;

    void OnEnable()
    {
        m_PuzzlePieces = FindObjectsOfType<BaseInteractivePuzzlePiece>();
        
        enableControlAction = EnableControl;
    }

    void Start()
    {
        // Delay the execution to ensure the player has been instantiated
        StartCoroutine(WaitAndInitialize());
    }

    IEnumerator WaitAndInitialize()
    {
        // Wait for a frame to ensure all Start methods have run
        yield return null;

        // Access NetworkManager and get the player instance
        GameObject marbleInstance = NetworkManager.Singleton.GetCurrentPlayerInstance();
        if (marbleInstance != null)
        {
            startingMarble = marbleInstance.GetComponent<Rigidbody>();
            startingMarble.isKinematic = false;
        }
        else
        {
            Debug.LogError("Marble prefab not found in the scene.");
        }
    }

    void EnableControl()
    {
        // The rest of your control logic
        m_IsTiming = true;
        foreach (var piece in m_PuzzlePieces)
        {
            piece.EnableControl();
        }
    }

    void Update()
    {
        if (m_IsTiming)
            timer += Time.deltaTime;

        textMesh.text = timer.ToString("0.00");
        
        if (Input.GetKeyDown(resetKeyCode))
            sceneCompletion.ReloadLevel();
    }

    public void GoalReached(float uiDelay)
    {
        m_IsTiming = false;
        StartCoroutine(CompleteLevelWithDelay(uiDelay));
    }

    public void Reset()
    {
        sceneCompletion.ReloadLevel();
    }

    IEnumerator CompleteLevelWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        sceneCompletion.CompleteLevel(timer);
    }
}
