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
    [DisallowMultipleComponent]
    [AddComponentMenu("Newton Physics/Rigid Body")]
    public class NewtonBody : MonoBehaviour {
        private void Awake() {
            m_CachedTransform = transform;
            World = NewtonWorld.Current;

            if (ReferenceEquals(World, null)) {
                throw new NullReferenceException("Newton world is null.");
            }
            var scripts = GetComponents<NewtonBodyScript>();
            foreach (var script in scripts) {
                m_scripts.Add(script);
            }

            InitRigidBody();
        }

        protected virtual void OnDestroy() {
            // Destroy native body
            DestroyRigidBody();
        }

        protected virtual void OnEnable() {
            if (World) {
                World.RegisterBody(this);
                m_Body.SetFreezeState(false);
            }
        }

        protected virtual void OnDisable() {
            if (World) {
                World.DeregisterBody(this);
                m_Body.SetFreezeState(true);
            }
        }

        // Update is called once per frame
        public virtual void OnUpdateTranform() {
            if (!m_Body.GetSleepState()) {
                IntPtr positionPtr = m_Body.GetInterpolatedPosition();
                IntPtr rotationPtr = m_Body.GetInterpolatedRotation();
                Marshal.Copy(positionPtr, m_PositionPtr, 0, 3);
                Marshal.Copy(rotationPtr, m_RotationPtr, 0, 4);
                m_CachedTransform.SetPositionAndRotation(new Vector3(m_PositionPtr[0], m_PositionPtr[1], m_PositionPtr[2]), new Quaternion(m_RotationPtr[1], m_RotationPtr[2], m_RotationPtr[3], m_RotationPtr[0]));

            }
        }

        void OnDrawGizmosSelected() {
            if (m_ShowGizmo) {
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

                Gizmos.color = Color.red;
                Gizmos.DrawRay(m_CenterOfMass, Vector3.right * m_GizmoScale);

                Gizmos.color = Color.green;
                Gizmos.DrawRay(m_CenterOfMass, Vector3.up * m_GizmoScale);

                Gizmos.color = Color.blue;
                Gizmos.DrawRay(m_CenterOfMass, Vector3.forward * m_GizmoScale);
            }
        }

        public virtual void InitRigidBody() {
            if (initialized)
                return;

            CreateBodyAndCollision();

            ResetCenterOfMass();

            m_Body.SetLinearDamping(m_LinearDamping);
            m_Body.SetAngularDamping(m_AngularDamping.x, m_AngularDamping.y, m_AngularDamping.z);

            var handle = GCHandle.Alloc(this);
            m_Body.SetUserData(GCHandle.ToIntPtr(handle));

            initialized = true;
        }

        public void ResetCenterOfMass() {
            m_Body.SetCenterOfMass(m_CenterOfMass.x, m_CenterOfMass.y, m_CenterOfMass.z, m_Ixx, m_Iyy, m_Izz, m_CalculateInertia);
        }

        public virtual void DestroyRigidBody() {
            if (m_Body != null) {
                var handle = GCHandle.FromIntPtr(m_Body.GetUserData());
                handle.Free();

                m_Body.Dispose();
                m_Body = null;
            }

            if (m_Collision != null) {
                m_Collision.Destroy();
                m_Collision = null;
            }
        }

        internal dNewtonBody GetBody() {
            if (World.GetWorld() == null) { throw new NullReferenceException("Native world instance is null. The World component was probably destroyed"); }
            if (!initialized) {
                InitRigidBody();
            }
            return m_Body;
        }

        public void CalculateBuoyancyForces(Vector4 plane, ref Vector3 force, ref Vector3 torque, float bodyDensity) {
            if (m_Body != null) {
                IntPtr planePtr = Marshal.AllocHGlobal(Marshal.SizeOf(plane));
                IntPtr forcePtr = Marshal.AllocHGlobal(Marshal.SizeOf(force));
                IntPtr torquePtr = Marshal.AllocHGlobal(Marshal.SizeOf(torque));

                Marshal.StructureToPtr(plane, planePtr, false);

                m_Body.CalculateBuoyancyForces(planePtr, forcePtr, torquePtr, bodyDensity);

                force = (Vector3)Marshal.PtrToStructure(forcePtr, typeof(Vector3));
                torque = (Vector3)Marshal.PtrToStructure(torquePtr, typeof(Vector3));

                Marshal.FreeHGlobal(planePtr);
                Marshal.FreeHGlobal(forcePtr);
                Marshal.FreeHGlobal(torquePtr);
            }
        }

        public void ApplyExternaForces() {
            // Apply force & torque accumulators
            if (m_ForceAcc.sqrMagnitude > 0) {
                m_Body.AddForce(m_ForceAcc.x, m_ForceAcc.y, m_ForceAcc.z);
            }
            if (m_TorqueAcc.sqrMagnitude > 0) {
                m_Body.AddTorque(m_TorqueAcc.x, m_TorqueAcc.y, m_TorqueAcc.z);
            }
            m_ForceAcc = Vector3.zero;
            m_TorqueAcc = Vector3.zero;
        }

        public Vector3 Position {
            get {
                if (m_Body != null) {
                    IntPtr positionPtr = m_Body.GetPosition();
                    Marshal.Copy(positionPtr, m_PositionPtr, 0, 3);
                    return new Vector3(m_PositionPtr[0], m_PositionPtr[1], m_PositionPtr[2]);
                }
                return Vector3.zero;
            }

            set {
                if (m_Body != null) {
                    m_Body.SetPosition(value.x, value.y, value.z);
                }
            }

        }

        public Quaternion Rotation {
            get {
                if (m_Body != null) {
                    IntPtr rotationPtr = m_Body.GetRotation();
                    Marshal.Copy(rotationPtr, m_RotationPtr, 0, 4);
                    return new Quaternion(m_RotationPtr[1], m_RotationPtr[2], m_RotationPtr[3], m_RotationPtr[0]);
                }
                return Quaternion.identity;
            }

            set {
                if (m_Body != null) {
                    m_Body.SetRotation(value.z, value.w, value.x, value.y);
                }
            }
        }

        public Vector3 Velocity {
            get {
                if (m_Body != null) {
                    IntPtr velPtr = m_Body.GetVelocity();
                    Marshal.Copy(velPtr, m_Vec3Ptr, 0, 3);
                    return new Vector3(m_Vec3Ptr[0], m_Vec3Ptr[1], m_Vec3Ptr[2]);
                }
                return Vector3.zero;
            }

            set {
                if (m_Body != null) {
                    m_Body.SetVelocity(value.x, value.y, value.z);
                }
            }

        }

        public Vector3 Omega {
            get {
                if (m_Body != null) {
                    IntPtr omgPtr = m_Body.GetOmega();
                    Marshal.Copy(omgPtr, m_Vec3Ptr, 0, 3);
                    return new Vector3(m_Vec3Ptr[0], m_Vec3Ptr[1], m_Vec3Ptr[2]);
                }
                return Vector3.zero;
            }

            set {
                if (m_Body != null) {
                    m_Body.SetOmega(value.x, value.y, value.z);
                }
            }

        }

        public float Mass {
            get { return m_Mass; }
            set {
                m_Mass = value;
                if (m_Body != null) {
                    m_Body.SetMass(value);
                }
            }
        }

        public Vector3 CenterOfMass {
            get {
                if (m_Body != null) {
                    IntPtr comPtr = m_Body.GetCenterOfMass();
                    Marshal.Copy(comPtr, m_Vec3Ptr, 0, 3);
                    return new Vector3(m_Vec3Ptr[0], m_Vec3Ptr[1], m_Vec3Ptr[2]);
                }
                return Vector3.zero;
            }

            set {
                if (m_Body != null) {
                    m_Body.SetCenterOfMass(value.x, value.y, value.z, m_Ixx, m_Iyy, m_Izz, m_CalculateInertia);
                }
            }

        }

        public float LinearDamping {
            get {
                if (m_Body != null) {
                    m_LinearDamping = m_Body.GetLinearDamping();
                }
                return m_LinearDamping;
            }

            set {
                m_LinearDamping = value;
                if (m_Body != null) {
                    m_Body.SetLinearDamping(value);
                }
            }
        }

        public Vector3 AngularDamping {
            get {
                if (m_Body != null) {
                    IntPtr dampingPtr = m_Body.GetAngularDamping();
                    Marshal.Copy(dampingPtr, m_Vec3Ptr, 0, 3);
                    m_AngularDamping = new Vector3(m_Vec3Ptr[0], m_Vec3Ptr[1], m_Vec3Ptr[2]);
                }
                return m_AngularDamping;
            }

            set {
                m_AngularDamping = new Vector3(value.x, value.y, value.z);
                if (m_Body != null) {
                    m_Body.SetAngularDamping(value.x, value.y, value.z);
                }
            }
        }

        public bool SleepState {
            get {
                if (m_Body != null)
                    return m_Body.GetSleepState();

                return false;
            }
            set {
                if (m_Body != null) {
                    m_Body.SetSleepState(value);
                }
            }
        }

        protected virtual void CreateBodyAndCollision() {
            if (m_Collision == null && m_Body == null) {
                m_Collision = new NewtonBodyCollision(this);
                m_Body = new dNewtonDynamicBody(World.GetWorld(), m_Collision.GetShape(), Utils.ToMatrix(m_CachedTransform.position, m_CachedTransform.rotation), m_Mass);
            }
        }

        public NewtonWorld World { get; private set; }

        #region Inspector
        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("m_mass")]
        protected float m_Mass = 0.0f;
        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("m_centerOfMass")]
        protected Vector3 m_CenterOfMass = new Vector3(0.0f, 0.0f, 0.0f);
        [SerializeField]
        protected float m_Ixx = 0.0f;
        [SerializeField]
        protected float m_Iyy = 0.0f;
        [SerializeField]
        protected float m_Izz = 0.0f;
        [SerializeField]
        protected bool m_CalculateInertia = true;
        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("m_isScene")]
        protected bool m_IsScene = false;
        public bool IsScene { get { return m_IsScene; } }
        [SerializeField]
        protected bool m_ShowGizmo = false;
        [SerializeField]
        protected float m_GizmoScale = 1.0f;
        [SerializeField]
        protected float m_LinearDamping = 0.1f;
        [SerializeField]
        private Vector3 m_AngularDamping = new Vector3(0.1f, 0.1f, 0.1f);
        #endregion // Inspector

        private Vector3 m_ForceAcc;
        private Vector3 m_TorqueAcc;

        internal dNewtonBody m_Body = null;
        internal NewtonBodyCollision m_Collision = null;
        private float[] m_PositionPtr = new float[3];
        private float[] m_RotationPtr = new float[4];
        private float[] m_Vec3Ptr = new float[3];
        private float[] m_ComPtr = new float[3];
        private Transform m_CachedTransform;

        internal List<NewtonBodyScript> m_scripts = new List<NewtonBodyScript>();
        private bool initialized = false;
    }
}
