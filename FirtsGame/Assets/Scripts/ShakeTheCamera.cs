using UnityEngine;

public class ShakeTheCamera : MonoBehaviour
{
    private Transform camTransform;

    private float shakeDur = 1f, shakePower = 0.08f, decreaseFactor = 1.5f;

    private Vector3 originPos;

    private void Start() {
        camTransform = GetComponent<Transform>();
        originPos = GetComponent<Transform>().localPosition;
    }

    private void Update() {
        if (shakeDur > 0){
            camTransform.localPosition = originPos + Random.insideUnitSphere * shakePower;
            shakeDur -= Time.deltaTime * decreaseFactor;
        } else {
            shakeDur = 0;
            camTransform.localPosition = originPos;
        }
    }


}
