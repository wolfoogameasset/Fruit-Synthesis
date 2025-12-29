using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class BtnRestart : MonoBehaviour
{
    public Button BtnRestarts;

    public void Awake()
    {
        BtnRestarts = transform.Find("Btn_Continue").GetComponent<Button>();
        OnAddBtnRestart();
    }

    void Start()
    {

        
    }
    public void OnAddBtnRestart()
    {
        Resources.Load("Assets/Resources/Panel/MainPanel.prefab");

    }
    

}
