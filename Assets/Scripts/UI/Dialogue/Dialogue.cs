using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue 
{
    // public variables -------------------------
    public string name;  // Name of the speaker
    [TextArea(3, 10)]
    public string[] sentences;  // Sentences part of this dialogue
}
