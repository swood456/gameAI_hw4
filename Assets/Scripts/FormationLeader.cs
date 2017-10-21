using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationLeader : FormationBase {

    public DestTrigger cur_dest;
    //public float max_speed;
    public float cur_max_speed;
    //public float acceleration;
    //public float rotational_speed;
    //public float avoid_rot_speed;

    //public Transform head;
    //public float raycast_dist;
    //public float whisker_delta;

    //Rigidbody2D rb;

    // Use this for initialization
    public override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cur_max_speed = max_speed;
    }
	
	// Update is called once per frame
	void Update () {
        //rot_left();
        
        RaycastHit2D left_hit, right_hit;

        left_hit = Physics2D.Raycast(head.position, transform.right + transform.up * whisker_delta, raycast_dist);
        Debug.DrawLine(head.position, head.position + (transform.right + transform.up * whisker_delta) * raycast_dist,  Color.red);

        right_hit = Physics2D.Raycast(head.position, transform.right - transform.up * whisker_delta, raycast_dist);
        Debug.DrawLine(head.position, head.position + (transform.right - transform.up * whisker_delta) * raycast_dist, Color.cyan);
        
        if (left_hit || right_hit)
        {
            Rotate_away(left_hit, right_hit);
        }
        else
        {
            // try a second, longer raycast for turning?
            RaycastHit2D long_left_hit = Physics2D.Raycast(head.position, transform.right + transform.up * whisker_delta, raycast_dist + 0.25f);
            RaycastHit2D long_right_hit = Physics2D.Raycast(head.position, transform.right - transform.up * whisker_delta, raycast_dist + 0.25f);

            if(!long_left_hit && !long_right_hit)
            {
                // rotate
                Vector2 _direction = (cur_dest.transform.position - transform.position).normalized;
                float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

                float lerp_angle = Mathf.LerpAngle(transform.eulerAngles.z, angle, Time.deltaTime * rotational_speed);
                //float lerp_angle = Mathf.LerpAngle(transform.eulerAngles.z, angle, Time.deltaTime);
                transform.rotation = Quaternion.Euler(0f, 0f, lerp_angle);
            }
            

            // accelerate
            rb.AddForce(transform.right * acceleration);
            if (rb.velocity.magnitude > cur_max_speed)
            {
                rb.velocity = rb.velocity.normalized * cur_max_speed;
            }
        }
        
	}

    public void Slowdown_leader(float mult)
    {
        cur_max_speed = max_speed * mult;
    }


    void Rotate_away(RaycastHit2D left_hit, RaycastHit2D right_hit)
    {
        //print("rotating away, left: " + left_hit.distance + " right : " + right_hit.distance);

        if (!left_hit)
        {
            Rot_left();
            return;
        }
        if(!right_hit)
        {
            Rot_right();
            return;
        }

        if(left_hit.distance < right_hit.distance)
        {
            //rot_left();
            Rot_right();
        }
        else
        {
            //rot_right();
            Rot_left();
        }
    }
    
    void Rot_left()
    {
        //print("rot left");
        Vector3 rot = transform.eulerAngles;
        rot.z += Time.deltaTime * avoid_rot_speed;
        transform.rotation = Quaternion.Euler(rot);
    }

    void Rot_right()
    {
        //print("rot right");
        Vector3 rot = transform.eulerAngles;
        rot.z -= Time.deltaTime * avoid_rot_speed;
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
