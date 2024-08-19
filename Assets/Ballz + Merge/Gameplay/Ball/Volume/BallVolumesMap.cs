using System.Collections.Generic;
using UnityEditor;

public class BallVolumesMap
{
    private const string VolumesPath = "Assets/Ballz + Merge/Gameplay/Ball/Volume/Volumes";

    private Dictionary<BallVolumesTypes, BallVolume> _volumes = new Dictionary<BallVolumesTypes, BallVolume>();

    public BallVolumesMap()
    {
#if UNITY_EDITOR
        //string[] guids = AssetDatabase.FindAssets("t:BallVolume", new[] { VolumesPath });

        //foreach (string guid in guids)
        //{
        //    BallVolume volume = AssetDatabase.LoadAssetAtPath<BallVolume>(AssetDatabase.GUIDToAssetPath(guid));
        //
        //    if (volume != null)
        //        _volumes.Add(volume.Type, volume);
        //}
#endif
    }

    public BallVolume GetVolume(BallVolumesTypes type) => _volumes[type];
}
