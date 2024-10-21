namespace NRKernal.NRExamples
{
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using System.Collections; // �ڷ�ƾ�� ����Ϸ��� �ʿ�

    /// <summary> Controller for TrackingImage example. </summary>
    [HelpURL("https://developer.xreal.com/develop/unity/image-tracking")]
    public class ImageTracking_test : MonoBehaviour
    {
        public GameObject TrackingImageVisualizerPrefab;
        public GameObject FitToScanOverlay;
        public GameObject PrefabForImage1;
        public GameObject PrefabForImage2;
        public TextMeshProUGUI debugText;

        // Dictionary to keep track of instantiated objects by their image index
        private Dictionary<int, GameObject> m_Visualizers = new Dictionary<int, GameObject>();
        private List<NRTrackableImage> m_TempTrackingImages = new List<NRTrackableImage>();

        // �ڷ�ƾ�� �����ϱ� ���� ��ųʸ� �߰�
        private Dictionary<int, Coroutine> stopTrackingCoroutines = new Dictionary<int, Coroutine>();

        public void Update()
        {
            if (NRFrame.SessionStatus != SessionState.Running)
            {
                LogDebugMessage("Session not running.");
                return;
            }

            NRFrame.GetTrackables<NRTrackableImage>(m_TempTrackingImages, NRTrackableQueryFilter.All);

            // �α� �߰� - Ʈ��ŷ ���� ��ȭ�� ����
            foreach (var image in m_TempTrackingImages)
            {
                var imageIndex = image.GetDataBaseIndex();
                var trackingState = image.GetTrackingState();
                LogDebugMessage($"Image index: {imageIndex}, Tracking State: {trackingState}");

                // Tracking ������ ���� Visualizer�� Ȱ��ȭ
                if (trackingState == TrackingState.Tracking)
                {
                    // �̹����� Ʈ��ŷ ���·� ��ȯ�Ǹ� ���� �ߴ� ��� �ڷ�ƾ�� ���
                    if (stopTrackingCoroutines.ContainsKey(imageIndex))
                    {
                        StopCoroutine(stopTrackingCoroutines[imageIndex]);
                        stopTrackingCoroutines.Remove(imageIndex);
                    }

                    ClearPreviousVisualizers(imageIndex);

                    if (!m_Visualizers.ContainsKey(imageIndex))
                    {
                        CreateVisualizerForImage(imageIndex, image);
                    }
                    else
                    {
                        m_Visualizers[imageIndex].SetActive(true);
                        LogDebugMessage($"Reactivating visualizer for image index: {imageIndex}");
                    }

                    FitToScanOverlay.SetActive(false);
                }
                else if (trackingState == TrackingState.Stopped)
                {
                    // Stopped ������ �� ���� �ð� ��� �� Visualizer ��Ȱ��ȭ
                    if (!stopTrackingCoroutines.ContainsKey(imageIndex))
                    {
                        Coroutine stopCoroutine = StartCoroutine(WaitAndDeactivateVisualizer(imageIndex, 2.0f)); // 2�� ��� �� ��Ȱ��ȭ
                        stopTrackingCoroutines.Add(imageIndex, stopCoroutine);
                    }
                }
            }
        }

        /// <summary>
        /// ���� Ʈ��ŷ ���� �̹��� �ܿ� �ٸ� �̹������� ������ ��� �ð�ȭ ������Ʈ�� �����ϰų� ��Ȱ��ȭ�մϴ�.
        /// </summary>
        private void ClearPreviousVisualizers(int currentImageIndex)
        {
            // ���� �̹����� �ð�ȭ ������Ʈ ��Ȱ��ȭ (�̹����� �ε����� ���� Ʈ��ŷ�� �̹����� �ٸ� ���)
            foreach (var imageIndex in new List<int>(m_Visualizers.Keys)) // ��ųʸ� Ű ����Ʈ ���� �� ��ȸ
            {
                if (imageIndex != currentImageIndex)
                {
                    m_Visualizers[imageIndex].SetActive(false);  // ������Ʈ ��Ȱ��ȭ
                    LogDebugMessage($"Deactivating visualizer for image index: {imageIndex} due to new image");
                }
            }
        }

        /// <summary>
        /// Ʈ��ŷ�� �̹����� �´� ������Ʈ�� �����մϴ�.
        /// </summary>
        /// <param name="imageIndex">Ʈ��ŷ�� �̹����� �ε���</param>
        /// <param name="image">NRTrackableImage ��ü</param>
        private void CreateVisualizerForImage(int imageIndex, NRTrackableImage image)
        {
            GameObject prefabToInstantiate = GetPrefabForImage(imageIndex);
            LogDebugMessage($"Creating new object for image index: {imageIndex} with prefab: {prefabToInstantiate.name}");

            // Ʈ��ŷ�� �̹����� ��ġ�� �״�� ���
            Vector3 imagePosition = image.GetCenterPose().position;

            // Ʈ��ŷ�� �̹����� ȸ�� ���� ��������, Y���� ���� (0)���� ����
            Quaternion imageRotation = image.GetCenterPose().rotation;
            Quaternion fixedRotation = Quaternion.Euler(0, imageRotation.eulerAngles.y, 0);  // Y�� ������ ȸ�� ��

            // Instantiate the object at the tracked position and fixed rotation
            GameObject instantiatedObject = Instantiate(prefabToInstantiate, imagePosition, fixedRotation);

            if (instantiatedObject == null)
            {
                LogDebugMessage($"Failed to instantiate object for image index: {imageIndex}");
                return;
            }

            // SetParent ȣ�� �� worldPositionStays �Ķ���͸� false�� �����Ͽ� ���� ��ǥ�� ����
            instantiatedObject.transform.SetParent(transform, false);

            // Add the new object to the dictionary
            m_Visualizers.Add(imageIndex, instantiatedObject);
            LogDebugMessage($"Object created and added for image index: {imageIndex}");
        }

        /// <summary>
        /// ������ �ε����� �´� �������� ��ȯ�մϴ�.
        /// </summary>
        private GameObject GetPrefabForImage(int imageIndex)
        {
            switch (imageIndex)
            {
                case 0:
                    return PrefabForImage1;
                case 1:
                    return PrefabForImage2;
                default:
                    return TrackingImageVisualizerPrefab;
            }
        }

        /// <summary>
        /// ���� �ð� ��� �� Visualizer�� ��Ȱ��ȭ�ϴ� �ڷ�ƾ
        /// </summary>
        private IEnumerator WaitAndDeactivateVisualizer(int imageIndex, float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            if (m_Visualizers.ContainsKey(imageIndex))
            {
                m_Visualizers[imageIndex].SetActive(false);
                LogDebugMessage($"Deactivating visualizer for image index: {imageIndex} after waiting for {waitTime} seconds");
            }

            stopTrackingCoroutines.Remove(imageIndex);
        }

        // �̹��� Ʈ��ŷ Ű�� �Լ�(���� ���)
        public void EnableImageTracking()
        {
            var config = NRSessionManager.Instance.NRSessionBehaviour.SessionConfig;
            config.ImageTrackingMode = TrackableImageFindingMode.ENABLE;
            NRSessionManager.Instance.SetConfiguration(config);
        }

        // �̹��� Ʈ��ŷ ���� �Լ�(���� ���)
        public void DisableImageTracking()
        {
            var config = NRSessionManager.Instance.NRSessionBehaviour.SessionConfig;
            config.ImageTrackingMode = TrackableImageFindingMode.DISABLE;
            NRSessionManager.Instance.SetConfiguration(config);
        }

        /// <summary>
        /// Logs debug messages to the TextMeshPro UI element.
        /// </summary>
        /// <param name="message">The message to display.</param>
        private void LogDebugMessage(string message)
        {
            if (debugText != null)
            {
                debugText.text += message + "\n"; // Append new message to the existing text
            }
        }
    }
}
