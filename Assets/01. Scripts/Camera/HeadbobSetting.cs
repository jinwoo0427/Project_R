using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="Camera/headBob")]
public class HeadbobSetting : ScriptableObject
{
	public Transform target;

	[Tooltip("�ϳ��� ���� ����Ŭ�� �ʿ��� �ּ� �Ÿ�. ĳ������ ���ڱ� �Ÿ��� ��Ÿ��.")]
	public float cycleDistance = 3;

	public float maxVerticalIntensity = 0.15f;

	public float maxHorizontalIntensity = 0.25f;

	public float intensitySpeedMultiplier = 1;

	[Header("���")]

	public AnimationCurve verticalIntensityCurve;

	public AnimationCurve horizontalIntensityCurve;

	public float lerpSpeed = 5;

	public float resetLerpSpeed = 5;
}
