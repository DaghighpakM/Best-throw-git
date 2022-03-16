using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
    public bool canMoveToTarget { get; set; }
    public Vector2 humanPos { get { return transform.position; } }

    [SerializeField] float _speedOfMoveToTarget;

    [SerializeField] GameObject _vFXOfBallCollisionWithWallPrefab;
    [SerializeField] Transform _wall_L, _wall_R;

    /// score options
    int _excellentNumberShootHistory = 1;  // number of Shooted without colliding with other guards to get more score
    int _jumpShootHistory = 0;   // number of _ballCs collision with wall or obstacle increases the score

    Rigidbody2D _rb;
    CircleCollider2D _col;
    Vector2 _target_position;

    // for loss
    float _stationYPositaion;
    float _stationColliderRadius;
    float _ballColloderDiameter;
    bool _canCheckForLossing = false;

    Color _nextColor;

    public ParticleSystem ballTrailer;

    CameraManeger _cameraManeger;

    int _newScoreReceive;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CircleCollider2D>();
        _cameraManeger = Camera.main.GetComponent<CameraManeger>();
        _ballColloderDiameter = _col.radius * 2;
        SetCanCheckForLossing(false);
        ballTrailer.Stop();
       
    }

    private void Update()
    {
        if (_canCheckForLossing)
        {
            if (transform.position.y < _stationYPositaion - _stationColliderRadius - _ballColloderDiameter)
            {
                SetCanCheckForLossing(false);
                GameManeger.Instance.Loss();
            }
        }
        if (canMoveToTarget)
        {
            transform.position = Vector2.Lerp(transform.position, _target_position, Time.deltaTime * _speedOfMoveToTarget);

            if (Vector2.Distance(transform.position, _target_position) < 0.1F)
            {
                canMoveToTarget = false;
                transform.position = _target_position;
            }
        }
    }

    /// <summary>
    /// set the Activity of Rigidbody2D 
    /// </summary>
    /// <param name="active"></param>
    public void SetActiveRb(bool active)
    {
        _rb.isKinematic = !active;
        if (!active)
            _rb.velocity = Vector2.zero;
    }

    /// <summary>
    /// push to Rigidbody2D for throw _ballCs
    /// </summary>
    /// <param name="force"></param>
    public void Add_force(Vector2 force)
    {
        _rb.AddForce(force, ForceMode2D.Impulse);
        _cameraManeger.SetIsShootedBall(true);
        _jumpShootHistory = 0;

    }

    public void SetCanCheckForLossing(bool canCheck)
    {
        _canCheckForLossing = canCheck;
    }

    // set ball color
    public void SetColor(Color c)
    {
        transform.GetComponent<SpriteRenderer>().color = c;
        var main = ballTrailer.main;
        main.startColor = c;
    }

    //called in Go to Target animation for change Color
    void changeColor()
    {
        SetColor(_nextColor);
    }

    //set nextColor color
    public void SetNextColor(Color c)
    {
        _nextColor = c;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contactPoint = collision.contacts[0];

        switch (collision.transform.tag)
        {
            case "Wall L":
            {
                AudioAndVibrationManeger.instance.play("Collosion with wall");
                GameObject gL = Instantiate(_vFXOfBallCollisionWithWallPrefab);
                gL.GetComponent<CollitionWithWall>().position = contactPoint.point;
                _jumpShootHistory++;
            }
            break;

            case "Wall R":
            {
                AudioAndVibrationManeger.instance.play("Collosion with wall");
                GameObject gR = Instantiate(_vFXOfBallCollisionWithWallPrefab);
                gR.GetComponent<CollitionWithWall>().position = contactPoint.point;
                _jumpShootHistory++;
            }
            break;

            case "Barrier":
            {
                AudioAndVibrationManeger.instance.play("Collosion with barrier");
                _jumpShootHistory++;
            }
            break;

            case LevelDesigner.GUARD_TAG:
            {
                AudioAndVibrationManeger.instance.play("Collosion with fail guard");

                Color c1, c2;
                c1 = GetComponent<SpriteRenderer>().color;
                c2 = collision.transform.GetComponent<SpriteRenderer>().color;

                VFXManeger.Instance.PlayBallToGuardVFX(contactPoint.point, c1, c2);

                if (!canMoveToTarget)
                {
                    _jumpShootHistory = 0;
                    _excellentNumberShootHistory = 0;
                }
                AudioAndVibrationManeger.instance.PlayCollidedBallWithFailGuardVibration();
            }
            break;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case LevelDesigner.SELECTED_GUARD_TAG:
            {
                collision.GetComponent<Animator>().SetBool("press", true);
            }
            break;
            case LevelDesigner.STATION_TAG:
            {
                _cameraManeger.SetIsShootedBall(false);
                SetCanCheckForLossing(false);
                if (!GameManeger.Instance.isDraging && transform.position.y > collision.transform.position.y)
                {
                    _stationYPositaion = collision.transform.position.y;
                    _stationColliderRadius = collision.GetComponent<CircleCollider2D>().radius;
                }
                       
                SetActiveRb(false);
                _target_position = collision.transform.position; 
                
                GameManeger.Instance.ReturnedToStation();
                if (transform.position != collision.transform.position && !GameManeger.Instance.isDraging)
                    canMoveToTarget = true;

                AudioAndVibrationManeger.instance.PlayWhenBallWentToStationOrTarget();
            }
            break;
            case LevelDesigner.TARGET_TAG:
            {
                _cameraManeger.SetIsShootedBall(false);

                SetCanCheckForLossing(false);
                _stationYPositaion = collision.transform.position.y;
                _stationColliderRadius = collision.GetComponent<CircleCollider2D>().radius;

                _target_position = collision.transform.position;
                SetActiveRb(false);
                canMoveToTarget = true;

                collision.transform.GetComponent<Target>().HideGuards();

                _newScoreReceive = 1;
                if (_excellentNumberShootHistory > 0)
                {
                    _newScoreReceive += _excellentNumberShootHistory;

                }
                if (_jumpShootHistory > 0)
                {
                    _newScoreReceive *= (_jumpShootHistory + 1);
                }

                PlayGoalSound(_excellentNumberShootHistory);

                ScoreReceiveVFXManeger.Instance.ShowNewScoreReceiveVFX(collision.transform, transform.GetComponent<SpriteRenderer>().color, _newScoreReceive, _excellentNumberShootHistory, _jumpShootHistory);
                if (_excellentNumberShootHistory < 10)
                {
                    _excellentNumberShootHistory++;
                }

                EdgeCollider2D edgeColliderGuard = collision.GetComponent<Target>().activeRotator.GetComponent<Rotator>().selectedGuard.GetComponent<EdgeCollider2D>();
                edgeColliderGuard.isTrigger = false;
                edgeColliderGuard.tag = LevelDesigner.GUARD_TAG;

                collision.tag = LevelDesigner.STATION_TAG;

                GameManeger.Instance.SuccessfulThrow(_newScoreReceive);
                GetComponent<Animator>().SetTrigger("Ball Go To Target");

                AudioAndVibrationManeger.instance.PlayWhenBallWentToStationOrTarget();
            }
            break;
            case LevelDesigner.PAUSEDER_BALL_TAG:
            {
                SetActiveRb(false);
            }
            break;
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == LevelDesigner.SELECTED_GUARD_TAG)
        {
            collision.GetComponent<Animator>().SetBool("press", false); // pressed animation pause
        }
        if (collision.tag == LevelDesigner.STATION_TAG || collision.tag == LevelDesigner.SHOOTER_TAG)
        {
            if (GameManeger.Instance.isDraging)
            {
                collision.tag = LevelDesigner.SHOOTER_TAG;
            }
            else
            {
                collision.tag = LevelDesigner.STATION_TAG;
                SetCanCheckForLossing(true);
            }
        }
    }

    /// <summary>
    /// reset the options of ball after reset game or continuing after loss 
    /// </summary>
    /// <param name="pos"></param>
    public void ResetOptions(Vector2 stationPos)
    {
        SetActiveRb(false);

        _stationYPositaion = stationPos.y;
        SetCanCheckForLossing(false);

        transform.position = new Vector2(stationPos.x, stationPos.y + 2f);
        SetActiveRb(true);
        _excellentNumberShootHistory = 1;
    }

    private void PlayGoalSound(int excellentHistoryNumber)
    {
        string soundName = "";
        switch (excellentHistoryNumber)
        {
            case 0:
                soundName = "Goal sound 0";
                break;
            case 1:
                soundName = "Goal sound 1";
                break;
            case 2:
                soundName = "Goal sound 2";
                break;
            case 3:
                soundName = "Goal sound 3";
                break;
            case 4:
                soundName = "Goal sound 4";
                break;
            case 5:
                soundName = "Goal sound 5";
                break;
            case 6:
                soundName = "Goal sound 6";
                break;
            case 7:
                soundName = "Goal sound 7";
                break;
            case 8:
                soundName = "Goal sound 8";
                break;
            case 9:
                soundName = "Goal sound 9";
                break;
            case 10:
                soundName = "Goal sound 10";
                break;
            default:
                break;
        }
        AudioAndVibrationManeger.instance.play(soundName);
    }
}
