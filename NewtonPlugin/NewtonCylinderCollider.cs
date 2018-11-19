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
using Newton.Internal;

namespace Newton {

    [AddComponentMenu("Newton Physics/Colliders/Cylinder")]
    public class NewtonCylinderCollider : NewtonCollider {
        public override dNewtonCollision Create(NewtonWorld world) {
            dNewtonCollision collider = new dNewtonCollisionCylinder(world.GetWorld(), m_Radius0, m_Radius1, m_Height);
            SetMaterial(collider);
            SetLayer(collider);
            return collider;
        }

        public float Radius0 { get { return m_Radius0; } }
        public float Radius1 { get { return m_Radius1; } }
        public float Height { get { return m_Height; } }

        #region Inspector
        [SerializeField]
        private float m_Radius0 = 0.25f;
        [SerializeField]
        private float m_Radius1 = 0.25f;
        [SerializeField]
        private float m_Height = 0.5f;
        #endregion
    }
}

