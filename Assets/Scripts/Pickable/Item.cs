using UnityEngine;

public class Item : MonoBehaviour {
    [SerializeField] private ItemStats itemStats;

    private float initialY;
    private void Start() {
        initialY = transform.position.y;
    }

    private void Update() {
        doHover();
    }

    private void doHover() {
        transform.position = new Vector3(transform.position.x, initialY + Mathf.Sin(Time.time) + 0.5f, transform.position.z);
    }



}