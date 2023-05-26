using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManagerIL : MonoBehaviour
{
    public enum HINT_COLOR
    {
        Black,
        Blue,
        Green,
        Red,
    }

    public HINT_COLOR hintColor = HINT_COLOR.Black;

    public Material[] hintMaterial;

    private new Renderer renderer;

    private int preColorIndex = -1;

    void Start()
    {
        renderer = transform.Find("Hint").GetComponent<Renderer>();
    }



    public void InitStage()
    {
        int index = 0;
        do
        {
            index = Random.Range(0, hintMaterial.Length);
        }
        while (index == preColorIndex);
        preColorIndex = index;


        renderer.material = hintMaterial[index];

        hintColor = (HINT_COLOR)index;
    }
}