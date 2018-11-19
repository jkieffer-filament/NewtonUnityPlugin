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
using Newton.Internal;

namespace Newton {
    [AddComponentMenu("Newton Physics/Colliders/Chamfered Cylinder")]
    public class NewtonChamferedCylinderCollider : NewtonCollider {
        public override dNewtonCollision Create(NewtonWorld world) {
            dNewtonCollision collider = new dNewtonCollisionChamferedCylinder(world.GetWorld(), m_Radius, m_Height);
            SetMaterial(collider);
            SetLayer(collider);
            m_Scale.y = 4.0f;
            return collider;
        }

        public float Radius { get { return m_Radius; } }
        public float Height { get { return m_Height; } }

        #region Inspector
        [SerializeField]
        private float m_Radius = 0.25f;
        [SerializeField]
        private float m_Height = 0.5f;
        #endregion
    }
}

