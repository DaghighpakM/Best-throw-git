
using UnityEngine;

public class Target : MonoBehaviour
{
    const float EPSILON_DISTANCE = 1f;  // A small distance that prevents it from sticking to other objects or walls

    [HideInInspector] public GameObject activeRotator;

    [SerializeField] GameObject _rotator90, _rotator120, _rotator180;

    [SerializeField] GameObject _barrierPrefab;

    // for move
    Vector2 _firstPoint, _scendPoint, _targetPoint;
    Vector2 _spaceOfMove = new Vector2();
    bool _canMove = false;
    bool _isMoveSeted = false;
    float _smoothTimeForMove;

    GameObject _barrier;
    Barrier _barrierCs; // barrier script
    Vector2 _spaceOfSetBarrier = new Vector2();
    Vector2 _barrierSize1 = new Vector2();
    bool _isBarrierSeted = false, _isBarrierMoveSeted = false, _isBarrierScalingSeted = false;

    Vector2 _spaceOfSetPosition = new Vector2();

    Vector2 _velocity = Vector2.zero;

    Vector2 _guardSize = Vector2.zero;

    // for show and hide animation
    bool _canShow = false, _canHide = false;
    [Range(0.1f, 20f)] [SerializeField] float _showAndHideAnimationSpeed = 10f;
    Vector3 _localScale = Vector3.one;

    Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (_canShow)
        {
            showing();
        }
        else if (_canHide)
        {
            hiding();
        }

