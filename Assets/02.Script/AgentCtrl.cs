using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentCtrl : Agent
{
    private Transform tr;
    private Rigidbody rb;
    public StageManager stageManager;
    public List<GameObject> stageList = new List<GameObject>();
    public List<Transform> startPoint = new List<Transform>();
    private int pastIndex =0;


    // 초기화 작업
    public override void Initialize()
    {
        MaxStep = 5000;
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
    }

    // 학습(에피소드)이 시작될 때 마다 호출되는 콜백
    public override void OnEpisodeBegin()
    {
        stageManager.InitStage();
        rb.velocity = rb.angularVelocity = Vector3.zero;
        transform.position = startPoint[pastIndex].position;
        transform.forward = startPoint[pastIndex].forward;
        
    }

    // 주변환경을 관측 및 수집정보를 브레인 전달
    public override void CollectObservations(VectorSensor sensor)
    {
    }

    // 브레인으로 부터 전달 받은 명령
    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.DiscreteActions;
        Vector3 dir = Vector3.zero;
        Vector3 rot = Vector3.zero;

        switch (action[0])
        {
            case 1: dir = tr.forward; break;
            case 2: dir = -tr.forward; break;
        }
        switch (action[1])
        {
            case 1: rot = -tr.up; break;
            case 2: rot = tr.up; break;
        }

        tr.Rotate(rot, Time.fixedDeltaTime * 100.0f);
        rb.AddForce(dir * 0.5f, ForceMode.VelocityChange);
        AddReward(-1 / (float)MaxStep);
    }

    // 개발자 테스트용 가상 명령
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.DiscreteActions;
        action.Clear();

        if (Input.GetKey(KeyCode.W))
        {
            action[0] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            action[0] = 2;
        }

        if (Input.GetKey(KeyCode.A))
        {
            action[1] = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            action[1] = 2;
        }
    }

    int miroCollCount;
    private void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("MIRO_WALL"))
        { AddReward(-0.001f); }
        if (coll.collider.CompareTag("TARGET"))
        {
            AddReward(+0.1f);
            rb.velocity = rb.angularVelocity = Vector3.zero;
            coll.gameObject.SetActive(false);
        }

        if (coll.collider.CompareTag("LAST_TARGET"))
        {
            AddReward(+1.0f);
            stageList[0].SetActive(false);
            stageList[1].SetActive(false);
            stageList[2].SetActive(false);
            stageList[3].SetActive(false);

            int index = Random.Range(0, 4);
            if(index == pastIndex)
            {
                index = Random.Range(0, 4);
            }
            // int index = 0;
            // do{
            //     index = Random.Range(0, 3);
            // }while(index != this.pastIndex);
            this.pastIndex = index;

            stageList[index].SetActive(true);
            transform.position = startPoint[index].position;
            EndEpisode();
        }
        if (coll.collider.CompareTag("DEAD_ZONE"))
        {
            AddReward(-1.0f);
            EndEpisode();
        }
    }
}