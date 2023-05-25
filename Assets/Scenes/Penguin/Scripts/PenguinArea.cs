using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

public class PenguinArea : MonoBehaviour
{
    public PenguinAgent penguinAgent;
    public GameObject penguinBaby;
    public TextMeshPro cumulativeRewardText;

    public Fish fishPrefab;
    private List<Fish> fishList = new List<Fish>();

    public static Vector3 ChooseRandomPosition(Vector3 center, float minAngle, float maxAngle, float minRadius, float maxRadius)
    {
        float radius = minRadius;
        float angle = minAngle;

        if (maxRadius > minRadius)
        {
            radius = Random.Range(minRadius, maxRadius);
        }

        if (maxAngle > minAngle)
        {
            angle = Random.Range(minAngle, maxAngle);
        }

        return center + Quaternion.Euler(0f, angle, 0f) * Vector3.forward * radius;
    }

    public int FishRemaining
    {
        get
        {
            return fishList.Count;
        }
    }

    private void Update()
    {
        cumulativeRewardText.text = penguinAgent.GetCumulativeReward().ToString("0.00");
    }

    public void ResetArea()
    {
        RemoveAllFish();
        PlacePenguin();
        PlaceBaby();
        SpawnFish(4, 0.5f);
    }

    private void PlacePenguin()
    {
        ResetRigidbodyVelocity(penguinAgent.GetComponent<Rigidbody>());
        penguinAgent.transform.position = ChooseRandomPosition(transform.position, 0f, 360f, 0f, 9f) + Vector3.up * 0.5f;
        penguinAgent.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }

    private void PlaceBaby()
    {
        ResetRigidbodyVelocity(penguinBaby.GetComponent<Rigidbody>());
        penguinBaby.transform.position = ChooseRandomPosition(transform.position, -45f, 45f, 4f, 9f) + Vector3.up * 0.5f;
        penguinBaby.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    private void SpawnFish(int count, float fishSpeed)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = ChooseRandomPosition(transform.position, 100f, 260f, 2f, 13f) + Vector3.up * 0.5f;
            Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            Fish fish = Instantiate(fishPrefab, spawnPos, rotation, transform);
            fish.fishSpeed = fishSpeed;
            fishList.Add(fish);
        }
    }

    public void RemoveSpecifidFish(GameObject fishObject)
    {
        Fish fish = fishObject.GetComponent<Fish>();
        fishList.Remove(fish);
        Destroy(fishObject);
    }

    private void RemoveAllFish()
    {
        if (fishList != null)
        {
            for (int i = 0; i < fishList.Count; i++)
            {
                if (fishList[i] != null)
                {
                    Destroy(fishList[i].gameObject);
                }
            }

            fishList.Clear();
        }
    }

    private void ResetRigidbodyVelocity(Rigidbody rigid)
    {
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }
}