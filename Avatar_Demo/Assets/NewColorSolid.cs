using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NewColorSolid : MonoBehaviour {
    public Material[] materials; //allows input of material colors in a set of sized array;
    public Renderer rend; //we are going to renderer the avatar joints;

    private int index = 1; //initialize at 1, otherwise you will have to click on the avatar twice to change color at first;

    void Start()
    {
        rend = GetComponent<Renderer>(); //gives functionality to the renderer;
        rend.enabled = true; //makes the rendered 3d object visable if enabled;
    }

    void OnMouseDown()
    {
        if(materials.Length == 0) //if there are no materials present in the array set then nothing happens;
        {
            return;
        }
        if(Input.GetMouseButton(0))
        {
            index += 1; //when the mouse is pressed down we increment up to the next index location;
            if(index == materials.Length + 1)
            {
                index = 1; //when it reaches the end of the material set array it starts over;
            }
            print(index); //used for debugging;

            rend.sharedMaterial = materials[index-1]; //this sets the material color values inside the index;
        }
    }
}
