using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace PotatoGame
{

    [RequireComponent(typeof(CharacterController))]    //basic controller for movement
    [RequireComponent(typeof(Animator))]                //for animation
    [RequireComponent(typeof(IKController))]            //to enabled use of IK
    public class MovementBase : MonoBehaviour
    {
        
        // public variables -------------------------
        [Title("Controls")]
        [SerializeField] protected float m_movementSpeed = 10f;             // Movement speed of the player
        [SerializeField] protected float m_rotationSpeed = 5f;              // Movement speed for rotation
        [Title("Physics")]
        [SerializeField] protected float m_groundOffset = 0.2f;             // Where does the ground stands in relation to the player
        [SerializeField] protected float m_gravityForce = -9.81f;           // Gravity that is applied
        [SerializeField] protected LayerMask m_ground;                      // Ground layer (physics)
        
        // private variables ------------------------
        protected CharacterController _mController;       // Instance of the character controller
        protected Animator _mAnim;                        // Instance of the animator linked to the player
        protected Transform _mGroundCheck;                // Instance of the ground check position
        protected Vector3 _mHeading = Vector3.zero;
        protected Vector3 _mVelocity;                     // Velocity to apply to the player
        protected bool _mIsGrounded;                      // Check if the controller is in contact with the ground
        protected Quaternion _mLookRotation;              // Rotation that need to be look at


        protected virtual void Awake()
        {
            _mController = this.GetComponent<CharacterController>();
            _mAnim = this.GetComponent<Animator>();
            _mGroundCheck = transform.GetChild(0).transform; //Todo eliminate this dependancy (maybe using a vector3 inside script?)
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }

        protected virtual void OnDrawGizmos()
        {
            if (_mGroundCheck)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(_mGroundCheck.position, m_groundOffset);
            }
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(this.transform.position, this.transform.position + _mHeading);
        }
    }

}
