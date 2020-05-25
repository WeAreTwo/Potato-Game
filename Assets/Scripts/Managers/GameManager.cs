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
        public InventoryController inventoryController;
        public TopViewCameraController cameraController;
        public PlantsController plantsController;
        public ShaderController shaderController;
        public VarietyPool varietyPool;

        protected void Awake()
        {
            if(playerController == null) Debug.LogAssertion("playerController not set in the game manager!");
            if(ikController == null) Debug.LogAssertion("ikController not set in the game manager!");
            if(inventoryController == null) Debug.LogAssertion("inventoryController not set in the game manager!");
            if(cameraController == null) Debug.LogAssertion("cameraController not set in the game manager!");
            if(plantsController == null) Debug.LogAssertion("plantsController not set in the game manager!");
            if(shaderController == null) Debug.LogAssertion("shaderController not set in the game manager!");
            if(varietyPool == null) Debug.LogAssertion("varietyPool not set in the game manager!");
        }
    }

}
