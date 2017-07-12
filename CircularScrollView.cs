using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(UIPanel))]
public class CircularScrollView : MonoBehaviour
{
    public enum Movement { Vertical, Horizontal }

    void Awake()
    {
        mTrans = transform;
        mPanel = GetComponent<UIPanel>();
    }

    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {

    }

    public void Drag ()
    {
        if (UICamera.currentScheme == UICamera.ControlScheme.Controller) return;

        if (enabled && NGUITools.GetActive(gameObject))
        {
            //if (mDragID == -10) mDragID = UICamera.currentTouchID;
            UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;

            // Prevents the drag "jump". Contributed by 'mixd' from the Tasharen forums.
            if (!mDragStarted)
            {
                mDragStarted = true;
                mDragStartOffset = UICamera.currentTouch.totalDelta;
            }

            Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos - mDragStartOffset);

            float dist = 0f;

            if (mPlane.Raycast(ray, out dist))
            {
                Vector3 currentPos = ray.GetPoint(dist);
                Vector3 offset = currentPos - mLastPos;
                mLastPos = currentPos;

                if (offset.x != 0f || offset.y != 0f || offset.z != 0f)
                {
                    offset = mTrans.InverseTransformDirection(offset);

                    if (movement == Movement.Horizontal)
                    {
                        offset.y = 0f;
                        offset.z = 0f;
                    }
                    else if (movement == Movement.Vertical)
                    {
                        offset.x = 0f;
                        offset.z = 0f;
                    }
                    offset = mTrans.TransformDirection(offset);
                }

                // Adjust the momentum
                momentum = Vector3.Lerp(momentum, momentum + offset * (0.01f * momentumAmount), 0.67f);

                Vector3 constraint = mPanel.CalculateConstrainOffset(bounds.min, bounds.max);

                if (constraint.magnitude > 1f)
                {
                    MoveAbsolute(offset * 0.5f);
                    momentum *= 0.5f;
                }
                else
                {
                    MoveAbsolute(offset);
                }
            }
        }
    }

    private void Press ( bool pressed )
    {

    }

    /// <summary>
    /// Move the scroll view by the specified local space amount.
    /// </summary>

    public virtual void MoveRelative ( Vector3 relative )
    {
        mTrans.localPosition += relative;
        Vector2 co = mPanel.clipOffset;
        co.x -= relative.x;
        co.y -= relative.y;
        mPanel.clipOffset = co;
    }

    /// <summary>
    /// Move the scroll view by the specified world space amount.
    /// </summary>

    public void MoveAbsolute ( Vector3 absolute )
    {
        Vector3 a = mTrans.InverseTransformPoint(absolute);
        Vector3 b = mTrans.InverseTransformPoint(Vector3.zero);
        MoveRelative(a - b);
    }

    public Movement movement;
    public List<object> sourceData;
    public BoxCollider dragArea;
    public Vector3 momentum;
    public int itemCount;

    private bool mDragStarted;
    private Vector2 mDragStartOffset;
    private Plane mPlane;
    private Vector3 mLastPos;
    private Transform mTrans;
    private float momentumAmount;
    private UIPanel mPanel;
    private Bounds bounds;
}
