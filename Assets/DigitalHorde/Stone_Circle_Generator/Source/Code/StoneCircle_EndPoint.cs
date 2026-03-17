using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DigitalHorde.ConcentricRings.StoneCircles {

    [ExecuteAlways]
    public class StoneCircle_EndPoint : MonoBehaviour {

        [SerializeField] private bool m_EnableSnapToTerrain = false;
        public bool EnableSnapToTerrain { get { return m_EnableSnapToTerrain; } set { m_EnableSnapToTerrain = value; } }
        [SerializeField] private bool m_IgnoreTerrainOffset = false;
        public bool IgnoreTerrainOffset { get { return m_IgnoreTerrainOffset; } set { m_IgnoreTerrainOffset= value; } }
        [SerializeField] private float m_HeightOffset = 0f;
        public float HeightOffset { get { return m_HeightOffset; } set { m_HeightOffset = value; } }
        [SerializeField] private float m_HeightOffsetOverride = 0f;
        public float HeightOffsetOverride { get { return m_HeightOffsetOverride; } set { m_HeightOffsetOverride = value; } }
        [SerializeField] private bool m_HideCollidingTerrainObjects = false;
        public bool HideCollidingTerrainObjects { get { return m_HideCollidingTerrainObjects; } set { m_HideCollidingTerrainObjects = value; } }
        [SerializeField] private bool m_HideSelfOnTerrainObjectsCollisions = false;
        public bool HideSelfOnTerrainObjectsCollisions { get { return m_HideSelfOnTerrainObjectsCollisions; } set { m_HideSelfOnTerrainObjectsCollisions = value; } }
        private Collider m_EndPointCollider;
        private Collider[] m_Colliders;
        [SerializeField] public List<GameObject> m_HiddenObjects = new List<GameObject>();

        protected virtual void OnEnable() {

            CheckForColliders();

        }
        protected virtual void Update() {

            HeightOffsetCheck();
            CollisionCheck();

#if (UNITY_EDITOR)
            SceneView.RepaintAll();
#endif

        }
        protected virtual void CheckForColliders() {

            // check if there are already colliders on the prefab:
            Collider collider = GetComponentInChildren<Collider>();
            if (collider != null) {

                m_EndPointCollider = collider;  

            } else {

                if (gameObject.GetComponentInChildren<MeshFilter>()) {

                    m_EndPointCollider = gameObject.AddComponent<MeshCollider>();
                    MeshFilter endPointMesh = gameObject.GetComponentInChildren<MeshFilter>();
                    m_EndPointCollider.GetComponent<MeshCollider>().sharedMesh = endPointMesh.sharedMesh;
                    m_EndPointCollider.GetComponent<MeshCollider>().convex = true;

                } else {

                    m_EndPointCollider = gameObject.AddComponent<BoxCollider>();
                    gameObject.GetComponent<BoxCollider>().size = new Vector3(0.1f, 0.1f, 0.1f);

                }

            }

        }
        protected virtual void UnHideColliders() {

            if (m_HiddenObjects.Count > 0) {

                for (int i = 0; i < m_HiddenObjects.Count; i++) {

                    if (m_HiddenObjects[i] != null) { 
                        
                        Renderer[] renderers = m_HiddenObjects[i].gameObject.GetComponentsInChildren<Renderer>();
                        Collider[] colliders = m_HiddenObjects[i].gameObject.GetComponentsInChildren<Collider>();

                        for (int ii = 0; ii < renderers.Length; ii++) { renderers[ii].enabled = true; }
                        for (int ii = 0; ii < colliders.Length; ii++) { colliders[ii].enabled = true; }

                    }

                }

            }

            if (!gameObject.transform.GetChild(0).gameObject.activeSelf) { gameObject.transform.GetChild(0).gameObject.SetActive(true); }
            m_EndPointCollider.isTrigger = false;

            m_HiddenObjects.Clear();

        }
        protected virtual void CollisionCheck() {

            if (!HideCollidingTerrainObjects) { return; }

            UnHideColliders();

            m_Colliders = Physics.OverlapSphere(transform.position, 3f);

            for (int i = 0; i < m_Colliders.Length; i++) {

                if (!m_Colliders[i].GetComponent<Terrain>() && 
                    !m_Colliders[i].GetComponent<StoneCircle_EndPoint>() &&
                    !m_Colliders[i].GetComponentInParent<StoneCircle_EndPoint>() &&
                    !m_Colliders[i].transform.IsChildOf(transform)) {

                    if (m_EndPointCollider.bounds.Intersects(m_Colliders[i].bounds)) {

                        if (HideSelfOnTerrainObjectsCollisions) {

                            if (gameObject.transform.GetChild(0).gameObject.activeSelf) { gameObject.transform.GetChild(0).gameObject.SetActive(false); }
                            m_EndPointCollider.isTrigger = true;

                        } else {

                            if (m_Colliders[i].gameObject.activeSelf) {

                                Renderer[] renderers = m_Colliders[i].gameObject.GetComponentsInChildren<Renderer>();
                                Collider[] colliders = m_Colliders[i].gameObject.GetComponentsInChildren<Collider>();

                                for (int ii = 0; ii < renderers.Length; ii++) { renderers[ii].enabled = false; }
                                for (int ii = 0; ii < colliders.Length; ii++) { colliders[ii].enabled = false; }

                                m_HiddenObjects.Add(m_Colliders[i].gameObject);

                            }

                        }

                    } 

                }

            }

        }
        public virtual void HideObjectsOnCollision() { HideCollidingTerrainObjects = true; }
        public virtual void UnHideObjectsOnCollision() { 
            
            HideCollidingTerrainObjects = false;
            UnHideColliders();

        }
        public virtual void HideSelfOnObjectsCollision() { HideSelfOnTerrainObjectsCollisions = true; }
        public virtual void UnHideSelfOnObjectsCollision() { 
            
            HideSelfOnTerrainObjectsCollisions = false;
            UnHideColliders();

        }
        public virtual void UnSnapToTerrain(float y) { 
            
            EnableSnapToTerrain = false;
            IgnoreTerrainOffset = false;
            transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
        
        }
        public virtual void SnapToTerrain() { // this toggles on snap to terrain

            EnableSnapToTerrain = true;
            HeightOffsetCheck();

        }
        protected void HeightOffsetCheck() {

            float terrainContactHeight = GetTerrainContactHeight();
            Vector3 pos = transform.position;

            if (EnableSnapToTerrain) {

                if (!IgnoreTerrainOffset) {

                    pos.y = terrainContactHeight + HeightOffsetOverride;

                } else {

                    pos.y = transform.parent.position.y + HeightOffsetOverride;

                }

            } else {

                pos.y = transform.parent.position.y + HeightOffsetOverride;

            }

            transform.position = pos;
            
        }
        protected virtual float GetTerrainContactHeight() {

            // USING LAYER MASK:
            /*
            float y = 0f;
            Vector3 samplePos = new Vector3(transform.position.x, transform.position.y + 5000f, transform.position.z);

            RaycastHit hit;
            if (Physics.Raycast(samplePos, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Terrain"))) {

                //Debug.Log("Terrain: " + hit.transform.gameObject.name);
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.point.y, Color.green);
                y = hit.point.y;

            }

            return y;
            */
            ///////////////////

            float y = 0f;
            Vector3 samplePos = new Vector3(transform.position.x, transform.position.y + 5000f, transform.position.z);

            RaycastHit[] hits;
            hits = Physics.RaycastAll(samplePos, Vector3.down, Mathf.Infinity);

            for (int i = 0; i < hits.Length; i++) {

                if (hits[i].transform.gameObject.GetComponent<Terrain>() != null) {

                    y = hits[i].point.y;
                    return y;

                }

            }

            return y;

        }

    }

}