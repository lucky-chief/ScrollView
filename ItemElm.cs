using UnityEngine;
using System.Collections;

public class ItemElm : MonoBehaviour {
    public UILabel label;
    public UISprite spt;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetLabel(string text)
    {
        int oldLableHeight = label.height;
        label.text = text;
        int newLabelHeight = label.height;
        int deltaHeight = newLabelHeight - oldLableHeight;
        spt.height += deltaHeight;
    }
}
