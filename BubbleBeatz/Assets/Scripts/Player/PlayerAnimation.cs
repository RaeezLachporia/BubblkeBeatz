using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;

    public void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        float speed = Mathf.Abs(rb.velocity.x);
        animator.SetFloat("Speed", speed);
        bool isJumping = !Mathf.Approximately(rb.velocity.y, 0f);
        animator.SetBool("isJumping", isJumping);
    }
}
