using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

/// <summary>
/// This editor class contains elements that are used throughout the concentric rings and deritive classes for custom inspectors, all other editor class should inherit from this one.
/// Examples:
///     - locations of background gradients
///     - methods to draw backgrounds, etc.
///     - custom styles
/// </summary>

namespace DigitalHorde.ConcentricRings {

    public class CRG_Editor : Editor {

        private readonly GUIStyle m_LabelStyle = new GUIStyle();
        public GUIStyle LabelStyle { get { return m_LabelStyle; } }
        private Texture m_DarkGreyGradient = null;
        public Texture DarkGreyGradient { get { return m_DarkGreyGradient; } set { m_DarkGreyGradient = value; } }
        private Texture m_LightGreyGradient = null;
        public Texture LightGreyGradient { get { return m_LightGreyGradient; } set { m_LightGreyGradient = value; } }
        private Texture m_BlueGradient = null;
        public Texture BlueGradient { get { return m_BlueGradient; } set { m_BlueGradient = value; } }

        protected virtual void OnEnable() {

            if (EditorGUIUtility.isProSkin) {

                DarkGreyGradient = AssetDatabase.LoadAssetAtPath<Texture2D>(SCG_Configuration.CRG_ROOT + "GUI_Textures/dark-theme-grad.png");
                m_LabelStyle.normal.textColor = new Color(0.75f, 0.75f, 0.75f);

            } else {

                DarkGreyGradient = AssetDatabase.LoadAssetAtPath<Texture2D>(SCG_Configuration.CRG_ROOT + "GUI_Textures/light-theme-grad.png");
                m_LabelStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f);

            }

            m_LabelStyle.fontSize = 13;
            m_LabelStyle.alignment = TextAnchor.UpperCenter;
            m_LabelStyle.fontStyle = FontStyle.Bold;

        }
        protected virtual void DrawHorizontalLine(float topMargin, Color color) {

            Rect hline = new Rect(0, topMargin, EditorGUIUtility.currentViewWidth, 1);
            EditorGUI.DrawRect(hline, color);

        }
        protected virtual void DrawGradientBackground(float topMargin, float height, Texture gradient) {

            GUI.DrawTexture(new Rect(0, topMargin, Screen.width, height), gradient, ScaleMode.StretchToFill, true, 10.0f);

        }
        public virtual string UpdateResourcesDropDown(string folderRoot, List<string> foldersPathsList, List<string> foldersList, int currentIndex) {

            foldersPathsList.Clear();
            foldersList.Clear();

            if (!Directory.Exists(folderRoot)) { Directory.CreateDirectory(folderRoot); AssetDatabase.Refresh(); }

            if (Directory.EnumerateFileSystemEntries(folderRoot).Any()) {

                var folders = AssetDatabase.GetSubFolders(folderRoot);
                foreach (var folder in folders) {

                    if (!foldersPathsList.Contains(folder) && Directory.GetFiles(folder, "*.prefab").Length > 0) { foldersPathsList.Add(folder); }

                }
                foldersPathsList.Sort();

                foreach (string folder in foldersPathsList) { foldersList.Add(folder.Split('/').Last()); }

                if (foldersPathsList.ElementAtOrDefault(currentIndex) != null) { return foldersPathsList[currentIndex]; } else { return string.Empty; }

            } else {

                return string.Empty;

            }

        }

    }

}
