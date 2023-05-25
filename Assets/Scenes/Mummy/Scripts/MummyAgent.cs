using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MummyAgent : Agent
{
    [SerializeField] private MeshRenderer floorRen;
    [SerializeField] private GameObject target;
    [SerializeField] private Material blueMat;
    [SerializeField] private Material redMat;
    [SerializeField] private Material grayMat;

    private Rigidbody mummyRb;

    private bool isEnd = false;

    public override void Initialize()
    {
        mummyRb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-3.5f, 3.5f), 0.05f, Random.Range(-3.5f, 3.5f));
        target.transform.localPosition = new Vector3(Random.Range(-3.5f, 3.5f), 0.5f, Random.Range(-3.5f, 3.5f));
        floorRen.material = grayMat;

        mummyRb.velocity = Vector3.zero;
        mummyRb.angularVelocity = Vector3.zero;

        isEnd = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.transform.localPosition);
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(mummyRb.velocity.x);
        sensor.AddObservation(mummyRb.velocity.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.ContinuousActions;

        Vector3 dir = (Vector3.forward * action[0]) + (Vector3.right * action[1]);
        mummyRb.AddForce(dir.normalized * 50.0f);

        if (isEnd == false) AddReward(-0.001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.ContinuousActions;
        action[0] = Input.GetAxis("Vertical");
        action[1] = Input.GetAxis("Horizontal");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isEnd) return;

        if (collision.transform.CompareTag("DeadZone"))
        {
            floorRen.material = redMat;
            SetReward(-1f);
            isEnd = true;
            Invoke("EndEpisode", 0.5f);
        }
        else if (collision.transform.CompareTag("Target"))
        {
            floorRen.material = blueMat;
            SetReward(1f);
            isEnd = true;
            Invoke("EndEpisode", 0.5f);
        }
    }
}
