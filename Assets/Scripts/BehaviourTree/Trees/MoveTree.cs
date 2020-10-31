using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    /*
     * NOTE
     *     NOT FINISHED **DO NOT USE YET**
     */
    [System.Serializable]
    public class MoveTree : BehaviourTree<AIController>
    {
        [SerializeField] protected RepeaterNode repeatMoveSequence;
     
        [SerializeField] protected SequenceNode moveSequenceNode;
        [SerializeField] protected SelectorNode moveSelectorNode;

        [SerializeField] protected MoveToNode moveToOne;
        [SerializeField] protected MoveToNode moveToTwo;
        [SerializeField] protected MoveToNode moveToThree;
        [SerializeField] protected MoveToNode moveToFour;
        
        public MoveTree(AIController context) : base(context)
        {
            this.context = context;
        }

        public override void Initialize()
        {
            moveToOne = new MoveToNode(context, context.tetherObject.transform.position);
            // moveToTwo = new MoveToNode(context, context.destinationTwo.transform.position);
            // moveToThree = new MoveToNode(context, context.destinationThree.transform.position);
            // moveToFour = new MoveToNode(context, context.destinationFour.transform.position);
            
            moveSequenceNode = new SequenceNode("Move Sequence",
                moveToOne,
                moveToTwo,
                moveToThree,
                moveToFour
            );
            
            //will complete move sequence before it checks again for player to follow
            moveSelectorNode = new SelectorNode("Follow Selector",
                // followSequenceNode,
                moveToTwo
            );
            
            //root node
            repeatMoveSequence = new RepeaterNode(
                moveSequenceNode, true
            );
        }

        public override void Run()
        {
            repeatMoveSequence.TickNode();
        }
    }

}