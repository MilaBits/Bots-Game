using UnityEngine;

public class ProgressPath
{
    public Vector3 start;
    public Vector3 end;
    private float currentProgress;
    private float fullProgress;
    public Transform target;

    public ProgressPath(Vector3 start, Vector3 end, Transform target)
    {
        this.start = start;
        this.end = end;
        this.target = target;
        fullProgress = Vector3.Distance(start, end);
    }

    public void SetProgress(float progress)
    {
        target.transform.position = Vector3.Lerp(start, end, progress / fullProgress);
    }

    public void Move(float progress)
    {
        currentProgress = Mathf.Clamp(currentProgress + progress, 0, fullProgress);
        target.transform.position = Vector3.Lerp(start, end, currentProgress / fullProgress);
    }
}