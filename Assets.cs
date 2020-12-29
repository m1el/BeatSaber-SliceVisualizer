using IPA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace SliceVisualizer
{
    class Assets : MonoBehaviour
    {
        public static Sprite RRect;
        internal static IPALogger Log { get; private set; }
        public static void Init(IPALogger logger)
        {
            Log = logger;
            RRect = LoadSpriteFromResources("SliceVisualizer.Resources.RRect.png");
        }

        public static Sprite LoadSpriteFromResources(string resourcePath, float pixelsPerUnit = 100.0f)
        {
            var assembly = Assembly.GetCallingAssembly();
            var stream = assembly.GetManifestResourceStream(resourcePath);
            byte[] imageData = new byte[stream.Length];
            stream.Read(imageData, 0, (int)stream.Length);
            if (imageData.Count() == 0) return null;
            var texture = new Texture2D(2, 2);
            texture.LoadRawTextureData(imageData);
            var rect = new Rect(0, 0, texture.width, texture.height);
            var sprite = Sprite.Create(texture, rect, new Vector2(0, 0), pixelsPerUnit);
            Log.Info(string.Format("Successfully loaded sprite {0}, w={1}, h={2}",
                resourcePath, texture.width, texture.height));
            return sprite;
        }
    }
}
