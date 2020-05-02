using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PotatoGame
{
    public class PlantSpawner : MonoBehaviour
    {
        //SPAWNER PARAMS
        [SerializeField] protected Vector3 areaSurface = new Vector3(1,1,1);
        [SerializeField] protected int sampleAmount = 20;
        [SerializeField] protected GameObject spawnObject;
        [SerializeField] protected List<GameObject> spawnObjects = new List<GameObject>();
    
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Generate();
        }

        protected void Generate()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                for (int i = 0; i < sampleAmount; i++)
                {
                    Vector3 position = SampleRandomPosition();
                    GameObject spawnedObj = Instantiate(spawnObject, position, Quaternion.identity ) as GameObject;
                    spawnObjects.Add(spawnedObj);
                }
            }
        }

        protected void UpdatePosition()
        {
            
        }

        protected Vector3 SampleRandomPosition()
        {
            var worldPos = this.transform.position;
            
            var X = Random.value * areaSurface.x - areaSurface.x/2;
            var Y = this.transform.position.y;
            var Z = Random.value * areaSurface.z - areaSurface.z/2;
            
            //this.transform.position + areaSurface.x/2
            //this.transform.position + areaSurface.z/2
            
            return new Vector3(X,Y,Z) + worldPos;
        }

        protected void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(this.transform.position, areaSurface);
        }
    }

}
