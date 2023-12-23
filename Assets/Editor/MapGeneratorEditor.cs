#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(RandomMapGenerator))]
public class RandomMapGeneratorEditor : Editor
{
    private List<int[,]> level;

    private void OnEnable()
    {
        RandomMapGenerator mapGenerator = (RandomMapGenerator)target;
        level = mapGenerator.level;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RandomMapGenerator mapGenerator = (RandomMapGenerator)target;

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
        for (int z = 0; z < gridSize; z++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < gridSize; x++)
            {
                int value = grid[x, z]; // Get the value from the grid

                // Set different colors based on the value
                Color blockColor = GetBlockColor(value);

                // Draw block
                EditorGUI.DrawRect(GUILayoutUtility.GetRect(cellSize, cellSize), blockColor);
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

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
    }
}
#endif
