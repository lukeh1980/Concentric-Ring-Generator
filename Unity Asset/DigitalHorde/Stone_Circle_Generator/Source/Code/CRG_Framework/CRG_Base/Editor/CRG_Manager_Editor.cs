using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom editor for the VR_Concentric_Rings_Manager_Base class, overrides basic saving, loading and managing rings functionality of the editor.
/// </summary>

namespace DigitalHorde.ConcentricRings {

[CustomEditor(typeof(CRG_Manager))]
    public class CRG_Manager_Editor : CRG_Editor {

        #region Member Variables
        private CRG_Manager m_BaseTarget;
        private float m_ButtonWidth;
        private int m_ToolbarInt = 0;
        private readonly string[] m_ToolbarStrings = { "Manage Ring Set", "Saving & Loading" };
        protected bool m_ShowOrientationSliders = false;
        protected bool m_UpdateOrientation = false;
        protected bool m_UpdateCenterObject = false;
        protected bool m_UpdateJSONPreset = false;
        protected bool m_UpdateSavePositionRotation = false;
        #endregion

        #region Serialized Properties
        protected SerializedProperty m_JSONPreset;
        protected SerializedProperty m_SavePositionRotation;
        protected SerializedProperty m_CenterPrefab;
        protected SerializedProperty m_XOrientation;
        protected SerializedProperty m_YOrientation;
        protected SerializedProperty m_ZOrientation;
        #endregion

        #region Methods
        protected override void OnEnable() {

            base.OnEnable();
            m_BaseTarget = (CRG_Manager)target;

            m_JSONPreset = serializedObject.FindProperty("m_JSONPreset");
            m_SavePositionRotation = serializedObject.FindProperty("m_SavePositionRotation");
            m_CenterPrefab = serializedObject.FindProperty("m_CenterPrefab");
            m_XOrientation = serializedObject.FindProperty("m_XOrientation");
            m_YOrientation = serializedObject.FindProperty("m_YOrientation");
            m_ZOrientation = serializedObject.FindProperty("m_ZOrientation");

        }
        public override void OnInspectorGUI() {

            // Title bar:
            DrawGradientBackground(0f, 53, DarkGreyGradient);
            GUILayout.Label(m_BaseTarget.Title, LabelStyle);

            // Draw toolbar buttons:
            GUILayout.Space(3f);
            m_ToolbarInt = GUILayout.Toolbar(m_ToolbarInt, m_ToolbarStrings);
            m_ButtonWidth = (EditorGUIUtility.currentViewWidth * 0.5f) - 13f;

            // Toolbar options:
            GUILayout.Space(15f);
            if (m_ToolbarInt == 1) { ShowSavingLoading(); } else if (m_ToolbarInt == 0) { ShowRingManager(); }

            serializedObject.ApplyModifiedProperties();

            UpdateOrientationCheck();
            UpdateCenterObject();
            UpdateJSONPreset();

            SceneView.RepaintAll();

        }
        protected virtual void ShowSavingLoading() { // saving/loading section

            DrawGradientBackground(53, 62, DarkGreyGradient); // NOTE: we put our gradient background before the base (first param is position from top, second is gradient height).

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save As Prefab", GUILayout.Width(m_ButtonWidth))) { m_BaseTarget.SaveAsPrefab(); }
            if (GUILayout.Button("Save As Preset", GUILayout.Width(m_ButtonWidth))) { m_BaseTarget.SaveAsPreset(); }
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_SavePositionRotation, new GUIContent("Save Position and Rotation (Prefabs Only)"), GUILayout.Height(20));
            if (EditorGUI.EndChangeCheck()) { m_UpdateSavePositionRotation = true; }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_JSONPreset, new GUIContent("Load Saved Preset"), GUILayout.Height(20));
            if (EditorGUI.EndChangeCheck()) { m_UpdateJSONPreset = true; }

        }
        protected virtual void UpdateJSONPreset() {

            if (m_UpdateJSONPreset) {

                if (m_BaseTarget.JSONPreset != null) {

                    m_BaseTarget.LoadPreset(m_BaseTarget.JSONPreset);

                }

                m_UpdateJSONPreset = false;

            }

        }
        protected virtual void ShowRingManager() { // ring manager section

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add New Ring", GUILayout.Width(m_ButtonWidth))) { m_BaseTarget.AddRing(); }
            
            // These two lines set the toggle to show the "Add Center Object" options.
            if (!m_BaseTarget.ToggleUseCenterObject && m_BaseTarget.Center == null) { 
                
                if (GUILayout.Button("Add Center Object", GUILayout.Width(m_ButtonWidth))) { m_BaseTarget.ToggleUseCenterObject = true; } 
            
            } else if (m_BaseTarget.ToggleUseCenterObject || m_BaseTarget.Center != null) { 
                
                if (GUILayout.Button("Remove Center Object", GUILayout.Width(m_ButtonWidth))) { m_BaseTarget.RemoveCenterObject(); m_BaseTarget.ToggleUseCenterObject = false; } 
            
            }
            
            EditorGUILayout.EndHorizontal();

            if (m_BaseTarget.ToggleUseCenterObject || m_BaseTarget.Center) { ShowCenterObjectOptions(); } // toggle on the Add Center Object options

        }
        protected virtual void ShowGlobalOptions() { // override this method to limit axes

            m_ShowOrientationSliders = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowOrientationSliders, "Adjust Ring Set Orientation");

            if (m_ShowOrientationSliders) {

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

            EditorGUILayout.EndFoldoutHeaderGroup();

        }
        protected virtual void ShowCenterObjectOptions() {

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_CenterPrefab, new GUIContent("Center Object"), GUILayout.Height(20));
            if (EditorGUI.EndChangeCheck()) { m_UpdateCenterObject = true; }

        }
        protected virtual void UpdateOrientationCheck() {

            if (m_UpdateOrientation) { m_BaseTarget.SetOrientation(new Vector3(m_BaseTarget.XOrientation, m_BaseTarget.YOrientation, m_BaseTarget.ZOrientation)); }
            m_UpdateOrientation = false;

        }
        protected virtual void UpdateCenterObject() {

            if (m_UpdateCenterObject) { // this is true if a prefab was dropped in the slot

                if (m_BaseTarget.Center != m_BaseTarget.CenterPrefab && m_BaseTarget.CenterPrefab != null) { m_BaseTarget.AddCenterObject(); m_BaseTarget.UsingCenterPieceOverride = true; }
                else if (m_BaseTarget.CenterPrefab == null) { m_BaseTarget.RemoveCenterObject(); }

            }

            m_UpdateCenterObject = false;

        }
        #endregion

    }

}
