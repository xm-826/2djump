
using UnityEngine;

public class AudioManager : MonoBehaviour
{   
    
    [Header("背景音乐")]
    //播放音乐的组件
    private AudioSource bgmSource;
    //音乐文件
    [SerializeField] private AudioClip bgmClip;
    
    [Header("特效音乐")]
    private AudioSource effectSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip getCoinSound;

 
    public static AudioManager Instance;
    private void Awake()
    {
        if(Instance ==null)
            Instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        // 获取两个AudioSource，顺序对应组件添加顺序
        AudioSource[] sources = GetComponents<AudioSource>();
        bgmSource = sources[0];
        effectSource = sources[1];

        //循环设置
        bgmSource.loop = true;
        effectSource.loop = false;

    }

    //背景音乐的播放
   public void PlayBGM()
    {
        bgmSource.clip = bgmClip;
        if(!bgmSource.isPlaying)
            bgmSource.Play();
        Debug.Log("音乐启用");
    }

    //暂停音乐
    public void PauseBGM() => bgmSource.Pause();
    //继续播放（从暂停处继续播放）
    public void ResumBGM() => bgmSource.UnPause();
    //停止音乐
    public void StopBGM() => bgmSource.Stop();
    //设置音乐
    public void SetBGMVolume(float vol) => bgmSource.volume = Mathf.Clamp01(vol);

    //播放点击音效
    public void PlayClickSound() => effectSource.PlayOneShot(clickSound);
    //播放跳跃音效
    public void PlayJumpSound() => effectSource.PlayOneShot(jumpSound);
    //播放受击音效
    public void PlayHitSound() => effectSource.PlayOneShot(hitSound);
    //播放金币音效
    public void PlayGetCoinSound() => effectSource.PlayOneShot(getCoinSound);
    
   
}
