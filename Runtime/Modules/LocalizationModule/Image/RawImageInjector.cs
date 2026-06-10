using UnityEngine;
using UnityEngine.UI;

namespace Moirai.Atropos.Localization
{
	public class RawImageInjector : ImageInjectorBase
	{
		private readonly RawImage _rawImage;
		private readonly Texture[] _textures;

		public RawImageInjector(RawImage rawImage, string localizedTextID, Texture[] textures)
			: base(localizedTextID)
		{
			_rawImage = rawImage;
			_textures = textures;
		}

		protected override void ApplyFromArray(int index)
		{
			_rawImage.texture = _textures[index];
		}

		protected override void ApplyAsset(Object asset)
		{
			_rawImage.texture = asset as Texture;
		}

		protected override string GetExpectedTypeName() => "Texture";

		protected override bool IsExpectedType(Object asset) => asset is Texture;

		protected override bool IsConvertibleType(Object asset) => asset is Sprite;

		protected override bool TryConvertAndApply(Object asset)
		{
			if (asset is Sprite sprite)
			{
				_rawImage.texture = sprite.texture;
				return true;
			}
			return false;
		}
	}
}
