using UnityEngine;

public class ExplodedCubes : MonoBehaviour
{
    public GameObject restartButton;
    private bool _collisionSet = false;

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Cube" && !_collisionSet){
            for (int i = other.transform.childCount - 1; i >=0; --i){
                Transform child = other.transform.GetChild(i);
                child.gameObject.AddComponent<Rigidbody>();
                child.gameObject.GetComponent<Rigidbody>().AddExplosionForce(70f, Vector3.up, 5f);
                child.SetParent(null);
            }

            Destroy(other.gameObject);
            _collisionSet = true;

            restartButton.SetActive(true);

            Camera.main.gameObject.AddComponent<ShakeTheCamera>();
        }
    }
}


