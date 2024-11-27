using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piling_Script : MonoBehaviour
{
    [SerializeField] private GameObject piles;
    [SerializeField] private Material ground;
    [SerializeField] private GameObject cube;
    private float animationDuration = 3.0f;
    private float fadeDuration = 2.0f;

    private GameObject[] pile_ground;

    private void OnEnable()
    {
        pile_ground = new GameObject[piles.transform.childCount];
        for (int i = 0; i < piles.transform.childCount; i++)
        {
            pile_ground[i] = piles.transform.GetChild(i).gameObject;
        }

        Debug.Log(pile_ground.Length);
        Color startColor = ground.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f);
        ground.color = targetColor;

        StartCoroutine(Move_OneLine());
    }

    private IEnumerator Move_OneLine()
    {
        int numPerLine = 18;
        float moveAmount = -10f;
        float animationDuration = 3.0f;
        float delayBetweenLines = 1.0f;

        // 모든 오브젝트 비활성화
        for (int i = 0; i < pile_ground.Length; i++)
        {
            pile_ground[i].SetActive(false);
        }
        yield return new WaitForSeconds(3.0f);
        // 한 줄씩 이동
        for (int i = 0; i < pile_ground.Length; i += numPerLine)
        {
            for (int j = 0; j < numPerLine && i + j < pile_ground.Length; j++)
            {
                GameObject obj = pile_ground[i + j];
                obj.SetActive(true);

                // 한 프레임 대기 (렌더링 초기화)
                yield return null;

                Vector3 startPos = obj.transform.position;
                Vector3 targetPos = new Vector3(startPos.x, startPos.y + moveAmount, startPos.z);

                StartCoroutine(MoveObject(obj, startPos, targetPos, animationDuration));
            }

            yield return new WaitForSeconds(delayBetweenLines);
        }

        yield return new WaitForSeconds(2.0f);
        StartCoroutine(FadeGroundAlpha());

        yield return new WaitForSeconds(2.0f);
        Debug.Log("큐브");
        cube.SetActive(true);

        // 큐브 Y값 -23 이동
        StartCoroutine(MoveCubeDown(cube, -23f, 2.0f)); // 2초 동안 -23만큼 이동
    }

    private IEnumerator MoveObject(GameObject obj, Vector3 startPos, Vector3 targetPos, float duration)
    {
        float halfwayDuration = duration / 2;
        float elapsedTime = 0f;

        Vector3 halfwayPos = new Vector3(startPos.x, startPos.y - 3f, startPos.z);
        while (elapsedTime < halfwayDuration)
        {
            float t = elapsedTime / halfwayDuration;
            obj.transform.position = Vector3.Lerp(startPos, halfwayPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        obj.transform.position = halfwayPos;

        elapsedTime = 0f;
        while (elapsedTime < halfwayDuration)
        {
            float t = elapsedTime / halfwayDuration;
            obj.transform.position = Vector3.Lerp(halfwayPos, targetPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        obj.transform.position = targetPos;
    }

    private IEnumerator MoveCubeDown(GameObject cube, float moveAmount, float duration)
    {
        if (cube == null) yield break;

        Vector3 startPos = cube.transform.position;
        Vector3 targetPos = new Vector3(startPos.x, startPos.y + moveAmount, startPos.z);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            cube.transform.position = Vector3.Lerp(startPos, targetPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cube.transform.position = targetPos; // 최종 위치 설정
    }

    private IEnumerator FadeGroundAlpha()
    {
        Color startColor = ground.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0.40f);
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            Color newColor = Color.Lerp(startColor, targetColor, elapsedTime / fadeDuration);
            ground.color = newColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
