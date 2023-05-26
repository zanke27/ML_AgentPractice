using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class MummyRayAgent : Agent
{
    [SerializeField] private GameObject stage;
    [SerializeField] private GameObject goodItem;
    [SerializeField] private GameObject badItem;
    [SerializeField] private Material redMat;
    [SerializeField] private Material blueMat;
    [SerializeField] private Material grayMat;
    [SerializeField] private MeshRenderer floorMesh;

    private Rigidbody rigidbody;
    private List<GameObject> itemList = new List<GameObject>();

    private int goodItemCount = 0;

    public override void Initialize()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        Reset();

        transform.localPosition = new Vector3(Random.Range(-24, 24), 0.5f, Random.Range(-24, 24));

        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        goodItemCount = 0;

        PrefabRandomSpawn(goodItem, 30);
        PrefabRandomSpawn(badItem, 10);
    }

    public override void CollectObservations(VectorSensor sensor)
    {

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = 0f;
        float turnAmount = 0f;

        if (actions.DiscreteActions[0] == 1f)
        {
            forwardAmount = -1;
        }
        else if (actions.DiscreteActions[0] == 2f)
        {
            forwardAmount = 1;
        }

        if (actions.DiscreteActions[1] == 1f)
        {
            turnAmount = -1;
        }
        else if (actions.DiscreteActions[1] == 2f)
        {
            turnAmount = 1f;
        }

        rigidbody.MovePosition(transform.position + transform.forward * forwardAmount * 5f * Time.fixedDeltaTime);
        transform.Rotate(transform.up * turnAmount * 100f * Time.fixedDeltaTime);

        AddReward(-1 / (float)MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int forwardBackAction = 0;
        int turnAction = 0;

        if (Input.GetKey(KeyCode.W))
        {
            forwardBackAction = 2;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            forwardBackAction = 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            turnAction = 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            turnAction = 2;
        }

        actionsOut.DiscreteActions.Array[0] = forwardBackAction;
        actionsOut.DiscreteActions.Array[1] = turnAction;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("GoodItem"))
        {
            StartCoroutine(SetFloorColor(blueMat));
            SetReward(1f);
            Destroy(collision.gameObject);
            goodItemCount++;
            if (goodItemCount == 30)
            {
                Reset();
                EndEpisode();
            }
        }
        else if (collision.transform.CompareTag("BadItem"))
        {
            StartCoroutine(SetFloorColor(redMat));
            SetReward(-1f);
            Reset();
            EndEpisode();
        }
        else if (collision.transform.CompareTag("Wall"))
        {
            SetReward(-0.1f);
        }
    }

    public void PrefabRandomSpawn(GameObject prefab, int count)
    {
        for (int i = 0; i < count; ++i)
        {
            GameObject obj = Instantiate(prefab, stage.transform);
            obj.transform.localPosition = new Vector3(Random.Range(-24, 24), 0.5f, Random.Range(-24, 24));
            itemList.Add(obj);
        }
    }

    public void Reset()
    {

        if (itemList != null)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i] != null)
                {
                    Destroy(itemList[i].gameObject);
                }
            }

            itemList.Clear();
        }
    }

    IEnumerator SetFloorColor(Material mat)
    {
        floorMesh.material = mat;
        yield return new WaitForSeconds(0.2f);
        floorMesh.material = grayMat;
    }
}
