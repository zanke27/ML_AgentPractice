using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private Material redMat;
    [SerializeField] private Material blackMat;
    [SerializeField] private Material blueMat;
    [SerializeField] private Material greenMat;
    [SerializeField] private Material grayMat;

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Reset()
    {

    }

    public IEnumerator SetBlueFloorColor()
    {
        meshRenderer.material = blueMat;
        yield return new WaitForSeconds(0.2f);
        meshRenderer.material = grayMat;
    }

    public IEnumerator SetRedFloorColor()
    {
        meshRenderer.material = redMat;
        yield return new WaitForSeconds(0.2f);
        meshRenderer.material = grayMat;
    }
}
