using System;
using System.Collections;
using System.Collections.Generic;
using PotatoGame;
using UnityEngine;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;

namespace PotatoGame
{
    public class HandTargetPosition : MonoBehaviour
    {
        // public variables -------------------------
        [ReadOnly] public FullBodyBipedIK m_ik; // Instance of the body ik on the player
        public Transform m_leftHandTarget; // Position where the left hand will be placed
        public Transform m_rightHandTarget; // Position where the right hand will be placed
        [Space(10)] public bool m_activateWeight; // Activate the weight for the hands (going to target)
        public float m_weightSpeed = 5f; // Speed for the weight transition

        // private variables ------------------------
        private float _mWeightValue; // Value that will be applied to the weight of the ik
        private bool _mDeactivated; // After an activation, deactivate all 
        
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
            // // When active, set the targets
            if (m_activateWeight)
                SetWeight();
            else if (!_mDeactivated)
                ClearTarget();
        }

        // ------------------------------------------
        // Methods
        // ------------------------------------------
        // Set and activate the weight -------------------------------------------------
        private void SetWeight()
        {
            _mDeactivated = false;
            _mWeightValue = Mathf.Lerp(_mWeightValue, 1f, m_weightSpeed * Time.deltaTime);

            // Place the hand effectors to the target position
            m_ik.solver.leftHandEffector.position = m_leftHandTarget.position;
            m_ik.solver.rightHandEffector.position = m_rightHandTarget.position;

            m_ik.solver.leftHandEffector.rotation = m_leftHandTarget.rotation;
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
            if (_mWeightValue > 0f)
            {
                _mWeightValue -= m_weightSpeed * Time.deltaTime;

                // Set the weight of the effectors to 1 (active)
                m_ik.solver.leftHandEffector.positionWeight = _mWeightValue;
                m_ik.solver.leftHandEffector.rotationWeight = _mWeightValue;
                m_ik.solver.rightHandEffector.positionWeight = _mWeightValue;
                m_ik.solver.rightHandEffector.rotationWeight = _mWeightValue;

                return;
            }
            else if (!_mDeactivated)
            {
                _mWeightValue = 0f;
                _mDeactivated = true;
                return;
            }
        }


        // Draw gizmos on play ---------------------------------------------------------
        private void OnDrawGizmos()
        {
            //CODRIN DRAWS POSITION OF LEFT AND RIGHT HAND 
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(m_leftHandTarget.position, 0.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(m_rightHandTarget.position, 0.1f);
        
            Gizmos.color = Color.red;
            Gizmos.DrawLine(m_leftHandTarget.position, m_leftHandTarget.position + m_leftHandTarget.forward * 0.5f);
            Gizmos.DrawLine(m_rightHandTarget.position, m_rightHandTarget.position + m_rightHandTarget.forward * 0.5f);
        }
    }
}
