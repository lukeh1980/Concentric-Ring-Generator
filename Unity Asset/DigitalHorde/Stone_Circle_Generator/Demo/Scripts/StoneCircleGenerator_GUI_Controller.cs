using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DigitalHorde.ConcentricRings.StoneCircles {

    public class StoneCircleGenerator_GUI_Controller : MonoBehaviour
    {

        [Header("References")]
        [SerializeField] private Camera m_MainCamera;
        [SerializeField] private GameObject m_GhostCircle;
        [SerializeField] private GameObject m_CreateNewStoneCircleButton;

        [Header("Endpoint Libraries")]
        [SerializeField] private List<GameObject> m_RandomEndpointList = new List<GameObject>();
        [SerializeField] private List<GameObject> m_RandomCenterPieceList = new List<GameObject>();
        [SerializeField] private List<GameObject> m_RandomRubbleList = new List<GameObject>();

        [Header("Random Configuration Parameters")]
        [SerializeField] private Vector2Int m_NumberOfRings;
        [SerializeField] private Vector2Int m_NumberOfEndpoints;
        [SerializeField] private Vector2 m_Radius;

        private int m_StoneCircleIncrement = 0;
        private Color m_CreateStoneCircleTextColorOrig = Color.white;

        private void Start()
        {

            m_CreateStoneCircleTextColorOrig = m_CreateNewStoneCircleButton.GetComponentInChildren<TextMeshProUGUI>().color;
            m_GhostCircle = Instantiate(m_GhostCircle);
            m_GhostCircle.SetActive(false);

        }

        private void Update()
        {

            m_GhostCircle.transform.position = GetTerrainContactPosition();

            if (Input.GetMouseButtonDown(0) && m_GhostCircle.activeSelf)
            {

                ToggleGhostCircle();
                CreateNewStoneCircle();

            }

        }

        public void ToggleGhostCircle()
        {

            if (m_GhostCircle.activeSelf)
            {

                m_GhostCircle.SetActive(false);
                m_CreateNewStoneCircleButton.GetComponentInChildren<TextMeshProUGUI>().color = m_CreateStoneCircleTextColorOrig;
                m_CreateNewStoneCircleButton.GetComponent<Button>().interactable = true;


            }
            else
            {

                m_GhostCircle.SetActive(true);
                m_CreateNewStoneCircleButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.grey;
                m_CreateNewStoneCircleButton.GetComponent<Button>().interactable = false;

            }

        }
        private void CreateNewStoneCircle()
        {

            m_GhostCircle.SetActive(false);

            m_StoneCircleIncrement++;
            Vector3 stoneCirclePos = GetTerrainContactPosition();

            StoneCircle_Manager stoneCircleManager = StoneCircleGenerator_API.GenerateStoneCircle(stoneCirclePos, "Stone Circle " + m_StoneCircleIncrement, this);
            StoneCircleGenerator_API.SnapCircleToTerrain(stoneCircleManager, true);
            StoneCircleGenerator_API.AddCenterPiece(stoneCircleManager, m_RandomCenterPieceList);

            int random = Random.Range(m_NumberOfRings.x, m_NumberOfRings.y);

            for (int i = 0; i < random; i++)
            {

                int randomEnpoints = Random.Range(m_NumberOfEndpoints.x, m_NumberOfEndpoints.y);
                float randomRadius = Random.Range(m_Radius.x, m_Radius.y);

                StoneCircle_Ring ring = StoneCircleGenerator_API.AddRing(stoneCircleManager, randomEnpoints, randomRadius, m_RandomEndpointList);

            }

        }

        private Vector3 GetTerrainContactPosition()
        {

            Vector3 pos = Vector3.zero;
            if (m_MainCamera == null) { return pos; }

            RaycastHit[] hits;
            hits = Physics.RaycastAll(m_MainCamera.transform.position, m_MainCamera.transform.TransformDirection(Vector3.forward), Mathf.Infinity);

            for (int i = 0; i < hits.Length; i++)
            {

                if (hits[i].transform.gameObject.GetComponent<Terrain>() != null)
                {

                    pos = hits[i].point;
                    return pos;

                }

            }

            return pos;

        }

    }

}
