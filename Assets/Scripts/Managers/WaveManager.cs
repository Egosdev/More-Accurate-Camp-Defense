using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    #region Singleton
    public static WaveManager Instance;

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
    }
    #endregion
    private int _currentWave = 0;
    public int numberOfAllExpectedZombiesForWave;
    public int zombieCount;
    public int CurrentWave
    {
        get
        {
            return _currentWave;
        }
    }
    public TMPro.TextMeshProUGUI waveText;
    public TMPro.TextMeshProUGUI zombieCountText;

    public Color colorNormal;
    public Color colorNormalDead;
    public Color colorSpeedy;
    public Color colorSpeedyDead;
    public Color colorFat;
    public Color colorFatDead;

    public Color[] colorNormalHands;
    public Color[] colorSpeedyHands;
    public Color[] colorFatHands;

    //think an algorithm for increasing expected zombie count dependent wave number
    //every wave only one but random fast spawner, just idea
    public void NextWave()
    {
        //Invoke("MakeNight", 1f);
        LightManager.Instance.TurnAllLights(true);
        SoundManager.Instance.PlaySound(CampManager.Instance.player.GetComponent<PlayerAim>().listener.GetComponent<AudioSource>(), SoundManager.Instance.waveStartSFX);
        _currentWave++;
        waveText.text = "Wave " + _currentWave.ToString();

        numberOfAllExpectedZombiesForWave = (5 * _currentWave) + 5;

        zombieCountText.text = "Zombie Count: " + numberOfAllExpectedZombiesForWave.ToString();
        zombieCount = numberOfAllExpectedZombiesForWave;

        int residual = numberOfAllExpectedZombiesForWave % transform.childCount;

        if (residual != 0)
        {
            numberOfAllExpectedZombiesForWave -= residual;
        }

        foreach (Transform spawner in transform)
        {
            spawner.gameObject.GetComponent<Spawner>().RefreshZombieStock(numberOfAllExpectedZombiesForWave / transform.childCount);
        }
        for (int i = 0; i < residual; i++)
        {
            transform.GetChild(i).GetComponent<Spawner>().ResidualZombies(1);
        }
    }

    public void CheckWaveCompleted()
    {
        if (PoolManager.Instance.enemyPoolTransform.childCount == 0)
        {
            foreach (Transform spawner in transform)
            {
                if (spawner.gameObject.GetComponent<Spawner>().readyToSpawnZombieCount != 0) return;
            }
            SoundManager.Instance.PlaySound(CampManager.Instance.player.GetComponent<PlayerAim>().listener.GetComponent<AudioSource>(), SoundManager.Instance.waveFinishedSFX);
            GameStateManager.Instance.CurrentGameState = GameStateManager.GameState.WaveCompleted;
            waveText.text = "Go to Campfire";
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (GameStateManager.Instance.CurrentGameState != GameStateManager.GameState.FreeRoam) return;

            SoundManager.Instance.PlaySingleSoundAtOnce(CampManager.Instance.player.GetComponent<PlayerAim>().ambience.GetComponent<AudioSource>(), SoundManager.Instance.ambience[1]);
            GameStateManager.Instance.CurrentGameState = GameStateManager.GameState.Wave;
        }
    }

    public void MakeNight()
    {
        UIManager.Instance.alphaValue = 0.25f;
    }
}
