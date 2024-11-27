using System.Collections;
using UnityEngine;

public class Ball_Moving : MonoBehaviour
{
    [SerializeField] private GameObject Ball;
    [SerializeField] private GameObject Null;

    [SerializeField] private float swingAngle = 60f; // �ִ� ����
    [SerializeField] private float swingSpeed = 1f;  // ���� �ӵ�

    private void Start()
    {
        StartCoroutine(PendulumMotion());
    }

    private IEnumerator PendulumMotion()
    {
        while (true)
        {
            float time = 0f;

            while (true)
            {
                time += Time.deltaTime * swingSpeed;

                // ���� � ����: angle = maxAngle * sin(time * speed)
                float angle = swingAngle * Mathf.Sin(time);

                // �θ��� ȸ�� ���� �����ϰ� ȸ�� ����
                Quaternion parentRotation = Ball.transform.parent.rotation;
                Quaternion localRotation = Quaternion.Euler(angle, 0, 0);

                Ball.transform.rotation = parentRotation * localRotation;
                Null.transform.rotation = parentRotation * localRotation;

                yield return null;
            }
        }
    }
}
    