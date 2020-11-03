using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PotatoGame
{
    //will be used to insert live agents to produce 2 of their seeds 
    public class SeedDispenser : MonoBehaviour
    {
        [SerializeField] protected bool isProcessing = false;

        
        [SerializeField] protected float processingTimer = 0;
        [SerializeField] protected float processingTime = 5.0f; //second to process plant

        [SerializeField] protected GameObject plantObject;
        [SerializeField] protected Plant plantComponent;

        public List<GameObject> agents = new List<GameObject>();

        protected void Update()
        {
            if(isProcessing) ProcessSeed();

            if (Input.GetKeyDown(KeyCode.I))
            {
                InsertPlant(agents[0]);
            }
        }

        protected void ProcessSeed()
        {
            processingTimer += Time.deltaTime;

            if (processingTimer >= processingTime)
            {
                DispenseSeeds();    //spawn 2 seeds
            }
        }

        protected void DispenseSeeds()
        {
            int yield = plantComponent.GrowthSettings.harvestYield;
            // Type type = plantComponent.GetType();
            
            
            for (int i = 0; i < yield; i++)
            {
                GameObject seed = Instantiate(
                    agents[0],
                    this.transform.position + Vector3.up * 2,
                    Quaternion.identity)
                    as GameObject;  //instantiate
            }

            //reset the dispenser
            plantObject = null;
            plantComponent = null;
            isProcessing = false;
            processingTimer = 0;
        }

        public void InsertPlant(GameObject plant)
        {
            if (isProcessing) return; //cannot insert if there is something in it 
            
            //make sure its of type plant
            if (plant.TryGetComponent(out Plant component))
            {
                plantComponent = component; //reference the plant component 
                isProcessing = true; //will now start to process
            }
        }
        
    }

}