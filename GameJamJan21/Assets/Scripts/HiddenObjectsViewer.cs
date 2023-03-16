using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public class HiddenObjectsViewer : EditorWindow
{
    [MenuItem("House of Secrets/Hidden Objects Viewer")]
    public static void ShowWindow()
    {
        var window = GetWindow<HiddenObjectsViewer>();
        window.titleContent = new GUIContent("Hidden Objects Viewer");
        window.GatherHiddenObjects();
    }

    #region GUI

    private static readonly GUILayoutOption ButtonWidth = GUILayout.Width(80);
    private static readonly GUILayoutOption BigButtonHeight = GUILayout.Height(35);

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Refresh"))
            {
                GatherHiddenObjects();
            }
        }
        GUILayout.EndHorizontal();
        bool odd = false;
        EditorGUILayout.LabelField("Hidden Objects (" + HiddenObjects.Count + ")", (GUIStyle)"ProjectBrowserHeaderBgMiddle");
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < HiddenObjects.Count; i++)
        {
            var hiddenObject = HiddenObjects[i];
            GUILayout.BeginHorizontal();
            {
                var gone = hiddenObject == null;
                GUILayout.Label(gone ? "null" : hiddenObject.name, (odd ? (GUIStyle)"ObjectPickerResultsOdd" : (GUIStyle)"ObjectPickerResultsEven"), GUILayout.ExpandWidth(true));
                odd = !odd;
                if (gone)
                {
                    GUILayout.Box("Select", ButtonWidth);
                    GUILayout.Box("Show", ButtonWidth);
                    GUILayout.Box("Delete", ButtonWidth);
                }
                else
                {
                    if (GUILayout.Button("Select", (GUIStyle)"sv_label_1", ButtonWidth))
                    {
                        Selection.activeGameObject = hiddenObject;
                    }
                    if (GUILayout.Button(IsHidden(hiddenObject) ? "Show" : "Hide", IsHidden(hiddenObject) ? (GUIStyle)"sv_label_0" : (GUIStyle)"sv_label_3", ButtonWidth))
                    {
                        hiddenObject.hideFlags ^= HideFlags.HideInHierarchy;
                        EditorSceneManager.MarkSceneDirty(hiddenObject.scene);
                    }
                    if (GUILayout.Button("Delete", (GUIStyle)"sv_label_5", ButtonWidth))
                    {
                        var scene = hiddenObject.scene;
                        DestroyImmediate(hiddenObject);
                        EditorSceneManager.MarkSceneDirty(scene);
                        GatherHiddenObjects();
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    #endregion

    #region Hidden Objects

    private List<GameObject> HiddenObjects = new List<GameObject>();
    private Vector2 scrollPos;

    private void GatherHiddenObjects()
    {
        HiddenObjects.Clear();

        var allObjects = FindObjectsOfType<GameObject>();
        foreach (var go in allObjects)
        {
            if ((go.hideFlags & HideFlags.HideInHierarchy) != 0)
            {
                HiddenObjects.Add(go);
            }
        }

        Repaint();
    }

    private static bool IsHidden(GameObject go)
    {
        return (go.hideFlags & HideFlags.HideInHierarchy) != 0;
    }

    #endregion
}