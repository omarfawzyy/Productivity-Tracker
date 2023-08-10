using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using TMPro;
using System.Linq;
public class DataController : MonoBehaviour
{
   //UserData model
    private List<Activity> activities;
    private Data data;
    
    public DatabaseReference db;
    AuthManager authscript;
    public GameObject authManager;
    public string user_id;
    private bool dataExists;
    public string idForEdited;
    private Dictionary<int,string> activitiesInfo;

    //singleton implementation
    public static DataController Instance;

    //variables useful to view
    public bool durationValid;
    public bool durationNotEmpty;
    public bool activityNotEmpty;
    public bool durationIsNotEmpty;
    public bool validDuration;
    public bool done;

    // GameObject containing UserData View
    public GameObject lister;
    private bool donee;
    private void Awake()
    {
        // Initiates instance of the class
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            Debug.Log("Boom shakalaka");

        }
        else
        {
            Instance = this;
            Debug.Log("Here we go again");

        }


    }

    void Start()
    {
        // establishes database connection
        Debug.Log("before");
        db = FirebaseDatabase.GetInstance("https://productivity-app-7a77c-default-rtdb.firebaseio.com/").RootReference;
        Debug.Log("after");
        authscript = authManager.GetComponent<AuthManager>();
        user_id = authscript.User.UserId;
        Debug.Log(user_id);
        //fetching activities from database
        StartCoroutine(fetchActivities(true));
    }

   
    public Dictionary<int, string> getActivtiesInfo() {
        activitiesInfo = new Dictionary<int, string>();
        foreach (Activity activity in activities) {
            activitiesInfo.Add(key: activity.id, value: activity.activityTitle);
        }
        return activitiesInfo;
    }

    public string getDuration(int id) {
        foreach (Activity activity in activities) {
            if (activity.id == id)
            {
                return activity.duration.ToString();
            }
           
        }
        return null;
    }
    public string getFinishedDuration(int id)
    {
        foreach (Activity activity in activities)
        {
            if (activity.id == id)
            {
                return activity.finished_duration.ToString();
            }

        }
        return null;
    }
    public void addActivityStarter(string activityTitle, string priority, string duration)
    {

        StartCoroutine(addActivity(activityTitle,priority,duration));
    }
    public void fetchActivitiesStarter()
    {
        StartCoroutine(fetchActivities(true));
    }
    private IEnumerator addActivity(string activityTitle, string priority, string duration)
    {
        done = false;
        var max_Id = db.Child("users").Child(this.user_id).Child("maxId").GetValueAsync();
        yield return new WaitUntil(predicate: () => max_Id.IsCompleted);

        if (max_Id.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to fetch data with{max_Id.Exception} ");
        }
        else if (max_Id.Result.Value == null)
        {
            List<int> arr = new List<int>();
            arr.Add(0);
             activityNotEmpty = true;
             durationIsNotEmpty = true;
             validDuration = true;
            // setting bool variable
            if (activityTitle == "")
            {
                activityNotEmpty = false;
            }
            if (duration == "")
            {
                durationIsNotEmpty = false;
            }
            
            if (!int.TryParse(duration, out int durationVal))
            {
                validDuration = false;
            }
            
            if (validDuration && activityNotEmpty && durationIsNotEmpty)
            {
                Activity activity = new Activity(0, activityTitle, durationVal, priority, 0, arr);
                Activity[] acts = { activity };
                data = new Data(1, acts);
                string json = JsonUtility.ToJson(this.data);
                db.Child("users").Child(user_id).SetRawJsonValueAsync(json);
                db.Child("users").Child(this.user_id).Child("maxId").SetRawJsonValueAsync("0");
                StartCoroutine(fetchActivities(false));
            }

        }
        else
        {

            int maxId = int.Parse(max_Id.Result.Value.ToString()) + 1;
            List<int> arr = new List<int>();
            arr.Add(0);
            activityNotEmpty = true;
            durationIsNotEmpty = true;
            validDuration = true;

            // setting bool variables
            if (activityTitle == "")
            {
                activityNotEmpty = false;
            }
            
            if (duration == "")
            {
                durationIsNotEmpty = false;

            }
            
            if (!int.TryParse(duration, out int durationVal))
            {
                validDuration = false;
            }
            

            if (validDuration & activityNotEmpty & durationIsNotEmpty)
            {
                Debug.Log("working");
                Activity activity = new Activity(maxId, activityTitle, durationVal, priority, 0, arr);
                string json = JsonUtility.ToJson(activity);
                Debug.Log(maxId);
                db.Child("users").Child(this.user_id).Child("activities").Child(maxId.ToString()).SetRawJsonValueAsync(json);
                db.Child("users").Child(this.user_id).Child("maxId").SetRawJsonValueAsync(maxId.ToString());
                StartCoroutine(fetchActivities(false));
            }

        }
        done = true;
        Debug.Log("first");

    }


    private IEnumerator fetchActivities(bool disp)
    {


        donee = false;
        activities = new List<Activity>();
        Debug.Log("after");
        var userref = db.Child("users").Child(this.user_id).GetValueAsync();
        yield return new WaitUntil(predicate: () => userref.IsCompleted);
        yield return new WaitUntil(predicate: () => userref.IsCompleted);
        Debug.Log("before");

        if (userref.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to fetch data with{userref.Exception} ");
        }
        else if (userref.Result.Value == null)
        {

            Debug.Log("empty records");
            lister.GetComponent<Lister>().Refresh();

        }
        else
        {

            DataSnapshot snapshot = userref.Result.Child("activities");
            // Do something with snapshot...
            if (snapshot.ChildrenCount > 0)
            {
                Debug.Log("huh?");
                foreach (DataSnapshot s in snapshot.Children)
                {


                    IDictionary dictUsers = s.Value as IDictionary;
                    List<int> ints = new List<int>();
                    foreach (object o in (IEnumerable)dictUsers["history"])
                    {
                        Debug.Log(dictUsers["id"].ToString());
                        ints.Add(int.Parse(o.ToString()));
                    }

                    Activity activity = new Activity(int.Parse(dictUsers["id"].ToString()), dictUsers["activityTitle"].ToString(), int.Parse(dictUsers["duration"].ToString()), dictUsers["priority"].ToString(), int.Parse(dictUsers["finished_duration"].ToString()), ints);
                    activities.Add(activity);
                    donee = true;


                }

                Debug.Log("after false active");
                lister.GetComponent<Lister>().Refresh();
            }
            else { Debug.Log("it dont exists bruv"); }
            
           

        }

    }
    public bool isEmpty() {
        if (activities.Count == 0 & donee) {
            return true;
        }
        else {
            return false;
        }
    }
    public void setDuration(string durationAmount)
    {
        durationNotEmpty = true;
        durationValid = true;
        if (durationAmount == "")
        {
            //setDurationPromptError.text = "Please enter a duration";
            durationNotEmpty = false;
        }
        else
        {
            if (int.TryParse(durationAmount, out int durationValue))
            {
                //setDurationPromptError.text = "";

            }
            else
            {
                //setDurationPromptError.text = "Invalid duration amount";
                durationValid = false;
                
            }
        }

        if (durationNotEmpty & durationValid)
        {
            db.Child("users").Child(this.user_id).Child("activities").Child(idForEdited).Child("finished_duration").SetRawJsonValueAsync(durationAmount);
            StartCoroutine(fetchActivities(true));
            
        }


    }
    public void doneDay()
    {
        foreach (Activity activity in this.activities)
        {
            activity.addToHistory(activity.finished_duration);
            activity.finished_duration = 0;
            db.Child("users").Child(this.user_id).Child("activities").Child(activity.id.ToString()).SetRawJsonValueAsync(JsonUtility.ToJson(activity));

        }
        fetchActivitiesStarter();

    }
    public List<int> getHistory(int id)
    {
        foreach (Activity activity in this.activities) {
            if (activity.id == id) {
                return activity.history;
            }
        }
        return null;
    }
}
[System.Serializable]
public class Activity
{
    public int id;
    public string activityTitle;
    public int duration;
    public int finished_duration;
    public string priority;
    public List<int> history;
    public Activity()
    {
    }
    public Activity(int id, string activity, int duration, string priority, int finished_duration, List<int> history)
    {
        this.id = id;
        this.activityTitle = activity;
        this.duration = duration;
        this.priority = priority;
        this.finished_duration = finished_duration;
        this.history = history;
    }
    public void setDuration(int finished_duration)
    {
        this.finished_duration = finished_duration;
    }
    public void addToHistory(int dur)
    {
        this.history.Add(dur);
    }
    
}
[System.Serializable]
class Data
{
    public Activity[] activities;
    public int maxId;
    public Data()
    {
    }
    public Data(int maxId, Activity[] activities)
    {
        this.maxId = maxId;
        this.activities = activities;
    }
}