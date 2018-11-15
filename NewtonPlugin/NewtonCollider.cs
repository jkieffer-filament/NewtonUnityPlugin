﻿/*
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
            Marshal.Copy(vertexDataPtr, m_debugDisplayVertexBuffer, 0, vertexCount * 3);
            int i0 = vertexCount - 1;
            for (int i1 = 0; i1 < vertexCount; i1++) {
                m_lineP0.x = m_debugDisplayVertexBuffer[i0 * 3 + 0];
                m_lineP0.y = m_debugDisplayVertexBuffer[i0 * 3 + 1];
                m_lineP0.z = m_debugDisplayVertexBuffer[i0 * 3 + 2];
                m_lineP1.x = m_debugDisplayVertexBuffer[i1 * 3 + 0];
                m_lineP1.y = m_debugDisplayVertexBuffer[i1 * 3 + 1];
                m_lineP1.z = m_debugDisplayVertexBuffer[i1 * 3 + 2];
                Gizmos.DrawLine(m_lineP0, m_lineP1);
                i0 = i1;
            }
        }

        public virtual void OnDrawGizmosSelected() {
            if (m_showGizmo) {

                ValidateEditorShape();
                if (m_editorShape != null) {
                    UpdateParams(m_editorShape);

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
                        m_editorShape.DebugRender(OnDrawFace, new dVector(eyepoint.x, eyepoint.y, eyepoint.z));
                    }
                }
            }
        }

        public void SetMaterial(dNewtonCollision shape) {
            int materialID = m_material ? m_material.GetInstanceID() : 0;
            shape.SetMaterialID(materialID);
            shape.SetAsTrigger(m_isTrigger);
        }

        public void SetLayer(dNewtonCollision shape) {
            shape.SetLayer(m_layer);
        }

        public virtual bool IsStatic() {
            return false;
        }

        public virtual Vector3 GetScale() {
            Vector3 scale = m_scale;
            if (m_inheritTransformScale) {
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
            if (m_editorShape != null) {
                m_editorShape.Dispose();
                m_editorShape = null;
                UpdateEditorParams();
            }
        }

        public void UpdateEditorParams() {
            ValidateEditorShape();
            if (m_editorShape != null) {
                UpdateParams(m_editorShape);
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

            dMatrix matrix = Utils.ToMatrix(m_posit, Quaternion.Euler(m_rotation));
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
            if (m_editorShape == null) {
                NewtonBody body = GetBodyInParent();

                if (body != null) {
                    if (body.World != null) {
                        m_editorShape = Create(body.World);
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
                    Body.m_Collision.AddCollider(this);
                }
            }
        }

        internal dNewtonCollision GetShape() {
            return m_Shape;
        }

        internal void SetShape(dNewtonCollision shape) {
            m_Shape = shape;
        }

        internal void DestroyShape() {
            m_Shape.Dispose();
            m_Shape = null;
        }

        public NewtonBody Body { get; internal set; }

        

        private dNewtonCollision m_Shape;
        private dNewtonCollision m_editorShape = null;
        public NewtonMaterial m_material = null;
        public LayerMask m_layer;
        public Vector3 m_posit = Vector3.zero;
        public Vector3 m_rotation = Vector3.zero;
        public Vector3 m_scale = Vector3.one;
        public bool m_isTrigger = false;
        public bool m_showGizmo = true;
        public bool m_inheritTransformScale = true;

        internal IntPtr m_ParentHandle;

        // Reuse the same buffer for debug display
        static Vector3 m_lineP0 = Vector3.zero;
        static Vector3 m_lineP1 = Vector3.zero;
        static float[] m_debugDisplayVertexBuffer = new float[64 * 3];
    }
}
