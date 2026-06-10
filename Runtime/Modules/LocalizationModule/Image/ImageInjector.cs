using UnityEngine;
using UnityEngine.UI;

namespace Moirai.Atropos.Localization
{
	public class ImageInjector : ImageInjectorBase
	{
		private readonly Image _image;
		private readonly Sprite[] _sprites;

		public ImageInjector(Image image, string localizedTextID, Sprite[] sprites)
			: base(localizedTextID)
		{
			_image = image;
			_sprites = sprites;
		}

		protected override void ApplyFromArray(int index)
		{
			_image.sprite = _sprites[index];
		}

		protected override void ApplyAsset(Object asset)
		{
			_image.sprite = asset as Sprite;
		}

		protected override string GetExpectedTypeName() => "Sprite";

		protected override bool IsExpectedType(Object asset) => asset is Sprite;

		protected override bool IsConvertibleType(Object asset) => asset is Texture2D;

		protected override bool TryConvertAndApply(Object asset)
		{
			if (asset is Texture2D texture)
			{
				Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
				_image.sprite = sprite;
				return true;
			}
			return false;
		}
	}
}
