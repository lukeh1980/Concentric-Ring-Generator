using System.Collections.Generic;
using UnityEngine;

namespace DigitalHorde.ConcentricRings.StoneCircles {

    public class StoneCircle_Preset {

        [System.Serializable]
        public struct StoneCircle_ManagerSettings {

            public Vector3 position;

            public float xOrientation;
            public float yOrientation;
            public float zOrientation;

            public bool useCenterPieceOverride;
            public bool ignoreTerrainOffset;
            public int centerPieceIndex;
            public string centerPiecePath;

            public bool snapEndPointsToTerrain;
            public float endPointHeightOffset;

            public bool hideCollidingTerrainObjects;
            public bool hideSelfOnCollidingTerrainObjects;

        }

        [System.Serializable]
        public struct StoneCircle_RingSettings {

            public int ringID;
            public string ringLabel;

            public int currentEndPointFolderIndex;
            public string currentEndPointFolderPath;

            public bool useEndPointOverride;
            public string endPointOverridePath;

            public bool overrideSettings;
            public int numberOfEndPointsOverride;
            public float radiusOverride;
            public int numberOfEndPoints;
            public float radius;
            public int entranceGapSize;
            public float ringOffsetHeight;
            public bool ignoreTerrainOffset;

            public bool orientEndPointsTowardCenter;
            public float xOrientation;
            public float yOrientation;
            public float zOrientation;

            public bool randomizeEndPointRotation;
            public Vector2 minMaxRandomRotationSlider;
            public bool randomizeEndPointTiltLean;
            public Vector2 minMaxTiltLeanSlider;

            public bool randomizeEndPointPlacement;
            public Vector2 minMaxEndPointPlacementSlider;
            public bool randomizeEndPointRadius;
            public Vector2 minMaxEndPointRadiusSlider;

            public bool randomizeEndPointScale;
            public bool keepProportion;
            public Vector2 randomizeEndPointScaleSlider;

            public bool randomizeEndPointGaps;
            public Vector2 randomizeEndPointGapsSlider;
            public bool replaceGapsWithRubble;
            public int currentGapsRubbleFolderIndex;
            public string currentGapsRubbleFolderPath;

        }

        public StoneCircle_ManagerSettings stoneCircleManager;
        public List<StoneCircle_RingSettings> stoneCircleRingsList;

    }

}