/*
* This software is provided 'as-is', without any express or implied
* warranty. In no event will the authors be held liable for any damages
* arising from the use of this software.
* 
* Permission is granted to anyone to use this software for any purpose,
* including commercial applications, and to alter it and redistribute it
* freely, subject to the following restrictions:
* 
* 1. The origin of this software must not be misrepresented; you must not
* claim that you wrote the original software. If you use this software
* in a product, an acknowledgment in the product documentation would be
* appreciated but is not required.
* 
* 2. Altered source versions must be plainly marked as such, and must not be
* misrepresented as being the original software.
* 
* 3. This notice may not be removed or altered from any source distribution.
*/

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newton.Internal;

namespace Newton {

    public delegate void OnDrawFaceCallback(IntPtr points, int vertexCount);

    abstract public class NewtonCollider : MonoBehaviour {
        abstract public dNewtonCollision Create(NewtonWorld world);

        public void OnDrawFace(IntPtr vertexDataPtr, int vertexCount) {
            Marshal.Copy(vertexDataPtr, s_DebugDisplayVertexBuffer, 0, vertexCount * 3);
            int i0 = vertexCount - 1;
            for (int i1 = 0; i1 < vertexCount; i1++) {
                s_LineP0.x = s_DebugDisplayVertexBuffer[i0 * 3 + 0];
                s_LineP0.y = s_DebugDisplayVertexBuffer[i0 * 3 + 1];
                s_LineP0.z = s_DebugDisplayVertexBuffer[i0 * 3 + 2];
                s_LineP1.x = s_DebugDisplayVertexBuffer[i1 * 3 + 0];
                s_LineP1.y = s_DebugDisplayVertexBuffer[i1 * 3 + 1];
                s_LineP1.z = s_DebugDisplayVertexBuffer[i1 * 3 + 2];
                Gizmos.DrawLine(s_LineP0, s_LineP1);
                i0 = i1;
            }
        }

        public virtual void OnDrawGizmosSelected() {
            if (m_ShowGizmo) {

                ValidateEditorShape();
                if (m_EditorShape != null) {
                    UpdateParams(m_EditorShape);

                    Transform bodyTransform = transform;
                    while ((bodyTransform != null) && (bodyTransform.gameObject.GetComponent<NewtonBody>() == null)) {
                        bodyTransform = bodyTransform.parent;
                    }

                    if (bodyTransform != null) {
                        Gizmos.matrix = Matrix4x4.TRS(bodyTransform.position, bodyTransform.rotation, Vector3.one);
                        Gizmos.color = Color.yellow;

                        Camera camera = Camera.current;
                        Matrix4x4 matrix = Matrix4x4.Inverse(camera.worldToCameraMatrix * Gizmos.matrix);
                        Vector4 eyepoint = matrix.GetColumn(3);
                        m_EditorShape.DebugRender(OnDrawFace, new dVector(eyepoint.x, eyepoint.y, eyepoint.z));
                    }
                }
            }
        }

        public void SetMaterial(dNewtonCollision shape) {
            int materialID = m_Material ? m_Material.GetInstanceID() : 0;
            shape.SetMaterialID(materialID);
            shape.SetAsTrigger(m_IsTrigger);
        }

        public void SetLayer(dNewtonCollision shape) {
            shape.SetLayer(m_Layer);
        }

        public virtual bool IsStatic() {
            return false;
        }

        public virtual Vector3 GetScale() {
            Vector3 scale = m_Scale;
            if (m_InheritTransformScale) {
                var lossyScale = transform.lossyScale;
                //scale.x *= transform.localScale.x;
                //scale.y *= transform.localScale.y;
                //scale.z *= transform.localScale.z;
                scale.x *= lossyScale.x;
                scale.y *= lossyScale.y;
                scale.z *= lossyScale.z;
            }
            return scale;
        }

        public void RecreateEditorShape() {
            if (m_EditorShape != null) {
                m_EditorShape.Dispose();
                m_EditorShape = null;
                UpdateEditorParams();
            }
        }

