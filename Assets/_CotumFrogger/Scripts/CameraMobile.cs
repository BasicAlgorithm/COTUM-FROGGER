using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using TensorFlowLite;
using UnityEngine;
using UnityEngine.UI;

public class CameraMobile : MonoBehaviour
{
    #region Private Fields

    // We have to implement this animator. PhotonAnimatorView catch this animator
    // and their data to transmit to others player.
    //private Animator animator;

    // maintain a flag internally to reconnect if target is lost or camera is switched
    bool isFollowing;

    #endregion

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

    enum Actions {FIST, PALM, NOTHING}
    Actions CurrentAction = Actions.NOTHING;
    
    Rigidbody m_Rb;
    float m_Speed = 2.0f;

    void Start()
    {
    }

    void OnDestroy()
    {
        webcamTexture?.Stop();
        palmDetect?.Dispose();
        landmarkDetect?.Dispose();
    }

    void Update()
    {
        if (isFollowing)
        {
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

    void MoveForward() {
        transform.position += Camera.main.transform.forward * Time.deltaTime * m_Speed;
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

        Vector3[] uppperJoints = {worldJoints[8], worldJoints[12], worldJoints[16], worldJoints[20]};
        Vector3 thumb = worldJoints[4];
        bool fist = true;
        bool palm = true;

        foreach (var fg in uppperJoints){
            fist = fist && (fg.y < thumb.y);
        }
        foreach (var fg in uppperJoints){
            palm = palm && (fg.y > thumb.y);
        }
        if (fist){
            CurrentAction = Actions.FIST;
        }
        if (palm) {
            CurrentAction = Actions.PALM;
        }
    }

    #region Public Methods

    // cached transform of the target
    Transform cameraTransform;

    // Cache for camera offset
    Vector3 cameraOffset = Vector3.zero;

    // Raises the start following event. 
    // Use this when you don't know at the time of editing what to follow, typically instances managed by the photon network.
    public void OnStart()
    {
        Debug.Log("CAMERA MOBILE ONSTART CALLED");

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

        isFollowing = true;
    }

    #endregion
}