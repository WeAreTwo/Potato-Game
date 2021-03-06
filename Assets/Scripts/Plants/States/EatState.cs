﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    [System.Serializable]
    public class Eat<T> : State where T : PotatoFSM
    {
        protected T component;

        protected float eatTimer = 0.0f;
        protected float eatTime = 3.0f;

        protected float popOutTimer = 0.0f;
        protected float popOutTime = 3.0f;

        public Eat(T component)
        {
            this.component = component;
        }

        //Everyframe while ur in this state
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            if(component.Planted) PopOut();
            EatPotato();
        }

        protected void PopOut()
        {
            if (popOutTimer < popOutTime)
            {
                popOutTimer += Time.deltaTime;
            }
            else
            {
                PopOutOfTheGround();
                popOutTimer = 0;
            }
        }
        
        protected void PopOutOfTheGround()
        {
            //set the planted state to false 
            component.Planted = false;

            // pop out of the ground 
            component.transform.position += new Vector3(0, component.GrowthSettings.growthRadius * 2, 0);
            component.transform.rotation = Random.rotation;
            
            //particles
            ParticleController.Instance.EmitAt(component.transform.position);
    
            // Activate gravity and defreeze all
            component.Rb.ActivatePhysics();
    
            component.potatoEyes.SetActive(true);
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
                if (Vector3.Distance(component.transform.position, targetPosition) < 2.5f * component.GrowthSettings.growthRadius)
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
            var allPlants = GameManager.Instance.plantsControllerFsm.Plants;
            if (allPlants != null)
            {
                foreach (var plant in allPlants)
                {
                    if (component == plant) continue; //ignore self by skipping it 

                    if (Vector3.Distance(component.transform.position, plant.transform.position) < component.GrowthSettings.growthRadius + plant.GrowthSettings.growthRadius)
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
                TriggerExit(PlantStates.IDLE); //idle in the same spot
            }

        }

    
        public override void DrawGizmos()
        {
            base.DrawGizmos();
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(component.transform.position, Vector3.one * component.GrowthSettings.growthRadius);
    
            if (component.victim != null)
            {
                //draw a line to target
                Gizmos.color = Color.black;
                Gizmos.DrawLine(component.transform.position, component.victim.transform.position);
            }
    
        }
    }
    
    
}