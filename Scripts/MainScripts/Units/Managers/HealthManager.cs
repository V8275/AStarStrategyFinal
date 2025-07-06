using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField]
    private int health = 1;

    private TeamUnit thisUnit;

    public void DecreaseHealth(int damage)
    {
        int resultHealth = Mathf.Clamp(health - damage, 0, health);

        health = resultHealth;

        if (health <= 0) Die();
    }

    private void Die()
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (thisUnit != null)
            gameManager.RemoveUnit(thisUnit);
    }

    public void SetUnit(TeamUnit unit) 
    {
        thisUnit = unit;
    }
}
