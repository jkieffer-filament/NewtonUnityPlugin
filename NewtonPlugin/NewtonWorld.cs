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

    public delegate void OnWorldBodyTransfromUpdateCallback();
    public delegate void OnWorldUpdateCallback(float timestep);

    [StructLayout(LayoutKind.Sequential)]
    internal struct _InternalRayHitInfo {
        internal float intersectParam;
        internal int layermask;
        internal IntPtr body;
        internal IntPtr collider;
        internal Vector3 position;
        internal Vector3 normal;
        internal uint collisionID;
    }

    public struct NewtonRayHitInfo {
        public NewtonBody body;
        //public NewtonCollider collider;
        public Vector3 position;
        public Vector3 normal;
        public uint collisionID;

    }


    [DisallowMultipleComponent]
    [AddComponentMenu("Newton Physics/Newton World")]
    public class NewtonWorld : MonoBehaviour {

        private static NewtonWorld s_Current;
        public static NewtonWorld Current {
            get {
                if (ReferenceEquals(s_Current, null)) {
                    s_Current = FindObjectOfType<NewtonWorld>();
                }
                return s_Current;
            }
        }


        public dNewtonWorld GetWorld() {
            return m_World;
        }

        private void Awake() {
            if (ReferenceEquals(s_Current, null)) {
                s_Current = this;
            } else if (!ReferenceEquals(s_Current, this)) {
                Destroy(this);
            }

            m_onWorldCallback = new OnWorldUpdateCallback(OnWorldUpdate);
            m_onWorldBodyTransfromUpdateCallback = new OnWorldBodyTransfromUpdateCallback(OnBodyTransformUpdate);

            m_World.SetAsyncUpdate(m_AsyncUpdate);
            m_World.SetFrameRate(1f/Time.fixedDeltaTime);
            m_World.SetThreadsCount(m_NumberOfThreads);
            m_World.SetSolverIterations(m_SolverIterationsCount);
            m_World.SetBroadPhase(m_BroadPhaseType);
            m_World.SetGravity(m_Gravity.x, m_Gravity.y, m_Gravity.z);
            m_World.SetSubSteps(m_SubSteps);
            m_World.SetParallelSolverOnLargeIsland(m_UseParallerSolver);
            m_World.SetDefaultMaterial(m_DefaultRestitution, m_DefaultStaticFriction, m_DefaultKineticFriction, true);
            m_World.SetCallbacks(m_onWorldCallback, m_onWorldBodyTransfromUpdateCallback);

            //Load all physics plug ins and choose the best one
            m_World.SelectPlugin(IntPtr.Zero);
            if (m_UseParallerSolver && (m_PluginsOptions > 0)) {
                string path = Application.dataPath;
                m_World.LoadPlugins(path);
                int index = 1;
                for (IntPtr plugin = m_World.FirstPlugin(); plugin != IntPtr.Zero; plugin = m_World.NextPlugin(plugin)) {
                    if (index == m_PluginsOptions) {
                        Debug.Log("Using newton physics solver: " + m_World.GetPluginName(plugin));
                        m_World.SelectPlugin(plugin);
                    }
                    index++;
                }
            } else {
                m_World.UnloadPlugins();
            }

            InitScene();
        }


        void OnDestroy() {
            if (ReferenceEquals(s_Current, this)) {
                s_Current = null;
            }

            DestroyScene();
            m_onWorldCallback = null;
            m_onWorldBodyTransfromUpdateCallback = null;

        }

        internal void RegisterBody(NewtonBody nb) {
            m_bodies.Add(nb); 
        }

        internal void DeregisterBody(NewtonBody nb) {
            m_bodies.Remove(nb);
        }

        private void InitScene() {
            Resources.LoadAll("Newton Materials");
            NewtonMaterialInteraction[] materialList = Resources.FindObjectsOfTypeAll<NewtonMaterialInteraction>();
            foreach (NewtonMaterialInteraction materialInteraction in materialList) {
                // register all material interactions.
                if (materialInteraction.m_material_0 && materialInteraction.m_material_1) {
                    int id0 = materialInteraction.m_material_0.GetInstanceID();
                    int id1 = materialInteraction.m_material_1.GetInstanceID();
                    m_World.SetMaterialInteraction(id0, id1, materialInteraction.m_restitution, materialInteraction.m_staticFriction, materialInteraction.m_kineticFriction, materialInteraction.m_collisionEnabled);
                }
            }
        }

        private void DestroyScene() {
            if (m_World != null) {
                foreach (NewtonBody nb in m_bodies) {
                    nb.DestroyRigidBody();
                }

                m_World = null;
            }
        }

        void FixedUpdate() {
            //Debug.Log("Update time :" + Time.deltaTime);
            if (m_SerializeSceneOnce) {
                m_SerializeSceneOnce = false;
                m_World.SaveSerializedScene(m_SaveSceneName);
            }

            m_World.Update(Time.fixedDeltaTime);
        }

        private void OnWorldUpdate(float timestep) {
            foreach (NewtonBody bodyPhysics in m_bodies) {
                // Apply force & torque accumulators
                bodyPhysics.ApplyExternaForces();

                foreach (NewtonBodyScript script in bodyPhysics.m_scripts) {
                    if (script.m_collisionNotification) {
                        for (IntPtr contact = m_World.GetFirstContactJoint(bodyPhysics.m_Body); contact != IntPtr.Zero; contact = m_World.GetNextContactJoint(bodyPhysics.m_Body, contact)) {
                            var body0 = (NewtonBody)GCHandle.FromIntPtr(m_World.GetBody0UserData(contact)).Target;
                            var body1 = (NewtonBody)GCHandle.FromIntPtr(m_World.GetBody1UserData(contact)).Target;
                            var otherBody = bodyPhysics == body0 ? body1 : body0;
                            script.OnCollision(otherBody);

                            if (script.m_contactNotification) {
                                for (IntPtr ct = m_World.GetFirstContact(contact); ct != IntPtr.Zero; ct = m_World.GetNextContact(contact, ct)) {
                                    //var normImpact = dNewtonContact.GetContactNormalImpact(ct);
                                    //script.OnContact(otherBody, normImpact);
                                }
                            }

                            script.OnPostCollision(otherBody);
                        }
                    }

                    // apply external force and torque if any
                    if (script.m_enableForceAndTorque) {
                        script.OnApplyForceAndTorque(timestep);
                    }
                }
            }
        }

        private void OnBodyTransformUpdate() {
            foreach (NewtonBody bodyPhysics in m_bodies) {
                bodyPhysics.OnUpdateTranform();
            }
        }

        public bool Raycast(Vector3 origin, Vector3 direction, float distance, out NewtonRayHitInfo hitInfo, int layerMask = 0) {
            Vector3 startPos = origin;
            Vector3 endPos = startPos + (direction * distance);

            var hitInfoPtr = m_World.Raycast(startPos.x, startPos.y, startPos.z, endPos.x, endPos.y, endPos.z, layerMask);
            if (hitInfoPtr != IntPtr.Zero) {
                _InternalRayHitInfo info = (_InternalRayHitInfo)Marshal.PtrToStructure(hitInfoPtr, typeof(_InternalRayHitInfo));

                if (info.body != IntPtr.Zero) {
                    hitInfo.body = (NewtonBody)GCHandle.FromIntPtr(info.body).Target;
                } else {
                    hitInfo.body = null;
                }

                //hitInfo.collider = null;
                hitInfo.position = info.position;
                hitInfo.normal = info.normal;
                hitInfo.collisionID = info.collisionID;
                return true;
            }

            hitInfo.body = null;
            //hitInfo.collider = null;
            hitInfo.position = Vector3.zero;
            hitInfo.normal = Vector3.zero;
            hitInfo.collisionID = 0;
            return false;
        }

        private dNewtonWorld m_World = new dNewtonWorld();
        [SerializeField]
        private bool m_AsyncUpdate = true;
        [SerializeField]
        private bool m_SerializeSceneOnce = false;
        [SerializeField]
        private bool m_UseParallerSolver = true;
        [SerializeField]
        private string m_SaveSceneName = "scene_01.bin";
        [SerializeField]
        private int m_BroadPhaseType = 0;
        [SerializeField]
        private int m_NumberOfThreads = 0;
        [SerializeField]
        private int m_SolverIterationsCount = 1;
        [SerializeField]
        private int m_SubSteps = 2;
        [SerializeField]
        private int m_PluginsOptions = 0;

        [SerializeField]
        internal Vector3 m_Gravity = new Vector3(0.0f, -9.8f, 0.0f);

        [SerializeField]
        private float m_DefaultRestitution = 0.4f;
        [SerializeField]
        private float m_DefaultStaticFriction = 0.8f;
        [SerializeField]
        private float m_DefaultKineticFriction = 0.6f;

        private OnWorldUpdateCallback m_onWorldCallback;
        private OnWorldBodyTransfromUpdateCallback m_onWorldBodyTransfromUpdateCallback;

        private HashSet<NewtonBody> m_bodies = new HashSet<NewtonBody>();
    }
}


