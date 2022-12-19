using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    //스프링 로프스크립트에 설명된 값
    private float strength;
    private float damper;
    private float target;
    private float velocity;
    private float value;
     
    public void SpringUpdate(float deltaTime)
    {
        // 이해할 수 없는 몇 가지 공식을 사용하여 애니메이션 값을 계산했음
        var direction = target - value >= 0 ? 1f : -1f;
        var force = Mathf.Abs(target - value) * strength;
        velocity += (force * direction - velocity * damper) * deltaTime;
        value += velocity * deltaTime;
    }

    public void Reset()
    {
        // 재설정 값
        velocity = 0f;
        value = 0f;
    }

    /// 여기에서 시뮬레이션 변수를 설정하는 데 사용되는 모든 기능을 찾을 수 있슴
    #region Setters

    public void SetValue(float value)
    {
        this.value = value;
    }

    public void SetTarget(float target)
    {
        this.target = target;
    }

    public void SetDamper(float damper)
    {
        this.damper = damper;
    }

    public void SetStrength(float strength)
    {
        this.strength = strength;
    }

    public void SetVelocity(float velocity)
    {
        this.velocity = velocity;
    }

    public float Value => value;

    #endregion
}

