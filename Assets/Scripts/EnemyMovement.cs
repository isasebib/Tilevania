using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    Rigidbody2D enemyRigidBody;
    CircleCollider2D circleCollider;
    void Start()
    {
        enemyRigidBody = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        MoveEnemy();
        Die();
    }

    void MoveEnemy()
    {
        enemyRigidBody.linearVelocity = new Vector2 (moveSpeed, 0f);
    }

    void OnTriggerExit2D(Collider2D collision) 
    {
        moveSpeed = -moveSpeed;
        FlipEnemy();
    }

    void FlipEnemy()
    {
            transform.localScale = new Vector2 (-(Mathf.Sign(enemyRigidBody.linearVelocity.x)), 1f);
    }

    void Die()
    {
        if (circleCollider.IsTouchingLayers(LayerMask.GetMask("Bullet")))
        {
            Destroy(gameObject, 0.1f);
        }
    }
}
