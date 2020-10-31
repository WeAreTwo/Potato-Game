using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    [System.Serializable]
    public class PickItemTree : BehaviourTree<AIController>
    {
        [SerializeField] protected SequenceNode pickUpSequence;
        [SerializeField] protected CheckNearbyItem checkNearbyItem;
        [SerializeField] protected PickUpItem pickUpItem;
        [SerializeField] protected WaitFor waitFor;
        [SerializeField] protected DropItem dropItem;
        
        public PickItemTree(AIController context) : base(context)
        {
            this.context = context;
        }

        public override void Initialize()
        {
            checkNearbyItem = new CheckNearbyItem(context);
            pickUpItem = new PickUpItem(context);
            waitFor = new WaitFor(context);
            dropItem = new DropItem(context);
            pickUpSequence = new SequenceNode(
                "Pick Up Sequence",
                checkNearbyItem,
                // new WaitFor(this, 2.0f),
                pickUpItem,
                waitFor,
                dropItem

            );
        }

        public override void Run()
        {
            pickUpSequence.TickNode();
        }
    }

}
