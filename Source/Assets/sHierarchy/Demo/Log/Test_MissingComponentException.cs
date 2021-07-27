/**
 * Copyright (c) Pixisoft. All rights reserved.
 * 
 * pixisoft.tw@gmail.com
 */
using UnityEngine;

namespace sHierarchy.Test
{
    public class Test_MissingComponentException : MonoBehaviour
    {
        /* Variables */

        /* Setter & Getter */

        /* Functions */

        private void Start()
        {
            SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
            sr.sprite = null;  // cause the error here
        }
    }
}
