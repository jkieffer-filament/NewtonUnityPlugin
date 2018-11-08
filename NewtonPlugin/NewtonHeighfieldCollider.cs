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
    [AddComponentMenu("Newton Physics/Colliders/Height field Collider")]
    class NewtonHeighfieldCollider : NewtonCollider {
        public override bool IsStatic() {
            return true;
        }
        public override Vector3 GetScale() {
            return new Vector3(1.0f, 1.0f, 1.0f);
        }

        public Vector3 GetBaseScale() {
            return new Vector3(1.0f, 1.0f, 1.0f);
        }

        private void SetDefualtParams() {
            m_isTrigger = false;
            m_inheritTransformScale = false;

            // static meshes can not be triggers.
            m_isTrigger = false;

            // In Unity terrain can not be scale, so we do not apply scale
            m_inheritTransformScale = false;

            // in unity terrain can not rotated or have local transformation either
            m_posit = new Vector3(0.0f, 0.0f, 0.0f);
            m_rotation = new Vector3(0.0f, 0.0f, 0.0f);
            transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
        }

        public override dNewtonCollision Create(NewtonWorld world) {
            TerrainData data = m_terrain.terrainData;
            //Debug.Log("xxxx  " + data.alphamapWidth + "   xxx  " + data.detailHeight);
            //Debug.Log("xxxx  " + data.heightmapScale);
            //Debug.Log("xxxx  " + data.size);

            int resolution = data.heightmapResolution;
            dVector scale = new dVector(data.size.x, data.size.y, data.size.z, 0.0f);

            m_oldSize = data.size;
            m_oldResolution = resolution;

            data.GetHeights(0, 0, resolution, resolution);

            int hash = 0;
            float elevationScale = data.size.y;
            float[] elevation = new float[resolution * resolution];
            for (int z = 0; z < resolution; z++) {
                for (int x = 0; x < resolution; x++) {
                    float value = data.GetHeight(x, z);
                    elevation[z * resolution + x] = value;
                    hash = Utils.dRand((int)(elevationScale * value), hash);
                }
            }
            m_elevationHash = hash;

            IntPtr elevationPtr = Marshal.AllocHGlobal(resolution * resolution * Marshal.SizeOf(typeof(float)));
            Marshal.Copy(elevation, 0, elevationPtr, elevation.Length);
            dNewtonCollision collider = new dNewtonCollisionHeightField(world.GetWorld(), elevationPtr, resolution, scale);
            Marshal.FreeHGlobal(elevationPtr);

            SetDefualtParams();
            SetMaterial(collider);
            SetLayer(collider);
            return collider;
        }

        private bool ElevationHasChanged() {
            TerrainData data = m_terrain.terrainData;
            int resolution = data.heightmapResolution;
            float scale = data.size.y;

            int hash = 0;
            for (int z = 0; z < resolution; z++) {
                for (int x = 0; x < resolution; x++) {
                    hash = Utils.dRand((int)(data.GetHeight(x, z) * scale), hash);
                }
            }
            bool state = (hash != m_elevationHash);
            m_elevationHash = hash;
            return state;
        }

        public override void OnDrawGizmosSelected() {
            if (m_showGizmo) {
                SetDefualtParams();
                TerrainData data = m_terrain.terrainData;
                if ((data.heightmapResolution != m_oldResolution) || (m_oldSize != data.size) || ElevationHasChanged()) {
                    RecreateEditorShape();
                }

                base.OnDrawGizmosSelected();
            }
        }

        public Terrain m_terrain = null;
        private int m_oldResolution = 0;
        private int m_elevationHash = 0;
        private Vector3 m_oldSize;
    }
}
