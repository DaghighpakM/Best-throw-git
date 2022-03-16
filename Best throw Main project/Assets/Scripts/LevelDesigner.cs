using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelDesigner : MonoBehaviour
{
    public static LevelDesigner Instance;

    public const string TARGET_TAG = "Target", STATION_TAG = "Station", SHOOTER_TAG = "Shooter", GUARD_TAG = "Guard",
        SELECTED_GUARD_TAG = "Selected Guard", LOSS_LINE_TAG = "LossLine", PAUSEDER_BALL_TAG = "Ball Pauseder";

    [Header("Color for game objects(ball and guards)")]
    [SerializeField] Color[] _gameObjectsColor;

    [Header("Build a number of steps to control and build levels")]
    [SerializeField] Step[] _steps;

    [Header("---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------")]
    [SerializeField] Ball _ballCs;
    [SerializeField] Transform _environment, _wall_R, _wall_L, _ballPauseder;


    public GameObject targetPrefab;
    public GameObject targetParent;

    [HideInInspector] public GameObject station, target;
    GameObject _nextTarget, _container; //container is A container for moving.

    [HideInInspector] public Vector2 firstPositionOfStation, firstPositionOfTarget;

    [HideInInspector] public Vector2 screenToWorldSize;

    [SerializeField] Trajectory _trajectoryCs;

    int _numberLevel = 0;

    Camera _mainCamera;
    Step _stepOfPattern;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        _mainCamera = Camera.main;

        screenToWorldSize.y = _mainCamera.orthographicSize;
        screenToWorldSize.x = screenToWorldSize.y * _mainCamera.aspect;
        _trajectoryCs._screenToWorldSizeX = screenToWorldSize.x;

        firstPositionOfStation = new Vector2(-screenToWorldSize.x / 2, -screenToWorldSize.y / 2 + 2f);
        firstPositionOfTarget = new Vector2(screenToWorldSize.x / 2, screenToWorldSize.y / 2 - 2f);

#if UNITY_EDITOR
        checkDataOfSteps();
#endif
        setPatternStep(0);
        preparationEnvironment();
        createTargets();
        GoToStartLevel();
    }

    /// <summary>
    /// Build station and target and next target from prefab target
    /// </summary>
    void createTargets()
    {
        station = Instantiate(targetPrefab, targetParent.transform);
        target = Instantiate(targetPrefab, targetParent.transform);
        _nextTarget = Instantiate(targetPrefab, targetParent.transform);

    }

    /// <summary>
    /// Build the first level and prepare to start the game
    /// </summary>
    public void GoToStartLevel()
    {

        station.transform.position = firstPositionOfStation;
        station.transform.localScale = Vector2.zero;
        station.tag = STATION_TAG;

        target.transform.position = firstPositionOfTarget;
        target.transform.localScale = Vector2.zero;

        target.tag = TARGET_TAG;
        Target target_cs = target.GetComponent<Target>();

        target_cs.SetGuards(GuardType._2Guard);
        setGuardsColor(target_cs);

        target_cs.activeRotator.transform.rotation = Quaternion.Euler(0, 0, 30f);

        _ballCs.SetColor(setSelectedGuard(target_cs.activeRotator.transform));
      
        _nextTarget.transform.localScale = Vector2.zero;

        _environment.position = new Vector2(0, station.transform.position.y);
    }

    /// <summary>
    /// Return to the first level settings to restart the game
    /// </summary>
    public void ResetToStartLevel()
    {
        this._numberLevel = 0;
        station.transform.position = firstPositionOfStation;
        station.transform.localScale = Vector2.zero;
        station.tag = STATION_TAG;

        target.transform.position = firstPositionOfTarget;
        target.transform.localScale = Vector2.zero;

        target.tag = TARGET_TAG;
        Target target_cs = target.GetComponent<Target>();
        target_cs.ResetToFirstState();

        target_cs.SetGuards(GuardType._2Guard);
        setGuardsColor(target_cs);

        target_cs.activeRotator.transform.rotation = Quaternion.Euler(0, 0, 30f);

        _ballCs.SetColor(setSelectedGuard(target_cs.activeRotator.transform));
        _ballCs.GetComponent<Ball>().ResetOptions(firstPositionOfStation);
        _nextTarget.transform.localScale = Vector2.zero;

        _environment.position = new Vector2(0, station.transform.position.y);
    }



    /// <summary>
    /// Build the next level
    /// </summary>
    /// <param name="numberLevel"></param>
    public void GoToNextLevel(int numberLevel)
    {

        this._numberLevel = numberLevel;

        setPatternStep(numberLevel);

        station.GetComponent<Target>().Hide();

        _container = station;
        station = target;
        target = _nextTarget;
        _nextTarget = _container;

        station.tag = STATION_TAG;
        target.tag = TARGET_TAG;

        setTarget();

        _environment.position = new Vector2(0, station.transform.position.y);
    }

    /// <summary>
    /// Checks the step information, such as startFrom,... to make sure it is correct.
    /// </summary>
    void checkDataOfSteps()
    {
        if (_steps.Length == 0)
        {
            Debug.LogError("steps is null! >> Make at least one step: in the inspector window >> Steps array");
            emergencyExit();
            return;
        }

        if (_steps[0].startFrom != 1)
        {
            if (_steps[0].name.Trim() == "")
            {
                Debug.LogError(@" value of ""start_from"" should not be other than 1 in ""steps[0]""");
            }
            else
            {
                Debug.LogError(@" value of ""start_from"" should not be other than 1 in """ + _steps[0].name + @""" step!");
            }
            emergencyExit();
        }

        for (int i = 0; i < _steps.Length; i++)
        {

            if (_steps[i].name.Trim() == "")
                _steps[i].name = "step " + i;

            if (!_steps[i].has2Guard && !_steps[i].has3Guard && !_steps[i].has4Guard)
            {
                Debug.LogError("In Step: " + _steps[i].name + ", at least one type of guard must be selected");
                emergencyExit();
            }

            if (i != 0)
            {
                if (_steps[i].startFrom <= _steps[i - 1].startFrom)
                {
                    Debug.LogError(" start_From at " + _steps[i].name + " step <= start_From at " + _steps[i - 1].name + " step!");
                    emergencyExit();
                }
            }

            // check Guard & Rotate Speeds
            if (_steps[i].rotateSpeeds.Length == 0)
            {
                Debug.LogError(@" At least one "" speed "" for rotation must be created in the rotateSpeeds array! ");
                emergencyExit();
            }
            else
            {
                for (int q = 0; q < _steps[i].rotateSpeeds.Length; q++)
                {
                    if (_steps[i].rotateSpeeds[q] == 0)
                    {
                        Debug.LogError("rotateSpeeds[" + q + "] should not be 0");
                        emergencyExit();
                    }
                }
            }

            // Check Barrier Data
            if (_steps[i].probabilityOfBarrierExistence != Probability.never) //If there is a possibility of building a barrier
            {
                if (_steps[i].minHeightBarrier > _steps[i].maxHeightBarrier)
                {
                    Debug.LogError(@" Error :: size of ""minHeightBarrier"" should not be bigger ""maxHeightBarrier"" in : " + _steps[i].name + " step! ");
                    emergencyExit();
                }

                if (_steps[i].barrierSizes.Length == 0)
                {
                    Debug.LogError(@" Error ::  Length of ""barrierSizes"" array is 0 in : " + _steps[i].name + " step! , at least one size must be seted");
                    emergencyExit();
                }
                else
                {
                    for (int t = 0; t < _steps[i].barrierSizes.Length; t++)
                    {
                        if (_steps[i].barrierSizes[t] == 0)
                        {
                            Debug.LogError(" Error :: size of barrierSizes[" + t + "] is 0 in : " + _steps[i].name + " step! ");
                            emergencyExit();
                        }
                    }
                }


                if (_steps[i].probabilityOfCanScaleBarrier != Probability.never)
                {
                    if (_steps[i].scalingValue == 0)
                    {
                        Debug.LogError(@" Error :: size of ""scaling_value"" should not be 0 in : " + _steps[i].name + "  step!");
                        emergencyExit();
                    }
                }

            }
            else
            {   // If the probability of building a barrier is zero
                if (_steps[i].barrierSizes.Length != 0 ||
                    _steps[i].probabilityOfCanScaleBarrier != Probability.never ||
                    _steps[i].ProbabilityOfCanMoveBarrier != Probability.never)
                {
                    Debug.LogWarning("If you want to exist Barrier in " + _steps[i].name + @" step!, you have to set the ""Probability_of_barrier_existence"" status other than ""never""." +
                        "\n\n\nOtherwise, if you want to fix this warning, you have to set the value as follows in " + _steps[i].name + " step : " +
                        "\n" + @"  set size of ""barrierSizes"" to 0 " +
                        "\n" + @"  set ""probabilityOfCanScaleBarrier"" to never " +
                        "\n" + @"  set ""ProbabilityOfCanMoveBarrier"" to never " +
                        "\n\n\n");
                }
            }

            // Check Target Move Data
            if (_steps[i].ProbabilityValueOfIsMoveTarget != Probability.never)
            {
                if (_steps[i].maxRangeOfSpaceMove == Vector2.zero)
                {
                    Debug.LogError("maxRangeOfSpaceMove value should not be (0,0) in " + _steps[i].name + " step!");
                    emergencyExit();
                }
                else if (_steps[i].minRangeOfSpaceMove.x < 0 || _steps[i].minRangeOfSpaceMove.y < 0 || _steps[i].maxRangeOfSpaceMove.x < 0 || _steps[i].maxRangeOfSpaceMove.y < 0 || _steps[i].minRangeOfSpaceMove.x > 1 || _steps[i].minRangeOfSpaceMove.y > 1 || _steps[i].maxRangeOfSpaceMove.x > 1 || _steps[i].maxRangeOfSpaceMove.y > 1)
                {
                    Debug.LogError(" The values ​​of minRangeOfSpaceMove , maxRangeOfSpaceMove must be between 0 and 1 in  " + _steps[i].name + " step!");
                    emergencyExit();
                }
                else if ((_steps[i].minRangeOfSpaceMove.x > _steps[i].maxRangeOfSpaceMove.x) || (_steps[i].minRangeOfSpaceMove.y > _steps[i].maxRangeOfSpaceMove.y))
                {
                    Debug.LogError("minRangeOfSpaceMove should not be bigger than maxRangeOfSpaceMove in " + _steps[i].name + " step!");
                    emergencyExit();
                }

                if (_steps[i].smoothTimesForMove.Length == 0)
                {
                    Debug.LogError(@" Error ::  Length of ""smoothTimesForMove"" array is 0 in : " + _steps[i].name + " step! , at least one element must be seted");
                    emergencyExit();
                }
                else
                {
                    for (int g = 0; g < _steps[i].smoothTimesForMove.Length; g++)
                    {
                        if (_steps[i].smoothTimesForMove[g] == 0)
                        {
                            Debug.LogError("smoothTimesForMove[" + g + "] should not be 0");
                            emergencyExit();
                        }
                    }
                }
            }
            else
            {
                if (_steps[i].maxRangeOfSpaceMove != Vector2.zero)
                {
                    Debug.LogWarning("If you want to move in " + _steps[i].name + @" step! , you have to set the ""probability"" status other than ""never"".   If you do not want it to exist, you will need to change the ""maxRangeOfSpaceMove"" value to zero to clear this Warning.");
                }
            }

            // Check Position Data
            if (_steps[i].maxHeightOfPositionTarget < _steps[i].minHeightOfPositionTarget)
            {
                Debug.LogError(@" ""minHeightOfPositionTarget"" should not be bigger ""maxHeightOfPositionTarget"" " + _steps[i].name + " step!");
                emergencyExit();
            }
        }
    }

    /// <summary>
    /// Exit the game due to incomplete input information in >> Level Designer GameObject >> Inspector window >> Array steps
    /// </summary>
    void emergencyExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
        Application.Quit();
    }

    /// <summary>
    /// Select a step as _stepOfPattern
    /// </summary>
    /// <param name="numberLevel"></param>
    void setPatternStep(int numberLvl)
    {
        for (int i = _steps.Length - 1; i >= 0; i--)
        {
            if (numberLvl >= _steps[i].startFrom)
            {
                _stepOfPattern = _steps[i];
                return;
            }
        }
    }

    /// <summary>
    /// preparation Wall,...
    /// </summary>
    void preparationEnvironment()
    {
        _wall_R.position = new Vector2(screenToWorldSize.x, firstPositionOfStation.y);
        _wall_L.position = new Vector2(-screenToWorldSize.x, firstPositionOfStation.y);
        _ballPauseder.position = new Vector2(0, firstPositionOfStation.y
            - screenToWorldSize.y
            );
        _ballPauseder.localScale = new Vector3(screenToWorldSize.x * 2, 1, 1);

    }

    /// <summary>
    /// Set the target characteristics such as the number of guards and the color of the guards and ...
    /// </summary>
    void setTarget()
    {
        Target targetCs = target.GetComponent<Target>();

        #region set target type
        GuardType guard_Type = GuardType._2Guard;

        if (_stepOfPattern.has2Guard && _stepOfPattern.has3Guard && _stepOfPattern.has4Guard)
        {
            guard_Type = (GuardType)Random.Range(2, 5);
        }
        else if (_stepOfPattern.has2Guard && _stepOfPattern.has3Guard)
        {
            guard_Type = (GuardType)Random.Range(2, 4);
        }
        else if (_stepOfPattern.has2Guard && _stepOfPattern.has4Guard)
        {
            int r = Random.Range(0, 2);
            if (r == 0)
            {
                guard_Type = GuardType._2Guard;
            }
            else if (r == 1)
            {
                guard_Type = GuardType._4Guard;
            }
        }
        else if (_stepOfPattern.has3Guard && _stepOfPattern.has4Guard)
        {
            guard_Type = (GuardType)Random.Range(3, 5);
        }
        else if (_stepOfPattern.has2Guard)
        {
            guard_Type = GuardType._2Guard;
        }
        else if (_stepOfPattern.has3Guard)
        {
            guard_Type = GuardType._3Guard;
        }
        else if (_stepOfPattern.has4Guard)
        {
            guard_Type = GuardType._4Guard;
        }

        targetCs.SetGuards(guard_Type);
        #endregion

        setGuardsColor(targetCs);

        _ballCs.SetNextColor(setSelectedGuard(targetCs.activeRotator.transform));

        setBarrier();

        setMoveTarget();

        targetCs.SetPosition(station.transform,
            _stepOfPattern.minHeightOfPositionTarget,
            _stepOfPattern.maxHeightOfPositionTarget
            );

        #region rotate target

        int random = Random.Range(0, _stepOfPattern.rotateSpeeds.Length);
        targetCs.activeRotator.GetComponent<Rotator>().Rotate(_stepOfPattern.rotateSpeeds[random]);

        #endregion
        targetCs.Show();
    }

    /// <summary>
    /// Set the colors of the guards
    /// </summary>
    /// <param name="targetCs"></param>
    void setGuardsColor(Target targetCs)
    {
        int rotatorChildCount = targetCs.activeRotator.transform.childCount;
        int[] random_numbers = RandomUtils.RandomUnRepeat(_gameObjectsColor.Length, rotatorChildCount);
        for (int i = 0; i < rotatorChildCount; i++)
        {
            targetCs.activeRotator.transform.GetChild(i).GetComponent<SpriteRenderer>().color = _gameObjectsColor[random_numbers[i]];
        }
    }

    /// <summary>
    /// Consider one of the guards as the selected guard so that the _ballCs can pass through it.
    /// We choose the color of the _ballCs in the same color as the selected guard.
    /// To be able to cross it and enter the target.
    /// </summary>
    /// <param name="activeRotatorTransform"></param>
    /// <returns></returns>
    Color setSelectedGuard(Transform activeRotatorTransform)
    {
        Transform selectedGuardTransform = null;
        int numberOfChild = 1;
        if (_numberLevel > 0)
            numberOfChild = UnityEngine.Random.Range(0, activeRotatorTransform.childCount);

        selectedGuardTransform = activeRotatorTransform.GetChild(numberOfChild);
        selectedGuardTransform.tag = SELECTED_GUARD_TAG;
        selectedGuardTransform.GetComponent<EdgeCollider2D>().isTrigger = true;
        activeRotatorTransform.GetComponent<Rotator>().selectedGuard = selectedGuardTransform;

        return selectedGuardTransform.GetComponent<SpriteRenderer>().color;
    }

    /// <summary>
    /// Set the target moving settings
    /// </summary>
    void setMoveTarget()
    {
        if (RandomUtils.RandomProbabilityTrue(_stepOfPattern.ProbabilityValueOfIsMoveTarget))
        {
            float speed = _stepOfPattern.smoothTimesForMove[Random.Range(0, _stepOfPattern.smoothTimesForMove.Length)];
            target.GetComponent<Target>().SetMoveTarget(station.transform, _stepOfPattern.minRangeOfSpaceMove, _stepOfPattern.maxRangeOfSpaceMove, speed);
        }
    }

    /// <summary>
    /// Set barrier features such as barrier size and barrier movement capability and ...
    /// </summary>
    void setBarrier()
    {
        if (RandomUtils.RandomProbabilityTrue(_stepOfPattern.probabilityOfBarrierExistence))
        {
            float barrierSize = 1;
            barrierSize = _stepOfPattern.barrierSizes[Random.Range(0, _stepOfPattern.barrierSizes.Length)];

            bool canMove = false;
            if (RandomUtils.RandomProbabilityTrue(_stepOfPattern.ProbabilityOfCanMoveBarrier))
                canMove = true;

            float valueOfScale = 0f;
            if (RandomUtils.RandomProbabilityTrue(_stepOfPattern.probabilityOfCanScaleBarrier))
                valueOfScale = _stepOfPattern.scalingValue;

            target.GetComponent<Target>().SetBarrier(station.transform, barrierSize, _stepOfPattern.minHeightBarrier, _stepOfPattern.maxHeightBarrier, canMove, valueOfScale);
        }
    }

    public void ShowTargetAndStation() // with lerp
    {
        station.GetComponent<Target>().Show();
        target.GetComponent<Target>().Show();
    }

    public void HideTargetAndStation() // with lerp
    {
        station.GetComponent<Target>().Hide();
        target.GetComponent<Target>().Hide();
    }
}

public enum GuardType
{
    _2Guard = 2,
    _3Guard = 3,
    _4Guard = 4
}

/// <summary>
/// Each step controls a number of steps
/// </summary>
[Serializable]
public class Step
{
    [Header("The name of this step")]
    public string name;

    [Header("What level should this step start from?  << Attention: Exceptionally, this value should be 1 for the first step! >>")]
    public int startFrom;

    [Header("Select guard type")]
    public bool has2Guard;
    public bool has3Guard;
    public bool has4Guard;

    [Header("Rotator speed (Note: Speed should not be 0)")]
    [Range(-200f, 200f)]
    public int[] rotateSpeeds;

    [Header("Barrier")]
    public Probability probabilityOfBarrierExistence = Probability.never;

    [Space(5)]
    [Range(0, 0.5f)]
    public float minHeightBarrier = 0;
    [Range(0, 0.5f)]
    public float maxHeightBarrier = 0.5f;

    [Space(5)]
    [Range(0.50f, 1f)]
    public float[] barrierSizes;

    [Space(5)]
    public Probability probabilityOfCanScaleBarrier = Probability.never;

    [Space(5)]
    [Range(0.01f, 0.5f)]
    public float scalingValue;
    [Space(5)]
    public Probability ProbabilityOfCanMoveBarrier = Probability.never;

    [Header("Move Target")]
    [Space(15)]
    public Probability ProbabilityValueOfIsMoveTarget = Probability.never;
    public Vector2 minRangeOfSpaceMove, maxRangeOfSpaceMove;
    [Range(0.01f, 2f)] public float[] smoothTimesForMove;

    [Header("Position")]
    [Space(15)]
    [Range(0, 1)] public float minHeightOfPositionTarget = 0;
    [Range(0, 1)] public float maxHeightOfPositionTarget = 0;

}

