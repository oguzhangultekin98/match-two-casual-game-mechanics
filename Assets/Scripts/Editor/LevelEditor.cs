using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(LevelDataCreator))]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LevelDataCreator level = (LevelDataCreator)target;
        EditorGUILayout.Space();
        EditorGUI.indentLevel++;
        AllBlocksAvailable[,] grid = level.levelGrid.grid;
        #region Style
        GUIStyle tableStyle = new GUIStyle("box");
        tableStyle.padding = new RectOffset(10, 10, 10, 10);
        tableStyle.margin.left = 32;

        GUIStyle headerColumnStyle = new GUIStyle();
        headerColumnStyle.fixedWidth = 35;

        GUIStyle columnStyle = new GUIStyle();
        columnStyle.fixedWidth = 65;

        GUIStyle rowStyle = new GUIStyle();
        rowStyle.fixedHeight = 25;

        GUIStyle rowHeaderStyle = new GUIStyle();
        rowHeaderStyle.fixedWidth = columnStyle.fixedWidth - 1;

        GUIStyle columnHeaderStyle = new GUIStyle();
        columnHeaderStyle.fixedWidth = 30;
        columnHeaderStyle.fixedHeight = 25.5f;

        GUIStyle columnLabelStyle = new GUIStyle();
        columnLabelStyle.fixedWidth = rowHeaderStyle.fixedWidth - 6;
        columnLabelStyle.alignment = TextAnchor.MiddleCenter;
        columnLabelStyle.fontStyle = FontStyle.Bold;

        GUIStyle cornerLabelStyle = new GUIStyle();
        cornerLabelStyle.fixedWidth = 42;
        cornerLabelStyle.alignment = TextAnchor.MiddleRight;
        cornerLabelStyle.fontStyle = FontStyle.BoldAndItalic;
        cornerLabelStyle.fontSize = 14;
        cornerLabelStyle.padding.top = -5;

        GUIStyle rowLabelStyle = new GUIStyle();
        rowLabelStyle.fixedWidth = 25;
        rowLabelStyle.alignment = TextAnchor.MiddleRight;
        rowLabelStyle.fontStyle = FontStyle.Bold;

        GUIStyle enumStyle = new GUIStyle("popup");
        rowStyle.fixedWidth = 65;
        #endregion StyleEnd
        EditorGUILayout.BeginHorizontal(tableStyle);
        for (int x = -1; x < level.levelGrid.xDim; x++)
        {
            EditorGUILayout.BeginVertical((x == -1) ? headerColumnStyle : columnStyle);
            for (int y = -1; y < level.levelGrid.yDim; y++)
            {
                if (x == -1 && y == -1)
                {
                    EditorGUILayout.BeginVertical(rowHeaderStyle);
                    EditorGUILayout.LabelField("[X,Y]", cornerLabelStyle);
                    EditorGUILayout.EndHorizontal();
                }
                else if (x == -1)
                {
                    EditorGUILayout.BeginVertical(columnHeaderStyle);
                    EditorGUILayout.LabelField(y.ToString(), rowLabelStyle);
                    EditorGUILayout.EndHorizontal();
                }
                else if (y == -1)
                {
                    EditorGUILayout.BeginVertical(rowHeaderStyle);
                    EditorGUILayout.LabelField(x.ToString(), columnLabelStyle);
                    EditorGUILayout.EndHorizontal();
                }

                if (x >= 0 && y >= 0)
                {
                    EditorGUILayout.BeginHorizontal(rowStyle);
                    grid[x, y] = (AllBlocksAvailable)EditorGUILayout.EnumPopup(level.levelGrid.grid[x, y], enumStyle);
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Save data into scriptable object"))
        {
            level.CreateLevelDataScriptableObj(level.levelGrid.xDim, level.levelGrid.yDim, grid, level.levelGrid.LevelName);
        }
    }
}