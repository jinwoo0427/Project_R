using System;
using UnityEngine;

namespace Jinwoo.FirstPersonController
{
	//'FirstPersonController' 클래스의 오디오 처리 담당
	public class FirstPersonControllerAudio : MonoBehaviour
	{
		#region Fields
		[Header("References "), Space(5)]

		[SerializeField]
		private FirstPersonController controller;
		[Space(5)]
		[Header("Settings"), Space(5)]

		//이 스크립트의 모든 sfx에 대한 마스터 볼륨 수정자
		[SerializeField, Range(0f, 1f)]
		private float masterVolume = 0.1f;

		//발자국 특수 효과를 트리거하기 위해 캐릭터가 이동해야 하는 최소 거리
		[SerializeField]
		private float footstepMinimumDistance = 0.2f;

		//발자국 sfx의 볼륨에 무작위 왜곡을 적용함
		[SerializeField]
		private float footstepRandomVolumeDelta = 0.2f;

		[SerializeField]
		private float proneFootstepMinimumDistance = 0.2f;

		//발자국 sfx의 볼륨에 무작위 왜곡을 적용함
		[SerializeField]
		private float proneFootstepRandomVolumeDelta = 0.2f;

		private float footstepControllerSpeedMinimumThreshold = 0.05f;

		[Space(5)]
		[Header("Audio Clips"), Space(5)]

		[SerializeField]
		private AudioClip[] footstepsClips;

		[SerializeField]
		private AudioClip[] proneFootstepsClips;

		[SerializeField]
		private AudioClip[] jumpsCountIncreasesClips;

		[SerializeField]
		private AudioClip jumpClip;

		[SerializeField]
		private AudioClip landClip;

		[SerializeField]
		private AudioClip beginSlideClip;

		[SerializeField]
		private AudioClip slideClip;

		[SerializeField]
		private AudioClip endSlideClip;

		[SerializeField]
		private AudioClip grapplingClip;

		[SerializeField]
		private AudioClip beginGrapplingClip;

		[SerializeField]
		private AudioClip endGrapplingClip;

		[SerializeField]
		private AudioClip beginGrapplingLineClip;

		[SerializeField]
		private AudioClip endGrapplingLineClip;

		[SerializeField]
		private AudioClip endFailedGrapplingLineClip;

		[SerializeField]
		private AudioClip beginWallRunClip;

		[SerializeField]
		private AudioClip wallRunClip;

		[SerializeField]
		private AudioClip endWallRunClip;

		[SerializeField]
		private AudioClip beginClimbClip;

		[SerializeField]
		private AudioClip endClimbClip;

		private Transform tr;

		//마지막 발자국 소리 재생까지의 거리
		private float currentFootstepDistance = 0f;

		private AudioSource currentSlideAudioSource;
		private float slideSoundCooldown;
		private float lastTimeSlide;

		private AudioSource currentGrapplingAudioSource;
		private float grapplingSoundCooldown;
		private float lastTimeGrappling;

		private AudioSource currentWallRunAudioSource;
		private float wallRunSoundCooldown;
		private float lastTimeWallRun;

		#endregion

		#region Methods
		private void Start()
		{
			//변환 캐싱은 이전 버전의 Unity에서 더 빠르다고 함(아마두?)
			tr = transform;

			//이벤트 플러그
			controller.OnLand += OnLand;
			controller.OnJump += OnJump;
			controller.OnJumpsCountIncrease += OnJumpsCountIncrease;

			controller.OnSlide += OnSlide;
			controller.OnEndSlide += OnEndSlide;
			controller.OnBeginSlide += OnBeginSlide;

			controller.OnGrappling += OnGrappling;
			controller.OnBeginGrappling += OnBeginGrappling;
			controller.OnEndGrappling += OnEndGrappling;

			controller.OnBeginGrapplingLine += OnBeginGrapplingLine;
			controller.OnEndGrapplingLine += OnEndGrapplingLine;
			controller.OnEndFailedGrapplingLine += OnEndFailedGrapplingLine;

			controller.OnBeginWallRun += OnBeginWallRun;
			controller.OnWallRun += OnWallRun;
			controller.OnEndWallRun += OnEndWallRun;

			controller.OnClimbBegin += OnClimbBegin;
			controller.OnClimbEnd += OnClimbEnd;

			//슬라이드하는 동안 루프를 통과하기 위해 슬라이드 클립의 길이를 캐시함
			if (slideClip)
			{
				slideSoundCooldown = slideClip.length;
				lastTimeSlide = -slideClip.length;
			}

			//그래플링하는 동안 반복하기 위해 그래플링 클립의 길이를 캐시
			if (grapplingClip)
			{
				grapplingSoundCooldown = grapplingClip.length;
				lastTimeGrappling = -grapplingClip.length;
			}

			//그래플링하는 동안 반복하기 위해 그래플링 클립의 길이를 캐시함
			if (wallRunClip)
			{
				wallRunSoundCooldown = wallRunClip.length;
				lastTimeWallRun = -wallRunClip.length;
			}
		}

