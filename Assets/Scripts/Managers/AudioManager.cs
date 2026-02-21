using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip cardFlipClip;
    [SerializeField] private AudioClip matchClip;
    [SerializeField] private AudioClip mismatchClip;
    [SerializeField] private AudioClip gameOverClip;

    [Header("Settings")]
    [SerializeField][Range(0f, 1f)] private float sfxVolume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }


    private void OnEnable()
    {
        GameManager.OnCardFlipped += HandleCardFlip;
        GameManager.OnCardsMatched += HandleMatch;
        GameManager.OnCardsMismatched += HandleMismatch;
        GameManager.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        GameManager.OnCardFlipped -= HandleCardFlip;
        GameManager.OnCardsMatched -= HandleMatch;
        GameManager.OnCardsMismatched -= HandleMismatch;
        GameManager.OnGameOver -= HandleGameOver;
    }

    private void HandleCardFlip(Card card)
    {
        PlayClip(cardFlipClip);
    }

    private void HandleMatch(Card cardA, Card cardB)
    {
        PlayClip(matchClip);
    }

    private void HandleMismatch(Card cardA, Card cardB)
    {
        PlayClip(mismatchClip);
    }

    private void HandleGameOver()
    {
        PlayClip(gameOverClip);
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;
        audioSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }
}