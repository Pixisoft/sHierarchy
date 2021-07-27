/**
 * Copyright (c) Pixisoft. All rights reserved.
 * 
 * pixisoft.tw@gmail.com
 */
using UnityEngine;

namespace sHierarchy.Test
{
    public class Test_RipComponents : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Test_Util.EnableCustomComponents(this.gameObject, false);
            }
        }
    }
}
