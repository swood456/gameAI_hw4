using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationManager : MonoBehaviour {

    //Vector2[]
    public float group_scalar = 2.0f;
    public List<FormationMember> members;

	// Use this for initialization
	void Start () {
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
        for(int i = 0; i < num_members; ++i)
        {
            // place the agents at positions along the circle
            Vector2 agent_pos;
            agent_pos.x = Mathf.Cos(2 * Mathf.PI * ((float)i / num_members)) * group_radius;
            agent_pos.y = Mathf.Sin(2 * Mathf.PI * ((float)i / num_members)) * group_radius;
            member_pos[i] = agent_pos;
        }

        return member_pos;
    }

    public void RemoveAgent(FormationMember f)
    {
        members.Remove(f);
        setup_members();
    }
}
