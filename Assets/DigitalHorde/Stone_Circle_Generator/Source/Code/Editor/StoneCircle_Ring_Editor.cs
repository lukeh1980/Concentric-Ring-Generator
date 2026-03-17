using UnityEngine;
using UnityEditor;

namespace DigitalHorde.ConcentricRings.StoneCircles {

    [ExecuteInEditMode]
    [CustomEditor(typeof(StoneCircle_Ring))]
    public class StoneCircle_Ring_Editor : CRG_Ring_Editor {

        #region Member Variables & Properties
        StoneCircle_Ring m_StoneCircleRingTarget;

        private int m_SelectedEndPointFolderIndex = 0;
        private bool m_ShowRandomizeEndPointRotations = false;
        private bool m_ShowRandomizeEndPointPlacement = false;
        private bool m_ShowRandomizeEndPointScale = false;
        private bool m_ShowRandomizeEndPointGaps = false;
        private bool m_ShowEndPointTypeOptions = true;
        private bool m_ShowRingSettings = true;

        private bool m_UpdateRingOffsetHeight = false;
        private bool m_UpdateIgnoreTerrainOffset = false;
        private bool m_UpdateEntranceGapSize = false;
        private bool m_UpdateRandomRotations = false;
        private bool m_UpdateRandomTiltLean = false;
        private bool m_UpdateRandomEndPointPlacement = false;
        private bool m_UpdateRandomEndPointRadius = false;
        private bool m_UpdateRandomEndPointScale = false;
        private bool m_UpdateKeepProportion = false;
        private bool m_UpdateRandomEndPointGaps = false;
        private bool m_UpdateReplaceGapsWithRubble = false;
        private int m_SelectedGapsRubbleFolderIndex = 0;
        private string m_EndPointResourceLocation = SCG_Configuration.ENDPOINT_RESOURCE_LOCATION;
        #endregion
        #region Serialized Properties
        SerializedProperty m_EntranceGapSize;
        SerializedProperty m_RingOffsetHeight;
        SerializedProperty m_IgnoreTerrainOffset;

        SerializedProperty m_RandomizeEndPointRotation;
        SerializedProperty m_MinMaxRandomRotationSlider;
        SerializedProperty m_RandomizeEndPointTiltLean;
        SerializedProperty m_MinMaxTiltLeanSlider;

        SerializedProperty m_RandomizeEndPointPlacement;
        SerializedProperty m_MinMaxEndPointPlacementSlider;
        SerializedProperty m_RandomizeEndPointRadius;
        SerializedProperty m_MinMaxEndPointRadiusSlider;

        SerializedProperty m_RandomizeEndPointGaps;
        SerializedProperty m_RandomizeEndPointGapsSlider;
        SerializedProperty m_ReplaceGapsWithRubble;

        SerializedProperty m_RandomizeEndPointScale;
        SerializedProperty m_KeepProportion;
        SerializedProperty m_RandomizeEndPointScaleSlider;
        #endregion

        #region Builtin Methods
        protected override void OnEnable() {

            base.OnEnable();

            m_StoneCircleRingTarget = (StoneCircle_Ring)target;
            m_SelectedGapsRubbleFolderIndex = m_StoneCircleRingTarget.CurrentGapsRubbleFolderIndex; // this updates the variable so the rubble type isn't reset when foldout is collapsed.

            m_EntranceGapSize = serializedObject.FindProperty("m_EntranceGapSize");
            m_RingOffsetHeight = serializedObject.FindProperty("m_RingOffsetHeight");
            m_IgnoreTerrainOffset = serializedObject.FindProperty("m_IgnoreTerrainOffset");

            if (m_StoneCircleRingTarget.RingOffsetHeight != 0) { m_UpdateRingOffsetHeight = true; }

            m_RandomizeEndPointRotation = serializedObject.FindProperty("m_RandomizeEndPointRotation");
            m_MinMaxRandomRotationSlider = serializedObject.FindProperty("m_MinMaxRandomRotationSlider");
            m_RandomizeEndPointTiltLean = serializedObject.FindProperty("m_RandomizeEndPointTiltLean");
            m_MinMaxTiltLeanSlider = serializedObject.FindProperty("m_MinMaxTiltLeanSlider");

            m_RandomizeEndPointPlacement = serializedObject.FindProperty("m_RandomizeEndPointPlacement");
            m_MinMaxEndPointPlacementSlider = serializedObject.FindProperty("m_MinMaxEndPointPlacementSlider");
            m_RandomizeEndPointRadius = serializedObject.FindProperty("m_RandomizeEndPointRadius");
            m_MinMaxEndPointRadiusSlider = serializedObject.FindProperty("m_MinMaxEndPointRadiusSlider");

            m_RandomizeEndPointGaps = serializedObject.FindProperty("m_RandomizeEndPointGaps");
            m_RandomizeEndPointGapsSlider = serializedObject.FindProperty("m_RandomizeEndPointGapsSlider");
            m_ReplaceGapsWithRubble = serializedObject.FindProperty("m_ReplaceGapsWithRubble");

            m_RandomizeEndPointScale = serializedObject.FindProperty("m_RandomizeEndPointScale");
            m_KeepProportion = serializedObject.FindProperty("m_KeepProportion");
            m_RandomizeEndPointScaleSlider = serializedObject.FindProperty("m_RandomizeEndPointScaleSlider");

        }
        
