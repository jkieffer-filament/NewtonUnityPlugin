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
    [AddComponentMenu("Newton Physics/Joints/Gear")]
    public class NewtonGear : NewtonJoint {
        public override void InitJoint() {
            Vector3 childPin = m_Pin.normalized;
            Vector3 parentPin = m_ParentPin.normalized;

            NewtonBody child = GetComponent<NewtonBody>();
            IntPtr otherBody = (m_OtherBody != null) ? m_OtherBody.GetBody().GetBody() : new IntPtr(0);

            dVector childPin_ = new dVector(childPin.x, childPin.y, childPin.z, 0.0f);
            dVector parentPin_ = new dVector(parentPin.x, parentPin.y, parentPin.z, 0.0f);
            m_Joint = new dNewtonJointGear(m_GearRatio, childPin_, parentPin_, child.GetBody().GetBody(), otherBody);
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = Color.cyan;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawRay(Vector3.zero, m_Pin.normalized * m_GizmoScale);
            if (m_OtherBody != null) {
                Gizmos.color = Color.cyan;
                Gizmos.matrix = m_OtherBody.transform.localToWorldMatrix;
                Gizmos.DrawRay(Vector3.zero, m_ParentPin * m_GizmoScale);
            }
        }

        [SerializeField]
        private Vector3 m_Pin = Vector3.right;
        [SerializeField]
        private Vector3 m_ParentPin = Vector3.right;
        [SerializeField]
        private float m_GearRatio = 1.0f;
    }


    [AddComponentMenu("Newton Physics/Joints/Differential Gear")]
    public class NewtonDifferentialGear : NewtonJoint {
        public override void InitJoint() {
            var childPinNorm = m_Pin.normalized;
            var parentPinNorm = m_ParentPin.normalized;
            var refPinNorm = m_ReferencePin.normalized;

            NewtonBody child = GetComponent<NewtonBody>();
            IntPtr otherBody = (m_OtherBody != null) ? m_OtherBody.GetBody().GetBody() : new IntPtr(0);
            IntPtr referenceBody = (m_ReferenceBody != null) ? m_ReferenceBody.GetBody().GetBody() : new IntPtr(0);

            dVector dChildPin = new dVector(childPinNorm.x, childPinNorm.y, childPinNorm.z, 0.0f);
            dVector dParentPin = new dVector(parentPinNorm.x, parentPinNorm.y, parentPinNorm.z, 0.0f);
            dVector dReferencePin = new dVector(refPinNorm.x, refPinNorm.y, refPinNorm.z, 0.0f);

            m_Joint = new dNewtonJointDifferentialGear(m_GearRatio, dChildPin, dParentPin, dReferencePin, child.GetBody().GetBody(), otherBody, referenceBody);
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = Color.cyan;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawRay(Vector3.zero, m_Pin.normalized * m_GizmoScale);
            if (m_OtherBody != null) {
                Gizmos.color = Color.cyan;
                Gizmos.matrix = m_OtherBody.transform.localToWorldMatrix;
                Gizmos.DrawRay(Vector3.zero, m_ParentPin.normalized * m_GizmoScale);
            }

            if (m_ReferenceBody != null) {
                Gizmos.color = Color.yellow;
                Gizmos.matrix = m_ReferenceBody.transform.localToWorldMatrix;
                Gizmos.DrawRay(Vector3.zero, m_ReferencePin.normalized * m_GizmoScale);
            }
        }

        [SerializeField]
        private NewtonBody m_ReferenceBody = null;
        [SerializeField]
        private Vector3 m_Pin = Vector3.right;
        [SerializeField]
        private Vector3 m_ParentPin = Vector3.right;
        [SerializeField]
        private Vector3 m_ReferencePin = Vector3.right;
        [SerializeField]
        private float m_GearRatio = 1.0f;
    }

    [AddComponentMenu("Newton Physics/Joints/Rack And Pinion")]
    public class NewtonRackAndPinion : NewtonJoint {
        public override void InitJoint() {
            NewtonBody child = GetComponent<NewtonBody>();
            IntPtr otherBody = (m_OtherBody != null) ? m_OtherBody.GetBody().GetBody() : new IntPtr(0);

            var gearPinNorm = m_GearPin.normalized;
            var slidePinNorm = m_SlidePin.normalized;

            dVector childPin = new dVector(gearPinNorm.x, gearPinNorm.y, gearPinNorm.z, 0.0f);
            dVector parentPin = new dVector(slidePinNorm.x, slidePinNorm.y, slidePinNorm.z, 0.0f);
            m_Joint = new dNewtonJointRackAndPinion(m_GearRatio, childPin, parentPin, child.GetBody().GetBody(), otherBody);
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = Color.cyan;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawRay(Vector3.zero, m_GearPin.normalized * m_GizmoScale);
            if (m_OtherBody != null) {
                Gizmos.color = Color.cyan;
                Gizmos.matrix = m_OtherBody.transform.localToWorldMatrix;
                Gizmos.DrawRay(Vector3.zero, m_SlidePin.normalized * m_GizmoScale);
            }
        }

        [SerializeField]
        private Vector3 m_GearPin = Vector3.right;
        [SerializeField]
        private Vector3 m_SlidePin = Vector3.right;
        [SerializeField]
        private float m_GearRatio = 1.0f;
    }
}

