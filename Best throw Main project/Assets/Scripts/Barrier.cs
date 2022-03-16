using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    [HideInInspector]
    public Vector2 main_scale = Vector2.one;  // if is scaling animation , this is first scale!

    bool _canShowing, _canHiding; // for hide and show animation!

    [Range(0.01f, 2f)]
    [SerializeField]
    float _speedOfShowAndHide;

    // move and position parameter
    bool _canMove = false;
    Vector2 _stationPos;
    float _barrierHeight;
    Transform _target;
    Vector2 _pos = new Vector2();

    // scaling animation parameter
    bool _canScale = false;
    [HideInInspector] public float valueOfScaling;
    Vector2 _secondScale, _targetScale;
    [Range(0.1f, 2f)]
    [SerializeField] float _scalingSpeed;

    private void Start()
    {
        transform.localScale = Vector2.zero;
        if (_speedOfShowAndHide <= 0)
        {
            _speedOfShowAndHide = 10f;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (_canMove)
        {
            SetPosition(_barrierHeight, _stationPos, _target.position);
        }

        if (_canShowing)
        {
            showing();
        }
        else if (_canHiding)
        {
            hiding();
        }
        else if (_canScale)
        {
            scaling();
        }

    }

    /// <summary>
    /// set the barrier position
    /// </summary>
    /// <param name="barrier_height">barrier height from station</param>
    /// <param name="station_pos"></param>
    /// <param name="target_pos"></param>
    public void SetPosition(float barrier_height, Vector2 station_pos, Vector2 target_pos)
    {
        _pos.x = station_pos.x + barrier_height * (target_pos.x - station_pos.x) / (target_pos.y - station_pos.y);
        _pos.y = station_pos.y + barrier_height;

        transform.position = _pos;
    }

    /// <summary>
    /// set move parameter
    /// </summary>
    /// <param name="barrier_height"></param>
    /// <param name="station_pos"></param>
    /// <param name="target"></param>
    public void SetMove(float barrier_height, Vector2 station_pos, Transform target)
    {
        this._target = target;
        this._barrierHeight = barrier_height;
        this._stationPos = station_pos;
        _canMove = true;
    }

    public void SetScalingAnimationParameters()
    {
        _secondScale = new Vector2(main_scale.x - valueOfScaling, main_scale.y - valueOfScaling);
        _targetScale = _secondScale;
        if (_scalingSpeed > 0)
            _scalingSpeed *= -1;
        _canScale = true;
    }

    // scaling animation
    void scaling()
    {
        transform.localScale = new Vector2(
            transform.localScale.x + _scalingSpeed * Time.deltaTime,
            transform.localScale.y + _scalingSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.localScale, _targetScale) <= Mathf.Abs(_scalingSpeed * Time.deltaTime * 2))
        {
            if (_targetScale == main_scale)
            {
                _targetScale = _secondScale;
                if (_scalingSpeed > 0)
                {
                    _scalingSpeed *= -1;
                }
            }
            else if (_targetScale == _secondScale)
            {
                _targetScale = main_scale;
                if (_scalingSpeed < 0)
                {
                    _scalingSpeed *= -1;
                }
            }
        }
    }
    /// <summary>
    /// stop moving
    /// </summary>
    public void StopMove()
    {
        _canMove = false;
    }
    /// <summary>
    /// show with animation
    /// </summary>
    public void Show()
    {
        _canShowing = true;
    }

    /// <summary>
    /// hide with animation
    /// </summary>
    public void Hide()
    {
        _canHiding = true;
    }

    /// <summary>
    /// called in update
    /// </summary>
    void showing()
    {

        transform.localScale = new Vector2(transform.localScale.x + _speedOfShowAndHide * Time.deltaTime, transform.localScale.y + _speedOfShowAndHide * Time.deltaTime);
        if (Vector2.Distance(transform.localScale, main_scale) <= (_speedOfShowAndHide * Time.deltaTime) * 2)
        {
            transform.localScale = main_scale;
            _canShowing = false;
        }
    }

    /// <summary>
    /// called in update
    /// </summary>
    void hiding()
    {
        transform.localScale = new Vector2(transform.localScale.x - _speedOfShowAndHide * Time.deltaTime, transform.localScale.y - _speedOfShowAndHide * Time.deltaTime);
        if (Vector2.Distance(transform.localScale, Vector2.zero) <= (_speedOfShowAndHide * Time.deltaTime) * 2)
        {
            transform.localScale = Vector2.zero;
            _canHiding = false;
            Destroy(gameObject);
        }
    }
}
