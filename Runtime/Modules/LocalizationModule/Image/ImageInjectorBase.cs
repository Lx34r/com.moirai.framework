using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Moirai.Atropos.Localization
{
    /// <summary>
    /// Base class for image-based localization injectors that share the common pattern of:
    /// - Checking if localization uses an index or a resource text ID
    /// - Loading assets asynchronously from the resource system
    /// - Handling Sprite/Texture type conversion with appropriate logging
    /// </summary>
    public abstract class ImageInjectorBase : IInjector
    {
        private readonly string _localizedTextID;

        protected ImageInjectorBase(string localizedTextID)
        {
            _localizedTextID = localizedTextID;
        }

        public void Inject<T1, T2>(T1 localizedData, T2 localizer) where T2 : LocalizerBase
        {
            if (localizedData is int index)
            {
                if (string.IsNullOrEmpty(_localizedTextID))
                {
                    ApplyFromArray(index);
                }
                else
                {
                    ApplyFromResource().Forget();
                }
            }
        }

        /// <summary>
        /// Apply the localized asset from a pre-assigned array by index.
        /// </summary>
        protected abstract void ApplyFromArray(int index);

        /// <summary>
        /// Apply the loaded asset to the target component.
        /// Called after the asset is successfully loaded and validated.
        /// </summary>
        protected abstract void ApplyAsset(Object asset);

        /// <summary>
        /// Get the expected asset type name for error messages.
        /// </summary>
        protected abstract string GetExpectedTypeName();

        /// <summary>
        /// Try to convert a mismatched asset type and apply it.
        /// Returns true if conversion was handled, false otherwise.
        /// </summary>
        protected abstract bool TryConvertAndApply(Object asset);

        private async UniTaskVoid ApplyFromResource()
        {
            string textIDValue = GameModule.Localization.GetTextFromId(_localizedTextID);
            var result = await GameModule.Resource.LoadAssetAsync<Object>(textIDValue);

            if (!IsExpectedType(result) && !IsConvertibleType(result))
            {
                Log.Error($"本地化图片类型错误，{textIDValue}");
            }

            if (TryConvertAndApply(result))
            {
                Log.Warning($"本地化图片类型错误，已自动转换：{result}");
                return;
            }

            ApplyAsset(result);
        }

        /// <summary>
        /// Check if the loaded asset is the expected primary type.
        /// </summary>
        protected abstract bool IsExpectedType(Object asset);

        /// <summary>
        /// Check if the loaded asset is a type that can be converted.
        /// </summary>
        protected abstract bool IsConvertibleType(Object asset);
    }
}
