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

        private GameObject mTarget = null;
        private Editor mEditor = null;

        private PreviewRenderUtility mPreviewRenderer = null;

        private Vector3 mCameraPos = Vector3.zero;
        private Vector2 mLastMousePos = Vector2.zero;

        /* Setter & Getter */

        public Camera camera { get { return this.mPreviewRenderer.camera; } }
        public Light DirectionalLight { get { return this.mPreviewRenderer.lights[0]; } }

        /* Functions */

        public override bool HasPreviewGUI()
        {
            return (HierarchyData.instance.preview.enabled && Selection.activeGameObject != null);
        }

        public override void Cleanup()
        {
            base.Cleanup();

            if (mPreviewRenderer != null)
                mPreviewRenderer.Cleanup();
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            InitRenderer();
            GetSelected();
            InitEditor();
            DoRotate();
            DrawSelected();
            DrawOptions();
        }

        private void GetSelected()
        {
            if (mTarget == Selection.activeGameObject)
                return;

            if (mEditor != null) UnityEngine.Object.DestroyImmediate(mEditor);
            mTarget = Selection.activeGameObject;

            InitScene();
        }

        private void InitEditor()
        {
            if (mTarget == null)
                return;

            if (mEditor == null)
                mEditor = Editor.CreateEditor(mTarget);
        }

        private void InitRenderer()
        {
            if (mPreviewRenderer == null)  // Initialize once
            {
                mPreviewRenderer = new PreviewRenderUtility();
                mPreviewRenderer.camera.nearClipPlane = 0.01f;
                mPreviewRenderer.camera.farClipPlane = 10000;  // Just set far plane to very far
                ResetCameraRotation();
            }

            mPreviewRenderer.camera.clearFlags =
                (HierarchyData.instance.preview.skybox) ?
                CameraClearFlags.Skybox : CameraClearFlags.SolidColor;

            UpdateDirectionalLight();
        }

        private void DrawSelected()
        {
            if (mTarget == null)
                return;

            Render(() =>
            {
                // empty..
            });

            mEditor.Repaint();

            //Handles.DrawCamera(GUILayoutUtility.GetLastRect(), camera);
            //mEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetLastRect(), EditorStyles.whiteLabel);
        }

        private void DoRotate()
        {
            if (Event.current.type == EventType.MouseDown)
                mLastMousePos = Event.current.mousePosition;

            if (EditorWindow.focusedWindow.titleContent.text != "Inspector" ||
                Event.current.type != EventType.MouseDrag)
                return;

            Vector3 mousePosition = Event.current.mousePosition;

            float rotateSpeed = HierarchyData.instance.preview.rotateSpeed;

            float rotX = (mousePosition.x - mLastMousePos.x) * rotateSpeed;
            float rotY = (mousePosition.y - mLastMousePos.y) * rotateSpeed;

            camera.transform.RotateAround(Vector3.zero, camera.transform.up, rotX);
            camera.transform.RotateAround(Vector3.zero, camera.transform.right, rotY);

            mLastMousePos = mousePosition;
        }

        private void Render(EmptyFunction func)
        {
            var boundaries = GUILayoutUtility.GetLastRect();
            mPreviewRenderer.BeginPreview(boundaries, GUIStyle.none);
            func.Invoke();
            camera.Render();
            var render = mPreviewRenderer.EndPreview();
            GUI.DrawTexture(boundaries, render);
        }

        private void UpdateDirectionalLight()
        {
            if (mPreviewRenderer == null || mPreviewRenderer.lights.Length == 0)
                return;

            if (DirectionalLight == null)
                return;

            DirectionalLight.transform.eulerAngles = HierarchyData.instance.preview.lightRotation;
            DirectionalLight.intensity = HierarchyData.instance.preview.lightIntensity;
        }

        private void DrawOptions()
        {
            if (GUILayout.Button("Reset", GUILayout.Width(50)))
                ResetCameraRotation();
        }

        private void ResetCameraRotation()
        {
            camera.transform.position = mCameraPos;
            camera.transform.LookAt(Vector3.zero, Vector3.up);
        }

        private void InitScene()
        {
            if (mPreviewRenderer == null || mTarget == null)
                return;

            GameObject go = GameObject.Instantiate(mTarget);
            go.transform.position = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.AddComponent<SphereCollider>();
            mPreviewRenderer.AddSingleGO(go);

            FocusObject(go);
        }

        private void FocusObject(GameObject go)
        {
            Collider collider = go.GetComponent<SphereCollider>();
            var bounds = collider.bounds;
            Vector3 objectSizes = bounds.max - bounds.min;
            float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
            float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * camera.fieldOfView);
            float distance = HierarchyData.instance.preview.distance * objectSize / cameraView;
            distance += 0.5f * objectSize;
            camera.transform.position = bounds.center - distance * camera.transform.forward;

            this.mCameraPos = camera.transform.position;  // record it down
        }
    }
}
#endif
