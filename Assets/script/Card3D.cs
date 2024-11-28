using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class Card3D: MonoBehaviour, IPointerDownHandler
{
    public Guid CardGUID;
    public bool Disabled;
    public Animator m_Animator;
    public GameObject CardFront;
    public Vector3 targetPosition;
    public Action<Card3D> HandleClick;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("3d pointer down");
        if (Disabled) return;
        HandleClick?.Invoke(this);
        //TriggerCardEffect();
    }
    
    public void SetCardState(int state) {
        m_Animator.SetInteger("aniState", state);
    }
    void Start()
    {
        Disabled = true;
  
    }
    void Update()
    {
        if (targetPosition != null)
        {
            float distance = Vector3.Distance(transform.position, targetPosition);
            if (distance > 0.1)
            {
                float step = (distance / 10) * Time.fixedDeltaTime;
                step = Mathf.Max(step,0.05f);
                transform.position = Vector3.Lerp(transform.position, targetPosition, step);

            }
            else
            {
                transform.position = targetPosition;
            }
        }
    }
    public void SetMeterial(Material m_Meterial)
    {
        CardFront.GetComponent<MeshRenderer>().material = m_Meterial;
    }

    private void TriggerCardEffect()
    {
        int aniState = m_Animator.GetInteger("aniState");

         
        if (aniState == 0)
        {
            m_Animator.SetInteger("aniState", 1);

        }
        else if (aniState == 1)
        {
            m_Animator.SetInteger("aniState", 0);
        }

    }
}
