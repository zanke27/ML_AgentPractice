using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MummyILAgent : Agent
{

    private StageManagerIL stageManager;
    private new Rigidbody rigidbody;
    private Renderer floorRenderer;
    private Material originMaterial;
    public Material goodMaterial, badMaterial;

    public float moveSpeed = 1.5f;
    public float turnSpeed = 200.0f;

    public override void Initialize()
    {
        stageManager = transform.parent.GetComponent<StageManagerIL>();

        rigidbody = GetComponent<Rigidbody>();
        floorRenderer = transform.parent.Find("Floor").GetComponent<Renderer>();
        originMaterial = floorRenderer.material;
    }

    public override void OnEpisodeBegin()
    {
        stageManager.InitStage();

        rigidbody.velocity = rigidbody.angularVelocity = Vector3.zero;
        transform.localPosition = new Vector3(0, 0.0f, -2.5f);
        transform.localRotation = Quaternion.identity;
    }

    public override void CollectObservations(VectorSensor sensor)
    {

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.DiscreteActions;

        Vector3 direction = Vector3.zero;
        Vector3 rotation = Vector3.zero;

        // Branch 0 : 정지 / 전진 / 후진
        switch (action[0])
        {
            case 1: direction = transform.forward; break;
            case 2: direction = -transform.forward; break;
        }

        // Branch 1 : 정지 / 좌회전 / 우회전
        switch (action[1])
        {
            case 1: rotation = -transform.up; break;
            case 2: rotation = transform.up; break;
        }

        transform.Rotate(rotation, Time.fixedDeltaTime * turnSpeed);
        rigidbody.AddForce(direction * moveSpeed, ForceMode.VelocityChange);

    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.DiscreteActions;
        actionsOut.Clear();

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == stageManager.hintColor.ToString())
        {
            SetReward(+1.0f);
            EndEpisode();

            StartCoroutine(ReverMaterial(goodMaterial));
        }
        else
        {
            if (collision.collider.CompareTag("Wall") || collision.gameObject.name == "Hint")
            {
                SetReward(-0.05f);
            }
            else
            {
                SetReward(-1.0f);
                EndEpisode();

                StartCoroutine(ReverMaterial(badMaterial));
            }
        }
    }

    private IEnumerator ReverMaterial(Material changeMaterial)
    {
        floorRenderer.material = changeMaterial;
        yield return new WaitForSeconds(0.2f);
        floorRenderer.material = originMaterial;
    }
}