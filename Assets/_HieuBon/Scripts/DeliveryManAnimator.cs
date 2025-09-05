using DG.Tweening;
using System;
using UnityEngine;

public class DeliveryManAnimator : MonoBehaviour
{
    public Animator motobike;
    public Animator shipper;

    public Animation boxAnimation;

    public void Wheelie(bool isEnable)
    {
        shipper.SetBool("Wheelie", isEnable);
        motobike.SetBool("Wheelie", isEnable);
    }

    public void OpenBox()
    {
        boxAnimation.Play("Shipper_Open_Box");
    }
    public void CloseBox()
    {
        boxAnimation.Play("Shipper_Close_Box");
    }
}
