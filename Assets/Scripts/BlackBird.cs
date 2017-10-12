using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBird : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
        Camera c = Camera.main;
        Vector2 mouse_pos_world;
        mouse_pos_world.x = Input.mousePosition.x;
        mouse_pos_world.y = Input.mousePosition.y;

        Vector3 f_pos = c.ScreenToWorldPoint(new Vector3(mouse_pos_world.x, mouse_pos_world.y, c.nearClipPlane));
        f_pos.z = 0.0f;
        transform.position = f_pos;
        //transform.position = Input.mousePosition;
        
    }
}
