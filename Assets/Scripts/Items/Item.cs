using UnityEngine;

public class Item : MonoBehaviour {
    [SerializeField] private ItemStats itemStats;


    private void Update() {
        doHover();
    }

    private void doHover() {
        transform.position = new Vector3(transform.position.x, Mathf.Sin(Time.time * 2) * 0.1f + 0.5f, transform.position.z);
    }



}