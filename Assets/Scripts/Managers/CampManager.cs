using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class CampManager : MonoBehaviour
{
    #region Singleton
    public static CampManager Instance;

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

    public List<GameObject> people;
    public GameObject campCenter;
    public GameObject player;
    public GameObject npcPref;
    public GameObject gearPref;
    public Transform spawnPoints;
    public Transform leaveCampPoint;
    public int materials;
    public int food;
    public int captainPersonality;
    public int giftCount;
    public int pillsCount;
    public Sprite[] statusIcons; //0 elope, 1 rebel
    public int reputation;
    public TMPro.TextMeshProUGUI shortAmmoText;
    public TMPro.TextMeshProUGUI longAmmoText;
    public TMPro.TextMeshProUGUI shellsAmmoText;

    public TMPro.TextMeshProUGUI giftText;
    public TMPro.TextMeshProUGUI pillsText;
    public TMPro.TextMeshProUGUI reputationText;
    public GameObject lose;
    public GameObject died;
    public bool isLose;

    private void Start()
    {
        materials = 50;
        food = 50;
        captainPersonality = 50;

        AddNPC("Ege Can", 21, 0);
    }

    public void AddNPC(string nameSurname, int age, int gender)
    {
        NPCSettings(CreatePeoplePref(), nameSurname, age, gender);
    }

    void NPCSettings(GameObject newComer, string nameSurname, int age, int gender)
    {
        newComer.GetComponent<NPCStat>().nameSurname = nameSurname;
        newComer.GetComponent<NPCStat>().age = age;
        newComer.GetComponent<NPCStat>().gender = gender;
        newComer.GetComponent<NPCStat>().playerRelationship = 60;
        newComer.name = nameSurname;
        newComer.transform.parent = PoolManager.Instance.NPCPoolTransform;

        people.Add(newComer);
        newComer.GetComponentInChildren<NPCBrain>().MoveToCoords(SelectRandomSpawnPoint());
    }
    GameObject CreatePeoplePref()
    {
        GameObject newComer = Instantiate(npcPref, campCenter.transform.position, Quaternion.identity);
        return newComer;
    }
    public void AddCouple(string nameSurnameMale, int ageMale, string nameSurnameFemale, int ageFemale)
    {
        GameObject Husband = CreatePeoplePref();
        GameObject Wife = CreatePeoplePref();

        NPCSettings(Husband, nameSurnameMale, ageMale, 0);
        NPCSettings(Wife, nameSurnameFemale, ageFemale, 1);
        Wife.GetComponent<NPCStat>().marriedPersonWith = Husband;
        Husband.GetComponent<NPCStat>().marriedPersonWith = Wife;
    }

    public void RemoveNPC(GameObject npc)
    {
        Destroy(npc);
    }
    public Transform SelectRandomSpawnPoint()
    {
        int randomIndex = Random.Range(0, spawnPoints.childCount);
        return spawnPoints.GetChild(randomIndex);
    }
    public void SomebodyExecuted()
    {
        int rebelChance = Random.Range(0, 101);
        SetPersonality(-1);

        for (int i = 0; i < PoolManager.Instance.NPCPoolTransform.childCount; i++)
        {
            if (PoolManager.Instance.NPCPoolTransform.GetChild(i).GetComponent<NPCStat>().playerRelationship > 75)
                PoolManager.Instance.NPCPoolTransform.GetChild(i).GetComponent<NPCStat>().ChangeRelationship(-5);
            if (PoolManager.Instance.NPCPoolTransform.GetChild(i).GetComponent<NPCStat>().playerRelationship > 50 && PoolManager.Instance.NPCPoolTransform.GetChild(i).GetComponent<NPCStat>().playerRelationship <= 75)
                PoolManager.Instance.NPCPoolTransform.GetChild(i).GetComponent<NPCStat>().ChangeRelationship(-7);
            if (PoolManager.Instance.NPCPoolTransform.GetChild(i).GetComponent<NPCStat>().playerRelationship > 0 && PoolManager.Instance.NPCPoolTransform.GetChild(i).GetComponent<NPCStat>().playerRelationship <= 50)
                PoolManager.Instance.NPCPoolTransform.GetChild(i).GetComponent<NPCStat>().ChangeRelationship(-10);
            if (PoolManager.Instance.NPCPoolTransform.GetChild(i).GetComponent<NPCStat>().playerRelationship <= 0)
                PoolManager.Instance.NPCPoolTransform.GetChild(i).GetComponent<NPCStat>().ChangeRelationship(-15);
        }
        if (rebelChance > 85)
            Rebellion();
    }

    public void SetMaterial(int value)
    {
        materials += value * 5;
        UIManager.Instance.materialSlider.value = materials;
        LoseCondition(materials);
    }
    public void SetPersonality(int value)
    {
        captainPersonality += value * 5;
        UIManager.Instance.personalitySlider.value = captainPersonality;
        LoseCondition(captainPersonality);
    }
    public void SetFood(int value)
    {
        food += value * 5;
        UIManager.Instance.foodSlider.value = food;
        LoseCondition(food);
    }
    public void LoseCondition(int stat)
    {
        if (stat <= 0 || stat >= 100)
        {
            UIManager.Instance.DoTransition(true);
            GameStateManager.Instance.CurrentGameState = GameStateManager.GameState.DayActions;
            lose.SetActive(true);
            isLose = true;
        }

    }
    public void Rebellion()
    {
        Debug.Log("rebellion");
        foreach (Transform npc in PoolManager.Instance.NPCPoolTransform)
        {
            if (npc.GetComponent<NPCStat>().playerRelationship < 0)
                npc.GetComponentInChildren<NPCBrain>().MakeRebel();
        }
    }
    public void Craft(int value)
    {
        SoundManager.Instance.PlaySound(player.GetComponent<PlayerAim>().listener.GetComponent<AudioSource>(), SoundManager.Instance.craftingSFX);
        if (value == 0)
        {
            if (materials > 0)
            {
                player.GetComponent<PlayerAim>().shortBulletAmmo += 30;
                SetMaterial(-1);
            }
        }
        else if (value == 1)
        {
            if (materials > 0)
            {
                player.GetComponent<PlayerAim>().longBulletAmmo += 30;
                SetMaterial(-1);
            }
        }
        else if (value == 2)
        {
            if (materials > 0)
            {
                player.GetComponent<PlayerAim>().redShellAmmo += 30;
                SetMaterial(-1);
            }
        }
        else if (value == 3)
        {
            if (materials > 0)
            {
                giftCount++;
                giftText.text = "Gift: " + giftCount.ToString();
                SetMaterial(-1);
            }
        }
        else if (value == 4)
        {
            if (food > 0)
            {
                pillsCount++;
                pillsText.text = "Medical Pills: " + pillsCount.ToString();
                SetFood(-1);
            }
        }
        DisplayAmmos();
    }

    public void DisplayAmmos()
    {
        shortAmmoText.text = "9mm: " + player.GetComponent<PlayerAim>().shortBulletAmmo.ToString();
        longAmmoText.text = "7.62mm: " + player.GetComponent<PlayerAim>().longBulletAmmo.ToString();
        shellsAmmoText.text = "Shells: " + player.GetComponent<PlayerAim>().redShellAmmo.ToString();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            if(isLose)
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }
}
