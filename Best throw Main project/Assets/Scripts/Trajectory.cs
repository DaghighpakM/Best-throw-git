using UnityEngine;

public class Trajectory : MonoBehaviour
{
    [SerializeField] int _dotsNumber;
    [SerializeField] GameObject _dotsParent;
    [SerializeField] GameObject _dotPrefab;
    [SerializeField] float _dotsSpacing;
    [SerializeField] float _deltaScaleForNextDots;
    [SerializeField] Color _dotsColor;

    Vector2 _scaleOfDot = new Vector2(1f, 1f);
    public float _screenToWorldSizeX { set; get; }

    Vector2 _pos;

    float _timeStamp;

    Transform[] _dotsList;

    private void Start()
    {
        Hide();
        prepareDots();
    }

    void prepareDots()
    {
        _dotsList = new Transform[_dotsNumber];

        for (int i = 0; i < _dotsNumber; i++)
        {
            _dotsList[i] = Instantiate(_dotPrefab, null).transform;

            _dotsList[i].parent = _dotsParent.transform;

            _scaleOfDot.x -= _deltaScaleForNextDots;
            _scaleOfDot.y -= _deltaScaleForNextDots;

            _dotsList[i].localScale = _scaleOfDot;
        }
    }

    public void UpdateDots(Vector3 ballPos, Vector2 ForceAplided, float alpha_Of_color)
    {
        _timeStamp = _dotsSpacing;
        for (int i = 0; i < _dotsNumber; i++)
        {
            _pos.x = (ballPos.x + ForceAplided.x * _timeStamp);
            _pos.y = (ballPos.y + ForceAplided.y * _timeStamp) - (Physics2D.gravity.magnitude * _timeStamp * _timeStamp) / 2f;

            if (_pos.x > _screenToWorldSizeX)
            {
                float h = _pos.x - _screenToWorldSizeX;
                _pos.x = _pos.x - h * 2;
            }
            else if (_pos.x < -_screenToWorldSizeX)
            {
                float h = _pos.x + _screenToWorldSizeX;
                _pos.x = _pos.x - h * 2;
            }

            _dotsList[i].position = _pos;

            _dotsColor.a = alpha_Of_color;
            _dotsList[i].GetComponent<SpriteRenderer>().color = _dotsColor;
            _timeStamp += _dotsSpacing;
        }
    }

    public void Hide()
    {
        _dotsParent.SetActive(false);
    }

    public void Show()
    {
        _dotsParent.SetActive(true);
    }
}
