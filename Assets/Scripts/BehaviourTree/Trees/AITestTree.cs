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
    public class AITestTree : BehaviourTree<AIController>
    {

        [SerializeField] protected RepeaterNode root;

        [SerializeField] protected SequenceNode moveSequenceNode;
        [SerializeField] protected SequenceNode grabSword;

        [SerializeField] protected CheckForItem hasSwordCondition;
        [SerializeField] protected MoveToNode moveToOne;
        [SerializeField] protected MoveToNode moveToTwo;
        [SerializeField] protected MoveToNode moveToThree;
        [SerializeField] protected MoveToNode moveToFour;

        public AITestTree(AIController context) : base(context)
        {
            this.context = context;
        }

        public override void Initialize()
        {
            //for each children, get component, set context (this)

            moveToOne = new MoveToNode(context, context.tetherObject.transform.position);
            // moveToTwo = new MoveToNode(context, context.destinationTwo.transform.position);
            // moveToThree = new MoveToNode(context, context.destinationThree.transform.position);
            // moveToFour = new MoveToNode(context, context.destinationFour.transform.position);

            //initiation behaviour tree here
            grabSword = new SequenceNode("Grab Sword",
                new CheckForItem(context)
                // new MoveToNode(context, context.destinationFour.transform.position)
            );

            moveSequenceNode = new SequenceNode("Move Sequence",
                moveToOne,
                moveToTwo,
                moveToThree
                // grabSword
            );

            root = new RepeaterNode(
                moveSequenceNode, false, 3
            );
        }

        public override void Run()
        {
            root.TickNode();
        }
    }
}