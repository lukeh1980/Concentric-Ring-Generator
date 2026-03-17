using System.Collections.Generic;
using UnityEngine;

namespace DigitalHorde.ConcentricRings {

    public class CRG_Preset {

        [System.Serializable]
        public struct ManagerSettings {

            public Vector3 position;

            public float xOrientation;
            public float yOrientation;
            public float zOrientation;

            public string centerPiecePath;

            public List<CRG_Ring> ringsList;

        }

        [System.Serializable]
        public struct RingSettings {

            public int ringID;
            public string ringLabel;

            public string endPointFolderPath;

            public bool overrideSettings;
            public int numberOfEndPointsOverride;
            public float radiusOverride;
            public int numberOfEndPoints;
            public float radius;

            public bool orientEndPointsTowardCenter;
            public float xOrientation;
            public float yOrientation;
            public float zOrientation;

        }

        public ManagerSettings manager;
        public List<RingSettings> ringsList;

    }

}
