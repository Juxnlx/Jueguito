using UnityEngine;

public class MonoBehaviourBlobbie : MonoBehaviour
{
    private float vertical;
    [SerializeField] private float speed;
    private Rigidbody2D rb2D;
    private Animator animator;
    public GameObject prefabBala;
    private float ultimoDisparo = 0;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        speed = 5;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        vertical = Input.GetAxisRaw("Vertical");
        animator.SetBool("Run", vertical != 0.0f);

        if (Input.GetKey(KeyCode.Space) && Time.time > ultimoDisparo + 0.5f)
        {
            Shoot();
            ultimoDisparo = Time.time;
        }
        ;
    }

    private void FixedUpdate()
    {
        rb2D.velocity = new Vector2(0, vertical * speed);
    }

    private void Shoot()
    {
        GameObject bala = Instantiate(
            prefabBala,
            transform.position + Vector3.right * 0.1f,
            Quaternion.identity
        );
        Destroy(bala, 1f);
    }

    public void destroyBala()
    {
        Destroy(gameObject);
    }
}