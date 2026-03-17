using UnityEngine;

namespace DigitalHorde.ConcentricRings.StoneCircles {

    public class StoneCircleGenerator_API_Reference : MonoBehaviour {

        [SerializeField] private MonoBehaviour m_InstantiatorReference;
        public MonoBehaviour InstantiatorReference { get { return m_InstantiatorReference; } set { m_InstantiatorReference = value; } }

    }

}
