using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    public interface IParticleEffect
    {
        ParticleSystem ParticleEffect{ get; set; }
        void TriggerParticleEffect();
    }
}
