using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class FloorAgent : Agent
{
    [SerializeField]
    private GameObject ball;
    private Rigidbody ballRigidBody;

    public override void Initialize()
    {
        ballRigidBody = ball.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));

        ballRigidBody.velocity = new Vector3(0f, 0f, 0f);
        ball.transform.localPosition = new Vector3(Random.Range(-1.5f, 1.5f), 1.5f, Random.Range(-1.5f, 1.5f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.rotation.z);
        sensor.AddObservation(transform.rotation.x);
        sensor.AddObservation(ball.transform.position - transform.position);
        sensor.AddObservation(ballRigidBody.velocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float zrot = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float xrot = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        transform.Rotate(new Vector3(0, 0, 1), zrot);
        transform.Rotate(new Vector3(1, 0, 0), xrot);

        if (DropBall())
        {
            SetReward(-1f);
            EndEpisode();
        }
        else
        {
            SetReward(0.1f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var ContinuousActionOut = actionsOut.ContinuousActions;
        ContinuousActionOut[0] = -Input.GetAxis("Horizontal");
        ContinuousActionOut[1] = Input.GetAxis("Vertical");
    }

    private bool DropBall()
    {
        return ball.transform.position.y - transform.position.y < -2f ||
            Mathf.Abs(ball.transform.position.x - transform.position.x) > 2.5f ||
            Mathf.Abs(ball.transform.position.z - transform.position.z) > 2.5f;
    }
}