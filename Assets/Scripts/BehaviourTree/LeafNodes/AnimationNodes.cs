using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    /*
     * NOTE:
     *     ANIMATION NODES WILL DERIVE FROM ACTIONS NODE AND
     *     WILL HANDLE ANY FORM OF ANIMATION INPUT FOR AI ACTIONS
     */
    
    public class AnimationNode<T> : ActionNode<T> where T : MonoBehaviour
    {
        public AnimationNode(T context) : base(context)
        {
            
        }

        public override NodeState TickNode()
        {
            throw new System.NotImplementedException();
        }
    }
}
