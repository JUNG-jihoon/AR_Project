using System.Collections;
using UnityEngine;

public class Mary_Sound : MonoBehaviour
{
    private AudioSource water_sound;
    private AudioSource thunder_sound;

    [SerializeField] private Material material;
    [SerializeField] private GameObject effect;

    // Start is called before the first frame update
    void OnEnable()
    {
        // 모든 AudioSource 컴포넌트 가져오기

            water_sound = GetComponent<AudioSource>();
            thunder_sound = transform.GetChild(0).GetComponent<AudioSource>();
        


        // 머티리얼 투명도 초기화 (0% 투명)
        SetMaterialAlpha(0f);

        //이펙트 끄기
        effect.SetActive(false);

        // water_sound 시작하자마자 재생
        water_sound.Play();

        // thunder_sound 2초 뒤에 재생
        StartCoroutine(PlayThunderSoundAfterDelay(10f));
    }


    private IEnumerator PlayThunderSoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 2초 대기
        thunder_sound.Play();
        yield return new WaitForSeconds(4f); // 4초 대기
        StartCoroutine(FadeMaterialAlpha(1f, 1f)); // 1초 동안 투명도를 100%로 올림
    }

    private void SetMaterialAlpha(float alpha)
    {
        if (material != null)
        {
            Color color = material.color;
            color.a = Mathf.Clamp01(alpha); // 알파 값은 0~1로 제한
            material.color = color;
        }
        else
        {
            Debug.LogError("Material이 설정되지 않았습니다.");
        }
    }

    private IEnumerator FadeMaterialAlpha(float targetAlpha, float duration)
    {
        if (material == null)
        {
            Debug.LogError("Material이 설정되지 않았습니다.");
            yield break;
        }

        Color color = material.color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration); // 선형 보간으로 투명도 조정
            color.a = alpha;
            material.color = color;
            yield return null;
        }

        // 최종적으로 정확히 targetAlpha로 설정
        color.a = targetAlpha;
        material.color = color;
        effect.SetActive(true);
    }
}
