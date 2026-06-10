using UnityEngine;

namespace Moirai.Atropos.Localization
{
    public class SpriteRendererInjector : ImageInjectorBase
    {
        private readonly SpriteRenderer _spriteRenderer;
        private readonly Sprite[] _sprites;

        public SpriteRendererInjector(SpriteRenderer spriteRenderer, string localizedTextID, Sprite[] sprites)
            : base(localizedTextID)
        {
            _spriteRenderer = spriteRenderer;
            _sprites = sprites;
        }

        protected override void ApplyFromArray(int index)
        {
            _spriteRenderer.sprite = _sprites[index];
        }

        protected override void ApplyAsset(Object asset)
        {
            _spriteRenderer.sprite = asset as Sprite;
        }

        protected override string GetExpectedTypeName() => "Sprite";

        protected override bool IsExpectedType(Object asset) => asset is Sprite;

        protected override bool IsConvertibleType(Object asset) => asset is Texture2D;

        protected override bool TryConvertAndApply(Object asset)
        {
            if (asset is Texture2D texture)
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                _spriteRenderer.sprite = sprite;
                return true;
            }
            return false;
        }
    }
}
