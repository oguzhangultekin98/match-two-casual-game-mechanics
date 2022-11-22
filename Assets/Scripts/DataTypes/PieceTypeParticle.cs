using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PieceTypeParticle
{
    [SerializeField]
    private PieceType type;
    public PieceType Type => type;

    [SerializeField]
    private GameObject particlePrefab;
    public GameObject ParticlePrefab => particlePrefab;
}
