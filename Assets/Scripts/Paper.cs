using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
public class Paper : MonoBehaviour
{
    public TextMeshProUGUI actionStory;
    public int[,] resultArray = new int[3, 2]; //0: reject, 1: accept
    [SerializeField] int[] rejectResults = new int[3];
    [SerializeField] int[] acceptResults = new int[3];
    [SerializeField] Color rejectColor;
    [SerializeField] Sprite rejectSprite;
    [SerializeField] int inkStatus; //0: none, 1: approve, 2: declince
    public bool isSingle;
    [SerializeField] GameObject questionInterface;
    [SerializeField] GameObject createSingleInterface;
    [SerializeField] GameObject createCoupleInterface;

    [Header("Single")]
    public TMP_InputField singleName;
    public TMP_InputField singleAge;
    public TMP_Dropdown singleGender;
    public TextMeshProUGUI warnCreateMessage;
    [Header("Couple")]
    public TMP_InputField[] coupleName;
    public TMP_InputField[] coupleAge;
    public TextMeshProUGUI warnCoupleCreateMessage;
    private void Start()
    {
        if(actionStory != null)
        {
            if (isSingle) actionStory.text = "a stranger begs for able to join your camp";
            else actionStory.text = "couple strangers begs for able to join your camp";
        }
        else
            RandomizeValues();

        for (int i = 0; i < 3; i++)
        {
            resultArray[i, 0] = rejectResults[i];
        }
        for (int i = 0; i < 3; i++)
        {
            resultArray[i, 1] = acceptResults[i];
        }
    }

    public void ClickPaper()
    {
        if (DayActionsManager.Instance.isHandEmpty)
        {
            if (inkStatus == 1)
            {
                GetComponent<Animator>().SetTrigger("done");
                GetComponent<UnityEngine.UI.Button>().interactable = false;
                Choose(true);
            }
            else if (inkStatus == 2)
            {
                GetComponent<Animator>().SetTrigger("done");
                GetComponent<UnityEngine.UI.Button>().interactable = false;
                Choose(false);
            }
            return;
        }
        GameObject newStampInk = Instantiate(DayActionsManager.Instance.stampInk, DayActionsManager.Instance.stamp.transform.position, Quaternion.identity);
        StartCoroutine(TraceInk(newStampInk));
    }

    public void ClickNPCPaper()
    {
        if (DayActionsManager.Instance.isHandEmpty)
        {
            if (inkStatus == 1)
            {
                transform.GetChild(0).gameObject.SetActive(false);
                GetComponent<UnityEngine.UI.Button>().interactable = false;
                SoundManager.Instance.PlaySound(CampManager.Instance.player.GetComponent<PlayerAim>().listener.GetComponent<AudioSource>(), SoundManager.Instance.pageSFX);
                if (isSingle)
                {
                    questionInterface.SetActive(false);
                    createSingleInterface.SetActive(true);
                }
                else
                {
                    questionInterface.SetActive(false);
                    createCoupleInterface.SetActive(true);
                }
            }
            else if (inkStatus == 2)
            {
                GetComponent<Animator>().SetTrigger("done");
                GetComponent<UnityEngine.UI.Button>().interactable = false;
                Choose(false);
            }
            return;
        }
        GameObject newStampInk = Instantiate(DayActionsManager.Instance.stampInk, DayActionsManager.Instance.stamp.transform.position, Quaternion.identity);
        StartCoroutine(TraceInk(newStampInk));
    }

