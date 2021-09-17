using UnityEngine;
using UnityEngine.EventSystems;

public class CameraResize : UIBehaviour
{
    private Camera _camera;
    private bool _isAwake;
    //public int BlocksPerLine => GameManager.Instance.BlockPerLine;

    protected override void Awake()
    {
        _camera = Camera.main;
        _isAwake = true;
    }

}
