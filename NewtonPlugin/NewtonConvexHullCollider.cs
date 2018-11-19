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
using System.Runtime.InteropServices;
using Newton.Internal;

namespace Newton {
    [AddComponentMenu("Newton Physics/Colliders/Convex Hull")]
    public class NewtonConvexHullCollider : NewtonCollider {
        public override dNewtonCollision Create(NewtonWorld world) {
            if (m_Mesh == null) {
                return null;
            }

            if (m_Mesh.vertices.Length < 4) {
                return null;
            }

            float[] array = new float[3 * m_Mesh.vertices.Length];
            for (int i = 0; i < m_Mesh.vertices.Length; i++) {
                array[i * 3 + 0] = m_Mesh.vertices[i].x;
                array[i * 3 + 1] = m_Mesh.vertices[i].y;
                array[i * 3 + 2] = m_Mesh.vertices[i].z;
            }

            IntPtr floatsPtr = Marshal.AllocHGlobal(array.Length * Marshal.SizeOf(typeof(float)));
            Marshal.Copy(array, 0, floatsPtr, array.Length);
            dNewtonCollision collision = new dNewtonCollisionConvexHull(world.GetWorld(), m_Mesh.vertices.Length, floatsPtr, 0.01f * (1.0f - m_Quality));
            if (collision.IsValid() == false) {
                collision.Dispose();
                collision = null;
            }
            Marshal.FreeHGlobal(floatsPtr);

            SetMaterial(collision);
            SetLayer(collision);
            return collision;
        }

        public Mesh Mesh { get { return m_Mesh; } }
        public float Quality { get { return m_Quality; } }

        #region Inspector
        [SerializeField]
        private Mesh m_Mesh;
        [SerializeField]
        private float m_Quality = 0.5f;
        #endregion
    }
}