        public void UpdateEditorParams() {
            ValidateEditorShape();
            if (m_EditorShape != null) {
                UpdateParams(m_EditorShape);
            }
        }

        public dNewtonCollision CreateBodyShape(NewtonWorld world) {
            dNewtonCollision shape = Create(world);
            if (shape != null) {
                UpdateParams(shape);
            }
            return shape;
        }

        // these are all privates 
        private void UpdateParams(dNewtonCollision shape) {
            Vector3 scale = GetScale();
            shape.SetScale(scale.x, scale.y, scale.z);

            dMatrix matrix = Utils.ToMatrix(m_Position, Quaternion.Euler(m_Rotation));
            if (transform.gameObject.GetComponent<NewtonBody>() == null) {
                matrix = matrix.matrixMultiply(Utils.ToMatrix(transform.position, transform.rotation));
                Transform bodyTransform = transform.parent;
                while ((bodyTransform != null) && (bodyTransform.gameObject.GetComponent<NewtonBody>() == null)) {
                    bodyTransform = bodyTransform.parent;
                }

                if (bodyTransform != null) {
                    dMatrix bodyMatrix = Utils.ToMatrix(bodyTransform.position, bodyTransform.rotation);
                    matrix = matrix.matrixMultiply(bodyMatrix.Inverse());
                }
            }
            shape.SetMatrix(matrix);
        }

        private NewtonBody GetBodyInParent() {
            NewtonBody body = null;
            Transform gameTransform = transform;
            while (gameTransform != null) {
                // this is a child body we need to find the root rigid body owning the shape
                if (body == null) {
                    body = gameTransform.gameObject.GetComponent<NewtonBody>();
                }
                gameTransform = gameTransform.parent;
            }
            return body;
        }

        private void ValidateEditorShape() {
            if (m_EditorShape == null) {
                NewtonBody body = GetBodyInParent();

                if (body != null) {
                    if (body.World != null) {
                        m_EditorShape = Create(body.World);
                    }
                }
            }
        }

        private void OnTransformParentChanged() {
            var newBody = GetBodyInParent();

            if (!ReferenceEquals(newBody, Body)) {
                if (!ReferenceEquals(Body, null)) {
                    Body.m_Collision.RemoveCollider(this);
                }

                Body = newBody;

                if (!ReferenceEquals(Body, null)) {
                    UpdateParams(m_Shape);
                    Body.m_Collision.AddCollider(this);
                }
            }
        }

        internal dNewtonCollision GetShape() {
            return m_Shape;
        }

        internal void SetShape(dNewtonCollision shape) {
            if (m_Shape != null) {
                throw new Exception("Collision shape already set for collider.");
            }

            m_Shape = shape;
            var handle = GCHandle.Alloc(this);
            m_Shape.SetUserData(GCHandle.ToIntPtr(handle));
        }

        internal void DestroyShape() {
            if (m_Shape != null) {
                var handle = GCHandle.FromIntPtr(m_Shape.GetUserData());
                handle.Free();

                m_Shape.Dispose();
                m_Shape = null;
            }
        }

        public NewtonBody Body { get; internal set; }

        

        protected dNewtonCollision m_Shape;
        private dNewtonCollision m_EditorShape = null;
        public NewtonMaterial m_Material = null;
        public int m_Layer;
        public Vector3 m_Position = Vector3.zero;
        public Vector3 m_Rotation = Vector3.zero;
        public Vector3 m_Scale = Vector3.one;
        public bool m_IsTrigger = false;
        public bool m_ShowGizmo = true;
        public bool m_InheritTransformScale = true;

        internal IntPtr m_ParentHandle;

        // Reuse the same buffer for debug display
        static private Vector3 s_LineP0 = Vector3.zero;
        static private Vector3 s_LineP1 = Vector3.zero;
        static private float[] s_DebugDisplayVertexBuffer = new float[64 * 3];
    }
}
