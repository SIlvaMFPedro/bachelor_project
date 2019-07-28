using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class q2_buttonClick : MonoBehaviour
{
    private Text[] texts;
    private Text selectedSuspect, conclusao;
    public Button bttn_diziaTer_Amido, bttn_diziaTer_Giz, bttn_diziaTer_Gesso, bttn_diziaTer_Sodio, bttn_diziaTer_Magnesio, bttn_diziaTer_Frutose, bttn_diziaTer_Sal;
    public Button bttn_Tem_Amido, bttn_Tem_Giz, bttn_Tem_Gesso, bttn_Tem_Sodio, bttn_Tem_Magnesio, bttn_Tem_Frutose, bttn_Tem_Sal;
    public Button bttn_Alibi_Sim, bttn_Alibi_Nao;
    public Button bttn_Voltar, bttn_Verdade, bttn_Mentira;

    private int suspect = -1;
    private int flag = 0;
    private int resetVariables = 0;
    private int size = 7;
    // Use this for initialization
    void Start()
    {
        texts = this.gameObject.GetComponentsInChildren<Text>();
        selectedSuspect = texts[0];

        Button q2_diziaTer_Amido = bttn_diziaTer_Amido.GetComponent<Button>();
        q2_diziaTer_Amido.onClick.AddListener(() => doSomething(q2_diziaTer_Amido.tag));
        Button q2_diziaTer_Giz = bttn_diziaTer_Giz.GetComponent<Button>();
        q2_diziaTer_Giz.onClick.AddListener(() => doSomething(q2_diziaTer_Giz.tag));
        Button q2_diziaTer_Gesso = bttn_diziaTer_Gesso.GetComponent<Button>();
        q2_diziaTer_Gesso.onClick.AddListener(() => doSomething(q2_diziaTer_Gesso.tag));
        Button q2_diziaTer_Sodio = bttn_diziaTer_Sodio.GetComponent<Button>();
        q2_diziaTer_Sodio.onClick.AddListener(() => doSomething(q2_diziaTer_Sodio.tag));
        Button q2_diziaTer_Magnesio = bttn_diziaTer_Magnesio.GetComponent<Button>();
        q2_diziaTer_Magnesio.onClick.AddListener(() => doSomething(q2_diziaTer_Magnesio.tag));
        Button q2_diziaTer_Frutose = bttn_diziaTer_Frutose.GetComponent<Button>();
        q2_diziaTer_Frutose.onClick.AddListener(() => doSomething(q2_diziaTer_Frutose.tag));
        Button q2_diziaTer_Sal = bttn_diziaTer_Sal.GetComponent<Button>();
        q2_diziaTer_Sal.onClick.AddListener(() => doSomething(q2_diziaTer_Sal.tag));

        Button q2_tem_Amido = bttn_Tem_Amido.GetComponent<Button>();
        q2_tem_Amido.onClick.AddListener(() => doSomething(q2_tem_Amido.tag));
        Button q2_tem_Giz = bttn_Tem_Giz.GetComponent<Button>();
        q2_tem_Giz.onClick.AddListener(() => doSomething(q2_tem_Giz.tag));
        Button q2_tem_Gesso = bttn_Tem_Gesso.GetComponent<Button>();
        q2_tem_Gesso.onClick.AddListener(() => doSomething(q2_tem_Gesso.tag));
        Button q2_tem_Sodio = bttn_Tem_Sodio.GetComponent<Button>();
        q2_tem_Sodio.onClick.AddListener(() => doSomething(q2_tem_Sodio.tag));
        Button q2_tem_Magnesio = bttn_Tem_Magnesio.GetComponent<Button>();
        q2_tem_Magnesio.onClick.AddListener(() => doSomething(q2_tem_Magnesio.tag));
        Button q2_tem_Frutose = bttn_Tem_Frutose.GetComponent<Button>();
        q2_tem_Frutose.onClick.AddListener(() => doSomething(q2_tem_Frutose.tag));
        Button q2_tem_Sal = bttn_Tem_Sal.GetComponent<Button>();
        q2_tem_Sal.onClick.AddListener(() => doSomething(q2_tem_Sal.tag));

        Button q2_alibi_Sim = bttn_Alibi_Sim.GetComponent<Button>();
        q2_alibi_Sim.onClick.AddListener(() => doSomething(q2_alibi_Sim.tag));
        Button q2_alibi_Nao = bttn_Alibi_Nao.GetComponent<Button>();
        q2_alibi_Nao.onClick.AddListener(() => doSomething(q2_alibi_Nao.tag));

        Button q2_voltar = bttn_Voltar.GetComponent<Button>();
        q2_voltar.onClick.AddListener(() => doSomething(q2_voltar.tag));

        Button q2_Mentira = bttn_Mentira.GetComponent<Button>();
        q2_Mentira.onClick.AddListener(() => doSomething(q2_Mentira.tag));
        Button q2_Verdade = bttn_Verdade.GetComponent<Button>();
        q2_Verdade.onClick.AddListener(() => doSomething(q2_Verdade.tag));

    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.activeInHierarchy)
        {
            selectedSuspect.text = GlobalVariables.q1_SelectedSuspect;
            if (selectedSuspect.text.Equals("Local Do Crime"))
            {
                suspect = 0;
                bttn_Alibi_Nao.interactable = false;
                bttn_Alibi_Sim.interactable = false;
                bttn_diziaTer_Amido.interactable = false;
                bttn_diziaTer_Frutose.interactable = false;
                bttn_diziaTer_Gesso.interactable = false;
                bttn_diziaTer_Giz.interactable = false;
                bttn_diziaTer_Magnesio.interactable = false;
                bttn_diziaTer_Sal.interactable = false;
                bttn_diziaTer_Sodio.interactable = false;
                bttn_Mentira.interactable = false;
                bttn_Verdade.interactable = false;
            }
            else if (selectedSuspect.text.Equals("Antonio"))
            {
                suspect = 1;
                bttn_Alibi_Nao.interactable = true;
                bttn_Alibi_Sim.interactable = true;
                bttn_diziaTer_Amido.interactable = true;
                bttn_diziaTer_Frutose.interactable = true;
                bttn_diziaTer_Gesso.interactable = true;
                bttn_diziaTer_Giz.interactable = true;
                bttn_diziaTer_Magnesio.interactable = true;
                bttn_diziaTer_Sal.interactable = true;
                bttn_diziaTer_Sodio.interactable = true;
                bttn_Mentira.interactable = true;
                bttn_Verdade.interactable = true;
            }
            else if (selectedSuspect.text.Equals("Maria"))
            {
                suspect = 2;
                bttn_Alibi_Nao.interactable = true;
                bttn_Alibi_Sim.interactable = true;
                bttn_diziaTer_Amido.interactable = true;
                bttn_diziaTer_Frutose.interactable = true;
                bttn_diziaTer_Gesso.interactable = true;
                bttn_diziaTer_Giz.interactable = true;
                bttn_diziaTer_Magnesio.interactable = true;
                bttn_diziaTer_Sal.interactable = true;
                bttn_diziaTer_Sodio.interactable = true;
                bttn_Mentira.interactable = true;
                bttn_Verdade.interactable = true;
            }
            else if (selectedSuspect.text.Equals("Manuela"))
            {
                suspect = 3;
                bttn_Alibi_Nao.interactable = true;
                bttn_Alibi_Sim.interactable = true;
                bttn_diziaTer_Amido.interactable = true;
                bttn_diziaTer_Frutose.interactable = true;
                bttn_diziaTer_Gesso.interactable = true;
                bttn_diziaTer_Giz.interactable = true;
                bttn_diziaTer_Magnesio.interactable = true;
                bttn_diziaTer_Sal.interactable = true;
                bttn_diziaTer_Sodio.interactable = true;
                bttn_Mentira.interactable = true;
                bttn_Verdade.interactable = true;
            }
            else if (selectedSuspect.text.Equals("Paula"))
            {
                suspect = 4;
                bttn_Alibi_Nao.interactable = true;
                bttn_Alibi_Sim.interactable = true;
                bttn_diziaTer_Amido.interactable = true;
                bttn_diziaTer_Frutose.interactable = true;
                bttn_diziaTer_Gesso.interactable = true;
                bttn_diziaTer_Giz.interactable = true;
                bttn_diziaTer_Magnesio.interactable = true;
                bttn_diziaTer_Sal.interactable = true;
                bttn_diziaTer_Sodio.interactable = true;
                bttn_Mentira.interactable = true;
                bttn_Verdade.interactable = true;
            }
            else if (selectedSuspect.text.Equals("Joaquim"))
            {
                suspect = 5;
                bttn_Alibi_Nao.interactable = true;
                bttn_Alibi_Sim.interactable = true;
                bttn_diziaTer_Amido.interactable = true;
                bttn_diziaTer_Frutose.interactable = true;
                bttn_diziaTer_Gesso.interactable = true;
                bttn_diziaTer_Giz.interactable = true;
                bttn_diziaTer_Magnesio.interactable = true;
                bttn_diziaTer_Sal.interactable = true;
                bttn_diziaTer_Sodio.interactable = true;
                bttn_Mentira.interactable = true;
                bttn_Verdade.interactable = true;
            }
            else if (selectedSuspect.text.Equals("Diogo"))
            {
                suspect = 6;
                bttn_Alibi_Nao.interactable = true;
                bttn_Alibi_Sim.interactable = true;
                bttn_diziaTer_Amido.interactable = true;
                bttn_diziaTer_Frutose.interactable = true;
                bttn_diziaTer_Gesso.interactable = true;
                bttn_diziaTer_Giz.interactable = true;
                bttn_diziaTer_Magnesio.interactable = true;
                bttn_diziaTer_Sal.interactable = true;
                bttn_diziaTer_Sodio.interactable = true;
                bttn_Mentira.interactable = true;
                bttn_Verdade.interactable = true;
            }
        }

    }

    void doSomething(string liro)
    {

        if (liro.Equals("q2_Amido"))
        {
            GlobalVariables.q2_DiziaTer_Amido[suspect] = 1;
            resetVariables = 1;
            flag = 1;
        }
        else if (liro.Equals("q2_Giz"))
        {
            GlobalVariables.q2_DiziaTer_Giz[suspect] = 1;
            resetVariables = 1;
            flag = 2;
        }
        else if (liro.Equals("q2_Gesso"))
        {
            GlobalVariables.q2_DiziaTer_Gesso[suspect] = 1;
            resetVariables = 1;
            flag = 3;
        }
        else if (liro.Equals("q2_Sodio"))
        {
            GlobalVariables.q2_DiziaTer_Sodio[suspect] = 1;
            resetVariables = 1;
            flag = 4;
        }
        else if (liro.Equals("q2_Magnesio"))
        {
            GlobalVariables.q2_DiziaTer_Magnesio[suspect] = 1;
            resetVariables = 1;
            flag = 5;
        }
        else if (liro.Equals("q2_Frutose"))
        {
            GlobalVariables.q2_DiziaTer_Frutose[suspect] = 1;
            resetVariables = 1;
            flag = 6;
        }
        else if (liro.Equals("q2_Sal"))
        {
            GlobalVariables.q2_DiziaTer_Sal[suspect] = 1;
            resetVariables = 1;
            flag = 7;
        }
        else if (liro.Equals("q2_Tem_Amido"))
        {
            GlobalVariables.q2_Tem_Amido[suspect] = 1;
            resetVariables = 2;
            flag = 1;
        }
        else if (liro.Equals("q2_Tem_Giz"))
        {
            GlobalVariables.q2_Tem_Giz[suspect] = 1;
            resetVariables = 2;
            flag = 2;
        }
        else if (liro.Equals("q2_Tem_Gesso"))
        {
            GlobalVariables.q2_Tem_Gesso[suspect] = 1;
            resetVariables = 2;
            flag = 3;
        }
        else if (liro.Equals("q2_Tem_Sodio"))
        {
            GlobalVariables.q2_Tem_Sodio[suspect] = 1;
            resetVariables = 2;
            flag = 4;
        }
        else if (liro.Equals("q2_Tem_Magnesio"))
        {
            GlobalVariables.q2_Tem_Magnesio[suspect] = 1;
            resetVariables = 2;
            flag = 5;
        }
        else if (liro.Equals("q2_Tem_Frutose"))
        {
            GlobalVariables.q2_Tem_Frutose[suspect] = 1;
            resetVariables = 2;
            flag = 6;
        }
        else if (liro.Equals("q2_Tem_Sal"))
        {
            GlobalVariables.q2_Tem_Sal[suspect] = 1;
            resetVariables = 2;
            flag = 7;
        }
        else if (liro.Equals("q2_Sim"))
            GlobalVariables.q2_Alibi[suspect] = 1;
        else if (liro.Equals("q2_Nao"))
            GlobalVariables.q2_Alibi[suspect] = 0;
        else if (liro.Equals("q2_Mentira"))
            GlobalVariables.q2_Conclusao[suspect] = 0;
        else if (liro.Equals("q2_Verdade"))
            GlobalVariables.q2_Conclusao[suspect] = 1;


        // resetVariables: 1 - DiziaTer ; 2 - Tem
        // flag: 1 - Amido ; 2 - Giz ; 3 - Gesso ; 4 - Sodio ; 5 - Magnesio ; 6 - Frutose ; 7 - Sal
        if(resetVariables == 1)
        {
            for(int i = 0; i < size; i++)
            {
                switch (flag)
                {
                    case 1:
                        GlobalVariables.q2_DiziaTer_Giz[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Gesso[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Sodio[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Magnesio[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Frutose[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Sal[suspect] = 0;
                    break;
                    case 2:
                        GlobalVariables.q2_DiziaTer_Amido[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Gesso[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Sodio[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Magnesio[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Frutose[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Sal[suspect] = 0;
                        break;
                    case 3:
                        GlobalVariables.q2_DiziaTer_Amido[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Giz[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Sodio[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Magnesio[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Frutose[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Sal[suspect] = 0;
                        break;
                    case 4:
                        GlobalVariables.q2_DiziaTer_Amido[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Giz[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Gesso[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Magnesio[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Frutose[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Sal[suspect] = 0;
                        break;
                    case 5:
                        GlobalVariables.q2_DiziaTer_Amido[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Giz[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Gesso[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Sodio[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Frutose[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Sal[suspect] = 0;
                        break;
                    case 6:
                        GlobalVariables.q2_DiziaTer_Amido[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Giz[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Gesso[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Sodio[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Magnesio[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Sal[suspect] = 0;
                        break;
                    case 7:
                        GlobalVariables.q2_DiziaTer_Amido[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Giz[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Gesso[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Sodio[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Magnesio[suspect] = 0;
                        GlobalVariables.q2_DiziaTer_Frutose[suspect] = 0;
                        break;
                }
            }
            resetVariables = 0;
            flag = 0;
        }else if(resetVariables == 2)
        {
            for (int i = 0; i < size; i++)
            {
                switch (flag)
                {
                    case 1:
                        GlobalVariables.q2_Tem_Giz[suspect] = 0;
                        GlobalVariables.q2_Tem_Gesso[suspect] = 0;
                        GlobalVariables.q2_Tem_Sodio[suspect] = 0;
                        GlobalVariables.q2_Tem_Magnesio[suspect] = 0;
                        GlobalVariables.q2_Tem_Frutose[suspect] = 0;
                        GlobalVariables.q2_Tem_Sal[suspect] = 0;
                        break;
                    case 2:
                        GlobalVariables.q2_Tem_Amido[suspect] = 0;
                        GlobalVariables.q2_Tem_Gesso[suspect] = 0;
                        GlobalVariables.q2_Tem_Sodio[suspect] = 0;
                        GlobalVariables.q2_Tem_Magnesio[suspect] = 0;
                        GlobalVariables.q2_Tem_Frutose[suspect] = 0;
                        GlobalVariables.q2_Tem_Sal[suspect] = 0;
                        break;
                    case 3:
                        GlobalVariables.q2_Tem_Amido[suspect] = 0;
                        GlobalVariables.q2_Tem_Giz[suspect] = 0;
                        GlobalVariables.q2_Tem_Sodio[suspect] = 0;
                        GlobalVariables.q2_Tem_Magnesio[suspect] = 0;
                        GlobalVariables.q2_Tem_Frutose[suspect] = 0;
                        GlobalVariables.q2_Tem_Sal[suspect] = 0;
                        break;
                    case 4:
                        GlobalVariables.q2_Tem_Amido[suspect] = 0;
                        GlobalVariables.q2_Tem_Giz[suspect] = 0;
                        GlobalVariables.q2_Tem_Gesso[suspect] = 0;
                        GlobalVariables.q2_Tem_Magnesio[suspect] = 0;
                        GlobalVariables.q2_Tem_Frutose[suspect] = 0;
                        GlobalVariables.q2_Tem_Sal[suspect] = 0;
                        break;
                    case 5:
                        GlobalVariables.q2_Tem_Amido[suspect] = 0;
                        GlobalVariables.q2_Tem_Giz[suspect] = 0;
                        GlobalVariables.q2_Tem_Gesso[suspect] = 0;
                        GlobalVariables.q2_Tem_Sodio[suspect] = 0;
                        GlobalVariables.q2_Tem_Frutose[suspect] = 0;
                        GlobalVariables.q2_Tem_Sal[suspect] = 0;
                        break;
                    case 6:
                        GlobalVariables.q2_Tem_Amido[suspect] = 0;
                        GlobalVariables.q2_Tem_Giz[suspect] = 0;
                        GlobalVariables.q2_Tem_Gesso[suspect] = 0;
                        GlobalVariables.q2_Tem_Sodio[suspect] = 0;
                        GlobalVariables.q2_Tem_Magnesio[suspect] = 0;
                        GlobalVariables.q2_Tem_Sal[suspect] = 0;
                        break;
                    case 7:
                        GlobalVariables.q2_Tem_Amido[suspect] = 0;
                        GlobalVariables.q2_Tem_Giz[suspect] = 0;
                        GlobalVariables.q2_Tem_Gesso[suspect] = 0;
                        GlobalVariables.q2_Tem_Sodio[suspect] = 0;
                        GlobalVariables.q2_Tem_Magnesio[suspect] = 0;
                        GlobalVariables.q2_Tem_Frutose[suspect] = 0;
                        break;
                }
            }
            resetVariables = 0;
            flag = 0;
        }
    }
}
