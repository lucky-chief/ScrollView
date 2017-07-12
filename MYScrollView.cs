using UnityEngine;
using System.Collections;

public class MYScrollView : MonoBehaviour
{
    public enum Movement { Vertical,Horizontal}

    [SerializeField]
    MYScrollViewContent content = null;
    [SerializeField]
    int maxSpeed = 100;
    [SerializeField]
    float scrollTimeHold = 0.1f;

    private Vector3 mLastPos;
    private Plane mPlane;
    private UIPanel mPanel;
    private Vector3 mMomentum = Vector3.zero;
    private Vector2 mLastTouchPos;
    private Vector2 mDragStartOffset = Vector2.zero;
    private float timePressed;
    private bool inited;
    private bool pressed;
    bool moveUpOrLeft = true;

    public UIPanel Panel { get { return mPanel; } }

    void Awake()
    {
        mPanel = GetComponent<UIPanel>();
    }

    void Start ()
    {

    }

    void Update()
    {
        if(pressed && UICamera.currentTouch != null)
        {
            mLastTouchPos = UICamera.currentTouch.pos;
        }
    }

    public void Init(int dataSourceCount,int renderIndex = 0)
    {
        content.DataSourceCount = dataSourceCount;
        content.RenderIndex = renderIndex;
        content.Render();
        inited = true;
    }

    public void Drag ( Vector2 delta )
    {
        if (!inited) return;
        if (enabled && NGUITools.GetActive(gameObject))
        {
            Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos - mDragStartOffset);
            float dist = 0f;

            if (mPlane.Raycast(ray, out dist))
            {
                Vector3 currentPos = ray.GetPoint(dist);
                Vector3 offset = currentPos - mLastPos;
                
                if (offset.y > 0) moveUpOrLeft = true;
                else if (offset.y < 0) moveUpOrLeft = false;
                mLastPos = currentPos;
                MoveAbsolute(offset);
                if (!moveUpOrLeft && content.DataRenderedIdxUp == -1) return;
                if (moveUpOrLeft && content.DataRenderedIdxDown == content.DataSourceCount - 1) return;
                if (null!=content.SpPanel.onMoving)
                {
                    content.SpPanel.onMoving(moveUpOrLeft);
                }
            }
        }
    }

    public void Press ( bool pressed )
    {
        if (!inited) return;
       // if (content.PullingBack) return;
        this.pressed = pressed;
        if (pressed)
        {
            content.PullingBack = false;
            timePressed = Time.time;
            mLastPos = UICamera.lastWorldPosition;
            mLastTouchPos = UICamera.currentTouch.pos;
            content.SpPanel.enabled = false;
            mPlane = new Plane(content.transform.rotation * Vector3.back, mLastPos);
        }
        else
        {
            if (moveUpOrLeft && content.DataRenderedIdxDown == content.DataSourceCount)
            {
                Debug.Log("cccccccccc: " + content.DataRenderedIdxDown);
                content.UpdateStopPosition();
                content.PullBack(true);
                return;
            }
            if (!moveUpOrLeft && content.DataRenderedIdxUp == -1)
            {
                Debug.Log("dddddddddddddddd");
                content.UpdateStopPosition(false);
                content.PullBack(false);
                return;
            }

            float now = Time.time;
            if (now - timePressed <= scrollTimeHold && (UICamera.currentTouch.pos - mLastTouchPos).sqrMagnitude > 0.1f)
            {
                Vector3 pos = content.transform.localPosition + new Vector3(0, moveUpOrLeft ? maxSpeed : -maxSpeed, 0);
                pos.x = Mathf.Round(pos.x);
                pos.y = Mathf.Round(pos.y);
                SpringPanel.Begin(content.gameObject, pos, 13f).strength = 8f;
            }
        }
    }

    void MoveAbsolute ( Vector3 absolute )
    {
        Vector3 a = content.transform.InverseTransformPoint(absolute);
        Vector3 b = content.transform.InverseTransformPoint(Vector3.zero);
        MoveRelative(a - b);
    }

    void MoveRelative ( Vector3 relative )
    {
        relative.x = 0;
        relative.z = 0;
        content.transform.localPosition += relative;
    }
}