		private void OnDestroy()
		{
			controller.OnLand -= OnLand;
			controller.OnJump -= OnJump;

			//FirstPersonController.OnSlide 이벤트는 캐릭터가 미끄러지는 동안 고정 업데이트마다 호출됨
			controller.OnSlide -= OnSlide;

			controller.OnEndSlide -= OnEndSlide;
		}

		//Update;
		void Update()
		{
			PlayFootsteps(footstepsClips, footstepMinimumDistance, footstepRandomVolumeDelta, FirstPersonController.ControllerState.Standing);
			PlayFootsteps(footstepsClips, footstepMinimumDistance, footstepRandomVolumeDelta, FirstPersonController.ControllerState.TacticalSprint);
			PlayFootsteps(footstepsClips, footstepMinimumDistance, footstepRandomVolumeDelta, FirstPersonController.ControllerState.Crouched);
			PlayFootsteps(proneFootstepsClips, proneFootstepMinimumDistance, proneFootstepRandomVolumeDelta, FirstPersonController.ControllerState.Proned);
		}

		private void PlayFootsteps(AudioClip[] clips, float stepMinimumDistance, float randomVolumeDelta, FirstPersonController.ControllerState state)
		{
			//목표 상태에 있지 않은 동안 발자국을 재생하지 않음.
			if (controller.currentControllerState != state)
			{
				return;
			}

			Vector3 vel = controller.GetVelocity();

			//속도에서 수평 속도 추출
			Vector3 horizontalVelocity = RemoveDotVector(vel, tr.up);

			float currentMovementSpeed = horizontalVelocity.magnitude;
			currentFootstepDistance += Time.deltaTime * currentMovementSpeed;

			//최소 거리에 도달함
			if (currentFootstepDistance > stepMinimumDistance)
			{
				//캐릭터가 바닥에 있고 최소 임계값보다 빠르게 움직이는 경우에만 발자국 소리를 재생함
				if (controller.IsGrounded() && currentMovementSpeed > footstepControllerSpeedMinimumThreshold)
				{
					int randomFoostepClipIndex = UnityEngine.Random.Range(0, clips.Length);
					AudioLibrary.Play2D(clips[randomFoostepClipIndex], masterVolume + masterVolume * UnityEngine.Random.Range(-randomVolumeDelta, randomVolumeDelta));
				}
				currentFootstepDistance = 0f;
			}
		}

		private void OnLand(float landDistance)
		{
			if (landDistance < 0.5f)
				return;

			//착지 오디오 클립을 재생
			AudioLibrary.Play2D(landClip, masterVolume);
		}

		private void OnJump()
		{
			//점프 오디오 클립을 재생
			AudioLibrary.Play2D(jumpClip, masterVolume);
		}

		private void OnJumpsCountIncrease(int jumpsCount)
		{
			if (jumpsCountIncreasesClips.Length < jumpsCount)
				return;

			AudioClip clip = jumpsCountIncreasesClips[jumpsCount - 1];

			if (clip != null)
			{
				AudioLibrary.Play2D(clip, masterVolume);
			}
		}

		private void OnBeginSlide()
		{
			if (beginSlideClip != null)
				AudioLibrary.Play2D(beginSlideClip, masterVolume);
		}

		private void OnEndSlide()
		{
			StopSlideSound();

			if (endSlideClip != null)
				AudioLibrary.Play2D(endSlideClip, masterVolume);

			lastTimeSlide = 0;
		}

		//FirstPersonController.OnSlide 이벤트는 캐릭터가 미끄러지는 동안 고정 업데이트마다 호출됨
		private void OnSlide()
		{
			if (IsSlideSoundCooldown() == false)
			{
				currentSlideAudioSource = AudioLibrary.Play2D(slideClip, masterVolume);
				lastTimeSlide = Time.time;
			}
		}

