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
    public TextMeshProUGUI setDurationPromptError;
    public TextMeshProUGUI text;
    public GameObject setDurationWindow;
    public TMP_InputField durationField;
    public TMP_InputField activityTitle;
    public TMP_InputField duration;
    public TMP_Dropdown priority;
    public TextMeshProUGUI activityError;
    public TextMeshProUGUI durationError;
    public TextMeshProUGUI taskAddedPrompt;

    string idForSetDuration;
    DataController connection;
  
   
    public void displayActivities()
    {
        connection = DataController.Instance;

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

        Dictionary<int,string> activities = connection.getActivtiesInfo();
        Debug.Log("ran");

        foreach(KeyValuePair<int,string> activity in activities)
        {
            Debug.Log("ran");
            temp = Instantiate(btntemp, transform);
            temp.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = activity.Value;
            temp.transform.GetChild(0).GetComponent<Button>().AddEventListener(activity.Key, handleDelete);
            temp.transform.GetChild(1).GetComponent<Button>().AddEventListener(activity.Key, showSetDuration);
            string durationPortion = connection.getFinishedDuration(activity.Key) + " / " + connection.getDuration(activity.Key);
            temp.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = durationPortion;
            float percentage = (float)int.Parse(connection.getFinishedDuration(activity.Key)) / (float)int.Parse(connection.getDuration(activity.Key));
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
    public void Refresh()
    {
        displayActivities();
    }

    // Update is called once per frame
   
    void handleDelete(int id)
    {
        connection.db.Child("users").Child(connection.user_id).Child("activities").Child(id.ToString()).SetRawJsonValueAsync(null);
        Debug.Log(id.ToString());
       connection.fetchActivitiesStarter();
        displayActivities();
    }

    void showSetDuration(int id)
    {
        //connection = dbConnection.Instance;
        setDurationWindow.SetActive(true);
        idForSetDuration = id.ToString();
        Debug.Log("first:");
        Debug.Log(idForSetDuration);
        connection.idForEdited = idForSetDuration;
    }
   public void clickSetDuration() {
        connection.setDuration(durationField.text.ToString());
        Refresh();
        if (!connection.durationValid)
        {
            setDurationPromptError.text = "please enter valid duration";
        }
        else if (!connection.durationNotEmpty)
        {
            setDurationPromptError.text = "please enter a duration";
        }
        else {
            setDurationWindow.SetActive(false);
        }
    }
    public void taskaddedbruh() {
        StartCoroutine(addTaskClicked());
    }
    public IEnumerator addTaskClicked() {
        connection.addActivityStarter(activityTitle.text.ToString(),priority.options[priority.value].text.ToString(),duration.text.ToString());
        yield return new WaitUntil(predicate: () => connection.done);

        Debug.Log("second");
        if (!connection.activityNotEmpty)
        {
            activityError.text = "Please enter activity";
        }
        else
        {
            activityError.text = "";
        }
        if (!connection.durationIsNotEmpty)
        {
            durationError.text = "Please enter a duration";
        }
        else
        {
            durationError.text = "";
            if(!connection.validDuration){
                durationError.text = "duration isn't valid";
            }
        }
       
        
        if (connection.validDuration & connection.activityNotEmpty & connection.durationIsNotEmpty)
        {
            ClearAll();
            taskAddedPrompt.text = "task added successfully";

        }
        
    }
    public void ClearAll() {
        activityError.text = "";
        durationError.text = "";
        taskAddedPrompt.text = "";
        activityTitle.text = "";
        priority.value = 0;
        duration.text = "";
    }

    
}