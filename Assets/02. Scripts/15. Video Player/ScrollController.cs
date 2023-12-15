using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
    public ScrollRect scrollRect;

    // Start is called before the first frame update
    void Start()
    {
        //scrollRect = GetComponent<ScrollRect>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        scrollRect.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        scrollRect.OnDrag(eventData);
    }

    public void OnScroll(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        scrollRect.OnScroll(eventData);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        scrollRect.OnEndDrag(eventData);
    }
}
