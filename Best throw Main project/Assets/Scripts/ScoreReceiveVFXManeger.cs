using UnityEngine;

public class ScoreReceiveVFXManeger : MonoBehaviour
{
    public static ScoreReceiveVFXManeger Instance;
    [SerializeField] GameObject ScoreReceiveVFX;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this; 
        }
    }
   
    public void ShowNewScoreReceiveVFX(Transform target, Color ballColor, int score, int excellentNumber, int jumpNumber)
    {
       // ScoreReceiveVFX.SetActive(true);
        ScoreReceiveVFX.GetComponent<ScoreReceiveVFX>().ShowScoreReceiveVFX(target,ballColor,score,excellentNumber,jumpNumber);
    }
}
