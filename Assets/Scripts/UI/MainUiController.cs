using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUiController : MonoBehaviour
{
    // public variables -------------------------
    public GameObject m_inventoryMenu;  // Instance for the inventory menu 
    
    
    // private variables ------------------------
    private bool _mInventoryOpen;  // Check if the inventory is open or not

    

    // ------------------------------------------
    // Start is called before update
    // ------------------------------------------
    void Start()
    {
        // Disable menus on start
        m_inventoryMenu.SetActive(false);
    }

    // ------------------------------------------
    // Update is called once per frame
    // ------------------------------------------
    void Update()
    {
        // Trigger the inventory menu
        if (Input.GetButtonDown("Inventory"))
            InventorySwitch();
        
    }

    // ------------------------------------------
    // Methods
    // ------------------------------------------
    // Get the inventory menu opened or closed ---------------------------------------
    private void InventorySwitch()
    {
        // Switch inventory state
        _mInventoryOpen = !_mInventoryOpen;
        
        // Able or disable inventory menu
        if (_mInventoryOpen)
            m_inventoryMenu.SetActive(true);
        else
            m_inventoryMenu.SetActive(false);
    }
    
}
