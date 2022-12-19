using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEvent : MonoBehaviour
{
    //이벤트
    public event Action<float> OnLand = delegate { };
    public event Action OnJump = delegate { };
    public event Action<int> OnJumpsCountIncrease = delegate { };
    public event Action OnSlide = delegate { }; //각 프레임 호출
    public event Action OnEndSlide = delegate { };
    public event Action OnBeginSlide = delegate { };

    public event Action OnBeginGrapplingLine = delegate { };
    public event Action OnEndGrapplingLine = delegate { };
    public event Action OnEndFailedGrapplingLine = delegate { };
    public event Action OnGrapplingLine = delegate { }; //각 프레임 호출
    public event Action OnBeginGrappling = delegate { };
    public event Action OnEndGrappling = delegate { };
    public event Action OnGrappling = delegate { }; //각 프레임 호출

    public event Action OnBeginWallRun = delegate { };
    public event Action OnWallRun = delegate { }; //각 프레임 호출
    public event Action OnEndWallRun = delegate { };

    public event Action OnClimbBegin = delegate { };
    public event Action OnClimbEnd = delegate { };

    public event Action OnColliderResized = delegate { };


}
