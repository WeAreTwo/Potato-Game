using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    //BASE CLASS FOR ALL LIVING THINGS THAT GROW
    public abstract class Plant : MonoBehaviour
    {
        
        [SerializeField] protected float growthRadius;                  //growth counter   

        [SerializeField] protected float growthTime = 0.0f;                  //growth counter   
        [SerializeField] protected float growthStartTime;             //time from when it was planted and growing
        [SerializeField] protected float growthCompletetionTime;      //time where it finished growing

        [SerializeField] protected bool isGrowing;
        
        protected virtual void Awake(){}
        protected virtual void Start(){}
        
        protected virtual void Update()
        {
            Grow();
        }

        protected virtual void Grow()
        {
            if (growthTime < growthCompletetionTime)
            {
                growthTime += Time.deltaTime;
                isGrowing = true;
            }
            else
            {
                isGrowing = false;
            }
        }

        protected virtual void OnEnable()
        {
            growthStartTime = Time.time;
        }
        
        protected virtual void OnDisable(){}

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(this.transform.position, growthRadius);
        }
    }

}
