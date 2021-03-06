﻿using System;
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
        [SerializeField] protected float minimumDistance = 1.0f; //minimum distance between each of the spawned points 
        [SerializeField] protected PlantFSM spawnObject;
        [SerializeField] protected List<PlantFSM> spawnObjects = new List<PlantFSM>();

        protected GameManager manager;
        
        // Start is called before the first frame update
        void Awake()
        {
            manager = GameManager.Instance;
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
                    Vector3 samplePosition = SampleRandomPosition();
                    Vector3 position = RaycastDown(samplePosition);

                    if (CheckValidity(position, GameManager.Instance.plantsControllerFsm.Plants))
                    {
                        PlantFSM spawnedObj = Instantiate(spawnObject, position + Vector3.up, Random.rotation ) as PlantFSM;
                    }
                }
            }
        }

        protected Vector3 SampleRandomPosition()
        {
            var worldPos = this.transform.position;
            
            var X = Random.value * areaSurface.x - areaSurface.x/2;
            var Y = this.transform.position.y;
            var Z = Random.value * areaSurface.z - areaSurface.z/2;
            
            return new Vector3(X,Y,Z) + worldPos;
        }

        protected Vector3 RaycastDown(Vector3 from)
        {
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(from, Vector3.down, out hit, Mathf.Infinity))
            {
                return hit.point;
            }

            return Vector3.zero;
        }

        protected bool CheckValidity(Vector3 currentPos, List<PlantFSM> otherObjects)
        {
            if (currentPos == Vector3.zero)
                return false;
            
            if (otherObjects != null)
            {
                foreach (var obj in otherObjects)
                {
                    Vector3 otherObjectPosition = obj.transform.position;
                    if (Vector3.Distance(currentPos, otherObjectPosition) < minimumDistance)
                        return false;
                }
            }
            return true; //is valid
        }

        protected void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(this.transform.position, areaSurface);
        }
    }

}
