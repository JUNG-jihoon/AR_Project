using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial_Script : MonoBehaviour
{
    [SerializeField] private Sprite[] Tutorial_Image; // ������ ��������Ʈ �迭
    [SerializeField] private Image displayImage; // UI �̹����� ����

    private int currentIndex = 0; // ���� �����ִ� �̹����� �ε���
    private GameObject overlayChild; // Overlay ������Ʈ�� ù ��° �ڽ�

    // Start is called before the first frame update
    void Start()
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

        if (Tutorial_Image.Length > 0 && displayImage != null)
        {
            StartCoroutine(ShowTutorialImages());
        }
        else
        {
            Debug.LogError("Tutorial_Image �迭�� ����ְų� displayImage�� ������� �ʾҽ��ϴ�!");
        }
    }

    IEnumerator ShowTutorialImages()
    {
        while (currentIndex < Tutorial_Image.Length)
        {
            // ���� �̹����� ǥ��
            displayImage.sprite = Tutorial_Image[currentIndex];

            // 10�� ���� ���
            yield return new WaitForSeconds(10f);

            // ���� �̹����� �̵�
            currentIndex++;
        }

        // ��� �̹����� �� �� �� ù ��° �ڽ� ������Ʈ �ٽ� Ȱ��ȭ
        gameObject.SetActive(false);
        if (overlayChild != null)
        {
            overlayChild.SetActive(true);
        }

        Debug.Log("Ʃ�丮�� �̹����� �������ϴ�.");
    }
}
