using UnityEngine;
using UnityEditor;

namespace DigitalHorde.ConcentricRings.StoneCircles {

    [CustomEditor(typeof(StoneCircle_Manager))]
    public class StoneCircle_Manager_Editor : CRG_Manager_Editor {

        #region Member Variables & Properties
        StoneCircle_Manager m_StoneCircleManagerTarget;
        private int m_SelectedCenterPieceIndex = 0;
        private bool m_TerrainFound = false;
        private bool m_UpdateSnapToTerrain = false;
        private bool m_UpdateHeightOffset = false;
        private bool m_ShowAdvancedOptions = false;
        private bool m_UpdateCenterPieceHeightOffset = false;
        private bool m_UpdateIgnoreTerrainOffset = false;
        private bool m_UpdateHideCollidingObjects = false;
        private bool m_UpdateHideSelfOnCollidingObjects = false;
        private string m_CenterPiecesResourceLocation = SCG_Configuration.CENTERPIECE_RESOURCE_LOCATION;
        #endregion
        #region Serialized Properties
        SerializedProperty m_SnapEndPointsToTerrain;
        SerializedProperty m_HeightOffset;
        SerializedProperty m_IgnoreTerrainOffset; // only applies to the center object
        SerializedProperty m_CenterPieceHeightOffset;
        SerializedProperty m_HideCollidingObjects;
        SerializedProperty m_HideSelfOnCollidingObjects;
        #endregion
        #region Builtin Methods
        protected override void OnEnable() {

            base.OnEnable();
            m_StoneCircleManagerTarget = (StoneCircle_Manager)target;

            m_SnapEndPointsToTerrain = serializedObject.FindProperty("m_SnapEndPointsToTerrain");
            m_HeightOffset = serializedObject.FindProperty("m_HeightOffset");
            m_CenterPieceHeightOffset = serializedObject.FindProperty("m_CenterPieceHeightOffset");
            m_IgnoreTerrainOffset = serializedObject.FindProperty("m_IgnoreTerrainOffset");
            m_HideCollidingObjects = serializedObject.FindProperty("m_HideCollidingObjects");
            m_HideSelfOnCollidingObjects = serializedObject.FindProperty("m_HideSelfOnCollidingObjects");

        }
        public override void OnInspectorGUI() {

            if (m_StoneCircleManagerTarget.InstantiatedByAPI) {

                DrawGradientBackground(0f, 53, DarkGreyGradient);
                GUILayout.Label("Stone Circle Generator Instantiated by API", LabelStyle);
                return;

            }

            base.OnInspectorGUI();

            m_StoneCircleManagerTarget.CurrentCenterPieceFolderPath = UpdateResourcesDropDown(m_CenterPiecesResourceLocation, m_StoneCircleManagerTarget.CenterPiecesFoldersPaths, m_StoneCircleManagerTarget.CenterPiecesFolders, m_StoneCircleManagerTarget.CurrentCenterPieceIndex);
            serializedObject.ApplyModifiedProperties();

            UpdateCenterObjectDropDown();
            UpdateCenterPieceHeightOffset();
            UpdateCenterPieceIgnoreTerrainOffset();
            UpdateHeightOffset();
            UpdateSnapToTerrain();
            UpdateHideCollidingObjects();
            UpdateHideSelfOnObjectsCollision();

        }
        #endregion
        #region Custom Methods
        protected override void ShowRingManager() {

            Terrain[] terrains = Terrain.activeTerrains;
            if (terrains.Length > 0) { m_TerrainFound = true; }

            DrawGradientBackground(53, 50, DarkGreyGradient);

            GUILayout.Space(5f);

            float vPos = 133f;
            if (m_TerrainFound) {

                if (!m_ShowAdvancedOptions) { vPos = 155f; }
                else if (m_ShowAdvancedOptions && m_StoneCircleManagerTarget.HideCollidingObjects) { vPos = 200f; }
                else if (m_ShowAdvancedOptions && !m_StoneCircleManagerTarget.HideCollidingObjects) { vPos = 175f; }

            } else {

                if (m_ShowAdvancedOptions && m_StoneCircleManagerTarget.HideCollidingObjects) { vPos = 177f; }
                else if (m_ShowAdvancedOptions && !m_StoneCircleManagerTarget.HideCollidingObjects) { vPos = 156f; }

            }

            DrawGradientBackground(vPos, 89, DarkGreyGradient);

            ShowGlobalOptions(vPos);

            base.ShowRingManager();

        }
        protected virtual void ShowGlobalOptions(float vPos) {

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_YOrientation, new GUIContent("Rotate Stone Circle"), GUILayout.Height(20));
            if (EditorGUI.EndChangeCheck()) { m_UpdateOrientation = true; }

            if (m_TerrainFound) {

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_SnapEndPointsToTerrain, new GUIContent("Snap to Terrain"), GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck()) { m_UpdateSnapToTerrain = true; }

                if (m_StoneCircleManagerTarget.transform.hasChanged) {

                    if (m_StoneCircleManagerTarget.SnapEndPointsToTerrain) { m_UpdateSnapToTerrain = true; }
                    m_StoneCircleManagerTarget.transform.hasChanged = false;

                }

            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_HeightOffset, new GUIContent("Height Offset"), GUILayout.Height(20));
            if (EditorGUI.EndChangeCheck()) { m_UpdateHeightOffset = true; }

