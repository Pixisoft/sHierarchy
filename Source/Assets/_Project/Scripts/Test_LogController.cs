/**
 * Copyright (c) Pixisoft. All rights reserved.
 * 
 * pixisoft.tw@gmail.com
 */
using UnityEngine;

namespace sHierarchy.Test
{
    public class Test_LogController : MonoBehaviour
    {
        /* Variables */

        /* Setter & Getter */

        /* Functions */

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
                Debug.Log("oy");
            if (Input.GetKeyDown(KeyCode.W))
                Debug.LogWarning("oy");
            if (Input.GetKeyDown(KeyCode.E))
                Debug.LogError("oy");
        }
    }
}
