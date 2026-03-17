using UnityEngine;
using UnityEditor;

/// <summary>
/// Add the Stone Ring Generator menu item, creates a new stone ring in the center of the viewport and adds a single ring.
/// </summary>
/// 

#if (UNITY_EDITOR)
namespace DigitalHorde.ConcentricRings.StoneCircles {

    public class StoneCircle_Menu : MonoBehaviour {

        [MenuItem("Tools/Stone Circle Generator/New Stone Circle")]
        private static void CreateStoneRings() {

            Camera view = UnityEditor.SceneView.lastActiveSceneView.camera;

            Ray ray = view.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;

            Vector3 pos = Vector3.zero;
            if (Physics.Raycast(ray, out hit)) { pos = hit.point; } else { pos = view.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, view.nearClipPlane)); }

            GameObject newRing = new GameObject("Stone Circle Manager");
            newRing.transform.position = pos;

            newRing.AddComponent<StoneCircle_Manager>();

            Selection.activeGameObject = newRing;

            newRing.GetComponent<StoneCircle_Manager>().AddRing();

        }

    }

}
#endif