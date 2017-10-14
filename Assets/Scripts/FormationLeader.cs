using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationLeader : MonoBehaviour {

    public DestTrigger cur_dest;
    public float max_speed;
    public float acceleration;
    public float rotational_speed;

    public Transform head;
    public float raycast_dist;
    public float whisker_delta;

    Rigidbody2D rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {

        
        RaycastHit2D left_hit, right_hit;

        left_hit = Physics2D.Raycast(head.position, transform.right + transform.up * whisker_delta, raycast_dist);
        Debug.DrawLine(head.position, head.position + (transform.right + transform.up * whisker_delta) * raycast_dist,  Color.red);

        right_hit = Physics2D.Raycast(head.position, transform.right - transform.up * whisker_delta, raycast_dist);
        Debug.DrawLine(head.position, head.position + (transform.right - transform.up * whisker_delta) * raycast_dist, Color.cyan);
        
        if (left_hit || right_hit)
        {
            rotate_away(left_hit, right_hit);
        }
        else
        {
            // rotate
            Vector2 _direction = (cur_dest.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

            float lerp_angle = Mathf.LerpAngle(transform.eulerAngles.z, angle, Time.deltaTime * rotational_speed);
            transform.rotation = Quaternion.Euler(0f, 0f, lerp_angle);

            // accelerate
            rb.AddForce(transform.right * acceleration);
            if (rb.velocity.magnitude > max_speed)
            {
                rb.velocity = rb.velocity.normalized * max_speed;
            }
        }
        
	}

    void rotate_away(RaycastHit2D left_hit, RaycastHit2D right_hit)
    {
        print("rotating away, left: " + left_hit.distance + " right : " + right_hit.distance);

        if (!left_hit)
        {
            rot_left();
            return;
        }
        if(!right_hit)
        {
            rot_right();
            return;
        }

        if(left_hit.distance < right_hit.distance)
        {
            rot_left();
        }
        else
        {
            rot_right();
        }
    }
    
    void rot_left()
    {
        print("rot left");
        Vector3 rot = transform.eulerAngles;
        rot.z += Time.deltaTime * rotational_speed * 100;
        transform.rotation = Quaternion.Euler(rot);
    }

    void rot_right()
    {
        print("rot right");
        Vector3 rot = transform.eulerAngles;
        rot.z -= Time.deltaTime * rotational_speed * 10;
        transform.rotation = Quaternion.Euler(rot);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DestTrigger dt = collision.GetComponent<DestTrigger>();
        if(dt)
        {
            //print("hit trigger" + dt.name);
            if(dt.next_target)
                cur_dest = dt.next_target;
        }
    }
}