		private void StopSlideSound()
		{
			if (currentSlideAudioSource != null && currentSlideAudioSource.isPlaying)
				currentSlideAudioSource.Stop();

			lastTimeSlide = 0;
		}

		//'slideSoundCooldown'은 슬라이드 클립의 길이이며, 슬라이드하는 동안 루프를 통과하기 위해 수행됨
		private bool IsSlideSoundCooldown()
		{
			return Time.time < lastTimeSlide + slideSoundCooldown;
		}

		private void OnBeginGrapplingLine()
		{
			if (beginGrapplingLineClip != null)
			{
				AudioLibrary.Play2D(beginGrapplingLineClip, masterVolume);
			}
		}

		private void OnEndGrapplingLine()
		{
			if (endGrapplingLineClip != null)
			{
				AudioLibrary.Play2D(endGrapplingLineClip, masterVolume);
			}
		}

		private void OnEndFailedGrapplingLine()
		{
			if (endFailedGrapplingLineClip != null)
			{
				AudioLibrary.Play2D(endFailedGrapplingLineClip, masterVolume);
			}
		}

		private void OnBeginGrappling()
		{
			if (beginGrapplingClip != null)
			{
				AudioLibrary.Play2D(beginGrapplingClip, masterVolume);
			}
		}

		private void OnGrappling()
		{
			if (IsGrapplingSoundCooldown() == false)
			{
				currentGrapplingAudioSource = AudioLibrary.Play2D(grapplingClip, masterVolume);
				lastTimeGrappling = Time.time;
			}
		}

		//'grapplingSoundCooldown'은 슬라이드 클립의 길이이며, 그래플링하는 동안 루프를 통과하기 위해 수행됨
		private bool IsGrapplingSoundCooldown()
		{
			return Time.time < lastTimeGrappling + grapplingSoundCooldown;
		}

		private void StopGrapplingSound()
		{
			if (currentGrapplingAudioSource != null && currentGrapplingAudioSource.isPlaying)
				currentGrapplingAudioSource.Stop();

			lastTimeGrappling = 0;
		}

		private void OnEndGrappling()
		{
			StopGrapplingSound();

			if (endGrapplingClip != null)
			{
				AudioLibrary.Play2D(endGrapplingClip, masterVolume);
			}
		}

		private void OnBeginWallRun()
		{
			if (beginWallRunClip != null)
			{
				AudioLibrary.Play2D(beginWallRunClip, masterVolume);
			}
		}

		private void OnEndWallRun()
		{
			StopWallRunSound();

			if (endWallRunClip != null)
			{
				AudioLibrary.Play2D(endWallRunClip, masterVolume);
			}
		}

		private void OnWallRun()
		{
			if (IsWallRunSoundCooldown() == false)
			{
				currentWallRunAudioSource = AudioLibrary.Play2D(wallRunClip, masterVolume);
				lastTimeWallRun = Time.time;
			}
		}

		//'wallRunSoundCooldown'은 슬라이드 클립의 길이이며 벽을 달리는 동안 루프를 통과하기 위해 수행됨
		private bool IsWallRunSoundCooldown()
		{
			return Time.time < lastTimeWallRun + wallRunSoundCooldown;
		}

		private void StopWallRunSound()
		{
			if (currentWallRunAudioSource != null && currentWallRunAudioSource.isPlaying)
				currentWallRunAudioSource.Stop();

			lastTimeWallRun = 0;
		}

		//벡터에서 'dir'과 같은 방향을 가리키는 모든 부분을 제거함
		public static Vector3 RemoveDotVector(Vector3 vec, Vector3 dir)
		{
			if (dir.sqrMagnitude != 1)
				dir.Normalize();

			float amount = Vector3.Dot(vec, dir);

			vec -= dir * amount;

			return vec;
		}
		private void OnClimbBegin()
		{
			if (beginClimbClip != null)
			{
				AudioLibrary.Play2D(beginClimbClip, masterVolume);
			}
		}

		private void OnClimbEnd()
		{
			if (endClimbClip != null)
			{
				AudioLibrary.Play2D(endClimbClip, masterVolume);
			}
		}

		public void SetMasterVolume(float value)
		{
			masterVolume = value;
		}

		#endregion
	}
}

