using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using UnityEngine;
using Sirenix.OdinInspector;

public class TutorialDialogue : MonoBehaviour
{
    // public variables -------------------------
    [Title("Dialogue")]
    public float m_delay;  // Delay (seconds) before tutorial dialogue is prompt
    public float m_WaitForNextTip;  // Time to wait before accessing next tip
    public Dialogue m_dialogue;  // The dialogue linked to this tutorial
    
    // private variables ------------------------
    private float _delayTimer; // Instance of the timer for the delay
    private bool _tutorialStarted = false;  // Check if the tutorial have started
    private DialogueManager _dm;  // Instance of the dialogue manager
    private float _waitTimer;  // Timer use for wait time between tips


    // ------------------------------------------
    // Start is called before update
    // ------------------------------------------
    void Start()
    {
        // Get component
        _dm = FindObjectOfType<DialogueManager>();
        
        // Set the timers
        _delayTimer = m_delay;
        _waitTimer = m_WaitForNextTip;

    }

    
    // ------------------------------------------
    // Update is called once per frame
    // ------------------------------------------
    void Update()
    {
        if (!_tutorialStarted)
            DelayCountDown();
        else
            CheckInput();
    }

    
    // ------------------------------------------
    // Methods
    // ------------------------------------------
    // Count down delay before starting tutorial -------------------------------
    private void DelayCountDown()
    {
        // Count down
        _delayTimer -= Time.deltaTime;

        // Check if the delay is over
        if (_delayTimer <= 0f)
        {
            StartTutorial();
            _tutorialStarted = true;
        }
    }
    
    
    // Start the tutorial via de dm --------------------------------------------
    private void StartTutorial()
    {
        // Start the tutorial dialogue on screen
        _dm.StartDialogue(m_dialogue);
    }
    

    // Check input for this tutorial -------------------------------------------
    private void CheckInput()
    {
        // Display the next sentence of the tutorial
        if (Input.GetButton("Jump") && _waitTimer <= 0f)
        {
            // Reset wait time
            _waitTimer = m_WaitForNextTip;
            _dm.DisplayNextSentence();
        }

        // Count down wait time
        _waitTimer -= Time.deltaTime;
    }
}
