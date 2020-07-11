using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace PotatoGame
{
    public class AIController : MovementBase
    {

        protected override void Update()
        {
            base.Update();
            // Make the player able to move
            CheckInput();
        }
        
        // Check user's input ------------------------------------------------------
        private void CheckInput()
        {
            // Check if the player is grounded
            _mIsGrounded = Physics.CheckSphere(_mGroundCheck.position, m_groundOffset, m_ground, QueryTriggerInteraction.Ignore);

            if (_mIsGrounded && _mVelocity.y < 0)
                _mVelocity.y = 0f;

            // Step between each movement
            var step = m_movementSpeed * Time.deltaTime;
            
            // Catch the inputs in a vector3
            // (make sure inputs makes sense with camera view)
            var move = _mHeading;
            move = Camera.main.transform.TransformDirection(move);
            move.y = 0f;

            // When we record input, move the controller
            if (move != Vector3.zero)
            {
                move = Vector3.ClampMagnitude(move, 1);
                _mController.Move(move * step);
                _mLookRotation = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Lerp(transform.rotation, _mLookRotation, m_rotationSpeed * Time.deltaTime);
                
                // Update the animator
                _mAnim.SetBool("walking", true);
            }
            else
            {
                // Return to iddle state
                _mAnim.SetBool("walking", false);
            }
            
            // Add gravity
            _mVelocity.y += m_gravityForce * Time.deltaTime;
            _mController.Move(_mVelocity * Time.deltaTime);
        }
        
    }
}
