using UnityEngine;

public class BellSoundOnAngle : MonoBehaviour
{
    [SerializeField] private AudioClip bellSound; // 종소리 오디오 클립
    private AudioSource audioSource;

    [SerializeField] private float maxAngle = 30f; // 최대 각도
    [SerializeField] private float minAngle = -30f; // 최소 각도
    private float lastPlayTime = 0f;
    private float playInterval = 2f; // 최소 재생 간격 (초)


    private void Start()
    {
        // AudioSource 초기화
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        // 현재 오브젝트의 로컬 X축 회전 각도 가져오기
        float currentAngle = transform.localRotation.eulerAngles.x;

        // 각도를 -180 ~ 180 범위로 변환 (Unity의 기본 각도는 0 ~ 360 범위)
        if (currentAngle > 180)
            currentAngle -= 360;

        if ((currentAngle >= maxAngle || currentAngle <= minAngle) && Time.time - lastPlayTime > playInterval)
        {
            audioSource.PlayOneShot(bellSound);
            lastPlayTime = Time.time;
        }
    }
}
