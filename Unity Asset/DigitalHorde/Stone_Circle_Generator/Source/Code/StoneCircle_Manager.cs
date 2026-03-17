using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

/// <summary>
/// This extends the base class adding/overriding the below functionality:
///     - limit orientation adjustments to the Y axis only.
///     - toggle snap endpoints to terrain
///     - add drop down to select directory to randomly grab center objects
/// </summary>

namespace DigitalHorde.ConcentricRings.StoneCircles {
    public class StoneCircle_Manager : CRG_Manager {

        #region Events
        public static Action SnapToTerrain;
        public static Action<float> SetGlobalHeightOffset;
        public static Action UnSnapToTerrain;
        public static Action HideObjectsOnCollision;
        public static Action UnHideObjectsOnCollision;
        public static Action HideSelfOnObjectsCollision;
        public static Action UnHideSelfOnObjectsCollision;
        #endregion

        #region Member Variables & Properties
        [SerializeField] private int m_CurrentCenterPieceIndex = 0;
        public int CurrentCenterPieceIndex { get { return m_CurrentCenterPieceIndex; } set { m_CurrentCenterPieceIndex = value; } }
        [SerializeField] List<string> m_CenterPiecesFolders = new List<string>();
        public List<string> CenterPiecesFolders { get { return m_CenterPiecesFolders; } }
        [SerializeField] List<string> m_CenterPiecesFoldersPaths = new List<string>();
        public List<string> CenterPiecesFoldersPaths { get { return m_CenterPiecesFoldersPaths; } }
        [SerializeField] private string m_CurrentCenterPieceFolderPath;
        public string CurrentCenterPieceFolderPath { get { return m_CurrentCenterPieceFolderPath; } set { m_CurrentCenterPieceFolderPath = value; } }
        [SerializeField] private bool m_RandomPrefabLoaded = false;
        public bool RandomPrefabLoaded { get { return m_RandomPrefabLoaded; } set { m_RandomPrefabLoaded = value; } }
        [SerializeField] private float m_CenterPieceHeightOffset = 0f;
        public float CenterPieceHeightOffset { get { return m_CenterPieceHeightOffset; } set { m_CenterPieceHeightOffset = value; } }
        [SerializeField] private bool m_IgnoreTerrainOffset = false;
        public bool IgnoreTerrainOffset { get { return m_IgnoreTerrainOffset; } set { m_IgnoreTerrainOffset = value; } }
        [SerializeField] float m_HeightOffset = -0.25f;
        public float HeightOffset { get { return m_HeightOffset; } set { m_HeightOffset = value; } }
        [SerializeField] bool m_SnapEndPointsToTerrain = false;
        public bool SnapEndPointsToTerrain { get { return m_SnapEndPointsToTerrain; } set { m_SnapEndPointsToTerrain = value; } }
        [SerializeField] private List<StoneCircle_Ring> m_StoneCircleRingsList = new List<StoneCircle_Ring>();
        public List<StoneCircle_Ring> StoneCircleRingsList { get { return m_StoneCircleRingsList; } set { m_StoneCircleRingsList = value; } }
        [SerializeField] private bool m_HideCollidingObjects = false;
        public bool HideCollidingObjects { get { return m_HideCollidingObjects; } set { m_HideCollidingObjects = value; } }
        [SerializeField] private bool m_HideSelfOnCollidingObjects = false;
        public bool HideSelfOnCollidingObjects { get { return m_HideSelfOnCollidingObjects; } set { m_HideSelfOnCollidingObjects = value; } }
        public bool InstantiatedByAPI { get; set; } = false;
        #endregion

        #region Builtin Methods
        protected override void OnEnable() {

            Title = SCG_Configuration.SCG_TITLE;

#if (UNITY_EDITOR)
            if (GetComponent<StoneCircle_UserData>() == null) { gameObject.AddComponent<StoneCircle_UserData>(); }
#endif

        }
        protected void Start() {

            if (InstantiatedByAPI) { this.hideFlags = HideFlags.HideInInspector; }

        }
        #endregion

