using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float Speed = 5f;
    private bool facingright;
    private float horizontal;

    private void Start()
    {
        facingright = true;
    }
    private void FixedUpdate()
    {
        var input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        Vector3 velocity = input.normalized * Speed;
        transform.position += velocity * Time.deltaTime;

        horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal > 0 && !facingright || horizontal < 0 && facingright)
        {
            facingright = !facingright;
            Vector3 thescale = transform.localScale;
            thescale.x *= -1;
            transform.localScale = thescale;
        }


    }
}
   