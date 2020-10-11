using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class DialogueManager : MonoBehaviour
{
    // public variables -------------------------
    public TextMeshProUGUI m_nameText;  // Display text on UI for the speaker name
    public TextMeshProUGUI m_dialogueText;  // Display text on UI for the dialogue text
    public Animator m_animator;  // Dialogue animator to trigger open and close animation
    
    // private variables ------------------------
    private Queue<string> _mSentences;  // Holds the whole dialogue in a queue


    // ------------------------------------------
    // Start is called before update
    // ------------------------------------------
    void Start()
    {
        // Assigned an instance of queue of type string
        _mSentences = new Queue<string>();
        
    }

    // ------------------------------------------
    // Update is called once per frame
    // ------------------------------------------
    void Update()
    {
        
    }

    // ------------------------------------------
    // Methods
    // ------------------------------------------
    // Start a dialogue from a given set of sentences --------------------------
    public void StartDialogue(Dialogue dialogue)
    {
        // Open animation and display name on UI
        m_animator.SetBool("IsOpen", true);
        m_nameText.text = dialogue.name;
        
        // Clear past dialogues
        _mSentences.Clear();

        // Enqueue each sentence from the new dialogue
        foreach (var sentence in dialogue.sentences)
        {
            _mSentences.Enqueue(sentence);
        }
        
        // Display the first sentence
        DisplayNextSentence();
    }

    
    // Display the next sentence of the dialogue -------------------------------
    public void DisplayNextSentence()
    {
        // Make sure the dialogue is not over
        if (_mSentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        // Print the next sentence on UI
        var sentence = _mSentences.Dequeue();
        m_dialogueText.text = sentence;
    }

    
    // End the current dialogue ------------------------------------------------
    public void EndDialogue()
    {
        // Close the dialogue 
        m_animator.SetBool("IsOpen", false);
    }
}
