using SplineMesh;
using UnityEngine;

[ExecuteInEditMode]
public class WhipGenerator : MonoBehaviour
{
    private Spline Spline { get => GetComponent<Spline>(); }
    public float startScale = 1, endScale = 1;
    public float startRoll = 1, endRoll = 1;

    private void OnValidate()
    {
        float currentLength = 0;
        foreach (CubicBezierCurve curve in Spline.GetCurves())
        {
            float startRate = currentLength / Spline.Length;
            currentLength += curve.Length;
            float endRate = currentLength / Spline.Length;
            curve.n1.Scale = Vector2.one * (startScale + (endScale - startScale) * startRate);
            curve.n2.Scale = Vector2.one * (startScale + (endScale - startScale) * endRate);
            curve.n1.Roll = startRoll + (endRoll - startRoll) * startRate;
            curve.n2.Roll = startRoll + (endRoll - startRoll) * endRate;
        }
    }
}