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
        // ��� AudioSource ������Ʈ ��������

            water_sound = GetComponent<AudioSource>();
            thunder_sound = transform.GetChild(0).GetComponent<AudioSource>();
        


        // ��Ƽ���� ���� �ʱ�ȭ (0% ����)
        SetMaterialAlpha(0f);

        //����Ʈ ����
        effect.SetActive(false);

        // water_sound �������ڸ��� ���
        water_sound.Play();

        // thunder_sound 2�� �ڿ� ���
        StartCoroutine(PlayThunderSoundAfterDelay(10f));
    }


    private IEnumerator PlayThunderSoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 2�� ���
        thunder_sound.Play();
        yield return new WaitForSeconds(4f); // 4�� ���
        StartCoroutine(FadeMaterialAlpha(1f, 1f)); // 1�� ���� ������ 100%�� �ø�
    }

    private void SetMaterialAlpha(float alpha)
    {
        if (material != null)
        {
            Color color = material.color;
            color.a = Mathf.Clamp01(alpha); // ���� ���� 0~1�� ����
            material.color = color;
        }
        else
        {
            Debug.LogError("Material�� �������� �ʾҽ��ϴ�.");
        }
    }

    private IEnumerator FadeMaterialAlpha(float targetAlpha, float duration)
    {
        if (material == null)
        {
            Debug.LogError("Material�� �������� �ʾҽ��ϴ�.");
            yield break;
        }

        Color color = material.color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration); // ���� �������� ���� ����
            color.a = alpha;
            material.color = color;
            yield return null;
        }

        // ���������� ��Ȯ�� targetAlpha�� ����
        color.a = targetAlpha;
        material.color = color;
        effect.SetActive(true);
    }
}
