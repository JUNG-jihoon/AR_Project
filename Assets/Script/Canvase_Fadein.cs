using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Canvas_Fadein : MonoBehaviour
{
    private AudioSource m_AudioSource;//����� ���� ������Ʈ

    private GameObject overlayChild; // Overlay ������Ʈ�� ù ��° �ڽ�

    private void OnEnable()
    {
        // "Overlay" �±׸� ���� ������Ʈ ã��
        GameObject overlayObject = GameObject.FindGameObjectWithTag("Overlay");
        if (overlayObject != null && overlayObject.transform.childCount > 0)
        {
            // ù ��° �ڽ� ������Ʈ�� ��������
            overlayChild = overlayObject.transform.GetChild(0).gameObject;

            // ù ��° �ڽ� ������Ʈ ��Ȱ��ȭ
            overlayChild.SetActive(false);
        }
        else
        {
            Debug.LogError("Overlay �±׸� ���� ������Ʈ�� �ڽ� ������Ʈ�� �����ϴ�!");
        }

        // ù ��° �ڽ�: Background �̹���
        Image background = transform.GetChild(0).GetComponent<Image>();
        // �� ��° �ڽ�: Background �̹���
        Image background_2 = transform.GetChild(1).GetComponent<Image>();
        // �� ��° �ڽ�: Text �̹���
        Image text = transform.GetChild(2).GetComponent<Image>();
        // ����� �ҽ� ��������
        m_AudioSource = GetComponent<AudioSource>();

        // �ʱ� ���� ���� (���� ���� ����, ������ 0����)
        SetInitialAlpha(background, 0f);
        SetInitialAlpha(background_2, 0f);
        SetInitialAlpha(text, 0f);

        // ���� ���� �� ���� ��� ����
        StartCoroutine(FadeInAndPlaySound(background, background_2, text, 4f));

    }

    private void SetInitialAlpha(Image image, float alpha)
    {
        if (image != null)
        {
            Color color = image.color;
            color.a = alpha; // ������ ����
            image.color = color;
        }
    }

    private IEnumerator FadeInAndPlaySound(Image background, Image background_2, Image text, float duration)
    {
        // ���ÿ� ������ ����
        Coroutine fadeBackground = StartCoroutine(FadeToTargetAlpha(background, 0.4f, duration));
        Coroutine fadeBackground2 = StartCoroutine(FadeToTargetAlpha(background_2, 0.4f, duration));
        Coroutine fadeText = StartCoroutine(FadeToTargetAlpha(text, 1.0f, duration));

        // ��� ���̵尡 �Ϸ�� ������ ���
        yield return fadeBackground;
        yield return fadeBackground2;
        yield return fadeText;

        // ���� ��� ����
        if (m_AudioSource != null)
        {
            m_AudioSource.Play();
            while (m_AudioSource.isPlaying)
            {
                yield return null; // ���� ��� �� ���
            }
        }

        // ���� ����� ���� �� ������Ʈ ��Ȱ��ȭ
        gameObject.SetActive(false);

        //�������� Ȱ��ȭ

        if (overlayChild != null)
        {
            overlayChild.SetActive(true);
        }

    }

    private IEnumerator FadeToTargetAlpha(Image image, float targetAlpha, float duration)
    {
        if (image == null) yield break;

        // ���� Alpha �� ����
        Color color = image.color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        // Alpha ���� ���������� ����
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration); // ���� ����
            image.color = color;
            yield return null;
        }

        // ���� Alpha �� ����
        color.a = targetAlpha;
        image.color = color;
    }
}
