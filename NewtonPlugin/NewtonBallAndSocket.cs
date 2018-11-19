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
    [AddComponentMenu("Newton Physics/Joints/BallAndSocket")]
    public class NewtonBallAndSocket : NewtonJoint {
        public override void InitJoint() {
            NewtonBody child = GetComponent<NewtonBody>();
            dMatrix matrix = Utils.ToMatrix(m_Pivot, m_Pin);
            IntPtr otherBody = (m_OtherBody != null) ? m_OtherBody.GetBody().GetBody() : IntPtr.Zero;
            m_Joint = new dNewtonJointBallAndSocket(matrix, child.GetBody().GetBody(), otherBody);

            Stiffness = m_Stiffness;
        }

        void OnDrawGizmosSelected() {


            Gizmos.color = Color.red;

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawRay(m_Pivot, m_Pin.normalized * m_GizmoScale);
        }

        [SerializeField]
        private Vector3 m_Pivot = Vector3.zero;
        [SerializeField]
        private Vector3 m_Pin = Vector3.right;
    }
}




