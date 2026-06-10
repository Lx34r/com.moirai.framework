using UnityEngine;

namespace Moirai.Atropos.Localization
{
	public class TextureInjector : ImageInjectorBase
	{
		private readonly Renderer _renderer;
		private readonly string _propertyName;
		private readonly Texture2D[] _texture2Ds;

		public TextureInjector(Renderer renderer, string localizedTextID, string propertyName, Texture2D[] texture2Ds)
			: base(localizedTextID)
		{
			_renderer = renderer;
			_propertyName = propertyName;
			_texture2Ds = texture2Ds;
		}

		protected override void ApplyFromArray(int index)
		{
			_renderer.material.SetTexture(_propertyName, _texture2Ds[index]);
		}

		protected override void ApplyAsset(Object asset)
		{
			_renderer.material.SetTexture(_propertyName, asset as Texture2D);
		}

		protected override string GetExpectedTypeName() => "Texture2D";

		protected override bool IsExpectedType(Object asset) => asset is Texture2D;

		protected override bool IsConvertibleType(Object asset) => asset is Sprite;

		protected override bool TryConvertAndApply(Object asset)
		{
			if (asset is Sprite sprite)
			{
				_renderer.material.SetTexture(_propertyName, sprite.texture);
				return true;
			}
			return false;
		}
	}
}
