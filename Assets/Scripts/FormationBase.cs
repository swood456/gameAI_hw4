using UnityEngine;
using System.Collections;

//public struct EmergentNode
//{
//    public GameObject node;
//    public GameObject following;
//    public GameObject follower;

//    //follow the entity described by e
//    public bool Follow(EmergentNode e)
//    {
//        //do not follow self
//        if (e.Equals(this) || following != null)
//        {
//            return false;
//        }
//        else if (e.node == following)
//        {
//            return true;
//        }
//        bool check = e.AddFollower(this);
//        if (check)
//        {
//            following = e.node;
//        }
//        return check;
//    }

//    //add entity e to Followers
//    public bool AddFollower(EmergentNode e)
//    {
//        //do not follow self
//        if (e.Equals(this))
//        {
//            return false;
//        }
//        if (follower == null)
//        {
//            follower = e.node;
//            return true;
//        }
//        return false;
//    }

//    //Clear Functions
//    public void StopFollowing()
//    {
//        following = null;
//    }

//    public void RemoveFollower()
//    {
//        follower = null;
//    }

//}

public class FormationBase : MonoBehaviour
{
    protected Rigidbody2D rb;

    public float max_speed;
    //public float cur_max_speed;
    public float acceleration;
    public float rotational_speed;
    public float avoid_rot_speed;

    public Transform head;
    public float raycast_dist;
    public float whisker_delta;

    public FormationBase following = null;
    public FormationBase follower = null;

    // Use this for initialization
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
}
