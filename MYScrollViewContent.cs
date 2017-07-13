using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(SpringPanel))]
public class MYScrollViewContent : MonoBehaviour
{
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
    bool mPullBack;
    int dataRenderedIdxUp;
    int dataRenderedIdxDown;
    Vector2 viewSize;
    bool mStopPosCaled = false;
    bool mLastCalStopPosDirUp = false;
    bool fulled = false;

    List<GameObject> renderedItems = new List<GameObject>();

    void Awake ()
    {
        mStopPos = transform.localPosition;
        mSpringPanel = GetComponent<SpringPanel>();
        viewSize = scrollView.Panel.GetViewSize();
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

    public bool PullingBack
    {
        get { return mPullBack; }
        set { mPullBack = value; }
    }

    public void Render ()
    {
        if (dataSourceCount == 0) return;
        dataRenderedIdxUp = -1;
        dataRenderedIdxDown = renderIndex;
        while (true)
        {
            if (equalSize)
            {
                int nowPos = -dataRenderedIdxDown * (size + gap);
                if (Mathf.Abs(nowPos) >= scrollView.Panel.GetViewSize().y)
                {
                    fulled = true;
                    if (dataRenderedIdxDown < dataSourceCount)
                    {
                        mStopPos = transform.localPosition;
                        if (dataRenderedIdxDown < dataSourceCount - 1)
                        {
                            GameObject item = NewItem(gameObject, new Vector3(0, -dataRenderedIdxDown * (size + gap), 0));
                            renderedItems.Add(item);
                            item.name = dataRenderedIdxDown.ToString();
                            scrollView.ItemChange(item, dataRenderedIdxDown);
                            dataRenderedIdxDown++;
                        }
                        return;
                    }
                    return;
                }
                else
                {
                    if (dataRenderedIdxDown == dataSourceCount) return;
                    GameObject item = NewItem(gameObject, new Vector3(0, nowPos, 0));
                    renderedItems.Add(item);
                    item.name = dataRenderedIdxDown.ToString();
                    scrollView.ItemChange(item, dataRenderedIdxDown);
                    dataRenderedIdxDown++;
                }
            }
            else
            {

            }
        }
    }

    public void PullBack ()
    {
        if (mPullBack) return;
        mPullBack = true;
        SpringPanel.Begin(gameObject, mStopPos, 13f).strength = 8f;
    }

    public void UpdateStopPosition ( bool moveUpOrLeft = true )
    {
        if (mStopPosCaled && moveUpOrLeft == mLastCalStopPosDirUp) return;
        if(!fulled) moveUpOrLeft = false;
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
                        if (dataRenderedIdxDown < dataSourceCount)
                        {
                            GameObject item = renderedItems[0];
                            renderedItems.RemoveAt(0);
                            renderedItems.Add(item);
                            trans.localPosition = new Vector3(0, -dataRenderedIdxDown * (size + gap), 0);
                            trans.gameObject.name = dataRenderedIdxDown + "";
                            if (dataRenderedIdxDown == dataSourceCount - 1)
                            {
                                UpdateStopPosition(true);
                                PullBack();
                            }
                            scrollView.ItemChange(trans.gameObject, dataRenderedIdxDown);
                            dataRenderedIdxDown++;
                            dataRenderedIdxUp++;
                        }
                        break;
                    }
                }
                else
                {
                    if (localP.y < -viewSize.y * 0.5f + scrollView.Panel.clipOffset.y + scrollView.transform.localPosition.y)
                    {
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
                                PullBack();
                            }
                            scrollView.ItemChange(trans.gameObject, dataRenderedIdxUp);
                            dataRenderedIdxDown--;
                            dataRenderedIdxUp--;
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
}
