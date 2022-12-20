using System.Collections;
using UnityEngine;

public class MovablePiece : MonoBehaviour
{
    private GamePiece piece;
    private IEnumerator moveCoroutine;

    private void Awake()
    {
        piece = GetComponent<GamePiece>();
    }

    public void MoveOnGrid(int newX, int newY, float time)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = MoveOnGridCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);
    }

    public void MoveOnWorld(Vector3 startPos, Vector3 endPos,float time)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = LerpToPosition(startPos, endPos, time);
        StartCoroutine(moveCoroutine);
    }
    private IEnumerator LerpToPosition(Vector3 startPoint, Vector3 endPoint, float time)
    {
        var percent = 0f;

        var center = (startPoint + endPoint) * 0.5f;

        center -= new Vector3(0, 1, 0);

        var riseRelCenter = startPoint - center;
        var setRelCenter = endPoint - center;

        while (percent < 1)
        {
            yield return null;
            transform.position = Vector3.Lerp(riseRelCenter, setRelCenter, percent);
            transform.position += center;

            percent += Time.deltaTime / time;
        }

        transform.position = endPoint;
    }

    private IEnumerator MoveOnGridCoroutine(int newX, int newY, float time)
    {

        piece.XCord = newX;
        piece.YCord = newY;

        Vector3 startPos = transform.position;
        Vector3 endPos = piece.Grid.GetWorldPosition(newX, newY);

        for (float t = 0; t <= 1 * time; t += Time.deltaTime)
        {
            piece.transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return null;
        }

        piece.transform.position = endPos;
    }
}
