namespace NRKernal.NRExamples
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    /// <summary> Controller for TrackingImage example. </summary>
    [HelpURL("https://developer.xreal.com/develop/unity/image-tracking")]
    public class ImageTracking_ING : MonoBehaviour
    {

        public GameObject FitToScanOverlay;
        public GameObject PrefabForImage1;
        public GameObject PrefabForImage2;
        public GameObject PrefabForImage3;
        public GameObject PrefabForImage4;
        public GameObject PrefabForImage5;
        public GameObject PrefabForImage6;
        public GameObject PrefabForImage7;
        public GameObject PrefabFordefault;
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

            // Clear all visualizers only if there are no tracked images detected
            if (m_TempTrackingImages.Count > 0)
            {
                ClearAllVisualizers();

            }

            foreach (var image in m_TempTrackingImages)
            {
                var imageIndex = image.GetDataBaseIndex();
                var trackingState = image.GetTrackingState();
                LogDebugMessage($"Processing image index: {imageIndex}, Tracking State: {trackingState}");

                // Create a new visualizer only if it doesn't exist yet
                if (!m_Visualizers.ContainsKey(imageIndex))
                {
                    GameObject prefabToInstantiate = GetPrefabForImage(imageIndex);
                    Pose imagePose = image.GetCenterPose();
                    Vector3 imagePosition = imagePose.position;

                    // Initial rotation (using Y-axis rotation only)
                    Quaternion yOnlyRotation = Quaternion.Euler(0, imagePose.rotation.eulerAngles.y, 0);

                    // Instantiate and add to dictionary
                    GameObject instantiatedObject = Instantiate(prefabToInstantiate, imagePosition, yOnlyRotation);
                    instantiatedObject.transform.SetParent(transform, true);
                    m_Visualizers.Add(imageIndex, instantiatedObject);
                    LogDebugMessage($"Object created and added for image index: {imageIndex}");
                    FitToScanOverlay.SetActive(false);
                }

                // Update rotation to match the camera's Y-axis rotation
                if (m_Visualizers.TryGetValue(imageIndex, out GameObject visualizerObject))
                {
                    Quaternion cameraRotation = Camera.main.transform.rotation;
                    float cameraYRotation = cameraRotation.eulerAngles.y;
                    Quaternion newRotation = Quaternion.Euler(0, cameraYRotation, 0);
                    visualizerObject.transform.rotation = newRotation;
                    LogDebugMessage($"Updated rotation for image index: {imageIndex} to match camera's Y rotation: {cameraYRotation}");
                }
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
                case 2:
                    return PrefabForImage3;
                case 3:
                    return PrefabForImage4;
                case 4:
                    return PrefabForImage5;
                case 5:
                    return PrefabForImage6;
                case 6:
                    return PrefabForImage7;
                default:
                    return PrefabFordefault;
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
        private void ClearMessage()
        {
            if (debugText != null)
            {
                debugText.text = null; // Append new message to the existing text
            }
        }


        public void ForceResetTrackingSession()
        {
            NRSessionManager.Instance.TrackableFactory.ResetTrackables();
            ClearMessage();
            LogDebugMessage("Force reset of tracking session with explicit resume and recenter.");
        }





    }
}
