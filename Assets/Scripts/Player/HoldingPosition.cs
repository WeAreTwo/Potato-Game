using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class HoldingPosition : MonoBehaviour
{
    // public variables -------------------------
    public FullBodyBipedIK m_ik;                    // Instance of the body ik on the player
    public Transform m_leftHandTarget;              // Position where the left hand will be placed
    public Transform m_rightHandTarget;             // Position where the right hand will be placed


    // private variables ------------------------


    // ------------------------------------------
    // Start is called before update
    // ------------------------------------------
    void Start()
    {
        
    }

    // ------------------------------------------
    // Update is called once per frame
    // ------------------------------------------
    void LateUpdate()
    {
        // Place the hand effectors to the target position
        m_ik.solver.leftHandEffector.position = m_leftHandTarget.position;
        m_ik.solver.leftHandEffector.rotation = m_leftHandTarget.rotation;
        m_ik.solver.rightHandEffector.position = m_rightHandTarget.position;
        m_ik.solver.rightHandEffector.rotation = m_rightHandTarget.rotation;
        
        // Set the weight of the effectors to 1 (active)
        m_ik.solver.leftHandEffector.positionWeight = 1f;
        m_ik.solver.leftHandEffector.rotationWeight = 1f;
        m_ik.solver.rightHandEffector.positionWeight = 1f;
        m_ik.solver.rightHandEffector.rotationWeight = 1f;
    }

    // ------------------------------------------
    // Methods
    // ------------------------------------------
}
