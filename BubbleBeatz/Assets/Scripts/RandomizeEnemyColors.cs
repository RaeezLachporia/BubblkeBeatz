using UnityEngine;

public class RandomizeEnemyColors : MonoBehaviour
{
    [Header("Material Settings")]
    [SerializeField] private Renderer targetRenderer;

    [Header("Color Ranges")]
    public Gradient shoeColors;
    public Gradient jacketColors;
    public Gradient hairColors;

    private MaterialPropertyBlock propBlock;

    void Start()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        if (propBlock == null)
            propBlock = new MaterialPropertyBlock();

        targetRenderer.GetPropertyBlock(propBlock);

        // Randomize colors using gradient or just random Color if you prefer
        Color shoeColor = shoeColors.Evaluate(Random.value);
        Color jacketColor = jacketColors.Evaluate(Random.value);
        Color hairColor = hairColors.Evaluate(Random.value);

        // These names must match Shader Graph property references
        propBlock.SetColor("_Shoes", shoeColor);
        propBlock.SetColor("_Jacket", jacketColor);
        propBlock.SetColor("_hair", hairColor);

        targetRenderer.SetPropertyBlock(propBlock);
    }
}