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
    public GameObject priorityLabel;
    public GameObject setDurationWindow;
    public TMP_InputField durationField;
    public TMP_InputField activityTitle;
    public TMP_InputField duration;
    public TMP_Dropdown priority;
    public TextMeshProUGUI activityError;
    public TextMeshProUGUI durationError;
    public TextMeshProUGUI taskAddedPrompt;
    public GameObject GraphView;
    public GameObject emptyHistoryPanel;
    string idForSetDuration;
    DataController connection;
    public GameObject emptyactivitiestext;

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

        Dictionary<int, string> activities = connection.getActivtiesInfo();

        if (activities.Count == 0 & activities != null)
        {
            emptyactivitiestext.SetActive(true);
        }
        else {
            emptyactivitiestext.SetActive(false);
        }
        foreach (string pr in new List<string>() { "High", "Medium", "Low" }) {
            bool first = true;
            foreach (KeyValuePair<int, string> activity in activities)
            {   if (connection.getPriority(activity.Key) == pr) {
                    if (first) {
                        GameObject activityLabel = Instantiate(priorityLabel, transform);
                        activityLabel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = pr;
                        activityLabel.gameObject.SetActive(true);
                        activityLabel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().gameObject.SetActive(true);
                    }
                    
                    first = false;
                    temp = Instantiate(btntemp, transform);
                    temp.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = activity.Value;
                    temp.GetComponent<Button>().AddEventListener(activity.Key, viewGraph);
                    temp.transform.GetChild(0).GetComponent<Button>().AddEventListener(activity.Key, handleDelete);
                    temp.transform.GetChild(1).GetComponent<Button>().AddEventListener(activity.Key, showSetDuration);
                    string durationPortion = connection.getFinishedDuration(activity.Key) + " / " + connection.getDuration(activity.Key);
                    temp.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = durationPortion;
                    float percentage = (float)int.Parse(connection.getFinishedDuration(activity.Key)) / (float)int.Parse(connection.getDuration(activity.Key));
                    if (percentage > 1)
                    {
                        percentage = 1;
                    }
                    temp.transform.GetChild(2).GetComponent<Image>().fillAmount = percentage;
                    Debug.Log("percentage:");
                    temp.SetActive(true);
                }
                
            }

        }
        
        //Destroy(btntemp);
        Debug.Log("disp");
    }
    public void Refresh()
    {
        displayActivities();
    }

    void viewGraph(int id) {

        List<int> hist = DataController.Instance.getHistory(id);
        if (hist.Count > 1)
        {
            GraphView.SetActive(true);
            Debug.Log(hist.Count);
            GraphView.GetComponent<Window_Graph>().startGraph(hist, id);
            Debug.Log(hist.Count);

        }
        else {
            emptyHistoryPanel.SetActive(true);
        }
    }
   
    void handleDelete(int id)
    {
        connection.deleteActivity(id);
        displayActivities();
    }

    void showSetDuration(int id)
    {
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
        connection = DataController.Instance;
        connection.addActivityStarter(activityTitle.text.ToString(),priority.options[priority.value].text.ToString(),duration.text.ToString());
        yield return new WaitUntil(predicate: () => connection.done);

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
        else {
            taskAddedPrompt.text = "task not added";
        }

        
    }
    public void emptyPage() {
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
    }
    public void ClearAll() {
        activityError.text = "";
        durationError.text = "";
        taskAddedPrompt.text = "";
        activityTitle.text = "";
        priority.value = 0;
        duration.text = "";
        durationField.text = "";
    }

    
}