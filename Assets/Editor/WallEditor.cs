using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Wall))]
public class WallEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        Wall wall = (Wall)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Wall Position", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Floor " + (wall.floor + 1).ToString() + ":", EditorStyles.boldLabel);

        DrawGrid(wall);
    }

    private void DrawGrid(Wall wall)
    {
        List<Vector3Int> doors = wall.room.doorSpawnPoints;
        int floor = wall.floor;
        Vector2Int scale = new Vector2Int(wall.room.scale.x, wall.room.scale.z);
        bool[,] check = new bool[(scale.x + 2), (scale.y + 2)];
        foreach (Vector3Int door in doors)
        {
            if (door.y != wall.floor) continue;
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
            if (check[x + 1, scale.y + 1])
            {
                string text = (wall.gridPosition == new Vector3Int(x, floor, scale.y)) ? "O" : "";
                if (GUILayout.Button(text, buttonStyle))
                {
                    wall.gridPosition = new Vector3Int(x, floor, scale.y);
                }
            }
            else
            {
                GUILayout.Space(38f);
            }
        }
        GUILayout.Space(38f);
        GUILayout.EndHorizontal();
        for (int y = scale.y - 1; y >= 0; y--)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < scale.x + 2; x++)
            {
                if (x == 0 || x == scale.x + 1)
                {
                    if (check[x, y + 1])
                    {
                        string text = (wall.gridPosition == new Vector3Int(x - 1, floor, y)) ? "O" : "";
                        if (GUILayout.Button(text, buttonStyle))
                        {
                            wall.gridPosition = new Vector3Int(x - 1, floor, y);
                        }
                    }
                    else
                    {
                        GUILayout.Space(38f);
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
            if (check[x + 1, 0])
            {
                string text = (wall.gridPosition == new Vector3Int(x, floor, -1)) ? "O" : "";
                if (GUILayout.Button(text, buttonStyle))
                {
                    wall.gridPosition = new Vector3Int(x, floor, -1);
                }
            }
            else
            {
                GUILayout.Space(38f);
            }
        }
        GUILayout.Space(38f);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }
}
