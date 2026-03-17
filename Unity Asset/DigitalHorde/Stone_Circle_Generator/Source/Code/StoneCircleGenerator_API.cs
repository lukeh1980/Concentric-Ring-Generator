using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace DigitalHorde.ConcentricRings.StoneCircles {

    public static class StoneCircleGenerator_API {

        /// <summary>
        /// Generates a new stone circle at a vector3 position with a name you provide. Use this as a starting point for creating stone circles during runtime.
        /// </summary>
        /// <param name="position">The position the stone circle will be generated at.</param>
        /// <param name="name">Name of the stone circle.</param>
        /// <returns></returns>
        public static StoneCircle_Manager GenerateStoneCircle(Vector3 position, string name) {

            GameObject stoneCircleManager = new GameObject(name);
            stoneCircleManager.transform.position = position;
            stoneCircleManager.AddComponent<StoneCircle_Manager>(); 

            stoneCircleManager.GetComponent<StoneCircle_Manager>().InstantiatedByAPI = true;

            return stoneCircleManager.GetComponent<StoneCircle_Manager>();

        }
        /// <summary>
        /// Generates a new stone circle at a vector3 position with a name you provide as well as the MonoBehavior that instantiated it in case you need a reference to it.
        /// </summary>
        /// <param name="position">The position the stone circle will be generated at.</param>
        /// <param name="name">Name of the stone circle.</param>
        /// <param name="instantiator">The Monobehavior that generated the circle (can be used as a reference later).</param>
        /// <returns></returns>
        public static StoneCircle_Manager GenerateStoneCircle(Vector3 position, string name, MonoBehaviour instantiator) {

            GameObject stoneCircleManager = new GameObject(name);
            stoneCircleManager.transform.position = position;
            stoneCircleManager.AddComponent<StoneCircle_Manager>();

            stoneCircleManager.GetComponent<StoneCircle_Manager>().InstantiatedByAPI = true;
            stoneCircleManager.AddComponent<StoneCircleGenerator_API_Reference>();
            stoneCircleManager.GetComponent<StoneCircleGenerator_API_Reference>().InstantiatorReference = instantiator;

            return stoneCircleManager.GetComponent<StoneCircle_Manager>();

        }
        #region Circle (StoneCircle_Manager) Methods
        /// <summary>
        /// Set the entire cirlce orientation, same as rotating the root transform. Setting a rotation that rotates on an axis other than the Y may lead to unpredictable results.
        /// </summary>
        /// <param name="manager">The stone circle that will be acted upon.</param>
        /// <param name="direction">Rotation to be applied to the entire circle (consider rotating the root transform also).</param>
        public static void SetCircleOrientation(StoneCircle_Manager manager, Quaternion direction) {

            manager.transform.rotation = direction;

        }
        /// <summary>
        /// Sets the stone circle end points to snap to the terrain.
        /// </summary>
        /// <param name="manager">The stone circle that will be acted upon.</param>
        /// <param name="snap">Snap enpoints to the terrain surface.</param>
        public static void SnapCircleToTerrain(StoneCircle_Manager manager, bool snap) {

            if (snap) {

                manager.SnapEndPointsToTerrain = true;
                manager.BroadcastSnapToTerrain();

            } else {

                manager.SnapEndPointsToTerrain = false;
                manager.BroadcastUnSnapToTerrain(); 
            
            }

        }
        /// <summary>
        /// Sets the height offset of the entire circle (rings inherit this offset).
        /// </summary>
        /// <param name="manager">The stone circle that will be acted upon.</param>
        /// <param name="height">Height offset to be applied (negative/positive values applied to the Y axis of the entire circle).</param>
        public static void SetCircleHeightOffset(StoneCircle_Manager manager, float height) {

            manager.HeightOffset = height;
            manager.SetHeightOffset();

        }
        /// <summary>
        /// Use this to add a random center piece from the gameobject list you provide, overloaded function allows you to specify a prefab to use as the center peice instead.
        /// </summary>
        /// <param name="manager">The stone circle that will be acted upon.</param>
        /// <param name="randomGameObjectList">List of GameObjects to randomly pull a center peice from.</param>
        public static void AddCenterPiece(StoneCircle_Manager manager, List<GameObject> randomGameObjectList) {

            if (randomGameObjectList.Count > 0) {

                int randomIndex = UnityEngine.Random.Range(0, randomGameObjectList.Count - 1);
                if (randomGameObjectList[randomIndex] != null) { manager.LoadCenterPiece(randomGameObjectList[randomIndex]); }

            }

        }
        public static void AddCenterPiece(StoneCircle_Manager manager, GameObject prefab) {

            if (prefab != null) { manager.LoadCenterPiece(prefab); }

        }
        /// <summary>
        /// Set a height offset for the center piece (e.g. adjust the depth in the terrain or have it float a distance above the terrain).
        /// </summary>
        /// <param name="manager">The stone circle that will be acted upon.</param>
        /// <param name="height">Height offset to be applied (negative/positive values applied to the Y axis of the center peice).</param>
        public static void SetCenterPieceHeightOffset(StoneCircle_Manager manager, float height) {

            manager.IgnoreTerrainOffset = false;
            manager.CenterPieceHeightOffset = height;
            manager.UpdateCenterPieceOffsetHeight();

        }
        /// <summary>
        /// Hides (best effort) any objects that an endpoint collides with (e.g. objects placed on the terrain not part of the circle).
        /// </summary>
        /// <param name="manager">The stone circle that will be acted upon.</param>
        public static void HideCollidingObjects(StoneCircle_Manager manager) { manager.BroadcastHideObjectsOnCollision(); }
        /// <summary>
        /// Hides the endpoint (if second parameter is set to true) if it collides with an object not part of the circle.
        /// </summary>
        /// <param name="manager">The stone circle that will be acted upon.</param>
        /// <param name="hideSelf">If set to true endpoints will be hidden rather than the colliding object.</param>
        public static void HideCollidingObjects(StoneCircle_Manager manager, bool hideSelf) {

            if (hideSelf) { manager.BroadcastHideSelfOnObjectsCollision(); } else { manager.BroadcastHideObjectsOnCollision(); }

        }
        /// <summary>
        /// Unhides objects hidden due to collisions, use this if the hideSelf bool was not used to hide objects.
        /// </summary>
        /// <param name="manager">The stone circle that will be acted upon.</param>
        public static void UnHideCollidingObjects(StoneCircle_Manager manager) { manager.BroadcastUnHideObjectsOnCollision(); }
        /// <summary>
        /// Unhides objects hidden due to collisions, use this if the hideSelf bool was used to hide objects. 
        /// </summary>
        /// <param name="manager">The stone circle that will be acted upon.</param>
        /// <param name="hideSelf">Set this to true if the endpoints were hidden rather than colliding objects.</param>
        public static void UnHideCollidingObjects(StoneCircle_Manager manager, bool hideSelf) {

            if (hideSelf) { manager.BroadcastUnHideSelfOnObjectsCollision(); } else { manager.BroadcastUnHideObjectsOnCollision(); }

        }
        /// <summary>
        /// Add a new ring to the circle allowing you to specify the number of endpoints, radius, and a list of random endpoints to pull from.
        /// </summary>
        /// <param name="manager">The stone circle that will be acted upon.</param>
        /// <param name="numEndPoints">Number of endpoints (stones) that will be generated.</param>
        /// <param name="ringRadius">Radius the ring of endpoints will be set at.</param>
        /// <param name="randomGameObjectList">List of gameObjects to pull random endpoints from.</param>
        /// <returns></returns>
        public static StoneCircle_Ring AddRing(StoneCircle_Manager manager, int numEndPoints, float ringRadius, List<GameObject> randomGameObjectList) {

            manager.AddRing();
            StoneCircle_Ring ring = manager.GetComponent<StoneCircle_Manager>().StoneCircleRingsList.Last();

            ring.InstantiatedByAPI = true;
            ring.RandomEndPointOverrideList = randomGameObjectList;
            ring.OverrideSettings = true;
            ring.RadiusOverride = ringRadius;
            ring.NumberOfEndPointsOverride = numEndPoints;
            ring.ReGenerateRing();

            return ring;

        }
        /// <summary>
        /// Add a new ring to the circle allowing you to specify the number of endpoints, radius, and a specific endpoint prefab to use.
        /// </summary>
        /// <param name="manager">The stone circle that will be acted upon.</param>
        /// <param name="numEndPoints">Number of endpoints (stones) that will be generated.</param>
        /// <param name="ringRadius">Radius the ring of endpoints will be set at.</param>
        /// <param name="endpoint">A single gameObject to be used for endpoints.</param>
        /// <returns></returns>
        public static StoneCircle_Ring AddRing(StoneCircle_Manager manager, int numEndPoints, float ringRadius, GameObject endpoint) {

            manager.AddRing();
            StoneCircle_Ring ring = manager.GetComponent<StoneCircle_Manager>().StoneCircleRingsList.Last();

            ring.InstantiatedByAPI = true;
            ring.RandomEndPointOverrideList.Clear();
            ring.RandomEndPointOverrideList.Add(endpoint);
            ring.OverrideSettings = true;
            ring.RadiusOverride = ringRadius;
            ring.NumberOfEndPointsOverride = numEndPoints;
            ring.ReGenerateRing();

            return ring;

        }
        /// <summary>
        /// Add a new ring by only specifying manager and random game object list.
        /// </summary>
        /// <param name="manager">The stone circle that will be acted upon.</param>
        /// <param name="randomGameObjectList">List of gameObjects to pull random endpoints from.</param>
        /// <returns></returns>
        public static StoneCircle_Ring AddRing(StoneCircle_Manager manager, List<GameObject> randomGameObjectList) {

            manager.AddRing();
            StoneCircle_Ring ring = manager.GetComponent<StoneCircle_Manager>().StoneCircleRingsList.Last();

            ring.InstantiatedByAPI = true;
            ring.RandomEndPointOverrideList = randomGameObjectList;
            ring.OverrideSettings = true;
            ring.RadiusOverride = 10f;
            ring.NumberOfEndPointsOverride = 10;
            ring.ReGenerateRing();

            return ring;

        }
        /// <summary>
        /// Add a new ring by only specifying manager and a specific prefab to use for endpoints.
        /// </summary>
        /// <param name="manager">The stone circle that will be acted upon.</param>
        /// <param name="endpoint">A single gameObject to be used for endpoints.</param>
        /// <returns></returns>
        public static StoneCircle_Ring AddRing(StoneCircle_Manager manager, GameObject endpoint) {

            manager.AddRing();
            StoneCircle_Ring ring = manager.GetComponent<StoneCircle_Manager>().StoneCircleRingsList.Last();

            ring.InstantiatedByAPI = true;
            ring.RandomEndPointOverrideList.Clear();
            ring.RandomEndPointOverrideList.Add(endpoint);
            ring.OverrideSettings = true;
            ring.RadiusOverride = 10f;
            ring.NumberOfEndPointsOverride = 10;
            ring.ReGenerateRing();

            return ring;

        }
        #endregion
        #region Ring Specific Methods
        /// <summary>
        /// Set the radius of the specified ring.
        /// </summary>
        /// <param name="ring">The ring to be acted upon.</param>
        /// <param name="radius">The new radius of the ring.</param>
        public static void Radius(StoneCircle_Ring ring, float radius) {

            ring.RadiusOverride = radius;
            ring.UpdateRadius();

        }
        /// <summary>
        /// Set the number of endpoints of the specified ring.
        /// </summary>
        /// <param name="ring">The ring to be acted upon.</param>
        /// <param name="numEndPoints">The new number of endpoints (endpoint types are randomized).</param>
        public static void NumberOfEndPoints(StoneCircle_Ring ring, int numEndPoints) {

            ring.NumberOfEndPointsOverride = numEndPoints;
            ring.ReGenerateRing();

        }
        /// <summary>
        /// Set the ring height offset (this offset will be in addition to the height offset set at the generator level).
        /// </summary>
        /// <param name="ring">The ring to be acted upon.</param>
        /// <param name="height">The height offset of each endpoint. If snap to terrain is used the offset will be above or below the terrain surface.</param>
        public static void SetRingOffsetHeight(StoneCircle_Ring ring, float height) {

            ring.RingOffsetHeight = height;
            ring.UpdateRingOffsetHeight();

        }
        /// <summary>
        /// Set the orientation (rotation in Y only) of individual rings.
        /// </summary>
        /// <param name="ring">The ring to be acted upon.</param>
        /// <param name="degrees">The degree rotation to apply to the ring.</param>
        public static void SetRingOrientation(StoneCircle_Ring ring, Vector3 degrees) {

            ring.SetOrientation(degrees);

        }
        /// <summary>
        /// Toggle the orientation of the endpoints to face the center (random rotations are additive).
        /// </summary>
        /// <param name="ring">The ring to be acted upon.</param>
        /// <param name="orientTowardsCenter">Orient endpoints to face the center, random rotations are applied after endpoints are oriented.</param>
        public static void OrientEndpointTowardCenter(StoneCircle_Ring ring, bool orientTowardsCenter) {

            ring.OrientEndPointsTowardCenter = orientTowardsCenter;
            ring.ResetEndPointOrientation();

        }
        /// <summary>
        /// Set the entrance gap size (percentage of endpoints) of the ring, max percentage is 70. 
        /// </summary>
        /// <param name="ring">The ring to be acted upon.</param>
        /// <param name="gapSize">Sets the entrance gap size as a percentage.</param>
        public static void SetRingGapSize(StoneCircle_Ring ring, int gapSize) {

            // default range is between 0 and 70 percent:
            if (gapSize < 0) { gapSize = 0; }
            if (gapSize > 70) { gapSize = 70; }

            ring.EntranceGapSize = gapSize;
            ring.UpdateEntranceGapSize();

        }
        /// <summary>
        /// Set random endpoint rotations, range is between -90f and 90f degrees.
        /// </summary>
        /// <param name="ring">The ring to be acted upon.</param>
        /// <param name="range">The range min/max to be randomly applied.</param>
        public static void SetRandomizeStoneRotations(StoneCircle_Ring ring, Vector2 range) {

            if (range.x < -90f) { range.x = -90f; }
            if (range.y > 90f) { range.y = 90f; }

            ring.SetEndPointRandomRotation(range);

        }
        /// <summary>
        /// Set random tilt & lean of endpoints, range is between -10f and 10f degrees.
        /// </summary>
        /// <param name="ring">The ring to be acted upon.</param>
        /// <param name="range">The range min/max to be randomly applied.</param>
        public static void SetRandomizeTiltAndLean(StoneCircle_Ring ring, Vector2 range) {

            if (range.x < -10f) { range.x = -10f; }
            if (range.y > 10f) { range.y = 10f; }

            ring.SetEndPointRandomTiltLean(range);

        }
        /// <summary>
        /// Set random endpoint placements (shifts endpoints closer/farther from each other, rotating from the center), takes a Vector2 range between -90f and 90f degrees.
        /// </summary>
        /// <param name="ring">The ring to be acted upon.</param>
        /// <param name="range">The range min/max to be randomly applied.</param>
        public static void SetRandomizeStonePlacement(StoneCircle_Ring ring, Vector2 range) {

            if (range.x < -90f) { range.x = -90f; }
            if (range.y > 90f) { range.y = 90f; }

            ring.RandomizePlacement(range);

        }
        /// <summary>
        /// Set random endpoint distance from center, takes a Vector2 range between -50f and 50f percentage of the base radius distance.
        /// </summary>
        /// <param name="ring">The ring to be acted upon.</param>
        /// <param name="range">The range min/max to be randomly applied.</param>
        public static void SetRandomizeRadiusDistance(StoneCircle_Ring ring, Vector2 range) {

            if (range.x < -50f) { range.x = -50f; }
            if (range.y > 50f) { range.y = 50f; }

            ring.RandomizeRadiusDistance(range);

        }
        /// <summary>
        /// Set random endpoint scaling, takes a Vector2 range between 0.25f and 5f.
        /// </summary>
        /// <param name="ring">The ring to be acted upon.</param>
        /// <param name="range">The range min/max to be randomly applied.</param>
        /// <param name="keepProportion">If true, endpoints will be scaled evenly.</param>
        public static void SetRandomizeStoneScale(StoneCircle_Ring ring, Vector2 range, bool keepProportion) {

            if (range.x < -0.25f) { range.x = -0.25f; }
            if (range.y > 5f) { range.y = 5f; }

            ring.SetRandomizeEndPointScale(range);
            if (keepProportion) { ring.SetEndPointProportion(); }

        }
        /// <summary>
        /// Set random endpoint gaps (where stones may be missing for example), takes a Vector2 percentage range between 0 and 100.
        /// </summary>
        /// <param name="ring">The ring to be acted upon.</param>
        /// <param name="range">The range min/max to be randomly applied.</param>
        public static void SetRandomizeStoneGaps(StoneCircle_Ring ring, Vector2 range) {

            if (range.x < 0f) { range.x = 0f; }
            if (range.y > 100f) { range.y = 100f; }

            ring.RandomizeGaps(range);

        }
        /// <summary>
        /// Set replacing gaps with a random prefab from a list of GameObjects.
        /// </summary>
        /// <param name="ring">The ring to be acted upon.</param>
        /// <param name="rubbleList">A list of gameObjects to be randomly pulled from to replace gaps.</param>
        public static void SetReplaceGapsWithRubble(StoneCircle_Ring ring, List<GameObject> rubbleList) {

            ring.RandomRubbleOverrideList = rubbleList;
            ring.RandomizeGapRubble();

        }
        /// <summary>
        /// Set replacing gaps with a specified prefab, takes a GameObject.
        /// </summary>
        /// <param name="ring">The ring to be acted upon.</param>
        /// <param name="rubble">A specific game object to replace gaps with.</param>
        public static void SetReplaceGapsWithRubble(StoneCircle_Ring ring, GameObject rubble) {

            ring.RandomRubbleOverrideList.Clear();
            ring.RandomRubbleOverrideList.Add(rubble);
            ring.RandomizeGapRubble();

        }
        /// <summary>
        /// Remove the specified ring from the circle.
        /// </summary>
        /// <param name="ring">The ring to be removed.</param>
        public static void RemoveRing(StoneCircle_Ring ring) {

            ring.RemoveRing();

        }
        #endregion

    }

}