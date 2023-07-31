using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject LoginUI;
    public GameObject RegisterUI;
    public GameObject Activities;
    public GameObject durationTab;

    public static UIManager instance;
    private void Start()
    {
        Debug.Log("this one worked");
    }
    public void loginScreen()
    {
        LoginUI.SetActive(true);
        RegisterUI.SetActive(false);
    }

    public void registerScreen()
    {
        LoginUI.SetActive(false);
        RegisterUI.SetActive(true);
    }
    public void addAct()
    {
        Activities.SetActive(false);
    }
    public void goAct() {
        Activities.SetActive(true);
    }
    public void DurationOff() {
        durationTab.SetActive(false);
    }
}
