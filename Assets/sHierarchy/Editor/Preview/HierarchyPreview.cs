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
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace sHierarchy
{
    /// <summary>
    /// Preview selected GameObject in Hierarchy.
    /// </summary>
    [CustomPreview(typeof(GameObject))]
    public class HierarchyPreview : ObjectPreview
    {
        /* Variables */

        private static GameObject mTarget = null;
        private static GameObject mClone = null;
        private static Editor mEditor = null;

        private static PreviewRenderUtility mPreviewRenderer = null;

        private static Vector2 mLastMousePos = Vector2.zero;

        private static bool mAutoRotate = false;

        /* Setter & Getter */

        public static Camera camera { get { return mPreviewRenderer.camera; } }
        public static Light DirectionalLight { get { return mPreviewRenderer.lights[0]; } }

        /* Functions */

        public override bool HasPreviewGUI()
        {
            if (!HierarchyData.instance.enabled || !HierarchyData.instance.preview.enabled ||
                Selection.activeGameObject == null || Selection.gameObjects.Length != 1)
                return false;

            // TODO: Close with certain cases

            return true;
        }

        public override void Cleanup()
        {
            base.Cleanup();
            CleanupInternal();
        }

        private static void CleanupInternal()
        {
            if (mPreviewRenderer == null)
                return;
            mPreviewRenderer.Cleanup();
            mPreviewRenderer = null;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            Draw();
        }

        public static void Draw()
        {
            InitRenderer();
            GetSelected();
            InitEditor();
            DoRotate();
            DrawSelected();
            DrawOptions();
        }

        private static void GetSelected()
        {
            if (mTarget == Selection.activeGameObject)
                return;

            if (mEditor != null) Object.DestroyImmediate(mEditor);
            mTarget = Selection.activeGameObject;
            mClone = null;

            InitScene();
        }

        private static void InitEditor()
        {
            if (mTarget == null)
                return;

            if (mEditor == null)
                mEditor = Editor.CreateEditor(mTarget);
        }

        private static void InitRenderer()
        {
            if (mPreviewRenderer == null)  // Initialize once
            {
                CleanupInternal();

                mPreviewRenderer = new PreviewRenderUtility();
                mPreviewRenderer.camera.nearClipPlane = 0.01f;
                mPreviewRenderer.camera.farClipPlane = 10000;  // Just set far plane to very far
                ResetCameraRotation();
            }

            camera.clearFlags =
                (HierarchyData.instance.preview.skybox) ?
                CameraClearFlags.Skybox : CameraClearFlags.SolidColor;

            UpdateDirectionalLight();
        }

        private static void DrawSelected()
        {
            if (mTarget == null)
                return;

            Render(() =>
            {
                // empty..
            });

            mEditor.Repaint();
        }

        private static void DoRotate()
        {
            if (Event.current.type == EventType.MouseDown)
                mLastMousePos = Event.current.mousePosition;

            float rotateSpeed = HierarchyData.instance.preview.rotateSpeed;

            if (mClone == null)
                return;

            if (!HierarchyUtil.IsFocusedWindow("Inspector") || Event.current.type != EventType.MouseDrag)
            {
                if (mAutoRotate)
                {
                    float speed = rotateSpeed * Time.deltaTime;
                    RotateX(speed);
                }
                return;
            }

            mAutoRotate = false;

            Vector3 mousePosition = Event.current.mousePosition;

            float rotX = (mousePosition.x - mLastMousePos.x) * rotateSpeed * Time.deltaTime;
            float rotY = (mousePosition.y - mLastMousePos.y) * rotateSpeed * Time.deltaTime;

            RotateX(-rotX);
            RotateY(-rotY);

            mLastMousePos = mousePosition;
        }

        private static void Render(EmptyFunction func)
        {
            var boundaries = GUILayoutUtility.GetLastRect();
            mPreviewRenderer.BeginPreview(boundaries, GUIStyle.none);
            func.Invoke();
            camera.Render();
            var render = mPreviewRenderer.EndPreview();
            GUI.DrawTexture(boundaries, render);
        }

        private static void UpdateDirectionalLight()
        {
            if (mPreviewRenderer == null || mPreviewRenderer.lights.Length == 0)
                return;

            if (DirectionalLight == null)
                return;

            DirectionalLight.transform.eulerAngles = HierarchyData.instance.preview.lightRotation;
            DirectionalLight.intensity = HierarchyData.instance.preview.lightIntensity;
        }

        private static void DrawOptions()
        {
            HierarchyUtil.Button("Reset", ResetCameraRotation);
        }

        private static void ResetCameraRotation()
        {
            if (mClone == null)
                return;

            mClone.transform.eulerAngles = Vector3.zero;
            mAutoRotate = HierarchyData.instance.preview.autoRotate;

            if (mClone.GetComponent<Canvas>() != null)
                mAutoRotate = false;
        }

        private static void InitScene()
        {
            if (mPreviewRenderer == null || mTarget == null)
                return;

            mClone = GameObject.Instantiate(mTarget);
            mClone.transform.position = Vector3.zero;
            mClone.transform.localScale = mClone.transform.localScale.normalized;
            EnableCustomComponents(mClone, false);
            mClone.AddComponent<SphereCollider>();
            mPreviewRenderer.AddSingleGO(mClone);

            ResetCameraRotation();

            FocusObject();
        }

        /// <summary>
        /// Return the Target's bounds for focusing the preview object.
        /// </summary>
        private static Bounds GetBounds()
        {
            Bounds bounds = new Bounds();
            RectTransform rect = mClone.GetComponent<RectTransform>();

            if (rect != null)
            {
                // https://docs.unity3d.com/ScriptReference/RectTransform.GetWorldCorners.html
                {
                    Vector3[] v = new Vector3[4];
                    rect.GetWorldCorners(v);
                    bounds.SetMinMax(v[0], v[2]);
                }
            }
            else
            {
                Collider collider = mClone.GetComponent<SphereCollider>();
                if (collider != null)
                    bounds = collider.bounds;
            }

            return bounds;
        }

        private static void FocusObject()
        {
            Bounds bounds = GetBounds();
            Vector3 objectSizes = bounds.max - bounds.min;
            float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
            float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * camera.fieldOfView);
            float distance = HierarchyData.instance.preview.distance * objectSize / cameraView;
            distance += 0.5f * objectSize;
            camera.transform.position = bounds.center - distance * camera.transform.forward;
        }

        private static void RotateX(float speed)
        {
            mClone.transform.RotateAround(Vector3.zero, Vector3.up, speed);
        }

        private static void RotateY(float speed)
        {
            mClone.transform.RotateAround(Vector3.zero, Vector3.right, speed);
        }

        private static void EnableCustomComponents(GameObject go, bool active)
        {
            foreach (var component in go.GetComponents<MonoBehaviour>())
            {
                if (component == null) continue;
                component.enabled = active;
            }

            for (int index = 0; index < go.transform.childCount; ++index)
                EnableCustomComponents(go.transform.GetChild(index).gameObject, active);
        }
    }
}
#endif
