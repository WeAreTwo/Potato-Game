using UnityEngine;
using System.Collections;
using UnityEditor;


namespace PotatoGame
{

    [CustomEditor(typeof(AIPotatoFSM))]
    public class PotatoAIEditor : Editor
    {
        void OnSceneGUI()
        {
            AIPotatoFSM aiPotato = (AIPotatoFSM) target;

            // if (potato.Fsm.CurrentStateKey != null)
            // {
                // Handles.color = Color.red;
                // Handles.Label(potato.transform.position + Vector3.up * 2,
                //     potato.transform.position.ToString() + "\nState: " +
                //     potato.Fsm.CurrentStateKey.ToString());
            // }
        }
    }

}