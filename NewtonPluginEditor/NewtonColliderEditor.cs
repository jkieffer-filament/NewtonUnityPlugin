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
using UnityEditor;
using System.Collections.Generic;

namespace Newton.Editor {
    using static ValidationHelpers;

    [CustomEditor(typeof(NewtonCollider))]
    public abstract class NewtonColliderEditor : UnityEditor.Editor {
        protected void SetupBaseProps() {
            // Setup the SerializedProperties
            m_showGizmoProp = serializedObject.FindProperty("m_ShowGizmo");
            m_posProp = serializedObject.FindProperty("m_Position");
            m_rotProp = serializedObject.FindProperty("m_Rotation");
            m_scaleProp = serializedObject.FindProperty("m_Scale");
            m_materialProp = serializedObject.FindProperty("m_Material");
            m_layerProp = serializedObject.FindProperty("m_Layer");
            m_isTriggerProp = serializedObject.FindProperty("m_IsTrigger");
            m_inheritScaleProp = serializedObject.FindProperty("m_InheritTransformScale");
            //Undo.undoRedoPerformed += OnUndoRedo;
        }

        void OnDestroy() {
            //Undo.undoRedoPerformed -= OnUndoRedo;
        }

        private void OnUndoRedo() {
            Validate(); // Trigger derived class to Validate and check if value changed.
        }

        protected abstract void Validate();

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_posProp, new GUIContent("Position"));
            EditorGUILayout.PropertyField(m_rotProp, new GUIContent("Rotation"));
            EditorGUILayout.PropertyField(m_scaleProp, new GUIContent("Scale"));
            EditorGUILayout.PropertyField(m_inheritScaleProp, new GUIContent("Inherit Scale"));
            EditorGUILayout.PropertyField(m_showGizmoProp, new GUIContent("Show Gizmo"));
            EditorGUILayout.PropertyField(m_isTriggerProp, new GUIContent("Is Trigger"));
            EditorGUILayout.PropertyField(m_materialProp, new GUIContent("Material"));
            m_layerProp.intValue = EditorGUILayout.LayerField(new GUIContent("Layer"), m_layerProp.intValue);



