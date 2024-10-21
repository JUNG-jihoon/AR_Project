namespace NRKernal.NRExamples
{
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using System.Collections; // 코루틴을 사용하려면 필요

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

        // 코루틴을 추적하기 위한 딕셔너리 추가
        private Dictionary<int, Coroutine> stopTrackingCoroutines = new Dictionary<int, Coroutine>();

        public void Update()
        {
            if (NRFrame.SessionStatus != SessionState.Running)
            {
                LogDebugMessage("Session not running.");
                return;
            }

            NRFrame.GetTrackables<NRTrackableImage>(m_TempTrackingImages, NRTrackableQueryFilter.All);

            // 로그 추가 - 트래킹 상태 변화를 추적
            foreach (var image in m_TempTrackingImages)
            {
                var imageIndex = image.GetDataBaseIndex();
                var trackingState = image.GetTrackingState();
                LogDebugMessage($"Image index: {imageIndex}, Tracking State: {trackingState}");

                // Tracking 상태일 때만 Visualizer를 활성화
                if (trackingState == TrackingState.Tracking)
                {
                    // 이미지가 트래킹 상태로 전환되면 기존 중단 대기 코루틴을 취소
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
                    // Stopped 상태일 때 일정 시간 대기 후 Visualizer 비활성화
                    if (!stopTrackingCoroutines.ContainsKey(imageIndex))
                    {
                        Coroutine stopCoroutine = StartCoroutine(WaitAndDeactivateVisualizer(imageIndex, 2.0f)); // 2초 대기 후 비활성화
                        stopTrackingCoroutines.Add(imageIndex, stopCoroutine);
                    }
                }
            }
        }

        /// <summary>
        /// 현재 트래킹 중인 이미지 외에 다른 이미지에서 생성된 모든 시각화 오브젝트를 삭제하거나 비활성화합니다.
        /// </summary>
        private void ClearPreviousVisualizers(int currentImageIndex)
        {
            // 기존 이미지의 시각화 오브젝트 비활성화 (이미지의 인덱스가 현재 트래킹된 이미지와 다른 경우)
            foreach (var imageIndex in new List<int>(m_Visualizers.Keys)) // 딕셔너리 키 리스트 복사 후 순회
            {
                if (imageIndex != currentImageIndex)
                {
                    m_Visualizers[imageIndex].SetActive(false);  // 오브젝트 비활성화
                    LogDebugMessage($"Deactivating visualizer for image index: {imageIndex} due to new image");
                }
            }
        }

        /// <summary>
        /// 트래킹된 이미지에 맞는 오브젝트를 생성합니다.
        /// </summary>
        /// <param name="imageIndex">트래킹된 이미지의 인덱스</param>
        /// <param name="image">NRTrackableImage 객체</param>
        private void CreateVisualizerForImage(int imageIndex, NRTrackableImage image)
        {
            GameObject prefabToInstantiate = GetPrefabForImage(imageIndex);
            LogDebugMessage($"Creating new object for image index: {imageIndex} with prefab: {prefabToInstantiate.name}");

            // 트래킹된 이미지의 위치는 그대로 사용
            Vector3 imagePosition = image.GetCenterPose().position;

            // 트래킹된 이미지의 회전 값을 가져오되, Y축을 고정 (0)으로 설정
            Quaternion imageRotation = image.GetCenterPose().rotation;
            Quaternion fixedRotation = Quaternion.Euler(0, imageRotation.eulerAngles.y, 0);  // Y축 고정된 회전 값

            // Instantiate the object at the tracked position and fixed rotation
            GameObject instantiatedObject = Instantiate(prefabToInstantiate, imagePosition, fixedRotation);

            if (instantiatedObject == null)
            {
                LogDebugMessage($"Failed to instantiate object for image index: {imageIndex}");
                return;
            }

            // SetParent 호출 시 worldPositionStays 파라미터를 false로 설정하여 월드 좌표를 유지
            instantiatedObject.transform.SetParent(transform, false);

            // Add the new object to the dictionary
            m_Visualizers.Add(imageIndex, instantiatedObject);
            LogDebugMessage($"Object created and added for image index: {imageIndex}");
        }

        /// <summary>
        /// 지정된 인덱스에 맞는 프리팹을 반환합니다.
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
        /// 일정 시간 대기 후 Visualizer를 비활성화하는 코루틴
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

        // 이미지 트래킹 키는 함수(기존 기능)
        public void EnableImageTracking()
        {
            var config = NRSessionManager.Instance.NRSessionBehaviour.SessionConfig;
            config.ImageTrackingMode = TrackableImageFindingMode.ENABLE;
            NRSessionManager.Instance.SetConfiguration(config);
        }

        // 이미지 트래킹 끄는 함수(기존 기능)
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
