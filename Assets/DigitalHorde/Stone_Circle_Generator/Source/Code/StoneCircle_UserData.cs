using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)
namespace DigitalHorde.ConcentricRings.StoneCircles {
    [DisallowMultipleComponent]
    public class StoneCircle_UserData : CRG_UserData {

        public void Start() {

            this.hideFlags = HideFlags.HideInInspector;

        }

        protected override void SetFileInformation() {

            m_PresetName = "Stone_Circle_Preset_" + DateTimeOffset.Now.ToUnixTimeSeconds();
            m_PrefabName = "Stone_Circle_" + DateTimeOffset.Now.ToUnixTimeSeconds();
            m_ParentPath = "Assets/DigitalHorde/Stone_Circle_Generator/UserData";

        }
        protected override void PrefabProcessing(GameObject prefab) {

            Transform[] allChildren = prefab.GetComponentsInChildren<Transform>(true);
            List<GameObject> children = new List<GameObject>();

            foreach (Transform child in allChildren) {

                if (!child.gameObject.activeSelf) {

                    children.Add(child.gameObject);

                    if (child.transform.parent.GetComponent<StoneCircle_EndPoint>()) { children.Add(child.transform.parent.gameObject); }

                }

            }

            for (int i = 0; i < children.Count; i++) { DestroyImmediate(children[i]); }

            foreach (var comp in prefab.GetComponentsInChildren<StoneCircle_EndPoint>()) { DestroyImmediate(comp); }

        }
        public override void SaveAsPreset() {

            StoneCircle_Preset savedPreset = new StoneCircle_Preset();
            StoneCircle_Manager manager = GetComponent<StoneCircle_Manager>();

            savedPreset.stoneCircleManager.position = RingManager.transform.position;
            savedPreset.stoneCircleManager.xOrientation = RingManager.XOrientation;
            savedPreset.stoneCircleManager.yOrientation = RingManager.YOrientation;
            savedPreset.stoneCircleManager.zOrientation = RingManager.ZOrientation;

            if (manager.Center != null && manager.CenterPrefab != null) {

                savedPreset.stoneCircleManager.useCenterPieceOverride = true;
                savedPreset.stoneCircleManager.centerPiecePath = AssetDatabase.GetAssetPath(RingManager.CenterPrefab);

            } else if (manager.Center != null && manager.CenterPrefab == null) {

                savedPreset.stoneCircleManager.useCenterPieceOverride = false;
                savedPreset.stoneCircleManager.centerPieceIndex = manager.CurrentCenterPieceIndex;
                savedPreset.stoneCircleManager.centerPiecePath = manager.CurrentCenterPieceFolderPath;

            }

            savedPreset.stoneCircleManager.ignoreTerrainOffset = GetComponent<StoneCircle_Manager>().IgnoreTerrainOffset;
            savedPreset.stoneCircleManager.snapEndPointsToTerrain = GetComponent<StoneCircle_Manager>().SnapEndPointsToTerrain;
            savedPreset.stoneCircleManager.endPointHeightOffset = GetComponent<StoneCircle_Manager>().HeightOffset;

            savedPreset.stoneCircleManager.hideCollidingTerrainObjects = GetComponent<StoneCircle_Manager>().HideCollidingObjects;
            savedPreset.stoneCircleManager.hideSelfOnCollidingTerrainObjects = GetComponent<StoneCircle_Manager>().HideSelfOnCollidingObjects;

            List<StoneCircle_Preset.StoneCircle_RingSettings> rings = new List<StoneCircle_Preset.StoneCircle_RingSettings>();

            foreach (var ring in manager.StoneCircleRingsList) {

                StoneCircle_Preset.StoneCircle_RingSettings ringSettings = new StoneCircle_Preset.StoneCircle_RingSettings();
                ringSettings = GenerateRingSettings(ring);
                rings.Add(ringSettings);

            }

            savedPreset.stoneCircleRingsList = rings;
            string json = JsonUtility.ToJson(savedPreset, true);
            SaveJSONToFile(json);

        }
        protected virtual StoneCircle_Preset.StoneCircle_RingSettings GenerateRingSettings(StoneCircle_Ring ring) {

            StoneCircle_Preset.StoneCircle_RingSettings ringSettings = new StoneCircle_Preset.StoneCircle_RingSettings();

            ringSettings.ringID = ring.RingID;
            ringSettings.ringLabel = ring.RingLabel;

            ringSettings.currentEndPointFolderPath = ring.CurrentEndPointsFolderPath;
            ringSettings.currentEndPointFolderIndex = ring.CurrentEndPointsFolderIndex;

            if (ring.EndPoint != null) { 
                
                ringSettings.useEndPointOverride = true;
                ringSettings.endPointOverridePath = AssetDatabase.GetAssetPath(ring.EndPoint);

            } else {

                ringSettings.useEndPointOverride = false;
                ringSettings.endPointOverridePath = "";

            }

            ringSettings.overrideSettings = ring.OverrideSettings;
            ringSettings.numberOfEndPointsOverride = ring.NumberOfEndPointsOverride;
            ringSettings.radiusOverride = ring.RadiusOverride;
            ringSettings.numberOfEndPoints = ring.NumberOfEndPoints;
            ringSettings.radius = ring.Radius;
            ringSettings.entranceGapSize = ring.EntranceGapSize;
            ringSettings.ringOffsetHeight = ring.RingOffsetHeight;
            ringSettings.ignoreTerrainOffset = ring.IgnoreTerrainOffset;

            ringSettings.orientEndPointsTowardCenter = ring.OrientEndPointsTowardCenter;
            ringSettings.xOrientation = ring.XOrientation;
            ringSettings.yOrientation = ring.YOrientation;
            ringSettings.zOrientation = ring.ZOrientation;

            ringSettings.randomizeEndPointRotation = ring.RandomizeEndPointRotation;
            ringSettings.minMaxRandomRotationSlider = ring.MinMaxRandomRotationSlider;
            ringSettings.randomizeEndPointTiltLean = ring.RandomizeEndPointTiltLean;
            ringSettings.minMaxTiltLeanSlider = ring.MinMaxTiltLeanSlider;

            ringSettings.randomizeEndPointPlacement = ring.RandomizeEndPointPlacement;
            ringSettings.minMaxEndPointPlacementSlider = ring.MinMaxEndPointPlacementSlider;
            ringSettings.randomizeEndPointRadius = ring.RandomizeEndPointRadius;
            ringSettings.minMaxEndPointRadiusSlider = ring.MinMaxEndPointRadiusSlider;

            ringSettings.randomizeEndPointScale = ring.RandomizeEndPointScale;
            ringSettings.keepProportion = ring.KeepProportion;
            ringSettings.randomizeEndPointScaleSlider = ring.RandomizeEndPointScaleSlider;

            ringSettings.randomizeEndPointGaps = ring.RandomizeEndPointGaps;
            ringSettings.randomizeEndPointGapsSlider = ring.RandomizeEndPointGapsSlider;
            ringSettings.replaceGapsWithRubble = ring.ReplaceGapsWithRubble;
            ringSettings.currentGapsRubbleFolderIndex = ring.CurrentGapsRubbleFolderIndex;
            ringSettings.currentGapsRubbleFolderPath = ring.CurrentGapsRubbleFolderPath;

            return ringSettings;

        }
        public override void LoadPreset(TextAsset JSON) {

            StoneCircle_Manager manager = GetComponent<StoneCircle_Manager>();

            StoneCircle_Preset json = JsonUtility.FromJson<StoneCircle_Preset>(JSON.text);
            manager.JSONPreset = JSON;

            // remove all existing rings and ring components:
            StoneCircle_Ring[] currentRings = gameObject.GetComponents<StoneCircle_Ring>();
            foreach (var ring in currentRings) {

                DestroyImmediate(ring.RingRoot);
                DestroyImmediate(ring);

            }

            manager.StoneCircleRingsList.Clear();

            if (manager.Center != null) { DestroyImmediate(manager.Center); }

            // update orientation settings:
            manager.XOrientation = json.stoneCircleManager.xOrientation;
            manager.YOrientation = json.stoneCircleManager.yOrientation;
            manager.ZOrientation = json.stoneCircleManager.zOrientation;

            if (json.stoneCircleManager.useCenterPieceOverride) {

                // check for center peice and add it:
                if (!String.IsNullOrEmpty(json.stoneCircleManager.centerPiecePath)) {

                    GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(json.stoneCircleManager.centerPiecePath);
                    manager.Center = null;
                    manager.CenterPrefab = obj;
                    manager.AddCenterObject();

                }

            } else if (!json.stoneCircleManager.useCenterPieceOverride && !String.IsNullOrEmpty(json.stoneCircleManager.centerPiecePath)) {

                // Load random center piece from directory:
                manager.CurrentCenterPieceIndex = json.stoneCircleManager.centerPieceIndex;
                manager.LoadRandomCenterPiece();

            }

            manager.SnapEndPointsToTerrain = json.stoneCircleManager.snapEndPointsToTerrain;
            manager.HeightOffset = json.stoneCircleManager.endPointHeightOffset;
            manager.IgnoreTerrainOffset = json.stoneCircleManager.ignoreTerrainOffset;

            manager.HideCollidingObjects = json.stoneCircleManager.hideCollidingTerrainObjects;
            manager.HideSelfOnCollidingObjects = json.stoneCircleManager.hideSelfOnCollidingTerrainObjects;

            // add each ring:
            for (int i = 0; i < json.stoneCircleRingsList.Count; i++) { AddRingFromPreset(json.stoneCircleRingsList[i], manager); }

            manager.SetOrientation(new Vector3(manager.XOrientation, manager.YOrientation, manager.ZOrientation));

            if (manager.HideCollidingObjects) { manager.BroadcastHideObjectsOnCollision(); }
            if (manager.HideSelfOnCollidingObjects) { manager.BroadcastHideSelfOnObjectsCollision(); }
            if (manager.IgnoreTerrainOffset) { manager.UpdateCenterPieceOffsetHeight(); }

            manager.JSONPreset = null;

        }
        protected virtual void AddRingFromPreset(StoneCircle_Preset.StoneCircle_RingSettings ringSettings, StoneCircle_Manager manager) {

            // create a new ring:
            StoneCircle_Ring newRing = gameObject.AddComponent<StoneCircle_Ring>();

            GameObject ringRoot = new GameObject();

            newRing.RingID = ringSettings.ringID;
            newRing.RingRoot = ringRoot;
            newRing.RingRoot.name = ringSettings.ringLabel;
            newRing.RingRoot.transform.SetPositionAndRotation(transform.position, transform.rotation);
            newRing.RingRoot.transform.parent = transform;
            newRing.StoneCircleManager = manager;
            newRing.RingLabel = ringSettings.ringLabel;
            newRing.CurrentEndPointsFolderPath = ringSettings.currentEndPointFolderPath;
            newRing.CurrentEndPointsFolderIndex = ringSettings.currentEndPointFolderIndex;

            if (ringSettings.useEndPointOverride) {

                // check for center peice and add it:
                if (!String.IsNullOrEmpty(ringSettings.endPointOverridePath)) {

                    GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(ringSettings.endPointOverridePath);
                    newRing.EndPoint = obj;

                }

            }

            newRing.OverrideSettings = ringSettings.overrideSettings;

            if (ringSettings.overrideSettings) {

                newRing.NumberOfEndPointsOverride = ringSettings.numberOfEndPointsOverride;
                newRing.RadiusOverride = ringSettings.radiusOverride;

            }

            newRing.NumberOfEndPoints = ringSettings.numberOfEndPoints;
            newRing.Radius = ringSettings.radius;
            newRing.LastRadius = ringSettings.radius;

            newRing.EntranceGapSize = ringSettings.entranceGapSize;
            newRing.RingOffsetHeight = ringSettings.ringOffsetHeight;
            newRing.IgnoreTerrainOffset = ringSettings.ignoreTerrainOffset;

            newRing.OrientEndPointsTowardCenter = ringSettings.orientEndPointsTowardCenter;
            newRing.XOrientation = ringSettings.xOrientation;
            newRing.YOrientation = ringSettings.yOrientation;
            newRing.ZOrientation = ringSettings.zOrientation;

            newRing.RandomizeEndPointRotation = ringSettings.randomizeEndPointRotation;
            newRing.MinMaxRandomRotationSlider = ringSettings.minMaxRandomRotationSlider;
            newRing.RandomizeEndPointTiltLean = ringSettings.randomizeEndPointTiltLean;
            newRing.MinMaxTiltLeanSlider = ringSettings.minMaxTiltLeanSlider;

            newRing.RandomizeEndPointPlacement = ringSettings.randomizeEndPointPlacement;
            newRing.MinMaxEndPointPlacementSlider = ringSettings.minMaxEndPointPlacementSlider;
            newRing.RandomizeEndPointRadius = ringSettings.randomizeEndPointRadius;
            newRing.MinMaxEndPointRadiusSlider = ringSettings.minMaxEndPointRadiusSlider;

            newRing.RandomizeEndPointScale = ringSettings.randomizeEndPointScale;
            newRing.KeepProportion = ringSettings.keepProportion;
            newRing.RandomizeEndPointScaleSlider = ringSettings.randomizeEndPointScaleSlider;

            newRing.RandomizeEndPointGaps = ringSettings.randomizeEndPointGaps;
            newRing.RandomizeEndPointGapsSlider = ringSettings.randomizeEndPointGapsSlider;
            newRing.ReplaceGapsWithRubble = ringSettings.replaceGapsWithRubble;
            newRing.CurrentGapsRubbleFolderIndex = ringSettings.currentGapsRubbleFolderIndex;
            newRing.CurrentGapsRubbleFolderPath = ringSettings.currentGapsRubbleFolderPath;

            manager.StoneCircleRingsList.Add(newRing);

            newRing.ReGenerateRing();
            newRing.SetOrientation(new Vector3(newRing.XOrientation, newRing.YOrientation, newRing.ZOrientation));

        }

    }

}
#endif