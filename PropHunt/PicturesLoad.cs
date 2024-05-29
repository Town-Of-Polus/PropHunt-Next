using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Linq;
using HarmonyLib;
using Hazel;
using System.Threading.Tasks;
using Reactor.Utilities.Extensions;
using AmongUs.GameOptions;
using System.Collections;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.InteropTypes;
using Reactor.Networking.Rpc;
using Unity.Services.Core.Telemetry.Internal;

namespace PropHunt
{
    public static class PicturesLoad
    {

        public static Dictionary<string, Sprite> CachedSprites = new();

        public static Sprite loadSpriteFromResources(string path, float pixelsPerUnit, bool cache = true)
        {
            try
            {
                if (cache && CachedSprites.TryGetValue(path + pixelsPerUnit, out var sprite)) return sprite;
                Texture2D texture = loadTextureFromResources(path);
                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
                if (cache) sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
                if (!cache) return sprite;
                return CachedSprites[path + pixelsPerUnit] = sprite;
            }
            catch
            {
                System.Console.WriteLine("Error loading sprite from path: " + path);
            }
            return null;
        }

        public static unsafe Texture2D loadTextureFromResources(string path)
        {
            try
            {
                Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(path);
                var length = stream.Length;
                var byteTexture = new Il2CppStructArray<byte>(length);
                stream.Read(new Span<byte>(IntPtr.Add(byteTexture.Pointer, IntPtr.Size * 4).ToPointer(), (int)length));
                if (path.Contains("HorseHats"))
                {
                    byteTexture = new Il2CppStructArray<byte>(byteTexture.Reverse().ToArray());
                }
                ImageConversion.LoadImage(texture, byteTexture, false);
                return texture;
            }
            catch
            {
                System.Console.WriteLine("Error loading texture from resources: " + path);
            }
            return null;
        }

        public static Texture2D loadTextureFromDisk(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                    var byteTexture = Il2CppSystem.IO.File.ReadAllBytes(path);
                    ImageConversion.LoadImage(texture, byteTexture, false);
                    return texture;
                }
            }
            catch
            {
                System.Console.WriteLine("Error loading texture from disk: " + path);
            }
            return null;
        }

        public static AudioClip loadAudioClipFromResources(string path, string clipName = "UNNAMED_TOR_AUDIO_CLIP")
        {
            // must be "raw (headerless) 2-channel signed 32 bit pcm (le)" (can e.g. use Audacity® to export)
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(path);
                var byteAudio = new byte[stream.Length];
                _ = stream.Read(byteAudio, 0, (int)stream.Length);
                float[] samples = new float[byteAudio.Length / 4]; // 4 bytes per sample
                int offset;
                for (int i = 0; i < samples.Length; i++)
                {
                    offset = i * 4;
                    samples[i] = (float)BitConverter.ToInt32(byteAudio, offset) / Int32.MaxValue;
                }
                int channels = 2;
                int sampleRate = 48000;
                AudioClip audioClip = AudioClip.Create(clipName, samples.Length / 2, channels, sampleRate, false);
                audioClip.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
                audioClip.SetData(samples, 0);
                return audioClip;
            }
            catch
            {
                System.Console.WriteLine("Error loading AudioClip from resources: " + path);
            }
            return null;
        }
    }
}
