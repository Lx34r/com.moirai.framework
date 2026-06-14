using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Moirai.Atropos.ConfigTable
{
    /// <summary>
    /// 配置表辅助器接口。
    /// </summary>
      public interface IConfigTableModule
      {
          /// <summary>
          /// 从配置表获取所有多语言文本。
          /// </summary>
          public Dictionary<string, List<string>> GetAllLocalizedStrings();

          /// <summary>
          /// 根据 ID 从配置表加载图标。
          /// </summary>
          /// <param name="id"></param>
          /// <param name="cancellationToken"></param>
          /// <returns></returns>
          public UniTask<Sprite> LoadSpriteByID(string id, CancellationToken cancellationToken = default);

          /// <summary>
          /// 根据 ID 从配置表获取弹窗资产的位置。
          /// </summary>
          /// <param name="id"></param>
          /// <returns></returns>
          public string GetUIWindowLocation(string id);
      }
}