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

    void Awake ()
    {
        pool.RegisterCache(renderItemPrefab);
        mStopPos = transform.localPosition;
        mSpringPanel = GetComponent<SpringPanel>();
        viewSize = scrollView.Panel.GetViewSize();
        contentStartPos = transform.localPosition;
    }

    void Start ()
    {
        mSpringPanel.onMoving += OnMoving;
        mSpringPanel.onFinished += OnFinish;
    }

    void OnDestroy ()
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
            contentStartPos = transform.localPosition;
        }
    }

    public void Render ()
    {
        if (dataSourceCount == 0) return;
        minIndex = 0;
        maxIndex = 0;
        float nowPos = 0;
        while (true)
        {
            if (Mathf.Abs(nowPos) < scrollView.Panel.GetViewSize().y)
            {
                fulled = false;
                if (maxIndex < dataSourceCount)
                {
                    GameObject item = pool.Pop(); //NewItem(gameObject, new Vector3(0, nowPos, 0));
                    item.transform.SetParent(transform);
                    item.transform.localScale = Vector3.one;
                    item.transform.localPosition = new Vector3(0, nowPos, 0);
                    renderedItems.Add(item);
                    item.name = maxIndex.ToString();
                    scrollView.ItemChange(item, maxIndex);
                    nowPos -= NGUIMath.CalculateRelativeWidgetBounds(transform, item.transform,true).size.y + gap;
                    if (ValidateMax())
                    {
                        maxIndex++;
                    }
                    else return;
                }
            }
            else
            {
                fulled = true;
                mStopPos = transform.localPosition;
                return;
            }
        }
    }

    public void PullBack ()
    {
        if (mPullBack) return;
        mPullBack = true;
        SpringPanel.Begin(gameObject, mStopPos, 13f).strength = 8f;
    }

    bool ValidateMin ()
    {
        return minIndex - 1 >= 0;
    }

    bool ValidateMax ()
    {
        return maxIndex < dataSourceCount;
    }

    public void UpdateStopPosition ( bool moveUpOrLeft = true )
    {
        if (maxIndex != dataSourceCount - 1 && minIndex != 0) return;
        // Debug.Log("AAAAAAAAAAAAAAAAAA: " + moveUpOrLeft);
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

    void OnMoving ( bool moveUpOrLeft )
    {
        if (mPullBack) return;
        if (!pressed) return;
        float movetum = transform.localPosition.y - contentStartPos.y;
        contentStartPos = transform.localPosition;
        int renderedCount = renderedItems.Count;
        if (movetum > 0)
        {
            if (maxIndex == dataSourceCount - 1) return;
            GameObject lastItem = renderedItems[renderedCount - 1];
            Bounds b = NGUIMath.CalculateRelativeWidgetBounds(transform, lastItem.transform,true);
            if (lastItem.transform.localPosition.y + transform.localPosition.y >= scrollView.Panel.clipOffset.y - scrollView.ViewSize.y * 0.5f + b.size.y)
            {
                Debug.Log("AAAAAAA: " + maxIndex);
                if (ValidateMax())
                {
                    GameObject item = pool.Pop();
                    item.transform.SetParent(transform);
                    item.transform.localPosition = lastItem.transform.localPosition - new Vector3(0, b.size.y + gap, 0);
                    item.SetActive(true);
                    renderedItems.Add(item);
                    scrollView.ItemChange(item, maxIndex);
                    maxIndex++;
                    Debug.Log("BBBBBBB: " + maxIndex);
                }
            }

            GameObject firstItem = renderedItems[0];
            b = NGUIMath.CalculateRelativeWidgetBounds(transform, firstItem.transform, true);
            if (firstItem.transform.localPosition.y + transform.localPosition.y >= scrollView.Panel.clipOffset.y + scrollView.ViewSize.y * 0.5f + b.size.y)
            {
                minIndex++;
                pool.Push(new List<GameObject>() { firstItem });
                firstItem.transform.SetParent(scrollView.transform);
                firstItem.SetActive(false);
                renderedItems.RemoveAt(0);
            }
        }
        else if (movetum < 0)
        {
            Debug.Log("=========: " + minIndex);
            if (minIndex == 0) return;
            GameObject firstItem = renderedItems[0];
            Debug.Log("firstItemName: " + firstItem.name);
            if (firstItem.transform.localPosition.y + transform.localPosition.y <= scrollView.Panel.clipOffset.y + scrollView.ViewSize.y * 0.5f)
            {
                Debug.Log("AAAAAAAAAAAAAAA");
                if (ValidateMin())
                {
                    minIndex--;
                    GameObject item = pool.Pop();
                    scrollView.ItemChange(item, minIndex);
                    Bounds b = NGUIMath.CalculateRelativeWidgetBounds(transform, item.transform, true);
                    item.transform.SetParent(transform);
                    item.transform.localPosition = firstItem.transform.localPosition + new Vector3(0, b.size.y + gap, 0);
                    item.SetActive(true);
                    renderedItems.Insert(0, item);
                }
            }

            GameObject lastItem = renderedItems[renderedCount - 1];
            if (lastItem.transform.localPosition.y + transform.localPosition.y <= scrollView.Panel.clipOffset.y - scrollView.ViewSize.y * 0.5f)
            {
                maxIndex--;
                pool.Push(new List<GameObject>() { lastItem });
                lastItem.transform.SetParent(scrollView.transform);
                lastItem.SetActive(false);
                renderedItems.RemoveAt(renderedCount - 1);
            }
        }
    }

    void OnFinish ()
    {
        mPullBack = false;
    }

    GameObject NewItem ( GameObject parent, Vector3 pos )
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

    public void RegisterCache ( GameObject tpl )
    {
        cacheTemplate = tpl;
    }

    public GameObject Pop ()
    {
        if (pool.Count > 0)
        {
            GameObject ret = pool[0];
            pool.RemoveAt(0);
            return ret;
        }
        else return GameObject.Instantiate<GameObject>(cacheTemplate);
    }

    public List<GameObject> Pop ( int count )
    {
        List<GameObject> retList = new List<GameObject>();
        if (pool.Count > 0) retList.Add(pool[0]);
        else retList.Add(GameObject.Instantiate<GameObject>(cacheTemplate));
        //for(int i = 0; i < pool.Count && count > 0; i++,count--)
        //{
        //    retList.Add(pool[0]);
        //    pool.RemoveAt(0);
        //}
        //for(int i = count; count > 0; i--)
        //{
        //    retList.Add(GameObject.Instantiate<GameObject>(cacheTemplate));
        //}
        return retList;
    }

    public void Push ( List<GameObject> items )
    {
        pool.AddRange(items);
    }
}
