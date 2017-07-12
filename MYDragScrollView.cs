using UnityEngine;
using System.Collections;

public class MYDragScrollView : MonoBehaviour {
    [SerializeField]
    MYScrollView scrollView;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDrag(Vector2 v)
    {
        scrollView.Drag(v);
    }

    void OnPress(bool pressed)
    {
        scrollView.Press(pressed);
    }
}
