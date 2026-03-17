using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This base class handles the basic functionality of generating a ring of endpoints:
///     - instantiating each endpoint a set radius from the center evenly (360 degrees / number of endpoints).
///     - orienting the endpoints toward the center.
///     - handling overrides for number of endpoints and radius
///     - maintaining an endpoint positions dictionary and endpoint objects dictionary
/// </summary>
/// 

namespace DigitalHorde.ConcentricRings {

    public class EndPoint {

        public Vector3 position;
        public Quaternion rotation;
        public Vector3 localPosition;
        public Quaternion localRotation;

    }

    [ExecuteInEditMode]
    [System.Serializable]
    public class CRG_Ring : MonoBehaviour {

        #region Member Variables & Properties
        [SerializeField] private int ringID;
        public int RingID { get { return ringID; } set { ringID = value; } }
        [SerializeField] private CRG_Manager m_RingManager;
        public CRG_Manager RingManager { get { return m_RingManager; } set { m_RingManager = value; } }
        [SerializeField] private GameObject m_RingRoot;
        public GameObject RingRoot { get { return m_RingRoot; } set { m_RingRoot = value; } }
        [SerializeField] private string m_RingLabel = "";
        public string RingLabel { get { return m_RingLabel; } set { m_RingLabel = value; } }
        [Range(0, 32)]
        [SerializeField] private int m_NumberOfEndPoints = 10;
        public int NumberOfEndPoints { get { return m_NumberOfEndPoints; } set { m_NumberOfEndPoints = value; } }
        [Range(3, 50)]
        [SerializeField] private float m_Radius = 10f;
        public float Radius { get { return m_Radius; } set { m_Radius = value; } }
        [SerializeField] private bool m_OverrideSettings = false;
        public bool OverrideSettings { get { return m_OverrideSettings; } set { m_OverrideSettings = value; } }
        //[Range(1, 1000)]
        [SerializeField] private int m_NumberOfEndPointsOverride = 10;
        public int NumberOfEndPointsOverride { get { return m_NumberOfEndPointsOverride; } set { m_NumberOfEndPointsOverride = value; } }
        //[Range(0, 5000)]
        [SerializeField] private float m_RadiusOverride = 10f;
        public float RadiusOverride { get { return m_RadiusOverride; } set { m_RadiusOverride = value; } }
        [SerializeField, Range(-360f, 360f)] private float m_XOrientation;
        public float XOrientation { get { return m_XOrientation; } set { m_XOrientation = value; } }
        [SerializeField, Range(-360f, 360f)] private float m_YOrientation;
        public float YOrientation { get { return m_YOrientation; } set { m_YOrientation = value; } }
        [SerializeField, Range(-360f, 360f)] private float m_ZOrientation;
        public float ZOrientation { get { return m_ZOrientation; } set { m_ZOrientation = value; } }
        [SerializeField] private bool m_OrientEndPointsTowardCenter = true;
        public bool OrientEndPointsTowardCenter { get { return m_OrientEndPointsTowardCenter; } set { m_OrientEndPointsTowardCenter = value; } }
        [SerializeField] private List<EndPoint> m_EndPointsPositions = new List<EndPoint>();
        public List<EndPoint> EndPointsPositions { get { return m_EndPointsPositions; } set { m_EndPointsPositions = value; } }
        [SerializeField] private GameObject m_EndPoint;
        public GameObject EndPoint { get { return m_EndPoint; } set { m_EndPoint = value; } }
        [SerializeField] private List<GameObject> m_EndPointObjects = new List<GameObject>();
        public List<GameObject> EndPointsObjects { get { return m_EndPointObjects; } set { m_EndPointObjects = value; } }
        #endregion

