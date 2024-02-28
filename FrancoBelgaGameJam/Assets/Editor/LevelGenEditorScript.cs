using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FrancoGameJam.Genration
{
    [CustomEditor(typeof(LevelGeneratorScript))]
    public class LevelGenEditorScript : Editor
    {
        public override void OnInspectorGUI()
        {
            LevelGeneratorScript Target = (LevelGeneratorScript)target;

            if (GUILayout.Button("Generate Level"))
            {
                if(Target.GenerateLevel())
                    Debug.Log("Generated Level");
                else
                    Debug.Log("Failed to Generate Level");
            }

            base.OnInspectorGUI();
        }
    }
}
