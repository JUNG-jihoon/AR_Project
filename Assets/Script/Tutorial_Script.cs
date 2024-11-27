using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial_Script : MonoBehaviour
{
    [SerializeField] private Sprite[] Tutorial_Image; // 보여줄 스프라이트 배열
    [SerializeField] private Image displayImage; // UI 이미지에 연결

    private int currentIndex = 0; // 현재 보여주는 이미지의 인덱스
    private GameObject overlayChild; // Overlay 오브젝트의 첫 번째 자식

    // Start is called before the first frame update
    void Start()
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

        if (Tutorial_Image.Length > 0 && displayImage != null)
        {
            StartCoroutine(ShowTutorialImages());
        }
        else
        {
            Debug.LogError("Tutorial_Image 배열이 비어있거나 displayImage가 연결되지 않았습니다!");
        }
    }

    IEnumerator ShowTutorialImages()
    {
        while (currentIndex < Tutorial_Image.Length)
        {
            // 현재 이미지를 표시
            displayImage.sprite = Tutorial_Image[currentIndex];

            // 10초 동안 대기
            yield return new WaitForSeconds(10f);

            // 다음 이미지로 이동
            currentIndex++;
        }

        // 모든 이미지를 다 본 후 첫 번째 자식 오브젝트 다시 활성화
        gameObject.SetActive(false);
        if (overlayChild != null)
        {
            overlayChild.SetActive(true);
        }

        Debug.Log("튜토리얼 이미지가 끝났습니다.");
    }
}
