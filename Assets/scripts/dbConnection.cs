using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using TMPro;
using System.Linq;
public class dbConnection : MonoBehaviour
{
    public GameObject first;
   
    public List<Activity> activities;
    public Transform content;
    public GameObject activityPrefab;
    public TextMeshPro activityLabel;
    [SerializeField] Data data;
    public TMP_InputField activityTitle;
    public TMP_InputField duration;
    public TMP_Dropdown priority;
    public DatabaseReference db;
    public TMP_InputField email;
    AuthManager authscript;
    public GameObject authManager;
    public string user_id;
    public GameObject actTab;
    private bool dataExists;
    public TMP_InputField durationField;
    public GameObject setDurationWindow;
    public string idForEdited;
    public TextMeshProUGUI activityError;
    public TextMeshProUGUI durationError;
    public TextMeshProUGUI taskAddedPrompt;
    public TextMeshProUGUI setDurationPromptError;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("before");
        db = FirebaseDatabase.GetInstance("https://productivity-app-7a77c-default-rtdb.firebaseio.com/").RootReference;
        Debug.Log("after");
        authscript = authManager.GetComponent<AuthManager>();
        user_id = authscript.User.UserId;
        Debug.Log(user_id);
        StartCoroutine(fetchActivitiess(true));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void addTask()
    {
       
        StartCoroutine(addActivity());
        
        

        
       
        // Debug.Log(userId);
        Debug.Log(JsonUtility.ToJson(this.data.activities));
        Debug.Log("task added");
    }
    public void fetchActivities()
    {
        StartCoroutine(fetchActivitiess(true));
    }
    private IEnumerator addActivity()
    {
      
        var userref = db.Child("users").Child(this.user_id).GetValueAsync();
        yield return new WaitUntil(predicate: () => userref.IsCompleted);
        if (userref.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to fetch data with{userref.Exception} ");
        }
        else if (userref.Result.Value == null)
        {
            taskAddedPrompt.text = "";
            string priorityVal = priority.options[priority.value].text;
            List<int> arr = new List<int>();
            arr.Add(0);
            bool activityNotEmpty = true;
            bool durationNotEmpty = true;
            bool validDuration = true;
            // input validation
            if (activityTitle.text == "")
            {
                activityError.text = "Please enter activity";
                activityNotEmpty = false;
            }
            else {
                activityError.text = "";
            }
            if (durationField.text == "")
            {
                durationError.text = "Please enter a duration";
                durationNotEmpty = false;
            }
            else {
                durationError.text = "";
            }
            if (int.TryParse(duration.text, out int durationVal))
            {
                durationError.text = "";

            }
            else
            {
                durationError.text = "Invalid duration amount";
                validDuration = false;
            }
            if (validDuration && activityNotEmpty && durationNotEmpty)
            {
                Activity activity = new Activity(0, activityTitle.text, durationVal, priorityVal, 0, arr);
                Activity[] acts = { activity };
                data = new Data(1, acts);
                string json = JsonUtility.ToJson(this.data);
                db.Child("users").Child(user_id).SetRawJsonValueAsync(json);
                taskAddedPrompt.text = "task added successfully";
            }

        }
        else
        {

            taskAddedPrompt.text = "";
            DataSnapshot result = userref.Result;
            string x = System.Convert.ToString(result.Child("maxId").Value) ;
            int i = int.Parse(x);
            int y = 1 + i;
            string maxId = y.ToString();
            string priorityVal = priority.options[priority.value].text;
            List<int> arr = new List<int>();
            arr.Add(0);
            bool activityNotEmpty = true;
            bool durationIsNotEmpty = true;
            bool validDuration = true;
            // input validation
            if (activityTitle.text == "")
            {
                activityError.text = "Please enter activity";
                activityNotEmpty = false;
            }
            else {
                activityError.text = "";
            }
            if (duration.text == "")
            {
                durationError.text = "Please enter a duration";
                durationIsNotEmpty = false;
                Debug.Log("duration emptyyy");

            }
            else {
                durationError.text = "";
            }
            if (int.TryParse(duration.text, out int durationVal))
            {
                durationError.text = "";

            }
            else {
                durationError.text = "Invalid duration amount";
                validDuration = false;
            }
            
            if (validDuration & activityNotEmpty & durationIsNotEmpty)
            {
                Debug.Log("working");
                Activity activity = new Activity(y, activityTitle.text, durationVal, priorityVal, 0, arr);
                string json = JsonUtility.ToJson(activity);
                db.Child("users").Child(this.user_id).Child("activities").Child(maxId).SetRawJsonValueAsync(json);

                db.Child("users").Child(this.user_id).Child("maxId").SetValueAsync(y);
                var acts = result.Child("activities").Child("0").Child("activityTitle").Value;
                taskAddedPrompt.text = "task added successfully";
                duration.text = "";
                activityTitle.text = "";
                StartCoroutine(fetchActivitiess(false));
            }

        }
        

    }


