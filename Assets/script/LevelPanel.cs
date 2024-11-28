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
    // ����
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

    // ��������
    [SerializeField] private float flyInDuration = 0.2f; // ����ʱ��
    [SerializeField] private Vector3 hiddenPosition = new Vector3(0, 800, 0); // ��ʼ����λ�ã���Ļ�Ϸ���
    [SerializeField] private Vector3 visiblePosition = Vector3.zero; // ��Ļ����λ��
    private RectTransform rectTransform;

    private bool isVisible = false;

    LevelUpBonus choosing;
    Action doNext;
    private void Awake()
    {
        // ȷ������
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // ��ȡ RectTransform
        rectTransform = GetComponent<RectTransform>();

        // ��ʼ��Ϊ���ɼ�״̬
        rectTransform.anchoredPosition = hiddenPosition;
    }

    /// <summary>
    /// ��ʾ Panel�����Ϸ�����
    /// </summary>
    public void ShowPanel()
    {
        if (isVisible) return; // ����Ѿ��ɼ������ظ���ʾ

        isVisible = true;
        AudioManager.instance.Play("levelUp");
        //ʹ�� DOTween ����
        rectTransform.DOAnchorPos(visiblePosition, flyInDuration)
                     .SetEase(Ease.OutCubic)
                     .onComplete=()=> {

                         LevelUpAnime();
                     };


    }

    /// <summary>
    /// ���� Panel���ɻ��Ϸ�
    /// </summary>
    public void HidePanel()
    {
        if (!isVisible) return; // ����Ѿ����أ����ظ�����

        isVisible = false;

        // ʹ�� DOTween ����
        rectTransform.DOAnchorPos(hiddenPosition, flyInDuration)
                     .SetEase(Ease.InCubic)
                     .OnComplete(() => Debug.Log("Panel �������"));

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

                // �󶨵���¼�
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
            Debug.LogError("Ԫ��������Ȩ��������ƥ�䣡");
            return new List<T>();
        }

        List<T> selectedElements = new List<T>();
        List<T> availableElements = new List<T>(elements);
        List<float> availableWeights = new List<float>(weights);

        for (int i = 0; i < count; i++)
        {
            // ����Ȩ��ѡ��һ��Ԫ��
            T selectedElement = SelectByWeight(availableElements, availableWeights);

            // ��ѡ�е�Ԫ����ӵ������
            selectedElements.Add(selectedElement);

            // �Ƴ���ѡ��Ԫ�غ����Ӧ��Ȩ��
            int index = availableElements.IndexOf(selectedElement);
            availableElements.RemoveAt(index);
            availableWeights.RemoveAt(index);
        }

        return selectedElements;
    }
    // ����Ȩ��ѡ��һ��Ԫ��
    private T SelectByWeight<T>(List<T> elements, List<float> weights)
    {
        // ����Ȩ���ܺ�
        float totalWeight = 0f;
        foreach (var weight in weights)
        {
            totalWeight += weight;
        }

        // ����һ�����ֵ��ѡ��һ��Ԫ��
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

        return default(T); // ���ᵽ�����ֹ����������
    }
    LevelUpBonus M1(int r)
    {
        var val = 15 + 5 * r;
        return new LevelUpBonus(r,
            () => $"����<color=rareColor>{val}s</color>ʣ��ʱ�䣨���ᳬ�����ʱ�䣩",
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
            () => $"����<color=rareColor>{val}s</color>��󵹼�ʱʱ�䣨ʣ��ʱ��Ҳ���ӣ�",
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
            () => $"����<color=rareColor>{val}</color> ��������",
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
            () => $"����<color=rareColor>{val}��</color> ��ɺ��EXP",
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
            () => $"����<color=rareColor>{val}s</color> ����ʱ��",
            () =>
            {
                GameControl.Instance.CheckTime += val;
            }
        );
    }
}
