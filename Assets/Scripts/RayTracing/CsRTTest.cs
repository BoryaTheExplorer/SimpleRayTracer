using Unity.VisualScripting;
using UnityEngine;

public class CsRTTest : MonoBehaviour
{
    [SerializeField] private ComputeShader _csRT;
    [SerializeField] private Renderer _target;

    private RenderTexture _renderTex;

    private ComputeBuffer _cameraBuffer;

    private void Start()
    {
        InitTexture();
        InitShader();
    }

    private void Update()
    {
        InitShader();
    }
    private void InitTexture()
    {
        _renderTex = new RenderTexture(512, 512, 0);
        _renderTex.enableRandomWrite = true;
        _renderTex.Create();
    }

    private void InitShader()
    {
        int kernelIndex = _csRT.FindKernel("CSMain");
        print("Kernel Index: " + kernelIndex);

        _csRT.SetTexture(kernelIndex, "ResultTexture", _renderTex);

        int threadGroupsX = Mathf.CeilToInt(512f / 8f);
        int threadGroupsY = Mathf.CeilToInt(512f / 8f);

        SetupCameraData();

        _csRT.Dispatch(kernelIndex, threadGroupsX, threadGroupsY, 1);

        if(_target != null)
        {
            _target.material.mainTexture = _renderTex;
        }

        if(_cameraBuffer != null)
        {
            _cameraBuffer.Release();
        }
    }

    private void CreateSpheres()
    {
        Sphere[] spheres = GenerateRandomSpheres();

        ComputeBuffer spheresBuffer = new ComputeBuffer(spheres.Length, sizeof (float) * 7);
        spheresBuffer.SetData(spheres);
        _csRT.SetBuffer(0, "Spheres", spheresBuffer);

        spheresBuffer.Release();
    }

    private Sphere[] GenerateRandomSpheres()
    {
        int count = 11;
        float limit = 10f;

        Sphere[] spheres = new Sphere[count];

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-limit, limit), Random.Range(-limit, limit), Random.Range(-limit, limit));
            float radius = Random.Range(1, 6);
            Vector3 col = new Vector3(pos.x, pos.y, pos.z);
            spheres[i] = new Sphere()
            {
                position = pos,
                radius = radius,
                color = col
            };
        }

        return spheres;
    }
    private void SetupCameraData()
    {
        Camera cam = Camera.main;
        CameraData[] data = new CameraData[1];

        data[0] = new CameraData()
        {
            position = cam.transform.position,
        };

        print(data[0].position);

        _cameraBuffer = new ComputeBuffer(1, sizeof(float) * 3);
        _cameraBuffer.SetData(data);
        _csRT.SetBuffer(0, "Camera", _cameraBuffer);
    }
}

public struct Sphere
{
    public Vector3 position;
    public float radius;
    public Vector3 color;
}
public struct CameraData
{
    public Vector3 position;
}
