using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    public class Potato : Plant
    {
        //potato params
        protected override void Awake()
        {
            base.Awake();
            this.gameObject.tag = ProjectTags.Potato;
        }

        protected override void Start()
        {
            base.Start();
            // this.transform.LookAt(this.transform.position + growingAxis);
            
        }

        protected override void Update()
        {
            base.Update();
            switch (plantStatus)
            {
                case PlantState.Planted:
                    this.transform.LookAt(this.transform.position + growingAxis);
                    break;
                default:
                    break;
            }
        }

    }

}