    private IEnumerator fetchActivitiess(bool disp)
    {
       

        if (disp) {
            taskAddedPrompt.text = "";
        }
        activities.Clear();
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

        }
        else
        {
            
            DataSnapshot snapshot =userref.Result.Child("activities");
            // Do something with snapshot...
            
            foreach (DataSnapshot s in snapshot.Children)
            {

               
                IDictionary dictUsers = s.Value as IDictionary;
                List<int> ints = new List<int>();
                foreach (object o in (IEnumerable)dictUsers["history"]) {
                  
                    ints.Add(int.Parse(o.ToString()));
                }

                Activity activity = new Activity(int.Parse(dictUsers["id"].ToString()), dictUsers["activityTitle"].ToString(), int.Parse(dictUsers["duration"].ToString()), dictUsers["priority"].ToString(),int.Parse(dictUsers["finished_duration"].ToString()), ints);
                activities.Add(activity);
                
              

            }
            Debug.Log("after false active");

            actTab.SetActive(false);
            actTab.SetActive(disp);
            

        }

    }

    public void setDuration(string actID) {
        bool durationNotEmpty = true;
        bool durationValid = true;
        if (durationField.text == "")
        {
            setDurationPromptError.text = "Please enter a duration";
            durationNotEmpty = false;
            durationField.text = "";

        }
        else
        {
            if (int.TryParse(durationField.text, out int durationValue))
            {
                setDurationPromptError.text = "";

            }
            else
            {
                setDurationPromptError.text = "Invalid duration amount";
                durationValid = false;
                durationField.text = "";
            }
        }
        
        if (durationNotEmpty & durationValid) {
            db.Child("users").Child(this.user_id).Child("activities").Child(idForEdited).Child("finished_duration").SetRawJsonValueAsync(durationField.text);
            setDurationWindow.SetActive(false);
            StartCoroutine(fetchActivitiess(true));
            durationField.text = "";
        }
        

    }
    public void doneDay() {
        foreach (Activity activity in this.activities) {
            activity.addToHistory(activity.finished_duration);
            activity.finished_duration = 0;
            db.Child("users").Child(this.user_id).Child("activities").Child(activity.id.ToString()).SetRawJsonValueAsync(JsonUtility.ToJson(activity));

        }
        fetchActivities();

    }
}
[System.Serializable]
public class Activity {
    public int id;
    public string activityTitle;
    public int duration;
    public int finished_duration;
    public string priority;
    public List<int> history;
    public Activity() {
    }
    public Activity(int id, string activity, int duration, string priority, int finished_duration, List<int> history) {
        this.id = id;
        this.activityTitle = activity;
        this.duration = duration;
        this.priority = priority;
        this.finished_duration = finished_duration;
        this.history = history;
    }
    public void setDuration(int finished_duration) {
        this.finished_duration = finished_duration;
    }
    public void addToHistory(int dur) {
        this.history.Add(dur);
    }
   
}
[System.Serializable]
class Data {
    public Activity[] activities;
    public int maxId;
    public Data() {
    }
    public Data(int maxId, Activity[] activities) {
        this.maxId = maxId;
        this.activities = activities;
    }
}