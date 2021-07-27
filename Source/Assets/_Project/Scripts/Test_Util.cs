/**
 * Copyright (c) Pixisoft. All rights reserved.
 * 
 * pixisoft.tw@gmail.com
 */
using UnityEngine;

namespace sHierarchy
{
    public static class Test_Util
    {
        public static void EnableCustomComponents(GameObject go, bool active)
        {
            foreach (var component in go.GetComponents<MonoBehaviour>())
                component.enabled = active;

            for (int index = 0; index < go.transform.childCount; ++index)
                EnableCustomComponents(go.transform.GetChild(index).gameObject, active);
        }
    }
}
