using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotePrefab : MonoBehaviour
{
    public float speed = 10f;
    public Vector2 direction = Vector2.right;

    private void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);
        if (Mathf.Abs(transform.position.x)>50 && Mathf.Abs(transform.position.y)>50)
        {
            Destroy(gameObject);
        }
    }
}
