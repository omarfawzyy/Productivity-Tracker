using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public static class ButtonExtension
{
    public static void AddEventListener<T>(this Button button, T param, Action<T> OnClick) {
        button.onClick.AddListener(delegate (){
            OnClick(param);
	});
    }
}
public class Lister : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI text;
    public GameObject dbmanager;
    public GameObject setDurationWindow;
    public TMP_InputField durationField;
    string idForSetDuration;
    //void Start()
    //{
    //    displayActivities(); 
    //}
    public void displayActivities()
    {
        if (transform.childCount > 1)
        {
            for (int i = 0; i < transform.childCount; i++)
            {

                if (i + 1 < transform.childCount)
                {
                    Destroy(transform.GetChild(1 + i).gameObject);
                }


            }
        }
        GameObject btntemp = transform.GetChild(0).gameObject;
        GameObject temp;

        List<Activity> activities = dbmanager.GetComponent<dbConnection>().activities;
        Debug.Log("ran");

        foreach (Activity activity in activities)
        {
            Debug.Log("ran");
            temp = Instantiate(btntemp, transform);
            temp.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = activity.activityTitle;
            temp.transform.GetChild(0).GetComponent<Button>().AddEventListener(activity.id, handleDelete);
            temp.transform.GetChild(1).GetComponent<Button>().AddEventListener(activity.id, showSetDuration);
            float percentage = (float)activity.finished_duration / (float)activity.duration;
            if (percentage > 1) {
                percentage = 1;
            }
            temp.transform.GetChild(2).GetComponent<Image>().fillAmount = percentage;
            Debug.Log("percentage:");
            Debug.Log(percentage);
            temp.SetActive(true);
        }
        //Destroy(btntemp);
        Debug.Log("disp");
    }
    private void OnEnable()
    {
        displayActivities();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void handleDelete(int id)
    {
        dbmanager.GetComponent<dbConnection>().db.Child("users").Child(dbmanager.GetComponent<dbConnection>().user_id).Child("activities").Child(id.ToString()).SetRawJsonValueAsync(null);
        Debug.Log(id.ToString());
        dbmanager.GetComponent<dbConnection>().fetchActivities();
        displayActivities();
    }

    void showSetDuration(int id)
    {
        setDurationWindow.SetActive(true);
        idForSetDuration = id.ToString();
        Debug.Log("first:");
        Debug.Log(idForSetDuration);
        dbmanager.GetComponent<dbConnection>().idForEdited = idForSetDuration;
    }
    
}