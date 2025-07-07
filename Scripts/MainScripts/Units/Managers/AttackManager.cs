using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [SerializeField]
    private int AttackDamage = 0;

    [SerializeField]
    private int attackRadius = 1;

    private AnimationManager animationManager;
    private GameManager gameManager;

    private void Start()
    {
        animationManager = GetComponent<AnimationManager>();
        gameManager = FindFirstObjectByType<GameManager>();
    }
    
    public void Attack(TeamUnit enemyUnit)
    {
        if (CloseToAttack(enemyUnit))
        {
            var EnemyHealth = enemyUnit.unit.GetHealthController();

            EnemyHealth.SetUnit(enemyUnit);
            animationManager.AnimationShoot();
            EnemyHealth.DecreaseHealth(AttackDamage);
        }
        else 
        {
            Debug.LogWarning("Target is too far for attack");
        }
    }

    private bool CloseToAttack(TeamUnit enemyUnit)
    {
        var currentposition = gameManager.SelectedUnit.unit.GetCurrentPosition();
        var enemyposition = enemyUnit.unit.GetCurrentPosition();

        print($"Distance: {Vector3.Distance(currentposition, enemyposition)}");

        if (Vector3.Distance(currentposition, enemyposition) > attackRadius)
        {
            return false;
        }

        RotateTowardsTarget(enemyposition);

        return true;
    }

    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - gameObject.transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            gameObject.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
