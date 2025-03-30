// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System.Collections.Generic;
using UnityEngine;

namespace Animancer
{
    /// <summary>
    /// Replaces the <see cref="SpriteRenderer.sprite"/> with a copy of it that uses a different <see cref="Texture"/>
    /// during every <see cref="LateUpdate"/>.
    /// </summary>
    /// 
    /// <remarks>This script is not specific to Animancer and will work with any animation system.</remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/SpriteRendererTextureSwap
    /// 
    [AddComponentMenu("Animancer/Sprite Renderer Texture Swap")]
    [HelpURL("https://kybernetik.com.au/animancer/api/Animancer/" + nameof(SpriteRendererTextureSwap))]
    [DefaultExecutionOrder(DefaultExecutionOrder)]
    public class SpriteRendererTextureSwap : MonoBehaviour
    {
        /************************************************************************************************************************/

        /// <summary>Execute very late (32000 is last).</summary>
        public const int DefaultExecutionOrder = 30000;

        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip("The SpriteRenderer that will have its Sprite modified")]
        private SpriteRenderer _Renderer;

        /// <summary>The <see cref="SpriteRenderer"/> that will have its <see cref="Sprite"/> modified.</summary>
        public ref SpriteRenderer Renderer => ref _Renderer;

        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip("The replacement for the original Sprite texture")]
        private Texture2D _Texture;

        /// <summary>The replacement for the original <see cref="Sprite.texture"/>.</summary>
        /// <remarks>
        /// If this texture has any <see cref="Sprite"/>s set up in its import settings, they will be completely
        /// ignored because this system creates new <see cref="Sprite"/>s at runtime. The texture doesn't even need to
        /// be set to <see cref="Sprite"/> mode.
        /// <para></para>
        /// Call <see cref="ClearCache"/> before setting this if you want to destroy any sprites created for the
        /// previous texture.
        /// </remarks>
        public Texture2D Texture
        {
            get => _Texture;
            set
            {
                _Texture = value;
                RefreshSpriteMap();
            }
        }

        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip("Should the secondary textures be swapped as well?")]
        private bool _SwapSecondaryTextures = true;

        /// <summary>Should the secondary textures be swapped as well?</summary>
        public bool SwapSecondaryTextures
        {
            get => _SwapSecondaryTextures;
            set
            {
                _SwapSecondaryTextures = value;
                RefreshSpriteMap();
            }
        }

        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip("The replacement secondary textures for the Sprite to use")]
        private SecondarySpriteTexture[] _SecondaryTextures;

        /// <summary>The replacement for the original <see cref="Sprite.GetSecondaryTextures"/>.</summary>
        /// <remarks>
        /// Swapped sprites are cached statically so they can be shared between instances. Unfortunately, the cache
        /// isn't tied to secondary textures so if multiple instances replace TextureA with TextureB and have different
        /// secondary textures then they would interfere with each other. That's unlikely to be a real issue because
        /// TextureB should always have TextureBNormals as a secondary texture. It would be possible to avoid this
        /// if necessary, but doing so would cost more performance and increase the complexity of this system.
        /// </remarks>
        public SecondarySpriteTexture[] SecondaryTextures
        {
            get => _SecondaryTextures;
            set
            {
                _SecondaryTextures = value;
                RefreshSpriteMap();
            }
        }

        /************************************************************************************************************************/

        private Dictionary<Sprite, Sprite> _SpriteMap;

        private void RefreshSpriteMap() => _SpriteMap = GetSpriteMap(_Texture);

        /************************************************************************************************************************/

        protected virtual void OnValidate()
        {
            if (_Renderer == null)
                TryGetComponent(out _Renderer);

            if (_SwapSecondaryTextures &&
                (_SecondaryTextures == null || _SecondaryTextures.Length == 0) &&
                _Renderer != null &&
                _Renderer.sprite != null)
            {
                var sprite = _Renderer.sprite;
                var count = sprite.GetSecondaryTextureCount();
                if (count > 0)
                {
                    _SecondaryTextures = new SecondarySpriteTexture[count];
                    sprite.GetSecondaryTextures(_SecondaryTextures);
                }
            }

            if (_SpriteMap != null)
                DestroySprites(_SpriteMap);
        }

        /************************************************************************************************************************/

        protected virtual void Awake() => RefreshSpriteMap();

        /************************************************************************************************************************/

        protected virtual void LateUpdate()
        {
            if (_Renderer == null)
                return;

            var sprite = _Renderer.sprite;
            var secondaryTextures = _SwapSecondaryTextures ? _SecondaryTextures : null;
            if (TrySwapTexture(_SpriteMap, _Texture, secondaryTextures, ref sprite))
                _Renderer.sprite = sprite;
        }

        /************************************************************************************************************************/

        /// <summary>Destroys all sprites created for the current <see cref="Texture"/>.</summary>
        public void ClearCache()
        {
            DestroySprites(_SpriteMap);
        }

        /************************************************************************************************************************/

        private static readonly Dictionary<Texture2D, Dictionary<Sprite, Sprite>>
            TextureToSpriteMap = new();

        /************************************************************************************************************************/

        /// <summary>Returns a cached dictionary mapping original sprites to duplicates using the specified `texture`.</summary>
        public static Dictionary<Sprite, Sprite> GetSpriteMap(Texture2D texture)
        {
            if (texture == null)
                return null;

            if (!TextureToSpriteMap.TryGetValue(texture, out var map))
                TextureToSpriteMap.Add(texture, map = new());

            return map;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// If the <see cref="Sprite.texture"/> is not already using the specified `texture`, this method replaces the
        /// `sprite` with a cached duplicate which uses that `texture` instead.
        /// </summary>
        public static bool TrySwapTexture(
            Dictionary<Sprite, Sprite> spriteMap,
            Texture2D texture,
            SecondarySpriteTexture[] secondaryTextures,
            ref Sprite sprite)
        {
            if (spriteMap == null ||
                sprite == null ||
                texture == null ||
                sprite.texture == texture)
                return false;

            if (!spriteMap.TryGetValue(sprite, out var otherSprite))
            {
                var pivot = sprite.pivot;
                pivot.x /= sprite.rect.width;
                pivot.y /= sprite.rect.height;

                secondaryTextures ??= GetSecondaryTexturesCached(sprite);

                otherSprite = Sprite.Create(texture,
                    sprite.rect, pivot, sprite.pixelsPerUnit,
                    0, SpriteMeshType.FullRect, sprite.border, false, secondaryTextures);

#if UNITY_ASSERTIONS
                var name = sprite.name;
                var originalTextureName = sprite.texture.name;
                var index = name.IndexOf(originalTextureName);
                if (index >= 0)
                {
                    var newName =
                        texture.name +
                        name[(index + originalTextureName.Length)..];

                    if (index > 0)
                        newName = name[..index] + newName;

                    name = newName;
                }

                otherSprite.name = name;
#endif

                spriteMap.Add(sprite, otherSprite);
            }

            sprite = otherSprite;
            return true;
        }

        /************************************************************************************************************************/

        private static List<SecondarySpriteTexture[]> _SecondaryTextureCache;

        /// <summary>A wrapper around <see cref="Sprite.GetSecondaryTextures"/> which reuses arrays of the same size.</summary>
        public static SecondarySpriteTexture[] GetSecondaryTexturesCached(Sprite sprite)
        {
            var count = sprite.GetSecondaryTextureCount();
            if (count == 0)
                return System.Array.Empty<SecondarySpriteTexture>();

            _SecondaryTextureCache ??= new();

            while (_SecondaryTextureCache.Count < count)
                _SecondaryTextureCache.Add(null);

            var textures = _SecondaryTextureCache[count - 1];
            if (textures == null)
            {
                textures = new SecondarySpriteTexture[count];
                _SecondaryTextureCache[count - 1] = textures;
            }

            sprite.GetSecondaryTextures(textures);
            return textures;
        }

        /// <summary>A wrapper around <see cref="Sprite.GetSecondaryTextures"/>.</summary>
        public static SecondarySpriteTexture[] GetSecondaryTextures(Sprite sprite)
        {
            var count = sprite.GetSecondaryTextureCount();
            if (count == 0)
                return System.Array.Empty<SecondarySpriteTexture>();

            var textures = new SecondarySpriteTexture[count];
            sprite.GetSecondaryTextures(textures);
            return textures;
        }

        /************************************************************************************************************************/

        /// <summary>Destroys all the <see cref="Dictionary{TKey, TValue}.Values"/>.</summary>
        public static void DestroySprites(Dictionary<Sprite, Sprite> spriteMap)
        {
            if (spriteMap == null)
                return;

            foreach (var sprite in spriteMap.Values)
                Destroy(sprite);

            spriteMap.Clear();
        }

        /************************************************************************************************************************/

        /// <summary>Destroys all sprites created for the `texture`.</summary>
        public static void DestroySprites(Texture2D texture)
        {
            if (TextureToSpriteMap.TryGetValue(texture, out var spriteMap))
            {
                TextureToSpriteMap.Remove(texture);
                DestroySprites(spriteMap);
            }
        }

        /************************************************************************************************************************/
    }
}

