using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationMember : MonoBehaviour {


    public Vector2 dest;

    // variables for me trying to get things to avoid walls
    public Transform head;
    public float whisker_delta;
    public float raycast_dist;
    public float avoid_rot_speed;

    Rigidbody2D rb;
    public float acceleration;
    public float max_speed;
    public float rotational_speed;

    // info for Emergent formations
    public EmergentNode node;
	
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        node.node = this.gameObject;
        node.following = null;
        node.follower = null;
	}

    // Update is called once per frame
    void Update()
    {
        if (node.following != null)
        {
            //print("I'm following " + node.following);
            dest = node.following.transform.position;
        }

        Move();
    }

    private void Move()
    {
        RaycastHit2D left_hit, right_hit;

        left_hit = Physics2D.Raycast(head.position, transform.right + transform.up * whisker_delta, raycast_dist);
        Debug.DrawLine(head.position, head.position + (transform.right + transform.up * whisker_delta) * raycast_dist, Color.red);

        right_hit = Physics2D.Raycast(head.position, transform.right - transform.up * whisker_delta, raycast_dist);
        Debug.DrawLine(head.position, head.position + (transform.right - transform.up * whisker_delta) * raycast_dist, Color.cyan);

        if (left_hit || right_hit)
        {
            Rotate_away(left_hit, right_hit);
            rb.velocity = rb.velocity.magnitude * transform.right;
        }
        else
        {            
            RaycastHit2D long_left_hit = Physics2D.Raycast(head.position, transform.up, whisker_delta);
            Debug.DrawLine(head.position, head.position + (transform.up) * whisker_delta, Color.gray);

            RaycastHit2D long_right_hit = Physics2D.Raycast(head.position, -transform.up, whisker_delta);
            Debug.DrawLine(head.position, head.position + (-transform.up) * whisker_delta, Color.gray);
            
            // rotate
            Vector2 _direction = (dest - (Vector2)transform.position).normalized;
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

            float lerp_angle = Mathf.LerpAngle(transform.eulerAngles.z, angle, Time.deltaTime * rotational_speed);
            if(lerp_angle > 0)
            {
                print("rot left");
                if(!long_right_hit)
                    transform.rotation = Quaternion.Euler(0f, 0f, lerp_angle);
            }
            else
            {
                print("rot right");
                if (!long_left_hit)
                    transform.rotation = Quaternion.Euler(0f, 0f, lerp_angle);
            }
            
        }

        // accelerate
        rb.AddForce(transform.right * acceleration);
        //rb.AddForce((dest - (Vector2)transform.position) * acceleration); // add a force towards their dest, not just forward
        if (rb.velocity.magnitude > max_speed)
        {
            //rb.velocity = rb.velocity.normalized * max_speed;
            rb.velocity = transform.forward * max_speed;
            //print(name + " is hitting max speed");
        }
    }

    void Rotate_away(RaycastHit2D left_hit, RaycastHit2D right_hit)
    {
        //print("rotating away, left: " + left_hit.distance + " right : " + right_hit.distance);

        if (!left_hit)
        {
            Rot_left();
            return;
        }
        if (!right_hit)
        {
            Rot_right();
            return;
        }

        if (left_hit.distance < right_hit.distance)
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

    //    //transform.position = dest;

    //    // seek towards dest
    //    /*
    //    rb.AddForce((dest - (Vector2)transform.position).normalized * acceleration);
    //    if (rb.velocity.magnitude > max_speed)
    //        rb.velocity = (dest - (Vector2)transform.position).normalized * max_speed;
    //    */

    //    // rotate if we are far enough away
    //    if(((Vector2)transform.position - dest).magnitude > 0.25f)
    //    {
    //        Vector2 _direction = (dest - (Vector2)transform.position).normalized;
    //        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

    //        float lerp_angle = Mathf.LerpAngle(transform.eulerAngles.z, angle, Time.deltaTime * rotational_speed);
    //        transform.rotation = Quaternion.Euler(0f, 0f, lerp_angle);
    //    }

    //    if(Vector2.Dot((Vector2)transform.position - dest, transform.right) < 0)
    //    {
    //        // accelerate
    //        rb.AddForce(transform.right * acceleration);
    //        if (rb.velocity.magnitude > max_speed)
    //        {
    //            rb.velocity = rb.velocity.normalized * max_speed;
    //        }
    //    }
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("collision start!");
        if (collision.gameObject.GetComponent<BlackBird>())
        {
            FormationManager f_manager = FindObjectOfType<FormationManager>();
            f_manager.RemoveAgent(this);
            Destroy(this.gameObject);
        }
    }
}
