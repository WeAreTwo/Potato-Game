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
            NavMeshMovement();
            // Make the player able to move
        }
        
        // Check user's input ------------------------------------------------------
        protected override void CheckInput()
        {
            if (isGrounded && movementVelocity.y < 0)
                movementVelocity.y = 0f;

            // Step between each movement
            movementStep = movementSpeed * Time.deltaTime;
            
            // Take player's inputs
            heading.x = Input.GetAxis("Horizontal");
            heading.z = Input.GetAxis("Vertical");

            // Catch the inputs in a vector3
            // (make sure inputs makes sense with camera view)
            movementDirection = heading;
            movementDirection = Camera.main.transform.TransformDirection(movementDirection);
            movementDirection.y = 0f;
        }
        
        protected void NavMeshMovement()
        {
            //refL https://www.youtube.com/watch?v=bH33Qvhvl40 Ciro from unity
            float inputMagnitude = movementDirection.sqrMagnitude;
            if (inputMagnitude >= .01f)
            {
                Vector3 newPosition = transform.position + movementDirection * (movementSpeed * Time.deltaTime);
                NavMeshHit hit;
                bool isValid = NavMesh.SamplePosition(newPosition, out hit, .3f, NavMesh.AllAreas);
                if (isValid)
                {
                    if ((transform.position - hit.position).magnitude >= .02f)
                    {
                        transform.position = hit.position;
                    }
                    else
                    {
                        //movement stopped this frame
                    }
                }
                else
                {
                    //no input from player
                }
            }
        }

    }
}
