using System.Collections;
using UnityEngine;

public class Ceiling_Animation : MonoBehaviour
{
    [SerializeField] private GameObject[] Cloner; // 오브젝트 배열
    [SerializeField] private float moveDuration = 4f; // 각 오브젝트가 내려오는 시간
    [SerializeField] private float delayAfterStart = 1.5f; // 내려오기 시작 후 다음 오브젝트까지 대기 시간

    [SerializeField] private Renderer sharedMaterialRenderer; // 공유 머티리얼을 사용하는 렌더러 중 하나
    [SerializeField] private Color highlightColor = Color.yellow; // 강조 색상
    [SerializeField] private float blinkSpeed = 1f; // 깜빡이는 속도
    [SerializeField] private int blinkCount = 10; // 마지막 오브젝트 깜빡이는 횟수

    [SerializeField] private Material cell_1;
    [SerializeField] private Material cell_2;

    private Material sharedMaterial; // 공유 머티리얼
    private Vector3[] originalPositions; // 오브젝트의 원래 위치 저장

    private void Start()
    {
        if (sharedMaterialRenderer == null)
        {
            Debug.LogError("공유 머티리얼을 사용하는 Renderer를 설정해야 합니다.");
            return;
        }

        // 공유 머티리얼 가져오기
        sharedMaterial = sharedMaterialRenderer.sharedMaterial;

        // 초기 상태에서 Emission 비활성화
        sharedMaterial.SetColor("_EmissionColor", Color.black);

        // 오브젝트 원래 위치 저장
        originalPositions = new Vector3[Cloner.Length];
        for (int i = 0; i < Cloner.Length; i++)
        {
            originalPositions[i] = Cloner[i].transform.position;
        }

        StartCoroutine(MoveCloners());
    }

    private void OnEnable()
    {
        // cell_1과 cell_2 초기 색상 설정 (회색, 투명도 100%)
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
            // 현재 오브젝트를 이동
            StartCoroutine(MoveObject(Cloner[i], -40f));

            // 마지막 오브젝트가 아니라면 대기 시간 적용
            if (i < Cloner.Length - 1)
            {
                yield return new WaitForSeconds(delayAfterStart);
            }
            else
            {
                // 마지막 오브젝트는 이동 후 깜빡임 추가
                yield return new WaitForSeconds(moveDuration);
                yield return BlinkFinalObject();

                // 깜빡임 이후 2초 대기
                yield return new WaitForSeconds(2f);

                // 모든 오브젝트를 원래 위치로 복귀
                yield return ResetPositions();

                // 복귀 후 1초 대기 후 cell_1과 cell_2의 투명도 0으로 설정
                yield return new WaitForSeconds(1f);
                StartCoroutine(FadeOutMaterial(cell_1, 1f)); // 1초 동안 서서히 투명도 0으로
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

        // 이동 완료 후 정확한 위치 설정
        obj.transform.position = targetPosition;
    }

    private IEnumerator BlinkFinalObject()
    {
        // 깜빡임 간격
        float intensityMultiplier = 5f; // Emission 강도 배율
        for (int i = 0; i < blinkCount; i++)
        {
            // Emission Color 변경: 강도를 추가적으로 곱해 더 밝게 표현
            Color finalColor = highlightColor * Mathf.LinearToGammaSpace(intensityMultiplier);
            sharedMaterial.SetColor("_EmissionColor", finalColor);

            yield return new WaitForSeconds(blinkSpeed / 2); // Emission 켜짐 간격

            // Emission 비활성화
            sharedMaterial.SetColor("_EmissionColor", Color.black);
            yield return new WaitForSeconds(blinkSpeed / 2); // Emission 꺼짐 간격
        }

        // 깜빡임 종료 후 Emission 초기화
        sharedMaterial.SetColor("_EmissionColor", Color.black);
    }

    private IEnumerator ResetPositions()
    {
        // 각 오브젝트를 원래 위치로 복귀
        for (int i = 0; i < Cloner.Length; i++)
        {
            StartCoroutine(MoveObjectToPosition(Cloner[i], originalPositions[i]));
        }

        // 모든 오브젝트 복귀 완료까지 대기
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

        // 복귀 완료 후 정확한 위치 설정
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
            startColor.a = Mathf.Lerp(startAlpha, 0f, t); // Alpha 값을 0으로 점차 변경
            material.color = startColor;
            yield return null;
        }

        // 최종적으로 Alpha를 0으로 설정
        startColor.a = 0f;
        material.color = startColor;
    }
}
