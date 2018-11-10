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

    [AddComponentMenu("Newton Physics/Joints/Slider")]
    public class NewtonSlider : NewtonJoint {
        public override void InitJoint() {
            NewtonBody child = GetComponent<NewtonBody>();
            dMatrix matrix = Utils.ToMatrix(m_Pivot, Quaternion.FromToRotation(Vector3.right, m_Pin));
            IntPtr otherBody = (m_OtherBody != null) ? m_OtherBody.GetBody().GetBody() : new IntPtr(0);
            m_Joint = new dNewtonJointSlider(matrix, child.GetBody().GetBody(), otherBody);

            Stiffness = m_Stiffness;
            EnableLimits = m_EnableLimits;
            SetSpringDamper = m_SetSpringDamper;
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;

            Gizmos.matrix = transform.localToWorldMatrix;
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
                    dNewtonJointSlider joint = (dNewtonJointSlider)m_Joint;
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
                    dNewtonJointSlider joint = (dNewtonJointSlider)m_Joint;
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
                    dNewtonJointSlider joint = (dNewtonJointSlider)m_Joint;
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
                    dNewtonJointSlider joint = (dNewtonJointSlider)m_Joint;
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
                    dNewtonJointSlider joint = (dNewtonJointSlider)m_Joint;
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
                    dNewtonJointSlider joint = (dNewtonJointSlider)m_Joint;
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
                    dNewtonJointSlider joint = (dNewtonJointSlider)m_Joint;
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
        private float m_MinLimit = -1.0f;
        [SerializeField]
        private float m_MaxLimit = 1.0f;
        [SerializeField]
        private bool m_SetSpringDamper = false;
        [SerializeField]
        private float m_SpringDamperForceMixing = 0.9f;
        [SerializeField]
        private float m_SpringConstant = 0.0f;
        [SerializeField]
        private float m_DamperConstant = 10.0f;
    }


    [AddComponentMenu("Newton Physics/Joints/Slider Actuator")]
    public class NewtonSliderActuator : NewtonJoint {
        public override void InitJoint() {
            NewtonBody child = GetComponent<NewtonBody>();
            dMatrix matrix = Utils.ToMatrix(m_Pivot, Quaternion.FromToRotation(Vector3.right, m_Pin));
            IntPtr otherBody = (m_OtherBody != null) ? m_OtherBody.GetBody().GetBody() : new IntPtr(0);
            m_Joint = new dNewtonJointSliderActuator(matrix, child.GetBody().GetBody(), otherBody);

            Speed = m_Speed;
            MaxForce = m_MaxForce;
            TargetPosition = m_TargetPosition;
        }

        void OnDrawGizmosSelected() {

            Gizmos.color = Color.red;

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawRay(m_Pivot, m_Pin.normalized * m_GizmoScale);
        }


        public float MaxForce {
            get {
                return m_MaxForce;
            }
            set {
                m_MaxForce = value;
                if (m_Joint != null) {
                    dNewtonJointSliderActuator joint = (dNewtonJointSliderActuator)m_Joint;
                    joint.SetMaxForce(m_MaxForce);
                }
            }
        }

        public float Speed {
            get {
                return m_Speed;
            }
            set {
                m_Speed = value;
                if (m_Joint != null) {
                    dNewtonJointSliderActuator joint = (dNewtonJointSliderActuator)m_Joint;
                    joint.SetSpeed(m_Speed);
                }
            }
        }

        public float TargetPosition {
            get {
                return m_TargetPosition;
            }
            set {
                m_TargetPosition = value;
                if (m_Joint != null) {
                    dNewtonJointSliderActuator joint = (dNewtonJointSliderActuator)m_Joint;
                    joint.SetTargetPosition(m_TargetPosition, m_MinPosition, m_MaxPosition);
                }
            }
        }

        public float MinimumPosition {
            get {
                return m_MinPosition;
            }
            set {
                m_MinPosition = value;
                if (m_Joint != null) {
                    dNewtonJointSliderActuator joint = (dNewtonJointSliderActuator)m_Joint;
                    joint.SetTargetPosition(m_TargetPosition, m_MinPosition, m_MaxPosition);
                }
            }
        }

        public float MaximumPosition {
            get {
                return m_MaxPosition;
            }
            set {
                m_MaxPosition = value;
                if (m_Joint != null) {
                    dNewtonJointSliderActuator joint = (dNewtonJointSliderActuator)m_Joint;
                    joint.SetTargetPosition(m_TargetPosition, m_MinPosition, m_MaxPosition);
                }
            }
        }

        public float GetPosition() {
            float position = 0.0f;
            if (m_Joint != null) {
                dNewtonJointSliderActuator joint = (dNewtonJointSliderActuator)m_Joint;
                position = joint.GetPosition();
            }
            return position;
        }

        /*
            public float GetJointSpeed()
            {
                float speed = 0.0f;
                if (m_joint != null)
                {
                    dNewtonJointSliderActuator joint = (dNewtonJointSliderActuator)m_joint;
                    speed = joint.GetSpeed();
                }
                return speed;
            }
        */

        [SerializeField]
        private Vector3 m_Pivot = Vector3.zero;
        [SerializeField]
        private Vector3 m_Pin = Vector3.right;
        [SerializeField]
        private float m_MaxForce = 10.0f;
        [SerializeField]
        private float m_Speed = 1.0f;
        [SerializeField]
        private float m_TargetPosition = 0.0f;
        [SerializeField]
        private float m_MinPosition = -1.0f;
        [SerializeField]
        private float m_MaxPosition = 1.0f;
    }
}



