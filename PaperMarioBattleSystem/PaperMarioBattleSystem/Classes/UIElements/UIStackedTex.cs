using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A UIElement containing <see cref="CroppedTexture2D"/>s overlaid on top of one another.
    /// </summary>
    public sealed class UIStackedTex : PosUIElement, ITintable
    {
        /// <summary>
        /// The list of <see cref="CroppedTexture2D"/>s to render. Later elements have their depths offset by DepthDiff.
        /// </summary>
        private List<CroppedTexture2D> TexList = new List<CroppedTexture2D>();

        /// <summary>
        /// The amount to offset the depth of each element by.
        /// </summary>
        public float DepthDiff = .01f;

        public bool FlipX = false;
        public bool FlipY = false;
        public float Rotation = 0f;
        public float BaseDepth = 0f;
        public float Scale = 1f;
        public Color TintColor { get; set; }

        public UIStackedTex()
        {
            
        }

        public UIStackedTex(float depthDiff, params CroppedTexture2D[] croppedTextures)
        {
            DepthDiff = depthDiff;

            TexList.AddRange(croppedTextures);
        }

        /// <summary>
        /// Adds a <see cref="CroppedTexture2D"/> to the <see cref="UIStackedTex"/>.
        /// </summary>
        /// <param name="croppedTex2D">The <see cref="CroppedTexture2D"/> to add.</param>
        public void AddElement(CroppedTexture2D croppedTex2D)
        {
            if (croppedTex2D == null || croppedTex2D.Tex == null)
            {
                Debug.LogError($"{nameof(croppedTex2D)} or it's {nameof(croppedTex2D.Tex)} is null! Not adding");
                return;
            }

            TexList.Add(croppedTex2D);
        }

        /// <summary>
        /// Removes a <see cref="CroppedTexture2D"/> from the <see cref="UIStackedTex"/>.
        /// </summary>
        /// <param name="croppedTex2D">The <see cref="CroppedTexture2D"/> to remove.</param>
        public void RemoveElement(CroppedTexture2D croppedTex2D)
        {
            TexList.Remove(croppedTex2D);
        }

        /// <summary>
        /// Removes an element at a particular index from the <see cref="UIStackedTex"/>.
        /// </summary>
        /// <param name="index">The zero-based index to remove the element at.</param>
        public void RemoveElementAt(int index)
        {
            if (index < 0 || index >= TexList.Count)
            {
                Debug.LogWarning($"Index {index} is out of range of {nameof(TexList)}'s count of {TexList.Count}");
                return;
            }

            TexList.RemoveAt(index);
        }

        /// <summary>
        /// Removes the first <see cref="CroppedTexture2D"/> added to the <see cref="UIStackedTex"/>.
        /// </summary>
        public void RemoveFirst()
        {
            RemoveElementAt(0);
        }

        /// <summary>
        /// Removes the last <see cref="CroppedTexture2D"/> added to the <see cref="UIStackedTex"/>.
        /// </summary>
        public void RemoveLast()
        {
            RemoveElementAt(TexList.Count - 1);
        }

        /// <summary>
        /// Removes all <see cref="CroppedTexture2D"/>s from the <see cref="UIStackedTex"/>.
        /// </summary>
        public void RemoveAllElements()
        {
            TexList.Clear();
        }

        public override void Update()
        {
            
        }

        public override void Draw()
        {
            for (int i = 0; i < TexList.Count; i++)
            {
                CroppedTexture2D croppedTex = TexList[i];
                float realDepth = BaseDepth + (i * DepthDiff);

                SpriteRenderer.Instance.Draw(croppedTex.Tex, Position, croppedTex.SourceRect, TintColor, Rotation, Vector2.Zero, Scale, FlipX, FlipY, realDepth, true);
            }
        }
    }
}
