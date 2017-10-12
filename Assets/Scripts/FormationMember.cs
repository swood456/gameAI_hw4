using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationMember : MonoBehaviour {

    public Vector2 dest;

	// Use this for initialization
	void Start () {
        //dest = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = dest;
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
