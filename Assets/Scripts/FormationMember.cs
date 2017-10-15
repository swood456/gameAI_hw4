using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationMember : MonoBehaviour {

    public Vector2 dest;

    Rigidbody2D rb;
    public float acceleration;
    public float max_speed;
	
	void Start () {
        //dest = transform.position;
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        //transform.position = dest;

        // seek towards dest
        rb.AddForce((dest - (Vector2)transform.position).normalized * acceleration);
        if (rb.velocity.magnitude > max_speed)
            rb.velocity = (dest - (Vector2)transform.position).normalized * max_speed;
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("collision start!");
        if(collision.gameObject.GetComponent<BlackBird>())
        {
            FormationManager f_manager = FindObjectOfType<FormationManager>();
            f_manager.RemoveAgent(this);
            Destroy(this.gameObject);
        }
    }
}
