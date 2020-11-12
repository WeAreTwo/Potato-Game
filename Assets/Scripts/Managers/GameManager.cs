using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    public class GameManager : Singleton<GameManager>
    {
        // (Optional) Prevent non-singleton constructor use.
        protected void Singleton() { }

        public PlayerController playerController;
        public IKController ikController;
        public TopViewCameraController cameraController;
        public PlantsControllerFSM plantsControllerFsm;
        public ShaderController shaderController;
        public VarietyPoolController varietyPoolController;

        protected void Awake()
        {
            
            // if (playerController == null) Debug.LogWarning("playerController not set in the game manager!");
            // if(ikController == null) Debug.LogWarning("ikController not set in the game manager!");
            // if(inventoryController == null) Debug.LogWarning("inventoryController not set in the game manager!");
            // if(cameraController == null) Debug.LogWarning("cameraController not set in the game manager!");
            // if(plantsController == null) Debug.LogWarning("plantsController not set in the game manager!");
            // if(shaderController == null) Debug.LogWarning("shaderController not set in the game manager!");
            // if(varietyPoolController == null) Debug.LogWarning("varietyPool not set in the game manager!"); 
            //
            
            if(playerController == null) GetController(ref playerController);
            if(ikController == null) GetController(ref ikController);
            if(cameraController == null) GetController(ref cameraController);
            if(plantsControllerFsm == null) GetController(ref plantsControllerFsm);
            if(shaderController == null) GetController(ref shaderController);
            if(varietyPoolController == null) GetController(ref varietyPoolController);
        }

        protected void GetScriptInScene<T>(ref T component) where T : MonoBehaviour
        {
            component = (T)FindObjectOfType(typeof(T));
        }

        protected void GetController<T>(ref T component) where T : MonoBehaviour
        {
            GetScriptInScene<T>(ref component);
            if (!component)
                Debug.LogWarning( typeof(T).ToString() + " not set in the game manager!");
        }
    }

}
