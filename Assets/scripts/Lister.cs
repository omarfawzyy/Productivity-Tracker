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
    public GameObject setDurationWindow;
    public TMP_InputField durationField;
    string idForSetDuration;
    dbConnection connection;
    //void Start()
    //{
    //    displayActivities(); 
    //}
    public void displayActivities()
    {
        connection = dbConnection.Instance;

        Debug.Log(dbConnection.number);
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

        List<Activity> activities = connection.activities;
        Debug.Log("ran");

        foreach (Activity activity in activities)
        {
            Debug.Log("ran");
            temp = Instantiate(btntemp, transform);
            temp.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = activity.activityTitle;
            temp.transform.GetChild(0).GetComponent<Button>().AddEventListener(activity.id, handleDelete);
            temp.transform.GetChild(1).GetComponent<Button>().AddEventListener(activity.id, showSetDuration);
            string durationPortion = activity.finished_duration.ToString() + " / " + activity.duration.ToString();
            temp.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = durationPortion;
            float percentage = (float)activity.finished_duration / (float)activity.duration;
            if (percentage > 1) {
                percentage = 1;
            }
            temp.transform.GetChild(2).GetComponent<Image>().fillAmount = percentage;
            Debug.Log("percentage:");
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
        connection.db.Child("users").Child(connection.user_id).Child("activities").Child(id.ToString()).SetRawJsonValueAsync(null);
        Debug.Log(id.ToString());
       connection.fetchActivities();
        displayActivities();
    }

    void showSetDuration(int id)
    {
        connection = dbConnection.Instance;
        setDurationWindow.SetActive(true);
        idForSetDuration = id.ToString();
        Debug.Log("first:");
        Debug.Log(idForSetDuration);
        connection.idForEdited = idForSetDuration;
    }
    
}