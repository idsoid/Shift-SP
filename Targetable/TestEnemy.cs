using UnityEngine;

public class TestEnemy : BaseEnemy, ITarget
{
    private Renderer rend;
    public float health { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        playerObj = GameManager.Instance.GetPlayer();
        enemyDamage = 10.0f;
        enemyAttackRate = 1.0f;
        enemyAttackRange = 1.5f;
        enemyAttackHeight = 1f;
        enemySpeed = 3.0f;
        enemyMaxHealth = 50.0f;
        health = enemyMaxHealth;
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("enemyHealth: " + health);
        Move();
        Attack();
        if (rend.material.color != Color.white)
        {
            rend.material.color = Color.Lerp(rend.material.color, Color.white, 0.1f);
        }
    }

    public void OnDamaged()
    {
        rend.material.color = Color.red;
    }

    public void OnDamaged(float damage)
    {
        rend.material.color = Color.red;
        health -= damage;
        if (health <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        health = enemyMaxHealth;
        enemyDamage = enemyMaxDamage;
        //If already under a pool
        if (transform.parent != null)
        {
            gameObject.SetActive(false);
        }
        //Put in pool
        else
        {
            transform.SetParent(GameManager.Instance.GetEnemyPool());
            GameManager.Instance.AddToEnemyPool(gameObject);
        }
        gameObject.SetActive(false);
    }
}
