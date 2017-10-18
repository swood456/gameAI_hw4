using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationMember : MonoBehaviour {


    public Vector2 dest;

    Rigidbody2D rb;
    public float acceleration;
    public float max_speed;
    public float rotational_speed;

    public EmergentNode node;
	
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        node.node = this.gameObject;
        node.following = null;
	}

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        //transform.position = dest;

        rb.AddForce(dest - rb.position);
        if (rb.velocity.magnitude > max_speed)
        {
            rb.velocity.Normalize();
            rb.velocity *= max_speed;
        }
    }

    void Emerge()
    {
        //add behaviour here
        //dest = 
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

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    print("collision start!");
    //    if(collision.gameObject.GetComponent<BlackBird>())
    //    {
    //        FormationManager f_manager = FindObjectOfType<FormationManager>();
    //        f_manager.RemoveAgent(this);
    //        Destroy(this.gameObject);
    //    }
    //}
}
