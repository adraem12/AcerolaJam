using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public static class Bezier
{
    public static Vector3 QuadraticBezierCurve(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 pos = 2 * (c - (2 * b) + a);
        return pos;
    }

    public static Vector3 CubicBezierCurve(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 pos = Mathf.Pow(1 - t, 3) * a + 3 * Mathf.Pow(1 - t, 2) * t * b + 3 * (1 - t) * Mathf.Pow(t, 2) * c + Mathf.Pow(t, 3) * d;
        return pos;
    }

    public static Vector3 LinearBezierCurve(Vector3 a, Vector3 b, float t)
    {
        Vector3 pos = a + t * (b - a);
        return pos;
    }
    public static float LinearFloatCurve(float a, float b, float t)
    {
        float ans = a + t * (b - a);
        return ans;
    }
}

public class PlayerWeapon : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] Transform weaponModel;
    public Transform weaponController;
    [SerializeField] Transform[] chainPoints;
    [SerializeField] LineRenderer chainLine;
    [SerializeField] LayerMask wallLayers;
    PlayerController player;
    List<Vector3> newPoints = new();
    List<Vector3> finalPoints = new();
    float currentDis, slidingRequired, requiredDis;
    public bool performingAttack;
    public bool canDamage;
    [Header("Adjustments")]
    [SerializeField] float curveSizeMultiplier = 1;
    [Range(0.02f, 0.2f)][SerializeField] float curveSmoothing = 0.1f;
    [Range(0, 2)][SerializeField] float rayDistance = 1;

    private void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }

    private void LateUpdate()
    {
        CheckWalls();
        GetNewPoints();
    }

    private void CheckWalls()
    {
        Vector3 startPoint = chainPoints[0].position;
        Vector3 tipPoint = Bezier.LinearBezierCurve(startPoint, weaponController.position, rayDistance);
        Debug.DrawLine(startPoint, tipPoint, Color.blue);
        requiredDis = (tipPoint - startPoint).magnitude;
        if(performingAttack)
            CheckEnemies(startPoint, tipPoint);
        if (Physics.Linecast(startPoint, tipPoint, out RaycastHit hit, wallLayers))
        {
            Vector3 disModifier = Bezier.LinearBezierCurve(startPoint, hit.point, 1);
            Debug.DrawLine(startPoint, disModifier, Color.magenta);
            currentDis = (startPoint - disModifier).magnitude;
            if (currentDis < requiredDis) 
                slidingRequired = currentDis / requiredDis; else slidingRequired = 1;
        }
        else
            slidingRequired = 1;
    }

    private void CheckEnemies(Vector3 startPoint, Vector3 tipPoint)
    {
        requiredDis = (tipPoint - startPoint).magnitude;
        if (Physics.Linecast(startPoint, tipPoint, out RaycastHit hit) && hit.collider.CompareTag("Enemy") && canDamage)
        {
            hit.collider.GetComponent<EnemyController>().TakeDamage(player.Damage);
            canDamage = false;
        }
    }

    void GetNewPoints()
    {
        newPoints.Clear(); 
        Vector3 firstPoint = chainPoints[0].position;
        newPoints.Add(firstPoint);
        Vector3 secondPoint = firstPoint + chainPoints[0].localScale.x * curveSizeMultiplier * GetDir(0);
        secondPoint = GetDistance(firstPoint, secondPoint);
        newPoints.Add(secondPoint);
        Vector3 finalPoint = GetDistance(chainPoints[0].position, chainPoints[1].position);
        Vector3 thirdPoint = finalPoint - chainPoints[1].localScale.x * curveSizeMultiplier * GetDir(1);
        thirdPoint = GetDistance(finalPoint, thirdPoint);
        newPoints.Add(thirdPoint);
        newPoints.Add(finalPoint);
        SubdividePoints();
    }

    Vector3 GetDir(int i)
    {
        Vector3 calculationDir = chainPoints[i].forward;
        return calculationDir;
    }

    Vector3 GetDistance(Vector3 prevPoint, Vector3 curPoint)
    {
        Vector3 pos = Bezier.LinearBezierCurve(prevPoint, curPoint, slidingRequired);
        return pos;
    }

    void SubdividePoints()
    {
        finalPoints.Clear();
        for (int i = 0; i < newPoints.Count - 1; i += 3)
            for (float j = 0; j < 1; j += curveSmoothing)
            {
                Vector3 points = Bezier.CubicBezierCurve(newPoints[i], newPoints[i + 1], newPoints[i + 2], newPoints[i + 3], j);
                finalPoints.Add(points);
            }
        finalPoints[0] = chainPoints[0].position;
        finalPoints[^1] = newPoints[^1];
        UpdateChainRenderer();
    }

    void UpdateChainRenderer()
    {
        if (finalPoints.Count <= 0) 
            return;
        weaponModel.position = newPoints[^1];
        chainLine.positionCount = finalPoints.Count;
        for (int i = 0; i < finalPoints.Count; i++)
            chainLine.SetPosition(i, finalPoints[i]);
    }

    void OnDrawGizmos()
    {
        if (newPoints.Count <= 0) 
            return;
        Gizmos.color = Color.yellow;
        for (int i = 0; i < newPoints.Count; i++)
        {
            Gizmos.DrawSphere(newPoints[i], 0.03f);
            if (i < newPoints.Count - 1) Gizmos.DrawLine(newPoints[i], newPoints[i + 1]);
        }
        if (finalPoints.Count <= 0) 
            return;
        Gizmos.color = Color.red;
        for (int i = 0; i < finalPoints.Count; i++)
        {
            Gizmos.DrawSphere(finalPoints[i], 0.01f);
            if (i < finalPoints.Count - 1) Gizmos.DrawLine(finalPoints[i], finalPoints[i + 1]);
        }
    }
}