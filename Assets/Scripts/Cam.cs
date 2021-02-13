using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public List<Transform> leaders;
    public Vector3 offset;
    public float smooth;
    Vector3 _velocity;

    public float minZoom;
    public float maxZoom;
    public float zoomLimit;

    Camera _cam;
    private void Awake()
    {
        _cam = GetComponent<Camera>();
        GetLeaders();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (leaders.Count <= 0)
            return;

        transform.position = Vector3.SmoothDamp(transform.position, Center() + offset, ref _velocity, smooth);
        float zoom = Mathf.Lerp(maxZoom, minZoom, MaxDistance());
        _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, zoom, Time.deltaTime);
    }

    float MaxDistance()
    {
        Bounds bounds = new Bounds();

        for (int i = 0; i < leaders.Count; i++)
        {
            if (leaders[i] != null)
                bounds.Encapsulate(leaders[i].position);
        }
        return bounds.size.x;
    }
    Vector3 Center()
    {
        Bounds bounds = new Bounds();

        for (int i = 0; i < leaders.Count; i++)
        {
            if (leaders[i] != null)
                bounds.Encapsulate(leaders[i].position);
        }
        return bounds.center;
    }

    void GetLeaders()
    {
        var l = FindObjectsOfType<Leader>();

        foreach (var item in l)
        {
            leaders.Add(item.transform);
        }
    }
}
