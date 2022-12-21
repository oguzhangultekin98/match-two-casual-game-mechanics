using UnityEngine;
using System.Collections;

public class ClearablePiece : MonoBehaviour
{
    private bool isBeingCleared = false;

    public bool IsBeingCleared => isBeingCleared;

    private GamePiece piece;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        piece = GetComponent<GamePiece>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Clear()
    {
        isBeingCleared = true;
        bool isPartOfGoal = false;
        if (piece.Type == PieceType.REGULAR)
            isPartOfGoal = LevelGoalHandler.Instance.PieceCleared(piece.Type, piece.ColorComponent.Color);
        else
            isPartOfGoal = LevelGoalHandler.Instance.PieceCleared(piece.Type, ColorType.RED);
        StartCoroutine(ClearCoroutine(isPartOfGoal));
    }

    public void DestroyPiece()
    {
        Destroy(gameObject);
    }

    private IEnumerator ClearCoroutine(bool isPiecePartOfGoal)
    {
        if (piece.Type != PieceType.REGULAR)
        {
            Destroy(gameObject);
        }
        else
        {
            if (isPiecePartOfGoal)
            {
                spriteRenderer.sortingOrder = piece.ColorComponent.HowManyPieceSpawned + 999;
                float timeToScatter = ScatterPiece();
                yield return new WaitForSeconds(timeToScatter);
                Vector3 goalLocation = LevelGoalHandler.Instance.GetGoalLocation(piece.ColorComponent.Color);
                MovePieceWorldPosition(goalLocation, time: 1.0f);
                yield return new WaitForSeconds(1f);
                SoundManager.Instance.PlayPieceReachedGoalSound();
            }
            Destroy(gameObject);
        }
    }

    private float ScatterPiece()
    {
        Vector3 pos = transform.position;
        Vector3 randomTargetLoc = new Vector3(pos.x + Random.Range(-0.3f, 0.3f), pos.y + Random.Range(0.1f, 0.3f), pos.z);
        float randomTime = Random.Range(0.1f, 0.3f);
        MovePieceWorldPosition(randomTargetLoc, randomTime);
        return randomTime;
    }

    private void MovePieceWorldPosition(Vector3 targetLoc, float time)
    {
        piece.MovableComponent.MoveOnWorld(transform.position, targetLoc, time);
    }
}