        public virtual void SetRingLabel(int ringCount) { // override this to customize ring naming in implementations

            RingLabel = "RING " + ringCount;

        }
        protected virtual int GetNumberOfEndPoints() {

            if (NumberOfEndPoints < 1) { NumberOfEndPoints = 1; }
            if (NumberOfEndPointsOverride < 1) { NumberOfEndPointsOverride = 1; }
            if (NumberOfEndPointsOverride > 1000) { NumberOfEndPointsOverride = 1000; }

            int num = NumberOfEndPoints;
            if (OverrideSettings) { num = NumberOfEndPointsOverride; }
            
            return num;

        }
        public virtual float GetRadius() {

            if (Radius < 3) { Radius = 3; }
            if (RadiusOverride < 1) { RadiusOverride = 1; } 

            float radius = Radius;
            if (OverrideSettings) { radius = RadiusOverride; }

            return radius;

        }
        public virtual void ReGenerateRing() {

            int numEndPoints = GetNumberOfEndPoints();
            float radius = GetRadius();

            if (numEndPoints < 1) { return; }
 
            EndPointsPositions.Clear();
            EndPointsPositions = GenerateEndPointPositions(numEndPoints, radius); // these functions could be refactored to act directly on the dictionary but I added the return and parameter to make what's happening more clear.
            ReGenerateEndPoints();

        }
        protected virtual List<EndPoint> GenerateEndPointPositions(int numEndPoints, float radius) {

            List<EndPoint> tempEndPoints = new List<EndPoint>();

            // Ideally this object would only be instantiated once at runtime, however to keep the hierarchy clean instantiate it each time a ring is rebuilt. Cannot use the RingRoot as rotator due to the objects being parented to the RingRoot.
            GameObject rotator = new GameObject();
            rotator.transform.SetPositionAndRotation(transform.position, Quaternion.identity); // NOTE: set this to stop the endpoints from spinning on account of the rotator not being reset, if using localRotation then radius adjustments won't work if ring is rotated. 
            rotator.transform.parent = transform;

            // Get rotation degrees:
            float degrees = 0f;
            if (numEndPoints > 0) { degrees = 360f / numEndPoints; }

            // Build a ring of empty game objects, adding the positions and rotations to the EndPoints dictionary:
            for (int i = 0; i < numEndPoints; i++) {

                GameObject endPoint = new GameObject(); // use object pooling instead maybe? performance is not a huge issue as this function will not be continually called.

                // Build ring in this order so stones can be disabled in order to generate entrance gap.
                float calcRot;
                if (i % 2 == 0) { calcRot = degrees * i; calcRot = -calcRot; } else { calcRot = degrees * i; }
                rotator.transform.Rotate(0.0f, calcRot, 0.0f, Space.Self);
                endPoint.transform.localPosition = rotator.transform.forward * radius;

                endPoint.transform.SetParent(RingRoot.transform, false); // parent the object
                //endPoint.transform.LookAt(RingRoot.transform.position); // orient the object to face the center

                // Take the position and rotation of the endPoint and store it in EndPoints dictionary, then destroy the temp endpoint.
                EndPoint endPointPos = new EndPoint {
                    position = endPoint.transform.position,
                    rotation = endPoint.transform.rotation,
                    localPosition = endPoint.transform.localPosition,
                    localRotation = endPoint.transform.localRotation
                };

                tempEndPoints.Add(endPointPos);

                DestroyImmediate(endPoint);

            }

            DestroyImmediate(rotator);

            return tempEndPoints;

        }
        protected virtual void ReGenerateEndPoints() { // Override this function in implementations to add objects to the positions generated.

            // Destroy endpoint objects before regenerating:
            for (int i = 0; i < EndPointsObjects.Count;) { if (EndPointsObjects[i].gameObject != null) { DestroyImmediate(EndPointsObjects[i]); } else { i++; } }
            EndPointsObjects.Clear();

            if (EndPoint != null) {

                for (int i = 0; i < EndPointsPositions.Count; i++) {

                    GameObject endPoint = Instantiate(EndPoint);

                    endPoint.transform.SetParent(RingRoot.transform, false); // parent the new object first then set the local position and rotation
                    endPoint.transform.localPosition = EndPointsPositions[i].localPosition;
                    endPoint.transform.localRotation = EndPointsPositions[i].localRotation;

                    EndPointsObjects.Add(endPoint);

                }

            }

            ResetEndPointOrientation();

        }
        public virtual void UpdateRadius() { // Basic method of adjusting the distance from center for each endpoint, override this if (for example) you want random distances.

            RingRoot.transform.rotation = Quaternion.identity;
            float radius = GetRadius();

            GameObject tempRingRoot = new GameObject();
            tempRingRoot.transform.position = RingRoot.transform.position;

            for (int i = 0; i < EndPointsObjects.Count; i++) {

                tempRingRoot.transform.position = new Vector3(tempRingRoot.transform.position.x, EndPointsObjects[i].transform.position.y, tempRingRoot.transform.position.z);
                tempRingRoot.transform.LookAt(EndPointsObjects[i].transform);

                EndPointsObjects[i].transform.localPosition = tempRingRoot.transform.forward * radius;

            }

            DestroyImmediate(tempRingRoot);

            SetOrientation(new Vector3(XOrientation, YOrientation, ZOrientation));

        }

        public virtual void RemoveRing() { // Override this in implementations.
            
            RingManager.RemoveRing(this);

        }
        public virtual void SetOrientation(Vector3 orientation) {

            RingRoot.transform.localRotation = Quaternion.Euler(orientation);
            ResetEndPointOrientation();

        }
        public virtual void ResetEndPointOrientation() {

            if (OrientEndPointsTowardCenter) {

                for (int i = 0; i < EndPointsObjects.Count; i++) {

                    EndPointsObjects[i].transform.LookAt(RingRoot.transform.position);

                }

            } else {

                for (int i = 0; i < EndPointsObjects.Count; i++) {

                    EndPointsObjects[i].transform.rotation = Quaternion.identity;

                }

            }

        }

    }

}