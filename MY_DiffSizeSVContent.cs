using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(SpringPanel))]
public class MY_DiffSizeSVContent : MonoBehaviour, IMY_SVContent
{
    [SerializeField]
    MY_ScrollView scrollView;
    [SerializeField]
    MY_ScrollView.Movement layout;
    [SerializeField]
    GameObject renderItemPrefab;
    [SerializeField]
    int gap = 10;

    int dataSourceCount;
    int renderIndex;

    SpringPanel mSpringPanel;
    Vector3 mStopPos;
    bool mPullBack;
    int minIndex;
    int maxIndex;
    Vector2 viewSize;
    bool mStopPosCaled = false;
    bool mLastCalStopPosDirUp = false;
    bool fulled = false;
    bool mBottomed = false;
    bool mToped = true;
    bool pressed;
    Vector3 contentStartPos;

    List<GameObject> renderedItems = new List<GameObject>();
    Pool pool = new Pool();

    void Awake()
    {
        mStopPos = transform.localPosition;
        mSpringPanel = GetComponent<SpringPanel>();
        viewSize = scrollView.Panel.GetViewSize();
        contentStartPos = transform.localPosition;
    }

    void Start()
    {
        mSpringPanel.onMoving += OnMoving;
        mSpringPanel.onFinished += OnFinish;
    }

    void OnDestroy()
    {
        mSpringPanel.onMoving -= OnMoving;
        mSpringPanel.onFinished -= OnFinish;
    }

    public Transform trans
    {
        get { return transform; }
    }

    public SpringPanel SpPanel
    {
        get
        {
            return mSpringPanel;
        }
    }

    public int DataSourceCount
    {
        get { return dataSourceCount; }
        set { dataSourceCount = value; }
    }

    public int RenderIndex
    {
        get { return renderIndex; }
        set { renderIndex = value; }
    }

    public int MinIndex
    {
        get { return minIndex; }
    }

    public int MaxIndex
    {
        get { return maxIndex; }
    }

    public bool PullingBack
    {
        get { return mPullBack; }
        set { mPullBack = value; }
    }

    public bool Pressed
    {
        get { return pressed; }
        set
        {
            pressed = value;
        }
    }

    public void Render()
    {
        if (dataSourceCount == 0) return;
        minIndex = 0;
        maxIndex = 0;
        float nowPos = 0;
        while (true)
        {
            if (Mathf.Abs(nowPos) >= scrollView.Panel.GetViewSize().y)
            {
                fulled = true;
                mStopPos = transform.localPosition;
                if (maxIndex < dataSourceCount)
                {
                    GameObject item = NewItem(gameObject, new Vector3(0, nowPos, 0));
                    renderedItems.Add(item);
                    item.name = maxIndex.ToString();
                    scrollView.ItemChange(item, maxIndex);
                    nowPos -= NGUIMath.CalculateRelativeWidgetBounds(transform, item.transform).size.y + gap;
                    if (maxIndex + 1 < dataSourceCount)
                    {
                        maxIndex++;
                    }
                }
                return;
            }
            else
            {
                if (maxIndex < dataSourceCount)
                {
                    GameObject item = NewItem(gameObject, new Vector3(0, nowPos, 0));
                    renderedItems.Add(item);
                    item.name = maxIndex.ToString();
                    scrollView.ItemChange(item, maxIndex);
                    nowPos -= NGUIMath.CalculateRelativeWidgetBounds(transform, item.transform).size.y + gap;
                    if (maxIndex + 1 < dataSourceCount)
                    {
                        maxIndex++;
                    }
                }
            }
        }
    }

    public void PullBack()
    {
        if (mPullBack) return;
        mPullBack = true;
        SpringPanel.Begin(gameObject, mStopPos, 13f).strength = 8f;
    }

    bool ValidateMin()
    {
        return minIndex - 1 >= 0;
    }

    bool ValidateMax()
    {
        return maxIndex + 1 < dataSourceCount;
    }

    public void UpdateStopPosition(bool moveUpOrLeft = true)
    {
        if (maxIndex != dataSourceCount - 1 && minIndex != 0) return;
        Debug.Log("AAAAAAAAAAAAAAAAAA: " + moveUpOrLeft);
        if (!fulled) moveUpOrLeft = false;
        GameObject item = moveUpOrLeft
                          ? renderedItems[renderedItems.Count - 1]
                          : renderedItems[0];
        Bounds b = NGUIMath.CalculateRelativeWidgetBounds(scrollView.transform, item.transform);
        float H = scrollView.Panel.GetViewSize().y * 0.5f;
        float h = b.extents.y;
        float y = moveUpOrLeft
                  ? scrollView.Panel.clipOffset.y + h - H
                  : scrollView.Panel.clipOffset.y - h + H;
        float deltaY = moveUpOrLeft ? y - b.center.y : b.center.y - y;
        Vector3 pos = moveUpOrLeft
                      ? transform.localPosition + new Vector3(0, deltaY, 0)
                      : transform.localPosition - new Vector3(0, deltaY, 0);
        mStopPosCaled = true;
        mLastCalStopPosDirUp = moveUpOrLeft;
        mStopPos = pos;
        //Debug.Log("======H======: " + H);
        //Debug.Log("======h======: " + h);
        //Debug.Log("======y======: " + y);
        //Debug.Log("======deltaY======: " + deltaY);
        //Debug.Log("======b.center.y======: " + b.center.y);
        //Debug.Log("======mStopPos======: " + mStopPos);
    }

