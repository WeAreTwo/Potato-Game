using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    /*
     * NOTE
     *     AGENT SCRIPTS ARE THE VEGETABLES
     *     PASSIVE AGENT WILL WALK AROUND, FOLLOW THE PLAYER AND RETURN TO BIOME
     */
    public class AgentPassive : AIController
    {
        
        [Header("Behaviour Tree")]
        [SerializeField] protected FollowTree followTree;

        
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            
            followTree = new FollowTree(this); //pass the context first
            followTree.Initialize(); //Initialize Tree
        }

        protected override void Update()
        {
            base.Update();
            
            followTree.Run();
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            followTree.DrawGizmos(); //draw tree gizmos 
        }
    }

}
