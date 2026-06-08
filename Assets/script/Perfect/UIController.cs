using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public static UIController instance;
    private static bool skipStartUI = false;

    [Header("按钮")]
    [Tooltip("没有存档时会自动隐藏此按钮")]
    [SerializeField] private GameObject continueBot;
    [SerializeField] private GameObject setUi;
    [SerializeField] private GameObject deathUi;
    [SerializeField] private GameObject startUi;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private SaveController saveController;
    private Animator animSet;
    private Animator animDie;
    private Animator animStart;

    [SerializeField] private PlayerController player;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }

        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        animSet = setUi.GetComponent<Animator>();
        animDie = deathUi.GetComponent<Animator>();
        animStart = startUi.GetComponent<Animator>();

        deathUi.SetActive(false);
        setUi.SetActive(false);

        if(GameManager.instance !=null)
        {
            UpdateCoin(GameManager.instance.CoinCount);
        }


        //如果存档存在则显示continue按钮
        if (continueBot != null)
        {
            continueBot.SetActive(SaveController.HasSaveFile());
        }

        // 重启时跳过开始界面，直接进入游戏
        if (skipStartUI)
        {
            skipStartUI = false;
            startUi.SetActive(false);
            animStart.SetBool("Out", true);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //isEsc = !isEsc;
            setUi.SetActive(true);
            animSet.SetBool("AppearSet", true);
        }
    }

    //Coin
    public void UpdateCoin(int count)
    {
        coinText.text = "Coin : "+ count;

    }

    /// <summary>
    /// setUI
    /// </summary>
    public void LoadNewGame()
    {
        AudioManager.Instance.PlayClickSound();
        SaveController.AutoLoadOnStart = false;
        //动画切换
        startUi.SetActive(false);
        animStart.SetBool("Out",true);

    }

    // 继续游戏：直接读档传送到存档点
    public void LoadContinue()
    {
        AudioManager.Instance.PlayClickSound();
        if (saveController != null)
        {
            saveController.Load();
        }
        startUi.SetActive(false);
        animStart.SetBool("Out", true);
    }

    //退出游戏
    public void ExitGame()
    {
        AudioManager.Instance.PlayClickSound();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }


    /// <summary>
    /// setUI
    /// </summary>
    //set-> 游戏界面 返回游戏
    public void ReturnGame()
    {
        AudioManager.Instance.PlayClickSound();
        setUi.SetActive(false);
        animSet.SetBool("AppearSet", false);
    }

    //回到开始界面
    public void ReturnBegin()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        setUi.SetActive(false);
        startUi.SetActive(true);
        animStart.SetBool("Out",false);
    }

    //胜利返回开始处
    public void EndReturn()
    {
        AudioManager.Instance.PlayClickSound();
        skipStartUI =true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }

    /// <summary>
    /// DieUI
    /// </summary>
    public void DieYes()
    {
       
        deathUi.SetActive(true);
        Invoke(nameof(PlayDieAnimation), 0.01f);

        
    }

    
    void PlayDieAnimation()
    {
        animDie.SetBool("AppearDie", true);
    }

    public void DieNo()
    {
        AudioManager.Instance.PlayClickSound();
       
        StartCoroutine(CloseUi());
        print("玩家点击 Try Again,触发复活");

    }

    private IEnumerator CloseUi()
    {
        animDie.SetBool("AppearDie", false);

        if (player != null)
        {
            player.IsRespawn(); 
        }

        yield return new WaitForSeconds(1f);

        deathUi.SetActive(false);


    }

   
    public void RestartGame()
    {
        AudioManager.Instance.PlayClickSound();
        StopAllCoroutines();
        skipStartUI = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void VolumeSetIn()
    {
        AudioManager.Instance.PlayClickSound();
        animSet.SetBool("flag1",true);
    }
    

    public void VolumeSetOut()
    {
        AudioManager.Instance.PlayClickSound();
        animSet.SetBool("flag1",false);
    }

}
