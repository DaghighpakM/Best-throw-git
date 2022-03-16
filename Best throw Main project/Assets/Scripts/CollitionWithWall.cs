using UnityEngine;

public class CollitionWithWall : MonoBehaviour
{
    public Vector2 position { set; get; }
    private void Start()
    {
        if (position.x < 0)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else if (position.x > 0)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
        transform.position = position;
    }

    // Called in animation event
    void Destroy()
    {
        Destroy(gameObject);
    }

}

