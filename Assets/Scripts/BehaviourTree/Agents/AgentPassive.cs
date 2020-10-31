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
        [SerializeField] protected PassiveAgentTree passiveAgentTree;

        protected int frameCount = 0;
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            
            passiveAgentTree = new PassiveAgentTree(this); //pass the context first
            passiveAgentTree.Initialize(); //Initialize Tree
        }

        protected override void Update()
        {
            base.Update();
            
            passiveAgentTree.Run(); //run the tree (updates)
            
            //debugging the link
            frameCount++;
            if (frameCount % 10 == 0)
            {
                passiveAgentTree.Debug();
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            passiveAgentTree.DrawGizmos(); //draw tree gizmos 
        }
    }

}