        if (_canMove)
        {
            transform.position = Vector2.SmoothDamp(transform.position, _targetPoint, ref _velocity, _smoothTimeForMove);

            if (Vector2.Distance(_targetPoint, transform.position) < 0.1f)
            {
                if (_targetPoint == _firstPoint)
                {
                    _targetPoint = _scendPoint;
                }
                else if (_targetPoint == _scendPoint)
                {
                    _targetPoint = _firstPoint;
                }
            }
        }
    }



    /// <summary>
    /// set the barrier parameters
    /// </summary>
    /// <param name="station"></param>
    /// <param name="barrierSize"></param>
    /// <param name="minHeight"></param>
    /// <param name="maxHeight"></param>
    /// <param name="canMove"></param>
    /// <param name="valueOfScale"></param>
    public void SetBarrier(Transform station, float barrierSize, float minHeight, float maxHeight, bool canMove, float valueOfScale)
    {
        _barrierSize1 = _barrierPrefab.GetComponent<SpriteRenderer>().bounds.size * barrierSize;

        _spaceOfSetBarrier.y = LevelDesigner.Instance.screenToWorldSize.y;
        _spaceOfSetBarrier.y += Camera.main.GetComponent<CameraManeger>().distanceOfStationWithCamera;
        _spaceOfSetBarrier.y -= _guardSize.y;
        _spaceOfSetBarrier.y -= station.GetComponent<SpriteRenderer>().bounds.size.y / 2;
        _spaceOfSetBarrier.y -= _barrierSize1.y;
        _spaceOfSetBarrier.y -= EPSILON_DISTANCE * 3;

        _spaceOfSetBarrier.y *= Random.Range(minHeight, maxHeight);

        _barrier = Instantiate(_barrierPrefab);
        _barrierCs = _barrier.GetComponent<Barrier>();

        _barrierCs.main_scale *= barrierSize;

        _isBarrierSeted = true;

        if (canMove)
        {
            _isBarrierMoveSeted = true;
        }
        else
        {
            _isBarrierMoveSeted = false;
        }

        if (valueOfScale != 0)
        {
            _barrierCs.valueOfScaling = valueOfScale;
            _isBarrierScalingSeted = true;
        }
        else
        {
            _isBarrierScalingSeted = false;
        }
    }

    /// <summary>
    /// set the move parameters for target
    /// </summary>
    /// <param name="station"></param>
    /// <param name="minRangeOfSpaceMove"></param>
    /// <param name="maxRangeOfSpaceMove"></param>
    /// <param name="smoothTimeForMove"></param>
    public void SetMoveTarget(Transform station, Vector2 minRangeOfSpaceMove, Vector2 maxRangeOfSpaceMove, float smoothTimeForMove)
    {
        _spaceOfMove.x = (LevelDesigner.Instance.screenToWorldSize.x * 2) - _guardSize.x - EPSILON_DISTANCE * 2;

        _spaceOfMove.y = LevelDesigner.Instance.screenToWorldSize.y;
        _spaceOfMove.y += Camera.main.GetComponent<CameraManeger>().distanceOfStationWithCamera;
        _spaceOfMove.y -= _guardSize.y;
        _spaceOfMove.y -= station.GetComponent<SpriteRenderer>().bounds.size.y / 2;
        _spaceOfMove.y -= EPSILON_DISTANCE * 2;

        if (_isBarrierSeted)
        {
            _spaceOfMove.y -= _spaceOfSetBarrier.y;
            _spaceOfMove.y -= _barrierSize1.y;
            _spaceOfMove.y -= EPSILON_DISTANCE;

        }

        _spaceOfMove.x *= RandomUtils.RandomWithSignRandom(minRangeOfSpaceMove.x, maxRangeOfSpaceMove.x);
        _spaceOfMove.y *= RandomUtils.RandomWithSignRandom(minRangeOfSpaceMove.y, maxRangeOfSpaceMove.y);

        _firstPoint = _spaceOfMove / 2;

        _scendPoint = _firstPoint * -1;

        if (_spaceOfMove == Vector2.zero) return;

        this._smoothTimeForMove = smoothTimeForMove;

        _isMoveSeted = true;
        if (_velocity != Vector2.zero) _velocity = Vector2.zero;

    }

    /// <summary>
    /// Calculate the position for the target
    /// </summary>
    /// <param name="station"></param>
    /// <param name="minDistance"></param>
    /// <param name="maxDistance"></param>
    public void SetPosition(Transform station, float minDistance, float maxDistance)
    {
        float station_size = station.GetComponent<SpriteRenderer>().bounds.size.y;

        _spaceOfSetPosition.x = (LevelDesigner.Instance.screenToWorldSize.x * 2) - _guardSize.x - EPSILON_DISTANCE;

        _spaceOfSetPosition.y = LevelDesigner.Instance.screenToWorldSize.y;
        _spaceOfSetPosition.y += Camera.main.GetComponent<CameraManeger>().distanceOfStationWithCamera;
        _spaceOfSetPosition.y -= _guardSize.y;
        _spaceOfSetPosition.y -= station_size / 2;
        _spaceOfSetPosition.y -= EPSILON_DISTANCE * 2;

        if (_isMoveSeted)
        {
            _spaceOfSetPosition.x -= Mathf.Abs(_spaceOfMove.x);
            _spaceOfSetPosition.y -= Mathf.Abs(_spaceOfMove.y);
        }

        if (_isBarrierSeted)
        {
            _spaceOfSetPosition.y -= _spaceOfSetBarrier.y;
            _spaceOfSetPosition.y -= _barrierSize1.y;
            _spaceOfSetPosition.y -= EPSILON_DISTANCE;

        }

        _spaceOfSetPosition.y *= Random.Range(minDistance, maxDistance);

        Vector2 position = new Vector2();

        position.x = Random.Range(-_spaceOfSetPosition.x / 2, _spaceOfSetPosition.x / 2);

        position.y = station.position.y;
        position.y += _spaceOfSetPosition.y;
        position.y += _guardSize.y / 2;
        position.y += station_size / 2;
        position.y += EPSILON_DISTANCE;

        if (_isMoveSeted)
        {
            position.y += Mathf.Abs(_spaceOfMove.y / 2);
        }

        if (_isBarrierSeted)
        {
            position.y += _barrierSize1.y;
            position.y += _spaceOfSetBarrier.y;
            position.y += EPSILON_DISTANCE;
        }

        if (_isMoveSeted)
        {
            _firstPoint += position;

            _scendPoint += position;

            _targetPoint = _scendPoint;

            transform.position = _firstPoint;

            DottedLine.instance.DrawDotLine(_firstPoint, _scendPoint);

            _canMove = true;

            if (_isBarrierSeted)
            {
                _barrierCs.SetPosition(station_size / 2 + EPSILON_DISTANCE + _spaceOfSetBarrier.y + _barrierSize1.y, station.position, position);
                if (_isBarrierMoveSeted)
                {
                    _barrierCs.SetMove(station_size / 2 + EPSILON_DISTANCE + _spaceOfSetBarrier.y + _barrierSize1.y, station.position, transform);
                }
                _barrierCs.Show();
            }
        }
        else
        {
            transform.position = position;
            if (_isBarrierSeted)
            {
                _barrierCs.SetPosition(station_size / 2 + EPSILON_DISTANCE + _spaceOfSetBarrier.y + _barrierSize1.y, station.position, position);
                _barrierCs.Show();
            }
        }

        if (_isBarrierSeted)
        {
            if (_isBarrierScalingSeted)
                _barrierCs.SetScalingAnimationParameters();
        }
    }

    /// <summary>
    /// Reset all options
    /// </summary>
    public void ResetToFirstState()
    {
        if (_isBarrierSeted)
        {
            _isBarrierSeted = false;
            if (_isBarrierMoveSeted)
                _barrierCs.StopMove();
            _barrierCs.Hide();
        }

        if (_canMove)
        {
            _canMove = false;
            _isMoveSeted = false;
            DottedLine.instance.DestroyAllDots();
        }

        try
        {
            Transform g = activeRotator.GetComponent<Rotator>().selectedGuard;
            g.tag = LevelDesigner.GUARD_TAG;
            g.GetComponent<EdgeCollider2D>().isTrigger = false;
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }

        if (activeRotator != null)
            activeRotator.GetComponent<Rotator>().DeActive();
    }

    /// <summary>
    /// Hide the guards
    /// </summary>
    public void HideGuards()
    {
        if (_isBarrierSeted)
        {
            _isBarrierSeted = false;

            if (_isBarrierMoveSeted)
                _barrierCs.StopMove();
            _barrierCs.Hide();
        }

        if (_canMove)
        {
            _canMove = false;
            _isMoveSeted = false;
            DottedLine.instance.DestroyAllDots();
        }
        if (activeRotator != null)
        {
            activeRotator.GetComponent<Rotator>().Hide();
        }
    }

    /// <summary>
    /// Show target
    /// </summary>
    public void Show()
    {
        if (transform.localScale == _localScale) return;
        GetComponent<CircleCollider2D>().enabled = true;
        _canShow = true;
    }

    /// <summary>
    /// Hide target
    /// </summary>
    public void Hide()
    {
        if (transform.localScale == Vector3.zero) return;
        GetComponent<CircleCollider2D>().enabled = false;
        _canHide = true;
    }

    /// <summary>
    /// Showing with animation (calling in Update method)
    /// </summary>
    void showing()
    {
        transform.localScale = new Vector2
            (
                transform.localScale.x + _showAndHideAnimationSpeed * Time.deltaTime,
                transform.localScale.y + _showAndHideAnimationSpeed * Time.deltaTime
            );
        if (Vector2.Distance(transform.localScale, _localScale) <= (_showAndHideAnimationSpeed * Time.deltaTime) * 2)
        {
            transform.localScale = _localScale;
            _canShow = false;
        }

    }

    /// <summary>
    /// Hiding with animation (calling in Update method)
    /// </summary>
    void hiding()
    {
        transform.localScale = new Vector2
            (
                transform.localScale.x - _showAndHideAnimationSpeed * Time.deltaTime,
                transform.localScale.y - _showAndHideAnimationSpeed * Time.deltaTime
            );
        if (Vector2.Distance(transform.localScale, Vector2.zero) <= (_showAndHideAnimationSpeed * Time.deltaTime) * 2)
        {
            transform.localScale = Vector2.zero;

            _canHide = false;
        }
    }

    /// <summary>
    /// Activate the rotator according to the selected guard type
    /// </summary>
    /// <param name="guardType"></param>
    public void SetGuards(GuardType guardType)
    {
        switch (guardType)
        {
            case GuardType._4Guard:
            {
                activeRotator = _rotator90;
                activeRotator.SetActive(true);
            }
            break;
            case GuardType._3Guard:
            {
                activeRotator = _rotator120;
                activeRotator.SetActive(true);
            }
            break;
            case GuardType._2Guard:
            {
                activeRotator = _rotator180;
                activeRotator.SetActive(true);
            }
            break;
        }
        if (activeRotator != null)
        {
            _guardSize = activeRotator.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite.bounds.size;
        }
    }
}
