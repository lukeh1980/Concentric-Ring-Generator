using UnityEditor;
using UnityEngine;

namespace DigitalHorde.ConcentricRings.StoneCircles {

    [CustomEditor(typeof(StoneCircleGenerator_API_Reference))]
    public class StoneCircleGenerator_API_Reference_Editor : CRG_Editor {

        StoneCircleGenerator_API_Reference m_StoneCircleAPIReferenceTarget;
        SerializedProperty m_InstantiatorReference;

        protected override void OnEnable() {

            base.OnEnable();
            m_StoneCircleAPIReferenceTarget = (StoneCircleGenerator_API_Reference)target;
            m_InstantiatorReference = serializedObject.FindProperty("m_InstantiatorReference");

        }

        public override void OnInspectorGUI() {

            DrawGradientBackground(0f, 60, DarkGreyGradient);
            GUILayout.Label("Stone Circle Generator Instantiated by API", LabelStyle);

            if (m_StoneCircleAPIReferenceTarget.InstantiatorReference != null) {

                EditorGUILayout.PropertyField(m_InstantiatorReference, new GUIContent("Stone Circle Generator Reference"), GUILayout.Height(20));

            }

            //base.OnInspectorGUI();

        }

    }

}
