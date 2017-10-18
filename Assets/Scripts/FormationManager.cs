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
    public bool right;

    public GameObject leftFollower;
    public GameObject rightFollower;

    public bool Full()
    {
        return (leftFollower == null || rightFollower == null);
    }

    public void Follow(GameObject f)
    {
        int check = f.GetComponent<EmergentNode>().AddFollower(node);
        if (check == 0) { right = true; following = f; }
        else if (check == 1) { right = false; following = f; }
    }

    public int AddFollower(GameObject f)
    {
        if (rightFollower == null)
        {
            rightFollower = f;
            return 0;
        }
        else if (leftFollower == null)
        {
            leftFollower = f;
            return 1;
        }
        return 2;
    }
}

public class FormationManager : MonoBehaviour {

    public float group_scalar = 2.0f;
    public FormationLeader leader;
    public List<FormationMember> members;
    public List<Slot> slots;
    bool emergent = false;


	// Use this for initialization
	void Start () {
        leader = GetComponentInChildren<FormationLeader>();
        foreach (FormationMember child in GetComponentsInChildren<FormationMember>())
        {
            members.Add(child);
        }
        slots = new List<Slot>();
        setup_members();
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
            foreach (FormationMember f in members)
            {
                if (f.node.following == null)
                {
                    GameObject toFollow = null;
                    float dist = Mathf.Infinity;

                    foreach(GameObject m in GetComponentsInChildren<GameObject>())
                    {
                        EmergentNode n = m.GetComponent<EmergentNode>();
                        if (m != f && n.Full() == false && Vector3.Distance(m.transform.position, f.transform.position) < dist) {
                            dist = Vector3.Distance(m.transform.position, f.transform.position);
                            toFollow = m;
                        }
                    }

                    if (toFollow != null)
                    {
                        f.node.Follow(toFollow);
                    }
                }
            }
        }
    }

    void setup_members()
    {
        setup_circle(members.Count);
    }

    void setup_circle(int num_members)
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

        assignMembers(member_pos, mid);
    }

    public void assignMembers(List<Vector2> s, Vector2 mid)
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

    public void RemoveAgent(FormationMember f)
    {
        members.Remove(f);
        setup_members();
    }

}
