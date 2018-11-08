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
        public dNewtonWorld GetWorld() {
            return m_world;
        }

        void Start() {
            m_onWorldCallback = new OnWorldUpdateCallback(OnWorldUpdate);
            m_onWorldBodyTransfromUpdateCallback = new OnWorldBodyTransfromUpdateCallback(OnBodyTransformUpdate);

            m_world.SetAsyncUpdate(m_asyncUpdate);
            m_world.SetFrameRate(m_updateRate);
            m_world.SetThreadsCount(m_numberOfThreads);
            m_world.SetSolverIterations(m_solverIterationsCount);
            m_world.SetBroadPhase(m_broadPhaseType);
            m_world.SetGravity(m_gravity.x, m_gravity.y, m_gravity.z);
            m_world.SetSubSteps(m_subSteps);
            m_world.SetParallelSolverOnLargeIsland(m_useParallerSolver);
            m_world.SetDefaultMaterial(m_defaultRestitution, m_defaultStaticFriction, m_defaultKineticFriction, true);
            m_world.SetCallbacks(m_onWorldCallback, m_onWorldBodyTransfromUpdateCallback);

            //Load all physics plug ins and choose the best one
            m_world.SelectPlugin(IntPtr.Zero);
            if (m_useParallerSolver && (m_pluginsOptions > 0)) {
                string path = Application.dataPath;
                m_world.LoadPlugins(path);
                int index = 1;
                for (IntPtr plugin = m_world.FirstPlugin(); plugin != IntPtr.Zero; plugin = m_world.NextPlugin(plugin)) {
                    if (index == m_pluginsOptions) {
                        Debug.Log("Using newton physics solver: " + m_world.GetPluginName(plugin));
                        m_world.SelectPlugin(plugin);
                    }
                    index++;
                }
            } else {
                m_world.UnloadPlugins();
            }

            InitScene();
        }

        void OnDestroy() {
            DestroyScene();
            m_onWorldCallback = null;
            m_onWorldBodyTransfromUpdateCallback = null;

        }

        internal void RegisterBody(NewtonBody nb) {
            if (!m_bodies.Contains(nb))     // Had to add this
            {
                m_bodies.Add(nb);
            }
        }

        internal void UnregisterBody(NewtonBody nb) {
            m_bodies.Remove(nb);
        }

        //private void InitPhysicsScene(GameObject root)
        //{
        //    NewtonBody bodyPhysics = root.GetComponent<NewtonBody>();
        //    if (bodyPhysics != null)
        //    {
        //        bodyPhysics.InitRigidBody();
        //    }

        //    foreach (Transform child in root.transform)
        //    {
        //        InitPhysicsScene(child.gameObject);
        //    }
        //}

        //private void InitPhysicsJoints(GameObject root)
        //{
        //    foreach (NewtonJoint joint in root.GetComponents<NewtonJoint>())
        //    {
        //        joint.Create();
        //    }

        //    foreach (Transform child in root.transform)
        //    {
        //        InitPhysicsJoints(child.gameObject);
        //    }
        //}

        private void InitScene() {
            Resources.LoadAll("Newton Materials");
            NewtonMaterialInteraction[] materialList = Resources.FindObjectsOfTypeAll<NewtonMaterialInteraction>();
            foreach (NewtonMaterialInteraction materialInteraction in materialList) {
                // register all material interactions.
                if (materialInteraction.m_material_0 && materialInteraction.m_material_1) {
                    int id0 = materialInteraction.m_material_0.GetInstanceID();
                    int id1 = materialInteraction.m_material_1.GetInstanceID();
                    m_world.SetMaterialInteraction(id0, id1, materialInteraction.m_restitution, materialInteraction.m_staticFriction, materialInteraction.m_kineticFriction, materialInteraction.m_collisionEnabled);
                }
            }

            //GameObject[] objectList = gameObject.scene.GetRootGameObjects();
            //foreach (GameObject rootObj in objectList)
            //{
            //    InitPhysicsScene(rootObj);
            //}
            //foreach (GameObject rootObj in objectList)
            //{
            //    InitPhysicsJoints(rootObj);
            //}
        }

        private void DestroyScene() {
            if (m_world != null) {
                foreach (NewtonBody nb in m_bodies) {
                    nb.DestroyRigidBody();
                }

                m_world = null;
            }
        }

        void Update() {
            //Debug.Log("Update time :" + Time.deltaTime);
            if (m_serializeSceneOnce) {
                m_serializeSceneOnce = false;
                m_world.SaveSerializedScene(m_saveSceneName);
            }

            m_world.Update(Time.deltaTime);
        }

        private void OnWorldUpdate(float timestep) {
            foreach (NewtonBody bodyPhysics in m_bodies) {
                // Apply force & torque accumulators
                bodyPhysics.ApplyExternaForces();

                foreach (NewtonBodyScript script in bodyPhysics.m_scripts) {
                    if (script.m_collisionNotification) {
                        for (IntPtr contact = m_world.GetFirstContactJoint(bodyPhysics.m_body); contact != IntPtr.Zero; contact = m_world.GetNextContactJoint(bodyPhysics.m_body, contact)) {
                            var body0 = (NewtonBody)GCHandle.FromIntPtr(m_world.GetBody0UserData(contact)).Target;
                            var body1 = (NewtonBody)GCHandle.FromIntPtr(m_world.GetBody1UserData(contact)).Target;
                            var otherBody = bodyPhysics == body0 ? body1 : body0;
                            script.OnCollision(otherBody);

                            if (script.m_contactNotification) {
                                for (IntPtr ct = m_world.GetFirstContact(contact); ct != IntPtr.Zero; ct = m_world.GetNextContact(contact, ct)) {
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

            var hitInfoPtr = m_world.Raycast(startPos.x, startPos.y, startPos.z, endPos.x, endPos.y, endPos.z, layerMask);
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

        private dNewtonWorld m_world = new dNewtonWorld();
        public bool m_asyncUpdate = true;
        public bool m_serializeSceneOnce = false;
        public bool m_useParallerSolver = true;
        public string m_saveSceneName = "scene_01.bin";
        public int m_broadPhaseType = 0;
        public int m_numberOfThreads = 0;
        public int m_solverIterationsCount = 1;
        public int m_updateRate = 60;
        public int m_subSteps = 2;
        public int m_pluginsOptions = 0;

        public Vector3 m_gravity = new Vector3(0.0f, -9.8f, 0.0f);

        public float m_defaultRestitution = 0.4f;
        public float m_defaultStaticFriction = 0.8f;
        public float m_defaultKineticFriction = 0.6f;

        private OnWorldUpdateCallback m_onWorldCallback;
        private OnWorldBodyTransfromUpdateCallback m_onWorldBodyTransfromUpdateCallback;

        private List<NewtonBody> m_bodies = new List<NewtonBody>();
    }
}


