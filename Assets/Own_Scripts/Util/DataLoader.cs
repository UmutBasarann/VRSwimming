using Avataris.VR.Scripts.Data.Core;
using UnityEngine;

namespace Avataris.VR.Scripts.Util
{
    public class DataLoader
    {
        #region Fields

        private TextAsset _jsonData;

        #endregion

        #region LoadFromJson

        public T LoadFromJson<T>(string dataPath)
            where T: IData
        {
            _jsonData = Resources.Load<TextAsset>(dataPath);
            return JsonUtility.FromJson<T>(_jsonData.text);
        }

        #endregion
    }
}
