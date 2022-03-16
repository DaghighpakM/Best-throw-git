using UnityEngine;
using System.Collections.Generic;

public class DottedLine : MonoBehaviour
{
    public static DottedLine instance;

    [Range(0.01f, 1f)]
    [SerializeField] float _size;

    [Range(0.01f, 2f)]
    [SerializeField] float _distance;

    [SerializeField] Sprite _dotSprite;

    [SerializeField] Color _color;

    List<GameObject> _dots = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    GameObject createDot()
    {
        GameObject dot = new GameObject("dot");
        dot.transform.localScale = Vector2.one * _size;
        dot.transform.parent = transform;

        SpriteRenderer sr = dot.AddComponent<SpriteRenderer>();
        sr.sprite = _dotSprite;
        sr.color = _color;

        return dot;
    }

    /// <summary>
    /// Show the path of move target with dots
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    public void DrawDotLine(Vector2 startPoint, Vector2 endPoint)
    {
        Vector2 point = startPoint;
        Vector2 direction = (endPoint - startPoint).normalized;

        while (Vector2.Distance(point, startPoint) < Vector2.Distance(endPoint, startPoint))
        {
            GameObject dot = createDot();
            dot.transform.position = point;
            _dots.Add(dot);
            point += (direction * _distance);
        }
    }

    public void DestroyAllDots()
    {
        foreach (GameObject dot in _dots)
        {
            Destroy(dot);
        }
        _dots.Clear();
    }
}


