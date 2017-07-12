using UnityEngine;
using System.Collections.Generic;
using System;

public class WrapContent : MonoBehaviour
{
    public delegate void OnWrapItem ( GameObject item, int dataPointer );

    public UIScrollView scrollView;
    public GameObject itemPrefab;

    public int sourceDataCount;
    public int dataPointer;
    public int itemCount;
    public int gap;
    public Vector2 viewSize;

    public OnWrapItem onWrapItem;
    public OnWrapItem onShow;

    private List<GameObject> renderedItemList = new List<GameObject>();
    private Dictionary<int, Bounds> renderedItemSizeDic = new Dictionary<int, Bounds>();
    private Vector3 scrollViewPos;

    void Start ()
    {
        scrollViewPos = scrollView.transform.localPosition;
        viewSize = scrollView.panel.GetViewSize();
        Render();
        Repos();
        scrollView.restrictWithinPanel = false;
    }

    void OnEnable ()
    {
        scrollView.onMomentumMove += OnScrolling;
        scrollView.onDragFinished += OnDragFinish;
        scrollView.onDragStarted += OnDragStarted;
    }

    private void OnDragStarted ()
    {
        scrollView.restrictWithinPanel = false;
    }

    private void OnDragFinish ()
    {
        scrollView.restrictWithinPanel = true;
        scrollView.RestrictWithinBounds(false,false,true);
    }

    void OnDisable ()
    {
        scrollView.onMomentumMove -= OnScrolling;
        scrollView.onDragFinished -= OnDragFinish;
        scrollView.onDragStarted -= OnDragStarted;
    }

    void Update ()
    {

    }

    private void Render ()
    {
        for (int i = 0; i < itemCount; i++)
        {
            GameObject item = GameObject.Instantiate<GameObject>(itemPrefab);
            item.transform.SetParent(transform);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;
            renderedItemList.Add(item);
            if (onShow != null)
            {
                onShow.Invoke(item, dataPointer);
            }
            dataPointer++;
            renderedItemSizeDic.Add(item.GetInstanceID(), NGUIMath.CalculateRelativeWidgetBounds(item.transform));
        }
    }

    int nowPos = 0;
    private void Repos ()
    {
        bool v = scrollView.movement == UIScrollView.Movement.Vertical;
        for (int i = 0; i < itemCount; i++)
        {
            renderedItemList[i].transform.localPosition = new Vector3(0, -nowPos, 0);
            nowPos += Mathf.RoundToInt(renderedItemSizeDic[renderedItemList[i].GetInstanceID()].size.y + gap);
        }
    }

    void OnScrolling ()
    {
        if (NGUIMath.CalculateRelativeWidgetBounds(transform).size.y <= scrollView.panel.GetViewSize().y)
        {
            scrollView.currentMomentum = Vector3.zero;
            SpringPanel.Begin(scrollView.gameObject, new Vector3(0, scrollViewPos.y, 0), 35);
            return;
        }

        for (int i = 0; i < itemCount; i++)
        {
            Transform trans = renderedItemList[i].transform;
            int instId = trans.gameObject.GetInstanceID();
            Vector3 worldP = transform.TransformPoint(trans.localPosition);
            Vector3 localP = scrollView.transform.parent.InverseTransformPoint(worldP);
            if (scrollView.currentMomentum.y > 0.01f)
            {
                if (localP.y - renderedItemSizeDic[instId].size.y > viewSize.y * 0.5f + scrollViewPos.y)
                {
                    if (dataPointer < sourceDataCount)
                    {
                        if (null != onWrapItem)
                        {
                            onWrapItem.Invoke(trans.gameObject, dataPointer);
                        }
                        Debug.Log("WrapContent Log: " + "scroll upwards! the logic data Index: " + dataPointer);
                        dataPointer++;
                        trans.localPosition = new Vector3(0, -nowPos, 0);
                        if (renderedItemSizeDic.ContainsKey(instId))
                        {
                            renderedItemSizeDic[instId] = NGUIMath.CalculateRelativeWidgetBounds(trans);
                            nowPos += Mathf.RoundToInt(renderedItemSizeDic[instId].size.y + gap);
                        }
                        break;
                    }
                }
            }
            else if (scrollView.currentMomentum.y < -0.01f)
            {
                if (localP.y - gap <= -viewSize.y * 0.5f + scrollViewPos.y)
                {
                    if (dataPointer - itemCount - 1 >= 0)
                    {
                        if (null != onWrapItem)
                        {
                            onWrapItem.Invoke(trans.gameObject, dataPointer - itemCount - 1);
                        }
                        Debug.Log("WrapContent Log: " + "scroll downwards! the logic data Index: " + (dataPointer - itemCount - 1));
                        dataPointer--;
                        if (renderedItemSizeDic.ContainsKey(instId))
                        {
                            nowPos -= Mathf.RoundToInt(renderedItemSizeDic[instId].size.y + gap);
                            renderedItemSizeDic[instId] = NGUIMath.CalculateRelativeWidgetBounds(trans);
                        }
                        trans.localPosition = new Vector3(0, renderedItemSizeDic[instId].size.y + renderedItemList[i < itemCount - 1 ? 1 + i : 0].transform.localPosition.y + gap, 0);
                        break;
                    }
                }
            }
        }
    }

}
