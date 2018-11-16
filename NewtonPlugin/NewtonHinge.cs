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
    [AddComponentMenu("Newton Physics/Joints/Hinge")]
    public class NewtonHinge : NewtonJoint {
        public override void InitJoint() {
            if (m_Initialized)
                return;

            NewtonBody child = GetComponent<NewtonBody>();
            dMatrix matrix = Utils.ToMatrix(m_Pivot, Quaternion.FromToRotation(Vector3.right, m_Pin));
            IntPtr otherBody = (m_OtherBody != null) ? m_OtherBody.GetBody().GetBody() : IntPtr.Zero;
            m_Joint = new dNewtonJointHinge(matrix, child.GetBody().GetBody(), otherBody);

            Stiffness = m_Stiffness;
            EnableCollision = m_EnableCollision;
            EnableLimits = m_EnableLimits;
            SetSpringDamper = m_SetSpringDamper;
            m_Initialized = true;
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawRay(m_Pivot, m_Pin.normalized * m_GizmoScale);
            if (m_EnableLimits) {
                // draw hinge limit
            }
        }

        public bool EnableLimits {
            get {
                return m_EnableLimits;
            }
            set {
                m_EnableLimits = value;
                if (m_Joint != null) {
                    dNewtonJointHinge joint = (dNewtonJointHinge)m_Joint;
                    joint.SetLimits(m_EnableLimits, m_MinLimit, m_MaxLimit);
                }
            }
        }

        public float MinimumLimit {
            get {
                return m_MinLimit;
            }
            set {
                m_MinLimit = value;
                if (m_Joint != null) {
                    dNewtonJointHinge joint = (dNewtonJointHinge)m_Joint;
                    joint.SetLimits(m_EnableLimits, m_MinLimit, m_MaxLimit);
                }
            }
        }

        public float MaximunLimit {
            get {
                return m_MaxLimit;
            }
            set {
                m_MaxLimit = value;
                if (m_Joint != null) {
                    dNewtonJointHinge joint = (dNewtonJointHinge)m_Joint;
                    joint.SetLimits(m_EnableLimits, m_MinLimit, m_MaxLimit);
                }
            }
        }

        public bool SetSpringDamper {
            get {
                return m_SetSpringDamper;
            }
            set {
                m_SetSpringDamper = value;
                if (m_Joint != null) {
                    dNewtonJointHinge joint = (dNewtonJointHinge)m_Joint;
                    joint.SetAsSpringDamper(m_SetSpringDamper, m_SpringDamperForceMixing, m_SpringConstant, m_DamperConstant);
                }
            }
        }

        public float SpringDamperForceMixing {
            get {
                return m_SpringDamperForceMixing;
            }
            set {
                m_SpringDamperForceMixing = value;
                if (m_Joint != null) {
                    dNewtonJointHinge joint = (dNewtonJointHinge)m_Joint;
                    joint.SetAsSpringDamper(m_SetSpringDamper, m_SpringDamperForceMixing, m_SpringConstant, m_DamperConstant);
                }
            }
        }

        public float SpringConstant {
            get {
                return m_SpringConstant;
            }
            set {
                m_SpringConstant = value;
                if (m_Joint != null) {
                    dNewtonJointHinge joint = (dNewtonJointHinge)m_Joint;
                    joint.SetAsSpringDamper(m_SetSpringDamper, m_SpringDamperForceMixing, m_SpringConstant, m_DamperConstant);
                }
            }
        }

        public float DamperConstant {
            get {
                return m_DamperConstant;
            }
            set {
                m_DamperConstant = value;
                if (m_Joint != null) {
                    dNewtonJointHinge joint = (dNewtonJointHinge)m_Joint;
                    joint.SetAsSpringDamper(m_SetSpringDamper, m_SpringDamperForceMixing, m_SpringConstant, m_DamperConstant);
                }
            }
        }

        [SerializeField]
        private Vector3 m_Pivot = Vector3.zero;
        [SerializeField]
        private Vector3 m_Pin = Vector3.right;
        [SerializeField]
        private bool m_EnableLimits = false;
        [SerializeField]
        private float m_MinLimit = -30.0f;
        [SerializeField]
        private float m_MaxLimit = 30.0f;
        [SerializeField]
        private bool m_SetSpringDamper = false;
        [SerializeField]
        private float m_SpringDamperForceMixing = 0.9f;
        [SerializeField]
        private float m_SpringConstant = 0.0f;
        [SerializeField]
        private float m_DamperConstant = 10.0f;
    }


    [AddComponentMenu("Newton Physics/Joints/Hinge Actuator")]
    public class NewtonHingeActuator : NewtonJoint {
        public override void InitJoint() {
            NewtonBody child = GetComponent<NewtonBody>();
            dMatrix matrix = Utils.ToMatrix(m_Pivot, Quaternion.FromToRotation(Vector3.right, m_Pin));
            IntPtr otherBody = (m_OtherBody != null) ? m_OtherBody.GetBody().GetBody() : IntPtr.Zero;
            m_Joint = new dNewtonJointHingeActuator(matrix, child.GetBody().GetBody(), otherBody);

            TargetAngle = m_TargetAngle;
            AngularRate = m_AngularRate;
            MaxTorque = m_MaxTorque;
            EnableCollision = m_EnableCollision;
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawRay(m_Pivot, m_Pin.normalized * m_GizmoScale);
        }


        public float MaxTorque {
            get {
                return m_MaxTorque;
            }
            set {
                m_MaxTorque = value;
                if (m_Joint != null) {
                    dNewtonJointHingeActuator joint = (dNewtonJointHingeActuator)m_Joint;
                    joint.SetMaxTorque(m_MaxTorque);
                }
            }
        }

        public float AngularRate {
            get {
                return m_AngularRate;
            }
            set {
                m_AngularRate = value;
                if (m_Joint != null) {
                    dNewtonJointHingeActuator joint = (dNewtonJointHingeActuator)m_Joint;
                    joint.SetAngularRate(m_AngularRate);
                }
            }
        }

        public float TargetAngle {
            get {
                return m_TargetAngle;
            }
            set {
                m_TargetAngle = value;
                if (m_Joint != null) {
                    dNewtonJointHingeActuator joint = (dNewtonJointHingeActuator)m_Joint;
                    joint.SetTargetAngle(m_TargetAngle, m_MinAngle, m_MaxAngle);
                }
            }
        }

        public float MinimumAngle {
            get {
                return m_MinAngle;
            }
            set {
                m_MinAngle = value;
                if (m_Joint != null) {
                    dNewtonJointHingeActuator joint = (dNewtonJointHingeActuator)m_Joint;
                    joint.SetTargetAngle(m_TargetAngle, m_MinAngle, m_MaxAngle);
                }
            }
        }

        public float MaximumAngle {
            get {
                return m_MaxAngle;
            }
            set {
                m_MaxAngle = value;
                if (m_Joint != null) {
                    dNewtonJointHingeActuator joint = (dNewtonJointHingeActuator)m_Joint;
                    joint.SetTargetAngle(m_TargetAngle, m_MinAngle, m_MaxAngle);
                }
            }
        }

        public float GetJointAngle() {
            float angle = 0.0f;
            if (m_Joint != null) {
                dNewtonJointHingeActuator joint = (dNewtonJointHingeActuator)m_Joint;
                angle = joint.GetAngle();
            }
            return angle;
        }
        /*
            public float GetJointSpeed()
            {
                float angle = 0.0f;
                if (m_joint != null)
                {
                    dNewtonJointHingeActuator joint = (dNewtonJointHingeActuator)m_joint;
                    angle = joint.GetAngle();
                }
                return angle;
            }
        */

        [SerializeField]
        private Vector3 m_Pivot = Vector3.zero;
        [SerializeField]
        private Vector3 m_Pin = Vector3.right;
        [SerializeField]
        private float m_MaxTorque = 10.0f;
        [SerializeField]
        private float m_AngularRate = 1.0f;
        [SerializeField]
        private float m_TargetAngle = 0.0f;
        [SerializeField]
        private float m_MinAngle = -360.0f;
        [SerializeField]
        private float m_MaxAngle = 360.0f;
    }
}



