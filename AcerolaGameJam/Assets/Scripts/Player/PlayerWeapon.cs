using System.Collections.Generic;
using UnityEngine;
using SplineMesh;
using System.Linq;

public static class Bezier
{
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
}

public class PlayerWeapon : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] Transform weaponEndPoint;
    public Transform weaponController;
    [SerializeField] Transform[] whipPoints;
    [SerializeField] Spline whipSpline;
    [SerializeField] LayerMask wallLayers;
    PlayerController player;
    readonly List<Vector3> newPoints = new();
    readonly List<Vector3> finalPoints = new();
    float currentDis, slidingRequired, requiredDis;
    [HideInInspector] public bool performingAttack;
    [HideInInspector] public bool canDamage;
    [Header("Adjustments")]
    [SerializeField] float curveSizeMultiplier = 1;
    [Range(0, 2)][SerializeField] float rayDistance = 1;
    [SerializeField] float frequency = 1;
    [SerializeField] float magnitude = 1;


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
        Vector3 startPoint = whipPoints[0].position;
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
        Vector3 firstPoint = whipPoints[0].position - player.transform.position;
        Vector3 secondPoint = firstPoint + curveSizeMultiplier * whipPoints[0].forward;
        Vector3 finalPoint = GetDistance(whipPoints[0].position - player.transform.position, whipPoints[1].position - player.transform.position);
        Vector3 thirdPoint = finalPoint - curveSizeMultiplier * whipPoints[1].forward;
        secondPoint += (magnitude * Mathf.Cos(Time.time * frequency) * transform.right + magnitude * Mathf.Sin(Time.time * frequency) * transform.up) * Mathf.InverseLerp(0, player.Range, Vector3.Distance(firstPoint, finalPoint));
        thirdPoint += (magnitude * Mathf.Sin(Time.time * frequency) * transform.right + magnitude * Mathf.Cos(Time.time * frequency) * transform.up) * Mathf.InverseLerp(0, player.Range, Vector3.Distance(firstPoint, finalPoint));
        secondPoint = GetDistance(firstPoint, secondPoint);
        thirdPoint = GetDistance(finalPoint, thirdPoint);
        newPoints.Add(firstPoint);
        newPoints.Add(secondPoint);
        newPoints.Add(thirdPoint);
        newPoints.Add(finalPoint);
        SubdividePoints();
        
    }

    Vector3 GetDistance(Vector3 prevPoint, Vector3 curPoint)
    {
        Vector3 pos = Bezier.LinearBezierCurve(prevPoint, curPoint, slidingRequired);
        return pos;
    }

    void SubdividePoints()
    {
        finalPoints.Clear();
        for (float i = 0; i < 1; i += 0.1f)
        {
            Vector3 point = Bezier.CubicBezierCurve(newPoints[0], newPoints[1], newPoints[2], newPoints[3], i);
            finalPoints.Add(point);
        }
        finalPoints[0] = newPoints.First();
        finalPoints[^1] = newPoints.Last();
        UpdateSpline();
    }

    void UpdateSpline()
    {
        weaponEndPoint.position = newPoints.Last();
        for (int i = 0; i < finalPoints.Count; i++)
            whipSpline.nodes[i].Position = Quaternion.LookRotation(new Vector3(-player.LookDirection.x, 0, player.LookDirection.z)) * finalPoints[i];
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