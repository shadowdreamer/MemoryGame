using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Messages : MySingleton<Messages>
{
    public GameObject MessageBox;
    // Start is called before the first frame update
    float duration = 0.1f;
    float hanging = 0.4f;
    public void  ShowMsg(string str)
    {
        TextMeshProUGUI tmp = MessageBox.GetComponent<TextMeshProUGUI>();

        tmp.text = str;
        MsgAnimation();
        AudioManager.instance.Play("bip");
    }
    void MsgAnimation()
    {
        TextMeshProUGUI Mesh = MessageBox.GetComponent<TextMeshProUGUI>();

        MessageBox.SetActive(true);
        Mesh.alpha = 0.2f;
        MessageBox.transform.localPosition = new Vector3(-900, 0, 0);

        Sequence sequence = DOTween.Sequence();

        sequence.Append( Mesh.transform.DOLocalMoveX(0, duration).SetEase(Ease.InQuad) );
        sequence.Append(Mesh.DOFade(1, duration).SetEase(Ease.InQuad));

        sequence.AppendInterval(hanging);



        sequence.Append(Mesh.transform.DOLocalMoveX(900, duration).SetEase(Ease.OutQuad));
        sequence.Append(Mesh.DOFade(0.2f, duration).SetEase(Ease.OutQuad));

        sequence.onComplete = () =>
        {
            MessageBox.SetActive(false);
        };

    }
}
