using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationManager : MonoBehaviour {

    public float group_scalar = 2.0f;
    public FormationLeader leader;
    public List<FormationMember> members;


	// Use this for initialization
	void Start () {
        foreach (FormationMember child in GetComponentsInChildren<FormationMember>())
        {
            members.Add(child);
        }
        setup_members();
    }

    private void Update()
    {
        setup_members();
    }

    void setup_members()
    {
        Vector2[] pos = setup_group(members.Count);

        for (int i = 0; i < members.Count; ++i)
        {
            members[i].dest = pos[i];
        }
    }

    Vector2[] setup_group(int num_members)
    {
        Vector2[] member_pos = new Vector2[num_members];

        // for now assume circle shape
        float group_radius = Mathf.Sqrt(group_scalar * num_members / Mathf.PI);

        Vector2 mid = leader.transform.position - leader.transform.right * group_radius;

        // update the leader's velocity based on members dist from center
        float dist_total = 0.0f;

        for (int i = 1; i <= num_members; ++i)
        {
            // place the agents at positions along the circle
            Vector2 agent_pos;
            agent_pos.x = Mathf.Cos(2 * Mathf.PI * ((float)i / (num_members + 1))) * group_radius + mid.x;
            agent_pos.y = Mathf.Sin(2 * Mathf.PI * ((float)i / (num_members + 1))) * group_radius + mid.y;
            member_pos[i-1] = agent_pos;

            //dist_total += members[i-1].transform;
            dist_total += ((Vector2)members[i-1].transform.position - mid).magnitude;
        }

        float avg_dist = dist_total / num_members;
        if(avg_dist > group_radius)
        {
            leader.slowdown_leader(group_radius / avg_dist);
        }
        else
        {
            leader.slowdown_leader(avg_dist / group_radius);
        }
        

        return member_pos;
    }

    /* this is mostly unfinished
    public void assignMembers(Vector2[] pos)
    {
        List<FormationMember> rem = members;
        foreach (Vector2 p in pos)
        {
            FormationMember bestFit;
            float bestDist = Mathf.Infinity;
            foreach(FormationMember m in rem)
            {
                float dist = Vector2.Distance(p, m.transform.position);
                if (dist < bestDist)
                {
                    bestFit = m;
                    bestDist = dist;
                }

                bestFit.dest = 
            }
        }
    }
    */

    public void RemoveAgent(FormationMember f)
    {
        members.Remove(f);
        setup_members();
    }   

}
