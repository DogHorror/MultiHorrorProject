#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(RandomMapGeneratorObsolete))]
public class RandomMapGeneratorEditor : Editor
{
    private List<int[,]> level;

    private void OnEnable()
    {
        RandomMapGeneratorObsolete mapGenerator = (RandomMapGeneratorObsolete)target;
        level = mapGenerator.level;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RandomMapGeneratorObsolete mapGenerator = (RandomMapGeneratorObsolete)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Level Elements", EditorStyles.boldLabel);

        for (int i = 0; i < level.Count; i++)
        {
            EditorGUILayout.LabelField("Level " + i.ToString() + ":", EditorStyles.boldLabel);

            int[,] grid = level[i];

            // Draw the grid blocks
            DrawGridBlocks(grid);
        }
    }

    private void DrawGridBlocks(int[,] grid)
    {
        float cellSize = 20.0f;
        int gridSize = 9;

        GUILayout.BeginVertical(EditorStyles.helpBox);
        for (int x = 0; x < gridSize; x++)
        {
            GUILayout.BeginHorizontal();
            for (int z = 0; z < gridSize; z++)
            {
                int value = grid[x, z]; // Get the value from the grid

                // Set different colors based on the value
                Color blockColor = GetBlockColor(value);
                //EditorGUILayout.LabelField(grid[x, z].ToString(), GUILayout.Width(20));

                // Draw block
                EditorGUI.DrawRect(GUILayoutUtility.GetRect(cellSize, cellSize), blockColor);
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }
    private Color GetBlockColor(int value)
    {
        Random.InitState(value);
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
    }
    /*
    private Color GetBlockColor(int value)
    {
    
        // Set different colors based on the value
        // Example: Assign different colors for different values
        switch (value)
        {
            case 0:
                return Color.white; // Example: Use white for value 0
            case 1:
                return Color.red; // Example: Use red for value 1
            case 2:
                return Color.blue; // Example: Use blue for value 2
            // Add more cases for other values as needed
            default:
                return Color.gray; // Use default color for other values
        }
    }*/
}
#endif
