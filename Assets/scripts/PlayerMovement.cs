using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float vitesse = 5f;
    private Rigidbody2D rb;
    private Vector2 direction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.Bloque)
        {
            direction = Vector2.zero;
            return;
        }

        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");
        direction = direction.normalized;

        // Le personnage se retourne selon la direction
        if (direction.x > 0.1f) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1f);
        if (direction.x < -0.1f) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1f);
    }

    void FixedUpdate()
    {
        rb.velocity = direction * vitesse;
    }
}
