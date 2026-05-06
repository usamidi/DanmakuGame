using System.Collections.Generic;
using UnityEngine;

public enum SFXType
{
    EnemyShoot, //se_tan01
    PlayerShoot, //se_plst_00
    PlayerDead, //se_pldead00
    PlayerGraze, //se_graze
    PlayerHitEnemy, //子弹打中敌人的声音 se_damage00
}

[System.Serializable]
public class SoundEffect
{
    public SFXType type;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float defaultVolume = 1.0f; 
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    public AudioSource sfxSource;
    public AudioSource bgmSource;

    [Header("Sound Effects Config")]
    // 在 Inspector 面板里配置这个列表
    public List<SoundEffect> sfxList;
    
    private Dictionary<SFXType, SoundEffect> sfxDictionary;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // DontDestroyOnLoad(gameObject);

        InitializeSFXDictionary();
    }

    // 4. 将列表转换为字典缓存
    private void InitializeSFXDictionary()
    {
        sfxDictionary = new Dictionary<SFXType, SoundEffect>();
        foreach (var sfx in sfxList)
        {
            if (!sfxDictionary.ContainsKey(sfx.type))
            {
                sfxDictionary.Add(sfx.type, sfx);
            }
            else
            {
                Debug.LogWarning($"[AudioManager] 检测到重复配置的音效类型: {sfx.type}");
            }
        }
    }
    
    public void PlaySFX(SFXType type)
    {
        if (sfxDictionary.TryGetValue(type, out SoundEffect sfx))
        {
            if (sfx.clip != null)
            {
                sfxSource.PlayOneShot(sfx.clip, sfx.defaultVolume);
            }
        }
        else
        {
            Debug.LogWarning($"[AudioManager] 找不到该音效类型的配置: {type}");
        }
    }

    // 重载方法：如果特定情况下需要覆盖默认音量
    public void PlaySFX(SFXType type, float customVolumeScale)
    {
        if (sfxDictionary.TryGetValue(type, out SoundEffect sfx))
        {
            if (sfx.clip != null)
            {
                sfxSource.PlayOneShot(sfx.clip, customVolumeScale);
            }
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }
}