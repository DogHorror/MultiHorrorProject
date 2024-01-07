using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Room))]
public class RoomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        Room room = (Room)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Door Elements", EditorStyles.boldLabel);
        if (room.scale.y > 0)
        {
            for (int i = 0; i < room.scale.y; i++)
            {
                EditorGUILayout.LabelField("Floor " + (i + 1).ToString() + ":", EditorStyles.boldLabel);

                DrawGrid(room, i);
            }
        }
    }

    private void DrawGrid(Room room, int floor)
    {
        List<Vector3Int> doors = room.doors;
        Vector2Int scale = new Vector2Int(room.scale.x, room.scale.z);
        bool[,] check = new bool[(scale.x + 2), (scale.y + 2)];
        foreach (Vector3Int door in doors)
        {
            if (door.y != floor) continue;
            check[door.x + 1, door.z + 1] = true;
        }
        
        float cellSize = 35.0f;
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fixedWidth = cellSize;
        buttonStyle.fixedHeight = cellSize;
        
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.BeginHorizontal();
        GUILayout.Space(38f);
        for (int x = 0; x < scale.x; x++)
        {
            string text = check[x + 1, 0] ? "O" : "X";
            if (GUILayout.Button(text, buttonStyle))
            {
                Vector3Int element = new Vector3Int(x, floor, -1);
                if (room.doors.Contains(element))
                {
                    room.doors.Remove(element);
                }
                else
                {
                    room.doors.Add(element);
                }
            }
        }
        GUILayout.Space(38f);
        GUILayout.EndHorizontal();
        for (int y = 0; y < scale.y; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < scale.x + 2; x++)
            {
                if (x == 0 || x == scale.x + 1)
                {
                    string text = check[x, y + 1] ? "O" : "X";
                    if (GUILayout.Button(text, buttonStyle))
                    {
                        Vector3Int element = new Vector3Int(x - 1, floor, y);
                        if (room.doors.Contains(element))
                        {
                            room.doors.Remove(element);
                        }
                        else
                        {
                            room.doors.Add(element);
                        }
                    }
                    continue;
                }

                GUILayout.Button( (x - 1) + ", " + y, buttonStyle);
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.BeginHorizontal();
        GUILayout.Space(38f);
        for (int x = 0; x < scale.x; x++)
        {
            string text = check[x + 1, scale.y + 1] ? "O" : "X";
            if (GUILayout.Button(text, buttonStyle))
            {
                Vector3Int element = new Vector3Int(x, floor, scale.y);
                if (room.doors.Contains(element))
                {
                    room.doors.Remove(element);
                }
                else
                {
                    room.doors.Add(element);
                }
            }
        }
        GUILayout.Space(38f);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
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
                //EditorGUILayout.LabelField(grid[x, z].ToString(), GUILayout.Width(20));

                // Draw block
                //EditorGUI.DrawRect(GUILayoutUtility.GetRect(cellSize, cellSize), blockColor);
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }
}
