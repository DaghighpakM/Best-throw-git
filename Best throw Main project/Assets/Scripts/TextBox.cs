using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
    Animator animator;
    public static TextBox Instance;

    public static string INTERNET_CONNECTION_ERROR = "Internet connection error !";
    public static string DATA_RECEIVING_ERROR = "Data receiving error!";
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        animator = GetComponent<Animator>();
    }

    public void ShowTextBox(string text)
    {
        GetComponentInChildren<Text>().text = text;
        animator.SetTrigger("Show");
    }


}
