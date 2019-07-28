using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class textScript : MonoBehaviour {

    private Text[] texts;
    private Text text1;

    // Use this for initialization
    void Start () {
        texts = this.gameObject.GetComponentsInChildren<Text>();
        text1 = texts[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.activeInHierarchy)
        {
            text1.text = GlobalVariables.textToShow; // muda a caixa de texto para mostrar o que a variavel global diz
        }
    }
}
