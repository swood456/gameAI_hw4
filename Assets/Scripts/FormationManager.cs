using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Slot
{
    public Vector2 pos;
    public FormationMember member;

    public void Set(Vector2 l)
    {
        member.dest = l + pos;
    }
}

public struct EmergentNode
{
    public GameObject node;
    public GameObject following;
    public GameObject follower;

    //follow the node e, fails this node is already following another
    public bool Follow(EmergentNode e)
    {
        //do not follow self
        if (e.Equals(this) || following != null)
        {
            return false;
        }
        else if (e.node == following)
        {
            return true;
        }
        bool check = e.AddFollower(this);
        if (check) {
            following = e.node;
        }
        return check;
    }

    public void StopFollowing()
    {
        following = null;
    }

    public bool AddFollower(EmergentNode e)
    {
        //do not follow self
        if (e.Equals(this))
        {
            return false;
        }
        if (follower == null)
        {
            follower = e.node;
            return true;
        }
        return false;
    }

    public void RemoveFollower()
    {
        follower = null;
    }

}

public class FormationManager : MonoBehaviour {

    public float group_scalar = 2.0f;
    public FormationLeader leader;
    public List<FormationMember> members;
    public List<Slot> slots;
    public bool emergent = false;
    List<GameObject> notFollowed = new List<GameObject>();


    // Use this for initialization
    void Start () {
        leader = GetComponentInChildren<FormationLeader>();
        foreach (FormationMember child in GetComponentsInChildren<FormationMember>())
        {
            members.Add(child);
        }
        slots = new List<Slot>();
        Setup_members();
    }

    private void Update()
    {
        if (emergent != true)
        {
            Vector2 lead = leader.transform.position;
            foreach (Slot mem in slots)
            {
                mem.Set(lead);
            }
        }
        else
        {
            //update to follow the closest free flock member
            foreach (FormationMember m in members)
            {
                GameObject toFollow = null;
                float dist;
                if (m.node.following != null)
                {
                    dist = Vector3.Distance(m.node.following.transform.position, m.transform.position);
                }
                else
                {
                    toFollow = null;
                    dist = Mathf.Infinity;
                }
                
                foreach(GameObject g in notFollowed)
                {
                    float newDist = Vector3.Distance(g.transform.position, m.transform.position);
                    if (newDist < dist)
                    {
                        dist = newDist;
                        toFollow = g;
                    }
                }

                if (toFollow != null)
                {
                    if (m.node.following != null)
                    {
                        notFollowed.Add(m.node.following);
                        GetE(m.node.following).RemoveFollower();
                    }
                    
                    m.node.StopFollowing();

                    notFollowed.Remove(toFollow);
                    m.node.Follow(GetE(toFollow));
                }
            }
        }
    }

    void Setup_members()
    {
        if (!emergent)
        {
            Setup_circle(members.Count);
        }
        else
        {
            AssignFollowers();
        }
    }

    void Setup_circle(int num_members)
    {
        List<Vector2> member_pos = new List<Vector2>();

        // for now assume circle shape
        float group_radius = Mathf.Sqrt(group_scalar * num_members / Mathf.PI);

        Vector2 mid = -leader.transform.right * group_radius;

        // update the leader's velocity based on members dist from center
        //float dist_total = 0.0f;

        for (int i = 1; i <= num_members; ++i)
        {
            // place the agents at positions along the circle
            Vector2 agent_pos;
            agent_pos.x = Mathf.Cos(2 * Mathf.PI * ((float)i / (num_members + 1)) + Mathf.PI) * group_radius + mid.x;
            agent_pos.y = Mathf.Sin(2 * Mathf.PI * ((float)i / (num_members + 1)) + Mathf.PI) * group_radius + mid.y;

            member_pos.Add(agent_pos);

            //dist_total += members[i-1].transform;
            //dist_total += ((Vector2)members[i-1].transform.position - mid).magnitude;
        }

        //float avg_dist = dist_total / num_members;
        //if(avg_dist > group_radius)
        //{
        //    leader.slowdown_leader(group_radius / avg_dist);
        //}
        //else
        //{
        //    leader.slowdown_leader(avg_dist / group_radius);
        //}

        AssignMembers(member_pos, mid);
    }

    public void AssignMembers(List<Vector2> s, Vector2 mid)
    {
        foreach (FormationMember m in members)
        {
            Vector2 bestFit = Vector2.zero;
            float bestDist = Mathf.Infinity;
            foreach(Vector2 t in s)
            {
                float dist = Vector2.Distance(t + mid, m.transform.position);
                if (dist < bestDist)
                {
                    bestFit = t;
                    bestDist = dist;
                }               
            }
            Slot tmp;
            tmp.pos = bestFit;
            tmp.member = m;

            slots.Add(tmp);

            s.Remove(bestFit);
        }
    }

    public void AssignFollowers()
    {
        List<GameObject> notFollowing = new List<GameObject>();
        notFollowed = new List<GameObject>();

        leader.node.follower = null;
        notFollowed.Add(leader.gameObject);
        
        foreach (FormationMember f in members)
        {
            EmergentNode e = f.node;
            e.StopFollowing();
            notFollowing.Add(f.gameObject);

            e.RemoveFollower();
            notFollowed.Add(f.gameObject);
        }
        print(notFollowing.Count);

        while (notFollowing.Count > 0)
        {
            GameObject x = notFollowing.First<GameObject>();

            GameObject toFollow = null;
            float dist = Mathf.Infinity;
            foreach (GameObject y in notFollowed)
            {
                if (x != y)
                {
                    float yDist = Vector3.Distance(x.transform.position, y.transform.position);
                    if (yDist < dist)
                    {
                       dist = yDist;
                       toFollow = y;
                    }
                }
            }
            if (toFollow != null)
            {
                print(x + " wants to follow " + toFollow);
                EmergentNode e = GetE(toFollow);
                if (GetE(x).Follow(e))
                {
                    print("Success!");
                    notFollowing.Remove(x);
                    notFollowed.Remove(toFollow);
                }
            }
        }
        print("all followers have been assigned");
    }

    public void RemoveAgent(FormationMember f)
    {
        members.Remove(f);
        Setup_members();
    }

    public EmergentNode GetE(GameObject g)
    {
        if (g == leader.gameObject)
        {
            return g.GetComponent<FormationLeader>().node;
        }
        else
        {
            return g.GetComponent<FormationMember>().node;
        }
    }

}
