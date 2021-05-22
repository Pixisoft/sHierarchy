#if UNITY_EDITOR
/**
 * Copyright (c) 2021 Jen-Chieh Shen
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software/algorithm and associated
 * documentation files (the "Software"), to use, copy, modify, merge or distribute copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * - The Software, substantial portions, or any modified version be kept free of charge and cannot be sold commercially.
 * 
 * - The above copyright and this permission notice shall be included in all copies, substantial portions or modified
 * versions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 * For any other use, please ask for permission by contacting the author.
 */
using UnityEditor;
using UnityEngine;

namespace sHierarchy
{
    /// <summary>
    /// Preview selected gameobject in Hierarch in Inspector window.
    /// </summary>
    public class Data_Preview : HierarchyComponent
    {
        /* Variables */

        private const string INFO =
            @"Preview selected GameObject in the inspector";

        private const string FOLD_NAME = "Separator";
        public bool foldout = false;

        public float rotateSpeed = 15f;
        public bool autoRotate = true;
        public float distance = 1.1f;  // how far apart from preview object

        #region Light
        public Vector3 lightRotation = new Vector3(50, -30, 0);
        public float lightIntensity = 1f;
        #endregion

        public bool skybox = false;

        /* Setter & Getter */

        /* Functions */

        public override bool GetEnabled()
        {
            var hcpe = HierarchyControlPanelEditor.instance;
            if (hcpe != null) return hcpe.f_preview;
            return this.enabled;
        }

        public override string FormKey(string name) { return HierarchyUtil.FormKey("preview.") + name; }

        public override void Init()
        {
            this.enabled = EditorPrefs.GetBool(FormKey("enabled"), this.enabled);
            this.rotateSpeed = EditorPrefs.GetFloat(FormKey("rotateSpeed"), this.rotateSpeed);
            this.autoRotate = EditorPrefs.GetBool(FormKey("autoRotate"), this.autoRotate);
            this.lightRotation = HierarchyUtil.GetVector3(FormKey("lightRotation"), lightRotation);
            this.lightIntensity = EditorPrefs.GetFloat(FormKey("lightIntensity"), lightIntensity);
            this.distance = EditorPrefs.GetFloat(FormKey("distance"), this.distance);
            this.skybox = EditorPrefs.GetBool(FormKey("skybox"), this.skybox);
        }

        public override void Draw()
        {
            foldout = EditorGUILayout.Foldout(foldout, "Preview");

            if (!foldout)
                return;

            HierarchyUtil.CreateGroup(() =>
            {
                HierarchyUtil.CreateInfo(INFO);

                this.enabled = HierarchyUtil.Toggle("Enabeld", this.enabled,
                    @"Enable/Disable all features from this section");

                EditorGUILayout.LabelField("Rotate", EditorStyles.boldLabel);

                HierarchyUtil.CreateGroup(() =>
                {
                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        this.rotateSpeed = EditorGUILayout.Slider("Rotate Speed", this.rotateSpeed, 0, 30);
                        HierarchyUtil.Button("Reset", ResetRotateSpeed);
                    });

                    this.autoRotate = EditorGUILayout.Toggle("Auto Rotate", this.autoRotate);
                });

                HierarchyUtil.LabelField("Light");

                HierarchyUtil.CreateGroup(() =>
                {
                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        this.lightRotation = EditorGUILayout.Vector3Field("Rotation", this.lightRotation);
                        HierarchyUtil.Button("Reset", ResetLightRotation);
                    });

                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        this.lightIntensity = EditorGUILayout.FloatField("Intensity", this.lightIntensity);
                        HierarchyUtil.Button("Reset", ResetLightIntensity);
                    });
                });

                HierarchyUtil.BeginHorizontal(() =>
                {
                    this.distance = EditorGUILayout.Slider("Distance", this.distance, 0, 10);
                    HierarchyUtil.Button("Reset", ResetDistance);
                });

                this.skybox = EditorGUILayout.Toggle("Skybox", this.skybox);
            });
        }

        public override void SavePref()
        {
            EditorPrefs.SetBool(FormKey("enabled"), this.enabled);
            EditorPrefs.SetFloat(FormKey("rotateSpeed"), this.rotateSpeed);
            EditorPrefs.SetBool(FormKey("autoRotate"), this.autoRotate);
            HierarchyUtil.SetVector3(FormKey("lightRotation"), this.lightRotation);
            EditorPrefs.SetFloat(FormKey("lightIntensity"), this.lightIntensity);
            EditorPrefs.SetFloat(FormKey("distance"), this.distance);
            EditorPrefs.SetBool(FormKey("skybox"), this.skybox);
        }

        private void ResetRotateSpeed() { this.rotateSpeed = 15f; }
        private void ResetDistance() { this.distance = 1.1f; }
        private void ResetLightRotation() { this.lightRotation = new Vector3(50, -30, 0); }
        private void ResetLightIntensity() { this.lightIntensity = 1f; }
    }
}
#endif
