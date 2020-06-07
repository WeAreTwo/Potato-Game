using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    //reference : https://answers.unity.com/questions/1040319/whats-the-proper-way-to-queue-and-space-function-c.html
    public class CoroutineQueue : MonoBehaviour
    {
        MonoBehaviour m_Owner = null;
        Coroutine m_InternalCoroutine = null;
        [SerializeField] public Queue<IEnumerator> actions = new Queue<IEnumerator>();
        
        public CoroutineQueue(MonoBehaviour aCoroutineOwner)
        {
            m_Owner = aCoroutineOwner;
        }
        
        public void StartLoop()
        {
            m_InternalCoroutine = m_Owner.StartCoroutine(Process());
        }
        
        public void StopLoop()
        {
            m_Owner.StopCoroutine(m_InternalCoroutine);
            m_InternalCoroutine = null;
        }
        
        public void EnqueueAction(IEnumerator aAction)
        {
            actions.Enqueue(aAction);
        }
        
        public void EnqueueWait(float aWaitTime)
        {
            actions.Enqueue(Wait(aWaitTime));
        }
     
        private IEnumerator Wait(float aWaitTime)
        {
            yield return new WaitForSeconds(aWaitTime);
        }

        private IEnumerator Process()
        {
            while (true)
            {
                if (actions.Count > 0)
                    yield return m_Owner.StartCoroutine(actions.Dequeue());
                else
                    yield return null;
            }
        }
    }
}
