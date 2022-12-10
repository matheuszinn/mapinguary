using UnityEngine;

// Render the screen in lower resolution

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ResoScaler : MonoBehaviour
{    
    [Range(1, 8)]
    public float scale = 1;

    public FilterMode filterMode = FilterMode.Point;

    private RenderTexture _rt;

    void OnPreRender()
    {
        // before rendering, setup our RenderTexture
        int width = Mathf.RoundToInt(Screen.width / scale);
        int height = Mathf.RoundToInt(Screen.height / scale);
        _rt = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);           
        GetComponent<Camera>().targetTexture = _rt;
    }

    void OnPostRender()
    {
        // after rendering we need to clear the targetTexture so that post effect will be able to render 
        // to the screen
        GetComponent<Camera>().targetTexture = null;
        RenderTexture.active = null;
    }
    
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // set our filtering mode and blit to the screen
        src.filterMode = filterMode;
        
        Graphics.Blit(src, dest);
        
        RenderTexture.ReleaseTemporary(_rt);
    }
    
}