namespace NRKernal.NRExamples
{
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;

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

        public void Update()
        {
            if (NRFrame.SessionStatus != SessionState.Running)
            {
                LogDebugMessage("Session not running.");
                return;
            }

            NRFrame.GetTrackables<NRTrackableImage>(m_TempTrackingImages, NRTrackableQueryFilter.New);

            // 새로운 이미지가 탐지되면 기존 오브젝트를 모두 삭제합니다.
            if (m_TempTrackingImages.Count > 0)
            {
                ClearAllVisualizers();
            }

            // Check the tracking state of current images and manage the visualizers accordingly
            foreach (var image in m_TempTrackingImages)
            {
                var imageIndex = image.GetDataBaseIndex();
                var trackingState = image.GetTrackingState();
                LogDebugMessage($"Processing image index: {imageIndex}, Tracking State: {trackingState}");

                // Create a new visualizer for the detected image
                GameObject prefabToInstantiate = GetPrefabForImage(imageIndex);
                LogDebugMessage($"Creating new object for image index: {imageIndex} with prefab: {prefabToInstantiate.name}");

                // Set a fixed rotation (0, 0, 0) to keep the object upright
                Quaternion fixedRotation = Quaternion.Euler(0, 0, 0); // 고정된 회전값
                Vector3 imagePosition = image.GetCenterPose().position; // 트래킹된 이미지의 위치는 그대로 사용

                // Instantiate the object    at the tracked position but with fixed rotation
                GameObject instantiatedObject = Instantiate(prefabToInstantiate, imagePosition, fixedRotation);

                if (instantiatedObject == null)
                {
                    LogDebugMessage($"Failed to instantiate object for image index: {imageIndex}");
                    continue;
                }

                instantiatedObject.transform.SetParent(transform, true); // Ensure it is parented correctly

                // Add the new object to the dictionary
                m_Visualizers.Add(imageIndex, instantiatedObject);
                LogDebugMessage($"Object created and added for image index: {imageIndex}");

                FitToScanOverlay.SetActive(false);
            }
        }


        /// <summary>
        /// 모든 시각화 오브젝트를 삭제합니다.
        /// </summary>
        private void ClearAllVisualizers()
        {
            foreach (var visualizer in m_Visualizers.Values)
            {
                if (visualizer != null)
                {
                    Destroy(visualizer);
                }
            }
            m_Visualizers.Clear();
            LogDebugMessage("All visualizers cleared.");
        }

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

        //이미지 트래킹 키는 함수(기존기능)
        public void EnableImageTracking()
        {
            var config = NRSessionManager.Instance.NRSessionBehaviour.SessionConfig;
            config.ImageTrackingMode = TrackableImageFindingMode.ENABLE;
            NRSessionManager.Instance.SetConfiguration(config);
        }

        //이미지 트래킹 키는 함수(기존기능)
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
