using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.Reflection;
using static UnityEngine.GraphicsBuffer;
using System.Xml;

public class LevelPanel : MonoBehaviour
{

    [Header("UI Components")]
    public GameObject Warp;
    public GameObject BonusItem;
    public GameObject BonusGrid;
    public GameObject SelectPointer;
    public GameObject LevelUpText;
    public GameObject ConfirmButton;
    // 单例
    private static LevelPanel instance;

    public static LevelPanel Instance
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

    MethodCollection BonusCollction;

    // 动画参数
    [SerializeField] private float flyInDuration = 0.2f; // 飞入时间
    [SerializeField] private Vector3 hiddenPosition = new Vector3(0, 800, 0); // 初始隐藏位置（屏幕上方）
    [SerializeField] private Vector3 visiblePosition = Vector3.zero; // 屏幕中央位置
    private RectTransform rectTransform;

    private bool isVisible = false;

    LevelUpBonus choosing;
    Action doNext;
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
        }

        // 获取 RectTransform
        rectTransform = GetComponent<RectTransform>();

        // 初始化为不可见状态
        rectTransform.anchoredPosition = hiddenPosition;
    }

    /// <summary>
    /// 显示 Panel，从上方飞入
    /// </summary>
    public void ShowPanel()
    {
        if (isVisible) return; // 如果已经可见，不重复显示

        isVisible = true;
        AudioManager.instance.Play("levelUp");
        //使用 DOTween 动画
        rectTransform.DOAnchorPos(visiblePosition, flyInDuration)
                     .SetEase(Ease.OutCubic)
                     .onComplete=()=> {

                         LevelUpAnime();
                     };


    }

    /// <summary>
    /// 隐藏 Panel，飞回上方
    /// </summary>
    public void HidePanel()
    {
        if (!isVisible) return; // 如果已经隐藏，不重复隐藏

        isVisible = false;

        // 使用 DOTween 动画
        rectTransform.DOAnchorPos(hiddenPosition, flyInDuration)
                     .SetEase(Ease.InCubic)
                     .OnComplete(() => Debug.Log("Panel 隐藏完成"));

    }

    public void ShowLevelUp(Action clicked)
    {
        ConfirmButton.SetActive(false);
        doNext = clicked;
        BonusCollction ??= new MethodCollection();
           // clear first
        foreach (Transform child in BonusGrid.transform)
        {
            Destroy(child.gameObject);
        }
        SelectPointer.SetActive(false);
        foreach (LevelUpBonus boun in BonusCollction.GetRandomMethods(3))
        {
            var ob = Instantiate(BonusItem, BonusGrid.transform.position, BonusGrid.transform.rotation);
            if (ob.TryGetComponent<Button>(out var button))
            {

                // 绑定点击事件
                button.onClick.AddListener(() =>
                {
                    choosing = boun;
                    MovePointerToBox(ob);
                    ConfirmButton.SetActive(true);

                });

            }
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = boun.GetDes();
            ob.transform.SetParent(BonusGrid.transform, false);
        }
        ShowPanel();
    }
    public void SubmiLevelUp()
    {
        if (choosing != null)
        {
            choosing.onLevelUp();
            HidePanel();
            doNext?.Invoke();
            foreach (Transform child in BonusGrid.transform)
            {
                Destroy(child.gameObject);
            }
            choosing = null;
            AudioManager.instance.Play("hint");
        }
    }

    public void MovePointerToBox(GameObject ob)
    {
        
        Vector3 relativePosition = Warp.transform.InverseTransformPoint(ob.transform.position);
        relativePosition.y -= 120;
        SelectPointer.SetActive(true);
        Debug.Log(relativePosition);
        //SelectPointer.transform.position = relativePosition;
        SelectPointer.transform.DOLocalMove(relativePosition, 0.2f).SetEase(Ease.OutQuad);
        AudioManager.instance.Play("cursor");

    }
    public void LevelUpAnime()
    {
        TextMeshProUGUI Mesh = LevelUpText.GetComponent<TextMeshProUGUI>();
        if(Mesh != null)
        {
            var loop = Mesh.DOFade(0f, 0.1f).SetLoops(10,LoopType.Yoyo)
                .onComplete = ()=> {
                    Mesh.alpha = 1f;
                };
        }
    }
}

public class LevelUpBonus
{
    public int rarity;
    public Func<string> des;

