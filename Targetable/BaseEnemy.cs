using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    //Basic
    protected GameObject playerObj;
    protected float enemyDamage;
    protected float enemyMaxDamage;
    protected float enemyAttackRate, enemyAttackRange, enemyAttackHeight;
    protected float enemySpeed;
    protected float enemyMaxHealth;

    public float EnemyMaxDamage { get => enemyMaxDamage; set => enemyMaxDamage = value; }
    public float EnemyMaxHealth { get => enemyMaxHealth; set => enemyMaxHealth = value; }

    //Checkers
    protected bool attackReady = true;

    public virtual void Attack()
    {
        bool withinRange = Vector3.Distance(transform.position, playerObj.transform.position) < enemyAttackRange;
        bool withinHeight = Mathf.Abs(transform.position.y - playerObj.transform.position.y) <= enemyAttackHeight;
        if (attackReady && withinRange && withinHeight)
        {
            playerObj.GetComponent<ITarget>().OnDamaged(enemyDamage);
            attackReady = false;
            Invoke(nameof(Cooldown), enemyAttackRate);
        }
    }
    public virtual void Move()
    {
        Vector2 enemyDir = new(playerObj.transform.position.x - transform.position.x, playerObj.transform.position.z - transform.position.z);
        enemyDir.Normalize();
        if (Vector3.Distance(transform.position, playerObj.transform.position) > enemyAttackRange)
        {
            transform.position += enemySpeed * Time.deltaTime * new Vector3(enemyDir.x, 0, enemyDir.y);
        }
    }
    public virtual void Cooldown()
    {
        attackReady = true;
    }
}
