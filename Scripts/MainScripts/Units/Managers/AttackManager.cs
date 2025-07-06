using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [SerializeField]
    private int AttackDamage = 0;

    [SerializeField]
    private int attackRadius = 1;

    public void Attack(TeamUnit enemyUnit)
    {
        if (CloseToAttack(enemyUnit))
        {
            var EnemyHealth = enemyUnit.unit.GetHealthController();
            EnemyHealth.SetUnit(enemyUnit);
            EnemyHealth.DecreaseHealth(AttackDamage);
        }
        else 
        {
            Debug.LogWarning("Target is too far for attack");
        }
    }

    private bool CloseToAttack(TeamUnit enemyUnit)
    {
        var currentposition = gameObject.transform.position;
        var enemyposition = enemyUnit.unit.GetCurrentPosition();

        print($"Distance: {Vector3.Distance(currentposition, enemyposition)}");

        if (Vector3.Distance(currentposition, enemyposition) > attackRadius)
            return false;

        return true;
    }
}
