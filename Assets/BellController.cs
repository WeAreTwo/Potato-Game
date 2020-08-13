using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BellController : MonoBehaviour
{
    // public variables -------------------------
    [Title("Bell Body")]
    public Rigidbody m_bellBody;  // Main body of the bell
    [MinMaxSlider(-360, 360, true)]public Vector2 m_bellRotRange;  // Rotation range for the bell body
    public float m_bellForce;  // Force applied to the bell when triggered
    public bool m_testRing;  // Test the bell
    
    // private variables ------------------------
    


    // ------------------------------------------
    // Start is called before update
    // ------------------------------------------
    void Start()
    {
        
        
    }

    // ------------------------------------------
    // Update is called once per frame
    // ------------------------------------------
    void Update()
    {
        if(m_testRing)
            DingDong();
        
    }

    // ------------------------------------------
    // Methods
    // ------------------------------------------
    // Ring the bel ding dong --------------------------------------------------
    private void DingDong()
    {
        m_testRing = false;
        
        m_bellBody.AddForce(Vector3.forward * m_bellForce);


    }
}