        public override void OnInspectorGUI() {

            if (m_StoneCircleRingTarget.InstantiatedByAPI) {

                DrawGradientBackground(0f, 53, DarkGreyGradient);
                GUILayout.Label("Instantiated by API", LabelStyle);
                return;

            }

            base.OnInspectorGUI();

            m_StoneCircleRingTarget.CurrentEndPointsFolderPath = UpdateResourcesDropDown(m_EndPointResourceLocation, m_StoneCircleRingTarget.EndPointsFoldersPathsList, m_StoneCircleRingTarget.EndPointsFoldersList, m_StoneCircleRingTarget.CurrentEndPointsFolderIndex);
            m_StoneCircleRingTarget.CurrentGapsRubbleFolderPath = UpdateResourcesDropDown(m_EndPointResourceLocation, m_StoneCircleRingTarget.GapsRubbleFoldersPathsList, m_StoneCircleRingTarget.GapsRubbleFoldersList, m_StoneCircleRingTarget.CurrentGapsRubbleFolderIndex);

            UpdateRingOffsetHeight();
            UpdateIgnoreTerrainOffset();
            UpdateEntranceGapSize();
            UpdateEndPointRotations();
            UpdateEndPointTiltLean();
            UpdateEndPointPlacement();
            UpdateEndPointRadiusDistance();
            UpdateRandomEndPointGaps();
            UpdateRandomEndPointScale();

        }
        
        #endregion
        #region Custom Methods
        protected override void ShowEndPointSettings() {

            m_ShowRingSettings = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowRingSettings, "End Point Settings");

            if (m_ShowRingSettings) {

                EditorGUI.indentLevel++;

                base.ShowEndPointSettings();

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_EntranceGapSize, new GUIContent("Entrance Gap Size"), GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck()) { m_UpdateEntranceGapSize = true; }

                EditorGUI.indentLevel--;

            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #region Random Stone Rotations Foldout
            m_ShowRandomizeEndPointRotations = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowRandomizeEndPointRotations, "Randomize Stone Rotations");
            if (m_ShowRandomizeEndPointRotations) {

                EditorGUI.indentLevel++;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_RandomizeEndPointRotation, new GUIContent("Enable"), GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck()) { m_UpdateRandomRotations = true; }

