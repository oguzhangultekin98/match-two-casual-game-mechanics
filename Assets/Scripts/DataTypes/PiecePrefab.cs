using UnityEngine;

[System.Serializable]
public struct PiecePrefab
{
    [SerializeField]
    private PieceType type;
    public PieceType Type => type;

    [SerializeField]
    private GameObject prefab;
    public GameObject Prefab => prefab;

    [SerializeField,Range(0f,1f)]
    private float spawnRate;
    public float SpawnRate => spawnRate;
}