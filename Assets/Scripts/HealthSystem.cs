using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth;
    public int health;
    [SerializeField] bool isBarricade;
    Barricade barricadeScript;
    [Header("Shoot Feel")]
    [SerializeField] Color target;
    [SerializeField] Color headshotColor;
    [SerializeField] Transform bloodFx;
    [SerializeField] Transform plane;
    [SerializeField] Animator anim;

    private void Start()
    {
        health = maxHealth;
        if (isBarricade)
        {
            barricadeScript = GetComponentInParent<Barricade>();
        }
        else
            target = GetComponentInChildren<SpriteRenderer>().color;
    }

    public void DragDeadBody(GameObject deadBody, Vector3 dir, float angle)
    {
        deadBody.SetActive(true);
        deadBody.transform.eulerAngles = new Vector3(0, 0, angle - 90);
        deadBody.GetComponent<Rigidbody2D>().AddForce(dir * 200);
    }

    public void Damage(int damageAmount, Vector2 dir, float angle, GameObject attacker)
    {
        health -= damageAmount;
        health = Mathf.Clamp(health, 0, health);
        if (!isBarricade && !CompareTag("Player"))
            Flash(angle, Color.white);

        if (CompareTag("Player"))
        {
            GetComponent<PlayerAim>().SetHealthBar(health);
            GetComponent<PlayerAim>().faceScript.CurrentFaceState = Face.FaceState.Pain;
        }
        if (CompareTag("Npc"))
        {
            GetComponent<NPCBrain>().faceScript.CurrentFaceState = Face.FaceState.Pain;
            if (attacker.CompareTag("Player"))
            {
                transform.parent.GetComponent<NPCStat>().ChangeRelationship(-2);
                if (transform.parent.gameObject == UIManager.Instance.displayWhichNpc)
                {
                    transform.parent.GetComponent<NPCStat>().OpenNPCStatsPanel();
                }
            }
        }

        if (isBarricade)
        {
            barricadeScript.BreakWood(health);
        }

        if (health <= 0) //bug alert: enemy sight collider list
        {
            StartCoroutine(Kill(dir, angle, attacker));
        }
    }
    public void HeadshotFeel(float angle)
    {
        Flash(angle, Color.red);
        anim.SetTrigger("headshot");
    }

    IEnumerator Kill(Vector2 dir, float angle, GameObject attacker)
    {
        yield return new WaitForSeconds(0.001f);
        if (isBarricade)
        {
            SoundManager.Instance.PlaySoundPitchRandomizer(barricadeScript.transform.GetComponent<AudioSource>(), SoundManager.Instance.GiveRandomClip(barricadeScript.breakSFX), 0.15f);
            GetComponent<Collider2D>().enabled = false;
        }
        else
        {
            if (gameObject.CompareTag("Enemy"))
            {
                int dropChance = Random.Range(0, 101);
                if (dropChance >= 90)
                {
                    GameObject gear = Instantiate(CampManager.Instance.gearPref, gameObject.transform.position, Quaternion.identity);
                    gear.transform.parent = PoolManager.Instance.materialPoolTransform;
                }
                DragDeadBody(gameObject.GetComponent<EnemyBrain>().deadBody, dir, angle);
                gameObject.transform.parent = null;
                WaveManager.Instance.CheckWaveCompleted();
                LevelingManager.Instance.GainExp(gameObject.GetComponent<EnemyBrain>().expAmount);
                gameObject.GetComponent<EnemyBrain>().listener.parent = PoolManager.Instance.corpsePoolTransform;
                SoundManager.Instance.PlaySoundPitchRandomizer(gameObject.GetComponent<EnemyBrain>().listener.GetComponent<AudioSource>(), SoundManager.Instance.GiveRandomClip(SoundManager.Instance.deathSFX), 0.15f);
                SoundManager.Instance.PlaySoundPitchRandomizer(gameObject.GetComponent<EnemyBrain>().listener.GetComponent<AudioSource>(), SoundManager.Instance.GiveRandomClip(SoundManager.Instance.bodyFallSFX), 0.15f, 0.15f);
                WaveManager.Instance.zombieCount--;
                WaveManager.Instance.zombieCountText.text = "Zombie Count: " + WaveManager.Instance.zombieCount.ToString();
                Destroy(gameObject);
            }
            if (gameObject.CompareTag("Npc"))
            {
                if (attacker.CompareTag("Player"))
                {
                    CampManager.Instance.SomebodyExecuted();
                    if (gameObject.GetComponentInParent<NPCStat>().marriedPersonWith != null)
                    {
                        if (gameObject.GetComponentInParent<NPCStat>().marriedPersonWith != attacker)
                        {
                            gameObject.GetComponentInParent<NPCStat>().marriedPersonWith.GetComponentInParent<NPCStat>().ChangeRelationship(-100);
                            gameObject.GetComponentInParent<NPCStat>().marriedPersonWith.GetComponentInChildren<NPCBrain>().DriveCrazy();
                            gameObject.GetComponentInParent<NPCStat>().marriedPersonWith.GetComponent<NPCStat>().marriedPersonWith = null;
                        }
                    }
                }
                if (transform.parent.gameObject == CampManager.Instance.player.GetComponent<PlayerAim>().marriedNPC)
                    CampManager.Instance.player.GetComponent<PlayerAim>().RefreshMultipliers();
                gameObject.GetComponent<NPCBrain>().listener.parent = PoolManager.Instance.corpsePoolTransform;
                DragDeadBody(gameObject.GetComponent<NPCBrain>().deadBody, dir, angle);
                SoundManager.Instance.PlaySoundPitchRandomizer(gameObject.GetComponent<NPCBrain>().listener.GetComponent<AudioSource>(), SoundManager.Instance.GiveRandomClip(SoundManager.Instance.deathSFX), 0.15f);
                SoundManager.Instance.PlaySoundPitchRandomizer(gameObject.GetComponent<NPCBrain>().listener.GetComponent<AudioSource>(), SoundManager.Instance.GiveRandomClip(SoundManager.Instance.bodyFallSFX), 0.15f, 0.15f);
                if (UIManager.Instance.displayWhichNpc == gameObject.transform.parent.gameObject)
                    UIManager.Instance.CloseNPCStatsPanel();
                Destroy(transform.parent.gameObject);
            }
            if (gameObject.CompareTag("Player"))
            {
                //Destroy(gameObject);
                GameStateManager.Instance.CurrentGameState = GameStateManager.GameState.DayActions;
                UIManager.Instance.DoTransition(true);
                CampManager.Instance.died.SetActive(true);
                CampManager.Instance.isLose = true;
            }
        }
    }

    public void SetMaxHealth(int value)
    {
        maxHealth = value;
    }
    public void Heal(int value)
    {
        health += value;
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    void Update()
    {
        if(!isBarricade)
            GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(GetComponentInChildren<SpriteRenderer>().color, target, Time.deltaTime * 15);
    }

    void Flash(float angle, Color flashColor)
    {
        ParticleSystem bloodFxClone = Instantiate(bloodFx, transform.position + new Vector3(0,0.5f), Quaternion.identity).GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
        bloodFxClone.transform.parent.parent.eulerAngles = new Vector3(0, 0, angle - 180);
        bloodFxClone.collision.SetPlane(0, plane);
        bloodFxClone.Play();
        bloodFxClone.transform.parent.parent.SetParent(PoolManager.Instance.bloodPoolTransform);
        GetComponentInChildren<SpriteRenderer>().color = flashColor;
    }
}