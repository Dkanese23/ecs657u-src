using System.Collections.Generic;
using UnityEngine;

// Dynamic camera system that automatically frames all combat participants
[RequireComponent(typeof(Camera))]
public class BattleFramingCamera : MonoBehaviour
{
    [Header("Targets")]
    public List<Transform> allTargets = new();

    [Header("Auto facing")]
    public bool faceFromPartyTowardEnemy = true; 
    Transform enemy;
    readonly List<Transform> party = new();

    [Header("View / Framing")]
    [Range(5f, 45f)] public float pitchDown = 15f;
    public float padding = 1.5f;
    public float minDistance = 6f, maxDistance = 20f;
    public float heightOffset = 0f;
    public float followSmooth = 10f;

    Camera cam;
    float currentDistance;

    void Awake()
    {
        cam = GetComponent<Camera>();
        currentDistance = Mathf.Clamp((minDistance + maxDistance) * 0.5f, minDistance, maxDistance);
    }

    // Assigns participants and initialises the list for framing calculations
    public void SetPartyAndEnemy(IEnumerable<Transform> partyList, Transform enemyTransform)
    {
        party.Clear();
        if (partyList != null) foreach (var t in partyList) if (t) party.Add(t);
        enemy = enemyTransform;

        allTargets.Clear();
        allTargets.AddRange(party);
        if (enemy) allTargets.Add(enemy);
    }

    void LateUpdate()
    {
        if (allTargets.Count == 0) return;

        // Calculate the bounding area and center point for all units
        Vector3 min = new(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        Vector3 max = new(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
        Vector3 center = Vector3.zero;
        int count = 0;

        foreach (var t in allTargets)
        {
            if (!t) continue;
            Vector3 p = t.position;
            min = Vector3.Min(min, p);
            max = Vector3.Max(max, p);
            center += p; count++;
        }
        if (count == 0) return;
        center /= count;
        center.y += heightOffset;

        // Calculate horizontal and vertical spans for the framing math
        Vector3 right = Vector3.right;
        float halfWidth = 0f, halfHeight = 0f;
        foreach (var t in allTargets)
        {
            if (!t) continue;
            Vector3 d = t.position - center;
            halfWidth  = Mathf.Max(halfWidth,  Mathf.Abs(Vector3.Dot(d, right)));
            halfHeight = Mathf.Max(halfHeight, Mathf.Abs(d.y));
        }
        halfWidth  += padding;
        halfHeight += padding;

        // Trigonometric FOV calculations to determine the ideal camera distance
        float fovV = Mathf.Deg2Rad * cam.fieldOfView;
        float fovH = 2f * Mathf.Atan(Mathf.Tan(fovV * 0.5f) * cam.aspect);

        float distV = halfHeight / Mathf.Tan(fovV * 0.5f);
        float distH = halfWidth  / Mathf.Tan(fovH * 0.5f);
        float targetDist = Mathf.Clamp(Mathf.Max(distV, distH), minDistance, maxDistance);

        // Orient the camera to face from the party side toward the enemy
        Vector3 forward;
        if (faceFromPartyTowardEnemy && enemy && party.Count > 0)
        {
            Vector3 partyCenter = Vector3.zero;
            int pc = 0; foreach (var t in party) { if (!t) continue; partyCenter += t.position; pc++; }
            if (pc > 0) partyCenter /= pc;
            forward = (enemy.position - partyCenter).normalized; 
        }
        else
        {
            forward = Vector3.forward;
        }

        // Apply smooth interpolation to camera position and rotation
        Quaternion rot = Quaternion.LookRotation(forward, Vector3.up) * Quaternion.Euler(pitchDown, 0f, 0f);
        Vector3 desiredPos = center - (rot * Vector3.forward) * targetDist;

        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * followSmooth);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * followSmooth);
    }
}