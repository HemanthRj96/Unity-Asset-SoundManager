using UnityEditor;
using Lacobus.Sound;
using UnityEngine;


namespace Lacobus_Editors.Sound
{
    [CustomEditor(typeof(SoundManager))]
    public class SoundManagerEditor : Editor
    {
        SerializedProperty sp_cluster;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            sp_cluster = serializedObject.FindProperty("_soundClusters");

            if (GUILayout.Button("Load all sound clusters"))
            {
                var cluster = Resources.FindObjectsOfTypeAll<SoundCluster>();
                sp_cluster.ClearArray();

                for (int i = 0; i < cluster.Length; ++i)
                {
                    sp_cluster.InsertArrayElementAtIndex(i);
                    sp_cluster.GetArrayElementAtIndex(i).objectReferenceValue = cluster[i];
                }
            }

            EditorGUILayout.PropertyField(sp_cluster);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
