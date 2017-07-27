using UnityEngine;
using System.Collections;
using System;

public class DemoTest : MonoBehaviour
{
    // string[] testStrs = { "AAAAAA", "AAAAAA\nAAAA", "AAAAAA\nAAA\nAAAAAAA", "AAAAAA", "AAAAAA\nAAA\nAAAAAAA\nAAAAAAAAAAAAAA","bbbb", "AAAAAA\nAAAA", "AAAAAA\nAAA\nAAAAAAA", "AAAAAA", "AAAAAA\nAAA\nAAAAAAA\nAAAAAAAAAAAAAA", "ccccc", "AAAAAA\nAAAA", "AAAAAA\nAAA\nAAAAAAA", "AAAAAA", "AAAAAA\nAAA\nAAAAAAA\nAAAAAAAAAAAAAA", "bbbb", "AAAAAA\nAAAA", "AAAAAA\nAAA\nAAAAAAA", "AAAAAA", "AAAAAA\nAAA\nAAAAAAA\nAAAAAAAAAAAAAA", "ccccc" };
    // string[] testStrs = { "1-1-1-1", "2-2-2-2-2", "3-3-3-3-3", "4-4-4-4-4", "5-5-5-5-5", "6-6-6-6-6", "7-7-7-7-7", "8-8-8-8-8", "9-9-9-9-9", "10-10-10-10", "11-11-11-11" };
     string[] testStrs = { "1-1-1-1\nAAAAAAAAAAA\nBBBBBBBBBBB", "2-2-2-2-2", "3-3-3-3-3\nAAAAAAAAAAA\nBBBBBBBBBBB\nAAAAAAAAAAA\nBBBBBBBBBBB\nAAAAAAAAAAA\nBBBBBBBBBBB", "4-4-4-4-4\nAAAAAAAAAAA\nBBBBBBBBBBB", "5-5-5-5-5\nAAAAAAAAAAA\nBBBBBBBBBBB\nAAAAAAAAAAA\nBBBBBBBBBBB", "6-6-6-6-6\nAAAAAAAAAAA\nBBBBBBBBBBB", "7-7-7-7-7\nAAAAAAAAAAA\nBBBBBBBBBBB\nAAAAAAAAAAA\nBBBBBBBBBBB\nAAAAAAAAAAA\nBBBBBBBBBBB", "8-8-8-8-8\nAAAAAAAAAAA\nBBBBBBBBBBB", "9-9-9-9-9\nAAAAAAAAAAA\nBBBBBBBBBBB\nAAAAAAAAAAA\nBBBBBBBBBBB", "10-10-10-10\nAAAAAAAAAAA\nBBBBBBBBBBB", "11-11-11-11" };
    // string[] testStrs = { "AAAAAA", "AAAAAA" , "AAAAAA" };

    public MY_ScrollView scrollView;
    // Use this for initialization
    void Start ()
    {
        scrollView.renderItemChange += ItemChange;
        scrollView.Init(testStrs.Length + 1);
    }

    private void ItemChange ( GameObject item, int dataIndex )
    {
        ItemElm elm = item.GetComponent<ItemElm>();
        elm.SetLabel(testStrs[dataIndex]);
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
