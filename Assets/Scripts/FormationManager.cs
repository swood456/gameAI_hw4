using System.Linq;
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

public class FormationManager : MonoBehaviour {

    public float group_scalar = 2.0f;
    public FormationLeader leader;
    public List<FormationMember> members;
    public List<Slot> slots = new List<Slot>();

    public bool emergent = false;
    public HashSet<FormationBase> notFollowed = new HashSet<FormationBase>();
    public HashSet<FormationBase> notFollowing = new HashSet<FormationBase>();


    // Use this for initialization
    void Start () {
        leader = GetComponentInChildren<FormationLeader>();
        foreach (FormationMember child in GetComponentsInChildren<FormationMember>())
        {
            members.Add(child);
        }
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
            //print("new loop! there are " + notFollowed.Count + " in notFollowed and " + notFollowing.Count + "in notFollowing");
            foreach (FormationMember m in members)
            {
                FollowClosest(m);
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
        ResetFollow();
        print(notFollowing.Count);
        int ittCheck = 0; //prevents infinite loop if something goes wrong
        while (notFollowing.Count > 0 && ittCheck < 50)
        {
            foreach (FormationMember m in members)
            {    
                if (notFollowing.Contains(m))
                {
                    //print(m + " is in notFollowing");
                    FollowClosest(m);
                }
            }
            ittCheck++;   
        }

    }

    public void RemoveAgent(FormationMember f)
    {
        members.Remove(f);
        Setup_members();
    }

    //Get the EmergentNode property of some FormationMember or FormationLeader
    //public EmergentNode GetE(ref GameObject g)
    //{
    //    if (g == leader.gameObject)
    //    {
    //        return g.GetComponent<FormationLeader>().node;
    //    }
    //    else
    //    {
    //        return g.GetComponent<FormationMember>().node;
    //    }
    //}

    //Clear notFollowing and notFollowed, update all emergent Nodes

    private void ResetFollow()
    {
        notFollowing.Clear();
        notFollowed.Clear();

        notFollowed.Add(leader);

        foreach (FormationBase f in members)
        {
            Unfollow(f);
            if (f.follower == null)
            {
                notFollowed.Add(f);
            }
        }
    }

    //reset following of g, and the follower of whatever g was following
    private void Unfollow(FormationBase g)
    {
        if (g.following != null)
        {
            g.following.follower = null;
            notFollowed.Add(g.following);  
        }

        g.following = null;
        notFollowing.Add(g);
    }

    //set follower as following toFollow
    private void FollowNode(FormationBase follower, FormationBase toFollow)
    {
        Debug.Assert(follower != null);
        Debug.Assert(toFollow != null);
        Debug.Assert(follower != toFollow);
        if (toFollow != null)
        {
            Unfollow(follower);
            follower.following = toFollow;
            toFollow.follower = follower;
            //print(follower + " is following " + toFollow);
            notFollowed.Remove(toFollow);
            notFollowing.Remove(follower);
        }
    }

    //set g to follow the closest unfollowed node
    private FormationBase FollowClosest(FormationBase m)
    {
        Debug.Assert(m != null);

        FormationBase toFollow = m.following;
        float closest = Mathf.Infinity;
        if (toFollow != null)
        {
            closest = Vector3.Distance(m.transform.position, toFollow.transform.position);
        }
         
        foreach (FormationBase nf in notFollowed)
        {
            if (m != nf && !FollowLoop(m, nf))
            {
                float dist = Vector3.Distance(m.transform.position, nf.transform.position);
                if (dist < closest)
                {
                    closest = dist;
                    toFollow = nf;
                }
            }
        }

        //print(m + " wants to follow " + toFollow);
        if (toFollow != null && toFollow != m.following)
        {
            FollowNode(m, toFollow.GetComponent<FormationBase>());
        }

        return toFollow;
    }

    private bool FollowLoop(FormationBase a, FormationBase b)
    {
        HashSet<FormationBase> chain = new HashSet<FormationBase>();
        chain.Add(a);
        FormationBase check = b;
        while (check != null)
        {
            if (chain.Contains(check))
            {
                return true;
            }
            chain.Add(check);
            check = check.following;
        }
        return false;
    }
}