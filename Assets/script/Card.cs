using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.EventSystems;
public class Card : MonoBehaviour,IPointerDownHandler
{
    public GameObject Back;
    public GameObject Image;
    public GameObject Pictrue;
    public Animator CardFlip;
    void Start()
    {
        Debug.Log("card start");
        Back.SetActive(true);
        // animator = GetComponent<Animator>();
        // if (animator == null)
        // {
        //     Debug.LogError("Animator 组件未找到！");
        // }
    }
    void Update()
    {
        //if(Input.GetMouseButtonDown(0)){
        //    StartFlip();
        //}
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        StartFlip();
        //if (timer == 0)
        //{
        //    //StartFlip();
        //}
        //throw new System.NotImplementedException();
        //animator.SetBool("FlipX", this.coverd);
    }
    private void StartFlip() 
    {
        //Back.SetActive(false);
        TriggerCardEffect().Start();
        //await Task.Delay(1000);
        //Back.SetActive(true);
    }

    private async Task TriggerCardEffect()
    {
        int aniState = CardFlip.GetInteger("aniState");
        if (aniState == 0)
        {
            CardFlip.SetInteger("aniState", 1);
            await Task.Delay(100);
            Back.SetActive(false);

        }
        else if(aniState ==1)
        {
            CardFlip.SetInteger("aniState", 0);
            await Task.Delay(100);
            Back.SetActive(true);
        }

    }
    //private void StartFlip()
    //{
    //    if(timer>0)return;
    //    StartCoroutine(CalculateFlip());
    //}
    //private void Flip()
    //{
    //    coverd = !coverd;
    //    Cover.SetActive(coverd);
    //}

    //IEnumerator CalculateFlip()
    //{
    //    for (int i = 0; i < 90; i++)
    //    {
    //        yield return new WaitForSeconds(0.01f);
    //        transform.Rotate(rotate);
    //        timer++;
    //        if (timer == 45 || timer == -45)
    //        {
    //            Flip();
    //        }
    //    }
    //    timer = 0;
    //}
}
