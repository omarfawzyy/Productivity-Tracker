

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using TMPro;
public class Window_Graph : MonoBehaviour {
    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    public GameObject content;
    DataController datacontroller;
    public TextMeshProUGUI durationDisplay;
    int[] valueListcopy;
    public void clear() {
        durationDisplay.text = "";
    }
    private void Awake() {
        graphContainer = content. GetComponent<RectTransform>();
        datacontroller = DataController.Instance;
    }
    public void startGraph(List<int> activityHist, int id) {
        valueListcopy = activityHist.ToArray();
        valueListcopy = valueListcopy[1..] ;
        int duration = int.Parse(datacontroller.getDuration(id));
        ShowGraph(valueListcopy,duration);
    }
    


    private GameObject CreateCircle(Vector2 anchoredPosition,int dur) {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.AddComponent<Button>();
        gameObject.GetComponent<Button>().AddEventListener(dur, showFinishedDuration);
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return gameObject;
    }
    void showFinishedDuration(int dur) {
        durationDisplay.text =  dur.ToString() + " mins";
    }

    private void ShowGraph(int[] valueList, int duration) {
        int numberOfComponents = ((valueList.Length + 1) /2) - 1;
        Debug.Log(numberOfComponents);
        int Maxnum = 0;
        
        foreach (int i in valueListcopy) {
            if (i > Maxnum) {
                Maxnum = i;
            }
        }
       
        for (int i = 0; i < numberOfComponents; i++) {
            GameObject instance = Instantiate(transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).gameObject, transform.GetChild(0).GetChild(0).GetChild(0));
            
        }
        float graphHeight = graphContainer.sizeDelta.y;
        float yMaximum;
        if (Maxnum < duration)
        {
            yMaximum = (float)duration;
        }
        else {
            yMaximum = (float)Maxnum;
        }
        float xSize = 50f;

        GameObject lastCircleGameObject = null;
        for (int i = 0; i < valueListcopy.Length; i++) {
            float xPosition = xSize + i * xSize;
            float yPosition;
            if (Maxnum == 0)
            {
                 yPosition = 0f;
            }
            else {
                 yPosition = (valueListcopy[i] / yMaximum) * graphHeight;

            }
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition),valueListcopy[i]);
            if (lastCircleGameObject != null) {
                CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
            }
            lastCircleGameObject = circleGameObject;
        }
        //average percentage display
        int sum = 0; 
        foreach (int num in valueListcopy) {
            sum = sum + num;
        }
       
        float avgPercentage = (float)sum / (float)(valueListcopy.Length * duration);
        if (avgPercentage <= 1)
        {
            transform.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount = avgPercentage;
            transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = ((int)(avgPercentage * 100)).ToString()+"%";

        }
        else {
            transform.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount = 1;
            transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "100";
        }

    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB) {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(1,1,1, .5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));
    }
    private void empty()
    {
        foreach (Transform child in transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).transform)
        {
            Destroy(child.gameObject);
        }
        int num = 0;
        foreach (Transform child in transform.GetChild(0).GetChild(0).GetChild(0).transform)
        {
            if (num > 1) {
                Destroy(child.gameObject);
            }
            num++;
        }
    }
    private void OnEnable()
    {
        transform.GetChild(0).GetChild(2).GetComponent<Scrollbar>().value = 10;
        Debug.Log("enabled");
    }
    private void OnDisable()
    {
        empty();
    }

}
