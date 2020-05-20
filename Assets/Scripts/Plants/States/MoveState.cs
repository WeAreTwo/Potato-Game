using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    [System.Serializable]
    public class Move<T> : State where T : Potato
    {
        protected T component;
        
        protected Vector3 seekPosition;
        protected float moveTimer = 0.0f;
        protected float moveTime = 5.0f;

        public Move(T component)
        {
            this.component = component;
        }
    
        //When you first switch into this state
        public override void OnStateStart()
        {
            base.OnStateStart();
            PickRandomPosition();
        }
    
        //Everyframe while ur in this state
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            MoveToPosition();
        }

        protected virtual void PickRandomPosition()
        {
            float randX = Random.Range(-1.0f, 1.0f);
            float randY = Random.Range(-1.0f, 1.0f);
            Vector3 randomPos = new Vector3(randX,0,randY);
            Vector3 currentPosXZ = new Vector3(component.transform.position.x,0,component.transform.position.z);
            seekPosition = currentPosXZ + (randomPos * component.seekRange);
        }
        
        protected virtual void MoveToPosition()
        {
            if (moveTimer <= moveTime)
            {
                moveTimer += Time.deltaTime;
                seekPosition.y = component.transform.position.y;
                Vector3 force = (seekPosition - component.transform.position).normalized;
                component.Rb.AddForce(force * component.seekForce);            
            
            }
            //condition for completion 
            if (Vector3.Distance(component.transform.position, seekPosition) < 1.5f * component.GrowthParams.growthRadius || moveTimer >= moveTime)
            {
                moveTimer = 0;  // reset the timer 
                MakeDecision(); // make new decision 
            }
        }

        protected virtual void CheckForNearbyPotatoes()
        {
            //Check is there are other potatoes nearby
            var allPlants = GameManager.Instance.plantsController.Plants;
            if (allPlants != null)
            {
                foreach (var plant in allPlants)
                {
                    if (component == plant) continue; //ignore self by skipping it 

                    if (Vector3.Distance(component.transform.position, plant.transform.position) < component.GrowthParams.growthRadius + plant.GrowthParams.growthRadius)
                        component.victim = plant;
                }
            }
        }

        protected virtual void MakeDecision()
        {
            CheckForNearbyPotatoes();
            
            //if there is a victim, go to eating mode 
            if (component.victim != null)
                TriggerExit(PlantStates.Eat);
            else
                PickRandomPosition();

        }
    
        public override void DrawGizmos()
        {
            base.DrawGizmos();
            //draw the seek range 
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(component.transform.position, component.seekRange);
            //draw the target 
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(seekPosition, 0.2f);
            //draw a line to target
            Gizmos.color = Color.black;
            Gizmos.DrawLine(component.transform.position, seekPosition);
    
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(component.transform.position, Vector3.one * component.GrowthParams.growthRadius);
        }
    }
    

}
