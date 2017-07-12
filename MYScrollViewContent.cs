using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(SpringPanel))]
public class MYScrollViewContent : MonoBehaviour
{

    public delegate void MYScrollViewItem ( GameObject item, int dataIndex );
    public event MYScrollViewItem renderItem;

    [SerializeField]
    MYScrollView scrollView;
    [SerializeField]
    MYScrollView.Movement layout;
    [SerializeField]
    GameObject renderItemPrefab;
    [SerializeField]
    bool equalSize = true;
    [SerializeField]
    int size;
    [SerializeField]
    int gap = 10;

    int dataSourceCount;
    int renderIndex;

    SpringPanel mSpringPanel;
    Vector3 mStopPos;
    Bounds mBounds;
    bool mPullBack;
    int dataRenderedIdxUp;
    int dataRenderedIdxDown;
    Vector2 viewSize;
    bool mStopPosCaled = false;

    List<GameObject> cacheUp = new List<GameObject>();
    List<GameObject> cacheDown = new List<GameObject>();
    List<GameObject> renderedItems = new List<GameObject>();

    void Awake ()
    {
        mStopPos = transform.localPosition;
        mSpringPanel = GetComponent<SpringPanel>();
        viewSize = scrollView.Panel.GetViewSize();
        mBounds = NGUIMath.CalculateAbsoluteWidgetBounds(transform);
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

    public int DataRenderedIdxUp
    {
        get { return dataRenderedIdxUp; }
    }

    public int DataRenderedIdxDown
    {
        get { return dataRenderedIdxDown; }
    }

    public Bounds ContentBounds
    {
        get
        {
            return mBounds;
        }
    }

    public void Render ()
    {
        int idx = 0;
        int dataIdx = renderIndex;
        dataRenderedIdxUp = renderIndex;
        dataRenderedIdxDown = renderIndex;
        while (true)
        {
            if (equalSize)
            {
                int nowPos = -idx * (size + gap);
                if (Mathf.Abs(nowPos) >= scrollView.Panel.GetViewSize().y)
                {
                    if (dataIdx < DataSourceCount)
                    {
                        mStopPos = transform.localPosition;
                        if (dataRenderedIdxUp > 0)
                        {
                            GameObject cacheItem = NewItem(scrollView.gameObject, Vector3.one * -10000);
                            cacheUp.Add(cacheItem);
                            ItemRenderCallback(cacheItem, dataRenderedIdxUp - 1);
                        }

                        if (dataRenderedIdxDown < dataSourceCount - 1)
                        {
                            GameObject cacheItem = NewItem(gameObject, new Vector3(0, -dataRenderedIdxDown * (size + gap), 0));
                            renderedItems.Add(cacheItem);
                            ItemRenderCallback(cacheItem, dataRenderedIdxDown);
                        }
                        return;
                    }
                    return;
                }
                else
                {
                    GameObject item = NewItem(gameObject, new Vector3(0, nowPos, 0));
                    renderedItems.Add(item);
                    ItemRenderCallback(item, dataIdx);

                    idx++;
                    dataIdx++;
                    dataRenderedIdxDown++;
                }
            }
            else
            {

            }
        }
    }

    public void PullBack ( bool moveUp = true )
    {
        if (mPullBack) return;
        mPullBack = true;
        SpringPanel.Begin(gameObject, mStopPos, 13f).strength = 25f;
    }

    public void UpdateStopPosition()
    {
        if (mStopPosCaled) return;
        GameObject item = renderedItems[renderedItems.Count - 1];
        Bounds b = NGUIMath.CalculateRelativeWidgetBounds(scrollView.transform, item.transform);
        float H = scrollView.Panel.GetViewSize().y * 0.5f;
        float h = b.extents.y;
        float y = scrollView.Panel.clipOffset.y + h - H;
        float deltaY = y - b.center.y;
        Vector3 pos = transform.localPosition + new Vector3(0, deltaY, 0);
        //pos.x = Mathf.Ceil(pos.x);
        //pos.y = Mathf.Ceil(pos.y);
        mStopPosCaled = true;
        mStopPos = pos;
        //Debug.Log("======H======: " + H);
        //Debug.Log("======h======: " + h);
        //Debug.Log("======y======: " + y);
        //Debug.Log("======deltaY======: " + deltaY);
        //Debug.Log("======b.center.y======: " + b.center.y);
        //Debug.Log("======mStopPos======: " + mStopPos);
    }

    void OnMoving ( bool moveUp )
    {
        if (mPullBack) return;
        if (equalSize)
        {
            for (int i = 0; i < renderedItems.Count; i++)
            {
                Transform trans = renderedItems[i].transform;
                Vector3 worldP = transform.TransformPoint(trans.localPosition);
                Vector3 localP = scrollView.transform.parent.InverseTransformPoint(worldP);
                if (moveUp)
                {
                    if (localP.y - size > viewSize.y * 0.5f + scrollView.transform.localPosition.y)
                    {
                        Debug.Log("=============111: " + dataRenderedIdxDown);
                        if (dataRenderedIdxDown < dataSourceCount)
                        {
                            GameObject item = renderedItems[0];
                            renderedItems.RemoveAt(0);
                            renderedItems.Add(item);
                            if (dataRenderedIdxDown < dataSourceCount)
                            {
                                trans.localPosition = new Vector3(0, -dataRenderedIdxDown * (size + gap), 0);
                                trans.gameObject.name = dataRenderedIdxDown + "";
                                if (dataRenderedIdxDown >= dataSourceCount - 1)
                                {
                                    UpdateStopPosition();
                                    PullBack(true);
                                    return;
                                }
                                dataRenderedIdxDown++;
                                dataRenderedIdxUp++;
                                if (null != renderItem)
                                    renderItem(trans.gameObject, dataRenderedIdxDown);
                            }
                        }
                        break;
                    }
                }
                else
                {
                    if (localP.y - gap > -viewSize.y * 0.5f + scrollView.transform.localPosition.y)
                    {
                        if (cacheUp.Count > 0)
                        {
                            GameObject item = cacheUp[0];
                            item.transform.SetParent(transform);
                            item.transform.localPosition = new Vector3(0, dataRenderedIdxUp * (size + gap), 0);
                            cacheUp.RemoveAt(0);
                            if (dataRenderedIdxUp > 0)
                            {
                                dataRenderedIdxDown--;
                                dataRenderedIdxUp--;
                                cacheUp.Add(trans.gameObject);
                                if (null != renderItem)
                                    renderItem(item, dataRenderedIdxUp);
                            }
                        }
                        break;
                    }
                }
            }
        }
    }

    void OnFinish ()
    {
        if (mPullBack)
        {
            mPullBack = false;
            return;
        }
    }

    GameObject NewItem ( GameObject parent, Vector3 pos )
    {
        GameObject go = NGUITools.AddChild(parent, renderItemPrefab); ;
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = pos;
        return go;
    }

    void ItemRenderCallback ( GameObject item, int dataIdx )
    {
        if (null != renderItem)
        {
            renderItem.Invoke(item, dataIdx);
        }
    }
}
