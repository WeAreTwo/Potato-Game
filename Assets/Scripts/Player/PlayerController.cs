using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace PotatoGame
{
    [RequireComponent(typeof(CharacterController))]    //basic controller for movement
    [RequireComponent(typeof(Animator))]                //for animation
    [RequireComponent(typeof(IKController))]            //to enabled use of IK
    [RequireComponent(typeof(NavMeshObstacle))]            //the player is an obstacle
    public class PlayerController : MovementBase
    {

        protected override void Update()
        {
            base.Update();
            // Make the player able to move
        }
        
        // Check user's input ------------------------------------------------------
        protected override void CheckInput()
        {
            if (_mIsGrounded && _mVelocity.y < 0)
                _mVelocity.y = 0f;

            // Step between each movement
            _movementStep = m_movementSpeed * Time.deltaTime;
            
            // Take player's inputs
            _mHeading.x = Input.GetAxis("Horizontal");
            _mHeading.z = Input.GetAxis("Vertical");

            // Catch the inputs in a vector3
            // (make sure inputs makes sense with camera view)
            _movementDirection = _mHeading;
            _movementDirection = Camera.main.transform.TransformDirection(_movementDirection);
            _movementDirection.y = 0f;
        }
    }
}
