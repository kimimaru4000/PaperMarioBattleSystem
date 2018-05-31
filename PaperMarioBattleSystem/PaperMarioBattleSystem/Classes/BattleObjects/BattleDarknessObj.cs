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
    /// An object associated with darkness in battle. It handles marking BattleEntities as targetable and untargetable when there
    /// are light sources present.
    /// <para>In the PM games, Mario and his Partner are always targetable in darkness. As such, this doesn't handle Players.</para>
    /// </summary>
    public class BattleDarknessObj : EntityListenerObj
    {
        /* What we need to do:
         * 1. We need all the BattleEntities in battle first
         * 2. Check which ones are light sources and add them to a separate list, paired with their light radii
         *    -Light sources are always targetable, so they don't need to be considered for untargeting
         * 3. Check if a light source is no longer a light source or its light radius was changed; if so, recalculate the targeting
         * 4. Check if non-light sources are now light sources; if so, recalculate the targeting
         * 5. When checking the targeting, (for now; possibly hurtboxes later) check if the BattleEntity's BattlePosition is inside the radius of a light source
         */

        /// <summary>
        /// The list of BattleEntities that aren't light sources.
        /// </summary>
        private List<BattleEntity> NonLightSources = new List<BattleEntity>();

        /// <summary>
        /// The list of currently untargetable BattleEntities due to the darkness.
        /// </summary>
        private List<BattleEntity> UntargetableEntities = new List<BattleEntity>();

        /// <summary>
        /// The Battle Entities that act as light sources.
        /// They are coupled with their light radii.
        /// </summary>
        public readonly List<LightSourceHolder> LightSources = new List<LightSourceHolder>();

        public BattleDarknessObj(BattleManager bManager) : base(bManager)
        {
            Initialize();

            ListenToEntityEvents();
        }

        public override void CleanUp()
        {
            RetargetEntities();

            NonLightSources.Clear();
            LightSources.Clear();

            base.CleanUp();
        }

        private void Initialize()
        {
            //Add all entities, then call the checker
            BManager.GetAllBattleEntities(NonLightSources, null);

            CheckUpdates();
            RecalculateTargets();
        }

        /// <summary>
        /// Retargets any BattleEntities made untargetable due to the darkness and clears the untargetable list.
        /// </summary>
        private void RetargetEntities()
        {
            //Re-target entities
            for (int i = 0; i < UntargetableEntities.Count; i++)
            {
                UntargetableEntities[i].SubtractIntAdditionalProperty(Enumerations.AdditionalProperty.Untargetable, 1);
                UntargetableEntities.RemoveAt(i);
                i--;
            }
        }

        /// <summary>
        /// Makes a BattleEntity untargetable and adds it to the untargetable list.
        /// </summary>
        /// <param name="entity">The BattleEntity to make untargetable.</param>
        private void UntargetEntity(BattleEntity entity)
        {
            if (entity == null) return;

            entity.AddIntAdditionalProperty(Enumerations.AdditionalProperty.Untargetable, 1);
            UntargetableEntities.Add(entity);
        }

        /// <summary>
        /// Recalculates which BattleEntities should be targetable or untargetable based on whether they're in light or not.
        /// </summary>
        private void RecalculateTargets()
        {
            //Re-target all entities, then recalculate which ones are untargetable
            RetargetEntities();

            //Go through all entities and check if they're in light
            for (int i = 0; i < NonLightSources.Count; i++)
            {
                //Skip over players, as they're always targetable
                if (NonLightSources[i].EntityType == Enumerations.EntityTypes.Player)
                    continue;

                Circle nonLightCircle = new Circle(NonLightSources[i].BattlePosition, 0d);

                bool inLight = false;

                //Go through all light sources to check the light
                for (int j = 0; j < LightSources.Count; j++)
                {
                    Circle lightCircle = new Circle(LightSources[j].Entity.BattlePosition, LightSources[j].LightRadius);

                    //Check if the entity is in the light and break if so
                    if (lightCircle.Intersects(nonLightCircle) == true)
                    {
                        inLight = true;
                        break;
                    }
                }

                //If not in light, make it untargetable
                if (inLight == false)
                {
                    UntargetEntity(NonLightSources[i]);
                }
            }
        }

        private void CheckUpdates()
        {
            //A flag indicating whether we should recalculate targets or not
            bool shouldRecalculate = false;

            for (int i = 0; i < LightSources.Count; i++)
            {
                BattleEntity lightSourceEntity = LightSources[i].Entity;

                //If the BattleEntity is no longer a light source, remove it from the light source list,
                //add it to the non light source list, and say we should recalculate the targeting
                if (IsLightSource(lightSourceEntity) == false)
                {
                    shouldRecalculate = true;

                    NonLightSources.Add(lightSourceEntity);
                    LightSources.RemoveAt(i);
                    i--;
                }
                //If it's still a light source, check if it has an updated light radius
                else
                {
                    double radius = GetLightRadius(lightSourceEntity);

                    //Update the value if it's not the same, and mark for recalculating
                    if (radius != LightSources[i].LightRadius)
                    {
                        LightSources[i].LightRadius = radius;

                        shouldRecalculate = true;
                    }
                }
            }

            //Check if any non-light sources are now light sources
            for (int i = 0; i < NonLightSources.Count; i++)
            {
                BattleEntity nonLightSource = NonLightSources[i];

                //If it's a light source, add it to the light source list, remove it from the non light source list, and mark it to recalculate targeting
                //Recalculating will handle removing it from the untargetable list if it's in it
                if (IsLightSource(nonLightSource) == true)
                {
                    LightSources.Add(new LightSourceHolder(nonLightSource, GetLightRadius(nonLightSource)));

                    NonLightSources.RemoveAt(i);
                    i--;

                    shouldRecalculate = true;
                }
            }

            //If we should recalculate targets, do so
            if (shouldRecalculate == true)
            {
                RecalculateTargets();
            }
        }

        //NOTE: This might be better suited for checking on any turn start or turn end
        //BattleEntities can't target others when it's not their turn anyway
        public override void Update()
        {
            CheckUpdates();
        }

        private bool IsLightSource(BattleEntity entity)
        {
            if (entity == null) return false;

            return entity.EntityProperties.HasAdditionalProperty(Enumerations.AdditionalProperty.LightSource);
        }

        private double GetLightRadius(BattleEntity entity)
        {
            if (entity == null) return 0d;

            return entity.EntityProperties.GetAdditionalProperty<double>(Enumerations.AdditionalProperty.LightSource);
        }

        protected override void EntityAdded(BattleEntity entity)
        {
            //If the entity is a light source, add it and re-handle the targets
            if (IsLightSource(entity) == true)
            {
                LightSources.Add(new LightSourceHolder(entity, GetLightRadius(entity)));

                RecalculateTargets();
            }
            //Otherwise, add it and re-handle the targets
            else
            {
                NonLightSources.Add(entity);

                RecalculateTargets();
            }
        }

        protected override void EntityRemoved(BattleEntity entity)
        {
            //Check if the entity removed was a light source
            if (IsLightSource(entity) == true)
            {
                bool removed = false;

                for (int i = 0; i < LightSources.Count; i++)
                {
                    if (entity == LightSources[i].Entity)
                    {
                        LightSources.RemoveAt(i);
                        removed = true;
                        break;
                    }
                }

                if (removed == true)
                {
                    RecalculateTargets();
                }
            }
            //Otherwise, remove it from the non light source list
            else
            {
                bool removed = NonLightSources.Remove(entity);

                if (removed == true)
                {
                    RecalculateTargets();
                }
            }
        }

        public class LightSourceHolder
        {
            public BattleEntity Entity;
            public double LightRadius;

            public LightSourceHolder(BattleEntity entity, double lightRadius)
            {
                Entity = entity;
                LightRadius = lightRadius;
            }
        }
    }
}
