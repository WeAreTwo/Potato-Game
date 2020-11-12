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
    [Title("Movement Inputs")]
    [Range(-1, 1)] public float m_horizontalAxis = 0f;  // Horizontal axis captured value
    [Range(-1, 1)] public float m_verticalAxis = 0f;  // Vertical axis captured value

    [HorizontalGroup("Split",0.5f)] [Button("Action",(ButtonSizes.Large))]
    private void ActionBtn() { this._actionBtn = !this._actionBtn; }
    [HorizontalGroup("Split",0.5f)] [Button("Plant",(ButtonSizes.Large))]
    private void PlantBtn() { this._actionBtn = !this._actionBtn; }
    
    

    // private variables ------------------------
    private bool _inputsReady = true;  // Determine if inputs are ready to be triggered
    private ActionController _actionController;  // Instance of the player's action controller
    private bool _actionBtn;  // Use for action button in the inspector
    private bool _plantBtn;  // Use for plant button in the inspector



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
        if (!_inputsReady) 
            return;
        
        MovementInputs();
        ActionInputs();
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
        if (Input.GetButtonDown("Action") || _actionBtn)
        {
            // For inspector
            _actionBtn = false;
            
            // Determine which action to do depending on hold state
            if (_actionController.m_holding)
                _actionController.Trow();
            else
                _actionController.PickUp();
        }

        if (Input.GetButtonDown("Plant") || _plantBtn)
        {
            // For inspector
            _plantBtn = false;
            
            // Make sure the action is in hold state
            if (_actionController.m_holding)
                _actionController.Plant();
        }
    }

    #endregion



}
