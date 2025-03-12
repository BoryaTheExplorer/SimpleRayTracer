using UnityEngine;

public class RayTracingMaster : MonoBehaviour
{
    [SerializeField] private Texture _skyboxTexture;
    [SerializeField] private ComputeShader _rtShader;

    [SerializeField] private bool _rayTrace = false;

    [SerializeField] private Light _directionalLight;

    private RenderTexture _target;
    private Camera _camera;

    private uint _currentSample = 0;
    private Material _addMaterial;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (transform.hasChanged)
        {
            _currentSample = 0;
            transform.hasChanged = false;
        }

        if (_directionalLight.transform.hasChanged)
        {
            _currentSample = 0;
            _directionalLight.transform.hasChanged = false;
        }
    }

    private void SetShaderParameters()
    {
        _rtShader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
        _rtShader.SetMatrix("_CameraInverseProjection", _camera.projectionMatrix.inverse);
        _rtShader.SetTexture(0, "_SkyboxTexture", _skyboxTexture);
        _rtShader.SetVector("_PixelOffset", new Vector2(Random.value, Random.value));

        Vector3 light = _directionalLight.transform.forward;
        _rtShader.SetVector("_DirectionalLight", new Vector4(light.x, light.y, light.z, _directionalLight.intensity));
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!_rayTrace)
        {
            Graphics.Blit(source, destination);
        }

        SetShaderParameters();
        Render(destination);
    }
    private void Render(RenderTexture destination)
    {
        InitRenderTexture();

        _rtShader.SetTexture(0, "Result", _target);

        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8f);

        _rtShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        if (_addMaterial == null)
        {
            _addMaterial = new Material(Shader.Find("Hidden/AddShader"));
        }

        _addMaterial.SetFloat("_Sample", _currentSample);

        Graphics.Blit(_target, destination, _addMaterial);
        _currentSample++;
    }

    private void InitRenderTexture()
    {
        if (_target == null || _target.width != Screen.width || _target.height != Screen.height)
        {
            if (_target != null )
            {
                _target.Release();
            }

            _target = new RenderTexture(Screen.width, Screen.height, 0, 
                                        RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            _target.enableRandomWrite = true;
            _target.Create();
        }
    }
}
