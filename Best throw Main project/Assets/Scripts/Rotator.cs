using UnityEngine;

/// <summary>
/// Rotator of guards.
/// </summary>
public class Rotator : MonoBehaviour
{
    bool can_Rotate = false;
    float _rotateSpeed = 0;
    [HideInInspector] public Transform selectedGuard = null;

    private void Update()
    {
        if (can_Rotate)
        {
            transform.Rotate(0, 0, _rotateSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// set the can_Rotate , rotation_speed for Rotate; 
    /// </summary>
    /// <param name="can_rotate"></param>
    /// <param name="speed"></param>
    public void Rotate(float speed)
    {
        if (speed == 0)
        {
            if (this.can_Rotate)
                this.can_Rotate = false;
        }
        else
        {
            this.can_Rotate = true;
            this._rotateSpeed = speed;
        }

    }
    /// <summary>
    /// Hide with animation
    /// </summary>
    public void Hide()
    {
        GetComponent<Animator>().SetTrigger("hide");
    }

    // called in animation and Target.cs
    public void DeActive()
    {
        transform.localScale = Vector2.one;
        transform.eulerAngles = Vector3.zero;
        if (can_Rotate)
            can_Rotate = false;
        gameObject.SetActive(false);
    }
}
