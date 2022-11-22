using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private Sound[] sounds;
    [SerializeField] private Sound pieceReachedGoal;
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

    private void Start()
    {
        foreach (var s in sounds)
        {
            SetNecessarySoundArguments(s);
        }
        SetNecessarySoundArguments(pieceReachedGoal);
    }

    private void SetNecessarySoundArguments(Sound s)
    {
        s.source = gameObject.AddComponent<AudioSource>();
        s.source.clip = s.clip;
        s.source.volume = s.volume;
    }

    public void PlayPieceReachedGoalSound()
    {
        pieceReachedGoal.source.Play();
    }

    public void PlayPieceClearedSound(PieceType pieceType)
    {
        AudioSource relatedSound;
        switch (pieceType)
        {
            case PieceType.REGULAR:
                relatedSound = FindRelatedSound(pieceType);
                relatedSound.Play();
                break;
            case PieceType.BALLOON:
                relatedSound = FindRelatedSound(pieceType);
                relatedSound.Play();
                break;
            case PieceType.DUCK:
                relatedSound = FindRelatedSound(pieceType);
                relatedSound.Play();
                break;
            default:
                Debug.LogWarning("Not Implemented Sound");
                break;
        }
    }

    private AudioSource FindRelatedSound(PieceType type)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].pieceType == type)
                return sounds[i].source;
        }
        Debug.LogWarning("Not Implemented Sound");
        return null;
    }
}
