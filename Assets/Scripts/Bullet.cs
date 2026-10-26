using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 5f;
    Rigidbody2D rigidbody;
    PlayerMovement player1;
    CircleCollider2D circleCollider;

    float xSpeed;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        player1 = FindAnyObjectByType<PlayerMovement>();
        xSpeed = player1.transform.localScale.x * bulletSpeed;
        circleCollider = GetComponent<CircleCollider2D>();

    }

    void Update()
    {
        Shoot();
        Die();
    }

    void Shoot()
    {
        rigidbody.linearVelocity = new Vector2 (xSpeed, 0f);
        Destroy(gameObject, 1.2f);
    }

    void Die()
    {
        if (circleCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            Destroy(gameObject, 0.1f);
        }
    }



}
