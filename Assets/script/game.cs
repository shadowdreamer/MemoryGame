using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    // 单例
    private static GameControl instance;

    public static GameControl Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("LevelPanel instance not found in the scene!");
            }
            return instance;
        }
    }


    [Header("UI Components")]
    public GameObject StartButton;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI ScoreWillGainText;
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI ExpText;

    // Start is called before the first frame update
    public GameObject cardPrefab;
    List<GameObject> CardsList = new() { };

    Card3D CheckingCard;
    private volatile bool _isChecking = false;

    
    // 基本参数
    public float CheckTime = 2f;
    public float BaseScore = 100f;
    public int BaseExp = 10;
    // game scores
    float Score;
    long Exp = 0;
    long Level = 1;
    long TotalNextExp = 0;
    long ExpForNext = 30;

    float ScoreWillGain = 100f;

    private void Awake()
    {
        // 确保单例
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        };
        TotalNextExp = ExpToLevel.GetCurrentLevel(1);
        StateDisplayUpdate();
    }
    void Start()
    {
        initGame();
    }
    private void Update()
    {
        StateDisplayUpdate();
    }
    void StateDisplayUpdate()
    {
        ScoreWillGainText.text = $"+{ScoreWillGain}";
        ScoreText.text = $"{Score}";
        LevelText.text = $"LEVEL: {Level}";
        ExpText.text = $"EXP: {TotalNextExp - ExpForNext}/{TotalNextExp}";
    }
    public void initGame()
    {
        CardStore.Instance.GetAllCardImages();

    }
    public async void ClickCheck(Card3D card)
    {
        if (_isChecking) return;
        _isChecking = true;
        var id = card.CardGUID;
        card.SetCardState(0);// 翻开卡片 0为正面 1盖卡


        if (CheckingCard == null)
        {
            CheckingCard = card;
            AudioManager.instance.Play("card-flip");
        }
        else
        {
            var lastId = CheckingCard.CardGUID;
            if (lastId == id)
            {
                card.Disabled = true;
                CheckingCard.Disabled = true;
                CheckingCard = null;
                AudioManager.instance.Play("flap");
                CheckTableStatus();

            }
            else
            {
                AudioManager.instance.Play("card-flip");
                await Task.Delay(500);
                AudioManager.instance.Play("card-pia");
                ScoreGetDown();
                CheckingCard.SetCardState(1);
                card.SetCardState(1);
                CheckingCard = null;
            }

        }
        _isChecking = false;
    }
    void ScoreGetDown()
    {
        if (ScoreWillGain > 40)
            ScoreWillGain -= 10;
    }
    public void StartDrawCardCoroutine()
    {
        StartButton?.SetActive(false);
        StartCoroutine(CastCards());
    }
    public IEnumerator CastCards()
    {
        Vector3 idlePos = new Vector3(0, -210f, -20f);
        List<Material> mats = CardStore.Instance.GetRandMaterials(5);

        foreach (Material _mat in mats)
        {
            Guid id = Guid.NewGuid();
            for (int i = 0; i < 2; i++)
            {
                GameObject gob = Instantiate(cardPrefab, idlePos, new Quaternion(0f, 0f, 0f, 1f));
                gob.GetComponent<Card3D>().SetMeterial(_mat);
                gob.GetComponent<Card3D>().targetPosition = idlePos;
                gob.GetComponent<Card3D>().CardGUID = id;
                gob.GetComponent<Card3D>().HandleClick = ClickCheck;

                CardsList.Add(gob);

            }
        }
        CardsList.Shuffle();
        //await Task.Delay(300);
        yield return new WaitForSeconds(0.3f);

        int col = 0, row = 0;
        for (int i = 0; i < CardsList.Count; i++)
        {
            AudioManager.instance.Play("card-cast", true);
            if (col > 4)
            {
                col = 0;
                row += 1;

            }
            CardsList[i].GetComponent<Card3D>().targetPosition = new Vector3(40 + 40 * col, -30 - row * 60, -20);
            col++;

            yield return new WaitForSeconds(0.046f);

        }
        Messages.Instance.ShowMsg("CHECK!!");
        yield return new WaitForSeconds(CheckTime);
        Messages.Instance.ShowMsg("START!!");

        foreach (GameObject gob in CardsList)
        {
            gob.GetComponent<Card3D>().Disabled = false;
            gob.GetComponent<Card3D>().SetCardState(1);
        }

        CountdownTimer.Instance.StartCountdown();

    }

    void CheckTableStatus()
    {
            StartCoroutine(StartNextRound());
        //var enableCards = CardsList.Find(gob =>
        //{
        //    return gob.GetComponent<Card3D>().Disabled == false;
        //});
        //if (enableCards == null)
        //{
        //}
    }
    IEnumerator StartNextRound()
    {

        Exp += BaseExp;
        Score += ScoreWillGain;
        var curLv = ExpToLevel.GetLevelFromExp(Exp, out ExpForNext);
        TotalNextExp = ExpToLevel.GetCurrentLevel(curLv);
        Debug.Log($"exp{Exp} level {curLv} next exp {ExpForNext}");
        yield return new WaitForSeconds(1f);
        ClearTable();
        CountdownTimer.Instance.PauseCountdown();
        if (curLv > Level)
        {
            bool buttonClicked = false;
            Level = curLv;
            LevelPanel.Instance.ShowLevelUp(() =>
            {
                buttonClicked = true;
            });
            while (!buttonClicked)
            {
                yield return null;
            }
        }
        ScoreWillGain = BaseScore;
        StartDrawCardCoroutine();
    }

    public void ClearTable()
    {
        foreach (GameObject gameObject in CardsList)
        {
            Destroy(gameObject);
        }
        CardsList.Clear();
    }

}

public class ExpToLevel
{
    // 参数配置
    private const int baseExp = 20;
    private const float increment = 1.5f;
    private const int growth = 10;
 
 
     

    // 获取某等级所需的总经验
    public static long GetTotalExpForLevel(long level)
    {
        if (level <= 1) return 0; // 1级需要0经验
        return (long)(baseExp * Mathf.Pow(level - 1, increment) + growth);
    }
    public static long GetCurrentLevel(long level)
    {
        if (level <= 1) return 30; // 1级需要0经验
        return GetTotalExpForLevel(level+1) - GetTotalExpForLevel(level);
    }
    // 计算当前等级和经验进度
    public static long GetLevelFromExp(long currentExp, out long expForNextLevel)
    {
        int level = 1;
        while (GetTotalExpForLevel(level + 1) <= currentExp)
        {
            level++;
        }

        // 计算距离下一级的剩余经验
        expForNextLevel = GetTotalExpForLevel(level + 1) - currentExp;
        return level;
    }
}

