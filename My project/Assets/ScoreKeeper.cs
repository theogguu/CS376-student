using TMPro;
using UnityEngine;

/// <summary>
/// Displays the score in whatever text component is store in the same game object as this
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class ScoreKeeper : MonoBehaviour
{
    /// <summary>
    /// There will only ever be one ScoreKeeper object, so we just store it in
    /// a static field so we don't have to call FindObjectOfType(), which is expensive.
    /// </summary>
    public static ScoreKeeper Singleton;

    /// <summary>
    /// Add points to the score
    /// </summary>
    /// <param name="points">Number of points to add to the score; can be positive or negative</param>
    public static void ScorePoints(int points)
    {
        Singleton.ScorePointsInternal(points);
    }

    /// <summary>
    /// Current score
    /// </summary>
    public int Score;

    /// <summary>
    /// Text component for displaying the score
    /// </summary>
    private TMP_Text scoreDisplay;

    // Audio when player wins
    public AudioSource a;
    public AudioClip win;
    
    // make sure update doesnt get called a million times
    public bool is_win;
    /// <summary>
    /// Initialize Singleton and ScoreDisplay.
    /// </summary>
    // ReSharper disable once UnusedMember.Local
    void Start()
    {
        Singleton = this;
        scoreDisplay = GetComponent<TMP_Text>();
        // Initialize the display
        ScorePointsInternal(0);
        a = GetComponent<AudioSource>();

        is_win = false;
    }

    void Update()
    {
        if ((Score >= 50) && (!is_win))
        {
            is_win = true;
            a.PlayOneShot(win);
            
            Player p = FindObjectOfType<Player>();
            p.is_Freeze = true;
            p.is_Alive = false;
            p.is_win = true;           
        }
    }

    /// <summary>
    /// Internal, non-static, version of ScorePoints
    /// </summary>
    /// <param name="delta"></param>
    private void ScorePointsInternal(int delta)
    {
        Score += delta;
        scoreDisplay.text = $"Score: {Score}";
    }
}
