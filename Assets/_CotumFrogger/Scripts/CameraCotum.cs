using UnityEngine;
using UnityEngine.UI;

using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using TensorFlowLite;
using System.IO;

namespace COTUM
{
    public class CameraCotum : MonoBehaviour
    {
        #region Private Fields

        [Tooltip("The distance in the local x-z plane to the target")]
        [SerializeField]
        private float distance = 3.0f;

        [Tooltip("The height we want the camera to be above the target")]
        [SerializeField]
        private float height = 2.0f;

        [Tooltip("Allow the camera to be offseted vertically from the target, for example giving more view of the sceneray and less ground.")]
        [SerializeField]
        private Vector3 centerOffset = Vector3.zero;

        [Tooltip("Set this as false if a component of a prefab being instanciated by Photon Network, and manually call OnStartFollowing() when and if needed.")]
        [SerializeField]
        private bool followOnStart = false;

        [Tooltip("The Smoothing for the camera to follow the target - desktop")]
        [SerializeField]
        private float SmoothSpeedDesktop = 0.5f;

        // cached transform of the target
        Transform cameraTransform;

        // maintain a flag internally to reconnect if target is lost or camera is switched
        bool isFollowing;

        // Cache for camera offset
        Vector3 cameraOffset = Vector3.zero;

        [SerializeField]
        private GameObject CanvasObject;

        // Setting from CAmera Mobile

        [SerializeField, FilePopup("*.tflite")] string palmModelFile;
        [SerializeField, FilePopup("*.tflite")] string landmarkModelFile;

        [SerializeField] RawImage cameraView = null;
        [SerializeField] bool runBackground;

        WebCamTexture webcamTexture;
        PalmDetect palmDetect;
        HandLandmarkDetect landmarkDetect;

        Vector3[] rtCorners = new Vector3[4];
        Vector3[] worldJoints = new Vector3[HandLandmarkDetect.JOINT_COUNT];
        PrimitiveDraw draw;
        List<PalmDetect.Result> palmResults;
        HandLandmarkDetect.Result landmarkResult;
        UniTask<bool> task;
        CancellationToken cancellationToken;

        enum Actions { FIST, PALM, NOTHING }
        Actions CurrentAction = Actions.NOTHING;

        Rigidbody m_Rb;

        [Tooltip("The Smoothing for the camera to follow the target - mobile")]
        [SerializeField]
        float SmoothSpeedMobile = 1.5f;


        #endregion

        #region MonoBehaviour Callbacks

        // MonoBehaviour method called on GameObject by Unity during initialization phase
        void Start()
        {
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                CanvasObject.SetActive(false);

                if (followOnStart)
                {
                    OnStartFollowing();
                }
            }
            else if (SystemInfo.deviceType == DeviceType.Handheld)
            {
            }
        }

