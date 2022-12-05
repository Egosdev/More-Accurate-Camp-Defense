using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameStateManager : MonoBehaviour
{
    #region Singleton
    public static GameStateManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        CurrentGameState = GameState.FreeRoam;
    }
    #endregion

    //Press F to next wave
    public enum GameState
    {
        DayActions,
        FreeRoam,
        Wave,
        WaveCompleted
    }
    private GameState currentGameState;
    public GameState CurrentGameState
    {
        get { return currentGameState; }
        set
        {
            currentGameState = value;
            OnStateChanged(value);
        }
    }

    void OnStateChanged(GameState state)
    {
        if(state == GameState.Wave)
        {
            WaveManager.Instance.NextWave();
        }
    }
}
