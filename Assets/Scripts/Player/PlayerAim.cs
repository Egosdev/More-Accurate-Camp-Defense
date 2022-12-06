using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class PlayerAim : MonoBehaviour
{
    [SerializeField] Transform aimTransform;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Animator aimAnim;
    Vector3 mouseWorldPosition;
    Vector3 bulletTraceVector;
    [SerializeField] float fireCooldownTimer;
    [SerializeField] GameObject shell;
    [SerializeField] LayerMask shootable;
    public GameObject handSocket;
    [SerializeField] Gun gunStatScript;
    [SerializeField] int currentGunId;
    [SerializeField] GameObject interactableObject;
    [SerializeField] Transform fireSound;
    public Transform listener;
    public Transform ambience;

    public float currentSpread;
    bool isReloading;
    int headShotChance;
    float pillTimer;
    [Header("Cursor & Aim Cam")]
    [SerializeField] GameObject cursor;
    [SerializeField] GameObject ghostCursor;
    [SerializeField] float aimCamOffset;
    [SerializeField] float aimCamMaxRadius;
    public float aimCrossTimer;
    public float waitForSecondsBackToBestAim;
    [SerializeField] LayerMask canSelectable;
    public GameObject marriedNPC;
    [SerializeField] GameObject tempRaycastObject;
    [SerializeField] Slider healthBar;
    [SerializeField] GameObject tooltip;
    [SerializeField] Transform flashlightTransform;

    [Header("Ammo UI")]
    [SerializeField] TextMeshProUGUI clipText;
    [SerializeField] TextMeshProUGUI AmmoText;
    [Header("Ammo Types")]
    public int shortBulletAmmo;
    public int longBulletAmmo;
    public int redShellAmmo;
    [Header("Ability Multipliers")]
    public float walkSpeedMultiplier = 1;
    public int damageMultiplier = 0;
    public int hsRateMultiplier = 0;
    public int reloadSpeedMultiplier = 0;
    public float spreadMultiplier = 1;
    public float experienceMultiplier = 1;
    public float maxHealthMultiplier = 1;
    public GameObject buffUI;
    public GameObject curseUI;
    //public int discountMultiplier = 1;

    [Header("User Feedback")]
    public GameObject repairIcon;
    public GameObject interactIcon;

    // Must Delete: Drawray
    private void Start()
    {
        gunStatScript = handSocket.GetComponent<Gun>();
    }

    private void Update()
    {
        if (GameStateManager.Instance.CurrentGameState == GameStateManager.GameState.DayActions) return;
        Aiming(CalculateMousePos());
        Fire();
        UpdateUITexts();
        Selectable();
        TakePills();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentGunId++;
            if (currentGunId == aimTransform.childCount) currentGunId = 0;
            SwapWeapon(currentGunId);
        }

        if (fireCooldownTimer > 0)
        {
            fireCooldownTimer -= Time.deltaTime;
        }

        if (lineRenderer.enabled)
        {
            var step = Time.deltaTime * 80;

            bulletTraceVector = Vector3.MoveTowards(bulletTraceVector, lineRenderer.GetPosition(1), step);
            lineRenderer.SetPosition(0, bulletTraceVector);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reloading());
        }
    }

    private void UpdateUITexts()
    {
        clipText.text = gunStatScript.currentClip.ToString();
        if (gunStatScript.ammoType == Gun.AmmoType.shortBullet)
            AmmoText.text = $"/{shortBulletAmmo}";
        else if (gunStatScript.ammoType == Gun.AmmoType.longBullet)
            AmmoText.text = $"/{longBulletAmmo}";
        else if (gunStatScript.ammoType == Gun.AmmoType.redShell)
            AmmoText.text = $"/{redShellAmmo}";
        else if (gunStatScript.ammoType == Gun.AmmoType.infinite)
            AmmoText.text = "/Inf";
    }

    private void Aiming(Vector3 calculatedMousePos)
    {
        Vector3 aimDir = (calculatedMousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, angle);
        flashlightTransform.eulerAngles = new Vector3(0, 0, angle - 90);

        Vector3 localScale = Vector3.one;
        if (angle > 90 || angle < -90)
        {
            localScale.y = -1f;
        }
        else
        {
            localScale.y = +1f;
        }
        aimTransform.localScale = localScale;
    }
    private Vector3 CalculateMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        cursor.transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, 0);

        ghostCursor.transform.position = mouseWorldPosition;
        if (Vector3.Distance(cursor.transform.position, transform.position) <= aimCamOffset)
        {
            ghostCursor.transform.position = transform.position;
        }
        ghostCursor.transform.position = Vector3.ClampMagnitude(ghostCursor.transform.position - transform.position, aimCamMaxRadius) + transform.position;
        ghostCursor.transform.position = new Vector3(ghostCursor.transform.position.x, ghostCursor.transform.position.y, 0);
        return mouseWorldPosition;
    }
    private void Fire()
    {
        if (fireCooldownTimer > 0) return;
        if (isReloading) return;
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (EventSystem.current.currentSelectedGameObject != null && !EventSystem.current.currentSelectedGameObject.CompareTag("UI"))
            {
                if (Input.GetMouseButtonDown(0))
                    SoundManager.Instance.PlaySound(CampManager.Instance.player.GetComponent<PlayerAim>().listener.GetComponent<AudioSource>(), SoundManager.Instance.UIClickSFX);
                return;
            }
        }
        if (Input.GetMouseButtonDown(0) && gunStatScript.currentClip == 0)
        {
            SoundManager.Instance.PlaySoundPitchRandomizer(listener.GetComponent<AudioSource>(), SoundManager.Instance.gunEmptySFX, 0.05f);
            return;
        }

        if (Input.GetMouseButton(0) && gunStatScript.currentClip > 0)
        {
            fireCooldownTimer = gunStatScript.fireRate;
            Vector2 direction = (CalculateMousePos() - transform.position);

            direction = direction.normalized;

            direction.x += UnityEngine.Random.Range(-currentSpread, currentSpread);
            direction.y += UnityEngine.Random.Range(-currentSpread, currentSpread);

            direction = direction.normalized;

            Debug.DrawRay(gunStatScript.gunEndPointTransform.position, direction * gunStatScript.range, Color.green, 5);

            RaycastHit2D hit = Physics2D.Raycast(gunStatScript.gunEndPointTransform.position, direction, gunStatScript.range, shootable);
            if (hit.collider != null)
            {
                Debug.Log(hit.transform.name + " was hit");
                if (hit.collider.CompareTag("Enemy"))
                {
                    if (IsHeadShot())
                    {
                        hit.collider.gameObject.GetComponent<HealthSystem>().Damage(damageMultiplier + gunStatScript.damage * 2, direction, aimAnim.transform.eulerAngles.z, gameObject);
                        lineRenderer.SetPosition(1, hit.transform.position - gunStatScript.gunEndPointTransform.position + new Vector3(0, 0.85f, 0));
                        SoundManager.Instance.PlaySoundPitchRandomizer(hit.transform.GetComponent<EnemyBrain>().listener.GetComponent<AudioSource>(), SoundManager.Instance.GiveRandomClip(SoundManager.Instance.headShotSFX), 0.15f, 0.15f);
                    }
                    else
                    {
                        hit.collider.gameObject.GetComponent<HealthSystem>().Damage(damageMultiplier + gunStatScript.damage, direction, aimAnim.transform.eulerAngles.z, gameObject);
                        lineRenderer.SetPosition(1, direction * hit.distance);
                        SoundManager.Instance.PlaySoundPitchRandomizer(hit.transform.GetComponent<EnemyBrain>().listener.GetComponent<AudioSource>(), SoundManager.Instance.GiveRandomClip(SoundManager.Instance.bloodSFX), 0.15f, 0.25f);
                    }
                }
                else if (hit.collider.CompareTag("Npc"))
                {
                    hit.collider.gameObject.GetComponent<HealthSystem>().Damage(damageMultiplier + gunStatScript.damage, direction, aimAnim.transform.eulerAngles.z, gameObject);
                    lineRenderer.SetPosition(1, direction * hit.distance);
                }
                else
                    lineRenderer.SetPosition(1, direction * hit.distance);
            }
            else
            {
                lineRenderer.SetPosition(1, direction * gunStatScript.range);
            }
            lineRenderer.transform.position = gunStatScript.gunEndPointTransform.position;
            aimAnim.SetTrigger(gunStatScript.gunName);
            EjectShell();
            gunStatScript.currentClip--;
            StartCoroutine(BulletTrail());
            aimCrossTimer = waitForSecondsBackToBestAim;
            fireSound.position = gunStatScript.gunEndPointTransform.position;
            //fireSound.GetComponent<AudioSource>().clip = gunStatScript.fireSoundClip;
            SoundManager.Instance.PlaySoundPitchRandomizer(fireSound.GetComponent<AudioSource>(), gunStatScript.fireSoundClip, 0.15f);
        }
    }
    private void EjectShell()
    {
        GameObject ejectedShell = Instantiate(shell, aimTransform.GetChild(0).transform.position, Quaternion.identity);
        float xVnot = UnityEngine.Random.Range(-6.5f, -9.5f);
        float yVnot = UnityEngine.Random.Range(6.5f, 9.5f);
        if (aimTransform.rotation.eulerAngles.z > 90 && aimTransform.rotation.eulerAngles.z < 270)
        {
            xVnot *= -1;
            if (Input.GetAxisRaw("Horizontal") == -1)
            {
                xVnot += 1f;
            }
            if (Input.GetAxisRaw("Horizontal") == +1)
            {
                xVnot += 8f;
            }
        }
        else
        {
            if (Input.GetAxisRaw("Horizontal") == -1)
            {
                xVnot -= 8f;
            }
            if (Input.GetAxisRaw("Horizontal") == +1)
            {
                xVnot -= 1f;
            }
        }
        ejectedShell.GetComponent<BulletShell>().xVnot = xVnot;
        ejectedShell.GetComponent<BulletShell>().yVnot = yVnot;
    }
    IEnumerator BulletTrail()
    {
        lineRenderer.SetPosition(0, Vector3.zero);
        bulletTraceVector = Vector3.zero;
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.2f);
        lineRenderer.enabled = false;

        lineRenderer.SetPosition(1, new Vector3(5, 0, 0));
    }
    IEnumerator Reloading()
    {
        if (CanAmmoRefill() == false) yield break;

        SoundManager.Instance.PlaySingleSoundAtOnce(listener.GetComponent<AudioSource>(), gunStatScript.reloadSoundClip);
        isReloading = true;
        aimAnim.SetTrigger("Reload");
        yield return new WaitForSeconds(gunStatScript.reloadTime + reloadSpeedMultiplier);
        if (isReloading)
            CalculateAmmo();
        isReloading = false;
    }

    private void SwapWeapon(int Id)
    {
        foreach (Transform gun in aimTransform)
        {
            gun.gameObject.SetActive(false);
        }
        handSocket = aimTransform.GetChild(Id).gameObject;
        gunStatScript = handSocket.GetComponent<Gun>();
        handSocket.SetActive(true);
        isReloading = false;
        SoundManager.Instance.PlaySingleSoundAtOnce(listener.GetComponent<AudioSource>(), gunStatScript.swapSoundClip);
    }

    private bool CanAmmoRefill()
    {
        if (isReloading) return false;
        if (gunStatScript.currentClip == gunStatScript.clipCapacity) return false;

        if (gunStatScript.ammoType == Gun.AmmoType.shortBullet)
        {
            if (shortBulletAmmo > 0)
                return true;
        }
        else if (gunStatScript.ammoType == Gun.AmmoType.longBullet)
        {
            if (longBulletAmmo > 0)
                return true;
        }
        else if (gunStatScript.ammoType == Gun.AmmoType.redShell)
        {
            if (redShellAmmo > 0)
                return true;
        }
        else if (gunStatScript.ammoType == Gun.AmmoType.infinite)
        {
            return true;
        }
        return false;
    }
    private void CalculateAmmo()
    {
        int diff = gunStatScript.clipCapacity - gunStatScript.currentClip;
        if (gunStatScript.ammoType == Gun.AmmoType.shortBullet)
        {
            if (shortBulletAmmo >= diff)
            {
                gunStatScript.currentClip += diff;
                shortBulletAmmo -= diff;
            }
            else
            {
                gunStatScript.currentClip += shortBulletAmmo;
                shortBulletAmmo -= shortBulletAmmo;
            }
        }
        if (gunStatScript.ammoType == Gun.AmmoType.longBullet)
        {
            if (longBulletAmmo >= diff)
            {
                gunStatScript.currentClip += diff;
                longBulletAmmo -= diff;
            }
            else
            {
                gunStatScript.currentClip += longBulletAmmo;
                longBulletAmmo -= longBulletAmmo;
            }
        }
        if (gunStatScript.ammoType == Gun.AmmoType.redShell)
        {
            if (redShellAmmo >= diff)
            {
                gunStatScript.currentClip += diff;
                redShellAmmo -= diff;
            }
            else
            {
                gunStatScript.currentClip += redShellAmmo;
                redShellAmmo -= redShellAmmo;
            }
        }
        if (gunStatScript.ammoType == Gun.AmmoType.infinite)
        {
            gunStatScript.currentClip = gunStatScript.clipCapacity;
        }
    }

    public void CalculateSpread(float spreadValue)
    {
        currentSpread = spreadValue * spreadMultiplier;
    }

    private bool IsHeadShot()
    {
        headShotChance = UnityEngine.Random.Range(0, 100);
        if (aimCrossTimer <= 0)
            headShotChance += 35 + hsRateMultiplier;
        if (headShotChance >= 90)
        {
            Debug.Log("HeadShot!");
            return true;
        }
        else
            return false;
    }

    private void Selectable() //0: green(select), 1: blue(mouseover)
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (interactableObject == null) return;
            if (interactableObject.GetComponentInParent<NPCStat>().playerRelationship < 50) return;

            interactableObject.GetComponent<NPCBrain>().MoveToCoords();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (interactableObject == null) return;

            if (interactableObject != null && interactableObject.CompareTag("Npc"))
                interactableObject.GetComponent<NPCBrain>().HighlightShadow(false, 0);
            interactableObject = null;
            if (UIManager.Instance.npcPanel)
                UIManager.Instance.CloseNPCStatsPanel();
        }
        if (interactableObject)
        {
            if (interactableObject.CompareTag("Npc"))
                interactableObject.GetComponent<NPCBrain>().HighlightShadow(true, 0);
        }
        Collider2D hitSelectableTemp = Physics2D.OverlapPoint(mouseWorldPosition, canSelectable);
        if (hitSelectableTemp == null)
        {
            tooltip.transform.parent.gameObject.SetActive(false);
            if (tempRaycastObject != null)
                DisplayInteractableObject(tempRaycastObject, false);
            return;
        }
        if (hitSelectableTemp.CompareTag("Npc"))
        {
            if (interactableObject == hitSelectableTemp.gameObject) return;
            if (tempRaycastObject != null) DisplayInteractableObject(tempRaycastObject, false);
            tempRaycastObject = hitSelectableTemp.gameObject;
            DisplayInteractableObject(tempRaycastObject, true);
            tooltip.transform.parent.gameObject.SetActive(true);
            tooltip.GetComponent<TextMeshPro>().sortingOrder = 100;
            if (hitSelectableTemp.GetComponentInChildren<NPCBrain>().isAngry)
                tooltip.GetComponent<TextMeshPro>().text = hitSelectableTemp.gameObject.GetComponentInParent<NPCStat>().nameSurname + " " + hitSelectableTemp.GetComponentInChildren<HealthSystem>().health + "/" + hitSelectableTemp.GetComponentInChildren<HealthSystem>().maxHealth + "   Rebel";
            else
                tooltip.GetComponent<TextMeshPro>().text = hitSelectableTemp.gameObject.GetComponentInParent<NPCStat>().nameSurname + " " + hitSelectableTemp.GetComponentInChildren<HealthSystem>().health + "/" + hitSelectableTemp.GetComponentInChildren<HealthSystem>().maxHealth + "   [E] Details";
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (interactableObject != null)
                    DisplayInteractableObject(interactableObject, false);
                interactableObject = hitSelectableTemp.gameObject;
                interactableObject.transform.parent.GetComponent<NPCStat>().OpenNPCStatsPanel(interactableObject.transform.parent.gameObject);
            }
        }
        if (hitSelectableTemp.CompareTag("Gear"))
        {
            tempRaycastObject = hitSelectableTemp.gameObject;
            hitSelectableTemp.transform.GetChild(0).gameObject.SetActive(true);
            tooltip.transform.parent.gameObject.SetActive(true);
            tooltip.GetComponent<TextMeshPro>().text = "+5 Material, [E] Take";
            tooltip.GetComponent<TextMeshPro>().sortingOrder = 100;
            if (Input.GetKeyDown(KeyCode.E))
            {
                SoundManager.Instance.PlaySound(listener.GetComponent<AudioSource>(), SoundManager.Instance.collectSFX);
                CampManager.Instance.SetMaterial(1);
                Destroy(hitSelectableTemp.gameObject);
            }
        }
        if (hitSelectableTemp.CompareTag("Barricade"))
        {
            tooltip.transform.parent.gameObject.SetActive(true);
            tooltip.GetComponent<TextMeshPro>().sortingOrder = 100;
            tooltip.GetComponent<TextMeshPro>().text = "Barricade: " + hitSelectableTemp.GetComponentInChildren<HealthSystem>().health + "/" + hitSelectableTemp.GetComponentInChildren<HealthSystem>().maxHealth;
        }
    }

    public void Marry()
    {
        if (marriedNPC != null)
        {
            marriedNPC.GetComponent<NPCStat>().statusHolder.SetActive(false);
            marriedNPC.GetComponent<NPCStat>().marriedPersonWith = null;
        }
        interactableObject.GetComponentInParent<NPCStat>().SetAbilities(true);
        marriedNPC.GetComponent<NPCStat>().statusHolder.SetActive(true);
        marriedNPC.GetComponent<NPCStat>().statusHolder.GetComponent<SpriteRenderer>().sprite = CampManager.Instance.statusIcons[0];
        buffUI.GetComponent<Image>().enabled = true;
        curseUI.GetComponent<Image>().enabled = true;
        SoundManager.Instance.PlaySound(listener.GetComponent<AudioSource>(), SoundManager.Instance.weddingSFX);
    }

    private void DisplayInteractableObject(GameObject obj, bool boolValue)
    {
        if (obj.CompareTag("Gear"))
        {
            obj.transform.GetChild(0).gameObject.SetActive(false);
            return;
        }

        obj.GetComponent<NPCBrain>().HighlightShadow(boolValue, 1);
        if (boolValue == false)
            tempRaycastObject = null;
    }

    public void RefreshMultipliers()
    {
        walkSpeedMultiplier = 1;
        damageMultiplier = 0;
        hsRateMultiplier = 0;
        reloadSpeedMultiplier = 0;
        spreadMultiplier = 1;
        experienceMultiplier = 1;
        ChangePlayerMaxHealth(1);
        buffUI.GetComponent<Image>().enabled = false;
        curseUI.GetComponent<Image>().enabled = false;
    }

    public void ChangePlayerMaxHealth(float marriageMaxHealthMultiplier)
    {
        maxHealthMultiplier = marriageMaxHealthMultiplier;
        int newHealth = (int)(100 * maxHealthMultiplier);
        GetComponent<HealthSystem>().SetMaxHealth(newHealth);
        healthBar.maxValue = newHealth;
    }
    public void SetHealthBar(int value)
    {
        healthBar.value = value;
    }
    public void TakePills()
    {
        if (pillTimer > 0)
        {
            pillTimer -= Time.deltaTime;
            pillTimer = Mathf.Clamp(pillTimer, 0, pillTimer);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                if (CampManager.Instance.pillsCount > 0 && GetComponent<HealthSystem>().health != GetComponent<HealthSystem>().maxHealth)
                {
                    pillTimer = 5;
                    SoundManager.Instance.PlaySound(listener.GetComponent<AudioSource>(), SoundManager.Instance.pillsSFX);
                    CampManager.Instance.pillsCount--;
                    CampManager.Instance.pillsText.text = "Medical Pills: " + CampManager.Instance.pillsCount.ToString();
                    GetComponent<HealthSystem>().health += 20;
                    GetComponent<HealthSystem>().health = Mathf.Clamp(GetComponent<HealthSystem>().health, GetComponent<HealthSystem>().health, GetComponent<HealthSystem>().maxHealth);
                    SetHealthBar(GetComponent<HealthSystem>().health);
                }
            }
        }
    }
}
