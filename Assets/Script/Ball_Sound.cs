using UnityEngine;

public class BellSoundOnAngle : MonoBehaviour
{
    [SerializeField] private AudioClip bellSound; // ���Ҹ� ����� Ŭ��
    private AudioSource audioSource;

    [SerializeField] private float maxAngle = 30f; // �ִ� ����
    [SerializeField] private float minAngle = -30f; // �ּ� ����
    private float lastPlayTime = 0f;
    private float playInterval = 2f; // �ּ� ��� ���� (��)


    private void Start()
    {
        // AudioSource �ʱ�ȭ
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        // ���� ������Ʈ�� ���� X�� ȸ�� ���� ��������
        float currentAngle = transform.localRotation.eulerAngles.x;

        // ������ -180 ~ 180 ������ ��ȯ (Unity�� �⺻ ������ 0 ~ 360 ����)
        if (currentAngle > 180)
            currentAngle -= 360;

        if ((currentAngle >= maxAngle || currentAngle <= minAngle) && Time.time - lastPlayTime > playInterval)
        {
            audioSource.PlayOneShot(bellSound);
            lastPlayTime = Time.time;
        }
    }
}
