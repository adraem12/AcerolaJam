using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SplineMesh 
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class ExampleTentacle : MonoBehaviour 
    {
        private Spline spline { get => GetComponent<Spline>(); }
        public float startScale = 1, endScale = 1;
        public float startRoll = 0, endRoll = 0;

        private void OnValidate() 
        {
            float currentLength = 0;
            foreach (CubicBezierCurve curve in spline.GetCurves()) 
            {
                float startRate = currentLength / spline.Length;
                currentLength += curve.Length;
                float endRate = currentLength / spline.Length;
                curve.n1.Scale = Vector2.one * (startScale + (endScale - startScale) * startRate);
                curve.n2.Scale = Vector2.one * (startScale + (endScale - startScale) * endRate);
                curve.n1.Roll = startRoll + (endRoll - startRoll) * startRate;
                curve.n2.Roll = startRoll + (endRoll - startRoll) * endRate;
            }
        }
    }
}