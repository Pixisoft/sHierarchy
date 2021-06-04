#if UNITY_EDITOR
/**
 * Copyright (c) Pixisoft Corporations. All rights reserved.
 * 
 * Licensed under the Source EULA. See COPYING in the asset root for license informtaion.
 */

namespace sHierarchy
{
    public enum HierarchyComponentFunctions
    {
        INIT, DRAW, SAVE_PREF,
    };

    public abstract class HierarchyComponent
    {
        protected bool enabled = true;

        public abstract bool GetEnabled();
        public virtual void SetEnabled(bool val) { this.enabled = val; }

        public abstract string FormKey(string name);
        public abstract void Init();
        public abstract void Draw();
        public abstract void SavePref();
    }
}
#endif
