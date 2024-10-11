using System;
using UnityEditor;
using UnityEngine;

public class SimpleGridSpawner : EditorWindow
{
    private GameObject[] _selectedPrefabs = Array.Empty<GameObject>();
    private GameObject[,] _grid;
    private float[,] _rotationYGrid;
    private int _gridSizeX, _gridSizeZ;
    private float _spacingX = 1f, _spacingZ = 1f, _spacingY;
    private bool _deleteMode;
    private Transform _parentObject;
    private int _selectedPrefabIndex = -1;
    private Vector2 _scrollPosition;
    private float _currentRotationY;
    private bool _showPrefabList = true;

    [MenuItem("Tools/Grid System")]
    public static void ShowWindow()
    {
        GetWindow<SimpleGridSpawner>("Grid System");
    }

    private void OnGUI()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height));

        GUILayout.Label("Grid Settings", EditorStyles.boldLabel);
        _gridSizeX = EditorGUILayout.IntSlider("Grid Size X", _gridSizeX, 0, 10);
        _gridSizeZ = EditorGUILayout.IntSlider("Grid Size Z", _gridSizeZ, 0, 10);
        _spacingX = EditorGUILayout.FloatField("Spacing X", _spacingX);
        _spacingZ = EditorGUILayout.FloatField("Spacing Z", _spacingZ);
        _spacingY = EditorGUILayout.FloatField("Spacing Y (Height)", _spacingY);

        GUILayout.Space(10);
        GUILayout.Label("Prefab Settings", EditorStyles.boldLabel);
        _showPrefabList = EditorGUILayout.Foldout(_showPrefabList, "Prefab List");
        if (_showPrefabList) ShowPrefabList();

        _currentRotationY = EditorGUILayout.Slider("Prefab Y Rotation", _currentRotationY, 0, 360);
        ShowPrefabPreviewButtons();

        if (GUILayout.Button("Reset Grid")) ResetGrid();
        if (GUILayout.Button("Delete Mode: " + (_deleteMode ? "ON" : "OFF"))) _deleteMode = !_deleteMode;

        DrawGridPreview();
        if (GUILayout.Button("Spawn Prefabs in Scene")) SpawnPrefabsInScene();

        EditorGUILayout.EndScrollView();
    }

    private void ShowPrefabList()
    {
        for (int i = 0; i < _selectedPrefabs.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            _selectedPrefabs[i] = (GameObject)EditorGUILayout.ObjectField("Prefab " + (i + 1), _selectedPrefabs[i], typeof(GameObject), false);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("+", GUILayout.Width(30))) AddNewPrefabSlot();
        if (GUILayout.Button("-", GUILayout.Width(30))) RemoveLastPrefabSlot();
        EditorGUILayout.EndHorizontal();
    }

    private void ShowPrefabPreviewButtons()
    {
        GUILayout.Space(10);
        GUILayout.Label("Prefab Previews", EditorStyles.boldLabel);

        float windowWidth = position.width;
        int maxColumns = Mathf.FloorToInt(windowWidth / 60);
        int currentColumn = 0;

        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < _selectedPrefabs.Length; i++)
        {
            if (_selectedPrefabs[i] != null)
            {
                Texture2D previewTexture = AssetPreview.GetAssetPreview(_selectedPrefabs[i]);
                if (GUILayout.Button(previewTexture, GUILayout.Width(50), GUILayout.Height(50))) _selectedPrefabIndex = i;

                currentColumn++;
                if (currentColumn >= maxColumns)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    currentColumn = 0;
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawGridPreview()
    {
        if (_grid == null || _grid.GetLength(0) != _gridSizeX || _grid.GetLength(1) != _gridSizeZ)
        {
            _grid = new GameObject[_gridSizeX, _gridSizeZ];
            _rotationYGrid = new float[_gridSizeX, _gridSizeZ];
        }

        float windowWidth = position.width;
        float cellSize = Mathf.Min(windowWidth / _gridSizeX - 10, 50);

        for (int z = _gridSizeZ - 1; z >= 0; z--)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < _gridSizeX; x++)
            {
                if (GUILayout.Button(_grid[x, z] != null ? AssetPreview.GetAssetPreview(_grid[x, z]) : null, GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                {
                    if (_deleteMode)
                    {
                        _grid[x, z] = null;
                        _rotationYGrid[x, z] = 0;
                    }
                    else if (_selectedPrefabIndex != -1 && _selectedPrefabs[_selectedPrefabIndex] != null)
                    {
                        _grid[x, z] = _selectedPrefabs[_selectedPrefabIndex];
                        _rotationYGrid[x, z] = _currentRotationY;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void ResetGrid()
    {
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int z = 0; z < _gridSizeZ; z++)
            {
                _grid[x, z] = null;
                _rotationYGrid[x, z] = 0;
            }
        }
    }

    private void SpawnPrefabsInScene()
    {
        if (_parentObject == null) _parentObject = new GameObject("GridParent").transform;

        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int z = 0; z < _gridSizeZ; z++)
            {
                if (_grid[x, z] != null)
                {
                    Vector3 transformPosition = new Vector3(x * _spacingX, _spacingY, z * _spacingZ);
                    GameObject spawned = (GameObject)PrefabUtility.InstantiatePrefab(_grid[x, z]);
                    spawned.transform.position = transformPosition;
                    spawned.transform.SetParent(_parentObject);
                    spawned.transform.GetChild(0).rotation = Quaternion.Euler(0, _rotationYGrid[x, z], 0);
                }
            }
        }
        _spacingY += 0.1f;
    }

    private void AddNewPrefabSlot() => Array.Resize(ref _selectedPrefabs, _selectedPrefabs.Length + 1);

    private void RemoveLastPrefabSlot()
    {
        if (_selectedPrefabs.Length > 0) Array.Resize(ref _selectedPrefabs, _selectedPrefabs.Length - 1);
    }
}