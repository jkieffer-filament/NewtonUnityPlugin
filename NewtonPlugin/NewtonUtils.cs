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

using Newton.Internal;

namespace Newton {
    public class Utils {
        static public dMatrix ToMatrix(Vector3 posit, Quaternion rotation) {
            dMatrix matrix = new dMatrix();
            Matrix4x4 entMatrix = Matrix4x4.identity;
            entMatrix.SetTRS(posit, rotation, Vector3.one);
            matrix.m_front = new dVector(entMatrix.m00, entMatrix.m10, entMatrix.m20, entMatrix.m30);
            matrix.m_up = new dVector(entMatrix.m01, entMatrix.m11, entMatrix.m21, entMatrix.m31);
            matrix.m_right = new dVector(entMatrix.m02, entMatrix.m12, entMatrix.m22, entMatrix.m32);
            matrix.m_posit = new dVector(entMatrix.m03, entMatrix.m13, entMatrix.m23, entMatrix.m33);
            return matrix;
        }


        static public dMatrix ToMatrix(Vector3 posit, Vector3 pin) {

            // make orthogonal basis vectors
            Vector3 frontBasis = pin.normalized;
            Vector3 upBasis = frontBasis.x != 0 || frontBasis.y != 0 ? new Vector3(-frontBasis.y, frontBasis.x, frontBasis.z) : new Vector3(frontBasis.x, frontBasis.z, -frontBasis.y);
            Vector3 rightBasis = Vector3.Cross(frontBasis, upBasis);

            dMatrix matrix = new dMatrix();
            matrix.m_front = new dVector(frontBasis.x, frontBasis.y, frontBasis.z, 0f);
            matrix.m_up = new dVector(upBasis.x, upBasis.y, upBasis.z, 0f);
            matrix.m_right = new dVector(rightBasis.x, rightBasis.y, rightBasis.z, 0f);
            matrix.m_posit = new dVector(posit.x, posit.y, posit.z, 1f);
            return matrix;
        }

        static public dMatrix ToMatrix(Vector3 posit, Vector3 pin0, Vector3 pin1) {

            // make orthogonal basis vectors
            Vector3 frontBasis = pin0.normalized;
            Vector3 upBasis = pin1.normalized;
            Vector3 rightBasis = Vector3.Cross(frontBasis, upBasis);
            upBasis = Vector3.Cross(rightBasis, frontBasis);

            dMatrix matrix = new dMatrix();
            matrix.m_front = new dVector(frontBasis.x, frontBasis.y, frontBasis.z, 0f);
            matrix.m_up = new dVector(upBasis.x, upBasis.y, upBasis.z, 0f);
            matrix.m_right = new dVector(rightBasis.x, rightBasis.y, rightBasis.z, 0f);
            matrix.m_posit = new dVector(posit.x, posit.y, posit.z, 0f);
            return matrix;
        }


        static public int dRand(int seed, int oldSeed) {
            return oldSeed + seed * 31415821;
        }
    }
}

