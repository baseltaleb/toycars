using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    int[] _numberOfTracks;
    SerializedProperty menuTracks;
    SerializedProperty numberOfTracks;
    bool deleteEntry = false;

    void OnEnable()
    {
        menuTracks = serializedObject.FindProperty("MenuTracks");
        numberOfTracks = serializedObject.FindProperty("NumberOfTracks");
        numberOfTracks.arraySize = menuTracks.arraySize;
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (numberOfTracks.arraySize != menuTracks.arraySize)
            numberOfTracks.arraySize = menuTracks.arraySize;

        EditorGUILayout.PropertyField(menuTracks, new GUIContent("Tracks in this level"));

        if (Event.current.type == EventType.DragPerform)
        {
            serializedObject.ApplyModifiedProperties();
            return;
        }

        if (menuTracks.isExpanded)
            ShowElements();

        DrawPropertiesExcluding(serializedObject, "m_Script", "MenuTracks", "NumberOfTracks");
        serializedObject.ApplyModifiedProperties();
    }

    void ShowElements()
    {
        if (menuTracks.arraySize == 0)
        {
            EditorGUILayout.HelpBox("LIST IS EMPTY", MessageType.Info);
            return;
        }

        for (int i = 0; i < menuTracks.arraySize; i++)
        {
            GUILayout.BeginHorizontal();
            Texture2D tex = AssetPreview.GetAssetPreview(menuTracks.GetArrayElementAtIndex(i).objectReferenceValue as GameObject);
            GUILayout.Box(tex, GUILayout.Height(75), GUILayout.Width(75));
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(menuTracks.GetArrayElementAtIndex(i), GUIContent.none);
            if (GUILayout.Button("×", GUILayout.Width(20)))
            {
                deleteEntry = true;
            }

            GUILayout.EndHorizontal();
            numberOfTracks.GetArrayElementAtIndex(i).intValue = EditorGUILayout.IntField("Number in menu:", numberOfTracks.GetArrayElementAtIndex(i).intValue);

            if (deleteEntry)
            {
                int oldSize = menuTracks.arraySize;
                menuTracks.DeleteArrayElementAtIndex(i);
                numberOfTracks.DeleteArrayElementAtIndex(i);
                if (menuTracks.arraySize == oldSize)
                {
                    menuTracks.DeleteArrayElementAtIndex(i);
                }
                if (numberOfTracks.arraySize == oldSize)
                {
                    numberOfTracks.DeleteArrayElementAtIndex(i);
                }
                deleteEntry = false;
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }

    void OnValidate()
    {
        menuTracks = serializedObject.FindProperty("MenuTracks");
        numberOfTracks = serializedObject.FindProperty("NumberOfTracks");
        numberOfTracks.arraySize = menuTracks.arraySize;
    }

}
