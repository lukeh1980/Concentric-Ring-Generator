using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.Net;

//#if (UNITY_EDITOR) 

/// <summary>
/// This extends the base class adding/overriding these features:
///     - limit orientation changes to the Y axis only
///     - Pull endpoints from directories
///     - Add randomization options
/// </summary>

namespace DigitalHorde.ConcentricRings.StoneCircles {

    public class StoneCircle_Ring : CRG_Ring {

        #region Member Variables & Properties
        [SerializeField] private StoneCircle_Manager m_StoneCircleManager;
        public StoneCircle_Manager StoneCircleManager { get { return m_StoneCircleManager; } set { m_StoneCircleManager = value; } }
        public List<GameObject> RandomEndPointOverrideList { get; set; } = new List<GameObject>();
        public List<GameObject> RandomRubbleOverrideList { get; set; } = new List<GameObject>();
        [SerializeField] private int m_CurrentEndPointsFolderIndex = 0;
        public int CurrentEndPointsFolderIndex { get { return m_CurrentEndPointsFolderIndex; } set { m_CurrentEndPointsFolderIndex = value; } }
        [SerializeField] private List<string> m_EndPointsFoldersList = new List<string>();
        public List<string> EndPointsFoldersList { get { return m_EndPointsFoldersList; } }
        [SerializeField] private List<string> m_EndPointsFoldersPathsList = new List<string>();
        public List<string> EndPointsFoldersPathsList { get { return m_EndPointsFoldersPathsList; } }
        [SerializeField] private string m_CurrentEndPointsFolderPath;
        public string CurrentEndPointsFolderPath { get { return m_CurrentEndPointsFolderPath; } set { m_CurrentEndPointsFolderPath = value; } }
        [SerializeField] private List<GameObject> m_HiddenEndPoints = new List<GameObject>();
        public List<GameObject> EntranceGapHiddenEndPoints { get { return m_HiddenEndPoints; } set { m_HiddenEndPoints = value; } }
        [SerializeField] private bool m_RingGenerated = false;
        public bool RingGenerated { get { return m_RingGenerated; } set { m_RingGenerated = value; } }

        // Height Offset:
        [SerializeField] float m_RingOffsetHeight = 0f;
        public float RingOffsetHeight { get { return m_RingOffsetHeight; } set { m_RingOffsetHeight = value; } }
        [SerializeField] bool m_IgnoreTerrainOffset = false;
        public bool IgnoreTerrainOffset { get { return m_IgnoreTerrainOffset; } set { m_IgnoreTerrainOffset = value; } }

        // Entrance gap size:
        [Range(0, 70)] [SerializeField] int m_EntranceGapSize = 0;
        public int EntranceGapSize { get { return m_EntranceGapSize; } set { m_EntranceGapSize = value; } }

        // Randomize end point Rotations:
        [SerializeField] bool m_RandomizeEndPointRotation = false;
        public bool RandomizeEndPointRotation { get { return m_RandomizeEndPointRotation; } set { m_RandomizeEndPointRotation = value; } }

#if (UNITY_EDITOR) 
        [MinMaxSlider(-90f, 90f)] 
#endif 
        [SerializeField] private Vector2 m_MinMaxRandomRotationSlider = new Vector2(-20f, 20f);
        public Vector2 MinMaxRandomRotationSlider { get { return m_MinMaxRandomRotationSlider; } set { m_MinMaxRandomRotationSlider = value; } }

        // Randomize end point Tilt & Lean:
        [SerializeField] bool m_RandomizeEndPointTiltLean = false;
        public bool RandomizeEndPointTiltLean { get { return m_RandomizeEndPointTiltLean; } set { m_RandomizeEndPointTiltLean = value; } }

#if (UNITY_EDITOR)
        [MinMaxSlider(-10f, 10f)]
#endif
        [SerializeField] private Vector2 m_MinMaxTiltLeanSlider = new Vector2(-3f, 3f);
        public Vector2 MinMaxTiltLeanSlider { get { return m_MinMaxTiltLeanSlider; } set { m_MinMaxTiltLeanSlider = value; } }