                if (m_StoneCircleRingTarget.RandomizeEndPointRotation) {

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(m_MinMaxRandomRotationSlider, new GUIContent("Random Degrees Range"), GUILayout.Height(20));
                    if (EditorGUI.EndChangeCheck()) { m_UpdateRandomRotations = true; }

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(m_RandomizeEndPointTiltLean, new GUIContent("Randomize Tilt & Lean"), GUILayout.Height(20));
                    if (m_StoneCircleRingTarget.RandomizeEndPointTiltLean) { EditorGUILayout.PropertyField(m_MinMaxTiltLeanSlider, new GUIContent("Random Degrees Range"), GUILayout.Height(20)); }
                    if (EditorGUI.EndChangeCheck()) { m_UpdateRandomTiltLean = true; }

                }

            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion
            #region Random Stone Placement Foldout
            m_ShowRandomizeEndPointPlacement = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowRandomizeEndPointPlacement, "Randomize Stone Placement");
            if (m_ShowRandomizeEndPointPlacement) {

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_RandomizeEndPointPlacement, new GUIContent("Enable"), GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck()) { m_UpdateRandomEndPointPlacement = true; }

                if (m_StoneCircleRingTarget.RandomizeEndPointPlacement) {

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(m_MinMaxEndPointPlacementSlider, new GUIContent("Random Percentage Range"), GUILayout.Height(20));
                    if (EditorGUI.EndChangeCheck()) { m_UpdateRandomEndPointPlacement = true; }

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(m_RandomizeEndPointRadius, new GUIContent("Randomize Radius Distance"), GUILayout.Height(20));
                    if (m_StoneCircleRingTarget.RandomizeEndPointRadius) { EditorGUILayout.PropertyField(m_MinMaxEndPointRadiusSlider, new GUIContent("Random Percentage Range"), GUILayout.Height(20)); }
                    if (EditorGUI.EndChangeCheck()) { m_UpdateRandomEndPointRadius = true; }

                }

            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion
            #region Random Stone Scale Foldout
            m_ShowRandomizeEndPointScale = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowRandomizeEndPointScale, "Randomize Stone Scale");
            if (m_ShowRandomizeEndPointScale) {

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_RandomizeEndPointScale, new GUIContent("Enable"), GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck()) { m_UpdateRandomEndPointScale = true; }

                if (m_StoneCircleRingTarget.RandomizeEndPointScale) {

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(m_KeepProportion, new GUIContent("Keep Proportion"), GUILayout.Height(20));
                    if (EditorGUI.EndChangeCheck()) { m_UpdateKeepProportion = true; }

                    EditorGUI.BeginChangeCheck();
                    if (m_StoneCircleRingTarget.RandomizeEndPointScale) { EditorGUILayout.PropertyField(m_RandomizeEndPointScaleSlider, new GUIContent("Random Scale Range"), GUILayout.Height(20)); }
                    if (EditorGUI.EndChangeCheck()) { m_UpdateRandomEndPointScale = true; }

                }

            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion
            #region Random Stone Gaps Foldout
            m_ShowRandomizeEndPointGaps = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowRandomizeEndPointGaps, "Randomize Stone Gaps");
            if (m_ShowRandomizeEndPointGaps) { // this dropdown being colapsed is causing the issue, if opened, keep it open, remember on all sections?

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_RandomizeEndPointGaps, new GUIContent("Enable"), GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck()) { m_UpdateRandomEndPointGaps = true; }

                if (m_StoneCircleRingTarget.RandomizeEndPointGaps) {

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(m_RandomizeEndPointGapsSlider, new GUIContent("Random Percentage Range"), GUILayout.Height(20));
                    if (EditorGUI.EndChangeCheck()) { m_UpdateRandomEndPointGaps = true; }

                    if (m_StoneCircleRingTarget.CurrentGapsRubbleFolderPath != "") {

                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(m_ReplaceGapsWithRubble, new GUIContent("Replace Gaps with Rubble"), GUILayout.Height(20));
                        if (EditorGUI.EndChangeCheck()) { m_UpdateReplaceGapsWithRubble = true; }

                        if (m_StoneCircleRingTarget.ReplaceGapsWithRubble) {

                            // Show dropdown:
                            EditorGUI.indentLevel++;
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PrefixLabel("Rubble Type Folder", LabelStyle);
                            EditorGUI.indentLevel--;
                            m_SelectedGapsRubbleFolderIndex = EditorGUILayout.Popup(m_StoneCircleRingTarget.CurrentGapsRubbleFolderIndex, m_StoneCircleRingTarget.GapsRubbleFoldersList.ToArray());
                            EditorGUILayout.EndHorizontal();

                        }

                    }

                }

            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            GUILayout.Space(10f);
            #endregion

        }
        protected override void ShowOrientationSettings() {

            m_ShowOrientationSliders = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowOrientationSliders, "Orientation & Offset");

            if (m_ShowOrientationSliders) {

                EditorGUI.indentLevel++;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_RingOffsetHeight, new GUIContent("Offset Ring Height"), GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck()) { m_UpdateRingOffsetHeight = true; }

                if (m_StoneCircleRingTarget.StoneCircleManager.SnapEndPointsToTerrain) {

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(m_IgnoreTerrainOffset, new GUIContent("Ignore Terrain Offset"), GUILayout.Height(20));
                    if (EditorGUI.EndChangeCheck()) { m_UpdateIgnoreTerrainOffset = true; }

                }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_OrientEndPointsTowardCenter, new GUIContent("Orient End Points Toward Center"), GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck()) { m_UpdateEndPointOrientation = true; }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_YOrientation, new GUIContent("Y Orientation"), GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck()) { m_UpdateOrientation = true; }

                EditorGUI.indentLevel--;

            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();

        }
        protected override void ShowEndPointSelection() { // override this in implementations without base (adding a dropdown of object types for example) 

            m_ShowEndPointTypeOptions = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowEndPointTypeOptions, "Stone Type Options");

            if (m_ShowEndPointTypeOptions) {

                if (m_StoneCircleRingTarget.CurrentEndPointsFolderPath != "") {

                    EditorGUI.indentLevel++;

                    // Show dropdown:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Stone Type Folder", LabelStyle);
                    EditorGUI.indentLevel--;
                    m_SelectedEndPointFolderIndex = EditorGUILayout.Popup(m_StoneCircleRingTarget.CurrentEndPointsFolderIndex, m_StoneCircleRingTarget.EndPointsFoldersList.ToArray());
                    EditorGUILayout.EndHorizontal();

                }

                EditorGUI.indentLevel++;

                m_EndPointOverrideLabel = "Stone Type Override";

                base.ShowEndPointSelection();

                EditorGUI.indentLevel--;

            }

            EditorGUILayout.EndFoldoutHeaderGroup();

        }
        protected override void EndPointChangedCheck() {

            if (m_EndPointChanged) { m_RingBaseTarget.ReGenerateRing(); } // end point override
            m_EndPointChanged = false; // end point override

            // Handle stone type change:
            if (m_StoneCircleRingTarget.CurrentEndPointsFolderIndex != m_SelectedEndPointFolderIndex) {

                m_StoneCircleRingTarget.CurrentEndPointsFolderIndex = m_SelectedEndPointFolderIndex;
                m_StoneCircleRingTarget.CurrentEndPointsFolderPath = UpdateResourcesDropDown(m_EndPointResourceLocation, m_StoneCircleRingTarget.EndPointsFoldersPathsList, m_StoneCircleRingTarget.EndPointsFoldersList, m_StoneCircleRingTarget.CurrentEndPointsFolderIndex);
                m_StoneCircleRingTarget.ReGenerateRing(); // regenerate if the folder has changed.

            } else if (!m_StoneCircleRingTarget.RingGenerated) {

                // generate ring when added
                m_StoneCircleRingTarget.CurrentEndPointsFolderPath = UpdateResourcesDropDown(m_EndPointResourceLocation, m_StoneCircleRingTarget.EndPointsFoldersPathsList, m_StoneCircleRingTarget.EndPointsFoldersList, m_StoneCircleRingTarget.CurrentEndPointsFolderIndex);
                m_StoneCircleRingTarget.ReGenerateRing();

            }

        }
        protected virtual void UpdateRingOffsetHeight() {

            if (m_UpdateRingOffsetHeight) { m_StoneCircleRingTarget.UpdateRingOffsetHeight(); }

            m_UpdateRingOffsetHeight = false;

        }
        protected virtual void UpdateIgnoreTerrainOffset() {

            if (m_UpdateIgnoreTerrainOffset) {

                m_StoneCircleRingTarget.UpdateIgnoreTerrainOffset();
            
            }

            m_UpdateIgnoreTerrainOffset = false;

        }
        protected virtual void UpdateEntranceGapSize() {

            if (m_UpdateEntranceGapSize) { m_StoneCircleRingTarget.UpdateEntranceGapSize(); }

            m_UpdateEntranceGapSize = false;

        }
        protected virtual void UpdateEndPointRotations() {
            
            if (m_UpdateRandomRotations) {

                if (!m_StoneCircleRingTarget.RandomizeEndPointRotation) { 
                    
                    m_StoneCircleRingTarget.RandomizeEndPointTiltLean = false; 
                    m_StoneCircleRingTarget.ResetEndPointOrientation(); 
                
                } else {

                    m_StoneCircleRingTarget.SetEndPointRandomRotation(m_StoneCircleRingTarget.MinMaxRandomRotationSlider);

                }

            }

            m_UpdateRandomRotations = false;

        }
        protected virtual void UpdateEndPointTiltLean() {

            if (m_UpdateRandomTiltLean) {

                if (!m_StoneCircleRingTarget.RandomizeEndPointTiltLean) { m_StoneCircleRingTarget.ResetEndPointTiltLean(); } else {

                    m_StoneCircleRingTarget.SetEndPointRandomTiltLean(m_StoneCircleRingTarget.MinMaxTiltLeanSlider);

                }

            }

            m_UpdateRandomTiltLean = false;

        }

        protected virtual void UpdateEndPointPlacement() {

            if (m_UpdateRandomEndPointPlacement) {

                if (m_StoneCircleRingTarget.RandomizeEndPointPlacement) {

                    m_StoneCircleRingTarget.RandomizePlacement(m_StoneCircleRingTarget.MinMaxEndPointPlacementSlider); 
                
                } else {
                    
                    m_StoneCircleRingTarget.RandomizeEndPointPlacement = false;
                    m_StoneCircleRingTarget.RandomizeEndPointRadius = false;
                    m_StoneCircleRingTarget.ResetPlacement(); // restore the placement positions.

                }

            }

            m_UpdateRandomEndPointPlacement = false;

        }
        protected virtual void UpdateEndPointRadiusDistance() {

            if (m_UpdateRandomEndPointRadius) {

                if (m_StoneCircleRingTarget.RandomizeEndPointRadius) {

                    m_StoneCircleRingTarget.RandomizeRadiusDistance(m_StoneCircleRingTarget.MinMaxEndPointRadiusSlider); 
                
                } else {

                    m_StoneCircleRingTarget.ResetRadiusDistance();

                }

            }

            m_UpdateRandomEndPointRadius = false;    

        }
        protected virtual void UpdateRandomEndPointGaps() {

            if (m_UpdateRandomEndPointGaps) {

                if (m_StoneCircleRingTarget.RandomizeEndPointGaps) {

                    m_StoneCircleRingTarget.RandomizeGaps(m_StoneCircleRingTarget.RandomizeEndPointGapsSlider);

                } else {

                    if (m_StoneCircleRingTarget.ReplaceGapsWithRubble) { m_StoneCircleRingTarget.RemoveRubble(); }
                    m_StoneCircleRingTarget.ResetGaps();

                }

            }

            m_UpdateRandomEndPointGaps = false;

            if (m_UpdateReplaceGapsWithRubble) {

                if (m_StoneCircleRingTarget.ReplaceGapsWithRubble) {

                    m_StoneCircleRingTarget.RandomizeGapRubble();

                } else {

                    m_StoneCircleRingTarget.RemoveRubble();

                }

            }

            m_UpdateReplaceGapsWithRubble = false;

            // Handle rubble type change:
            if (m_StoneCircleRingTarget.CurrentGapsRubbleFolderIndex != m_SelectedGapsRubbleFolderIndex) {

                m_StoneCircleRingTarget.CurrentGapsRubbleFolderIndex = m_SelectedGapsRubbleFolderIndex;
                m_StoneCircleRingTarget.CurrentGapsRubbleFolderPath = UpdateResourcesDropDown(m_EndPointResourceLocation, m_StoneCircleRingTarget.GapsRubbleFoldersPathsList, m_StoneCircleRingTarget.GapsRubbleFoldersList, m_StoneCircleRingTarget.CurrentGapsRubbleFolderIndex);
                m_StoneCircleRingTarget.RandomizeGapRubble();

            }

        }
        protected virtual void UpdateRandomEndPointScale() {

            if (m_UpdateRandomEndPointScale) {

                if (m_StoneCircleRingTarget.RandomizeEndPointScale) {

                    m_StoneCircleRingTarget.SetRandomizeEndPointScale(m_StoneCircleRingTarget.RandomizeEndPointScaleSlider);

                } else {

                    m_StoneCircleRingTarget.ResetEndPointScale();

                }

            }

            if (m_UpdateKeepProportion) {

                if (m_StoneCircleRingTarget.KeepProportion) {

                    m_StoneCircleRingTarget.SetEndPointProportion();

                } else {

                    m_StoneCircleRingTarget.ResetEndPointProportion();

                }

            }

            m_UpdateRandomEndPointScale = false;
            m_UpdateKeepProportion = false;

        }
        #endregion

    }

}