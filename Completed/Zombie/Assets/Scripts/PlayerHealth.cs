﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 플레이어의 체력을 담당
// 체력과 데미지, 사망에 관한 기본 구현은 상속한 LivingEntity에 구현되어 있다
// LivingEntity를 확장하여 플레이어로서 필요한 체력 관련 처리를 추가한다
// 이외에도 아이템을 먹는 처리를 여기서 한다
public class PlayerHealth : LivingEntity {
    public Slider healthSlider; // 체력을 표시할 UI 슬라이더

    private AudioSource playerAudioPlayer; // 플레이어 소리 재생기
    public AudioClip deathClip; // 사망 소리
    public AudioClip hitClip; // 피격 소리
    public AudioClip itemPickupClip; // 아이템 습득 소리

    private Animator playerAnimator; // 플레이어의 애니메이터
    public PlayerMovement playerMovement; // 플레이어의 PlayerMovement 컴포넌트
    public PlayerShooter playerShooter; // 플레이어의 PlayerShooter 컴포넌트

    private void Start () {
        // 초기 설정을 한다

        // 사용할 컴포넌트들의 참조를 가져온다
        playerAnimator = GetComponent<Animator> ();
        playerAudioPlayer = GetComponent<AudioSource> ();

        // 체력 슬라이더의 값을 현재 체력값으로 갱신한다
        healthSlider.value = health;
    }

    public void RestoreHealth (float newHealth) {
        // 체력을 회복하는 처리가 온다

        health += newHealth; // 체력을 추가한다
        healthSlider.value = health; // 갱신된 체력으로 체력 슬라이더를 갱신한다
    }

    public override void OnDamage (float damage, Vector3 hitPoint, Vector3 hitDirection) {
        // 데미지를 입었을때의 처리
        playerAudioPlayer.PlayOneShot (hitClip); // 피격 사운드 재생
        FlashEffect.instance.Flash (); // 화면 깜박임 효과 사용

        base.OnDamage (damage, hitPoint, hitDirection); // 부모 클래스의 OnDamage 메서드를 실행하여 데미지를 적용

        healthSlider.value = health; // 갱신된 체력을 체력 슬라이더에 반영
    }

    public override void Die () {
        // 죽었을때의 처리
        playerAudioPlayer.PlayOneShot (deathClip); // 사망음 재생
        playerAnimator.SetTrigger ("Die"); // 애니메이터의 Die 트리거를 발동시켜 사망 애니메이션 재생

        playerMovement.enabled = false; // 자신의 PlayerMovement 컴포넌트를 비활성화
        playerShooter.enabled = false; // 자신의 PlayerShooter 컴포넌트를 비활성화

        GameManager.instance.Gameover (); // 게임 오버 처리 실행

        base.Die (); // 부모 클래스의 Die 메서드를 실행하여 사망 적용
    }

    private void OnTriggerEnter (Collider other) {
        // 아이템과 충돌한 경우 아이템을 사용하는 처리를 한다

        if (!dead) { // 사망하지 않은 경우에만 아이템 사용가능
            Item item = other.GetComponent<Item> (); // 충돌한 상대방으로 부터 Item 컴포넌트를 가져오기 시도

            if (item != null) { // 충돌한 상대방으로부터 Item 컴포넌트가 가져오는데 성공했다면
                playerAudioPlayer.PlayOneShot (itemPickupClip); // 아이템 습득 소리 재생
                item.Use (gameObject); // 아이템 사용. Use 메서드에 자신의 게임 오브젝트를 넘겨준다
            }
        }
    }
}