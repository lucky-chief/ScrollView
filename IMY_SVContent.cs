using UnityEngine;

public interface IMY_SVContent
{
    void Render ();
    void PullBack ();
    void UpdateStopPosition ( bool moveUpOrLeft = true );

    int DataSourceCount { get; set; }
    int MinIndex { get; }
    int RenderIndex { get; set; }
    int MaxIndex { get; }
    bool PullingBack { get; set; }
    bool Pressed { get; set; }
    SpringPanel SpPanel{get;}
    Transform trans { get; }
}
