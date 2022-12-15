using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject zombie;
    [SerializeField] float spawnerRadius = 2f;
    [SerializeField] Vector2 minMaxSpawnRate;
    public int expectedZombieCount;
    public int readyToSpawnZombieCount;

    [Header("Don't Touch")]
    [SerializeField] float cooldownTimer;
    [SerializeField] float spawnRate;

    private void Start()
    {
        spawnRate = Random.Range(minMaxSpawnRate.x, minMaxSpawnRate.y);
        cooldownTimer = spawnRate;
    }

    private void Update()
    {
        if(GameStateManager.Instance.CurrentGameState != GameStateManager.GameState.Wave) return;

        if (cooldownTimer >= 0)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownTimer = Mathf.Clamp(cooldownTimer, 0, cooldownTimer);
        }
        if (cooldownTimer == 0)
        {
            if (readyToSpawnZombieCount <= 0) return;

            readyToSpawnZombieCount--;
            cooldownTimer = spawnRate;
            GameObject newZombie = Instantiate(zombie, new Vector3(transform.position.x + Random.Range(-spawnerRadius, spawnerRadius), transform.position.y + Random.Range(-spawnerRadius, spawnerRadius)), Quaternion.identity);
            SoundManager.Instance.PlaySoundPitchRandomizer(newZombie.GetComponent<EnemyBrain>().listener.GetComponent<AudioSource>(), SoundManager.Instance.GiveRandomClip(SoundManager.Instance.zombieSpawnSFX), 0.10f);
            newZombie.transform.SetParent(PoolManager.Instance.enemyPoolTransform);
            int randomNumber = Random.Range(0, 100);

            if (randomNumber < 15) //%15
            {
                Debug.Log("fat spawned");
                ZombieCusomizer(newZombie, Random.Range(50,60), 1, 3, 1.5f, Random.Range(20, 25), EnemyBrain.EnemyType.Fat, WaveManager.Instance.colorFat, WaveManager.Instance.colorFatDead, WaveManager.Instance.colorFatHands);
            }
            else if (randomNumber < 40) //%25
            {
                Debug.Log("speedy spawned");
                ZombieCusomizer(newZombie, Random.Range(10, 15), Random.Range(5, 7), 4, 1.5f, Random.Range(10, 15), EnemyBrain.EnemyType.Speedy, WaveManager.Instance.colorSpeedy, WaveManager.Instance.colorSpeedyDead, WaveManager.Instance.colorSpeedyHands);
            }
            else if (randomNumber < 100) //%60
            {
                Debug.Log("normal spawned");
                ZombieCusomizer(newZombie, Random.Range(20, 25), 3, Random.Range(5, 7), 1.5f, Random.Range(10, 15), EnemyBrain.EnemyType.Normal, WaveManager.Instance.colorNormal, WaveManager.Instance.colorNormalDead, WaveManager.Instance.colorNormalHands);
            }
        }
    }

    public void RefreshZombieStock(int expectedZombiesForThisWave)
    {
        cooldownTimer = spawnRate;
        expectedZombieCount = expectedZombiesForThisWave;
        readyToSpawnZombieCount = expectedZombieCount;
    }
    public void ResidualZombies(int residual)
    {
        expectedZombieCount += residual;
        readyToSpawnZombieCount += residual;
    }

    private void ZombieCusomizer(GameObject zombie, int health, int speed, int attackDamage, float attackCooldown, int expAmount, EnemyBrain.EnemyType type, Color color, Color deadColor, Color[] handColors)
    {
        zombie.GetComponent<HealthSystem>().maxHealth = health;
        zombie.GetComponent<HealthSystem>().health = health;
        zombie.GetComponent<AIPath>().maxSpeed = speed + Random.Range(-0.2f, 0.2f);
        zombie.GetComponent<EnemyBrain>().enemyType = type;
        zombie.GetComponent<EnemyBrain>().expAmount = expAmount;
        zombie.GetComponentInChildren<EnemyAttack>().attackDamage = attackDamage;
        zombie.GetComponentInChildren<EnemyAttack>().attackCooldown = attackCooldown + Random.Range(-0.2f, 0.2f);
        zombie.GetComponentInChildren<SpriteRenderer>().color = color;
        zombie.GetComponent<EnemyBrain>().deadBody.GetComponent<SpriteRenderer>().color = deadColor;
        zombie.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = handColors[0];
        zombie.transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color = handColors[1];
    }
}
