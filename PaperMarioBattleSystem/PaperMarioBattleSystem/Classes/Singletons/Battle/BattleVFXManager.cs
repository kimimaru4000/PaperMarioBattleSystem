using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Handles VFX in battle. This be used for any type of visual that needs to be rendered.
    /// <para>This is a Singleton.</para>
    /// </summary>
    public class BattleVFXManager : IUpdateable, IDrawable, ICleanup
    {
        #region Singleton Fields

        public static BattleVFXManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BattleVFXManager();
                }

                return instance;
            }
        }

        private static BattleVFXManager instance = null;

        #endregion

        /// <summary>
        /// The list of VFXElements.
        /// </summary>
        private readonly List<VFXElement> VFXElements = new List<VFXElement>();

        private BattleVFXManager()
        {

        }

        public void CleanUp()
        {
            ClearAllVFX();

            instance = null;
        }

        /// <summary>
        /// Adds a VFXElement.
        /// </summary>
        /// <param name="vfxElement">The VFXElement to add.</param>
        public void AddVFXElement(VFXElement vfxElement)
        {
            VFXElements.Add(vfxElement);
        }

        /// <summary>
        /// Removes a VFXElement.
        /// </summary>
        /// <param name="vfxElement">The VFXElement to remove.</param>
        public void RemoveVFXElement(VFXElement vfxElement)
        {
            bool removed = VFXElements.Remove(vfxElement);
            if (removed == true)
            {
                vfxElement.CleanUp();
            }
        }

        /// <summary>
        /// Removes a VFXElement by index.
        /// </summary>
        /// <param name="index">The index of the VFXElement to remove from the list.</param>
        private void RemoveVFXElement(int index)
        {
            if (index < 0 || index >= VFXElements.Count)
            {
                Debug.LogError($"{index} is out of range for the {nameof(VFXElements)} list, which has a count of {VFXElements.Count}");
                return;
            }

            VFXElements[index].CleanUp();
            VFXElements.RemoveAt(index);
        }

        /// <summary>
        /// Removes all VFXElements.
        /// </summary>
        public void ClearAllVFX()
        {
            for (int i = 0; i < VFXElements.Count; i++)
            {
                RemoveVFXElement(i);
                i--;
            }
        }

        public void Update()
        {
            for (int i = 0; i < VFXElements.Count; i++)
            {
                VFXElements[i].Update();
                if (VFXElements[i].ShouldRemove == true)
                {
                    RemoveVFXElement(i);
                    i--;
                }
            }
        }

        public void Draw()
        {
            for (int i = 0; i < VFXElements.Count; i++)
            {
                VFXElements[i].Draw();
            }
        }
    }
}
