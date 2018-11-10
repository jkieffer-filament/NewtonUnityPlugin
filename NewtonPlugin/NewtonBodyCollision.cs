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

using UnityEngine;
using System;
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

            List<ColliderShapePair> colliderList = new List<ColliderShapePair>();
            TraverseColliders(body.gameObject, colliderList, body.gameObject, body);

            if (body.m_isScene) {
                m_collidersArray = new ColliderShapePair[colliderList.Count + 1];
                NewtonSceneCollider sceneCollider = body.gameObject.AddComponent<NewtonSceneCollider>();
                dNewtonCollisionScene sceneShape = (dNewtonCollisionScene)sceneCollider.Create(body.World);

                m_collidersArray[0].m_shape = sceneShape;
                m_collidersArray[0].m_collider = sceneCollider;

                int index = 1;
                sceneShape.BeginAddRemoveCollision();
                foreach (ColliderShapePair pair in colliderList) {
                    m_collidersArray[index] = pair;
                    sceneShape.AddCollision(pair.m_shape);
                    index++;
                }
                sceneShape.EndAddRemoveCollision();
            } else if (colliderList.Count == 0) {
                m_collidersArray = new ColliderShapePair[1];
                NewtonCollider collider = body.gameObject.AddComponent<NewtonNullCollider>();
                m_collidersArray[0].m_collider = collider;
                m_collidersArray[0].m_shape = collider.Create(body.World);
            } else if (colliderList.Count == 1) {
                m_collidersArray = new ColliderShapePair[1];
                m_collidersArray[0] = colliderList[0];
            } else {
                m_collidersArray = new ColliderShapePair[colliderList.Count + 1];
                NewtonCompoundCollider compoundCollider = body.gameObject.AddComponent<NewtonCompoundCollider>();
                dNewtonCollisionCompound compoundShape = (dNewtonCollisionCompound)compoundCollider.Create(body.World);

                m_collidersArray[0].m_shape = compoundShape;
                m_collidersArray[0].m_collider = compoundCollider;

                int index = 1;
                compoundShape.BeginAddRemoveCollision();
                foreach (ColliderShapePair pair in colliderList) {
                    m_collidersArray[index] = pair;
                    compoundShape.AddCollision(pair.m_shape);
                    index++;
                }
                compoundShape.EndAddRemoveCollision();
            }
        }

        public void Destroy() {
            for (int i = 0; i < m_collidersArray.Length; i++) {
                m_collidersArray[i].m_shape.Dispose();
                m_collidersArray[i].m_shape = null;
                m_collidersArray[i].m_collider = null;
            }
        }

        private void TraverseColliders(GameObject gameObject, List<ColliderShapePair> colliderList, GameObject rootObject, NewtonBody body) {
            // Don't fetch colliders from children with NewtonBodies
            if ((gameObject == rootObject) || (gameObject.GetComponent<NewtonBody>() == null)) {
                //Fetch all colliders
                foreach (NewtonCollider collider in gameObject.GetComponents<NewtonCollider>()) {
                    dNewtonCollision shape = collider.CreateBodyShape(body.World);
                    if (shape != null) {
                        ColliderShapePair pair;
                        pair.m_collider = collider;
                        pair.m_shape = shape;
                        colliderList.Add(pair);
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
                                    ColliderShapePair pair;
                                    Vector3 treePosit = terrain.transform.position + treeCollider.m_posit + posit;
                                    //Debug.Log("xxx1 " + treePosit);
                                    dMatrix matrix = Utils.ToMatrix(treePosit, Quaternion.identity);
                                    treeShape.SetMatrix(matrix);

                                    pair.m_collider = treeCollider;
                                    pair.m_shape = treeShape;
                                    colliderList.Add(pair);
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

        public dNewtonCollision GetShape() {
            return m_collidersArray[0].m_shape;
        }

        private ColliderShapePair[] m_collidersArray;
    }
}

