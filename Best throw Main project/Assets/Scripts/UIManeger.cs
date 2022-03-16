using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class UIManeger : MonoBehaviour
{
    [Header("Link of Instagram Page")]
    [SerializeField] string _instagramPageLink;

    [Header("Timer of progressBar for watch Reward Ads.")]
    [SerializeField] float _totalTimeForTimer;
    float _variableTime;
    [SerializeField] Image _progressFill;
    [SerializeField] Text _timerTxt;
    bool _canStartTimerOfContinueWithAds = false;

    [Space]
    [SerializeField] RectTransform _uiCanvasRectTransform;

    [Space]
    [SerializeField] ParticleSystem _newRecordVfxPs;

    [Space]
    [SerializeField] Text _scoreTxt;

    [Space]
    [SerializeField] Text _lossMenuScoreTxt;
    [SerializeField] Text _lossMenuBestScoreTxt;

    [Space(15)]
    [SerializeField] GameObject _HelpEfect;

    [SerializeField] Button _pauseBtn;

    [Header("Setting btns images Component")]
    [SerializeField] Image _soundBtnImg;
    [SerializeField] Image _vibrationBtnImg;
    [SerializeField] Image _musicBtnImg;

    [Header("Setting btns Shodow images Component")]
    [SerializeField] Image _soundBtnImgShodow;
    [SerializeField] Image _vibrationBtnImgShodow;
    [SerializeField] Image _musicBtnImgShodow;

    [Header("Setting buttons Images Status On")]
    [SerializeField] Sprite _soundBtnOnStatusSp;
    [SerializeField] Sprite _musicBtnOnStatusSp;
    [SerializeField] Sprite _vibrationBtnOnStatusSp;

    [Header("Setting buttons Images Status Off")]
    [SerializeField] Sprite _soundBtnOffStatusSp;
    [SerializeField] Sprite _musicBtnOffStatusSp;
    [SerializeField] Sprite _vibrationBtnOffStatusSp;

    Animator _animator;

    public bool isStartMenuShowed { set; get; } = false;
    bool _isSettingMenuShowed = false;
    bool _isPauseMenuShowed = false;
    bool _isLossMenuShowed = false;
    bool _isExitDialogShowed = false;
    bool _isRankListShowed = false;

    public static UIManeger instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        _animator = GetComponent<Animator>();
        ParticleSystem.ShapeModule shapeModule = _newRecordVfxPs.shape;
        shapeModule.scale = new Vector3(_uiCanvasRectTransform.GetComponent<RectTransform>().sizeDelta.x / 100f, 1, 1);
        _HelpEfect.SetActive(false);


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EscapeButtonClick();
        }

        // for timer of Continue with watch Reward Ads.
        if (_canStartTimerOfContinueWithAds)
        {
            _variableTime = _variableTime - Time.deltaTime;
            _timerTxt.text = _variableTime.ToString("0");
            _progressFill.fillAmount = _variableTime / _totalTimeForTimer;
            if (_variableTime <= 0)
            {
                _canStartTimerOfContinueWithAds = false;
                timerOfContinueIsEnded();
            }
        }
    }

    #region called from other scripts (Main Metods)

    #region Start Menu 
    /// <summary>
    /// Showed start Menu with start animation
    /// </summary>
    public void ShowFirstStartMenu()
    {
        isStartMenuShowed = true;
        showGameName(true, true);

    }
    public void ShowStartMenu()
    {
        isStartMenuShowed = true;
        showGameName(true);
        showDownBtns(true);
    }
    public void HideStartMenu()
    {
        isStartMenuShowed = false;
        HideHelpEfect();
        showGameName(false);
        showDownBtns(false);
        showHeader(true);
        hideSettingMenu();

    }
    #endregion

    #region Pause Menu
    public void ShowPauseMenu()
    {
        //tapsell
        if (TapsellManager.Instance.isNativeLoaded)
        {
            _animator.SetTrigger("Pause menu with Ads (show)");
        }
        else
        {
            _animator.SetTrigger("Pause menu (show)");
        }
        showDownBtns(true);
        _isPauseMenuShowed = true;
    }
    public void HidePauseMenu()
    {
        _animator.SetTrigger("Pause menu (hide)");

        showDownBtns(false);
        _isPauseMenuShowed = false;
        hideSettingMenu();
    }
    #endregion

    #region Loss Menu
    public void ShowLossMenuWithContinue(int score, int bestScore)
    {
        showHeader(false);
        _lossMenuScoreTxt.text = score.ToString();
        _lossMenuBestScoreTxt.text = bestScore.ToString();
        _animator.SetTrigger("Loss menu with continue (show)");

        startTimerForContinueWithAds();
        _isLossMenuShowed = true;
    }
    public void ShowLossMenu(int score, int bestScore)
    {
        showHeader(false);
        _lossMenuScoreTxt.text = score.ToString();
        _lossMenuBestScoreTxt.text = bestScore.ToString();
        _animator.SetTrigger("Loss menu (show)");
        _isLossMenuShowed = true;
    }
    public void HideLossMenu()
    {
        _animator.SetTrigger("Loss meno (hide)");
        _isLossMenuShowed = false;

        if (_canStartTimerOfContinueWithAds)
            _canStartTimerOfContinueWithAds = false;
    }

    public void ShowLossMenuAfterNotCompletWatchRewardVideo()
    {
        _animator.SetTrigger("Translate from loss menu(with Continue btn) to Loss menu(without Continue btn)");
    }



    #region timer for Continue with watch reward ads

    private void startTimerForContinueWithAds()
    {
        _variableTime = _totalTimeForTimer;
        _canStartTimerOfContinueWithAds = true;
    }

    private void timerOfContinueIsEnded()
    {
        _animator.SetTrigger("Translate from loss menu(with Continue btn) to Loss menu(without Continue btn)");

    }
    #endregion

    #endregion

    public void PlayNewRecordVFX()
    {
        AudioAndVibrationManeger.instance.play("New record VFX");
        _animator.SetTrigger("New Record board");
        _newRecordVfxPs.Play();
    }

    public void SetScoreText(int score)
    {
        _scoreTxt.text = score.ToString();
        StartCoroutine(t());

    }

    IEnumerator t()  // resized after epsilon time score panel 
    {
        yield return new WaitForSeconds(0.001f);
        _scoreTxt.supportRichText = true;
        _scoreTxt.supportRichText = false;
    }

    public void changeSoundBtnStatus(bool mute)
    {
        Image img = _soundBtnImg;
        if (img.sprite == _soundBtnOnStatusSp && !mute) return;
        if (img.sprite == _soundBtnOffStatusSp && mute) return;
        Image img2 = _soundBtnImgShodow;
        if (mute)
        {
            img.sprite = _soundBtnOffStatusSp;
            img2.sprite = _soundBtnOffStatusSp;
        }
        else
        {
            img.sprite = _soundBtnOnStatusSp;
            img2.sprite = _soundBtnOnStatusSp;
        }
    }
    public void changeMusicBtnStatus(bool mute)
    {
        Image img = _musicBtnImg;
        if (img.sprite == _musicBtnOnStatusSp && !mute) return;
        if (img.sprite == _musicBtnOffStatusSp && mute) return;
        Image img2 = _musicBtnImgShodow;

        if (mute)
        {
            img.sprite = _musicBtnOffStatusSp;
            img2.sprite = _musicBtnOffStatusSp;
        }
        else
        {
            img.sprite = _musicBtnOnStatusSp;
            img2.sprite = _musicBtnOnStatusSp;
        }
    }
    public void changeVibrationBtnStatus(bool mute)
    {
        Image img = _vibrationBtnImg;
        if (img.sprite == _vibrationBtnOnStatusSp && !mute) return;
        if (img.sprite == _vibrationBtnOffStatusSp && mute) return;
        Image img2 = _vibrationBtnImgShodow;
        if (mute)
        {
            img.sprite = _vibrationBtnOffStatusSp;
            img2.sprite = _vibrationBtnOffStatusSp;
        }
        else
        {
            img.sprite = _vibrationBtnOnStatusSp;
            img2.sprite = _vibrationBtnOnStatusSp;
        }
    }
    #endregion

    #region  metods for controll animations
    public void showHeader(bool show)    //Header is pause btn + score panel
    {
        _pauseBtn.interactable = show;

        if (show)
            _animator.SetTrigger("Header (show)");
        else
            _animator.SetTrigger("Header (hide)");
    }

    void showDownBtns(bool show)
    {
        if (show)
            _animator.SetTrigger("Down btns (show)");
        else
            _animator.SetTrigger("Down btns (hide)");
    }

    void showGameName(bool show, bool isStartGame = false)
    {
        if (show && isStartGame)
        {
            _animator.SetTrigger("Game name start (show)");
        }
        else if (show && !isStartGame)
        {
            _animator.SetTrigger("Game name (show)");
        }
        else if (!show && !isStartGame)
        {
            _animator.SetTrigger("Game name (hide)");
        }
    }

    void showSettingMenu()
    {
        if (!_isSettingMenuShowed)
        {
            _isSettingMenuShowed = true;
            _animator.SetTrigger("Setting menu (show)");
        }
    }

    void hideSettingMenu()
    {
        if (_isSettingMenuShowed)
        {
            _isSettingMenuShowed = false;
            _animator.SetTrigger("Setting menu (hide)");
        }
    }

    // called in anim
    public void startGameAnimationEnded()
    {
        showDownBtns(true);
        GameManeger.Instance.StartOnDeley();
    }

    // called in anim
    void playGameNameSound()
    {
        AudioAndVibrationManeger.instance.play("Showing game name");
    }

    // called in anim
    void ShowHelpEfect()
    {
        _HelpEfect.SetActive(true);
        _HelpEfect.transform.position = new Vector2(0, -0.8f);
    }

    void HideHelpEfect()
    {
        _HelpEfect.SetActive(false);
    }

    void showExitMenu()
    {
        _isExitDialogShowed = true;
        _animator.SetTrigger("Exit menu (show)");
    }

    void hideExitMenu()
    {
        _isExitDialogShowed = false;
        _animator.SetTrigger("Exit menu (hide)");
    }

    void showAboutMenu()
    {
        _animator.SetTrigger("About meno (show)");
    }

    void hideAboutmenu()
    {
        _animator.SetTrigger("About meno (hide)");
    }


    #endregion

    #region Button's Cklick Event's
    public void PauseBtnClick()
    {
        _isPauseMenuShowed = true;

        GameManeger.Instance.Pause();
        AudioAndVibrationManeger.instance.play("Buttons click");
    }
    public void RestartBtnClick()
    {
        AudioAndVibrationManeger.instance.play("Buttons click");

        GameManeger.Instance.Restart();
        if (_isLossMenuShowed)
        {
            HideLossMenu();

        }
        if (_isPauseMenuShowed)
        {
            HidePauseMenu();
            showHeader(false);
        }
        ShowStartMenu();

    }
    public void NothanksBtnClick()
    {
        AudioAndVibrationManeger.instance.play("Buttons click");

        if (Random.Range(0, 2) == 1)
        {
            if (TapsellManager.Instance.isIntertitialLoaded)
            {
                TapsellManager.Instance.ShowInterstitialAd();
            }
        }
        GameManeger.Instance.Restart();
        HideLossMenu();
        ShowStartMenu();
    }

    public void ContinueBtnClick()  // in loss menu when can watch reward ads
    {
        AudioAndVibrationManeger.instance.play("Buttons click");


        // tapsell
        if (TapsellManager.Instance.isRewardLoaded)
        {
            TapsellManager.Instance.ShowReward();
        }
        else
        {
            ShowLossMenuAfterNotCompletWatchRewardVideo();

        }

        _canStartTimerOfContinueWithAds = false;
    }

    public void ResumeBtnClick()  // in pause menu
    {
        AudioAndVibrationManeger.instance.play("Buttons click");

        GameManeger.Instance.Resume();
        HidePauseMenu();
    }

    public void AboutBtnClick()
    {
        AudioAndVibrationManeger.instance.play("Buttons click");
        showAboutMenu();
    }
    public void RatingBtnClick()
    {
        AudioAndVibrationManeger.instance.play("Buttons click");

        MyketIntetnt myketIntetnt = new MyketIntetnt();
        myketIntetnt.Like(Application.identifier);

    }
    public void InstagramBtnClick()
    {
        AudioAndVibrationManeger.instance.play("Buttons click");
        Application.OpenURL(_instagramPageLink);

    }
    public void SettingBtnClick()
    {
        AudioAndVibrationManeger.instance.play("Buttons click");
        if (_isSettingMenuShowed)
            hideSettingMenu();
        else
            showSettingMenu();
    }

    public void SoundBtnClick()
    {
        AudioAndVibrationManeger.instance.play("Buttons click");
        bool isMute = AudioAndVibrationManeger.instance.GetSoundMute();
        AudioAndVibrationManeger.instance.SetSoundMute(!isMute);
        changeSoundBtnStatus(!isMute);

    }
    public void MusicBtnClick()
    {
        AudioAndVibrationManeger.instance.play("Buttons click");
        bool isMute = AudioAndVibrationManeger.instance.GetMusicMute();

        AudioAndVibrationManeger.instance.SetMusicMute(!isMute);
        if (isStartMenuShowed)
        {
            if (isMute)
            {
                AudioAndVibrationManeger.instance.play("Menu background music");
            }
            else if (!isMute)
            {
                AudioAndVibrationManeger.instance.stop("Menu background music");
            }
        }

        changeMusicBtnStatus(!isMute);
    }
    public void VibrationBtnClick()
    {
        AudioAndVibrationManeger.instance.play("Buttons click");
        bool isMute = AudioAndVibrationManeger.instance.GetVibrationMute();
        AudioAndVibrationManeger.instance.SetVibrationMute(!isMute);
        changeVibrationBtnStatus(!isMute);
    }


    public void YesExitBtnClick()
    {
        AudioAndVibrationManeger.instance.play("Buttons click");
        Application.Quit();
    }

    public void NoExitBtnClick()
    {
        AudioAndVibrationManeger.instance.play("Buttons click");
        hideExitMenu();
    }


    public void OkAboutBtnClick()
    {
        AudioAndVibrationManeger.instance.play("Buttons click");
        hideAboutmenu();
    }
    void EscapeButtonClick()
    {
        if (!isStartMenuShowed && !_isPauseMenuShowed && !_isLossMenuShowed)
        {
            ShowPauseMenu();
            return;
        }

        if (!_isExitDialogShowed)
        {
            if (_isPauseMenuShowed || isStartMenuShowed)
            {
                showExitMenu();
            }
        }
    }
    #endregion


    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    public void SmoongameBtnClick()
    {
        AudioAndVibrationManeger.instance.play("Buttons click");
        Application.OpenURL("https://smoongame.com/");
    }

    public void OtherAppbtnClick()
    {
        AudioAndVibrationManeger.instance.play("Buttons click");
        MyketIntetnt myketIntent = new MyketIntetnt();
        myketIntent.OpenDeveloperWeb("Smoon game");

        Debug.Log("hjhjhjh");
    }


    public void RankBtnClick()
    {
        AudioAndVibrationManeger.instance.play("Buttons click");

        if (_isRankListShowed)
            return;

        if (Ranking.Instance.CheckInternet())
        {
            _isRankListShowed = true;
            Ranking.Instance.ShowRankList();
        }
        else
        {
            TextBox.Instance.ShowTextBox(TextBox.INTERNET_CONNECTION_ERROR);
        }

    }

    public void OkBtnInRankListClick()
    {
        AudioAndVibrationManeger.instance.play("Buttons click");

        if (_isRankListShowed)
        {
            Ranking.Instance.hideRankList();
            _isRankListShowed = false;
        }

    }



}
