using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(SpringPanel))]
public class MYScrollViewContent : MonoBehaviour
{

    public delegate void MYScrollViewItem(GameObject item, int dataIndex);
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
    bool mLastCalStopPosDirUp = false;

    List<GameObject> renderedItems = new List<GameObject>();

    void Awake()
    {
        mStopPos = transform.localPosition;
        mSpringPanel = GetComponent<SpringPanel>();
        viewSize = scrollView.Panel.GetViewSize();
        mBounds = NGUIMath.CalculateAbsoluteWidgetBounds(transform);
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

    public bool PullingBack
    {
        get { return mPullBack; }
        set { mPullBack = value; }
    }

    public void Render()
    {
        dataRenderedIdxUp = -1;
        dataRenderedIdxDown = renderIndex;
        while (true)
        {
            if (equalSize)
            {
                int nowPos = -dataRenderedIdxDown * (size + gap);
                if (Mathf.Abs(nowPos) >= scrollView.Panel.GetViewSize().y)
                {
                    if (dataRenderedIdxDown < DataSourceCount)
                    {
                        mStopPos = transform.localPosition;
                        //if (dataRenderedIdxUp > 0)
                        //{
                        //    GameObject cacheItem = NewItem(scrollView.gameObject, Vector3.one * -10000);
                        //    ItemRenderCallback(cacheItem, dataRenderedIdxUp - 1);
                        //}

                        if (dataRenderedIdxDown < dataSourceCount - 1)
                        {
                            GameObject item = NewItem(gameObject, new Vector3(0, -dataRenderedIdxDown * (size + gap), 0));
                            renderedItems.Add(item);
                            item.name = dataRenderedIdxDown.ToString();
                            ItemRenderCallback(item, dataRenderedIdxDown);
                            dataRenderedIdxDown++;
                        }
                        return;
                    }
                    return;
                }
                else
                {
                    GameObject item = NewItem(gameObject, new Vector3(0, nowPos, 0));
                    renderedItems.Add(item);
                    item.name = dataRenderedIdxDown.ToString();
                    ItemRenderCallback(item, dataRenderedIdxDown);
                    dataRenderedIdxDown++;
                }
            }
            else
            {

            }
        }
    }

    public void PullBack(bool moveUp = true)
    {
        if (mPullBack) return;
        mPullBack = true;
        SpringPanel.Begin(gameObject, mStopPos, 13f).strength = 8f;
    }

    public void UpdateStopPosition(bool moveUpOrLeft = true)
    {
        if (mStopPosCaled && moveUpOrLeft == mLastCalStopPosDirUp) return;
        GameObject item = moveUpOrLeft
                          ? renderedItems[renderedItems.Count - 1]
                          : renderedItems[0];
        Debug.Log("item-name: " + item.name);
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
        if (equalSize)
        {
            for (int i = 0; i < renderedItems.Count; i++)
            {
                Transform trans = renderedItems[i].transform;
                Vector3 worldP = transform.TransformPoint(trans.localPosition);
                Vector3 localP = scrollView.transform.parent.InverseTransformPoint(worldP);
                if (moveUpOrLeft)
                {
                    if (localP.y - size > viewSize.y * 0.5f + scrollView.Panel.clipOffset.y + scrollView.transform.localPosition.y)
                    {
                        Debug.Log("=============111: " + dataRenderedIdxDown);
                        if (dataRenderedIdxDown < dataSourceCount)
                        {
                            GameObject item = renderedItems[0];
                            renderedItems.RemoveAt(0);
                            renderedItems.Add(item);
                            trans.localPosition = new Vector3(0, -dataRenderedIdxDown * (size + gap), 0);
                            trans.gameObject.name = dataRenderedIdxDown + "";
                            if (dataRenderedIdxDown == dataSourceCount - 1)
                            {
                                UpdateStopPosition();
                                PullBack(true);
                               // return;
                            }
                            dataRenderedIdxDown++;
                            dataRenderedIdxUp++;
                            if (null != renderItem)
                                renderItem(trans.gameObject, dataRenderedIdxDown);
                        }
                        break;
                    }
                }
                else
                {
                    if (localP.y < -viewSize.y * 0.5f + scrollView.Panel.clipOffset.y + scrollView.transform.localPosition.y)
                    {
                        Debug.Log("============dataRenderedIdxUp: " + dataRenderedIdxUp);
                        if (dataRenderedIdxUp >= 0)
                        {
                            int renderedCount = renderedItems.Count;
                            GameObject item = renderedItems[renderedCount - 1];
                            renderedItems.RemoveAt(renderedCount - 1);
                            renderedItems.Insert(0, item);
                            trans.localPosition = new Vector3(0, -dataRenderedIdxUp * (size + gap), 0);
                            trans.gameObject.name = dataRenderedIdxUp + "";
                            if (dataRenderedIdxUp == 0)
                            {
                                UpdateStopPosition(false);
                                PullBack(false);
                              //  return;
                            }
                            dataRenderedIdxDown--;
                            dataRenderedIdxUp--;
                            if (null != renderItem)
                                renderItem(trans.gameObject, dataRenderedIdxUp);
                        }
                        break;
                    }
                }
            }
        }
    }

    void OnFinish()
    {
        if (mPullBack)
        {
            mPullBack = false;
            return;
        }
    }

    GameObject NewItem(GameObject parent, Vector3 pos)
    {
        GameObject go = NGUITools.AddChild(parent, renderItemPrefab); ;
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = pos;
        return go;
    }

    void ItemRenderCallback(GameObject item, int dataIdx)
    {
        if (null != renderItem)
        {
            renderItem.Invoke(item, dataIdx);
        }
    }
}
