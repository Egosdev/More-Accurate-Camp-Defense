using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barricade : MonoBehaviour
{
    [SerializeField] HealthSystem barricadeHealthScript;
    [SerializeField] int barricadeMaxHealth;
    [SerializeField] GameObject barricadeParts;
    [SerializeField] bool isCloseEnoughForRepair;
    [SerializeField] AudioClip repairSFX, repairPartSFX;
    public AudioClip[] breakSFX, breakPartSFX;

    [Header("Don't Touch")]
    [SerializeField] int barricadePartCount;
    [SerializeField] int eachPartHealth;
    [SerializeField] List<Color> woodColors;
    [SerializeField] float repairTimer;

    private void Start()
    {
        foreach (Transform wood in barricadeParts.transform)
        {
            woodColors.Add(wood.GetComponent<SpriteRenderer>().color);
        }

        barricadePartCount = barricadeParts.transform.childCount;

        if (barricadeMaxHealth % barricadePartCount != 0)
        {
            barricadeMaxHealth -= barricadeMaxHealth % barricadePartCount;
        }

        barricadeHealthScript.health = barricadeMaxHealth;
        eachPartHealth = barricadeMaxHealth / barricadePartCount;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (barricadeHealthScript.health != barricadeMaxHealth)
            {
                isCloseEnoughForRepair = true;
                collision.GetComponent<PlayerAim>().repairIcon.SetActive(true);
                collision.GetComponent<PlayerAim>().interactIcon.SetActive(true);
            }
            else
            {
                isCloseEnoughForRepair = false;
                collision.GetComponent<PlayerAim>().repairIcon.SetActive(false);
                collision.GetComponent<PlayerAim>().interactIcon.SetActive(false);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isCloseEnoughForRepair = false;
            collision.GetComponent<PlayerAim>().repairIcon.SetActive(false);
            collision.GetComponent<PlayerAim>().interactIcon.SetActive(false);
        }
    }
    // collider goes false at HealthSystem script
    public void BreakWood(int currentHealth)
    {
        for (int i = (barricadePartCount - 1); i >= 0; i--)
        {
            if (eachPartHealth * i >= currentHealth && eachPartHealth * (i - 1) < currentHealth)
            {
                if (i != 0 && !barricadeParts.transform.GetChild(i).GetComponent<WoodPart>().isBroken)
                {
                    SoundManager.Instance.PlaySoundPitchRandomizer(GetComponent<AudioSource>(), SoundManager.Instance.GiveRandomClip(breakPartSFX), 0.15f);
                }
                for (int j = barricadePartCount - 1; j >= i; j--)
                {
                    barricadeParts.transform.GetChild(j).GetComponent<WoodPart>().isBroken = true;
                    barricadeParts.transform.GetChild(j).gameObject.SetActive(false);
                }
            }
            if(eachPartHealth * (i+1) > currentHealth && eachPartHealth * i <= currentHealth)
            {
                StartCoroutine(FlashPart(i));
            }
        }
    }
    public void RepairWood()
    {
        repairTimer = 2;
        for (int i = 0; i <= (barricadePartCount - 1); i++)
        {
            if (!barricadeParts.transform.GetChild(i).gameObject.activeSelf)
            {
                barricadeParts.transform.GetChild(i).gameObject.SetActive(true);
                barricadeParts.transform.GetChild(i).GetComponent<WoodPart>().isBroken = false;

                if (barricadeHealthScript.health == 0)
                {
                    transform.GetChild(0).GetComponent<Collider2D>().enabled = true;
                }

                barricadeHealthScript.health = eachPartHealth * (i + 1);
                if (barricadeHealthScript.health == barricadeMaxHealth)
                {
                    break;
                }
                SoundManager.Instance.PlaySoundPitchRandomizer(GetComponent<AudioSource>(), repairPartSFX, 0.15f);
                return;
            }
        }

        foreach (Transform wood in barricadeParts.transform)
        {
            wood.GetComponent<WoodPart>().isBroken = false;
        }
        barricadeHealthScript.health = barricadeMaxHealth;
        SoundManager.Instance.PlaySoundPitchRandomizer(GetComponent<AudioSource>(), repairSFX, 0.15f);
    }

    private void Update()
    {
        if (repairTimer >= 0)
        {
            repairTimer -= Time.deltaTime;
            repairTimer = Mathf.Clamp(repairTimer, 0, repairTimer);
        }
        if (Input.GetKeyDown(KeyCode.E) && isCloseEnoughForRepair && repairTimer == 0)
        {
            RepairWood();
            CampManager.Instance.SetMaterial(-1);
        }
    }

    IEnumerator FlashPart(int i)
    {
        //Debug.Log($"flash {i}");
        barricadeParts.transform.GetChild(i).GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(0.05f);
        barricadeParts.transform.GetChild(i).GetComponent<SpriteRenderer>().color = woodColors[i];
    }
}
