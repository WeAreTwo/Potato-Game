using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace PotatoGame
{
    public class AIController : MovementBase
    {
        protected Vector3 mouseTest = Vector3.zero;

        protected override void Update()
        {
            base.Update();
            // CheckInput();    
            MoveToMousePosition();
        }
        
        // Check user's input ------------------------------------------------------
        protected override void CheckInput()
        {
            if (_mIsGrounded && _mVelocity.y < 0)
                _mVelocity.y = 0f;

            // Step between each movement
            _movementStep = m_movementSpeed * Time.deltaTime;
            
            // Catch the inputs in a vector3
            // (make sure inputs makes sense with camera view)
            float distToDest = Vector3.Distance(mouseTest, this.transform.position);
            if (distToDest < 1.0f)
            {
                _mHeading = Vector3.zero;
            }
            else
            {
                _mHeading = (mouseTest - this.transform.position).normalized;
            }
            _mHeading.y = 0;
            
            _movementDirection = _mHeading;
            _movementDirection = Camera.main.transform.TransformDirection(_movementDirection);
            _movementDirection.y = 0f;
        }

        protected virtual void MoveToMousePosition()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 touchPos = Input.mousePosition;
                Transform camTrans = Camera.main.transform;
                float dist = Vector3.Dot(this.transform.position - camTrans.position, camTrans.forward);
                touchPos.z = dist;
                mouseTest = Camera.main.ScreenToWorldPoint(touchPos);
                mouseTest.y = 0;
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(mouseTest, 0.5f);
        }
    }
}
