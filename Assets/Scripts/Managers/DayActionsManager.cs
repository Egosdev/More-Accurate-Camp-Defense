using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DayActionsManager : MonoBehaviour
{
    #region Singleton
    public static DayActionsManager Instance;

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

    public List<GameObject> waitingAnswerPaperList;
    public RectTransform stamp;
    public GameObject stampInk;
    public Animator[] stampAnim;
    public GameObject[] colorStam;
    public bool isApprove = true;
    public GameObject materialCircles;
    public GameObject personalityCircles;
    public GameObject foodCircles;
    public Color redCircle;
    public Color greenCircle;
    public bool isHandEmpty = true;
    public GameObject paperPref;
    public GameObject npcCreatePref;
    public GameObject paperPile;
    public GameObject pagePool;
    public TMPro.TextMeshProUGUI paperworkCount;
    public GameObject backToCampWarn;
    public Canvas gameplayCanvas;
    public Canvas dayActionsCanvas;
    public GameObject feedbackDone;

    private void Update()
    {
        if (GameStateManager.Instance.CurrentGameState != GameStateManager.GameState.DayActions) return;

        Vector2 movePos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            UIManager.Instance.DayActionCanvas.transform as RectTransform,
            Input.mousePosition, UIManager.Instance.DayActionCanvas.worldCamera,
            out movePos);

        stamp.position = UIManager.Instance.DayActionCanvas.transform.TransformPoint(movePos);

        if (Input.GetMouseButtonDown(0))
        {
            if (isHandEmpty) return;
            SoundManager.Instance.PlaySound(CampManager.Instance.player.GetComponent<PlayerAim>().listener.GetComponent<AudioSource>(), SoundManager.Instance.stampDownSFX);
            if (isApprove)
                stampAnim[0].SetBool("isMouseDown", true);
            else
                stampAnim[1].SetBool("isMouseDown", true);
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (isHandEmpty) return;
            SoundManager.Instance.PlaySound(CampManager.Instance.player.GetComponent<PlayerAim>().listener.GetComponent<AudioSource>(), SoundManager.Instance.stampUpSFX);
            if (isApprove)
                stampAnim[0].SetBool("isMouseDown", false);
            else
                stampAnim[1].SetBool("isMouseDown", false);
        }
    }

    public void DisplayCircleIcons(int material, int personality, int food) //circle color bug???
    {
        ClearDisplayCircles();
        if (material != 0)
        {
            for (int i = 1; i <= Mathf.Abs(material); i++)
            {
                materialCircles.transform.GetChild(i - 1).gameObject.SetActive(true);
                if (material > 0)
                    materialCircles.transform.GetChild(i - 1).GetComponent<Image>().color = greenCircle;
                if (material < 0)
                    materialCircles.transform.GetChild(i - 1).GetComponent<Image>().color = redCircle;
            }
        }
        if (personality != 0)
        {
            for (int i = 1; i <= Mathf.Abs(personality); i++)
            {
                personalityCircles.transform.GetChild(i - 1).gameObject.SetActive(true);
                if (personality > 0)
                    personalityCircles.transform.GetChild(i - 1).GetComponent<Image>().color = greenCircle;
                if (personality < 0)
                    personalityCircles.transform.GetChild(i - 1).GetComponent<Image>().color = redCircle;
            }
        }
        if (food != 0)
        {
            for (int i = 1; i <= Mathf.Abs(food); i++)
            {
                foodCircles.transform.GetChild(i - 1).gameObject.SetActive(true);
                if (food > 0)
                    foodCircles.transform.GetChild(i - 1).GetComponent<Image>().color = greenCircle;
                if (food < 0)
                    foodCircles.transform.GetChild(i - 1).GetComponent<Image>().color = redCircle;
            }
        }
    }

    public void ClearDisplayCircles()
    {
        foreach (Transform circle in materialCircles.transform)
        {
            circle.gameObject.SetActive(false);
        }
        foreach (Transform circle in personalityCircles.transform)
        {
            circle.gameObject.SetActive(false);
        }
        foreach (Transform circle in foodCircles.transform)
        {
            circle.gameObject.SetActive(false);
        }
    }

    public void ChangeStamp(bool isClickedGreen)
    {
        isHandEmpty = false;
        stampAnim[0].Rebind();
        stampAnim[1].Rebind();

        if (!isClickedGreen)
        {
            colorStam[0].SetActive(false);
            colorStam[1].SetActive(true);
            isApprove = false;
        }
        else if (isClickedGreen)
        {
            colorStam[1].SetActive(false);
            colorStam[0].SetActive(true);
            isApprove = true;
        }
    }

    public void PutItBack()
    {
        isApprove = true;
        isHandEmpty = true;
    }

    public void TakePaper()
    {
        if (waitingAnswerPaperList.Count != 0)
        {
            waitingAnswerPaperList[0].SetActive(true);
            SoundManager.Instance.PlaySound(CampManager.Instance.player.GetComponent<PlayerAim>().listener.GetComponent<AudioSource>(), SoundManager.Instance.pageSFX);
        }
        else
        {
            feedbackDone.SetActive(true);
        }
    }

    public void GoToDayActions()
    {

        GameStateManager.Instance.CurrentGameState = GameStateManager.GameState.DayActions;
        StartCoroutine(CreatePaperworks());
        StartCoroutine(ChangeCanvas(true));
    }

    public void BackToCamp()
    {
        if (waitingAnswerPaperList.Count == 0)
        {
            GameStateManager.Instance.CurrentGameState = GameStateManager.GameState.FreeRoam;
            WaveManager.Instance.waveText.text = "Press 'F' To Next Wave";
            StartCoroutine(ChangeCanvas(false));
            SoundManager.Instance.PlaySound(CampManager.Instance.player.GetComponent<PlayerAim>().listener.GetComponent<AudioSource>(), SoundManager.Instance.dayTimeSFX);
            SoundManager.Instance.PlaySingleSoundAtOnce(CampManager.Instance.player.GetComponent<PlayerAim>().ambience.GetComponent<AudioSource>(), SoundManager.Instance.ambience[0]);
        }
        else
        {
            StartCoroutine(DisplayWarnMessage());
        }
    }
    IEnumerator DisplayWarnMessage()
    {
        backToCampWarn.SetActive(true);
        yield return new WaitForSeconds(0.75f);
        backToCampWarn.SetActive(false);
    }

    IEnumerator CreatePaperworks()
    {
        yield return new WaitForSeconds(0.2f);
        int randomNumber = Random.Range(2, 5);
        for (int i = 0; i < randomNumber; i++)
        {
            GameObject newPaper = Instantiate(paperPref, paperPile.transform.position, Quaternion.identity);
            newPaper.transform.SetParent(pagePool.transform);
            newPaper.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            waitingAnswerPaperList.Add(newPaper);
        }
        int randomChance = Random.Range(0, 101);
        if (randomChance > 50)
        {
            paperworkCount.text = (randomNumber + 1).ToString();
            GameObject newRequest = Instantiate(npcCreatePref, paperPile.transform.position, Quaternion.identity);
            newRequest.transform.SetParent(pagePool.transform);
            newRequest.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            waitingAnswerPaperList.Add(newRequest);

            if (randomChance > 75)
                newRequest.GetComponent<Paper>().isSingle = false;
            else
                newRequest.GetComponent<Paper>().isSingle = true;
        }
        else
            paperworkCount.text = randomNumber.ToString();
    }

    IEnumerator ChangeCanvas(bool isDayActionState)
    {
        UIManager.Instance.DoTransition(true);
        yield return new WaitForSeconds(1f);
        dayActionsCanvas.gameObject.SetActive(isDayActionState);
        gameplayCanvas.gameObject.SetActive(!isDayActionState);
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.DoTransition(false);

    }
}
