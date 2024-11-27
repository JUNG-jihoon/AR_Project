using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Canvas_Fadein : MonoBehaviour
{
    private AudioSource m_AudioSource;//재생할 음성 오브젝트

    private GameObject overlayChild; // Overlay 오브젝트의 첫 번째 자식

    private void OnEnable()
    {
        // "Overlay" 태그를 가진 오브젝트 찾기
        GameObject overlayObject = GameObject.FindGameObjectWithTag("Overlay");
        if (overlayObject != null && overlayObject.transform.childCount > 0)
        {
            // 첫 번째 자식 오브젝트를 가져오기
            overlayChild = overlayObject.transform.GetChild(0).gameObject;

            // 첫 번째 자식 오브젝트 비활성화
            overlayChild.SetActive(false);
        }
        else
        {
            Debug.LogError("Overlay 태그를 가진 오브젝트나 자식 오브젝트가 없습니다!");
        }

        // 첫 번째 자식: Background 이미지
        Image background = transform.GetChild(0).GetComponent<Image>();
        // 두 번째 자식: Background 이미지
        Image background_2 = transform.GetChild(1).GetComponent<Image>();
        // 세 번째 자식: Text 이미지
        Image text = transform.GetChild(2).GetComponent<Image>();
        // 오디오 소스 가져오기
        m_AudioSource = GetComponent<AudioSource>();

        // 초기 색상 설정 (기존 색상 유지, 투명도만 0으로)
        SetInitialAlpha(background, 0f);
        SetInitialAlpha(background_2, 0f);
        SetInitialAlpha(text, 0f);

        // 투명도 조정 및 음성 재생 시작
        StartCoroutine(FadeInAndPlaySound(background, background_2, text, 4f));

    }

    private void SetInitialAlpha(Image image, float alpha)
    {
        if (image != null)
        {
            Color color = image.color;
            color.a = alpha; // 투명도만 변경
            image.color = color;
        }
    }

    private IEnumerator FadeInAndPlaySound(Image background, Image background_2, Image text, float duration)
    {
        // 동시에 투명도를 조정
        Coroutine fadeBackground = StartCoroutine(FadeToTargetAlpha(background, 0.4f, duration));
        Coroutine fadeBackground2 = StartCoroutine(FadeToTargetAlpha(background_2, 0.4f, duration));
        Coroutine fadeText = StartCoroutine(FadeToTargetAlpha(text, 1.0f, duration));

        // 모든 페이드가 완료될 때까지 대기
        yield return fadeBackground;
        yield return fadeBackground2;
        yield return fadeText;

        // 음성 재생 시작
        if (m_AudioSource != null)
        {
            m_AudioSource.Play();
            while (m_AudioSource.isPlaying)
            {
                yield return null; // 음성 재생 중 대기
            }
        }

        // 음성 재생이 끝난 후 오브젝트 비활성화
        gameObject.SetActive(false);

        //오버레이 활성화

        if (overlayChild != null)
        {
            overlayChild.SetActive(true);
        }

    }

    private IEnumerator FadeToTargetAlpha(Image image, float targetAlpha, float duration)
    {
        if (image == null) yield break;

        // 현재 Alpha 값 저장
        Color color = image.color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        // Alpha 값을 점진적으로 변경
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration); // 선형 보간
            image.color = color;
            yield return null;
        }

        // 최종 Alpha 값 설정
        color.a = targetAlpha;
        image.color = color;
    }
}
