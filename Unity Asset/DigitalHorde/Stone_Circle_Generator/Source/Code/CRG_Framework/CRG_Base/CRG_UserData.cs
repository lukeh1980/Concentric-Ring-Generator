using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

/// <summary>
/// This component is responsible for loading and saving presets and saving prefabs, and anything else related to data.
/// </summary>
/// 

#if (UNITY_EDITOR)
namespace DigitalHorde.ConcentricRings {
    [DisallowMultipleComponent]
    public class CRG_UserData : MonoBehaviour {

        #region Member Variables & Properties
        private CRG_Manager m_RingManager;
        public CRG_Manager RingManager { get { return m_RingManager; } set { m_RingManager = value; } }
        protected string m_PresetName;
        protected string m_PrefabName;
        protected string m_ParentPath;
        #endregion

        void Reset() { 
            
            this.hideFlags = HideFlags.HideInInspector; 
        
        }

        protected virtual void SetFileInformation() {

            m_PresetName = "Concentric_Ring_Set_Preset_" + DateTimeOffset.Now.ToUnixTimeSeconds();
            m_PrefabName = "Concentric_Ring_Set_" + DateTimeOffset.Now.ToUnixTimeSeconds();
            m_ParentPath = "Assets/VarcanRex/Stone_Circle_Generator/UserData";

        }

