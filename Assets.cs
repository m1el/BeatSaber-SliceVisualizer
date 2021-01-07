using System.Linq;
using System.Reflection;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace SliceVisualizer
{
    public class Assets : MonoBehaviour
    {
        public static Sprite? RRect { get; private set; }
        public static Sprite? Circle { get; private set; }
        public static Sprite? Arrow { get; private set; }
        public static Sprite? White { get; private set; }
        private static IPALogger Log = null!;
        public static void Init(IPALogger logger)
        {
            Log = logger;
            // var assembly = Assembly.GetCallingAssembly();
            // Log.Info(string.Join(", ", assembly.GetManifestResourceNames()));
            RRect = LoadSpriteFromResources("SliceVisualizer.Assets.RRect.png");
            Circle = LoadSpriteFromResources("SliceVisualizer.Assets.Circle.png");
            Arrow = LoadSpriteFromResources("SliceVisualizer.Assets.Arrow.png");
            White = LoadSpriteFromResources("SliceVisualizer.Assets.White.png", 1f);
        }
        private static Sprite MakePixel(Color color, float size)
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, color);
            var rect = new Rect(0, 0, 1, 1);
            var sprite = Sprite.Create(texture, rect, Vector2.zero, 1f / size);
            return sprite;
        }

        private static Sprite? LoadSpriteFromResources(string resourcePath, float pixelsPerUnit = 256.0f)
        {
            var assembly = Assembly.GetCallingAssembly();
            var stream = assembly.GetManifestResourceStream(resourcePath);
            var imageData = new byte[stream.Length];
            stream.Read(imageData, 0, (int)stream.Length);
            if (imageData.Count() == 0) {
                return null;
            }
            var texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);
            var rect = new Rect(0, 0, texture.width, texture.height);
            var sprite = Sprite.Create(texture, rect, Vector2.zero, pixelsPerUnit);
            Log.Info(string.Format("Successfully loaded sprite {0}, w={1}, h={2}",
                resourcePath, texture.width, texture.height));
            return sprite;
        }
    }
}
