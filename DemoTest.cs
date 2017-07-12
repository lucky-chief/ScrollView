using UnityEngine;
using System.Collections;

public class DemoTest : MonoBehaviour
{
   // string[] testStrs = { "AAAAAA", "AAAAAA\nAAAA", "AAAAAA\nAAA\nAAAAAAA", "AAAAAA", "AAAAAA\nAAA\nAAAAAAA\nAAAAAAAAAAAAAA","bbbb", "AAAAAA\nAAAA", "AAAAAA\nAAA\nAAAAAAA", "AAAAAA", "AAAAAA\nAAA\nAAAAAAA\nAAAAAAAAAAAAAA", "ccccc", "AAAAAA\nAAAA", "AAAAAA\nAAA\nAAAAAAA", "AAAAAA", "AAAAAA\nAAA\nAAAAAAA\nAAAAAAAAAAAAAA", "bbbb", "AAAAAA\nAAAA", "AAAAAA\nAAA\nAAAAAAA", "AAAAAA", "AAAAAA\nAAA\nAAAAAAA\nAAAAAAAAAAAAAA", "ccccc" };
    string[] testStrs = { "AAAAAA", "AAAAAA", "AAAAAA", "AAAAAA", "AAAAAA", "AAAAAA", "AAAAAA", "AAAAAA", "AAAAAA", "AAAAAA", "AAAAAA" };
  //  string[] testStrs = { "AAAAAA"};

    public MYScrollView scrollView;
    // Use this for initialization
    void Start ()
    {
        scrollView.Init(testStrs.Length);
    }

    // Update is called once per frame
    void Update ()
    {

    }

    void onShow ( GameObject item, int index )
    {
        Debug.Log("=========: " + index);
        ItemElm elm = item.GetComponent<ItemElm>();
        elm.SetLabel(testStrs[index]);
    }

    void onWrapItem ( GameObject item, int index )
    {
        ItemElm elm = item.GetComponent<ItemElm>();
        elm.SetLabel(testStrs[index]);
    }
}