        #region Custom Methods
        public virtual void SaveAsPrefab() {

            GetComponent<CRG_Manager>().enabled = false;
            foreach (var comp in GetComponents<CRG_Ring>()) { comp.enabled = false; }

            // duplicate our ring and remove components from the root:
            GameObject temp = Instantiate(gameObject);

            DestroyImmediate(temp.GetComponent<CRG_Manager>());
            DestroyImmediate(temp.GetComponent<CRG_UserData>());

            foreach (var comp in temp.GetComponents<CRG_Ring>()) { DestroyImmediate(comp); }

            if (RingManager.SavePositionRotation) { 
                
                temp.gameObject.transform.SetPositionAndRotation(transform.position, transform.rotation); 
            
            } else {

                temp.gameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            }

            PrefabProcessing(temp);
            SavePrefabToFile(temp);

            GetComponent<CRG_Manager>().enabled = true;
            foreach (var comp in GetComponents<CRG_Ring>()) { comp.enabled = true; }

        }
        protected virtual void PrefabProcessing(GameObject prefab) { } // additional processing in implementations
        protected virtual void SavePrefabToFile(GameObject prefab) {

            SetFileInformation();

            // check if our save directory exists and create it if not:
            string folderName = "SavedPrefabs";
            if (!Directory.Exists(m_ParentPath + "/" + folderName)) { AssetDatabase.CreateFolder(m_ParentPath, folderName); }

            // set our prefab save path & name and save the prefab:
            string localPath = m_ParentPath + "/" + folderName + "/" + m_PrefabName + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(prefab, localPath);
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();

            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(localPath);
            Selection.activeObject = obj;

            // destroy temp object:
            DestroyImmediate(prefab);

        }
        public virtual void SaveAsPreset() {

            CRG_Preset savedPreset = new CRG_Preset();
            savedPreset.manager.position = RingManager.transform.position;

            savedPreset.manager.xOrientation = RingManager.XOrientation;
            savedPreset.manager.yOrientation = RingManager.YOrientation;
            savedPreset.manager.zOrientation = RingManager.ZOrientation;

            if (RingManager.Center != null && RingManager.CenterPrefab != null) { savedPreset.manager.centerPiecePath = AssetDatabase.GetAssetPath(RingManager.CenterPrefab); }

            savedPreset.manager.ringsList = RingManager.RingsList;
            
            List<CRG_Preset.RingSettings> rings = new List<CRG_Preset.RingSettings>();

            foreach (var ring in RingManager.RingsList) {

                CRG_Preset.RingSettings ringSettings = new CRG_Preset.RingSettings();
                ringSettings = GenerateRingSettings(ring);
                rings.Add(ringSettings);

            }

            rings.Reverse();
            savedPreset.ringsList = rings;
            string json = JsonUtility.ToJson(savedPreset, true);

            SaveJSONToFile(json);

        }
        protected virtual CRG_Preset.RingSettings GenerateRingSettings(CRG_Ring ring) {

            CRG_Preset.RingSettings ringSettings = new CRG_Preset.RingSettings();

            ringSettings.ringID = ring.RingID;
            ringSettings.ringLabel = ring.RingLabel;
            if (ring.EndPoint != null) { ringSettings.endPointFolderPath = AssetDatabase.GetAssetPath(ring.EndPoint); }
            ringSettings.overrideSettings = ring.OverrideSettings;
            ringSettings.numberOfEndPointsOverride = ring.NumberOfEndPointsOverride;
            ringSettings.radiusOverride = ring.RadiusOverride;
            ringSettings.numberOfEndPoints = ring.NumberOfEndPoints;
            ringSettings.radius = ring.Radius;

            ringSettings.orientEndPointsTowardCenter = ring.OrientEndPointsTowardCenter;
            ringSettings.xOrientation = ring.XOrientation;
            ringSettings.yOrientation = ring.YOrientation;
            ringSettings.zOrientation = ring.ZOrientation;

            return ringSettings;

        }
        protected virtual void SaveJSONToFile(string json) {

            SetFileInformation();

            // check if our save directory exists and create it if not:
            string folderName = "SavedPresets";
            if (!Directory.Exists(m_ParentPath + "/" + folderName)) { AssetDatabase.CreateFolder(m_ParentPath, folderName); }
            string localPath = m_ParentPath + "/" + folderName + "/" + m_PresetName + ".json";

            System.IO.File.WriteAllText(localPath, json);

            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();

        }
        public virtual void LoadPreset(TextAsset JSON) {
            
            CRG_Preset json = JsonUtility.FromJson<CRG_Preset>(JSON.text);
            RingManager.JSONPreset = JSON;

            // remove all existing rings and ring components:
            CRG_Ring[] currentRings = gameObject.GetComponents<CRG_Ring>();
            foreach (var ring in currentRings) {

                DestroyImmediate(ring.RingRoot);
                DestroyImmediate(ring);

            }
            RingManager.RingsList.Clear();

            if (RingManager.Center != null) { DestroyImmediate(RingManager.Center); }

            // update position and orientation settings:
            //RingManager.transform.position = json.manager.position; // this probably isn't desirable, consider putting a toggle to save/load ring location
            RingManager.XOrientation = json.manager.xOrientation;
            RingManager.YOrientation = json.manager.yOrientation;
            RingManager.ZOrientation = json.manager.zOrientation;

            // check for center peice and add it:
            if (!String.IsNullOrEmpty(json.manager.centerPiecePath)) {

                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(json.manager.centerPiecePath);
                RingManager.Center = null;
                RingManager.CenterPrefab = obj;
                RingManager.AddCenterObject();

            }

            // add each ring:
            for (int i = 0; i < json.ringsList.Count; i++) { AddRingFromPreset(json.ringsList[i]); }

            RingManager.SetOrientation(new Vector3(RingManager.XOrientation, RingManager.YOrientation, RingManager.ZOrientation));

        }
        protected virtual void AddRingFromPreset(CRG_Preset.RingSettings ringSettings) {

            // create a new ring:
            CRG_Ring newRing = RingManager.gameObject.AddComponent<CRG_Ring>();
            GameObject ringRoot = new GameObject();

            newRing.RingID = ringSettings.ringID;

            newRing.RingRoot = ringRoot;
            newRing.RingRoot.name = "Ring " + newRing.RingID;
            newRing.RingRoot.transform.SetPositionAndRotation(transform.position, transform.rotation);
            newRing.RingRoot.transform.parent = transform;
            newRing.RingManager = GetComponent<CRG_Manager>();

            newRing.RingLabel = ringSettings.ringLabel;

            // load endpoint if set:
            if (!String.IsNullOrEmpty(ringSettings.endPointFolderPath)) {

                newRing.EndPoint = AssetDatabase.LoadAssetAtPath<GameObject>(ringSettings.endPointFolderPath);

                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(ringSettings.endPointFolderPath);
                newRing.EndPoint = obj;

            }

            newRing.OverrideSettings = ringSettings.overrideSettings;

            if (ringSettings.overrideSettings) {

                newRing.NumberOfEndPointsOverride = ringSettings.numberOfEndPointsOverride;
                newRing.RadiusOverride = ringSettings.radiusOverride;

            }

            newRing.NumberOfEndPoints = ringSettings.numberOfEndPoints;
            newRing.Radius = ringSettings.radius;

            newRing.OrientEndPointsTowardCenter = ringSettings.orientEndPointsTowardCenter;
            newRing.XOrientation = ringSettings.xOrientation;
            newRing.YOrientation = ringSettings.yOrientation;
            newRing.ZOrientation = ringSettings.zOrientation;

            RingManager.RingsList.Add(newRing);

            newRing.ReGenerateRing();
            newRing.SetOrientation(new Vector3(newRing.XOrientation, newRing.YOrientation, newRing.ZOrientation));

        }
        #endregion

    }

}
#endif