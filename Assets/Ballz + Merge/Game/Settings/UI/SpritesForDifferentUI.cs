using UnityEngine;

[CreateAssetMenu(fileName = "SpritesForDifferentUI", menuName = "Bellz+Merge/Game/SpritesForDifferentUI", order = 51)]
public class SpritesForDifferentUI : ScriptableObject
{
    [SerializeField] private Sprite _horizontalMainSprite;
    [SerializeField] private Sprite _verticalMainSprite;
    [SerializeField] private Sprite _horizontalShaderSprite;
    [SerializeField] private Sprite _verticalShaderSprite;

    public Sprite HorizontalMainSprite => _horizontalMainSprite;
    public Sprite VerticalMainSprite => _verticalMainSprite;
    public Sprite HorizontalShaderSprite => _horizontalShaderSprite;
    public Sprite VerticalShaderSprite => _verticalShaderSprite;
}