            serializedObject.ApplyModifiedProperties();
        }

        SerializedProperty m_showGizmoProp;
        SerializedProperty m_posProp;
        SerializedProperty m_rotProp;
        SerializedProperty m_scaleProp;
        SerializedProperty m_materialProp;
        SerializedProperty m_layerProp;
        SerializedProperty m_isTriggerProp;
        SerializedProperty m_inheritScaleProp;
    }

    [CustomEditor(typeof(NewtonSphereCollider))]
    public class NewtonSphereColliderEditor : NewtonColliderEditor {
        void OnEnable() {
            base.SetupBaseProps();
            m_radiusProp = serializedObject.FindProperty("m_Radius");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_radiusProp, new GUIContent("Radius"));
            Validate();
        }

        protected override void Validate() {
            NewtonSphereCollider collision = (NewtonSphereCollider)target;
            if (RadiusChangedAndValid(collision.Radius, m_radiusProp.floatValue, 0.01f)) {
                serializedObject.ApplyModifiedProperties(); //Transfer Editor value change to target object and add the change to the Undo/Redo stack
                collision.RecreateEditorShape();
                //Debug.Log("Sphere radius changed");
            }
        }
        SerializedProperty m_radiusProp;
    }

    [CustomEditor(typeof(NewtonBoxCollider))]
    public class NewtonBoxColliderEditor : NewtonColliderEditor {
        void OnEnable() {
            base.SetupBaseProps();
            m_dimensionProp = serializedObject.FindProperty("m_Size");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_dimensionProp, new GUIContent("Dimension"));
            Validate();
        }

        protected override void Validate() {
            NewtonBoxCollider collision = (NewtonBoxCollider)target;

            if (VolumeChangedAndValid(collision.Size, m_dimensionProp.vector3Value, 0.01f)) {
                serializedObject.ApplyModifiedProperties();
                collision.RecreateEditorShape();
                //Debug.Log("Box dimensions changed");
            }
        }

        SerializedProperty m_dimensionProp;
    }

    [CustomEditor(typeof(NewtonCylinderCollider))]
    public class NewtonCylinderColliderEditor : NewtonColliderEditor {
        void OnEnable() {
            base.SetupBaseProps();
            m_radius0Prop = serializedObject.FindProperty("m_Radius0");
            m_radius1Prop = serializedObject.FindProperty("m_Radius1");
            m_heightProp = serializedObject.FindProperty("m_Height");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_radius0Prop, new GUIContent("Radius 0"));
            EditorGUILayout.PropertyField(m_radius1Prop, new GUIContent("Radius 1"));
            EditorGUILayout.PropertyField(m_heightProp, new GUIContent("Height"));
            Validate();
        }

        protected override void Validate() {
            NewtonCylinderCollider collision = (NewtonCylinderCollider)target;

            if (RadiusChangedAndValid(collision.Radius0, m_radius0Prop.floatValue, 0.01f) || RadiusChangedAndValid(collision.Radius1, m_radius1Prop.floatValue, 0.01f) || HeightChangedAndValid(collision.Height, m_heightProp.floatValue, 0.01f)) {
                serializedObject.ApplyModifiedProperties();
                collision.RecreateEditorShape();
                //Debug.Log("Cylinder shape changed");
            }
        }

        SerializedProperty m_radius0Prop;
        SerializedProperty m_radius1Prop;
        SerializedProperty m_heightProp;
    }

    [CustomEditor(typeof(NewtonCapsuleCollider))]
    public class NewtonCapsuleColliderEditor : NewtonColliderEditor {
        void OnEnable() {
            base.SetupBaseProps();
            m_radius0Prop = serializedObject.FindProperty("m_Rdius0");
            m_radius1Prop = serializedObject.FindProperty("m_Radius1");
            m_heightProp = serializedObject.FindProperty("m_Height");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_radius0Prop, new GUIContent("Radius 0"));
            EditorGUILayout.PropertyField(m_radius1Prop, new GUIContent("Radius 1"));
            EditorGUILayout.PropertyField(m_heightProp, new GUIContent("Height"));
            Validate();
        }

        protected override void Validate() {
            NewtonCapsuleCollider collision = (NewtonCapsuleCollider)target;

            if (RadiusChangedAndValid(collision.Radius0, m_radius0Prop.floatValue, 0.01f) || RadiusChangedAndValid(collision.Radius1, m_radius1Prop.floatValue, 0.01f) || HeightChangedAndValid(collision.Height, m_heightProp.floatValue, 0.01f)) {
                serializedObject.ApplyModifiedProperties();
                collision.RecreateEditorShape();
                //Debug.Log("Capsule shape changed");
            }
        }

        SerializedProperty m_radius0Prop;
        SerializedProperty m_radius1Prop;
        SerializedProperty m_heightProp;
    }

    [CustomEditor(typeof(NewtonConeCollider))]
    public class NewtonCapsuleConeEditor : NewtonColliderEditor {
        void OnEnable() {
            base.SetupBaseProps();
            m_radiusProp = serializedObject.FindProperty("m_Radius");
            m_heightProp = serializedObject.FindProperty("m_Height");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_radiusProp, new GUIContent("Radius"));
            EditorGUILayout.PropertyField(m_heightProp, new GUIContent("Height"));
            Validate();

        }

        protected override void Validate() {
            NewtonConeCollider collision = (NewtonConeCollider)target;

            if (RadiusChangedAndValid(collision.Radius, m_radiusProp.floatValue, 0.01f) || HeightChangedAndValid(collision.Height, m_heightProp.floatValue, 0.01f)) {
                serializedObject.ApplyModifiedProperties();
                collision.RecreateEditorShape();
                //Debug.Log("Cone shape changed");
            }
        }

        SerializedProperty m_radiusProp;
        SerializedProperty m_heightProp;
    }

    [CustomEditor(typeof(NewtonChamferedCylinderCollider))]
    public class NewtonChamferedCylinderEditor : NewtonColliderEditor {
        void OnEnable() {
            base.SetupBaseProps();
            m_radiusProp = serializedObject.FindProperty("m_Radius");
            m_heightProp = serializedObject.FindProperty("m_Height");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_radiusProp, new GUIContent("Radius"));
            EditorGUILayout.PropertyField(m_heightProp, new GUIContent("Height"));
            Validate();
        }

        protected override void Validate() {
            NewtonChamferedCylinderCollider collision = (NewtonChamferedCylinderCollider)target;

            if (RadiusChangedAndValid(collision.Radius, m_radiusProp.floatValue, 0.01f) || HeightChangedAndValid(collision.Height, m_heightProp.floatValue, 0.01f)) {
                serializedObject.ApplyModifiedProperties();
                collision.RecreateEditorShape();
                //Debug.Log("Chamfer cylinder shape changed");
            }
        }

        SerializedProperty m_radiusProp;
        SerializedProperty m_heightProp;
    }

    [CustomEditor(typeof(NewtonConvexHullCollider))]
    public class NewtonConvexHullEditor : NewtonColliderEditor {
        void OnEnable() {
            base.SetupBaseProps();
            m_meshProp = serializedObject.FindProperty("m_Mesh");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(m_meshProp, new GUIContent("Mesh"));
            Validate();
        }

        protected override void Validate() {
            NewtonConvexHullCollider collision = (NewtonConvexHullCollider)target;

            if (collision.Mesh != (Mesh)m_meshProp.objectReferenceValue) {
                serializedObject.ApplyModifiedProperties();
                collision.RecreateEditorShape();
                //Debug.Log("Convex mesh changed");
            }
        }

        SerializedProperty m_meshProp;
    }

    [CustomEditor(typeof(NewtonTreeCollider))]
    public class NewtonTreeColliderEditor : NewtonColliderEditor {
        void OnEnable() {
            base.SetupBaseProps();
            m_meshProp = serializedObject.FindProperty("m_Mesh");
            m_optimizeProp = serializedObject.FindProperty("m_Optimize");
            m_freezeTransformProp = serializedObject.FindProperty("m_FreezeScale");
            m_rebuildMeshProp = serializedObject.FindProperty("m_RebuildMesh");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(m_meshProp, new GUIContent("Mesh"));
            EditorGUILayout.PropertyField(m_optimizeProp, new GUIContent("Optimize"));
            EditorGUILayout.PropertyField(m_freezeTransformProp, new GUIContent("Freeze Transform"));
            EditorGUILayout.PropertyField(m_rebuildMeshProp, new GUIContent("Rebuild mesh"));
            Validate();
        }

        protected override void Validate() {
            NewtonTreeCollider collision = (NewtonTreeCollider)target;

            if (collision.Mesh != (Mesh)m_meshProp.objectReferenceValue || collision.Optimize != m_optimizeProp.boolValue || collision.FreezeScale != m_freezeTransformProp.boolValue || collision.RebuildMesh != m_rebuildMeshProp.boolValue) {
                serializedObject.ApplyModifiedProperties();
                collision.RecreateEditorShape();
                //Debug.Log("Tree collider changed");
            }
        }

        SerializedProperty m_meshProp;
        SerializedProperty m_optimizeProp;
        SerializedProperty m_rebuildMeshProp;
        SerializedProperty m_freezeTransformProp;
    }
}