        void OnDestroy()
        {
            if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                webcamTexture?.Stop();
                palmDetect?.Dispose();
                landmarkDetect?.Dispose();
            }
        }
        void Update()
        {
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                return;
            }
            else if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                // prefab follow rotation of camera
                //transform.rotation = Quaternion.Euler(new Vector3(0.0f, Camera.main.transform.eulerAngles.y, 0.0f));
                
                // camera follow position of prefab (it doesnt work yet)
                //Camera.main.transform.position = transform.position;

                if (runBackground)
                {
                    if (task.Status.IsCompleted())
                    {
                        task = InvokeAsync();
                    }
                }
                else
                {
                    Invoke();
                }

                // Debug.Log(CurrentAction);

                if (CurrentAction == Actions.PALM)
                    MoveForward();

                if (palmResults == null || palmResults.Count <= 0) return;

                if (landmarkResult == null || landmarkResult.score < 0.3f) return;

                UpdateJoint(landmarkResult.joints);
            }
        }

        void LateUpdate()
        {
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                // The transform target may not destroy on level load, 
                // so we need to cover corner cases where the Main Camera is different everytime we load a new scene,
                // and reconnect when that happens
                if (cameraTransform == null && isFollowing)
                {
                    OnStartFollowing();
                }

                // only follow is explicitly declared
                if (isFollowing)
                {
                    Follow();
                }
            }
            else if (SystemInfo.deviceType == DeviceType.Handheld)
            {
            }
        }

        #endregion

        #region Public Methods

        // Raises the start following event. 
        // Use this when you don't know at the time of editing what to follow, typically instances managed by the photon network.
        public void OnStartFollowing()
        {

            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                Debug.Log("COTUM: CAMERACOTUM.OnStartFollowing() I am on desktop");
                cameraTransform = Camera.main.transform;
                isFollowing = true;
                centerOffset.y = 2.0f;
                // we don't smooth anything, we go straight to the right camera shot
                Cut();
            }
            else if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                Debug.Log("COTUM: CAMERACOTUM.OnStartFollowing() I am on mobile");

                string palmPath = Path.Combine(Application.streamingAssetsPath, palmModelFile);
                palmDetect = new PalmDetect(palmPath);

                string landmarkPath = Path.Combine(Application.streamingAssetsPath, landmarkModelFile);
                landmarkDetect = new HandLandmarkDetect(landmarkPath);
                Debug.Log($"landmark dimension: {landmarkDetect.Dim}");

                string cameraName = WebCamUtil.FindName(WebCamKind.WideAngle, false);
                webcamTexture = new WebCamTexture(cameraName, 1280, 720, 30);
                cameraView.texture = webcamTexture;
                webcamTexture.Play();
                Debug.Log($"Starting camera: {cameraName}");

                draw = new PrimitiveDraw();
                m_Rb = GetComponent<Rigidbody>();

            }
            else
            {
                Debug.LogError("COTUM: CAMERACOTUM.OnStartFollowing(): We can't create a prefab due to your device.", this);
            }

        }

        #endregion

        #region Private Methods

        // Follow the target smoothly
        void Follow()
        {
            cameraOffset.z = -distance;
            cameraOffset.y = height;

            cameraTransform.position = Vector3.Lerp(cameraTransform.position, this.transform.position + this.transform.TransformVector(cameraOffset), SmoothSpeedDesktop * Time.deltaTime);

            cameraTransform.LookAt(this.transform.position + centerOffset);

        }

        void Cut()
        {
            cameraOffset.z = -distance;
            cameraOffset.y = height;

            cameraTransform.position = this.transform.position + this.transform.TransformVector(cameraOffset);

            cameraTransform.LookAt(this.transform.position + centerOffset);
        }

        void Invoke()
        {
            palmDetect.Invoke(webcamTexture);
            cameraView.material = palmDetect.transformMat;
            cameraView.rectTransform.GetWorldCorners(rtCorners);

            palmResults = palmDetect.GetResults(0.7f, 0.3f);

            if (palmResults.Count <= 0) return;

            landmarkDetect.Invoke(webcamTexture, palmResults[0]);

            landmarkResult = landmarkDetect.GetResult();
        }

        async UniTask<bool> InvokeAsync()
        {
            palmResults = await palmDetect.InvokeAsync(webcamTexture, cancellationToken);
            cameraView.material = palmDetect.transformMat;
            cameraView.rectTransform.GetWorldCorners(rtCorners);

            if (palmResults.Count <= 0) return false;

            landmarkResult = await landmarkDetect.InvokeAsync(webcamTexture, palmResults[0], cancellationToken);

            return true;
        }

        void MoveForward()
        {
            // We move the prefab
            transform.position += Camera.main.transform.forward * Time.deltaTime * SmoothSpeedMobile;
        }

        void UpdateJoint(Vector3[] joints)
        {
            // draw.color = Color.blue;

            Vector3 min = rtCorners[0];
            Vector3 max = rtCorners[2];

            Matrix4x4 mtx = WebCamUtil.GetMatrix(-webcamTexture.videoRotationAngle, false, webcamTexture.videoVerticallyMirrored);

            float zScale = max.x - min.x;
            for (int i = 0; i < HandLandmarkDetect.JOINT_COUNT; i++)
            {
                Vector3 p0 = mtx.MultiplyPoint3x4(joints[i]);
                Vector3 p1 = MathTF.Lerp(min, max, p0);
                p1.z += (p0.z - 0.5f) * zScale;
                worldJoints[i] = p1;
            }
            /*
            for (int i = 0; i < HandLandmarkDetect.JOINT_COUNT; i++)
            {
                draw.Cube(worldJoints[i], 0.1f);
            }

            var connections = HandLandmarkDetect.CONNECTIONS;
            for (int i = 0; i < connections.Length; i += 2)
            {
                draw.Line3D(
                    worldJoints[connections[i]],
                    worldJoints[connections[i + 1]],
                    0.05f);
            }
            draw.Apply();
            */

            Vector3[] uppperJoints = { worldJoints[8], worldJoints[12], worldJoints[16], worldJoints[20] };
            Vector3 thumb = worldJoints[4];
            bool fist = true;
            bool palm = true;

            foreach (var fg in uppperJoints)
            {
                fist = fist && (fg.y < thumb.y);
            }
            foreach (var fg in uppperJoints)
            {
                palm = palm && (fg.y > thumb.y);
            }
            if (fist)
            {
                CurrentAction = Actions.FIST;
            }
            if (palm)
            {
                CurrentAction = Actions.PALM;
            }
        }
        #endregion
    }
}