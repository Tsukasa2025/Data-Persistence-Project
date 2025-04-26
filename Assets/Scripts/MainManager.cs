using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;
    public SaveData HighScoreData = new SaveData();
    public static string UserName;

    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text HighScoreText;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    // Awake()
    void Awake()
    {
        //  メニューに戻さないので、singletons の処理は不要。有効にすると メイ
        //  ンシーンの表示がおかしくなる

        /*
        if (Instance != null)
        {
            Destroy(gameObject);

            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadScore();
        */

        //Instance = this;

        // ハイスコアの読み取りと表示
        SaveData data = LoadScore();

        if (data != null)
        {
            HighScoreData = data;
        }

        HighScoreText.text = GetHighScoreString(HighScoreData.Name,
            HighScoreData.Score);
    }

    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
    }

    // UpdateHighScore()
    public void UpdateHighScore()
    {
        // これを DeathZone.OnCollisionEnter() から呼ぶ

        if (m_Points > HighScoreData.Score)
        {
            // Serialize UserName and Score
            HighScoreData = new SaveData(MainManager.UserName, m_Points);
            SaveScore();

            // Update HighScoreText
            HighScoreText.text = GetHighScoreString(MainManager.UserName,
                m_Points);
        }
    }

    // GetHighScoreString(string, int)
    public static string GetHighScoreString(string name, int point)
    {
        return $"Best Score : {name} : {point}";
    }

    // class SaveData
    [System.Serializable]
    public class SaveData
    {
        public string Name;
        public int Score;

        // SaveData()
        public SaveData() { }

        // SaveData(string, int)
        public SaveData(string name, int score)
        {
            Name = name;
            Score = score;
        }
    }

    // SaveScore()
    public void SaveScore()
    {
        string json = JsonUtility.ToJson(HighScoreData);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json",
            json);
    }

    // LoadScore()
    public static SaveData LoadScore()
    {
        SaveData data = null;
        string path = Application.persistentDataPath + "/savefile.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            data = JsonUtility.FromJson<SaveData>(json);
        }

        return data;
    }
}
