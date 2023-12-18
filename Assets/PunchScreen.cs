using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PunchScreen : MonoBehaviour
{
   public void Punch()
   {
       transform.DOShakeRotation(3, 10, 10, 90, false);
   }
}