        // Randomize end point Placement:
        [SerializeField] bool m_RandomizeEndPointPlacement = false;
        public bool RandomizeEndPointPlacement { get { return m_RandomizeEndPointPlacement; } set { m_RandomizeEndPointPlacement = value; } }

#if (UNITY_EDITOR)
        [MinMaxSlider(-90f, 90f)]
#endif
        [SerializeField] private Vector2 m_MinMaxEndPointPlacementSlider = new Vector2(-20f, 20f);
        public Vector2 MinMaxEndPointPlacementSlider { get { return m_MinMaxEndPointPlacementSlider; } set { m_MinMaxEndPointPlacementSlider = value; } }

        // Randomize end point Radius:
        [SerializeField] bool m_RandomizeEndPointRadius = false;
        public bool RandomizeEndPointRadius { get { return m_RandomizeEndPointRadius; } set { m_RandomizeEndPointRadius = value; } }

#if (UNITY_EDITOR)
        [MinMaxSlider(-50f, 50f)]
#endif
        [SerializeField] private Vector2 m_MinMaxEndPointRadiusSlider = new Vector2(-3f, 3f);
        public Vector2 MinMaxEndPointRadiusSlider { get { return m_MinMaxEndPointRadiusSlider; } set { m_MinMaxEndPointRadiusSlider = value; } }

        [SerializeField] private float m_LastRadius;
        public float LastRadius { get { return m_LastRadius; } set { m_LastRadius = value; } }  

        // Randomize endpoint scale:
        [SerializeField] bool m_RandomizeEndPointScale = false;
        public bool RandomizeEndPointScale { get { return m_RandomizeEndPointScale; } set { m_RandomizeEndPointScale = value; } }
        [SerializeField] bool m_KeepProportion = false;
        public bool KeepProportion { get { return m_KeepProportion; } set { m_KeepProportion = value; } }

#if (UNITY_EDITOR)
        [MinMaxSlider(0.5f, 3.0f)]
#endif
        [SerializeField] Vector2 m_RandomizeEndPointScaleSlider = new Vector2(0.8f, 1.5f);
        public Vector2 RandomizeEndPointScaleSlider { get { return m_RandomizeEndPointScaleSlider; } set { m_RandomizeEndPointScaleSlider = value; } }

        // Randomize endpoint gaps:
        [SerializeField] List<GameObject> m_RandomEndPointGapsList = new List<GameObject>();
        public List<GameObject> RandomEndPointGapsList { get { return m_RandomEndPointGapsList; } set { m_RandomEndPointGapsList = value; } }
        [SerializeField] bool m_RandomizeEndPointGaps = false;
        public bool RandomizeEndPointGaps { get { return m_RandomizeEndPointGaps; } set { m_RandomizeEndPointGaps = value; } }
#if (UNITY_EDITOR)
        [MinMaxSlider(0f, 100f)]
#endif
        [SerializeField] Vector2 m_RandomizeEndPointGapsSlider = new Vector2(10f, 20f);
        public Vector2 RandomizeEndPointGapsSlider { get { return m_RandomizeEndPointGapsSlider; } set { m_RandomizeEndPointGapsSlider = value; } }
        [SerializeField] private bool m_ReplaceGapsWithRubble = false;
        public bool ReplaceGapsWithRubble { get { return m_ReplaceGapsWithRubble; } set { m_ReplaceGapsWithRubble = value; } }
        [SerializeField] private int m_CurrentGapsRubbleFolderIndex = 0;
        public int CurrentGapsRubbleFolderIndex { get { return m_CurrentGapsRubbleFolderIndex; } set { m_CurrentGapsRubbleFolderIndex = value; } }
        [SerializeField] private List<string> m_GapsRubbleFoldersList = new List<string>();
        public List<string> GapsRubbleFoldersList { get { return m_GapsRubbleFoldersList; } }
        [SerializeField] private List<string> m_GapsRubbleFoldersPathsList = new List<string>();
        public List<string> GapsRubbleFoldersPathsList { get { return m_GapsRubbleFoldersPathsList; } }
        [SerializeField] private string m_CurrentGapsRubbleFolderPath;
        public string CurrentGapsRubbleFolderPath { get { return m_CurrentGapsRubbleFolderPath; } set { m_CurrentGapsRubbleFolderPath = value; } }
        [SerializeField] private List<GameObject> m_GapsRubbleList = new List<GameObject>();
        public List<GameObject> GapsRubbleList { get { return m_GapsRubbleList; } set { m_GapsRubbleList = value; } }

        public bool InstantiatedByAPI { get; set; } = false;
#endregion

        #region Builtin Methods