            m_ShowAdvancedOptions = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowAdvancedOptions, "Advanced Options");
            if (m_ShowAdvancedOptions) {

                EditorGUI.indentLevel++;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_HideCollidingObjects, new GUIContent("Hide Colliding Objects"), GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck()) { m_UpdateHideCollidingObjects = true; }

                if (m_StoneCircleManagerTarget.HideCollidingObjects) {

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(m_HideSelfOnCollidingObjects, new GUIContent("Hide Self On Collisions"), GUILayout.Height(20));
                    if (EditorGUI.EndChangeCheck()) { m_UpdateHideSelfOnCollidingObjects = true; }

                }

                EditorGUI.indentLevel--;

            }

            GUILayout.Space(10f);
            GUILayout.Space(5f);

        }
        protected override void ShowCenterObjectOptions() {

            if (m_StoneCircleManagerTarget.CurrentCenterPieceFolderPath != "") {

                // Show dropdown to center piece:
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Center Piece Folder", LabelStyle);
                m_SelectedCenterPieceIndex = EditorGUILayout.Popup(m_StoneCircleManagerTarget.CurrentCenterPieceIndex, m_StoneCircleManagerTarget.CenterPiecesFolders.ToArray());
                m_StoneCircleManagerTarget.LoadRandomCenterPiece();
                EditorGUILayout.EndHorizontal();

            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_CenterPrefab, new GUIContent("Override Center Piece"), GUILayout.Height(20));
            if (EditorGUI.EndChangeCheck()) { m_UpdateCenterObject = true; }

            if (m_StoneCircleManagerTarget.SnapEndPointsToTerrain) {

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_IgnoreTerrainOffset, new GUIContent("Ignore Terrain Offset"), GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck()) { m_UpdateIgnoreTerrainOffset = true; }

            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_CenterPieceHeightOffset, new GUIContent("Center Piece Height Offset"), GUILayout.Height(20));
            if (EditorGUI.EndChangeCheck()) { m_UpdateCenterPieceHeightOffset = true; }

        }
        protected void UpdateCenterObjectDropDown() {

            // Handle stone type change:
            if (m_StoneCircleManagerTarget.CurrentCenterPieceIndex != m_SelectedCenterPieceIndex) {

                m_StoneCircleManagerTarget.RandomPrefabLoaded = false;
                if (m_StoneCircleManagerTarget.UsingCenterPieceOverride) { m_StoneCircleManagerTarget.RemoveCenterObject(); }

                m_StoneCircleManagerTarget.CurrentCenterPieceIndex = m_SelectedCenterPieceIndex;
                m_StoneCircleManagerTarget.LoadRandomCenterPiece();

            }

        }
        protected virtual void UpdateCenterPieceHeightOffset() {

            if (m_UpdateCenterPieceHeightOffset) {

                m_StoneCircleManagerTarget.UpdateCenterPieceOffsetHeight();

            }

            m_UpdateCenterPieceHeightOffset = false;

        }
        protected virtual void UpdateCenterPieceIgnoreTerrainOffset() {

            if (m_UpdateIgnoreTerrainOffset) {

                m_StoneCircleManagerTarget.UpdateCenterPieceIgnoreTerrainOffset();

            }

            m_UpdateIgnoreTerrainOffset = false;

        }
        protected virtual void UpdateHeightOffset() {

            if (m_UpdateHeightOffset) {

                m_StoneCircleManagerTarget.SetHeightOffset();

            }

            m_UpdateHeightOffset = false;

        }
        protected virtual void UpdateSnapToTerrain() {

            if (m_UpdateSnapToTerrain) {

                if (m_StoneCircleManagerTarget.SnapEndPointsToTerrain) { 
                    
                    m_StoneCircleManagerTarget.BroadcastSnapToTerrain();

                } else { 
                    
                    m_StoneCircleManagerTarget.BroadcastUnSnapToTerrain();

                }

            }

            m_UpdateSnapToTerrain = false;

        }
        protected virtual void UpdateHideCollidingObjects() {

            if (m_UpdateHideCollidingObjects) {

                if (m_StoneCircleManagerTarget.HideCollidingObjects) {

                    m_StoneCircleManagerTarget.BroadcastHideObjectsOnCollision();

                } else if (!m_StoneCircleManagerTarget.HideCollidingObjects) {

                    m_StoneCircleManagerTarget.BroadcastUnHideObjectsOnCollision();

                }

                m_UpdateHideCollidingObjects = false;

            }

        }
        protected virtual void UpdateHideSelfOnObjectsCollision() {

            if (m_UpdateHideSelfOnCollidingObjects) {

                if (m_StoneCircleManagerTarget.HideSelfOnCollidingObjects) {

                    m_StoneCircleManagerTarget.BroadcastHideSelfOnObjectsCollision();

                } else if (!m_StoneCircleManagerTarget.HideSelfOnCollidingObjects) {

                    m_StoneCircleManagerTarget.BroadcastUnHideSelfOnObjectsCollision();

                }

                m_UpdateHideSelfOnCollidingObjects = false;

            }

        }
        
        #endregion

    }

}
