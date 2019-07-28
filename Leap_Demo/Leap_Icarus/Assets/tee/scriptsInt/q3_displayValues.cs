using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class q3_displayValues : MonoBehaviour
{

    Text[] c;
    private int size = 7;
    private int suspect = 12;
    // Use this for initialization
    void Start()
    {
        c = this.gameObject.GetComponentsInChildren<Text>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.activeInHierarchy)
        {
           suspect = 12; // Antonio começa aqui
            for (int i = 1; i < size; i++)
            {
                if (GlobalVariables.q2_DiziaTer_Amido[i] == 1)
                    c[suspect].text = "Amido";
                else if (GlobalVariables.q2_DiziaTer_Giz[i] == 1)
                    c[suspect].text = "Giz";
                else if (GlobalVariables.q2_DiziaTer_Gesso[i] == 1)
                    c[suspect].text = "Gesso";
                else if (GlobalVariables.q2_DiziaTer_Sodio[i] == 1)
                    c[suspect].text = "Sodio";
                else if (GlobalVariables.q2_DiziaTer_Magnesio[i] == 1)
                    c[suspect].text = "Magnesio";
                else if (GlobalVariables.q2_DiziaTer_Frutose[i] == 1)
                    c[suspect].text = "Frutose";
                else if (GlobalVariables.q2_DiziaTer_Sal[i] == 1)
                    c[suspect].text = "Sal";

                suspect += 1;

                if (GlobalVariables.q2_Tem_Amido[i] == 1)
                    c[suspect].text = "Amido";
                else if (GlobalVariables.q2_Tem_Giz[i] == 1)
                    c[suspect].text = "Giz";
                else if (GlobalVariables.q2_Tem_Gesso[i] == 1)
                    c[suspect].text = "Gesso";
                else if (GlobalVariables.q2_Tem_Sodio[i] == 1)
                    c[suspect].text = "Sodio";
                else if (GlobalVariables.q2_Tem_Magnesio[i] == 1)
                    c[suspect].text = "Magnesio";
                else if (GlobalVariables.q2_Tem_Frutose[i] == 1)
                    c[suspect].text = "Frutose";
                else if (GlobalVariables.q2_Tem_Sal[i] == 1)
                    c[suspect].text = "Sal";

                suspect += 1;

                if (GlobalVariables.q2_Alibi[i] == 1)
                    c[suspect].text = "Sim";
                else if (GlobalVariables.q2_Alibi[i] == 0)
                    c[suspect].text = "Nao";

                suspect += 1;

                if (GlobalVariables.q2_Conclusao[i] == 0)
                    c[suspect].text = "Mentira";
                else if (GlobalVariables.q2_Conclusao[i] == 1)
                    c[suspect].text = "Verdade";

                suspect += 1;
            }
            if (GlobalVariables.q2_Tem_Amido[0] == 1)
                c[suspect].text = "Amido";
            else if (GlobalVariables.q2_Tem_Giz[0] == 1)
                c[suspect].text = "Giz";
            else if (GlobalVariables.q2_Tem_Gesso[0] == 1)
                c[suspect].text = "Gesso";
            else if (GlobalVariables.q2_Tem_Sodio[0] == 1)
                c[suspect].text = "Bicarbonato de Sodio";
            else if (GlobalVariables.q2_Tem_Magnesio[0] == 1)
                c[suspect].text = "Sulfato de Magnesio";
            else if (GlobalVariables.q2_Tem_Frutose[0] == 1)
                c[suspect].text = "Frutose";
            else if (GlobalVariables.q2_Tem_Sal[0] == 1)
                c[suspect].text = "Sal";
        }
    }
}