#if UNITY_EDITOR
/**
 * Copyright (c) Pixisoft. All rights reserved.
 * 
 * pixisoft.tw@gmail.com
 */
using System;
using System.Collections.Generic;
using UnityEngine;

namespace sHierarchy
{
    [Serializable]
    public struct InstanceInfo
    {
        public Component guessedComponent;  // guess component
        public List<Component> components;  // components on GameObject
        public bool isSeparator;
        public string goName;
        public int prefabInstanceID;
        public bool isGoActive;

        public bool isLastElement;
        public bool hasChilds;
        public bool topParentHasChild;

        public int nestingGroup;
        public int nestingLevel;
    }
}
#endif
