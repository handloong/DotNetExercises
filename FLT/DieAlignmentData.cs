/// <summary>
/// 表示芯片（Die）或晶圆（Wafer）在对准过程中的位置和角度数据。
/// </summary>
public struct DieAlignmentData
{
    /// <summary>
    /// Stage（平台）在 X 轴方向的位置。
    /// 单位：毫米（mm）或根据系统设定而定。
    /// </summary>
    public double StageX;

    /// <summary>
    /// Stage（平台）在 Y 轴方向的位置。
    /// 单位：毫米（mm）或根据系统设定而定。
    /// </summary>
    public double StageY;

    /// <summary>
    /// Stage 的旋转角度。
    /// 单位：弧度（radians），也可能为角度（degrees），需视具体系统而定。
    /// </summary>
    public double StageAngle;

    /// <summary>
    /// 在 X 方向上相对于参考位置的微调偏移量。
    /// 单位：微米（μm）
    /// </summary>
    public double DeltaXMic;

    /// <summary>
    /// 在 Y 方向上相对于参考位置的微调偏移量。
    /// 单位：微米（μm）
    /// </summary>
    public double DeltaYMic;

    /// <summary>
    /// 相对于参考角度的旋转偏移量。
    /// 单位：角度（degrees）
    /// </summary>
    public double AngleDeg;
}