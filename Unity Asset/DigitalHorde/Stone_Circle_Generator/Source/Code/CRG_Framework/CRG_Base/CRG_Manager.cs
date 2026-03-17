using System.Collections.Generic;
using System;
using UnityEngine;

namespace DigitalHorde.ConcentricRings {
    
    [ExecuteInEditMode]
    [System.Serializable]
    public class CRG_Manager : MonoBehaviour {

        #region Member Variables & Properties
        private string m_Title;
        public string Title { get { return m_Title; } set { m_Title = value; } }
        [SerializeField] private TextAsset m_JSONPreset;
        public TextAsset JSONPreset { get { return m_JSONPreset; } set { m_JSONPreset = value; } }
        private bool m_ToggleUseCenterObject = false;
        public bool ToggleUseCenterObject { get { return m_ToggleUseCenterObject; } set { m_ToggleUseCenterObject = value; } }
        [SerializeField] private List<CRG_Ring> m_SortedRingsList = new List<CRG_Ring>();
        public List<CRG_Ring> RingsList { get { return m_SortedRingsList; } set { m_SortedRingsList = value; } }
        [SerializeField] private GameObject m_CenterPrefab;
        public GameObject CenterPrefab { get { return m_CenterPrefab; } set { m_CenterPrefab = value; } }
        [SerializeField] private GameObject m_Center;
        public GameObject Center { get { return m_Center; } set { m_Center = value; } }
        private bool m_UsingCenterPieceOverride = false;
        public bool UsingCenterPieceOverride { get { return m_UsingCenterPieceOverride; } set { m_UsingCenterPieceOverride = value; } }
        [SerializeField] private bool m_SavePositionRotation = true;
        public bool SavePositionRotation { get { return m_SavePositionRotation; } set { m_SavePositionRotation = value; } }
        [SerializeField, Range(-360f, 360f)] private float m_XOrientation;
        public float XOrientation { get { return m_XOrientation; } set { m_XOrientation = value; } }
        [SerializeField, Range(-360f, 360f)] private float m_YOrientation;
        public float YOrientation { get { return m_YOrientation; } set { m_YOrientation = value; } }
        [SerializeField, Range(-360f, 360f)] private float m_ZOrientation;
        public float ZOrientation { get { return m_ZOrientation; } set { m_ZOrientation = value; } }
        #endregion

        #region Builtin Methods
        protected virtual void OnEnable() {

            Title = "CONCENTRIC RING GENERATOR";

#if (UNITY_EDITOR)
            if (GetComponent<CRG_UserData>() == null) { gameObject.AddComponent<CRG_UserData>(); }
#endif

        }
        protected virtual void Update() { }
        #endregion

        #region Custom Methods
        public virtual void AddRing() {

            CRG_Ring ring = gameObject.AddComponent<CRG_Ring>();
            GameObject ringRoot = new GameObject();

            // get number of components, then for loop component up one less than the count:
            CRG_Ring[] rings = GetComponents<CRG_Ring>();

#if (UNITY_EDITOR)
            if (rings.Length > 0) { for (int i = 0; i < rings.Length - 1; i++) { UnityEditorInternal.ComponentUtility.MoveComponentUp(ring); } }
#endif

            int ringID;
            if (RingsList.Count > 0) { ringID = RingsList.Count; } else { ringID = 1; }
            ringID++;
            ring.RingID = RingsList.Count + 1;

            ring.RingRoot = ringRoot;
            ring.RingRoot.name = "Ring " + ring.RingID;
            ring.RingRoot.transform.SetPositionAndRotation(transform.position, transform.rotation);
            ring.RingRoot.transform.parent = transform;
            ring.RingManager = GetComponent<CRG_Manager>();

            ring.SetRingLabel(ring.RingID);

            RingsList.Add(ring);

        }
        public virtual void RemoveRing(CRG_Ring ring) {

            RingsList.Remove(ring);
            DestroyImmediate(ring.RingRoot);
            DestroyImmediate(ring);

        }
        public virtual void AddCenterObject() {

            if (CenterPrefab != null) {

                if (Center != null) { DestroyImmediate(Center); } // destroy a Center if exists (CenterPrefab was replaced)
                
                Center = Instantiate(CenterPrefab, transform.position, transform.rotation, transform);
                ToggleUseCenterObject = true; // toggle on the center object in the editor
                Center.transform.SetAsFirstSibling();
                Center.name = "Ring Center";

            }

        }
        public virtual void RemoveCenterObject() {

            if (Center != null) {

                CenterPrefab = null;
                DestroyImmediate(Center);
                Center = null;
                UsingCenterPieceOverride = false;

            }

        }
        public virtual void SetOrientation(Vector3 orientation) { transform.localRotation = Quaternion.Euler(orientation); }

#if (UNITY_EDITOR)
        public virtual void SaveAsPreset() {

            CRG_UserData userDataManager = GetComponent<CRG_UserData>();
            userDataManager.RingManager = this;
            userDataManager.SaveAsPreset();

        }
        public virtual void LoadPreset(TextAsset JSON) {

            CRG_UserData userDataManager = GetComponent<CRG_UserData>();
            userDataManager.RingManager = this;
            userDataManager.LoadPreset(JSON);

        }
        public virtual void SaveAsPrefab() {

            CRG_UserData userDataManager = GetComponent<CRG_UserData>();
            userDataManager.RingManager = this;
            userDataManager.SaveAsPrefab();

        }
#endif

#endregion

    }

}