using System.Collections;
using UnityEngine;

public class Ball_Moving : MonoBehaviour
{
    [SerializeField] private GameObject Ball;
    [SerializeField] private GameObject Null;

    [SerializeField] private float swingAngle = 60f; // 최대 각도
    [SerializeField] private float swingSpeed = 1f;  // 진동 속도

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

                // 진자 운동 공식: angle = maxAngle * sin(time * speed)
                float angle = swingAngle * Mathf.Sin(time);

                // 부모의 회전 영향 제거하고 회전 적용
                Quaternion parentRotation = Ball.transform.parent.rotation;
                Quaternion localRotation = Quaternion.Euler(angle, 0, 0);

                Ball.transform.rotation = parentRotation * localRotation;
                Null.transform.rotation = parentRotation * localRotation;

                yield return null;
            }
        }
    }
}
    