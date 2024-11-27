using System.Collections;
using UnityEngine;

public class Ceiling_Animation : MonoBehaviour
{
    [SerializeField] private GameObject[] Cloner; // ������Ʈ �迭
    [SerializeField] private float moveDuration = 4f; // �� ������Ʈ�� �������� �ð�
    [SerializeField] private float delayAfterStart = 1.5f; // �������� ���� �� ���� ������Ʈ���� ��� �ð�

    [SerializeField] private Renderer sharedMaterialRenderer; // ���� ��Ƽ������ ����ϴ� ������ �� �ϳ�
    [SerializeField] private Color highlightColor = Color.yellow; // ���� ����
    [SerializeField] private float blinkSpeed = 1f; // �����̴� �ӵ�
    [SerializeField] private int blinkCount = 10; // ������ ������Ʈ �����̴� Ƚ��

    [SerializeField] private Material cell_1;
    [SerializeField] private Material cell_2;

    private Material sharedMaterial; // ���� ��Ƽ����
    private Vector3[] originalPositions; // ������Ʈ�� ���� ��ġ ����

    private void Start()
    {
        if (sharedMaterialRenderer == null)
        {
            Debug.LogError("���� ��Ƽ������ ����ϴ� Renderer�� �����ؾ� �մϴ�.");
            return;
        }

        // ���� ��Ƽ���� ��������
        sharedMaterial = sharedMaterialRenderer.sharedMaterial;

        // �ʱ� ���¿��� Emission ��Ȱ��ȭ
        sharedMaterial.SetColor("_EmissionColor", Color.black);

        // ������Ʈ ���� ��ġ ����
        originalPositions = new Vector3[Cloner.Length];
        for (int i = 0; i < Cloner.Length; i++)
        {
            originalPositions[i] = Cloner[i].transform.position;
        }

        StartCoroutine(MoveCloners());
    }

    private void OnEnable()
    {
        // cell_1�� cell_2 �ʱ� ���� ���� (ȸ��, ���� 100%)
        SetInitialMaterialColor(cell_1, new Color(128 / 255f, 128 / 255f, 128 / 255f, 1f));
        SetInitialMaterialColor(cell_2, new Color(128 / 255f, 128 / 255f, 128 / 255f, 1f));
    }
    private void SetInitialMaterialColor(Material material, Color color)
    {
        if (material != null)
        {
            material.color = color;
        }
    }
    private IEnumerator MoveCloners()
    {
        for (int i = 0; i < Cloner.Length; i++)
        {
            // ���� ������Ʈ�� �̵�
            StartCoroutine(MoveObject(Cloner[i], -40f));

            // ������ ������Ʈ�� �ƴ϶�� ��� �ð� ����
            if (i < Cloner.Length - 1)
            {
                yield return new WaitForSeconds(delayAfterStart);
            }
            else
            {
                // ������ ������Ʈ�� �̵� �� ������ �߰�
                yield return new WaitForSeconds(moveDuration);
                yield return BlinkFinalObject();

                // ������ ���� 2�� ���
                yield return new WaitForSeconds(2f);

                // ��� ������Ʈ�� ���� ��ġ�� ����
                yield return ResetPositions();

                // ���� �� 1�� ��� �� cell_1�� cell_2�� ���� 0���� ����
                yield return new WaitForSeconds(1f);
                StartCoroutine(FadeOutMaterial(cell_1, 1f)); // 1�� ���� ������ ���� 0����
                StartCoroutine(FadeOutMaterial(cell_2, 1f));
            }
        }
    }

    private IEnumerator MoveObject(GameObject obj, float yOffset)
    {
        Vector3 startPosition = obj.transform.position;
        Vector3 targetPosition = startPosition + new Vector3(0, yOffset, 0);

        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            obj.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        // �̵� �Ϸ� �� ��Ȯ�� ��ġ ����
        obj.transform.position = targetPosition;
    }

    private IEnumerator BlinkFinalObject()
    {
        // ������ ����
        float intensityMultiplier = 5f; // Emission ���� ����
        for (int i = 0; i < blinkCount; i++)
        {
            // Emission Color ����: ������ �߰������� ���� �� ��� ǥ��
            Color finalColor = highlightColor * Mathf.LinearToGammaSpace(intensityMultiplier);
            sharedMaterial.SetColor("_EmissionColor", finalColor);

            yield return new WaitForSeconds(blinkSpeed / 2); // Emission ���� ����

            // Emission ��Ȱ��ȭ
            sharedMaterial.SetColor("_EmissionColor", Color.black);
            yield return new WaitForSeconds(blinkSpeed / 2); // Emission ���� ����
        }

        // ������ ���� �� Emission �ʱ�ȭ
        sharedMaterial.SetColor("_EmissionColor", Color.black);
    }

    private IEnumerator ResetPositions()
    {
        // �� ������Ʈ�� ���� ��ġ�� ����
        for (int i = 0; i < Cloner.Length; i++)
        {
            StartCoroutine(MoveObjectToPosition(Cloner[i], originalPositions[i]));
        }

        // ��� ������Ʈ ���� �Ϸ���� ���
        yield return new WaitForSeconds(moveDuration);
    }

    private IEnumerator MoveObjectToPosition(GameObject obj, Vector3 targetPosition)
    {
        Vector3 startPosition = obj.transform.position;

        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            obj.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        // ���� �Ϸ� �� ��Ȯ�� ��ġ ����
        obj.transform.position = targetPosition;
    }

    private IEnumerator FadeOutMaterial(Material material, float duration)
    {
        if (material == null) yield break;

        Color startColor = material.color;
        float startAlpha = startColor.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            startColor.a = Mathf.Lerp(startAlpha, 0f, t); // Alpha ���� 0���� ���� ����
            material.color = startColor;
            yield return null;
        }

        // ���������� Alpha�� 0���� ����
        startColor.a = 0f;
        material.color = startColor;
    }
}
