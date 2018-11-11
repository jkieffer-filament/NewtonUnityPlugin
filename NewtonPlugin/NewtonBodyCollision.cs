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

using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newton.Internal;

namespace Newton {
    public class NewtonBodyCollision {
        struct ColliderShapePair {
            public NewtonCollider m_collider;
            public dNewtonCollision m_shape;
        }

        public NewtonBodyCollision(NewtonBody body) {
            if (body.World.GetWorld() == null) { throw new NullReferenceException("Native world instance is null. The World component was probably destroyed"); }

            m_Body = body;

            List<NewtonCollider> colliderList = new List<NewtonCollider>();
            TraverseColliders(body.gameObject, colliderList, body.gameObject, body);

            if (body.m_isScene) {
                NewtonSceneCollider sceneCollider = body.gameObject.AddComponent<NewtonSceneCollider>();
                dNewtonCollisionScene sceneShape = (dNewtonCollisionScene)sceneCollider.Create(body.World);
                sceneCollider.SetShape(sceneShape);

                m_RootCollider = sceneCollider;
                m_Colliders.Add(sceneCollider);

                sceneShape.BeginAddRemoveCollision();
                foreach (NewtonCollider collider in colliderList) {
                    m_Colliders.Add(collider);
                    collider.m_ParentHandle = sceneShape.AddCollision(collider.GetShape());
                }
                sceneShape.EndAddRemoveCollision();
            } else if (colliderList.Count == 0) {
                NewtonCollider collider = body.gameObject.AddComponent<NewtonNullCollider>();
                collider.SetShape(collider.Create(body.World));

                m_RootCollider = collider;
                m_Colliders.Add(collider);
            } else if (colliderList.Count == 1) {
                m_RootCollider = colliderList[0];
                m_Colliders.Add(colliderList[0]);
            } else {

                NewtonCompoundCollider compoundCollider = body.gameObject.AddComponent<NewtonCompoundCollider>();
                dNewtonCollisionCompound compoundShape = (dNewtonCollisionCompound)compoundCollider.Create(body.World);
                compoundCollider.SetShape(compoundShape);

                m_RootCollider = compoundCollider;
                m_Colliders.Add(compoundCollider);

                compoundShape.BeginAddRemoveCollision();
                foreach (NewtonCollider collider in colliderList) {
                    m_Colliders.Add(collider);
                    collider.m_ParentHandle = compoundShape.AddCollision(collider.GetShape());
                }
                compoundShape.EndAddRemoveCollision();
            }
        }

        public void Destroy() {
            m_Body = null;

            m_RootCollider = null;
            foreach (var collider in m_Colliders) {
                collider.DestroyShape();
            }
            m_Colliders.Clear();
        }

        private void TraverseColliders(GameObject gameObject, List<NewtonCollider> colliderList, GameObject rootObject, NewtonBody body) {
            // Don't fetch colliders from children with NewtonBodies
            if ((gameObject == rootObject) || (gameObject.GetComponent<NewtonBody>() == null)) {
                //Fetch all colliders
                foreach (NewtonCollider collider in gameObject.GetComponents<NewtonCollider>()) {
                    dNewtonCollision shape = collider.CreateBodyShape(body.World);
                    if (shape != null) {
                        collider.Body = body;
                        collider.SetShape(shape);
                        colliderList.Add(collider);
                    }
                }

                Terrain terrain = gameObject.GetComponent<Terrain>();
                if (terrain) {
                    NewtonHeighfieldCollider heighfield = gameObject.GetComponent<NewtonHeighfieldCollider>();
                    if (heighfield) {
                        TerrainData data = terrain.terrainData;

                        int treesCount = data.treeInstanceCount;
                        TreeInstance[] treeInstanceArray = data.treeInstances;
                        TreePrototype[] treeProtoArray = data.treePrototypes;

                        Vector3 posit = Vector3.zero;
                        for (int i = 0; i < treesCount; i++) {
                            TreeInstance tree = treeInstanceArray[i];
                            posit.x = tree.position.x * data.size.x;
                            posit.y = tree.position.y * data.size.y;
                            posit.z = tree.position.z * data.size.z;

                            //Debug.Log("xxx0 " + posit);
                            TreePrototype treeProto = treeProtoArray[tree.prototypeIndex];
                            GameObject treeGameObject = treeProto.prefab;
                            foreach (NewtonCollider treeCollider in treeGameObject.GetComponents<NewtonCollider>()) {
                                dNewtonCollision treeShape = treeCollider.CreateBodyShape(body.World);
                                if (treeShape != null) {
                                    Vector3 treePosit = terrain.transform.position + treeCollider.m_posit + posit;
                                    //Debug.Log("xxx1 " + treePosit);
                                    dMatrix matrix = Utils.ToMatrix(treePosit, Quaternion.identity);
                                    treeShape.SetMatrix(matrix);

                                    treeCollider.Body = body;
                                    treeCollider.SetShape(treeShape);
                                    colliderList.Add(treeCollider);
                                }
                            }
                        }
                    }
                }


                foreach (Transform child in gameObject.transform) {
                    TraverseColliders(child.gameObject, colliderList, rootObject, body);
                }
            }
        }

