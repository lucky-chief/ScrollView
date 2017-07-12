using UnityEngine;
using System.Collections;

public class DragScrollView : MonoBehaviour
{

    public CircularScrollView scrollView;

    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {

    }

    /// <summary>
    /// Drag the object along the plane.
    /// </summary>

    void OnDrag ( Vector2 delta )
    {
        if (scrollView && NGUITools.GetActive(this))
        {
            scrollView.Drag();
        }
    }
}
