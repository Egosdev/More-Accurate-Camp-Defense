using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour, LightManager.ILightSetter
{
    [SerializeField] GameObject flashlight;
    [SerializeField] GameObject circleLight;

    public void SetLight(bool turnOn)
    {
        if (turnOn)
            Invoke("OpenFlashlight", 0.5f);
        else
            flashlight.SetActive(false);
        circleLight.SetActive(turnOn);
    }

    public void OpenFlashlight()
    {
        flashlight.SetActive(true);
    }

    void Update()
    {
        if (GameStateManager.Instance.CurrentGameState == GameStateManager.GameState.DayActions || GameStateManager.Instance.CurrentGameState == GameStateManager.GameState.FreeRoam) return;

        if(Input.GetKeyDown(KeyCode.N))
        {
            if (flashlight.activeSelf)
                flashlight.SetActive(false);
            else
                flashlight.SetActive(true);
        }
    }
}