        protected virtual void OnEnable() { LastRadius = Radius; }
        protected void Start() {

            if (InstantiatedByAPI) { this.hideFlags = HideFlags.HideInInspector; }

        }
        #endregion

        #region Custom Methods
        protected override void ReGenerateEndPoints() { // Override this function in implementations to add objects to the positions generated.

            ResetPlacement();
            
            // Destroy endpoint objects before regenerating:
            for (int i = 0; i < EndPointsObjects.Count;) { if (EndPointsObjects[i].gameObject != null) { DestroyImmediate(EndPointsObjects[i]); } else { i++; } }
            EndPointsObjects.Clear();
            RemoveRubble();

            for (int i = 0; i < EndPointsPositions.Count; i++) {

                GameObject endPoint = null;

                if (RandomEndPointOverrideList.Count > 0) {

                    endPoint = LoadRandomPrefab(RandomEndPointOverrideList);

                } else {

                    #if (UNITY_EDITOR)
                    endPoint = LoadRandomPrefab(CurrentEndPointsFolderPath);
                    #endif

                }

                if (endPoint != null) {

                    endPoint.transform.SetParent(RingRoot.transform, false); // parent the new object first then set the local position and rotation
                    endPoint.transform.localPosition = EndPointsPositions[i].localPosition;
                    endPoint.transform.localRotation = EndPointsPositions[i].localRotation;
                    endPoint.name = "Stone " + (i + 1);
                    endPoint.AddComponent<StoneCircle_EndPoint>();

                    if (GetComponent<StoneCircle_Manager>().SnapEndPointsToTerrain) { endPoint.GetComponent<StoneCircle_EndPoint>().SnapToTerrain(); }

                    EndPointsObjects.Add(endPoint);

                }

            }

            ResetEndPointOrientation();
            UpdateEntranceGapSize();
            UpdateRingOffsetHeight();
            UpdateIgnoreTerrainOffset();
            CheckRandomizationOptions();

            RingGenerated = true; // if this checks false when ring is added then the ring will generate automatically.

            if (StoneCircleManager.HideCollidingObjects) { StoneCircleManager.BroadcastHideObjectsOnCollision(); }
            if (StoneCircleManager.HideSelfOnCollidingObjects) { StoneCircleManager.BroadcastHideSelfOnObjectsCollision(); }

        }
        protected virtual void CheckRandomizationOptions() {

            if (RandomizeEndPointRotation) { SetEndPointRandomRotation(MinMaxRandomRotationSlider); }
            if (RandomizeEndPointTiltLean) { SetEndPointRandomTiltLean(MinMaxTiltLeanSlider); }
            if (RandomizeEndPointPlacement) { RandomizePlacement(MinMaxEndPointPlacementSlider); }
            if (RandomizeEndPointRadius) { RandomizeRadiusDistance(MinMaxEndPointRadiusSlider); }
            if (RandomizeEndPointGaps) { RandomizeGaps(RandomizeEndPointGapsSlider); }
            if (RandomizeEndPointScale) { SetRandomizeEndPointScale(RandomizeEndPointScaleSlider); }

        }
        protected virtual GameObject LoadRandomPrefab(List<GameObject> sourcePrefabList) {

            GameObject endPoint = null;

            if (EndPoint != null) { endPoint = Instantiate(EndPoint); } else {

                if (sourcePrefabList.Count > 0) {

                    int randomIndex = UnityEngine.Random.Range(0, sourcePrefabList.Count - 1);
                    if (sourcePrefabList[randomIndex] != null) { endPoint = Instantiate(sourcePrefabList[randomIndex]); }

                }

            }

            endPoint = ProcessLoadedPrefab(endPoint);
            return endPoint;

        }
#if (UNITY_EDITOR)
        protected virtual GameObject LoadRandomPrefab(string sourceFolderPath) {

            GameObject endPoint = null;
            if (EndPoint != null) { endPoint = Instantiate(EndPoint); } else {
                
                if (System.IO.Directory.Exists(sourceFolderPath)) {

                    List<string> endPointFiles = new List<string>();
                    string[] files = System.IO.Directory.GetFiles(sourceFolderPath);

                    if (files.Length > 0) {

                        foreach (string file in files) {

                            // Only load files with prefab extension and add it to list of stones:
                            if (string.Equals(file.Split('.').Last(), "prefab", StringComparison.OrdinalIgnoreCase)) { endPointFiles.Add(file); }

                        }

                        // Load random prefab from list:
                        GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(endPointFiles[UnityEngine.Random.Range(0, endPointFiles.Count)]);
                        endPoint = Instantiate(asset, new Vector3(0, 0, 0), Quaternion.identity);

                    }

                }

            }

            endPoint = ProcessLoadedPrefab(endPoint);
            return endPoint;

        }
#endif
        protected virtual GameObject ProcessLoadedPrefab(GameObject prefab) {

            if (prefab == null) { return prefab; }

            if (prefab.transform.childCount < 1) { // prefab doesn't have a parent, create an empty and parent the prefab to it, return parent.

                GameObject temp = new GameObject();
                temp.name = prefab.name;

                prefab.transform.position = Vector3.zero;
                prefab.transform.rotation = Quaternion.identity;
                temp.transform.position = Vector3.zero;
                temp.transform.rotation = Quaternion.identity;

                prefab.transform.SetParent(temp.transform);
                return temp;

            } else { return prefab; }

        }
        public virtual void RandomizePlacement(Vector2 range) {

            ResetPlacement(); // reset each time random placement is calculated.

            int numEndPoints = GetNumberOfEndPoints();

            for (int i = 0; i < EndPointsObjects.Count; i++) {

                float xRange = range.x / 100f; // slider ranges are percentages
                float yRange = range.y / 100f;
                float deg = 360f / (numEndPoints * 2); // get half the number of degrees between each stone

                float randomPlacement = UnityEngine.Random.Range(deg * xRange, deg * yRange); // rotate by random percentage of degrees between stones
                EndPointsObjects[i].transform.RotateAround(transform.position, Vector3.up, randomPlacement);

            }

            if (!RandomizeEndPointRotation) { ResetEndPointOrientation(); } else { SetEndPointRandomRotation(MinMaxRandomRotationSlider); }
            if (RandomizeEndPointTiltLean) { SetEndPointRandomTiltLean(MinMaxTiltLeanSlider); }
            if (RandomizeEndPointRadius) { RandomizeRadiusDistance(MinMaxEndPointRadiusSlider); }
            if (ReplaceGapsWithRubble) { RepositionRubble(); }

        }
        public virtual void ResetPlacement() {

            float radius = GetRadius();

            List<EndPoint> tempEndPointPositions = new List<EndPoint>();
            tempEndPointPositions = GenerateEndPointPositions(EndPointsObjects.Count, radius);

            for (int i = 0; i < EndPointsObjects.Count; i++) {

                EndPointsObjects[i].transform.position = tempEndPointPositions[i].position;
                EndPointsObjects[i].transform.localPosition = tempEndPointPositions[i].localPosition;

            }

            if (ReplaceGapsWithRubble) { RepositionRubble(); }

        }
        public override void UpdateRadius() { // Basic method of adjusting the distance from center for each endpoint, override this if (for example) you want random distances.

            RingRoot.transform.rotation = Quaternion.identity;
            float radius = GetRadius();

            GameObject tempRingRoot = new GameObject();
            tempRingRoot.transform.position = RingRoot.transform.position;

            for (int i = 0; i < EndPointsObjects.Count; i++) {

                tempRingRoot.transform.position = new Vector3(tempRingRoot.transform.position.x, EndPointsObjects[i].transform.position.y, tempRingRoot.transform.position.z);
                tempRingRoot.transform.LookAt(EndPointsObjects[i].transform);

                float distance = Vector3.Distance(tempRingRoot.transform.position, EndPointsObjects[i].transform.position);
                float adjustedRadius;

                if (distance > LastRadius) {

                    float adjusted = distance - LastRadius;
                    adjustedRadius = radius + adjusted;
                
                } else {

                    float adjusted = LastRadius - distance;
                    adjustedRadius = radius - adjusted;

                }

                EndPointsObjects[i].transform.localPosition = tempRingRoot.transform.forward * adjustedRadius;

                if (RandomEndPointGapsList.Contains(EndPointsObjects[i]) && ReplaceGapsWithRubble) {

                    for (int ii = 0; ii < RandomEndPointGapsList.Count; ii++) {

                        if (EndPointsObjects[i] == RandomEndPointGapsList[ii] && ii < GapsRubbleList.Count) { GapsRubbleList[ii].transform.localPosition = tempRingRoot.transform.forward * adjustedRadius; }

                    }

                }

            }

            DestroyImmediate(tempRingRoot);
            LastRadius = radius;
            Vector3 orientation = new Vector3(0f, YOrientation, 0f);
            SetOrientation(orientation);

        }
        public virtual void RandomizeRadiusDistance(Vector2 range) {

            float ringRadius = GetRadius();
            RingRoot.transform.rotation = Quaternion.identity;

            GameObject tempRingRoot = new GameObject();
            tempRingRoot.transform.position = RingRoot.transform.position;

            for (int i = 0; i < EndPointsObjects.Count; i++) {

                float randomRadius = UnityEngine.Random.Range(range.x, range.y); // negative is closer to center, positive is further away from center
                float distanceOffset;
                float calculatedRadius;

                if (randomRadius > 0) {

                    float percentage = randomRadius / 100f;
                    distanceOffset = ringRadius * percentage;
                    calculatedRadius = ringRadius + distanceOffset;

                } else {

                    float percentage = Mathf.Abs(randomRadius) / 100f;
                    distanceOffset = ringRadius * percentage;
                    calculatedRadius = ringRadius - distanceOffset;

                }

                tempRingRoot.transform.LookAt(EndPointsObjects[i].transform);
                EndPointsObjects[i].transform.localPosition = tempRingRoot.transform.forward * calculatedRadius;

            }

            DestroyImmediate(tempRingRoot);
            
            if (!RandomizeEndPointRotation) { ResetEndPointOrientation(); } else { SetEndPointRandomRotation(MinMaxRandomRotationSlider); }
            if (RandomizeEndPointTiltLean) { SetEndPointRandomTiltLean(MinMaxTiltLeanSlider); }

            Vector3 orientation = new Vector3(0f, YOrientation, 0f);
            SetOrientation(orientation);

            if (ReplaceGapsWithRubble) { RepositionRubble(); }

        }
        public virtual void ResetRadiusDistance() {

            float ringRadius = GetRadius();
            RingRoot.transform.rotation = Quaternion.identity;

            GameObject tempRingRoot = new GameObject();
            tempRingRoot.transform.position = RingRoot.transform.position;

            for (int i = 0; i < EndPointsObjects.Count; i++) {

                tempRingRoot.transform.position = new Vector3(tempRingRoot.transform.position.x, EndPointsObjects[i].transform.position.y, tempRingRoot.transform.position.z);
                tempRingRoot.transform.LookAt(EndPointsObjects[i].transform);
                EndPointsObjects[i].transform.localPosition = tempRingRoot.transform.forward * ringRadius;

            }

            DestroyImmediate(tempRingRoot);

            if (!RandomizeEndPointRotation) { ResetEndPointOrientation(); } else { SetEndPointRandomRotation(MinMaxRandomRotationSlider); }
            if (RandomizeEndPointTiltLean) { SetEndPointRandomTiltLean(MinMaxTiltLeanSlider); }

            Vector3 orientation = new Vector3(0f, YOrientation, 0f);
            SetOrientation(orientation);

            if (ReplaceGapsWithRubble) { RepositionRubble(); }

        }
        public virtual void UpdateIgnoreTerrainOffset() {

            foreach (GameObject go in EndPointsObjects) {

                go.GetComponentInChildren<StoneCircle_EndPoint>().IgnoreTerrainOffset = IgnoreTerrainOffset;

            }

            if (GapsRubbleList.Count > 0) {

                foreach (GameObject go in GapsRubbleList) {

                    go.GetComponentInChildren<StoneCircle_EndPoint>().IgnoreTerrainOffset = IgnoreTerrainOffset;

                }

            }

        }
        public virtual void UpdateRingOffsetHeight() {

            foreach (GameObject go in EndPointsObjects) {

                go.GetComponentInChildren<StoneCircle_EndPoint>().HeightOffsetOverride = RingOffsetHeight + StoneCircleManager.HeightOffset;

            }

            if (GapsRubbleList.Count > 0) {

                foreach (GameObject go in GapsRubbleList) {

                    go.GetComponentInChildren<StoneCircle_EndPoint>().HeightOffsetOverride = RingOffsetHeight + StoneCircleManager.HeightOffset;

                }

            }

        }
        public virtual void UpdateEntranceGapSize() {

            if (EndPointsObjects.Count < 1) { return; }

            for (int i = 0; i < EndPointsObjects.Count; i++) {

                if (!RandomEndPointGapsList.Contains(EndPointsObjects[i])) { EndPointsObjects[i].SetActive(true); }
            
            }

            if (ReplaceGapsWithRubble) { for (int i = 0; i < GapsRubbleList.Count; i++) { GapsRubbleList[i].SetActive(true); } }

            int numStones = GetNumberOfEndPoints();

            float stonesToRemove = Mathf.Round(numStones * (EntranceGapSize / 100f));
            for (int i = 0; i < stonesToRemove; i++) {

                if (EndPointsObjects[i]) {

                    EndPointsObjects[i].SetActive(false);
                    if (RandomEndPointGapsList.Contains(EndPointsObjects[i]) && ReplaceGapsWithRubble) {

                        for (int ii = 0; ii < RandomEndPointGapsList.Count; ii++) {

                            if (RandomEndPointGapsList[ii] == EndPointsObjects[i] && ii < GapsRubbleList.Count) { GapsRubbleList[ii].SetActive(false); }

                        }
                    
                    }

                }

            }

        }
        public virtual void RandomizeGaps(Vector2 range) { // this is used as a percentage

            RandomEndPointGapsList.Clear();
            RemoveRubble();

            if (EndPointsObjects.Count < 1) { return; }

            for (int i = 0; i < EndPointsObjects.Count; i++) { EndPointsObjects[i].SetActive(true); }

            int numberOfEndpoints = GetNumberOfEndPoints();

            if (numberOfEndpoints > 0) {

                float randomPercentage = UnityEngine.Random.Range(range.x, range.y) / 100f;
                float numStonesToHide = Mathf.Round(numberOfEndpoints * randomPercentage);

                for (int i = 0; i < numStonesToHide; i++) {

                    int rand = UnityEngine.Random.Range(0, EndPointsObjects.Count);
                    if (EndPointsObjects[rand].activeSelf) {

                        EndPointsObjects[rand].SetActive(false);
                        RandomEndPointGapsList.Add(EndPointsObjects[rand]);

                    }

                }

            }

            if (ReplaceGapsWithRubble) { RandomizeGapRubble(); }
            UpdateEntranceGapSize();

        }
        public virtual void ResetGaps() {

            for (int i = 0; i < EndPointsObjects.Count; i++) {

                EndPointsObjects[i].SetActive(true);

            }

            RandomEndPointGapsList.Clear();
            UpdateEntranceGapSize();

        }
        public virtual void RandomizeGapRubble() {

            RemoveRubble();

            for (int i = 0; i < RandomEndPointGapsList.Count; i++) {

                if (EntranceGapHiddenEndPoints.Contains(RandomEndPointGapsList[i])) { continue; }

                GameObject rubble = null;

                if (RandomRubbleOverrideList.Count > 0) {

                    rubble = LoadRandomPrefab(RandomRubbleOverrideList);

                } else {

                    #if (UNITY_EDITOR)
                    rubble = LoadRandomPrefab(CurrentGapsRubbleFolderPath);
                    #endif

                }

                if (rubble != null) {

                    rubble.transform.SetParent(RingRoot.transform, false); // parent the new object first then set the local position and rotation
                    rubble.transform.localPosition = RandomEndPointGapsList[i].gameObject.transform.localPosition;
                    rubble.transform.Rotate(0f, UnityEngine.Random.Range(0f, 360f), 0f, Space.Self);

                    rubble.name = "Rubble " + (i + 1);
                    rubble.AddComponent<StoneCircle_EndPoint>();
                    if (GetComponent<StoneCircle_Manager>().SnapEndPointsToTerrain) { rubble.GetComponent<StoneCircle_EndPoint>().EnableSnapToTerrain = true; }

                    GapsRubbleList.Add(rubble); // add to rubble list

                }

            }

        }
        protected virtual void RepositionRubble() {

            if (GapsRubbleList.Count < 1) { return; }

            for (int i = 0; i < RandomEndPointGapsList.Count; i++) {

                GapsRubbleList[i].transform.localPosition = RandomEndPointGapsList[i].transform.localPosition;

            }

        }
        public virtual void RemoveRubble() {

            for (int i = 0; i < GapsRubbleList.Count; i++) { DestroyImmediate(GapsRubbleList[i]); }
            GapsRubbleList.Clear();

        }
        public virtual void SetEndPointRandomRotation(Vector2 range) {

            ResetEndPointOrientation(); // reset orientation before applying random rotations.

            for (int i = 0; i < EndPointsObjects.Count; i++) {

                // randomly flip 180 degrees:
                float random = UnityEngine.Random.Range(0f, 1f);
                if (random > 0.5f) { EndPointsObjects[i].transform.Rotate(0f, 180f, 0f, Space.Self); }

                float yRot = UnityEngine.Random.Range(range.x, range.y);
                EndPointsObjects[i].transform.Rotate(0f, yRot, 0f, Space.Self);

            }

            if (RandomizeEndPointTiltLean) { SetEndPointRandomTiltLean(MinMaxTiltLeanSlider); }

            if (ReplaceGapsWithRubble && GapsRubbleList.Count > 0) {

                for (int i = 0; i < RandomEndPointGapsList.Count; i++) {

                    float yRot = UnityEngine.Random.Range(range.x, range.y);
                    GapsRubbleList[i].transform.Rotate(0f, yRot, 0f, Space.Self);

                }

            }

        }
        public virtual void SetEndPointRandomTiltLean(Vector2 tiltLean) {

            ResetEndPointTiltLean(); // reset tilt & lean before applying random rotations.

            for (int i = 0; i < EndPointsObjects.Count; i++) {

                float randomX = UnityEngine.Random.Range(tiltLean.x, tiltLean.y);
                float randomZ = UnityEngine.Random.Range(tiltLean.x, tiltLean.y);
                EndPointsObjects[i].transform.localRotation = Quaternion.Euler(randomX, 0, randomZ);

            }

        }
        public virtual void ResetEndPointTiltLean() {

            for (int i = 0; i < EndPointsObjects.Count; i++) {

                EndPointsObjects[i].transform.localEulerAngles = new Vector3(0, EndPointsObjects[i].transform.localEulerAngles.y, 0);

            }

        }
        public override void ResetEndPointOrientation() {

            if (OrientEndPointsTowardCenter) {

                for (int i = 0; i < EndPointsObjects.Count; i++) {

                    Vector3 pos = new Vector3(RingRoot.transform.position.x, EndPointsObjects[i].transform.position.y, RingRoot.transform.position.z);
                    EndPointsObjects[i].transform.LookAt(pos);

                }

            } else {

                for (int i = 0; i < EndPointsObjects.Count; i++) {

                    EndPointsObjects[i].transform.rotation = Quaternion.identity;

                }

            }

        }
        public override void SetOrientation(Vector3 orientation) {

            RingRoot.transform.localRotation = Quaternion.Euler(orientation);
            //ResetEndPointOrientation(); // remove this in the override so stones don't align when rotating orientation...this option works well on the base class to keep end points oriented toward the center when rotating all axes.

        }
        public virtual void SetRandomizeEndPointScale(Vector2 range) {

            ResetEndPointScale();
            Vector3 newScale;

            for (int i = 0; i < EndPointsObjects.Count; i++) {

                if (KeepProportion) {

                    float randomScale = UnityEngine.Random.Range(range.x, range.y);
                    newScale = new Vector3(randomScale, randomScale, randomScale);

                } else {

                    float x = UnityEngine.Random.Range(range.x, range.y);
                    float y = UnityEngine.Random.Range(range.x, range.y);
                    float z = UnityEngine.Random.Range(range.x, range.y);
                    newScale = new Vector3(x, y, z);

                }

                EndPointsObjects[i].transform.localScale = newScale;

            }

        }
        public virtual void ResetEndPointScale() {

            for (int i = 0; i < EndPointsObjects.Count; i++) {

                EndPointsObjects[i].transform.localScale = new Vector3(1, 1, 1);

            }

        }
        public virtual void SetEndPointProportion() {

            for (int i = 0; i < EndPointsObjects.Count; i++) {

                float[] scaleArray = { EndPointsObjects[i].transform.localScale.x, EndPointsObjects[i].transform.localScale.y, EndPointsObjects[i].transform.localScale.z };
                float max = Mathf.Max(scaleArray);
                EndPointsObjects[i].transform.localScale = new Vector3(max, max, max);

            }

        }
        public virtual void ResetEndPointProportion() {

            if (RandomizeEndPointScale) { SetRandomizeEndPointScale(RandomizeEndPointScaleSlider); }

        }
        public override void RemoveRing() {

            StoneCircleManager.RemoveRing(this);

        }
#endregion

    }

}