using UnityEngine;
using UnityEditor;

/// <summary>
/// This base class handles the basic logic of removing the ring, setting the number of endpoints and radius, and handling overrides of the ring.
/// </summary>

namespace DigitalHorde.ConcentricRings {

    [CustomEditor(typeof(CRG_Ring))]
    public class CRG_Ring_Editor : CRG_Editor {

        #region Member Variables
        protected CRG_Ring m_RingBaseTarget;
        protected bool m_EnableOverrideSettings;
        protected bool m_NumberOfEndPointsChanged;
        protected bool m_RadiusChanged;
        protected bool m_EndPointChanged;
        protected bool m_RingLabelChanged;
        protected bool m_UpdateOrientation;
        protected bool m_UpdateEndPointOrientation;
        protected bool m_ShowOrientationSliders;
        protected string m_EndPointOverrideLabel = "End Point Object";
        #endregion

        #region Serialized Properties
        protected SerializedProperty m_RingLabel;
        protected SerializedProperty m_NumberOfEndPoints;
        protected SerializedProperty m_Radius;
        protected SerializedProperty m_OverrideSettings;
        protected SerializedProperty m_NumberOfEndPointsOverride;
        protected SerializedProperty m_RadiusOverride;
        protected SerializedProperty m_EndPoint;
        protected SerializedProperty m_XOrientation;
        protected SerializedProperty m_YOrientation;
        protected SerializedProperty m_ZOrientation;
        protected SerializedProperty m_OrientEndPointsTowardCenter;
        #endregion

        protected override void OnEnable() {

            base.OnEnable();
            m_RingBaseTarget = (CRG_Ring)target;

            m_RingLabel = serializedObject.FindProperty("m_RingLabel");
            m_NumberOfEndPoints = serializedObject.FindProperty("m_NumberOfEndPoints");
            m_Radius = serializedObject.FindProperty("m_Radius");
            m_OverrideSettings = serializedObject.FindProperty("m_OverrideSettings");
            m_NumberOfEndPointsOverride = serializedObject.FindProperty("m_NumberOfEndPointsOverride");
            m_RadiusOverride = serializedObject.FindProperty("m_RadiusOverride");
            m_XOrientation = serializedObject.FindProperty("m_XOrientation");
            m_YOrientation = serializedObject.FindProperty("m_YOrientation");
            m_ZOrientation = serializedObject.FindProperty("m_ZOrientation");
            m_OrientEndPointsTowardCenter = serializedObject.FindProperty("m_OrientEndPointsTowardCenter");
            m_EndPoint = serializedObject.FindProperty("m_EndPoint");

        }
        public override void OnInspectorGUI() {

            // Check if override bool is set on target (when loading a preset)
            if (m_RingBaseTarget.OverrideSettings && !m_EnableOverrideSettings) { m_EnableOverrideSettings = true; }

            ShowRingLabel();
            ShowOrientationSettings();
            ShowEndPointSelection();
            ShowEndPointSettings();

            if (GUILayout.Button("Remove Ring")) { m_RingBaseTarget.RemoveRing(); GUIUtility.ExitGUI(); }

            serializedObject.ApplyModifiedProperties();

            EndPointChangedCheck();
            RadiusChangeCheck();
            NumberOfEndPointsChangeCheck();
            UpdateOrientationCheck();
            SetEndPointOrientationCheck();
            UpdateRingLabel();

        }
        protected virtual void ShowEndPointSettings() {

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_OverrideSettings, new GUIContent("Override End Point Settings"), GUILayout.Height(20));
            if (EditorGUI.EndChangeCheck()) {

                m_NumberOfEndPointsChanged = true;
                m_RadiusChanged = true;

                if (!m_RingBaseTarget.OverrideSettings) { 
                    
                    m_EnableOverrideSettings = true;

                } else { 
                    
                    m_EnableOverrideSettings = false;

                }

            } 
                
            if (m_EnableOverrideSettings) {

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_NumberOfEndPointsOverride, new GUIContent("Number Of End Points"), GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck()) { m_NumberOfEndPointsChanged = true; }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_RadiusOverride, new GUIContent("Radius"), GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck()) { m_RadiusChanged = true; }

            } else {

                // Look for Number of End Points:
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_NumberOfEndPoints, new GUIContent("Number Of End Points"), GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck()) { m_NumberOfEndPointsChanged = true; }

                // Look for Radius Changes:
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_Radius, new GUIContent("Radius"), GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck()) { m_RadiusChanged = true; }

            }


        }
        protected virtual void ShowOrientationSettings() {

            m_ShowOrientationSliders = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowOrientationSliders, "Orientation Settings");

            if (m_ShowOrientationSliders) {

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_OrientEndPointsTowardCenter, new GUIContent("Orient End Points Toward Center"), GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck()) { m_UpdateEndPointOrientation = true; }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_XOrientation, new GUIContent("X Orientation"), GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck()) { m_UpdateOrientation = true; }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_YOrientation, new GUIContent("Y Orientation"), GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck()) { m_UpdateOrientation = true; }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_ZOrientation, new GUIContent("Z Orientation"), GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck()) { m_UpdateOrientation = true; }

            }

        }
        protected virtual void RadiusChangeCheck() {

            if (m_RadiusChanged) { m_RingBaseTarget.UpdateRadius(); m_RadiusChanged = false; }
        
        }
        protected virtual void NumberOfEndPointsChangeCheck() {

            if (m_NumberOfEndPointsChanged) { m_RingBaseTarget.ReGenerateRing(); m_NumberOfEndPointsChanged = false; }
        
        }
        protected virtual void ShowEndPointSelection() { // override this in implementations without base (adding a dropdown of object types for example) 

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_EndPoint, new GUIContent(m_EndPointOverrideLabel));
            if (EditorGUI.EndChangeCheck()) { m_EndPointChanged = true; }

        }
        protected virtual void EndPointChangedCheck() {

            if (m_RingBaseTarget.EndPoint != null && m_EndPointChanged) { m_RingBaseTarget.ReGenerateRing(); } // set a flag to regenerate instead?
            m_EndPointChanged = false;

        }
        protected virtual void ShowRingLabel() {

            GUILayout.Space(5f);

            DrawGradientBackground(0, 40, DarkGreyGradient);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_RingLabel, new GUIContent("Ring Name"), GUILayout.Height(20));
            if (EditorGUI.EndChangeCheck()) { m_RingLabelChanged = true; }

            GUILayout.Space(5f);

        }
        protected virtual void UpdateOrientationCheck() {

            if (m_UpdateOrientation) {

                m_RingBaseTarget.SetOrientation(new Vector3(m_RingBaseTarget.XOrientation, m_RingBaseTarget.YOrientation, m_RingBaseTarget.ZOrientation));

            }

            m_UpdateOrientation = false;

        }
        protected virtual void SetEndPointOrientationCheck() {

            if (m_UpdateEndPointOrientation) {
               
                m_RingBaseTarget.ResetEndPointOrientation(); // this is called when you click out and back in the inspector causing the stones to reset rotations

            }

            m_UpdateEndPointOrientation = false;

        }
        protected virtual void UpdateRingLabel() {

            if (m_RingLabelChanged) { m_RingBaseTarget.RingRoot.name = m_RingLabel.stringValue; }
            m_RingLabelChanged = false;

        }

    }

}