    IEnumerator TraceInk(GameObject newStampInk)
    {
        if (!DayActionsManager.Instance.isApprove)
        {
            newStampInk.GetComponent<UnityEngine.UI.Image>().sprite = rejectSprite;
            newStampInk.GetComponent<UnityEngine.UI.Image>().color = rejectColor;
            inkStatus = 2;
        }
        else
            inkStatus = 1;
        newStampInk.transform.position = DayActionsManager.Instance.stamp.transform.position;
        newStampInk.transform.SetParent(transform.GetChild(0));
        yield return new WaitForSeconds(0.45f);
    }
    public void Choose(bool isAccept)
    {
        SoundManager.Instance.PlaySound(CampManager.Instance.player.GetComponent<PlayerAim>().listener.GetComponent<AudioSource>(), SoundManager.Instance.pageSFX);
        DayActionsManager.Instance.paperworkCount.text = (DayActionsManager.Instance.waitingAnswerPaperList.Count - 1).ToString();
        CampManager.Instance.SetMaterial(resultArray[0, Convert.ToInt32(isAccept)]);
        CampManager.Instance.SetPersonality(resultArray[1, Convert.ToInt32(isAccept)]);
        CampManager.Instance.SetFood(resultArray[2, Convert.ToInt32(isAccept)]);
        DayActionsManager.Instance.waitingAnswerPaperList.Remove(gameObject);
        StartCoroutine(DestroyPaper());
    }
    public void DisplayCircleUI()
    {
        if (DayActionsManager.Instance.isHandEmpty) return;

        if(!DayActionsManager.Instance.isApprove)
            DayActionsManager.Instance.DisplayCircleIcons(resultArray[0, 0], resultArray[1, 0], resultArray[2, 0]);
        if (DayActionsManager.Instance.isApprove)
            DayActionsManager.Instance.DisplayCircleIcons(resultArray[0, 1], resultArray[1, 1], resultArray[2, 1]);
    }
    public void ClearCircleUI()
    {
        DayActionsManager.Instance.ClearDisplayCircles();
    }
    public void RandomizeValues()
    {
        for (int i = 0; i < 3; i++)
        {
            rejectResults[i] = UnityEngine.Random.Range(-3, 3);
        }
        for (int i = 0; i < 3; i++)
        {
            acceptResults[i] = UnityEngine.Random.Range(-3, 3);
        }
    }
    public void MouseExitCheck()
    {
        if(inkStatus == 0)
            DayActionsManager.Instance.ClearDisplayCircles();
        if(inkStatus == 1 && !DayActionsManager.Instance.isApprove)
            DayActionsManager.Instance.DisplayCircleIcons(resultArray[0, 1], resultArray[1, 1], resultArray[2, 1]);
        if(inkStatus == 2 && DayActionsManager.Instance.isApprove)
            DayActionsManager.Instance.DisplayCircleIcons(resultArray[0, 0], resultArray[1, 0], resultArray[2, 0]);

    }

    IEnumerator DestroyPaper()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    public void CheckCreate(bool isSingleCreate)
    {
        if (isSingleCreate) CheckNpcCanCreateable();
        if (!isSingleCreate) CheckCoupleCanCreateable();
    }

    public void CheckNpcCanCreateable()
    {
        if (singleName.text.Length == 0)
        {
            warnCreateMessage.text = "Please enter name & surname!";
            return;
        }
        else if (singleAge.text.Length == 0)
        {
            warnCreateMessage.text = "Please enter age!";
            return;
        }
        else if (int.Parse(singleAge.text) < 18)
        {
            warnCreateMessage.text = "Age must be older than 18!";
            return;
        }
        CampManager.Instance.AddNPC(singleName.text, int.Parse(singleAge.text), singleGender.value);
        warnCreateMessage.text = "Done!";
        Choose(true);
    }

    public void CheckCoupleCanCreateable()
    {
        for (int i = 0; i < 2; i++)
        {
            if (coupleName[i].text.Length == 0)
            {
                warnCoupleCreateMessage.text = "Please enter names & surnames!";
                return;
            }
            else if (coupleAge[i].text.Length == 0)
            {
                warnCoupleCreateMessage.text = "Please enter ages!";
                return;
            }
            else if (int.Parse(coupleAge[i].text) < 18)
            {
                warnCoupleCreateMessage.text = "Ages must be older than 18!";
                return;
            }
        }

        CampManager.Instance.AddCouple(coupleName[0].text, int.Parse(coupleAge[0].text), coupleName[1].text, int.Parse(coupleAge[1].text));
        warnCoupleCreateMessage.text = "Done!";
        Choose(true);
    }
}
