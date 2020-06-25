using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    public class ParticleController : Singleton<ParticleController>
    {
        [SerializeField] protected ParticleSystem _particleSystem;
        [SerializeField] protected List<ParticleSystem> _particleSystemPool = new List<ParticleSystem>();

        protected ParticleSystem _usedParticleSytem;
        
        // public ParticleSystem _ParticleSystem { get { return _particleSystem; } set { _particleSystem = value; } }

        public void EmitAt(Vector3 position)
        {
            var particle = GetAvailableParticleSystem();
            if(!particle) return;
            else
            {
                particle.transform.position = position;
                particle.Emit(4);
            }
        }

        protected ParticleSystem GetAvailableParticleSystem()
        {
            foreach (var pSystem in _particleSystemPool)
            {
                if (!pSystem.isEmitting && pSystem != _usedParticleSytem)
                {
                    _usedParticleSytem = pSystem;
                    return pSystem;
                }
            }

            return null;
        }

        protected void OnDrawGizmos()
        {
            foreach (var pSystem in _particleSystemPool)
            {
                // if (pSystem.isEmitting)
                // {
                //     Gizmos.color = Color.magenta;
                //     Gizmos.DrawSphere(pSystem.transform.position, 1.0f);
                // }
            }
        }
    }
}