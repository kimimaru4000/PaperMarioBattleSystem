using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A class for defining Extension Methods
    /// </summary>
    public static class Extensions
    {
        #region Texture2D Extensions
        
        /// <summary>
        /// Gets the origin of a Texture2D by ratio instead of specifying width and height
        /// </summary>
        /// <param name="texture2D">The texture to get the origin for</param>
        /// <param name="x">The X ratio of the origin, between 0 and 1</param>
        /// <param name="y">The Y ratio of the origin, between 0 and 1</param>
        /// <returns>A Vector2 with the origin</returns>
        public static Vector2 GetOrigin(this Texture2D texture2D, float x, float y)
        {
            int xVal = (int)(texture2D.Width * UtilityGlobals.Clamp(x, 0f, 1f));
            int yVal = (int)(texture2D.Height * UtilityGlobals.Clamp(y, 0f, 1f));

            return new Vector2(xVal, yVal);
        }

        /// <summary>
        /// Gets the center origin of a Texture2D
        /// </summary>
        /// <param name="texture2D">The texture to get the origin for</param>
        /// <returns>A Vector2 with the center origin</returns>
        public static Vector2 GetCenterOrigin(this Texture2D texture2D)
        {
            return texture2D.GetOrigin(.5f, .5f);
        }

        /// <summary>
        /// Gets the texture coordinates at specified X and Y values of a Texture2D in a Vector2. The returned X and Y values will be from 0 to 1.
        /// </summary>
        /// <param name="texture2D">The Texture2D to get the texture coordinates from.</param>
        /// <param name="sourceRect">The Rectangle to get the coordinates from.</param>
        /// <returns>A Vector2 with the Rectangle's X and Y values divided by the texture's width and height, respectively.</returns>
        public static Vector2 GetTexCoordsAt(this Texture2D texture2D, Rectangle? sourceRect)
        {
            Vector2 texCoords = Vector2.Zero;

            if (sourceRect != null)
            {
                return GetTexCoordsAt(texture2D, sourceRect.Value.X, sourceRect.Value.Y);
            }

            return texCoords;
        }

        /// <summary>
        /// Gets the texture coordinates at specified X and Y values of a Texture2D in a Vector2. The returned X and Y values will be from 0 to 1.
        /// </summary>
        /// <param name="texture2D">The Texture2D to get the texture coordinates from.</param>
        /// <param name="x">The X position on the texture.</param>
        /// <param name="y">The Y position on the texture.</param>
        /// <returns>A Vector2 with the X and Y values divided by the texture's width and height, respectively.</returns>
        public static Vector2 GetTexCoordsAt(this Texture2D texture2D, int x, int y)
        {
            Vector2 texCoords = Vector2.Zero;

            //Get the ratio of the X and Y values from the Width and Height of the texture
            if (texture2D.Width > 0)
                texCoords.X = x / (float)texture2D.Width;
            if (texture2D.Height > 0)
                texCoords.Y = y / (float)texture2D.Height;

            return texCoords;
        }

        #endregion

        #region SpriteFont Extensions

        /// <summary>
        /// Gets the origin of a SpriteFont by ratio instead of specifying width and height
        /// </summary>
        /// <param name="spriteFont">The font to get the origin for</param>
        /// <param name="text">The text to be displayed</param>
        /// <param name="x">The X ratio of the origin, between 0 and 1</param>
        /// <param name="y">The Y ratio of the origin, between 0 and 1</param>
        /// <returns>A Vector2 with the origin</returns>
        public static Vector2 GetOrigin(this SpriteFont spriteFont, string text, float x, float y)
        {
            if (string.IsNullOrEmpty(text) == true) return Vector2.Zero;

            Vector2 size = spriteFont.MeasureString(text);
            size.X *= UtilityGlobals.Clamp(x, 0f, 1f);
            size.Y *= UtilityGlobals.Clamp(y, 0f, 1f);

            return size;
        }

        /// <summary>
        /// Gets the center origin of a SpriteFont
        /// </summary>
        /// <param name="spriteFont">The font to get the origin for</param>
        /// <param name="text">The text to be displayed</param>
        /// <returns>A Vector2 with the center origin</returns>
        public static Vector2 GetCenterOrigin(this SpriteFont spriteFont, string text)
        {
            return spriteFont.GetOrigin(text, .5f, .5f);
        }

        #endregion

        #region Rectangle Extensions

        /// <summary>
        /// Gets the origin of a Rectangle
        /// </summary>
        /// <param name="rectangle">The Rectangle to get the origin for</param>
        /// <param name="x">The X ratio of the origin, from 0 to 1</param>
        /// <param name="y">The Y ratio of the origin, from 0 to 1</param>
        /// <returns>A Vector2 with the origin</returns>
        public static Vector2 GetOrigin(this Rectangle rectangle, float x, float y)
        {
            int xVal = (int)(rectangle.Width * UtilityGlobals.Clamp(x, 0f, 1f));
            int yVal = (int)(rectangle.Height * UtilityGlobals.Clamp(y, 0f, 1f));

            return new Vector2(xVal, yVal);
        }

        /// <summary>
        /// Gets the center origin of a Rectangle
        /// </summary>
        /// <param name="rectangle">The Rectangle to get the origin for</param>
        /// <returns>A Vector2 with the center origin</returns>
        public static Vector2 GetCenterOrigin(this Rectangle rectangle)
        {
            return rectangle.GetOrigin(.5f, .5f);
        }

        #endregion

        #region Color Extensions

        /// <summary>
        /// Divides a Color's components by a scalar amount.
        /// </summary>
        /// <param name="color">The Color.</param>
        /// <param name="scalar">The scalar value to divide the Color components by.</param>
        /// <returns>A Color which has the components of the original Color divided by the scalar amount and floored due to integer casting.</returns>
        public static Color Divide(this Color color, float scalar)
        {
            return new Color((int)(color.R / scalar), (int)(color.G / scalar), (int)(color.B / scalar), (int)(color.A / scalar));
        }

        /// <summary>
        /// Multiplies a Color's components by a scalar amount, using the ceiling of the resulting values.
        /// </summary>
        /// <param name="color">The Color.</param>
        /// <param name="scalar">The scalar value to multiply the Color components by.</param>
        /// <returns>A Color which has the components of the original Color multiplied by the scalar amount, using the ceiling of the results.</returns>
        public static Color CeilingMult(this Color color, float scalar)
        {
            return new Color((int)Math.Ceiling(color.R * scalar), (int)Math.Ceiling(color.G * scalar), (int)Math.Ceiling(color.B * scalar), (int)Math.Ceiling(color.A * scalar));
        }

        #endregion

        #region Vector2 Extensions

        /// <summary>
        /// Halves the Vector2
        /// </summary>
        /// <param name="vector2">The Vector2 to halve</param>
        /// <returns>A Vector2 with the X and Y components halved</returns>
        public static Vector2 Halve(this Vector2 vector2)
        {
            return vector2 / 2f;
        }

        /// <summary>
        /// Halves the Vector2, truncating the X and Y components to the nearest integer
        /// </summary>
        /// <param name="vector2">The Vector2 to halve</param>
        /// <returns>A Vector2 with the X and Y components halved as integer values</returns>
        public static Vector2 HalveInt(this Vector2 vector2)
        {
            return new Vector2((int)(vector2.X / 2f), (int)(vector2.Y / 2f));
        }

        #endregion

        #region BattleEntity Extensions

        /// <summary>
        /// A helper method for adding an integer AdditionalProperty to the BattleEntity.
        /// </summary>
        /// <param name="entity">The BattleEntity.</param>
        /// <param name="additionalProperty">The AdditionalProperty to add. If the BattleEntity already has it, it must have an integer value.</param>
        /// <param name="addVal">The amount to add to the value of this AdditionalProperty.</param>
        public static void AddIntAdditionalProperty(this BattleEntity entity, AdditionalProperty additionalProperty, int addVal)
        {
            int value = entity.EntityProperties.GetAdditionalProperty<int>(additionalProperty) + addVal;
            entity.EntityProperties.AddAdditionalProperty(additionalProperty, value);
        }

        /// <summary>
        /// A helper method for subtracting an integer AdditionalProperty from a BattleEntity.
        /// </summary>
        /// <param name="entity">The BattleEntity.</param>
        /// <param name="additionalProperty">The AdditionalProperty to subtract from. If the BattleEntity already has it, it must have an integer value.</param>
        /// <param name="subtractVal">The amount to subtract from the value of this AdditionalProperty.</param>
        /// <param name="removeWhenZeroOrLess">Whether to keep the property removed if the value afterwards is less than or equal to 0.</param>
        public static void SubtractIntAdditionalProperty(this BattleEntity entity, AdditionalProperty additionalProperty, int subtractVal, bool removeWhenZeroOrLess = true)
        {
            int value = entity.EntityProperties.GetAdditionalProperty<int>(additionalProperty) - subtractVal;
            entity.EntityProperties.RemoveAdditionalProperty(additionalProperty);
            if (removeWhenZeroOrLess == false || value > 0)
            {
                entity.EntityProperties.AddAdditionalProperty(additionalProperty, value);
            }
        }

        /// <summary>
        /// Adds a ShowHP AdditionalProperty to the BattleEntity.
        /// </summary>
        /// <param name="entity">The BattleEntity.</param>
        public static void AddShowHPProperty(this BattleEntity entity)
        {
            AddIntAdditionalProperty(entity, AdditionalProperty.ShowHP, 1);
        }

        /// <summary>
        /// Subtracts a ShowHP AdditionalProperty from the BattleEntity.
        /// </summary>
        /// <param name="entity">The BattleEntity.</param>
        public static void SubtractShowHPProperty(this BattleEntity entity)
        {
            SubtractIntAdditionalProperty(entity, AdditionalProperty.ShowHP, 1);
        }

        /// <summary>
        /// Tells if the BattleEntity is Invincible or not.
        /// </summary>
        /// <param name="entity">The BattleEntity to check for being Invincible.</param>
        /// <returns>true if the BattleEntity has the Invincible AdditionalProperty, otherwise false.</returns>
        public static bool IsInvincible(this BattleEntity entity)
        {
            return (entity.EntityProperties.GetAdditionalProperty<int>(AdditionalProperty.Invincible) > 0);
        }

        /// <summary>
        /// A helper method for adding and removing Status Effect immunities.
        /// All other fields on the BattleEntity's StatusProperties are preserved.
        /// </summary>
        /// <param name="entity">The BattleEntity.</param>
        /// <param name="statusType">The StatusType to add or remove an immunity for.</param>
        /// <param name="immune">Whether the BattleEntity is immune to the StatusType or not.</param>
        public static void AddRemoveStatusImmunity(this BattleEntity entity, StatusTypes statusType, bool immune)
        {
            //Get the StatusProperty
            StatusPropertyHolder statusProperty = entity.EntityProperties.GetStatusProperty(statusType);

            //Increase the immunity value by 1 if set to be immune, and decrease it by 1 when clearing immunity
            int immunity = statusProperty.Immunity;
            if (immune == true)
            {
                immunity += 1;
            }
            else
            {
                immunity -= 1;
            }

            //Fill in all of the existing StatusProperty's information to preserve it
            //The only difference is the immunity value
            entity.EntityProperties.AddStatusProperty(statusType,
                new StatusPropertyHolder(statusProperty.StatusPercentage, statusProperty.AdditionalTurns, immunity));
        }

        /// <summary>
        /// Gets the opposing EntityType of this BattleEntity.
        /// <para>This applies only to Players and Enemies.
        /// Neutral and other types of BattleEntities will simply return their own EntityTypes.</para>
        /// </summary>
        /// <param name="battleEntity"></param>
        /// <returns></returns>
        public static EntityTypes GetOpposingEntityType(this BattleEntity battleEntity)
        {
            //If this is a Player, return Enemy
            if (battleEntity.EntityType == EntityTypes.Player)
                return EntityTypes.Enemy;

            //If this is an Enemy, return Player
            if (battleEntity.EntityType == EntityTypes.Enemy)
                return EntityTypes.Player;

            //Otherwise return the default
            return battleEntity.EntityType;
        }

        /// <summary>
        /// A helper method to get the true target of this BattleEntity.
        /// If a BattleEntity is defending this one, thus taking its hits, and is active, then return that BattleEntity, otherwise return this one.
        /// </summary>
        /// <param name="battleEntity">The BattleEntity.</param>
        /// <returns>This BattleEntity if there is none defending this one and/or the defender is not active.
        /// Otherwise, the BattleEntity defending this one.</returns>
        public static BattleEntity GetTrueTarget(this BattleEntity battleEntity)
        {
            //The BattleEntity defending is active only on phases other than the entity's phase
            if (BattleManager.Instance.CurEntityPhase != battleEntity.EntityType)
            {
                //Check if we have a defender and return it if so
                BattleEntity defendedByEntity = battleEntity.EntityProperties.GetAdditionalProperty<BattleEntity>(AdditionalProperty.DefendedByEntity);
                if (defendedByEntity != null)
                {
                    return defendedByEntity;
                }
            }

            //In all other cases, return this BattleEntity
            return battleEntity;
        }

        #endregion

        #region List Extensions

        /// <summary>
        /// Removes an <see cref="IList{T}"/> of elements from the <see cref="List{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the List and IList.</typeparam>
        /// <param name="list">The <see cref="List{T}"/> to remove elements from.</param>
        /// <param name="elements">The elements to remove from the <see cref="List{T}"/>.</param>
        public static void RemoveFromList<T>(this List<T> list, IList<T> elements)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                list.Remove(elements[i]);
            }
        }

        #endregion

        #region Dictionary Extensions

        /// <summary>
        /// Copies unique keys and values from a <see cref="Dictionary{TKey, TValue}"/> into an existing <see cref="Dictionary{TKey, TValue}"/>.
        /// If the key already exists in the dictionary to copy to, it will replace it.
        /// </summary>
        /// <typeparam name="T">The type of the key.</typeparam>
        /// <typeparam name="U">The type of the value.</typeparam>
        /// <param name="dictCopiedTo">The Dictionary to copy values to.</param>
        /// <param name="dictCopiedFrom">The Dictionary to copy from.</param>
        public static void CopyDictionaryData<T,U>(this Dictionary<T,U> dictCopiedTo, Dictionary<T,U> dictCopiedFrom)
        {
            //Don't do anything if null, since there's nothing to copy from
            if (dictCopiedFrom == null) return;

            //Go through all keys and values
            foreach (KeyValuePair<T, U> kvPair in dictCopiedFrom)
            {
                T key = kvPair.Key;

                //Replace if already exists
                if (dictCopiedTo.ContainsKey(key) == true)
                {
                    dictCopiedTo.Remove(key);
                }

                dictCopiedTo.Add(key, kvPair.Value);
            }
        }

        /// <summary>
        /// Copies the keys and values from this <see cref="Dictionary{TKey, TValue}"/> into a new <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the key.</typeparam>
        /// <typeparam name="U">The type of the value.</typeparam>
        /// <param name="dictionary">The Dictionary to copy from.</param>
        /// <returns>A new <see cref="Dictionary{TKey, TValue}"/> with the same key-value pairs as <paramref name="dictionary"/>.</returns>
        public static Dictionary<T, U> CopyDictionary<T,U>(this Dictionary<T, U> dictionary)
        {
            Dictionary<T, U > newDict = new Dictionary<T, U>();
            
            //Copy all elements into the new Dictionary
            foreach (KeyValuePair<T, U> kvPair in dictionary)
            {
                newDict.Add(kvPair.Key, kvPair.Value);
            }

            return newDict;
        }

        #endregion

        #region SpriteRenderer Extensions

        public static void DrawSliced(this SpriteRenderer spriteRenderer, SlicedTexture2D slicedTex2D, Rectangle posAndSize,
            Color color, /*float rotation, Vector2 absOrigin, bool flipX, bool flipY,*/ float layer)
        {
            if (slicedTex2D == null || slicedTex2D.Tex == null)
            {
                Debug.LogError($"{nameof(slicedTex2D)} or its Texture is null, so nothing can be drawn!");
                return;
            }

            //NOTE: Do this until we figure out how to effectively apply these to sliced textures
            float rotation = 0f;
            Vector2 absOrigin = Vector2.Zero;
            bool flipX = false;
            bool flipY = false;

            for (int i = 0; i < slicedTex2D.Regions.Length; i++)
            {
                Rectangle destRect = slicedTex2D.GetRectForIndex(posAndSize, i);

                spriteRenderer.Draw(slicedTex2D.Tex, destRect, slicedTex2D.Regions[i], color, rotation, absOrigin, flipX, flipY, layer, true);
            }
        }

        public static void DrawUISliced(this SpriteRenderer spriteRenderer, SlicedTexture2D slicedTex2D, Rectangle posAndSize,
            Color color, /*float rotation, Vector2 absOrigin, bool flipX, bool flipY,*/ float layer)
        {
            if (slicedTex2D == null || slicedTex2D.Tex == null)
            {
                Debug.LogError($"{nameof(slicedTex2D)} or its Texture is null, so nothing can be drawn!");
                return;
            }

            //NOTE: Do this until we figure out how to effectively apply these to sliced textures
            float rotation = 0f;
            Vector2 absOrigin = Vector2.Zero;
            bool flipX = false;
            bool flipY = false;

            for (int i = 0; i < slicedTex2D.Regions.Length; i++)
            {
                Rectangle destRect = slicedTex2D.GetRectForIndex(posAndSize, i);

                //Sliced textures must use absolute origins since they can have vastly different sizes for each region
                spriteRenderer.DrawUI(slicedTex2D.Tex, destRect, slicedTex2D.Regions[i], color, rotation, absOrigin, flipX, flipY, layer, true);
            }
        }

        #endregion
    }
}