    void OnMoving(bool moveUpOrLeft)
    {
        if (mPullBack) return;
        if (!pressed) return;
        float movetum = transform.localPosition.y - contentStartPos.y;
        int renderedCount = renderedItems.Count;
        if(movetum > 0)
        {
            GameObject lastItem = renderedItems[renderedCount - 1];
            Bounds b = NGUIMath.CalculateRelativeWidgetBounds(transform, lastItem.transform);
            if (lastItem.transform.localPosition.y + transform.localPosition.y >= scrollView.Panel.clipOffset.y - scrollView.ViewSize.y * 0.5f + b.size.y)
            {
                if(ValidateMax())
                {
                    minIndex++;
                    maxIndex++;
                    GameObject item = pool.Pop()[0];
                    renderedItems.Add(item);
                    scrollView.ItemChange(item,maxIndex);
                }
                Debug.Log("xxxxxxxxxxxxxxxxx");
            }

            //float abs = Mathf.Abs(movetum);
            //if (abs >= gap)
            //{
            //    contentStartPos = trans.localPosition;
            //    cacheDown[0].transform.SetParent(transform);
            //    cacheDown[0].transform.localScale = Vector3.one;
            //    cacheDown[0].transform.localPosition = renderedItems[renderedItems.Count - 1].transform.localPosition + new Vector3(0, gap, 0);
            //    cacheDown.RemoveAt(0);
            //    if(cacheDown.Count == 0)
            //    {
            //        FreshCacheDown(null);
            //    }
            //}
        }
        else if(movetum < 0)
        {

        }

        //for (int i = 0; i < renderedItems.Count; i++)
        //{
        //    Transform trans = renderedItems[i].transform;
        //    Vector3 worldP = transform.TransformPoint(trans.localPosition);
        //    Vector3 localP = scrollView.transform.parent.InverseTransformPoint(worldP);
        //    if (moveUpOrLeft)
        //    {
        //        if (localP.y - size > viewSize.y * 0.5f + scrollView.Panel.clipOffset.y + scrollView.transform.localPosition.y)
        //        {
        //            if (dataRenderedIdxDown < dataSourceCount)
        //            {
        //                if (mBottomed) return;
        //                Vector3 pos = renderedItems[renderedItems.Count - 1].transform.localPosition;
        //                GameObject item = renderedItems[0];
        //                renderedItems.RemoveAt(0);
        //                renderedItems.Add(item);
        //                trans.localPosition = pos - new Vector3(0, size + gap, 0);
        //                if (dataRenderedIdxDown == dataSourceCount - 1)
        //                {
        //                    mBottomed = true;
        //                    mToped = false;
        //                    UpdateStopPosition(true);
        //                    PullBack();
        //                    //return;
        //                }
        //                trans.gameObject.name = dataRenderedIdxDown + "";
        //                scrollView.ItemChange(trans.gameObject, dataRenderedIdxDown);
        //                if (dataRenderedIdxDown + 1 < dataSourceCount)
        //                {
        //                    mBottomed = false;
        //                    mToped = false;
        //                    dataRenderedIdxDown++;
        //                    dataRenderedIdxUp++;
        //                    //Debug.Log("==========dataRenderedIdxDown: " + dataRenderedIdxDown);
        //                    //Debug.Log("==========dataRenderedIdxUp: " + dataRenderedIdxUp);
        //                }
        //            }
        //            break;
        //        }
        //    }
        //    else
        //    {
        //        if (localP.y < -viewSize.y * 0.5f + scrollView.Panel.clipOffset.y + scrollView.transform.localPosition.y)
        //        {
        //            if (dataRenderedIdxUp >= 0)
        //            {
        //                if (mToped) return;
        //                Vector3 pos = renderedItems[0].transform.localPosition;
        //                int renderedCount = renderedItems.Count;
        //                GameObject item = renderedItems[renderedCount - 1];
        //                renderedItems.RemoveAt(renderedCount - 1);
        //                renderedItems.Insert(0, item);
        //                Debug.Log("==========dataRenderedIdxUp: " + dataRenderedIdxUp);
        //                trans.localPosition = pos + new Vector3(0, size + gap, 0);
        //                if (dataRenderedIdxUp == 0)
        //                {
        //                    mToped = true;
        //                    mBottomed = false;
        //                    UpdateStopPosition(false);
        //                    PullBack();
        //                    // return;
        //                }
        //                trans.gameObject.name = dataRenderedIdxUp + "";
        //                scrollView.ItemChange(trans.gameObject, dataRenderedIdxUp);
        //                if (dataRenderedIdxUp - 1 >= 0)
        //                {
        //                    mToped = false;
        //                    mBottomed = false;
        //                    dataRenderedIdxDown--;
        //                    dataRenderedIdxUp--;
        //                    Debug.Log("==========dataRenderedIdxDown: " + dataRenderedIdxDown);
        //                    Debug.Log("==========dataRenderedIdxUp: " + dataRenderedIdxUp);
        //                }
        //            }
        //            break;
        //        }
        //    }
        //}
    }

    void OnFinish()
    {
        mPullBack = false;
    }

    GameObject NewItem(GameObject parent, Vector3 pos)
    {
        GameObject go = NGUITools.AddChild(parent, renderItemPrefab); ;
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = pos;
        return go;
    }
}

class Pool
{
    private List<GameObject> pool = new List<GameObject>();
    private GameObject cacheTemplate;

    public void RegisterCache(GameObject tpl)
    {
        cacheTemplate = tpl;
    }

    public List<GameObject> Pop(int count = 1)
    {
        List<GameObject> retList = new List<GameObject>();
        for(int i = 0; i < pool.Count && count > 0; i++,count--)
        {
            retList.Add(pool[0]);
            pool.RemoveAt(0);
        }
        for(int i = count; count > 0; i--)
        {
            retList.Add(GameObject.Instantiate<GameObject>(cacheTemplate));
        }
        return retList;
    }

    public void Push(List<GameObject> items)
    {
        pool.AddRange(items);
    }
}
