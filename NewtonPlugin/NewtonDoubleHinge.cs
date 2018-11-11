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
    [AddComponentMenu("Newton Physics/Joints/Double Hinge")]
    public class NewtonDoubleHinge : NewtonJoint {
        public override void InitJoint() {
            NewtonBody child = GetComponent<NewtonBody>();
            dMatrix matrix = Utils.ToMatrix(m_Pivot, m_Pin0, m_Pin1);
            IntPtr otherBody = (m_OtherBody != null) ? m_OtherBody.GetBody().GetBody() : IntPtr.Zero;
            m_Joint = new dNewtonJointDoubleHinge(matrix, child.GetBody().GetBody(), otherBody);

            Stiffness = m_Stiffness;
            EnableLimits_0 = m_EnableLimits_0;
        }

        void OnDrawGizmosSelected() {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(m_Pivot, m_Pin0.normalized * m_GizmoScale);

            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(m_Pivot, m_Pin1.normalized * m_GizmoScale);
        }

        public bool EnableLimits_0 {
            get {
                return m_EnableLimits_0;
            }
            set {
                m_EnableLimits_0 = value;
                if (m_Joint != null) {
                    dNewtonJointDoubleHinge joint = (dNewtonJointDoubleHinge)m_Joint;
                    joint.SetLimits_0(m_EnableLimits_0, m_MinLimit_0, m_MaxLimit_0);
                }
            }
        }

        public float MinimumLimit_0 {
            get {
                return m_MinLimit_0;
            }
            set {
                m_MinLimit_0 = value;
                if (m_Joint != null) {
                    dNewtonJointDoubleHinge joint = (dNewtonJointDoubleHinge)m_Joint;
                    joint.SetLimits_0(m_EnableLimits_0, m_MinLimit_0, m_MaxLimit_0);
                }
            }
        }

        public float MaximunLimit_0 {
            get {
                return m_MaxLimit_0;
            }
            set {
                m_MaxLimit_0 = value;
                if (m_Joint != null) {
                    dNewtonJointDoubleHinge joint = (dNewtonJointDoubleHinge)m_Joint;
                    joint.SetLimits_0(m_EnableLimits_0, m_MinLimit_0, m_MaxLimit_0);
                }
            }
        }


        public bool EnableLimits_1 {
            get {
                return m_EnableLimits_1;
            }
            set {
                m_EnableLimits_1 = value;
                if (m_Joint != null) {
                    dNewtonJointDoubleHinge joint = (dNewtonJointDoubleHinge)m_Joint;
                    joint.SetLimits_1(m_EnableLimits_1, m_MinLimit_1, m_MaxLimit_1);
                }
            }
        }

        public float MinimumLimit_1 {
            get {
                return m_MinLimit_1;
            }
            set {
                m_MinLimit_1 = value;
                if (m_Joint != null) {
                    dNewtonJointDoubleHinge joint = (dNewtonJointDoubleHinge)m_Joint;
                    joint.SetLimits_1(m_EnableLimits_1, m_MinLimit_1, m_MaxLimit_1);
                }
            }
        }

        public float MaximunLimit_1 {
            get {
                return m_MaxLimit_1;
            }
            set {
                m_MaxLimit_1 = value;
                if (m_Joint != null) {
                    dNewtonJointDoubleHinge joint = (dNewtonJointDoubleHinge)m_Joint;
                    joint.SetLimits_1(m_EnableLimits_1, m_MinLimit_1, m_MaxLimit_1);
                }
            }
        }

        [SerializeField]
        private Vector3 m_Pivot = Vector3.zero;
        [SerializeField]
        private Vector3 m_Pin0 = Vector3.right;
        [SerializeField]
        private Vector3 m_Pin1 = Vector3.up;
        [SerializeField]
        private bool m_EnableLimits_0 = false;
        [SerializeField]
        private float m_MinLimit_0 = -30.0f;
        [SerializeField]
        private float m_MaxLimit_0 = 30.0f;
        [SerializeField]
        private bool m_EnableLimits_1 = false;
        [SerializeField]
        private float m_MinLimit_1 = -30.0f;
        [SerializeField]
        private float m_MaxLimit_1 = 30.0f;
    }


    [AddComponentMenu("Newton Physics/Joints/Universal Actuator")]
    public class NewtonDoubleHingeActuator : NewtonJoint {
        public override void InitJoint() {
            NewtonBody child = GetComponent<NewtonBody>();
            dMatrix matrix = Utils.ToMatrix(m_Pivot, m_Pin0, m_Pin1);
            IntPtr otherBody = (m_OtherBody != null) ? m_OtherBody.GetBody().GetBody() : IntPtr.Zero;
            m_Joint = new dNewtonJointDoubleHingeActuator(matrix, child.GetBody().GetBody(), otherBody);

            TargetAngle0 = m_TargetAngle0;
            AngularRate0 = m_AngularRate0;
            MaxTorque0 = m_MaxTorque0;

            TargetAngle1 = m_TargetAngle1;
            AngularRate1 = m_AngularRate1;
            MaxTorque1 = m_MaxTorque1;
        }

        void OnDrawGizmosSelected() {
            Gizmos.matrix = transform.worldToLocalMatrix;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(m_Pivot, m_Pin0.normalized * m_GizmoScale);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(m_Pivot, m_Pin1.normalized * m_GizmoScale);
        }

        public float MaxTorque0 {
            get {
                return m_MaxTorque0;
            }
            set {
                m_MaxTorque0 = value;
                if (m_Joint != null) {
                    dNewtonJointDoubleHingeActuator joint = (dNewtonJointDoubleHingeActuator)m_Joint;
                    joint.SetMaxTorque0(m_MaxTorque0);
                }
            }
        }

        public float AngularRate0 {
            get {
                return m_AngularRate0;
            }
            set {
                m_AngularRate0 = value;
                if (m_Joint != null) {
                    dNewtonJointDoubleHingeActuator joint = (dNewtonJointDoubleHingeActuator)m_Joint;
                    joint.SetAngularRate0(m_AngularRate0);
                }
            }
        }

        public float TargetAngle0 {
            get {
                return m_TargetAngle0;
            }
            set {
                m_TargetAngle0 = value;
                if (m_Joint != null) {
                    dNewtonJointDoubleHingeActuator joint = (dNewtonJointDoubleHingeActuator)m_Joint;
                    joint.SetTargetAngle0(m_TargetAngle0, m_MinAngle0, m_MaxAngle0);
                }
            }
        }

        public float MinimumAngle0 {
            get {
                return m_MinAngle0;
            }
            set {
                m_MinAngle0 = value;
                if (m_Joint != null) {
                    dNewtonJointDoubleHingeActuator joint = (dNewtonJointDoubleHingeActuator)m_Joint;
                    joint.SetTargetAngle0(m_TargetAngle0, m_MinAngle0, m_MaxAngle0);
                }
            }
        }

        public float MaximumAngle0 {
            get {
                return m_MaxAngle0;
            }
            set {
                m_MaxAngle0 = value;
                if (m_Joint != null) {
                    dNewtonJointDoubleHingeActuator joint = (dNewtonJointDoubleHingeActuator)m_Joint;
                    joint.SetTargetAngle0(m_TargetAngle0, m_MinAngle0, m_MaxAngle0);
                }
            }
        }

        public float GetJointAngle0() {
            float angle = 0.0f;
            if (m_Joint != null) {
                dNewtonJointDoubleHingeActuator joint = (dNewtonJointDoubleHingeActuator)m_Joint;
                angle = joint.GetAngle0();
            }
            return angle;
        }

        /*
            public float GetJointSpeed0()
            {
                float angle = 0.0f;
                if (m_joint != null)
                {
                    dNewtonJointUniversalActuator joint = (dNewtonJointUniversalActuator)m_joint;
                    angle = joint.GetAngle();
                }
                return angle;
            }
        */


        public float MaxTorque1 {
            get {
                return m_MaxTorque1;
            }
            set {
                m_MaxTorque1 = value;
                if (m_Joint != null) {
                    dNewtonJointDoubleHingeActuator joint = (dNewtonJointDoubleHingeActuator)m_Joint;
                    joint.SetMaxTorque1(m_MaxTorque1);
                }
            }
        }

        public float AngularRate1 {
            get {
                return m_AngularRate1;
            }
            set {
                m_AngularRate1 = value;
                if (m_Joint != null) {
                    dNewtonJointDoubleHingeActuator joint = (dNewtonJointDoubleHingeActuator)m_Joint;
                    joint.SetAngularRate1(m_AngularRate1);
                }
            }
        }

        public float TargetAngle1 {
            get {
                return m_TargetAngle1;
            }
            set {
                m_TargetAngle1 = value;
                if (m_Joint != null) {
                    dNewtonJointDoubleHingeActuator joint = (dNewtonJointDoubleHingeActuator)m_Joint;
                    joint.SetTargetAngle1(m_TargetAngle1, m_MinAngle1, m_MaxAngle1);
                }
            }
        }

        public float MinimumAngle1 {
            get {
                return m_MinAngle1;
            }
            set {
                m_MinAngle1 = value;
                if (m_Joint != null) {
                    dNewtonJointDoubleHingeActuator joint = (dNewtonJointDoubleHingeActuator)m_Joint;
                    joint.SetTargetAngle1(m_TargetAngle1, m_MinAngle1, m_MaxAngle1);
                }
            }
        }

        public float MaximumAngle1 {
            get {
                return m_MaxAngle1;
            }
            set {
                m_MaxAngle1 = value;
                if (m_Joint != null) {
                    dNewtonJointDoubleHingeActuator joint = (dNewtonJointDoubleHingeActuator)m_Joint;
                    joint.SetTargetAngle1(m_TargetAngle1, m_MinAngle1, m_MaxAngle1);
                }
            }
        }

        public float GetJointAngle1() {
            float angle = 0.0f;
            if (m_Joint != null) {
                dNewtonJointDoubleHingeActuator joint = (dNewtonJointDoubleHingeActuator)m_Joint;
                angle = joint.GetAngle1();
            }
            return angle;
        }

        [SerializeField]
        private Vector3 m_Pivot = Vector3.zero;
        [SerializeField]
        private Vector3 m_Pin0 = Vector3.right;
        [SerializeField]
        private Vector3 m_Pin1 = Vector3.up;
        [SerializeField]
        private float m_MaxTorque0 = 10.0f;
        [SerializeField]
        private float m_AngularRate0 = 1.0f;
        [SerializeField]
        private float m_TargetAngle0 = 0.0f;
        [SerializeField]
        private float m_MinAngle0 = -360.0f;
        [SerializeField]
        private float m_MaxAngle0 = 360.0f;
        [SerializeField]
        private float m_MaxTorque1 = 10.0f;
        [SerializeField]
        private float m_AngularRate1 = 1.0f;
        [SerializeField]
        private float m_TargetAngle1 = 0.0f;
        [SerializeField]
        private float m_MinAngle1 = -360.0f;
        [SerializeField]
        private float m_MaxAngle1 = 360.0f;

    }
}



