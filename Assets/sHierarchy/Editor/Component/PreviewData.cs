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
    public class PreviewData : HierarchyComponent
    {
        /* Variables */

        public bool foldout = false;

        public bool enabled = true;
        public float rotateSpeed = 1.0f;

        #region Light
        public Vector3 lightRotation = new Vector3(50, -30, 0);
        public float lightIntensity = 1f;
        #endregion

        /* Setter & Getter */

        /* Functions */

        public string FormKey(string name) { return HierarchyUtil.FormKey("preview.") + name; }

        public void Init()
        {
            this.enabled = EditorPrefs.GetBool(FormKey("enabled"), this.enabled);
            this.rotateSpeed = EditorPrefs.GetFloat(FormKey("rotateSpeed"), this.rotateSpeed);
            this.lightRotation = HierarchyUtil.GetVector3(FormKey("lightRotation"), lightRotation);
            this.lightIntensity = EditorPrefs.GetFloat(FormKey("lightIntensity"), lightIntensity);
        }

        public void Draw()
        {
            foldout = EditorGUILayout.Foldout(foldout, "Preview");

            if (!foldout)
                return;

            HierarchyUtil.CreateGroup(() =>
            {
                this.enabled = EditorGUILayout.Toggle("Enabeld", this.enabled);

                HierarchyUtil.BeginHorizontal(() =>
                {
                    this.rotateSpeed = EditorGUILayout.Slider("Rotate Speed", this.rotateSpeed, 0, 10);

                    if (GUILayout.Button("Reset", GUILayout.Width(50)))
                        ResetRotateSpeed();
                });

                EditorGUILayout.LabelField("Light", EditorStyles.boldLabel);

                HierarchyUtil.CreateGroup(() =>
                {
                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        this.lightRotation = EditorGUILayout.Vector3Field("Rotation", this.lightRotation);

                        if (GUILayout.Button("Reset", GUILayout.Width(50)))
                            ResetLightRotation();
                    });

                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        this.lightIntensity = EditorGUILayout.FloatField("Intensity", this.lightIntensity);

                        if (GUILayout.Button("Reset", GUILayout.Width(50)))
                            ResetLightIntensity();
                    });
                });
            });
        }

        public void SavePref()
        {
            EditorPrefs.SetBool(FormKey("enabled"), this.enabled);
            EditorPrefs.SetFloat(FormKey("rotateSpeed"), this.rotateSpeed);
            HierarchyUtil.SetVector3(FormKey("lightRotation"), this.lightRotation);
            EditorPrefs.SetFloat(FormKey("lightIntensity"), this.lightIntensity);
        }

        private void ResetRotateSpeed() { this.rotateSpeed = 1f; }

        private void ResetLightRotation() { this.lightRotation = new Vector3(50, -30, 0); }

        private void ResetLightIntensity() { this.lightIntensity = 1f; }
    }
}
#endif
