using UnityEngine;
using System.Collections;

public class MY_ScrollViewDragger : MonoBehaviour {
    [SerializeField]
    MY_ScrollView scrollView;

    void OnDrag(Vector2 v)
    {
        scrollView.Drag(v);
    }

    void OnPress(bool pressed)
    {
        scrollView.Press(pressed);
    }
}
