using System.Collections.Generic;
using UnityEngine;
using DigitalHorde.ConcentricRings.StoneCircles;

/// <summary>
/// 
/// Bare bones example using the API to generate stone circles at runtime. 
/// 
/// First instantiate a new Stone Circle Manager: 
///     StoneCircle_Manager myStoneCircle = StoneCircleAPI.GenerateStoneCircle(new Vector3(0,0,0), "My Circle Generator");
///     
/// Next add rings, setting the number of endpoints, radius, and reference a list to pull prefabs from:
///     StoneCircle_Ring myRing1 = StoneCircleAPI.AddRing(myStoneCircle, 3, 5f, myEndPointList);
///     
/// Alternately use a single GameObject as the endpoint:
///     StoneCircle_Ring myRing2 = StoneCircleAPI.AddRing(myStoneCircle, 3, 5f, myGameObject); 
///     
/// </summary>
/// 

namespace DigitalHorde.Examples {

    public class Runtime_API_Example : MonoBehaviour
    {

        [Header("Stone Circle Locations:")]
        [SerializeField] private Transform m_CircleLocationA;
        [SerializeField] private Transform m_CircleLocationB;
        [SerializeField] private Transform m_CircleLocationC;

        [Header("End Point Prefabs:")]
        [SerializeField] private GameObject m_MyPrefab;
        [SerializeField] private List<GameObject> m_RandomEndpointList = new List<GameObject>();
        [SerializeField] private List<GameObject> m_RandomRubbleList = new List<GameObject>();

        void Start()
        {

            // This example creates 3 stone circles:
            StoneCircle_Manager myStoneCircleManager1 = StoneCircleGenerator_API.GenerateStoneCircle(m_CircleLocationA.position, "My Stone Circle 1", this);
            StoneCircle_Manager myStoneCircleManager2 = StoneCircleGenerator_API.GenerateStoneCircle(m_CircleLocationB.position, "My Stone Circle 2", this);
            StoneCircle_Manager myStoneCircleManager3 = StoneCircleGenerator_API.GenerateStoneCircle(m_CircleLocationC.position, "My Stone Circle 3", this);

            // Add rings with basic settings to each generator:
            StoneCircle_Ring ring1a = StoneCircleGenerator_API.AddRing(myStoneCircleManager1, 10, 5f, m_RandomEndpointList); // add ring with number of endpoints and radius specified.
            StoneCircle_Ring ring2a = StoneCircleGenerator_API.AddRing(myStoneCircleManager1, 3, 10f, m_MyPrefab);  // add ring with number of endpoints and radius specified.

            StoneCircle_Ring ring1b = StoneCircleGenerator_API.AddRing(myStoneCircleManager2, m_RandomEndpointList);  // add ring without specifying number of endpoints or radius (default of 10 endpoints and 10f radius will be used).
            StoneCircle_Ring ring2b = StoneCircleGenerator_API.AddRing(myStoneCircleManager2, m_RandomEndpointList); // add ring without specifying number of endpoints or radius (default of 10 endpoints and 10f radius will be used).

            StoneCircle_Ring ring1c = StoneCircleGenerator_API.AddRing(myStoneCircleManager3, m_RandomEndpointList);  // add ring without specifying number of endpoints or radius (default of 10 endpoints and 10f radius will be used).
            StoneCircle_Ring ring2c = StoneCircleGenerator_API.AddRing(myStoneCircleManager3, m_RandomEndpointList); // add ring without specifying number of endpoints or radius (default of 10 endpoints and 10f radius will be used).

            // Snap the circle's endpoints to the terrain:
            StoneCircleGenerator_API.SnapCircleToTerrain(myStoneCircleManager1, true);
            StoneCircleGenerator_API.SnapCircleToTerrain(myStoneCircleManager2, true);
            StoneCircleGenerator_API.SnapCircleToTerrain(myStoneCircleManager3, true);

            // EXAMPLE API CALLS ARE BELOW:
            ///////////////////////////////
            /*
            // Set the orientation (rotation) of the circle:
            StoneCircleGenerator_API.SetCircleOrientation(myStoneCircleManager1, transform.rotation); // second parameter is a Quaternion, same as rotating the root transform of the circle

            // Globally adjust the circle height offset:
            StoneCircleGenerator_API.SetCircleHeightOffset(myStoneCircleManager1, 0f);

            // Add a center piece to the circle:
            StoneCircleGenerator_API.AddCenterPiece(myStoneCircleManager1, m_RandomEndpointList);
            StoneCircleGenerator_API.AddCenterPiece(myStoneCircleManager1, m_MyPrefab); // NOTE: Replaces above center piece.

            // Adjust the height offset of the center peice:
            StoneCircleGenerator_API.SetCenterPieceHeightOffset(myStoneCircleManager1, 20f);

            // Update ring endpoints by referencing each ring:
            StoneCircleGenerator_API.NumberOfEndPoints(ring1a, 28); // set ring1a's number of endpoints to 28.
            StoneCircleGenerator_API.Radius(ring1a, 30.5f); // set the radius of ring1a to 30.5f.

            StoneCircleGenerator_API.NumberOfEndPoints(ring2a, 10);
            StoneCircleGenerator_API.Radius(ring2a, 15f);

            // Set the orientation of each ring individually:
            StoneCircleGenerator_API.SetRingOrientation(ring1a, new Vector3(0f, 78f, 0f));

            // Set the height offset of each ring individually:
            StoneCircleGenerator_API.SetRingOffsetHeight(ring1a, 0f);

            // Orient the endpoints of the ring towards the center:
            StoneCircleGenerator_API.OrientEndpointTowardCenter(ring1a, true);

            // Set the entrance gap size:
            StoneCircleGenerator_API.SetRingGapSize(ring1a, 20);

            // Set random endpoint rotations:
            StoneCircleGenerator_API.SetRandomizeStoneRotations(ring1a, new Vector2(-10f, 10f));

            // Set random tilt & lean:
            StoneCircleGenerator_API.SetRandomizeTiltAndLean(ring1a, new Vector2(-5f, 5f));

            // Set random stone placement:
            StoneCircleGenerator_API.SetRandomizeStonePlacement(ring1a, new Vector2(-10f, 10f));

            // Set random radius distance:
            StoneCircleGenerator_API.SetRandomizeRadiusDistance(ring1a, new Vector2(-2f, 2f));

            // Set random stone scale:
            StoneCircleGenerator_API.SetRandomizeStoneScale(ring1a, new Vector2(0.75f, 1.25f), true);

            // Set random stone gaps:
            StoneCircleGenerator_API.SetRandomizeStoneGaps(ring1a, new Vector2(50f, 80f));

            // Set replace gaps with random rubble from a list:
            StoneCircleGenerator_API.SetReplaceGapsWithRubble(ring1a, m_RandomRubbleList);

            // Set replace gaps with a prefab:
            StoneCircleGenerator_API.SetReplaceGapsWithRubble(ring1a, m_MyPrefab);

            // Remove the ring:
            StoneCircleGenerator_API.RemoveRing(ring1a);
            */

        }

    }

}
