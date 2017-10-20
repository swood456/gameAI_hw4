using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoLevelManager : MonoBehaviour {

    public DestTrigger center_target;

    public List<FormationMember> members;

    Vector2 fake_leader;
    Vector2[] member_leader_delta;
    float movespeed;

    private void Start()
    {
        foreach (FormationMember child in GetComponentsInChildren<FormationMember>())
        {
            members.Add(child);
        }
        fake_leader = transform.position;
        movespeed = members[0].max_speed;

        Setup_members(true);
    }

    

    private void Update()
    {
        // when we get close, set up the 
        if(Vector2.Distance(fake_leader, center_target.transform.position) < 0.75f)
        {
            if(center_target.next_target)
                center_target = center_target.next_target;
        }

        for(int i = 0; i < members.Count; ++i)
        {
            members[i].dest = fake_leader + member_leader_delta[i];
        }

        fake_leader += movespeed * ((Vector2)center_target.transform.position - fake_leader).normalized * Time.deltaTime;
    }
    void Setup_members(bool first_time)
    {
        member_leader_delta = new Vector2[members.Count];
        float group_radius = 3.0f;
        for(int i = 0; i < members.Count; ++i)
        {
            Vector2 agent_pos;
            agent_pos.x = Mathf.Cos(2 * Mathf.PI * ((float)i / (members.Count)) + Mathf.PI) * group_radius;
            agent_pos.y = Mathf.Sin(2 * Mathf.PI * ((float)i / (members.Count)) + Mathf.PI) * group_radius;

            member_leader_delta[i] = agent_pos;
            if(first_time)
            {
                members[i].transform.position = agent_pos + fake_leader;
            }
        }
    }

    public void RemoveAgent(FormationMember f)
    {
        members.Remove(f);
        Setup_members(false);
    }
}
