using UnityEngine;
using System.Collections;
using ScriptableEvents.Events;

public class ClearablePiece : MonoBehaviour
{
    private bool isBeingCleared = false;
    public bool IsBeingCleared => isBeingCleared;

    private GamePiece piece;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObjectScriptableEvent Event_PieceCleared;

    private void Awake()
    {
        piece = GetComponent<GamePiece>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Clear()
    {
        isBeingCleared = true;
        StartCoroutine(ClearCoroutine());
    }

    public void DestroyPiece()
    {
        Destroy(gameObject);
    }

    private IEnumerator ClearCoroutine()
    {
        Event_PieceCleared.Raise(this.gameObject);
        if (piece.Type != PieceType.REGULAR)
        {
            Destroy(gameObject);
        }
        else
        {
            spriteRenderer.sortingOrder = 999;
            yield return new WaitForSeconds(0.1f);
            SoundManager.Instance.PlayPieceReachedGoalSound();
            Destroy(gameObject);
        }
    }
}