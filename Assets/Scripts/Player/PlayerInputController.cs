using System;
using System.Collections;
using System.Collections.Generic;
using PotatoGame;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(PlayerController))]

public class PlayerInputController : MonoBehaviour
{
    // public variables -------------------------
    [ReadOnly] public float m_horizontalAxis = 0f;  // Horizontal axis captured value
    [ReadOnly] public float m_verticalAxis = 0f;  // Vertical axis captured value
    

    // private variables ------------------------
    private bool _inputsReady = true;  // Determine if inputs are ready to be triggered
    private ActionController _actionController;  // Instance of the player's action controller



    // ------------------------------------------
    // Start is called before update
    // ------------------------------------------
    void Start()
    {
        // Get the player's action controller
        if (GetComponentInChildren<ActionController>())
            _actionController = GetComponentInChildren<ActionController>();
        else
        {
            // Can't listen to input
            Debug.LogError("No player's action controller found. Action controller is required for player's inputs");
            _inputsReady = false;
        }
    }

    // ------------------------------------------
    // Update is called once per frame
    // ------------------------------------------
    void Update()
    {
        if (_inputsReady)
        {
            MovementInputs();
            ActionInputs();
        }
        
    }

    // ------------------------------------------
    // Methods
    // ------------------------------------------

    #region Inputs

    // Capture player's movement inputs ----------------------------------------
    private void MovementInputs()
    {
        // Get basic movement axis
        m_horizontalAxis = Input.GetAxis("Horizontal");
        m_verticalAxis = Input.GetAxis("Vertical");
    }
    
    
    // Capture player's Action inputs ------------------------------------------
    private void ActionInputs()
    {
        // If main action input is triggered
        if (Input.GetButtonDown("Action"))
        {
            if (_actionController.m_holding)
            {
                _actionController.Trow();
            }
            else
            {
                _actionController.PickUp();
            }
        }

        if (Input.GetButtonDown("Plant"))
        {
            if (_actionController.m_holding)
            {
                _actionController.Plant();
            }
        }
    }
    
    

    #endregion
    
}
