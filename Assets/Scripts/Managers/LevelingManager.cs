using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelingManager : MonoBehaviour
{
    #region Singleton
    public static LevelingManager Instance;

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

    //5x^2 + 95x formula
    Slider slider;
    public int currentLevel;
    public int baseExp;
    [SerializeField] int expBarFillSpeed;
    [SerializeField] int customExp;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] RipplePostProcessor ripplePostProcessorScript;

    [Header("Don't Touch")]
    [SerializeField] int currentExp;
    [SerializeField] int requiredExpForNextLevel;
    [SerializeField] float target;
    [SerializeField] int targetMemory;
    private void Start()
    {
        currentExp = baseExp;
        requiredExpForNextLevel = baseExp;
        slider = GetComponent<Slider>();
        CalculateNextLevelingFormula();
    }

    private void Update()
    {
        if (target == 0)
        {
            slider.value = Mathf.MoveTowards(slider.value, target, Time.deltaTime * expBarFillSpeed * (currentLevel * (currentLevel/2)));
            if (slider.value == 0)
            {
                target = targetMemory;
            }
        }
        else
        {
            slider.value = Mathf.MoveTowards(slider.value, target, Time.deltaTime * expBarFillSpeed * currentLevel);
            if (slider.value == target)
            {
                targetMemory = 0;
            }

        }

        if (slider.value == requiredExpForNextLevel)
        {
            CalculateNextLevelingFormula();
        }
        slider.value = Mathf.Clamp(slider.value, 0, requiredExpForNextLevel);

        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    GainExp(customExp);
        //}
    }

    public void GainExp(int value)
    {
        currentExp += (int)Mathf.Round(value * CampManager.Instance.player.GetComponent<PlayerAim>().experienceMultiplier);
        target = currentExp;

        if(currentExp >= requiredExpForNextLevel)
            ripplePostProcessorScript.RippleEffect();
    }

    private void CalculateNextLevelingFormula()
    {
        currentLevel++;
        if(currentLevel != 1)
            SoundManager.Instance.PlaySound(CampManager.Instance.player.GetComponent<PlayerAim>().listener.GetComponent<AudioSource>(), SoundManager.Instance.levelUpSFX);
        levelText.text = currentLevel.ToString();
        if (currentExp < requiredExpForNextLevel)
            targetMemory += currentExp - requiredExpForNextLevel;
        else
            targetMemory = currentExp - requiredExpForNextLevel;

        currentExp -= requiredExpForNextLevel;
        requiredExpForNextLevel += (currentLevel - 1) * 10;
        slider.maxValue = requiredExpForNextLevel;
        target = 0;
        CampManager.Instance.reputation++;
        CampManager.Instance.reputationText.text = "Reputation: " + CampManager.Instance.reputation.ToString();
    }
}
