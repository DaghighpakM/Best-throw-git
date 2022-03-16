using UnityEngine;
using System.Collections;
public class CameraManeger : MonoBehaviour
{
    public float distanceOfStationWithCamera;

    bool _canFollowBall = false;
    bool _isShootedBall = false;

    [SerializeField] Ball _ballCs;
    [SerializeField] float _smoothTime;
    [SerializeField] float _maxSpeedOfLerp;

    Vector3 _velocity;
    Vector3 _nextPointToLerp = new Vector3(0, 0, -10);

    private void Awake()
    {
        StartCoroutine(Update1());
    }

    // void FixedUpdate()
    // {
    //     if (_canFollowBall)
    //     {
    //         _nextPointToLerp.y = _ballCs.humanPos.y;
    //         if (!_isShootedBall)
    //         {
    //             _nextPointToLerp.y += distanceOfStationWithCamera;
    //         }
    //         transform.position = Vector3.SmoothDamp(transform.position, _nextPointToLerp, ref _velocity, _smoothTime, _maxSpeedOfLerp * Time.deltaTime);
    //     }
    // }

    IEnumerator Update1()
    {
        yield return null;

        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (_canFollowBall)
            {
                _nextPointToLerp.y = _ballCs.humanPos.y;
                if (!_isShootedBall)
                {
                    _nextPointToLerp.y += distanceOfStationWithCamera;
                }
                transform.position = Vector3.SmoothDamp(transform.position, _nextPointToLerp, ref _velocity, _smoothTime, _maxSpeedOfLerp * Time.deltaTime);
            }
        }

    }

    public void SetCanFollowBall(bool canFollow)
    {
        _canFollowBall = canFollow;
    }

    public void SetIsShootedBall(bool isShooted)
    {
        _isShootedBall = isShooted;
    }

    public void SetPositionRelativeToStationPosition(Vector2 statonPos)
    {
        transform.position = new Vector3(0, statonPos.y + distanceOfStationWithCamera, -10f);
    }
}
