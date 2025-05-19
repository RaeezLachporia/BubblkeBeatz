using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotePrefab : MonoBehaviour
{
    public float speed = 10f;
    public Vector2 direction = Vector2.right;
    public int damage = 1;
    public LayerMask Ground;
    public bool isCharged = false;
    public bool isOnBeat = false;
    private void Start()
    {
        if (isOnBeat)
        {
            transform.localScale *= 1.5f;
        }
    }
    private void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);
        if (Mathf.Abs(transform.position.x)>50 && Mathf.Abs(transform.position.y)>50)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyBubbleBobbleAI enemy = collision.GetComponent<EnemyBubbleBobbleAI>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage,isCharged);
            Destroy(gameObject);
        }

        if (((1<<collision.gameObject.layer) & Ground) !=0)
        {
            Destroy(gameObject);
        }
    }
}
