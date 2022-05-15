using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class AgentCtrlUpdate : Agent
{
    Transform tr;
    Rigidbody rb;
    public float rotaSpeed = 100.0f;
    public float speed = 0.1f;

    public override void Initialize()
    {
        MaxStep = 5000;
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        rb.velocity = rb.angularVelocity = Vector3.zero;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.DiscreteActions;
        Vector3 dir = Vector3.zero;
        Vector3 rota = Vector3.zero;

        switch (action[0])
        {
            case 1: dir = tr.forward; break;
            case 2: dir = -tr.forward; break;
        }
        switch (action[1])
        {
            case 1: rota = -tr.up; break;
            case 2: rota = tr.up; break;
        }

        tr.Rotate(rota, Time.fixedDeltaTime * rotaSpeed);
        rb.AddForce(dir * speed, ForceMode.VelocityChange);
        AddReward(-1 / (float)MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.DiscreteActions;
        action.Clear();

        if(Input.GetKey(KeyCode.W)) action[0] = 1;
        if(Input.GetKey(KeyCode.S)) action[0] = 2;
        if(Input.GetKey(KeyCode.A)) action[1] = 1;
        if(Input.GetKey(KeyCode.D)) action[1] = 2;
    }
}
