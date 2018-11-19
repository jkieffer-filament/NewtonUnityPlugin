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
    [AddComponentMenu("Newton Physics/Joints/Corkscrew")]
    public class NewtonJointCorkscrew : NewtonJoint {
        public override void InitJoint() {
            NewtonBody child = GetComponent<NewtonBody>();

            dMatrix matrix = Utils.ToMatrix(m_Pivot, m_Pin);
            IntPtr otherBody = (m_OtherBody != null) ? m_OtherBody.GetBody().GetBody() : IntPtr.Zero;
            m_Joint = new dNewtonJointCorkscrew(matrix, child.GetBody().GetBody(), otherBody);

            Stiffness = m_Stiffness;
            EnableCollision = m_EnableCollision;
            EnableLimits = m_EnableLimits;
            SetSpringDamper = m_SetSpringDamper;
        }

        void OnDrawGizmosSelected() {


            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(m_Pivot, m_Pin.normalized * m_GizmoScale);
        }

        public bool EnableLimits {
            get {
                return m_EnableLimits;
            }
            set {
                m_EnableLimits = value;
                if (m_Joint != null) {
                    dNewtonJointCorkscrew joint = (dNewtonJointCorkscrew)m_Joint;
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
                    dNewtonJointCorkscrew joint = (dNewtonJointCorkscrew)m_Joint;
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
                    dNewtonJointCorkscrew joint = (dNewtonJointCorkscrew)m_Joint;
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
                    dNewtonJointCorkscrew joint = (dNewtonJointCorkscrew)m_Joint;
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
                    dNewtonJointCorkscrew joint = (dNewtonJointCorkscrew)m_Joint;
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
                    dNewtonJointCorkscrew joint = (dNewtonJointCorkscrew)m_Joint;
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
                    dNewtonJointCorkscrew joint = (dNewtonJointCorkscrew)m_Joint;
                    joint.SetAsSpringDamper(m_SetSpringDamper, m_SpringDamperForceMixing, m_SpringConstant, m_DamperConstant);
                }
            }
        }


        public bool SetAngularSpringDamper {
            get {
                return m_SetAngularSpringDamper;
            }
            set {
                m_SetAngularSpringDamper = value;
                if (m_Joint != null) {
                    dNewtonJointCorkscrew joint = (dNewtonJointCorkscrew)m_Joint;
                    joint.SetAsAngularSpringDamper(m_SetAngularSpringDamper, m_AngularSpringDamperForceMixing, m_AngularSpringConstant, m_AngularDamperConstant);
                }
            }
        }

        public float AngularSpringDamperForceMixing {
            get {
                return m_AngularSpringDamperForceMixing;
            }
            set {
                m_AngularSpringDamperForceMixing = value;
                if (m_Joint != null) {
                    dNewtonJointCorkscrew joint = (dNewtonJointCorkscrew)m_Joint;
                    joint.SetAsAngularSpringDamper(m_SetAngularSpringDamper, m_AngularSpringDamperForceMixing, m_AngularSpringConstant, m_AngularDamperConstant);
                }
            }
        }

        public float AngularSpringConstant {
            get {
                return m_AngularSpringConstant;
            }
            set {
                m_AngularSpringConstant = value;
                if (m_Joint != null) {
                    dNewtonJointCorkscrew joint = (dNewtonJointCorkscrew)m_Joint;
                    joint.SetAsAngularSpringDamper(m_SetAngularSpringDamper, m_AngularSpringDamperForceMixing, m_AngularSpringConstant, m_AngularDamperConstant);
                }
            }
        }

        public float AngularDamperConstant {
            get {
                return m_AngularDamperConstant;
            }
            set {
                m_AngularDamperConstant = value;
                if (m_Joint != null) {
                    dNewtonJointCorkscrew joint = (dNewtonJointCorkscrew)m_Joint;
                    joint.SetAsAngularSpringDamper(m_SetAngularSpringDamper, m_AngularSpringDamperForceMixing, m_AngularSpringConstant, m_AngularDamperConstant);
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

        [SerializeField]
        private bool m_SetAngularSpringDamper = false;
        [SerializeField]
        private float m_AngularSpringDamperForceMixing = 0.9f;
        [SerializeField]
        private float m_AngularSpringConstant = 0.0f;
        [SerializeField]
        private float m_AngularDamperConstant = 10.0f;
    }
}


