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
    class NewtonWheelCollider : NewtonChamferedCylinderCollider {
        public override dNewtonCollision Create(NewtonWorld world) {
            dNewtonCollision shape = base.Create(world);
            m_Scale.y = 1.0f;
            return shape;
        }
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(NewtonWheelCollider))]
    [AddComponentMenu("Newton Physics/Vehicle/Rigid Wheel Body")]
    class NewtonWheelBody : NewtonBody {
        void Start() {
            m_IsScene = false;
            m_Shape = GetComponent<NewtonWheelCollider>();
            m_Shape.m_Scale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        public void CreateTire() {
            Debug.Log("create actual wheel");

            var handle = GCHandle.Alloc(this);

            dTireData data = new dTireData();
            //data.m_owner = GCHandle.ToIntPtr(handle);
            m_Wheel = new dNewtonWheel((dNewtonVehicle)m_Vehicle.m_Body, data);
        }

        public void DestroyTire() {
            Debug.Log("destroy actual wheel");
            //var handle = GCHandle.FromIntPtr(m_wheel.GetUserData());
            //handle.Free();

            m_Wheel.Dispose();
            m_Wheel = null;
        }

        public override void InitRigidBody() {
            if (m_Vehicle == null) {
                // if the tire is not attached to a vehicle the this is simple rigid body 
                base.InitRigidBody();
            }
        }

        public NewtonVehicleBody Vechicle {  get { return m_Vehicle; } }

        #region Inspector
        [SerializeField]
        private NewtonVehicleBody m_Vehicle = null;
        [SerializeField]
        private float m_PivotOffset = 0.0f;
        [Range(0.0f, 45.0f)]
        [SerializeField]
        private float m_MaxSteeringAngle = 20.0f;
        [SerializeField]
        private float m_SuspensionDamping = 1000.0f;
        [SerializeField]
        private float m_SuspensionSpring = 100.0f;
        [SerializeField]
        private float m_SuspensionLength = 0.3f;
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float m_LateralStiffness = 0.5f;
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float m_LongitudialStiffness = 0.5f;
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float m_AligningMomentTrail = 0.5f;
        [Range(0, 2)]
        [SerializeField]
        private int m_SuspentionType = 1;
        [SerializeField]
        private bool m_HasFender = false;
        #endregion // Inspector

        private NewtonWheelCollider m_Shape = null;
        private dNewtonWheel m_Wheel = null;
    }
}

