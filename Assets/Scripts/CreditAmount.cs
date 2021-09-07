using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class CreditAmount : MonoBehaviourPun
{
    public int credit;
    private TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    public void Add(int Addedcredit)
    {
        credit += Addedcredit;

        text.text = credit.ToString();

        credit = 0;
    }
}
