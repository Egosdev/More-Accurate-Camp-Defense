using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Singleton
    public static UIManager Instance;

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

    public GameObject npcPanel;
    public GameObject npcPanelHealthBar;
    public TextMeshProUGUI npcPanelNameSurname;
    public TextMeshProUGUI npcPanelAge;
    public TextMeshProUGUI npcPanelGender;
    public TextMeshProUGUI npcPanelMartialStatus;
    public TextMeshProUGUI npcPanelRelationship;
    public TextMeshProUGUI npcPanelBuffMultiplier;
    public TextMeshProUGUI npcPanelCurseMultiplier;
    public GameObject npcPanelBuffIcon;
    public GameObject npcPanelCurseIcon;
    public Button npcPanelRecruitButton;
    public Button npcPanelFlirtButton;
    public Sprite[] perks;
    public GameObject displayWhichNpc;
    public GameObject debugPanel;
    public GameObject craftingPanel;
    public Slider materialSlider;
    public Slider personalitySlider;
    public Slider foodSlider;
    public Canvas DayActionCanvas;
    public Image transition;
    public float alphaValue;

    public void CloseNPCStatsPanel()
    {
        npcPanel.SetActive(false);
    }

    public void GiveGiftButton()
    {
        if (CampManager.Instance.giftCount > 0)
        {
            CampManager.Instance.giftCount--;
            CampManager.Instance.giftText.text = CampManager.Instance.giftCount.ToString();
            displayWhichNpc.GetComponent<NPCStat>().ChangeRelationship(10);
            displayWhichNpc.GetComponent<NPCStat>().OpenNPCStatsPanel();
        }
    }

    public void RecruitButton()
    {
        if (displayWhichNpc.GetComponentInChildren<NPCBrain>().job == 3) return;

        if (CampManager.Instance.reputation > 0 && !displayWhichNpc.GetComponentInChildren<NPCBrain>().isSoldier)
        {
            CampManager.Instance.reputation--;
            CampManager.Instance.reputationText.text = CampManager.Instance.reputation.ToString();
            displayWhichNpc.GetComponentInChildren<NPCBrain>().Recruit();
            displayWhichNpc.GetComponent<NPCStat>().OpenNPCStatsPanel();
        }
    }
    public void FlirtButton()
    {
        displayWhichNpc.GetComponentInChildren<NPCBrain>().Flirt();
        displayWhichNpc.GetComponent<NPCStat>().OpenNPCStatsPanel();
    }
    public void HealButton()
    {
        if(CampManager.Instance.pillsCount > 0 && displayWhichNpc.GetComponentInChildren<HealthSystem>().health != displayWhichNpc.GetComponentInChildren<HealthSystem>().maxHealth)
        {
            CampManager.Instance.pillsCount--;
            CampManager.Instance.pillsText.text = CampManager.Instance.pillsCount.ToString();
            displayWhichNpc.GetComponentInChildren<HealthSystem>().Heal(25);
            displayWhichNpc.GetComponent<NPCStat>().ChangeRelationship(1);
            displayWhichNpc.GetComponent<NPCStat>().OpenNPCStatsPanel();
        }
    }

    public void DoTransition(bool isTransparent)
    {
        if(isTransparent)
        {
            alphaValue = 1;
        }
        else
        {
            alphaValue = 0;
        }
    }

    private void Update()
    {
        transition.color = new Color(0, 0, 0, Mathf.MoveTowards(transition.color.a, alphaValue, Time.deltaTime));

        if(Input.GetKeyDown(KeyCode.Delete))
        {
            if(debugPanel.activeSelf)
                debugPanel.SetActive(false);
            else
                debugPanel.SetActive(true);

            if (craftingPanel.activeSelf)
                craftingPanel.SetActive(false);
        }
    }
}
