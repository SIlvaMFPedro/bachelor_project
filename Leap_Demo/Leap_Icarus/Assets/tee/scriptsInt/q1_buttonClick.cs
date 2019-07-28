using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class q1_buttonClick : MonoBehaviour
{
    public Button bttn_Local, bttn_Antonio, bttn_Maria, bttn_Manuela, bttn_Paula, bttn_Joaquim, bttn_Diogo;
    // Use this for initialization
    void Start()
    {
        Button q1_Local = bttn_Local.GetComponent<Button>();
        q1_Local.onClick.AddListener(() => doSomething(q1_Local.tag));

        Button q1_Antonio = bttn_Antonio.GetComponent<Button>();
        q1_Antonio.onClick.AddListener(() => doSomething(q1_Antonio.tag));

        Button q1_Maria = bttn_Maria.GetComponent<Button>();
        q1_Maria.onClick.AddListener(() => doSomething(q1_Maria.tag));

        Button q1_Manuela = bttn_Manuela.GetComponent<Button>();
        q1_Manuela.onClick.AddListener(() => doSomething(q1_Manuela.tag));

        Button q1_Paula = bttn_Paula.GetComponent<Button>();
        q1_Paula.onClick.AddListener(() => doSomething(q1_Paula.tag));

        Button q1_Joaquim = bttn_Joaquim.GetComponent<Button>();
        q1_Joaquim.onClick.AddListener(() => doSomething(q1_Joaquim.tag));

        Button q1_Diogo = bttn_Diogo.GetComponent<Button>();
        q1_Diogo.onClick.AddListener(() => doSomething(q1_Diogo.tag));
    }

    void doSomething(string inString)
    {
        if(inString.Equals("q1_Local"))
            GlobalVariables.q1_SelectedSuspect = "Local Do Crime";
        else if (inString.Equals("q1_Antonio"))
            GlobalVariables.q1_SelectedSuspect = "Antonio";
        else if (inString.Equals("q1_Maria"))
            GlobalVariables.q1_SelectedSuspect = "Maria";
        else if (inString.Equals("q1_Manuela"))
            GlobalVariables.q1_SelectedSuspect = "Manuela";
        else if (inString.Equals("q1_Paula"))
            GlobalVariables.q1_SelectedSuspect = "Paula";
        else if (inString.Equals("q1_Joaquim"))
            GlobalVariables.q1_SelectedSuspect = "Joaquim";
        else if (inString.Equals("q1_Diogo"))
            GlobalVariables.q1_SelectedSuspect = "Diogo";
    }
}