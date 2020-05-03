using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    public class Potato : Plant
    {
        //potato params

        protected override void Start()
        {
            base.Start();
            this.transform.LookAt(this.transform.position + growingAxis);
            
        }

        protected override void Update()
        {
            base.Update();
            this.transform.LookAt(this.transform.position + growingAxis);
        }

        protected override void Grow()
        {
            base.Grow();
        }
    }

}
