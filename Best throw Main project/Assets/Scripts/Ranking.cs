using UnityEngine;
using SimpleJSON;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;

enum RequestType
{
    SetData = 1,
    GetSpecialData = 2,
    GetAllData = 3,
    ChangeName = 4,
    SetIsNameChanged = 5
}

public class Ranking : MonoBehaviour
{
    string DEVICE_ID = "";

    SaveLoadSystem saveLoadSystem;
    public static Ranking Instance;

    JSONObject _userData = null;
    JSONArray _UsersData = null;

    bool _isUserNameChanged = false;
    const string IS_CHANGED_NAME = "jdj738%dg75";

    [SerializeField]
    GameObject[] rankItems_0_10;

    [SerializeField] GameObject seperator, userAfter10Rank, userAfter10RankPrev, userAfter10RankNext;

    Text UserRowTxtInList = null;

    [SerializeField] Color allUsersColor, userColor;

    [SerializeField] Text errorTextInChangeMenu;

    [SerializeField] InputField InputName;

    [SerializeField] Button OkBtnInChangeNameMenu;
    bool isUserNameIsDuplicate = false;

    string playerName = null;
    Animator animator;


    [SerializeField]
    GameObject _loading;

    bool _isRankListShowed = false;

    bool requestGetDataUserIsDone = false;
    UnityWebRequest requestSetIsNameChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        animator = GetComponent<Animator>();
        DEVICE_ID = SystemInfo.deviceUniqueIdentifier;
        LoadPrefs();
    }

    void LoadPrefs()
    {
        saveLoadSystem = new SaveLoadSystem();

        _isUserNameChanged = saveLoadSystem.LoadBool(IS_CHANGED_NAME);
    }

    void SetIsUserNameChanged(bool isChanged)
    {
        _isUserNameChanged = isChanged;
        saveLoadSystem.SaveBool(IS_CHANGED_NAME, isChanged);
    }

    public void CheckPlayerData()
    {
        StartCoroutine(checkPlayerData());
    }


    IEnumerator checkPlayerData()
    {
        GetUserData();
        yield return null;

        while (!requestGetDataUserIsDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (_userData != null)
        {

            int rankScore = _userData[3];
            int curentBestScore = GameManeger.Instance.GetBestScore();
            if (rankScore > curentBestScore)
            {
                GameManeger.Instance.SetBestScore(rankScore);
            }
            else if (rankScore < curentBestScore)
            {
                if (rankScore == -1)  // error internet
                {
                    // Debug.Log("error internet conection");
                }
                else
                    SetPlayerRank(curentBestScore);
            }

            if (_isUserNameChanged)
            {
                if ((int)_userData[4] > 0)  // is changed name in data base
                {

                }
                else
                {
                    SetIsUserNameChangedReq(true);
                }
            }
            else
            {
                if ((int)_userData[4] > 0)
                {
                    SetIsUserNameChanged(true);
                }
                else
                {
                    SetIsUserNameChanged(false);
                }
            }
            playerName = _userData[2];
        }
    }


    #region Set user data

    public void SetPlayerRank(int BestScore)
    {
        if (!CheckInternet())
            return;

        StartCoroutine(SetRank(BestScore));
    }

    IEnumerator SetRank(int rank)
    {
        WWWForm www = new WWWForm();

        www.AddField("ReqType", (int)RequestType.SetData);
        www.AddField("UniqeId", SystemInfo.deviceUniqueIdentifier);
        www.AddField("Score", rank);
        www.AddField("VersionApp", Application.version);

        UnityWebRequest request = UnityWebRequest.Post("https://smoongame.com/bestThrowRank/", www);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            // Debug.Log(request.error);
        }
        else
        {
            //  Debug.Log(request.downloadHandler.text);
        }

        request.Dispose();

    }

    public void SetIsUserNameChangedReq(bool isChaneged)
    {
        if (!CheckInternet())
            return;

        StartCoroutine(setIsUserNameChangedCro(isChaneged));
    }

    IEnumerator setIsUserNameChangedCro(bool isChanged)
    {
        yield return null;

        WWWForm www = new WWWForm();

        www.AddField("ReqType", (int)RequestType.SetIsNameChanged);
        www.AddField("UniqeId", SystemInfo.deviceUniqueIdentifier);
        www.AddField("IsChangedName", Tools.changeBoolToInt(isChanged));


        requestSetIsNameChanged = UnityWebRequest.Post("https://smoongame.com/bestThrowRank/", www);
        yield return requestSetIsNameChanged.SendWebRequest();


        if (requestSetIsNameChanged.result != UnityWebRequest.Result.Success)
        {
            //Debug.Log(request.error);
        }
        else
        {
            //Debug.Log(request.downloadHandler.text);
        }


        requestSetIsNameChanged.Dispose();
    }

    public void ChangeName(string name)
    {
        if (!CheckInternet())
        {
            TextBox.Instance.ShowTextBox(TextBox.INTERNET_CONNECTION_ERROR);
            return;
        }


        isUserNameIsDuplicate = false;
        StartCoroutine(changePlayerNameCro(name));
    }


    IEnumerator changePlayerNameCro(string name)
    {
        WWWForm www = new WWWForm();

        www.AddField("ReqType", (int)RequestType.ChangeName);
        www.AddField("UniqeId", SystemInfo.deviceUniqueIdentifier);
        www.AddField("Name", name);


        UnityWebRequest request = UnityWebRequest.Post("https://smoongame.com/bestThrowRank/", www);
        yield return request.SendWebRequest();



        if (request.result != UnityWebRequest.Result.Success)
        {
            //  Debug.Log(request.error);
            TextBox.Instance.ShowTextBox(TextBox.DATA_RECEIVING_ERROR);

        }
        else
        {
            // Debug.Log(request.downloadHandler.text);

            if (request.downloadHandler.text == "Error")
            {
                setErrorMassageInChaneNameMenu("Error : This name exists. Please enter another name.");
                Invoke(nameof(updateNameRandom), 1.0f);
            }
            else
            {
                playerName = name;
                updateNameInRankList();
                HideChangeNameMenu();
            }
        }


        request.Dispose();
    }
    #endregion

    #region Get user data

    public JSONObject GetUserData()
    {
        if (!CheckInternet())
            return null;

        StartCoroutine(getUserData());

        return _userData;
    }

    IEnumerator getUserData()
    {
        _userData = null;
        requestGetDataUserIsDone = false;
        yield return null;
        WWWForm www = new WWWForm();

        www.AddField("ReqType", (int)RequestType.GetSpecialData);
        www.AddField("UniqeId", SystemInfo.deviceUniqueIdentifier);
        www.AddField("DeviceModel", SystemInfo.deviceModel);
        www.AddField("VersionApp", Application.version);

        UnityWebRequest requestGetDataUser = UnityWebRequest.Post("https://smoongame.com/bestThrowRank/", www);
        requestGetDataUser.SendWebRequest();

        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (requestGetDataUser.isDone)
            {
                break;
            }
        }


        if (requestGetDataUser.result != UnityWebRequest.Result.Success)
        {
            TextBox.Instance.ShowTextBox(TextBox.DATA_RECEIVING_ERROR);
            _userData = null;
            requestGetDataUserIsDone = true;
        }
        else
        {
            //Debug.Log(request.downloadHandler.text);
            JSONArray array = JSON.Parse(requestGetDataUser.downloadHandler.text) as JSONArray;
            _userData = array[array.Count - 1].AsObject;
            requestGetDataUserIsDone = true;
        }
        _loading.SetActive(false);

        requestGetDataUser.Dispose();

        yield return null;

    }

    #endregion

    #region Get All Users

    void GetAllRank()
    {
        StartCoroutine(getAllUserData());

    }

    IEnumerator getAllUserData()
    {
        CheckPlayerData();
        yield return null;


        _UsersData = null;

        WWWForm www = new WWWForm();

        www.AddField("ReqType", (int)RequestType.GetAllData);
        //  www.AddField("UniqeId", SystemInfo.deviceUniqueIdentifier);

        while (!requestGetDataUserIsDone)
        {
            yield return new WaitForEndOfFrame();
        }


        UnityWebRequest request = UnityWebRequest.Post("https://smoongame.com/bestThrowRank/", www);
        request.SendWebRequest();

        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (request.isDone)
                break;
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            //Debug.Log(request.error);

            _UsersData = null;
            TextBox.Instance.ShowTextBox(TextBox.DATA_RECEIVING_ERROR);
            _loading.SetActive(false);
        }
        else
        {
            // Debug.Log(request.downloadHandler.text);
            _UsersData = JSON.Parse(request.downloadHandler.text) as JSONArray;
            showRankItems();
            _loading.SetActive(false);
        }

        request.Dispose();

        yield return null;
    }

    #endregion
    public bool CheckInternet()
    {
        return (Application.internetReachability != NetworkReachability.NotReachable);
    }


    #region UI

    public void showRankItems()
    {

        if (!_isUserNameChanged)
            Invoke(nameof(ShowChangeNameMenu), 0.3f);

        if (_UsersData == null)
        {
            // failed get ranks
            return;
        }


        JSONObject userData_1 = null;
        JSONObject preUserData = null;
        JSONObject nextUserData = null;
        int userDataRank = 0;
        Image userItemBackground = null;


        for (int i = 0; i < _UsersData.Count; i++)
        {
            GameObject item = rankItems_0_10[i];
            item.SetActive(true);
            item.transform.GetChild(0).GetComponent<Text>().text = _UsersData[i].AsObject[2]; // name
            item.transform.GetChild(1).GetComponent<Text>().text = _UsersData[i].AsObject[3]; // score

            if (_UsersData[i].AsObject[1] == DEVICE_ID)
            {
                userData_1 = _UsersData[i].AsObject;
                item.transform.GetChild(3).gameObject.SetActive(true); // edit name btn
                UserRowTxtInList = item.transform.GetChild(0).GetComponent<Text>();
                userItemBackground = item.GetComponent<Image>();
                playerName = _UsersData[i].AsObject[2];
            }

            if (i == 9)
                break;
        }

        if (userData_1 == null)
        {
            userItemBackground = userAfter10Rank.GetComponent<Image>();
            for (int i = 0; i < _UsersData.Count; i++)
            {
                if (_UsersData[i].AsObject[1] == DEVICE_ID)
                {
                    userData_1 = _UsersData[i].AsObject;
                    userDataRank = i;
                    userAfter10Rank.SetActive(true);
                    userAfter10Rank.transform.GetChild(0).GetComponent<Text>().text = _UsersData[userDataRank].AsObject[2]; // name
                    userAfter10Rank.transform.GetChild(1).GetComponent<Text>().text = _UsersData[userDataRank].AsObject[3]; // score
                    userAfter10Rank.transform.GetChild(2).GetComponent<Text>().text = (userDataRank + 1).ToString(); // number
                    userAfter10Rank.transform.GetChild(3).gameObject.SetActive(true); // edit name btn
                    playerName = _UsersData[userDataRank].AsObject[2];
                    UserRowTxtInList = userAfter10Rank.transform.GetChild(0).GetComponent<Text>();
                    break;
                }
            }

            if (userDataRank > 10)
            {
                preUserData = _UsersData[userDataRank - 1].AsObject;
                userAfter10RankPrev.SetActive(true);
                userAfter10RankPrev.transform.GetChild(0).GetComponent<Text>().text = _UsersData[userDataRank - 1].AsObject[2]; // name
                userAfter10RankPrev.transform.GetChild(1).GetComponent<Text>().text = _UsersData[userDataRank - 1].AsObject[3]; // score
                userAfter10RankPrev.transform.GetChild(2).GetComponent<Text>().text = (userDataRank).ToString(); // number
            }


            if (_UsersData.Count > userDataRank + 1)
            {
                userAfter10RankNext.SetActive(true);
                nextUserData = _UsersData[userDataRank + 1].AsObject;
                userAfter10RankNext.transform.GetChild(0).GetComponent<Text>().text = _UsersData[userDataRank + 1].AsObject[2]; // name
                userAfter10RankNext.transform.GetChild(1).GetComponent<Text>().text = _UsersData[userDataRank + 1].AsObject[3]; // score
                userAfter10RankNext.transform.GetChild(2).GetComponent<Text>().text = (userDataRank + 2).ToString(); // number

            }
        }

        if (userDataRank > 11)
        {
            seperator.SetActive(true);
        }

        userItemBackground.color = userColor;


    }


    void updateNameInRankList()
    {
        UserRowTxtInList.text = playerName;
    }

    public void ShowRankList()
    {
        if (!CheckInternet())
            return;

        _isRankListShowed = true;
        _loading.SetActive(true);


        GetAllRank();

        animator.SetTrigger("Show");
    }

    public void hideRankList()
    {
        _isRankListShowed = false;
        animator.SetTrigger("Hide");
        Invoke(nameof(resetItems), 0.8f);
        _loading.SetActive(false);
    }

    void resetItems()
    {
        if (_isRankListShowed)
            return;

        foreach (var item in rankItems_0_10)
        {
            item.GetComponent<Image>().color = allUsersColor;
            item.transform.gameObject.SetActive(false);
            item.transform.GetChild(3).gameObject.SetActive(false);
        }

        seperator.SetActive(false);
        userAfter10Rank.SetActive(false);
        userAfter10Rank.transform.GetChild(3).gameObject.SetActive(false);
        userAfter10Rank.GetComponent<Image>().color = allUsersColor;

        userAfter10RankPrev.SetActive(false);
        userAfter10RankNext.SetActive(false);

    }


    public void ShowChangeNameMenu()
    {
        if (!_isRankListShowed)
            return;

        if (playerName != null)
            InputName.text = playerName;

        animator.SetTrigger("Edit menu show");
    }


    public void HideChangeNameMenu()
    {
        animator.SetTrigger("Edit menu hide");
    }

    void setErrorMassageInChaneNameMenu(string m)
    {
        errorTextInChangeMenu.text = m;
    }

    void updateNameRandom()
    {
        InputName.SetTextWithoutNotify(InputName.text + Random.Range(1000, 9999));
        setErrorMassageInChaneNameMenu("");
    }


    public void OkBtnInChaneNameMenuClick()
    {
        if (InputName.text == playerName)
        {
            HideChangeNameMenu();
            SetIsUserNameChangedReq(true);
            SetIsUserNameChanged(true);
        }
        else
        {
            ChangeName(InputName.text);
        }

    }

    public void OnInputNameChanged(string txt)
    {
        OkBtnInChangeNameMenu.interactable = true;

        if (string.IsNullOrEmpty(txt.Trim()))
        {
            OkBtnInChangeNameMenu.interactable = false;
            setErrorMassageInChaneNameMenu("Error : Cannot be empty !");
            return;
        }
        else
        {

        }

        if (Regex.IsMatch(txt, "^[a-zA-Z0-9_ ]*$"))
        {

        }
        else
        {
            OkBtnInChangeNameMenu.interactable = false;
            setErrorMassageInChaneNameMenu("Error : Please type in English");
            return;
        }

        if (txt.Length > 4)
        {


        }
        else
        {
            OkBtnInChangeNameMenu.interactable = false;
            setErrorMassageInChaneNameMenu("Error : The number of characters can not be less than 4");
            return;
        }

        setErrorMassageInChaneNameMenu("");
    }

    public void ChangeMiniButtonClick()
    {
        ShowChangeNameMenu();
    }
    #endregion
}

