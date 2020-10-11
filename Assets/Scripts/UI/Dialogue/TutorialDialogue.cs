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
    private float _mDelayTimer; // Instance of the timer for the delay
    private bool _mTutorialStarted = false;  // Check if the tutorial have started
    private DialogueManager m_dm;  // Instance of the dialogue manager
    private float _mWaitTimer;  // Timer use for wait time between tips


    // ------------------------------------------
    // Start is called before update
    // ------------------------------------------
    void Start()
    {
        // Get component
        m_dm = FindObjectOfType<DialogueManager>();
        
        // Set the timers
        _mDelayTimer = m_delay;
        _mWaitTimer = m_WaitForNextTip;

    }

    
    // ------------------------------------------
    // Update is called once per frame
    // ------------------------------------------
    void Update()
    {
        if (!_mTutorialStarted)
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
        _mDelayTimer -= Time.deltaTime;

        // Check if the delay is over
        if (_mDelayTimer <= 0f)
        {
            StartTutorial();
            _mTutorialStarted = true;
        }
    }
    
    
    // Start the tutorial via de dm --------------------------------------------
    private void StartTutorial()
    {
        // Start the tutorial dialogue on screen
        m_dm.StartDialogue(m_dialogue);
    }
    

    // Check input for this tutorial -------------------------------------------
    private void CheckInput()
    {
        // Display the next sentence of the tutorial
        if (Input.GetButton("Jump") && _mWaitTimer <= 0f)
        {
            // Reset wait time
            _mWaitTimer = m_WaitForNextTip;
            m_dm.DisplayNextSentence();
        }

        // Count down wait time
        _mWaitTimer -= Time.deltaTime;
    }
}
