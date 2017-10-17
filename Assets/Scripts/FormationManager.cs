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

public class FormationManager : MonoBehaviour {

    public float group_scalar = 2.0f;
    public FormationLeader leader;
    public List<FormationMember> members;
    public List<Slot> slots;


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
        Vector2 lead = leader.transform.position;
        foreach (Slot mem in slots)
        {
            mem.Set(lead);
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

        Vector2 mid = leader.transform.position - leader.transform.right * group_radius;
        print("right =" + leader.transform.right + " mid = " + mid);

        // update the leader's velocity based on members dist from center
        //float dist_total = 0.0f;

        for (int i = 1; i <= num_members; ++i)
        {
            // place the agents at positions along the circle
            Vector2 agent_pos;
            agent_pos.x = Mathf.Cos(2 * Mathf.PI * ((float)i / (num_members + 1))) * group_radius + mid.x;
            agent_pos.y = Mathf.Sin(2 * Mathf.PI * ((float)i / (num_members + 1))) * group_radius + mid.y;

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

        assignMembers(member_pos);
    }

    public void assignMembers(List<Vector2> s)
    {
        foreach (FormationMember m in members)
        {
            Vector2 bestFit = Vector2.zero;
            float bestDist = Mathf.Infinity;
            foreach(Vector2 t in s)
            {
                float dist = Vector2.Distance(t, m.transform.position);
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