        public void AddCollider(NewtonCollider collider) {

            if (collider.GetShape() == null) {
                throw new NullReferenceException("Can not add collider with null shape.");
            }

            bool success = m_Colliders.Add(collider);

            if (success) {
                if (m_Body.m_isScene) {
                    var sceneShape = (dNewtonCollisionScene)GetShape();

                    sceneShape.BeginAddRemoveCollision();
                    sceneShape.AddCollision(collider.GetShape());
                    sceneShape.EndAddRemoveCollision();
                } else if (m_RootCollider is NewtonNullCollider) {
                    m_Body.GetBody().SetCollision(collider.GetShape());

                    m_RootCollider.DestroyShape();
                    m_Colliders.Remove(m_RootCollider);
                    Component.Destroy(m_RootCollider);

                    m_RootCollider = collider;
                   
                } else if (m_RootCollider is NewtonCompoundCollider) {
                    var compoundShape = (dNewtonCollisionCompound)GetShape();

                    compoundShape.BeginAddRemoveCollision();
                    collider.m_ParentHandle = compoundShape.AddCollision(collider.GetShape());
                    compoundShape.EndAddRemoveCollision();
                } else {

                    NewtonCompoundCollider compoundCollider = m_Body.gameObject.AddComponent<NewtonCompoundCollider>();
                    dNewtonCollisionCompound compoundShape = (dNewtonCollisionCompound)compoundCollider.Create(m_Body.World);
                    compoundCollider.SetShape(compoundShape);

                    m_Colliders.Add(compoundCollider);

                    compoundShape.BeginAddRemoveCollision();
                    m_RootCollider.m_ParentHandle = compoundShape.AddCollision(m_RootCollider.GetShape());
                    collider.m_ParentHandle = compoundShape.AddCollision(collider.GetShape());
                    compoundShape.EndAddRemoveCollision();

                    m_RootCollider = compoundCollider;
                    m_Body.GetBody().SetCollision(compoundShape);
                }

                m_Body.ResetCenterOfMass();
            }

        }

        public void RemoveCollider(NewtonCollider collider) {

            if (ReferenceEquals(collider, m_RootCollider) && (collider is NewtonNullCollider || collider is NewtonCompoundCollider || collider is NewtonSceneCollider)) {
                throw new NullReferenceException(string.Format("Can not remove root collider of type {0}.", collider.GetType().Name));
            }

            bool success = m_Colliders.Remove(collider);

            if (success) {
                if (m_Body.m_isScene) {
                    var sceneShape = (dNewtonCollisionScene)GetShape();

                    sceneShape.BeginAddRemoveCollision();
                    sceneShape.RemoveCollision(collider.m_ParentHandle);
                    collider.m_ParentHandle = IntPtr.Zero;
                    sceneShape.EndAddRemoveCollision();

                } else if (m_Colliders.Count == 0) {

                    NewtonCollider nullCollider = m_Body.gameObject.AddComponent<NewtonNullCollider>();
                    dNewtonCollision nullShape = nullCollider.Create(m_Body.World);
                    nullCollider.SetShape(nullShape);

                    m_RootCollider = nullCollider;
                    m_Colliders.Add(nullCollider);

                    m_Body.GetBody().SetCollision(nullShape);
                } else {
                    if (m_Colliders.Count == 2) {
                        m_Body.GetBody().SetCollision(collider.GetShape());

                        m_RootCollider.DestroyShape();
                        m_Colliders.Remove(m_RootCollider);
                        Component.Destroy(m_RootCollider);


                        m_RootCollider = m_Colliders.First();

                    } else {
                        var compoundShape = (dNewtonCollisionCompound)GetShape();

                        compoundShape.BeginAddRemoveCollision();
                        compoundShape.RemoveCollision(collider.m_ParentHandle);
                        collider.m_ParentHandle = IntPtr.Zero;
                        compoundShape.EndAddRemoveCollision();
                    }
                }
            

                 m_Body.ResetCenterOfMass();
            }
        }

        public dNewtonCollision GetShape() {
            return m_RootCollider.GetShape();
        }

        private NewtonBody m_Body;
        private NewtonCollider m_RootCollider;
        private HashSet<NewtonCollider> m_Colliders = new HashSet<NewtonCollider>();
    }
}

