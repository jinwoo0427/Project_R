using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="Camera/headBob")]
public class HeadbobSetting : ScriptableObject
{
	public Transform target;

	[Tooltip("하나의 헤드밥 사이클에 필요한 최소 거리. 캐릭터의 발자국 거리를 나타냄.")]
	public float cycleDistance = 3;

	public float maxVerticalIntensity = 0.15f;

	public float maxHorizontalIntensity = 0.25f;

	public float intensitySpeedMultiplier = 1;

	[Header("고급")]

	public AnimationCurve verticalIntensityCurve;

	public AnimationCurve horizontalIntensityCurve;

	public float lerpSpeed = 5;

	public float resetLerpSpeed = 5;
}
