using System;
using System.Collections;
using System.Collections.Generic;
using PotatoGame;
using UnityEngine;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;

public class HandTargetPosition : MonoBehaviour
{
    // public variables -------------------------
    [ReadOnly] public FullBodyBipedIK m_ik;         // Instance of the body ik on the player
    public Transform m_leftHandTarget;              // Position where the left hand will be placed
    public Transform m_rightHandTarget;             // Position where the right hand will be placed
    public bool m_activateWeight;                   // Activate the weight for the hands (going to target)


    // private variables ------------------------
    private float _mWeightValue;                    // Value that will be applied to the weight of the ik


    // ------------------------------------------
    // Start is called before update
    // ------------------------------------------
    void Start()
    {
        // Get Components
        m_ik = GameObject.FindGameObjectWithTag("Player").GetComponent<FullBodyBipedIK>();
    }

    // ------------------------------------------
    // Update is called once per frame
    // ------------------------------------------
    void LateUpdate()
    {
        // When active, set the targets
        if(m_activateWeight)
            SetWeight();
        else
            ClearTarget();
    }

    // ------------------------------------------
    // Methods
    // ------------------------------------------
    // Set and activate the weight -------------------------------------------------
    private void SetWeight()
    {
        _mWeightValue = 1f;
        
        // Place the hand effectors to the target position
        m_ik.solver.leftHandEffector.position = m_leftHandTarget.position;
        m_ik.solver.leftHandEffector.rotation = m_leftHandTarget.rotation;
        m_ik.solver.rightHandEffector.position = m_rightHandTarget.position;
        m_ik.solver.rightHandEffector.rotation = m_rightHandTarget.rotation;
        
        // Set the weight of the effectors to 1 (active)
        m_ik.solver.leftHandEffector.positionWeight = _mWeightValue;
        m_ik.solver.leftHandEffector.rotationWeight = _mWeightValue;
        m_ik.solver.rightHandEffector.positionWeight = _mWeightValue;
        m_ik.solver.rightHandEffector.rotationWeight = _mWeightValue;
    }

    
    // Clear the target when no more activated -------------------------------------
    private void ClearTarget()
    {
        // Do this once per clear
        if (_mWeightValue == 0f)
            return;

        _mWeightValue = 0f;

        // Set the weight of the effectors to 0 (inactive)
        m_ik.solver.leftHandEffector.positionWeight = _mWeightValue;
        m_ik.solver.leftHandEffector.rotationWeight = _mWeightValue;
        m_ik.solver.rightHandEffector.positionWeight = _mWeightValue;
        m_ik.solver.rightHandEffector.rotationWeight = _mWeightValue;
    }
}
