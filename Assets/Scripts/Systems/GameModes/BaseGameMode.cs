using System;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class BaseGameMode : MonoBehaviour
{
    protected bool gameOver = false;

    public bool GameOver { get => gameOver; }

    public event Action<bool> GameEnding;
    private void Start()
    {
        InitializeMode();
    }

    public virtual void InitializeMode()
    {

    }

    public virtual void EndGame(bool won)
    {
        if (gameOver) return;
        GameEnding.Invoke(won);
        gameOver = true;
    }
}

