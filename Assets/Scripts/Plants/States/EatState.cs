using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    [System.Serializable]
    public class Eat<T> : State where T : Potato
    {
        protected T component;

        protected float eatTimer = 0.0f;
        protected float eatTime = 3.0f;

        public Eat(T component)
        {
            this.component = component;
        }

        //Everyframe while ur in this state
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            EatPotato();
        }

        protected virtual void EatPotato()
        {
            //condition for completion
            if (component.victim == null)
            {
                component.eatingEffect.SetActive(false);
                MakeDecision();
            }
            else
            {
                Vector3 targetPosition = component.victim.transform.position;
                if (Vector3.Distance(component.transform.position, targetPosition) < 2.5f * component.GrowthParams.growthRadius)
                {
                    eatTimer += Time.deltaTime;
                    if (eatTimer >= eatTime)
                    {
                        eatTimer = 0.0f;
                        if (component.victim.Health - 25.0f <= 0)
                        {
                            // killCount++;
                            component.victim.Health -= 25.0f;
                            component.victim.Kill();
                        }
                        else
                        {
                            component.victim.Health -= 25.0f;
                        }
                    }
                }
                else
                {
                    Vector3 force = (targetPosition - component.transform.position).normalized;
                    component.Rb.AddForce(force * component.seekForce);
                }
                
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
            // CheckForNearbyPotatoes();
            
            float roll = Random.value;   //roll
            if (component.victim != null)
            {
                component.eatingEffect.SetActive(true);
                // TriggerExit(PlantStates.Eat);
            }
            // else if (roll < 0.45f)
            // {
            //     TriggerExit(PlantStates.Move);
            // }
            else
            {
                TriggerExit(PlantStates.Idle); //idle in the same spot
            }

        }

    
        public override void DrawGizmos()
        {
            base.DrawGizmos();
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(component.transform.position, Vector3.one * component.GrowthParams.growthRadius);
    
            if (component.victim != null)
            {
                //draw a line to target
                Gizmos.color = Color.black;
                Gizmos.DrawLine(component.transform.position, component.victim.transform.position);
            }
    
        }
    }
    
    
}