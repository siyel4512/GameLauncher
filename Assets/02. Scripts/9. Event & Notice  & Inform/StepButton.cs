using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StepButton : MonoBehaviour
{
    public int indexNum;

    public SwipeUI swipeUI;
    public GameObject selectImage;
    
    public EventTrigger.Entry clickEvent;

    // Start is called before the first frame update
    void Start()
    {
        SetEvent();
    }

    public void SetEvent()
    {
        clickEvent = new EventTrigger.Entry();
        clickEvent.eventID = EventTriggerType.PointerClick;
        clickEvent.callback.AddListener( (eventData) => swipeUI.BTN_UpdateStepButtons(indexNum));
        
        GetComponent<EventTrigger>().triggers.Add(clickEvent);
    }
}
