using UnityEngine;

public interface IMY_SVContent
{
    void Render ();
    void PullBack ();
    void UpdateStopPosition ( bool moveUpOrLeft = true );

    int DataSourceCount { get; set; }
    int DataRenderedIdxUp { get; }
    int RenderIndex { get; set; }
    int DataRenderedIdxDown { get; }
    bool PullingBack { get; set; }
    SpringPanel SpPanel{get;}
    Transform trans { get; }
}
