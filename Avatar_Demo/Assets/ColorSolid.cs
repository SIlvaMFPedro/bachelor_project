using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSolid : MonoBehaviour {
    public Material[] materials; //allows input of material colors in a set of sized array;
    public Renderer rend; //we are going to renderer the avatar joints;
    public float changeInterval = 0.33F;


    //private int index = 1; //initialize at 1, otherwise you will have to click on the avatar twice to change color at first;

    void Start()
    {
        rend = GetComponent<Renderer>(); //gives functionality to the renderer;
        rend.enabled = true; //makes the rendered 3d object visable if enabled;
    }

    void Update()
    {
        if (materials.Length == 0)
        {
            return;
        }
        //we want this material index now;
        int index = Mathf.FloorToInt(Time.time / changeInterval);

        //take a model with materials count so that animation repeats;
        index = index % materials.Length;

        //assign it to the renderer;
        rend.sharedMaterial = materials[index];

    }
}
