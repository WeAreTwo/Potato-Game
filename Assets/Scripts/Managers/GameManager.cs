using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Potato
{

    public class GameManager : Singleton<GameManager>
    {
        // (Optional) Prevent non-singleton constructor use.
        protected void Singleton() { }

        public PlayerController playerController;
        public TopViewCameraController cameraController;

    }

}