        #region Custom Methods
        public override void AddRing() {

            StoneCircle_Ring ring = gameObject.AddComponent<StoneCircle_Ring>();
            GameObject ringRoot = new GameObject();

            // get number of components, then for loop component up one less than the count:
            StoneCircle_Ring[] rings = GetComponents<StoneCircle_Ring>();

#if (UNITY_EDITOR)
            if (rings.Length > 0) { for (int i = 0; i < rings.Length - 1; i++) { UnityEditorInternal.ComponentUtility.MoveComponentUp(ring); } }
#endif

            int ringID;
            if (StoneCircleRingsList.Count > 0) { ringID = StoneCircleRingsList.Count; } else { ringID = 1; }
            ringID++;
            ring.RingID = StoneCircleRingsList.Count + 1;

            ring.RingRoot = ringRoot;
            ring.RingRoot.AddComponent<Rigidbody>();
            ring.RingRoot.GetComponent<Rigidbody>().isKinematic = true;
            ring.RingRoot.GetComponent<Rigidbody>().useGravity = false;
            ring.RingRoot.name = "Ring " + ring.RingID;
            ring.RingRoot.transform.SetPositionAndRotation(transform.position, transform.rotation);
            ring.RingRoot.transform.parent = transform;
            ring.StoneCircleManager = GetComponent<StoneCircle_Manager>();

            ring.SetRingLabel(ring.RingID);

            StoneCircleRingsList.Add(ring);

        }
        public virtual void RemoveRing(StoneCircle_Ring ring) {

            ring.RemoveRubble();

            BroadcastUnHideSelfOnObjectsCollision();
            BroadcastUnHideObjectsOnCollision();

            StoneCircleRingsList.Remove(ring);
            DestroyImmediate(ring.RingRoot);
            DestroyImmediate(ring);

            if (HideCollidingObjects) { BroadcastHideObjectsOnCollision(); }
            if (HideSelfOnCollidingObjects) { BroadcastHideSelfOnObjectsCollision(); }

        }
        public virtual void UpdateCenterPieceOffsetHeight() {

            Center.GetComponentInChildren<StoneCircle_EndPoint>().HeightOffsetOverride = HeightOffset + CenterPieceHeightOffset;

        }
        public virtual void UpdateCenterPieceIgnoreTerrainOffset() {

            Center.GetComponentInChildren<StoneCircle_EndPoint>().IgnoreTerrainOffset = IgnoreTerrainOffset;

        }
        public virtual void SnapRingRootToTerrain() {

            Vector3 pos = transform.position;
            pos.y = Terrain.activeTerrain.SampleHeight(transform.position) + HeightOffset;
            transform.position = pos;

#if (UNITY_EDITOR)
            SceneView.RepaintAll();
#endif

        }
        public virtual void SetHeightOffset() {

            if (Center != null) { UpdateCenterPieceOffsetHeight(); }

            foreach (StoneCircle_Ring ring in StoneCircleRingsList) {

                ring.UpdateRingOffsetHeight();

            }

        }
        public virtual void BroadcastSnapToTerrain() { 
            
            for (int i = 0; i < StoneCircleRingsList.Count; i++) {

                for (int j = 0; j < StoneCircleRingsList[i].EndPointsObjects.Count; j++) {

                    StoneCircleRingsList[i].EndPointsObjects[j].GetComponent<StoneCircle_EndPoint>().SnapToTerrain();

                }

            }
        
        }
        public virtual void BroadcastUnSnapToTerrain() { 
            
            foreach (StoneCircle_Ring ring in StoneCircleRingsList) {

                ring.IgnoreTerrainOffset = false;

            }

            for (int i = 0; i < StoneCircleRingsList.Count; i++) {

                for (int j = 0; j < StoneCircleRingsList[i].EndPointsObjects.Count; j++) {

                    StoneCircleRingsList[i].EndPointsObjects[j].GetComponent<StoneCircle_EndPoint>().UnSnapToTerrain(transform.position.y);

                }

            }

        }
        public virtual void BroadcastHideObjectsOnCollision() { 

            for (int i = 0; i < StoneCircleRingsList.Count; i++) {

                for (int j = 0; j < StoneCircleRingsList[i].EndPointsObjects.Count; j++) {

                    StoneCircleRingsList[i].EndPointsObjects[j].GetComponent<StoneCircle_EndPoint>().HideObjectsOnCollision();

                }

            }

        }
        public virtual void BroadcastUnHideObjectsOnCollision() { 

            for (int i = 0; i < StoneCircleRingsList.Count; i++) {

                for (int j = 0; j < StoneCircleRingsList[i].EndPointsObjects.Count; j++) {

                    StoneCircleRingsList[i].EndPointsObjects[j].GetComponent<StoneCircle_EndPoint>().UnHideObjectsOnCollision();

                }

            }

        }
        public virtual void BroadcastHideSelfOnObjectsCollision() { 

            for (int i = 0; i < StoneCircleRingsList.Count; i++) {

                for (int j = 0; j < StoneCircleRingsList[i].EndPointsObjects.Count; j++) {

                    StoneCircleRingsList[i].EndPointsObjects[j].GetComponent<StoneCircle_EndPoint>().HideSelfOnObjectsCollision();

                }

            }

        }
        public virtual void BroadcastUnHideSelfOnObjectsCollision() {
            
            for (int i = 0; i < StoneCircleRingsList.Count; i++) {

                for (int j = 0; j < StoneCircleRingsList[i].EndPointsObjects.Count; j++) {

                    StoneCircleRingsList[i].EndPointsObjects[j].GetComponent<StoneCircle_EndPoint>().UnHideSelfOnObjectsCollision();

                }

            }

        }
        public virtual void LoadCenterPiece(GameObject centerPiece) {

            GameObject center = null;
            center = Instantiate(centerPiece, new Vector3(0, 0, 0), Quaternion.identity);

            CenterPrefab = center;
            AddCenterObject();
            RandomPrefabLoaded = true;

            DestroyImmediate(center);

        }
        public virtual void LoadRandomCenterPiece() {

            if (CenterPrefab == null && !RandomPrefabLoaded) {
                
                GameObject center = null;
                List<string> centerFiles = new List<string>();

                CurrentCenterPieceFolderPath = CenterPiecesFoldersPaths[CurrentCenterPieceIndex];
                string[] files = System.IO.Directory.GetFiles(CurrentCenterPieceFolderPath);

                if (files.Length > 0) {

                    foreach (string file in files) {

                        // Only load files with prefab extension and add it to list of stones:
                        if (string.Equals(file.Split('.').Last(), "prefab", StringComparison.OrdinalIgnoreCase)) { centerFiles.Add(file); }

                    }

                    // Load random prefab from list:
                    GameObject asset = null;

#if (UNITY_EDITOR)
                    asset = AssetDatabase.LoadAssetAtPath<GameObject>(centerFiles[UnityEngine.Random.Range(0, centerFiles.Count)]);
#endif

                    center = Instantiate(asset, new Vector3(0, 0, 0), Quaternion.identity);

                }

                CenterPrefab = center;
                AddCenterObject();
                RandomPrefabLoaded = true;

                DestroyImmediate(center);

            }

        }
        public override void AddCenterObject() {

            base.AddCenterObject();

            Center.AddComponent<StoneCircle_EndPoint>();
            if (SnapEndPointsToTerrain) {

                Center.GetComponent<StoneCircle_EndPoint>().SnapToTerrain();

            }

        }
        public override void RemoveCenterObject() {

            base.RemoveCenterObject();
            RandomPrefabLoaded = false;

        }
        #endregion

    }

}