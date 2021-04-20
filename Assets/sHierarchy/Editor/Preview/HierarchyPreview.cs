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
using UnityEngine;

namespace sHierarchy
{
    /// <summary>
    /// Preview selected GameObject in Hierarchy.
    /// </summary>
    [CustomPreview(typeof(GameObject))]
    public class HierarchyPreview : ObjectPreview
    {
        private GameObject mTarget;
        private Editor mEditor;

        private PreviewRenderUtility mPreviewRenderer;

        private Vector2 mLastMousePos = Vector2.zero;

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
            GetSelected();
            InitEditor();
            InitRenderer();
            DoRotate();
            DrawSelected();
        }

        private void GetSelected()
        {
            if (mTarget == Selection.activeGameObject)
                return;

            if (mEditor != null) UnityEngine.Object.DestroyImmediate(mEditor);
            mTarget = Selection.activeGameObject;
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
                mPreviewRenderer.camera.clearFlags = CameraClearFlags.SolidColor;
                mPreviewRenderer.camera.transform.position = new Vector3(0, 0, -10);
                mPreviewRenderer.camera.farClipPlane = 10000;  // Just set far plane to very far
            }

            UpdateDirectionalLight();
        }

        private void DrawSelected()
        {
            if (mTarget == null)
                return;

            var meshFilter = mTarget.GetComponent<MeshFilter>();
            var meshRenderer = mTarget.GetComponent<MeshRenderer>();

            if (meshFilter == null || meshRenderer == null)
                return;

            DrawSelectedMesh(meshFilter.sharedMesh, meshRenderer.sharedMaterial);
            mEditor.Repaint();

            //mEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetLastRect(), EditorStyles.whiteLabel);
        }

        private void DrawSelectedMesh(Mesh mesh, Material material)
        {
            Render(() => 
            {
                mPreviewRenderer.DrawMesh(mesh, Matrix4x4.identity, material, 0);
            });
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

            Camera cam = mPreviewRenderer.camera;

            cam.transform.RotateAround(Vector3.zero, cam.transform.up, rotX);
            cam.transform.RotateAround(Vector3.zero, cam.transform.right, rotY);

            mLastMousePos = mousePosition;
        }

        private void Render(EmptyFunction func)
        {
            var boundaries = GUILayoutUtility.GetLastRect();
            mPreviewRenderer.BeginPreview(boundaries, GUIStyle.none);
            func.Invoke();
            mPreviewRenderer.camera.Render();
            var render = mPreviewRenderer.EndPreview();
            GUI.DrawTexture(boundaries, render);
        }

        private void UpdateDirectionalLight()
        {
            if (mPreviewRenderer == null || mPreviewRenderer.lights.Length == 0)
                return;

            Light directional = mPreviewRenderer.lights[0];

            if (directional == null)
                return;

            directional.transform.eulerAngles = HierarchyData.instance.preview.lightRotation;
            directional.intensity = HierarchyData.instance.preview.lightIntensity;
        }
    }
}
#endif
