using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;
    [SerializeField] private ColorSpriteScriptableObj[] colorSpriteScriptableObjects;
    [SerializeField] private PieceTypeParticle[] typeParticles;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void CreatePieceClearParticle(PieceType type, Vector3 loc, ColorType colorType)
    {
        if (type == PieceType.REGULAR)
        {
            Color pieceColor = GetRGBColor(colorType);
            PieceTypeParticle relatedTypeParticle = FindRelatedTypeParticle(type);
            Vector3 pos = AdjustLocToMakeParticlesInfront(loc);
            GameObject particleParentObj = Instantiate(relatedTypeParticle.ParticlePrefab, pos, Quaternion.identity);
            ColorParticles(particleParentObj, pieceColor);
            Destroy(particleParentObj, 2f);
        }
        else
        {
            Debug.LogWarning("Particle not implemented");
        }
    }

    private void ColorParticles(GameObject particleParentObj, Color color)
    {
        ParticleSystem[] particleSystems = particleParentObj.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < particleSystems.Length; i++)
        {
            var settings = particleSystems[i].main;
            settings.startColor = new ParticleSystem.MinMaxGradient(color);
        }
    }

    private Vector3 AdjustLocToMakeParticlesInfront(Vector3 loc)
    {
        loc.z = -10;
        return loc;
    }

    private PieceTypeParticle FindRelatedTypeParticle(PieceType colorType)
    {
        for (int i = 0; i < typeParticles.Length; i++)
        {
            if (typeParticles[i].Type == colorType)
            {
                return typeParticles[i];
            }
        }
        return new PieceTypeParticle();
    }

    private Vector4 GetRGBColor(ColorType colorType)
    {
        ColorSprite colorSprite = FindRelatedColorSprite(colorType);
        return colorSprite.GetRGBColor();
    }

    private ColorSprite FindRelatedColorSprite(ColorType colorType)
    {
        for (int i = 0; i < colorSpriteScriptableObjects.Length; i++)
        {
            if (colorSpriteScriptableObjects[i].value.color == colorType)
            {
                return colorSpriteScriptableObjects[i].value;
            }
        }
        return new ColorSprite();
    }
}
