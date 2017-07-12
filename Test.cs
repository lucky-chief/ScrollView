using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if(Input.GetKeyUp(KeyCode.Alpha1))
        {
            Bounds b = NGUIMath.CalculateRelativeWidgetBounds(transform);
            Debug.Log("========: " + b.ToString());
        }
    }
}
