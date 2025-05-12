using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]private float speed;
    [SerializeField] private float JumpForce = 12f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    public Rigidbody2D RB;
    private float nextFireTime = 0f;
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float noteCooldown = 0.3f;
    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        //Horizontal Movement
        RB.velocity = new Vector2(horizontalInput*speed, RB.velocity.y);
        if (horizontalInput>0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontalInput <0 )
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        //Jumping
        if (Input.GetKey(KeyCode.Space) && Mathf.Abs(RB.velocity.y)<0.01f)
        {
            RB.velocity = new Vector2(RB.velocity.x, JumpForce);
        }
        //gravity
        if (RB.velocity.y <0)
        {
            RB.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if(RB.velocity.y >0 && !Input.GetKey(KeyCode.Space))
        {
            RB.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.F)&& Time.deltaTime>=nextFireTime)
        {
            ShootNote();
            nextFireTime = Time.deltaTime + noteCooldown; 
        }
        
    }
    private void ShootNote()
    {
        GameObject note = Instantiate(notePrefab, firePoint.position, Quaternion.identity);
        NotePrefab projectile = note.GetComponent<NotePrefab>();
        projectile.direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    }
}
