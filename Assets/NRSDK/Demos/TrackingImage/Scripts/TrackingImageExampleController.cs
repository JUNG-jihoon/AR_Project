/****************************************************************************
* Copyright 2019 Xreal Technology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.xreal.com/        
* 
*****************************************************************************/

namespace NRKernal.NRExamples
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary> Controller for TrackingImage example. </summary>
    [HelpURL("https://developer.xreal.com/develop/unity/image-tracking")]
    public class TrackingImageExampleController : MonoBehaviour
    {
        /// <summary> A prefab for visualizing an TrackingImage. </summary>
        public TrackingImageVisualizer TrackingImageVisualizerPrefab;

        /// <summary> The overlay containing the fit to scan user guide. </summary>
        public GameObject FitToScanOverlay;

        /// <summary> The visualizers. </summary>
        private Dictionary<int, TrackingImageVisualizer> m_Visualizers
            = new Dictionary<int, TrackingImageVisualizer>();

        /// <summary> The temporary tracking images. </summary>
        private List<NRTrackableImage> m_TempTrackingImages = new List<NRTrackableImage>();

        /// <summary> Updates this object. </summary>
        public void Update()
        {
#if !UNITY_EDITOR
            // Check that motion tracking is tracking.
            if (NRFrame.SessionStatus != SessionState.Running)
            {
                return;
            }
#endif
            // Get updated augmented images for this frame.
            NRFrame.GetTrackables<NRTrackableImage>(m_TempTrackingImages, NRTrackableQueryFilter.New);

            // Create visualizers and anchors for updated augmented images that are tracking and do not previously
            // have a visualizer. Remove visualizers for stopped images.
            foreach (var image in m_TempTrackingImages)
            {
                // 기존에 생성된 모든 visualizer를 제거
                ClearExistingVisualizers();

                TrackingImageVisualizer visualizer = null;
                m_Visualizers.TryGetValue(image.GetDataBaseIndex(), out visualizer);
                if (image.GetTrackingState() != TrackingState.Stopped && visualizer == null)
                {
                    NRDebugger.Info("Create new TrackingImageVisualizer!");
                    // 오브젝트를 생성할 때 회전 값을 보정하여 올바르게 보이도록 조정합니다.
                    visualizer = (TrackingImageVisualizer)Instantiate(
                        TrackingImageVisualizerPrefab,
                        image.GetCenterPose().position,
                        image.GetCenterPose().rotation * Quaternion.Euler(0, 0, 0) // 필요에 따라 회전값을 조정합니다.
                    );

                    visualizer.Image = image;
                    visualizer.transform.parent = transform;
                    m_Visualizers.Add(image.GetDataBaseIndex(), visualizer);
                }
                else if (image.GetTrackingState() == TrackingState.Stopped && visualizer != null)
                {
                    m_Visualizers.Remove(image.GetDataBaseIndex());
                    Destroy(visualizer.gameObject);
                }

                FitToScanOverlay.SetActive(false);
            }
        }

        /// <summary>
        /// 기존에 생성된 모든 visualizer를 제거하는 함수.
        /// </summary>
        private void ClearExistingVisualizers()
        {
            foreach (var visualizer in m_Visualizers.Values)
            {
                Destroy(visualizer.gameObject);
            }
            m_Visualizers.Clear();
        }

        /// <summary> Enables the image tracking. </summary>
        public void EnableImageTracking()
        {
            var config = NRSessionManager.Instance.NRSessionBehaviour.SessionConfig;
            config.ImageTrackingMode = TrackableImageFindingMode.ENABLE;
            NRSessionManager.Instance.SetConfiguration(config);
        }

        /// <summary> Disables the image tracking. </summary>
        public void DisableImageTracking()
        {
            var config = NRSessionManager.Instance.NRSessionBehaviour.SessionConfig;
            config.ImageTrackingMode = TrackableImageFindingMode.DISABLE;
            NRSessionManager.Instance.SetConfiguration(config);
        }
    }
}
