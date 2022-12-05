using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCStat : MonoBehaviour
{
    public string nameSurname;
    public int age;
    public int gender; //0 male, 1 female
    public GameObject marriedPersonWith;
    public int playerRelationship;
    public float buffMultiplier = 1;
    public float curseMultiplier = 1;
    public GameObject feedbackIcons;
    public GameObject statusHolder;
    Coroutine feedbackCoroutine;

    public AbilityType buffType;
    public AbilityType curseType;
    public enum AbilityType
    {
        WalkSpeed = 0,
        Damage = 1,
        HeadShotRate = 2,
        ReloadSpeed = 3,
        AccurateAim = 4,
        ExperienceMultiplier = 5,
        MaxHealth = 6,
        MaterialDiscount = 7,
    }
    private void Start()
    {
        SetAbilities(false);
    }

    public void SetAbilities(bool isMarriage)
    {
        if (isMarriage)
        {
            CampManager.Instance.player.GetComponent<PlayerAim>().RefreshMultipliers();
            CampManager.Instance.player.GetComponent<PlayerAim>().marriedNPC = gameObject;
        }

        if (!isMarriage)
        {
            buffType = (AbilityType)Random.Range(0, (int)AbilityType.MaterialDiscount + 1);
            curseType = (AbilityType)Random.Range(0, (int)AbilityType.MaterialDiscount + 1);
            if (buffType == curseType)
            {
                if (buffType == AbilityType.MaterialDiscount)
                    buffType--;
                else
                    buffType++;
            }
        }

        int buff = (int)buffType;
        int curse = (int)curseType;

        GetMultiplierValue(buff, false, isMarriage);
        GetMultiplierValue(curse, true, isMarriage);

        if (!isMarriage)
        {
            buffMultiplier = (float)System.Math.Round(buffMultiplier, 2);
            curseMultiplier = (float)System.Math.Round(curseMultiplier, 2);
        }
    }

    void GetMultiplierValue(int i, bool isCurse, bool isMarriage)
    {
        int value = 1;
        if (isCurse) value = -1;
        switch (i)
        {
            case 0: //walk (*)
                {
                    if (isMarriage)
                    {
                        if (value == 1)
                        {
                            CampManager.Instance.player.GetComponent<PlayerAim>().walkSpeedMultiplier = buffMultiplier;
                            CampManager.Instance.player.GetComponent<PlayerAim>().buffUI.GetComponent<Image>().sprite = UIManager.Instance.perks[i];
                        }
                        if (value == -1)
                        {
                            CampManager.Instance.player.GetComponent<PlayerAim>().walkSpeedMultiplier = curseMultiplier;
                            CampManager.Instance.player.GetComponent<PlayerAim>().curseUI.GetComponent<Image>().sprite = UIManager.Instance.perks[i];
                        }

                        return;
                    }
                    if (value == 1)
                        buffMultiplier = Random.Range(1.1f, 1.6f);
                    if (value == -1)
                        curseMultiplier = Random.Range(0.6f, 0.95f);
                    return;
                }
            case 1: //damage (+)
                {
                    if (isMarriage)
                    {
                        if (value == 1)
                        {
                            CampManager.Instance.player.GetComponent<PlayerAim>().damageMultiplier = (int)buffMultiplier;
                            CampManager.Instance.player.GetComponent<PlayerAim>().buffUI.GetComponent<Image>().sprite = UIManager.Instance.perks[i];
                        }
                        if (value == -1)
                        {
                            CampManager.Instance.player.GetComponent<PlayerAim>().damageMultiplier = (int)curseMultiplier;
                            CampManager.Instance.player.GetComponent<PlayerAim>().curseUI.GetComponent<Image>().sprite = UIManager.Instance.perks[i];
                        }
                        return;
                    }
                    if (value == 1)
                        buffMultiplier = Random.Range(1, 4);
                    if (value == -1)
                        curseMultiplier = Random.Range(-4, -1);
                    return;
                }
            case 2: //hs rate (+)
                {
                    if (isMarriage)
                    {
                        if (value == 1)
                        {
                            CampManager.Instance.player.GetComponent<PlayerAim>().hsRateMultiplier = (int)buffMultiplier;
                            CampManager.Instance.player.GetComponent<PlayerAim>().buffUI.GetComponent<Image>().sprite = UIManager.Instance.perks[i];
                        }

                        if (value == -1)
                        {
                            CampManager.Instance.player.GetComponent<PlayerAim>().hsRateMultiplier = (int)curseMultiplier;
                            CampManager.Instance.player.GetComponent<PlayerAim>().curseUI.GetComponent<Image>().sprite = UIManager.Instance.perks[i];
                        }
                        return;
                    }
                    if (value == 1)
                        buffMultiplier = Random.Range(1, 15);
                    if (value == -1)
                        curseMultiplier = Random.Range(-15, -1);
                    return;
                }
            case 3: //reload speed (+)
                {
                    if (isMarriage)
                    {
                        if (value == 1)
                        {
                            CampManager.Instance.player.GetComponent<PlayerAim>().reloadSpeedMultiplier = (int)buffMultiplier;
                            CampManager.Instance.player.GetComponent<PlayerAim>().buffUI.GetComponent<Image>().sprite = UIManager.Instance.perks[i];
                        }

                        if (value == -1)
                        {
                            CampManager.Instance.player.GetComponent<PlayerAim>().reloadSpeedMultiplier = (int)curseMultiplier;
                            CampManager.Instance.player.GetComponent<PlayerAim>().curseUI.GetComponent<Image>().sprite = UIManager.Instance.perks[i];
                        }
                        return;
                    }
                    if (value == 1)
                        buffMultiplier = Random.Range(-0.3f, -0.1f);
                    if (value == -1)
                        curseMultiplier = Random.Range(0.1f, 0.3f);
                    return;
                }
            case 4: //current spread (*)
                {
                    if (isMarriage)
                    {
                        if (value == 1)
                        {
                            CampManager.Instance.player.GetComponent<PlayerAim>().spreadMultiplier = buffMultiplier;
                            CampManager.Instance.player.GetComponent<PlayerAim>().buffUI.GetComponent<Image>().sprite = UIManager.Instance.perks[i];
                        }
                        if (value == -1)
                        {
                            CampManager.Instance.player.GetComponent<PlayerAim>().spreadMultiplier = curseMultiplier;
                            CampManager.Instance.player.GetComponent<PlayerAim>().curseUI.GetComponent<Image>().sprite = UIManager.Instance.perks[i];
                        }
                        return;
                    }
                    if (value == 1)
                        buffMultiplier = Random.Range(0.5f, 0.9f); 
                    if (value == -1)
                        curseMultiplier = Random.Range(1.1f, 1.5f);
                    return;
                }
            case 5: //experience (*)
                {
                    if (isMarriage)
                    {
                        if (value == 1)
                        {
                            CampManager.Instance.player.GetComponent<PlayerAim>().experienceMultiplier = buffMultiplier;
                            CampManager.Instance.player.GetComponent<PlayerAim>().buffUI.GetComponent<Image>().sprite = UIManager.Instance.perks[i];
                        }
                        if (value == -1)
                        {
                            CampManager.Instance.player.GetComponent<PlayerAim>().experienceMultiplier = curseMultiplier;
                            CampManager.Instance.player.GetComponent<PlayerAim>().curseUI.GetComponent<Image>().sprite = UIManager.Instance.perks[i];
                        }
                        return;
                    }
                    if (value == 1)
                        buffMultiplier = Random.Range(1.1f, 1.75f); 
                    if (value == -1)
                        curseMultiplier = Random.Range(0.6f, 0.95f);
                    return;
                }
            case 6: //max health (*)
                {
                    if (isMarriage)
                    {
                        if (value == 1)
                        {
                            CampManager.Instance.player.GetComponent<PlayerAim>().ChangePlayerMaxHealth(buffMultiplier);
                            CampManager.Instance.player.GetComponent<PlayerAim>().buffUI.GetComponent<Image>().sprite = UIManager.Instance.perks[i];
                        }
                        if (value == -1)
                        {
                            CampManager.Instance.player.GetComponent<PlayerAim>().ChangePlayerMaxHealth(curseMultiplier);
                            CampManager.Instance.player.GetComponent<PlayerAim>().curseUI.GetComponent<Image>().sprite = UIManager.Instance.perks[i];
                        }
                        return;
                    }
                    if (value == 1)
                        buffMultiplier = Random.Range(1.1f, 2f); 
                    if (value == -1)
                        curseMultiplier = Random.Range(0.9f, 0.75f);
                    return;
                }
            case 7:
                {
                    if (value == 1)
                        buffMultiplier = Random.Range(0.1f, 0.5f); //discount (*)
                    if (value == -1)
                        curseMultiplier = Random.Range(1.1f, 1.25f);
                    return;
                }
            default:
                {
                    return;
                }
        }
    }

    public void OpenNPCStatsPanel()
    {
        UIManager.Instance.npcPanel.SetActive(true);
        UIManager.Instance.npcPanelHealthBar.GetComponent<Slider>().value = GetComponentInChildren<HealthSystem>().health;
        UIManager.Instance.npcPanelNameSurname.text = nameSurname;
        UIManager.Instance.npcPanelAge.text = "Age: " + age.ToString();
        if(gender == 0)
            UIManager.Instance.npcPanelGender.text = "Gender: Male";
        if(gender == 1)
            UIManager.Instance.npcPanelGender.text = "Gender: Female";
        if(marriedPersonWith)
            UIManager.Instance.npcPanelMartialStatus.text = "Married: " + marriedPersonWith.name;
        if(!marriedPersonWith)
            UIManager.Instance.npcPanelMartialStatus.text = "Status: Single";
        UIManager.Instance.npcPanelRelationship.text = "Relationship: " + playerRelationship.ToString();
        UIManager.Instance.npcPanelBuffMultiplier.text = buffMultiplier.ToString();
        UIManager.Instance.npcPanelCurseMultiplier.text = curseMultiplier.ToString();
        UIManager.Instance.npcPanelBuffIcon.GetComponent<Image>().sprite = UIManager.Instance.perks[(int)buffType];
        UIManager.Instance.npcPanelCurseIcon.GetComponent<Image>().sprite = UIManager.Instance.perks[(int)curseType];
        if (GetComponentInChildren<NPCBrain>().isSoldier)
            UIManager.Instance.npcPanelRecruitButton.interactable = false;
        if (!GetComponentInChildren<NPCBrain>().isSoldier)
            UIManager.Instance.npcPanelRecruitButton.interactable = true;
        if(marriedPersonWith == CampManager.Instance.player)
            UIManager.Instance.npcPanelFlirtButton.interactable = false;
        if (marriedPersonWith != CampManager.Instance.player)
            UIManager.Instance.npcPanelFlirtButton.interactable = true;

    }
    public void OpenNPCStatsPanel(GameObject whichNPC)
    {
        UIManager.Instance.displayWhichNpc = whichNPC;
        OpenNPCStatsPanel();
    }

    public void ChangeRelationship(int value)
    {
        playerRelationship += value;

        if(feedbackCoroutine != null) StopCoroutine(feedbackCoroutine);
        if (value < 0)
        {
            feedbackCoroutine = StartCoroutine(DisplayRelation(feedbackIcons.transform.GetChild(0).gameObject));
        }
        else
            feedbackCoroutine = StartCoroutine(DisplayRelation(feedbackIcons.transform.GetChild(1).gameObject));
    }

    IEnumerator DisplayRelation(GameObject whichTempDisplayIcon)
    {
        whichTempDisplayIcon.SetActive(true);
        yield return new WaitForSeconds(1f);
        whichTempDisplayIcon.SetActive(false);
    }
}