    public Action onLevelUp;
 
    public LevelUpBonus(int rarity, Func<string> des, Action onLevelUp)
    {
        this.rarity = rarity;
        this.des = des;
        this.onLevelUp = onLevelUp;
    }
    public string GetDes()
    {
        string str = des();
        string color = "#000000";
        if (rarity == 0) color = "#1EFF00";
        if (rarity == 1) color = "#0070DD";
        if (rarity == 2) color = "#A335EE";
        if (rarity == 5) color = "#FF8000";
        if (rarity == 10) color = "#DF2A8A";

        return str.Replace("rareColor", color);
    }
}
 public class MethodCollection
{


    List<int> rarites = new() { 0, 1, 2, 5, 10 };
    List<float> rWeights = new() { 60f, 30f, 20f, 10f, 2f };

    public IEnumerable<LevelUpBonus> GetRandomMethods(int count)
    {
        List<Func<int,LevelUpBonus>> methods = new() {
            M1,M2,M3,M4,M5
        };
        List<float> metWeights = new () { 5f, 3f, 2f, 1f, 1f };
        List<Func<int, LevelUpBonus>> picked = GetRandomElementsWithWeights(methods, metWeights, count);

        for (int i = 0; i < count; i++)
        {
            int r = SelectByWeight(rarites,rWeights);

            yield return picked[i](r);

        }
    }

    public List<T> GetRandomElementsWithWeights<T>(List<T> elements, List<float> weights, int count)
    {
        if (elements.Count != weights.Count)
        {
            Debug.LogError("元素数量和权重数量不匹配！");
            return new List<T>();
        }

        List<T> selectedElements = new List<T>();
        List<T> availableElements = new List<T>(elements);
        List<float> availableWeights = new List<float>(weights);

        for (int i = 0; i < count; i++)
        {
            // 根据权重选择一个元素
            T selectedElement = SelectByWeight(availableElements, availableWeights);

            // 将选中的元素添加到结果中
            selectedElements.Add(selectedElement);

            // 移除已选的元素和其对应的权重
            int index = availableElements.IndexOf(selectedElement);
            availableElements.RemoveAt(index);
            availableWeights.RemoveAt(index);
        }

        return selectedElements;
    }
    // 根据权重选择一个元素
    private T SelectByWeight<T>(List<T> elements, List<float> weights)
    {
        // 计算权重总和
        float totalWeight = 0f;
        foreach (var weight in weights)
        {
            totalWeight += weight;
        }

        // 生成一个随机值，选择一个元素
        float randomValue = UnityEngine.Random.Range(0f, totalWeight);

        float cumulativeWeight = 0f;
        for (int i = 0; i < elements.Count; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue <= cumulativeWeight)
            {
                return elements[i];
            }
        }

        return default(T); // 不会到这里，防止编译器错误
    }
    LevelUpBonus M1(int r)
    {
        var val = 15 + 5 * r;
        return new LevelUpBonus(r,
            () => $"增加<color=rareColor>{val}s</color>剩余时间（不会超过最大时间）",
            () =>
            {
                CountdownTimer.Instance.IncreaceCountdown(val);
            }
        );
    }
    LevelUpBonus M2(int r)
    {
        var val = 10 + 6 * r;
        return new LevelUpBonus(r,
            () => $"增加<color=rareColor>{val}s</color>最大倒计时时间（剩余时间也增加）",
            () =>
            {
                CountdownTimer.Instance.IncreaceAllTime(val);
            }
        );
    }
    LevelUpBonus M3(int r)
    {
        var val = 5 + 5 * r;
        return new LevelUpBonus(r,
            () => $"增加<color=rareColor>{val}</color> 基础分数",
            () =>
            {
                GameControl.Instance.BaseScore += val;
            }
        );
    }
    LevelUpBonus M4(int r)
    {
        var val = 4 + 2 * r;

        return new LevelUpBonus(r,
            () => $"增加<color=rareColor>{val}点</color> 完成后的EXP",
            () =>
            {
                GameControl.Instance.BaseExp += val;
            }
        );
    }
    LevelUpBonus M5(int r)
    {
        var val = 0.2f + 0.1f * r;
        return new LevelUpBonus(r,
            () => $"增加<color=rareColor>{val}s</color> 记忆时间",
            () =>
            {
                GameControl.Instance.CheckTime += val;
            }
        );
    }
}
