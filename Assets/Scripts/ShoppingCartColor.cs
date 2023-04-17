using UnityEngine;

public class ShoppingCartColor : AGuidListener {
    [SerializeField] private MeshRenderer meshRenderer;

    private void Start() {
        this.guidSource = GetComponent<AGuidSource>();
    }

    public void SetColor(UnityEngine.Color color) {
        MaterialPropertyBlock mp = new MaterialPropertyBlock();
        mp.SetColor("_BaseColor", color);
        this.meshRenderer.SetPropertyBlock(mp);
    }
}
