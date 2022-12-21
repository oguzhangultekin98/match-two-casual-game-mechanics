using System.Collections.Generic;
using UnityEngine;

public class Match
{
    public readonly List<GamePiece> gamePieces = new List<GamePiece>();

    public void AddGamePiece(GamePiece piece)
    {
        gamePieces.Add(piece);
    }

    public void AddGamePieces(List<GamePiece> pieces)
    {
        gamePieces.AddRange(pieces);
    }
}
