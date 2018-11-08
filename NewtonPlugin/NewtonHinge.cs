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
using System.Runtime.InteropServices;
using Newton.Internal;

namespace Newton {
    [AddComponentMenu("Newton Physics/Joints/Hinge")]
    public class NewtonHinge : NewtonJoint {
        public override void InitJoint() {
            if (m_Initialized)
                return;

            NewtonBody child = GetComponent<NewtonBody>();
            dMatrix matrix = Utils.ToMatrix(m_posit, Quaternion.Euler(m_rotation));
            IntPtr otherBody = (m_OtherBody != null) ? m_OtherBody.GetBody().GetBody() : new IntPtr(0);
            m_Joint = new dNewtonJointHinge(matrix, child.GetBody().GetBody(), otherBody);

            Stiffness = m_Stiffness;
            EnableLimits = m_enableLimits;
            SetSpringDamper = m_setSpringDamper;
            m_Initialized = true;
        }

        void OnDrawGizmosSelected() {
            Matrix4x4 bodyMatrix = Matrix4x4.identity;
            Matrix4x4 localMatrix = Matrix4x4.identity;
            bodyMatrix.SetTRS(transform.position, transform.rotation, Vector3.one);
            localMatrix.SetTRS(m_posit, Quaternion.Euler(m_rotation), Vector3.one);

            Gizmos.color = Color.red;

            Gizmos.matrix = bodyMatrix;
            Gizmos.DrawRay(m_posit, localMatrix.GetColumn(0) * m_GizmoScale);
            if (m_enableLimits) {
                // draw hinge limit
            }
        }

        public bool EnableLimits {
            get {
                return m_enableLimits;
            }
            set {
                m_enableLimits = value;
                if (m_Joint != null) {
                    dNewtonJointHinge joint = (dNewtonJointHinge)m_Joint;
                    joint.SetLimits(m_enableLimits, m_minLimit, m_maxLimit);
                }
            }
        }

        public float MinimumLimit {
            get {
                return m_minLimit;
            }
            set {
                m_minLimit = value;
                if (m_Joint != null) {
                    dNewtonJointHinge joint = (dNewtonJointHinge)m_Joint;
                    joint.SetLimits(m_enableLimits, m_minLimit, m_maxLimit);
                }
            }
        }

        public float MaximunLimit {
            get {
                return m_maxLimit;
            }
            set {
                m_maxLimit = value;
                if (m_Joint != null) {
                    dNewtonJointHinge joint = (dNewtonJointHinge)m_Joint;
                    joint.SetLimits(m_enableLimits, m_minLimit, m_maxLimit);
                }
            }
        }

        public bool SetSpringDamper {
            get {
                return m_setSpringDamper;
            }
            set {
                m_setSpringDamper = value;
                if (m_Joint != null) {
                    dNewtonJointHinge joint = (dNewtonJointHinge)m_Joint;
                    joint.SetAsSpringDamper(m_setSpringDamper, m_springDamperForceMixing, m_springConstant, m_damperConstant);
                }
            }
        }

        public float SpringDamperForceMixing {
            get {
                return m_springDamperForceMixing;
            }
            set {
                m_springDamperForceMixing = value;
                if (m_Joint != null) {
                    dNewtonJointHinge joint = (dNewtonJointHinge)m_Joint;
                    joint.SetAsSpringDamper(m_setSpringDamper, m_springDamperForceMixing, m_springConstant, m_damperConstant);
                }
            }
        }

        public float SpringConstant {
            get {
                return m_springConstant;
            }
            set {
                m_springConstant = value;
                if (m_Joint != null) {
                    dNewtonJointHinge joint = (dNewtonJointHinge)m_Joint;
                    joint.SetAsSpringDamper(m_setSpringDamper, m_springDamperForceMixing, m_springConstant, m_damperConstant);
                }
            }
        }

