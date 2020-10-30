using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

namespace PotatoGame
{
    [RequireComponent(typeof(FullBodyBipedIK))] //this script relies on this componenet 
    public class IKController : MonoBehaviour
    {
        // public variables -------------------------
        protected FullBodyBipedIK m_ik; // Instance of the body ik on the player
        public Transform m_leftHandTarget; // Position where the left hand will be placed
        public Transform m_rightHandTarget; // Position where the right hand will be placed
        protected bool m_activateWeight; // Activate the weight for the hands (going to target)
        protected float m_weightSpeed = 5f; // Speed for the weight transition

        // private variables ------------------------
        private float _weightValue; // Value that will be applied to the weight of the ik
        private bool _deactivated; // After an activation, deactivate all 
        

        public bool ActivateWeight { get => m_activateWeight; set => m_activateWeight = value; }
        public Transform LeftHandTarget { get => m_leftHandTarget; set => m_leftHandTarget = value; }
        public Transform RightHandTarget { get => m_rightHandTarget; set => m_rightHandTarget = value; }


        protected void Awake()
        {
            m_ik = this.GetComponent<FullBodyBipedIK>();
        }
        
        protected void LateUpdate()
        {
            // When active, set the targets
            if (m_activateWeight)
                SetWeight();
            else if (!_deactivated)
                ClearTarget();
        }
        
        protected void SetTransform(IKEffector effector, Transform target)
        {
            effector.position = target.position;
            effector.rotation = target.rotation;
        }

        protected void SetWeightValue(IKEffector effector, float weight)
        {
            effector.positionWeight = weight;
            effector.rotationWeight = weight;
        }

        protected void SetWeight()
        {
            _deactivated = false;
            _weightValue = Mathf.Lerp(_weightValue, 1f, m_weightSpeed * Time.deltaTime);

            // Place the hand effectors to the target position
            SetTransform(m_ik.solver.leftHandEffector, m_leftHandTarget);
            SetTransform(m_ik.solver.rightHandEffector, m_rightHandTarget);

            // Set the weight of the effectors to 1 (active)
            SetWeightValue(m_ik.solver.leftHandEffector, _weightValue);
            SetWeightValue(m_ik.solver.rightHandEffector, _weightValue);
        }


        // Clear the target when no more activated -------------------------------------
        protected void ClearTarget()
        {
            // Do this once per clear
            if (_weightValue > 0f)
            {
                _weightValue -= m_weightSpeed * Time.deltaTime;

                // Set the weight of the effectors to 1 (active)
                SetWeightValue(m_ik.solver.leftHandEffector, _weightValue);
                SetWeightValue(m_ik.solver.rightHandEffector, _weightValue);
            }
            else if (!_deactivated)
            {
                _weightValue = 0f;
                _deactivated = true;
            }
        }

        protected void OnDrawGizmos()
        {
            if (m_leftHandTarget != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(m_leftHandTarget.position, 0.1f);
                Gizmos.DrawLine(m_leftHandTarget.position, m_leftHandTarget.position + m_leftHandTarget.forward * 0.5f);
            }

            if (m_rightHandTarget != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(m_rightHandTarget.position, 0.1f);
                Gizmos.DrawLine(m_rightHandTarget.position, m_rightHandTarget.position + m_rightHandTarget.forward * 0.5f);
            }

            
        }
    }
}
