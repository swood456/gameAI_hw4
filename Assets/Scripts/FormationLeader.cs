using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationLeader : MonoBehaviour {

    public Vector2 dest;
    public float max_speed;
    public float acceleration;
    public float rotational_speed;

    Rigidbody2D rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {


        // rotate
        Vector2 _direction = (dest - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        
        float lerp_angle = Mathf.LerpAngle(transform.eulerAngles.z, angle, Time.deltaTime * rotational_speed);
        transform.rotation = Quaternion.Euler(0f, 0f, lerp_angle);
        
        // accelerate
        rb.AddForce(transform.right);
        if(rb.velocity.magnitude > max_speed)
        {
            rb.velocity = rb.velocity.normalized * max_speed;
        }
	}
}