        public float DamperConstant {
            get {
                return m_damperConstant;
            }
            set {
                m_damperConstant = value;
                if (m_Joint != null) {
                    dNewtonJointHinge joint = (dNewtonJointHinge)m_Joint;
                    joint.SetAsSpringDamper(m_setSpringDamper, m_springDamperForceMixing, m_springConstant, m_damperConstant);
                }
            }
        }

        public Vector3 m_posit = Vector3.zero;
        public Vector3 m_rotation = Vector3.zero;
        public bool m_enableLimits = false;
        public float m_minLimit = -30.0f;
        public float m_maxLimit = 30.0f;
        public bool m_setSpringDamper = false;
        public float m_springDamperForceMixing = 0.9f;
        public float m_springConstant = 0.0f;
        public float m_damperConstant = 10.0f;
    }


    [AddComponentMenu("Newton Physics/Joints/Hinge Actuator")]
    public class NewtonHingeActuator : NewtonJoint {
        public override void InitJoint() {
            NewtonBody child = GetComponent<NewtonBody>();
            dMatrix matrix = Utils.ToMatrix(m_posit, Quaternion.Euler(m_rotation));
            IntPtr otherBody = (m_OtherBody != null) ? m_OtherBody.GetBody().GetBody() : new IntPtr(0);
            m_Joint = new dNewtonJointHingeActuator(matrix, child.GetBody().GetBody(), otherBody);

            TargetAngle = m_targetAngle;
            AngularRate = m_angularRate;
            MaxTorque = m_maxTorque;
        }

        void OnDrawGizmosSelected() {
            Matrix4x4 bodyMatrix = Matrix4x4.identity;
            Matrix4x4 localMatrix = Matrix4x4.identity;
            bodyMatrix.SetTRS(transform.position, transform.rotation, Vector3.one);
            localMatrix.SetTRS(m_posit, Quaternion.Euler(m_rotation), Vector3.one);

            Gizmos.color = Color.red;

            Gizmos.matrix = bodyMatrix;
            Gizmos.DrawRay(m_posit, localMatrix.GetColumn(0) * m_GizmoScale);
        }


        public float MaxTorque {
            get {
                return m_maxTorque;
            }
            set {
                m_maxTorque = value;
                if (m_Joint != null) {
                    dNewtonJointHingeActuator joint = (dNewtonJointHingeActuator)m_Joint;
                    joint.SetMaxToque(m_maxTorque);
                }
            }
        }

        public float AngularRate {
            get {
                return m_angularRate;
            }
            set {
                m_angularRate = value;
                if (m_Joint != null) {
                    dNewtonJointHingeActuator joint = (dNewtonJointHingeActuator)m_Joint;
                    joint.SetAngularRate(m_angularRate);
                }
            }
        }

        public float TargetAngle {
            get {
                return m_targetAngle;
            }
            set {
                m_targetAngle = value;
                if (m_Joint != null) {
                    dNewtonJointHingeActuator joint = (dNewtonJointHingeActuator)m_Joint;
                    joint.SetTargetAngle(m_targetAngle, m_minAngle, m_maxAngle);
                }
            }
        }

        public float MinimumAngle {
            get {
                return m_minAngle;
            }
            set {
                m_minAngle = value;
                if (m_Joint != null) {
                    dNewtonJointHingeActuator joint = (dNewtonJointHingeActuator)m_Joint;
                    joint.SetTargetAngle(m_targetAngle, m_minAngle, m_maxAngle);
                }
            }
        }

        public float MaximumAngle {
            get {
                return m_maxAngle;
            }
            set {
                m_maxAngle = value;
                if (m_Joint != null) {
                    dNewtonJointHingeActuator joint = (dNewtonJointHingeActuator)m_Joint;
                    joint.SetTargetAngle(m_targetAngle, m_minAngle, m_maxAngle);
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

        public Vector3 m_posit = Vector3.zero;
        public Vector3 m_rotation = Vector3.zero;
        public float m_maxTorque = 10.0f;
        public float m_angularRate = 1.0f;
        public float m_targetAngle = 0.0f;
        public float m_minAngle = -360.0f;
        public float m_maxAngle = 360.0f;
    }
}



