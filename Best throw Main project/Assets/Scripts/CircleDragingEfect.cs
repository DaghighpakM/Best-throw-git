using UnityEngine;

public class CircleDragingEfect : MonoBehaviour
{
    [SerializeField] Transform _ball;
    Vector2 _stationPos;
    float _spriteSize;
    SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteSize = _spriteRenderer.sprite.bounds.size.x - 0.5f;
        transform.localScale = Vector2.zero;
    }

    public void ShowDragingEfect(Vector2 stationPos)
    {
        transform.position = stationPos;
        this._stationPos = stationPos;
        _spriteRenderer.color = _ball.GetComponent<SpriteRenderer>().color;
    }

    public void UpdateDragingEfect(float colorAlpha)
    {
        colorAlpha *= 0.4f;
        float scale = Vector2.Distance(_ball.position, _stationPos) / _spriteSize * 2;
        transform.localScale = new Vector2(scale, scale);
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, colorAlpha);
    }

    public void HideDragingEfect()
    {
        transform.localScale = Vector2.zero;
    }
}
