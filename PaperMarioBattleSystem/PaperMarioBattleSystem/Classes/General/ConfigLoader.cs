using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Xna.Framework;
using PaperMarioBattleSystem.Extensions;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    public static class ConfigLoader
    {
        //Constants for helping parse
        private const string RootNode = "battle";
        private const string BattlePropertiesNode = "battleproperties";
        private const string BattleSettingAttribute = "battlesetting";
        private const string RunnableAttribute = "runnable";

        private const string EntityNode = "battleentity";

        private const string BadgeNode = "badge";
        private const string ItemNode = "item";

        private const string MarioType = "mario";

        private const string TypeAttribute = "type";

        private const string HPAttribute = "hp";
        private const string FPAttribute = "fp";
        private const string AttackAttribute = "attack";
        private const string DefenseAttribute = "defense";
        private const string BootsAttribute = "boots";
        private const string HammerAttribute = "hammer";
        private const string SPAttribute = "sp";

        /// <summary>
        /// Loads the Config.xml file from the designated path. The values are used to set up the battle.
        /// <para>This may be revised later to streamline loading.</para>
        /// </summary>
        /// <param name="filePath">The path to the config file.</param>
        /// <param name="battleProperties">The BattleProperties to set.</param>
        /// <param name="mario">Mario, whose stats may be modified.</param>
        /// <param name="enemies">The enemies to start the battle with.</param>
        /// <returns>true if the config was loaded without any errors, otherwise false.</returns>
        public static bool LoadConfig(in string filePath, ref BattleGlobals.BattleProperties battleProperties, BattleMario mario, List<BattleEntity> enemies)
        {
            //Ensure the file exists first
            if (File.Exists(filePath) == false)
            {
                Debug.LogError($"Could not find Config file at path {filePath}!");
                return false;
            }

            //Catch exceptions when loading the config
            //The config isn't necessary for the game to run, so we can initialize default settings if it failed to load
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(filePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error when reading Config: {e.Message}");
                return false;
            }

            //Try to load the root node
            XmlNode root = null;
            foreach (XmlNode child in doc.ChildNodes)
            {
                if (child.Name.ToLower() == RootNode)
                {
                    root = child;
                    break;
                }
            }

            //There's no root node, so we can't proceed
            if (root == null)
            {
                Debug.LogError($"No root element {RootNode} found in config!");
                return false;
            }

            //Go through the children and parse them out
            XmlNodeList nodelist = root.ChildNodes;

            for (int i = 0; i < nodelist.Count; i++)
            {
                XmlNode childNode = nodelist[i];

                string nameToLower = childNode.Name.ToLower();

                //If it's the battle's properties, parse it
                if (nameToLower == BattlePropertiesNode)
                {
                    battleProperties = ReadBattleProperties(childNode);
                }
                //Otherwise if it's a BattleEntity, parse that
                else if (nameToLower == EntityNode)
                {
                    ReadBattleEntity(childNode, mario, enemies);
                }
            }

            return true;
        }

        private static BattleGlobals.BattleProperties ReadBattleProperties(in XmlNode childNode)
        {
            //Set up new battle properties
            BattleGlobals.BattleProperties properties = new BattleGlobals.BattleProperties();

            XmlAttributeCollection attributes = childNode.Attributes;

            //Go through the attributes and parse the information
            foreach (XmlAttribute attr in attributes)
            {
                string nameToLower = attr.Name.ToLower();

                if (nameToLower == BattleSettingAttribute)
                {
                    Enum.TryParse(attr.Value, out properties.BattleSetting);
                }
                else if (nameToLower == RunnableAttribute)
                {
                    bool.TryParse(attr.Value, out properties.Runnable);
                }
            }

            return properties;
        }

        /// <summary>
        /// Parses a BattleEntity from the config.
        /// </summary>
        /// <param name="node">The XmlNode containing the BattleEntity's information.</param>
        /// <param name="mario">The Mario reference. Mario is already instantiated, so only his stats are modified here.</param>
        /// <param name="enemies">The list of Neutral BattleEntities or Enemies to add instantiated ones to.</param>
        private static void ReadBattleEntity(in XmlNode node, BattleMario mario, List<BattleEntity> enemies)
        {
            //Define necessary information
            string typeName = string.Empty;

            bool isMario = false;
            List<object> parameters = null;

            //Go through the attributes
            foreach (XmlAttribute attribute in node.Attributes)
            {
                string nameToLower = attribute.Name.ToLower();

                //We found a Type name for this BattleEntity; see what it is
                if (nameToLower == TypeAttribute)
                {
                    string toLower = attribute.Value.ToLower();

                    typeName = $"{nameof(PaperMarioBattleSystem)}.{toLower}";

                    //For Mario, read in other information; everyone else exits
                    if (toLower == MarioType)
                    {
                        isMario = true;
                    }
                    else
                    {
                        parameters = new List<object>();
                    }
                }
                //Parse stats for Mario
                else if (isMario == true)
                {
                    int val = 0;

                    if (int.TryParse(attribute.Value, out val) == true)
                    {
                        if (nameToLower == HPAttribute) mario.MStats.MaxHP = mario.MStats.HP = val;
                        else if (nameToLower == FPAttribute) mario.MStats.MaxFP = mario.MStats.FP = val;
                        else if (nameToLower == AttackAttribute) mario.MStats.BaseAttack = val;
                        else if (nameToLower == DefenseAttribute) mario.MStats.BaseDefense = val;
                        else if (nameToLower == BootsAttribute) mario.MStats.BootLevel = (EquipmentGlobals.BootLevels)val;
                        else if (nameToLower == HammerAttribute) mario.MStats.HammerLevel = (EquipmentGlobals.HammerLevels)val;
                        else if (nameToLower == SPAttribute) mario.MStats.SSStarPower.GainStarPower(val);
                    }
                }
                //Parse constructor information for other BattleEntities
                else
                {
                    object primitive = ReadPrimitiveType(attribute);
                    parameters.Add(primitive);
                }
            }

            //Return since Mario is created beforehand, as he's required for battle
            if (isMario == true)
            {
                ReadBattleEntityEquipment(mario, node);
                return;
            }

            //Try to instantiate the BattleEntity
            BattleEntity entity = InstantiateObject<BattleEntity>(typeName, (parameters.Count == 0) ? Array.Empty<object>() : parameters.ToArray());

            //We couldn't create this BattleEntity, so return
            if (entity == null)
                return;

            //Check for a Partner
            if (entity.EntityType == Enumerations.EntityTypes.Player)
            {
                //Add the Partner to the Partner inventory
                BattlePlayer player = (BattlePlayer)entity;
                if (player.PlayerType == Enumerations.PlayerTypes.Partner)
                {
                    Inventory.Instance.partnerInventory.AddPartner(player as BattlePartner);
                }
            }
            else
            {
                enemies.Add(entity);
            }

            //Read the equipment for this BattleEntity
            ReadBattleEntityEquipment(entity, node);
        }

        /// <summary>
        /// Parses equipment, such as Items and Badges, and equips it to a BattleEntity.
        /// <para>For Enemies, only the last one encountered will be held.</para>
        /// </summary>
        /// <param name="entity">The BattleEntity to equip.</param>
        /// <param name="node">The XmlNode containing the equipment information.</param>
        private static void ReadBattleEntityEquipment(in BattleEntity entity, in XmlNode node)
        {
            //Go through all children
            foreach (XmlNode childNode in node.ChildNodes)
            {
                string nameToLower = childNode.Name.ToLower();

                //Equip a Badge
                if (nameToLower == BadgeNode)
                {
                    ReadBadge(entity, childNode);
                }
                //Equip an Item
                else if (nameToLower == ItemNode)
                {
                    ReadItem(entity, childNode);
                }
            }
        }

        /// <summary>
        /// Parses a Badge and equips it to a BattleEntity.
        /// </summary>
        /// <param name="entity">The BattleEntity to equip the Badge to.</param>
        /// <param name="childNode">The XmlNode containing the Badge information.</param>
        private static void ReadBadge(in BattleEntity entity, in XmlNode childNode)
        {
            //Return if the BattleEntity isn't a Player nor an Enemy; no one else has Badges, so parsing it is pointless
            if (entity.EntityType != Enumerations.EntityTypes.Player && entity.EntityType != Enumerations.EntityTypes.Enemy) return;

            Badge badge = (Badge)ReadCollectible(childNode);

            //Return if null
            if (badge == null)
                return;

            //Add it to the Inventory if it's a Player
            if (entity.EntityType == Enumerations.EntityTypes.Player)
            {
                Inventory.Instance.AddBadge(badge);

                //Activate and equip the Badge
                BattlePlayer player = (BattlePlayer)entity;
                player.ActivateAndEquipBadge(badge);
            }
            //If it's an Enemy, set it as its held Badge
            else
            {
                BattleEnemy enemy = (BattleEnemy)entity;
                enemy.SetHeldCollectible(badge);
            }
        }

        /// <summary>
        /// Parses an Item and gives it to a BattleEntity.
        /// </summary>
        /// <param name="entity">The BattleEntity to give the Item to.</param>
        /// <param name="childNode">The XmlNode containing the Item information.</param>
        private static void ReadItem(in BattleEntity entity, in XmlNode childNode)
        {
            //Return if the BattleEntity isn't a Player nor an Enemy; no one else has Items, so parsing it is pointless
            if (entity.EntityType != Enumerations.EntityTypes.Player && entity.EntityType != Enumerations.EntityTypes.Enemy) return;

            Item item = (Item)ReadCollectible(childNode);

            //Return if null
            if (item == null)
                return;

            //Add it to the Inventory if it's a Player
            if (entity.EntityType == Enumerations.EntityTypes.Player)
            {
                Inventory.Instance.AddItem(item);
            }
            //If it's an Enemy, set it as its held Item
            else
            {
                BattleEnemy enemy = (BattleEnemy)entity;
                enemy.SetHeldCollectible(item);
            }
        }

        /// <summary>
        /// Parses a Collectible and tries to instantiate it based on the information provided.
        /// </summary>
        /// <param name="childNode">The XmlNode containing the Collectible information.</param>
        /// <returns>A Collectible with the supplied information.
        /// If it fails to instantiate the Collectible, then null.</returns>
        private static Collectible ReadCollectible(in XmlNode childNode)
        {
            XmlAttributeCollection attributes = childNode.Attributes;

            string typeName = string.Empty;

            List<object> parameters = new List<object>();

            //Go through all attributes
            foreach (XmlAttribute attr in attributes)
            {
                string nameToLower = attr.Name.ToLower();

                if (nameToLower == TypeAttribute)
                {
                    //Get the Type name with the current assembly
                    typeName = $"{nameof(PaperMarioBattleSystem)}.{attr.Value.ToLower()}";
                }
                //Parse constructor parameters
                else
                {
                    object primitive = ReadPrimitiveType(attr);
                    parameters.Add(primitive);
                }
            }

            //Instantiate the Collectible
            return InstantiateObject<Collectible>(typeName, (parameters.Count == 0) ? Array.Empty<object>() : parameters.ToArray());
        }

        /// <summary>
        /// Tries to instantiate an object based off its type name and constructor arguments.
        /// </summary>
        /// <typeparam name="T">The type to instantiate.</typeparam>
        /// <param name="typeName">The fully qualified name of the type.</param>
        /// <param name="constructorArgs">The constructor arguments required to instantiate the object.</param>
        /// <returns>An object of type <typeparamref name="T"/> with the supplied parameters.
        /// If it fails to instantiate the object, then null.</returns>
        private static T InstantiateObject<T>(in string typeName, in object[] constructorArgs) where T : class
        {
            //Try to create the object out of this information
            try
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                Type type = asm.GetType(typeName, true, true);

                T instantiatedObj = (T)Activator.CreateInstance(type, constructorArgs);
                return instantiatedObj;
            }
            catch (Exception e)
            {
                //We can't create the object
                Debug.LogError($"Error parsing object: {e.Message}");

                return null;
            }
        }

        /// <summary>
        /// Reads and returns a primitive type from an XmlAttribute.
        /// </summary>
        /// <param name="attribute">The XmlAttribute.</param>
        /// <returns>An object containing a primitive type, otherwise null.</returns>
        private static object ReadPrimitiveType(XmlAttribute attribute)
        {
            string nameToLower = attribute.Name.ToLower();

            //Try to convert standard types
            //Converting others will be trickier with the way we're parsing the config, so this will need to change if we require it
            //Most objects take in only primitives, so it will work for now
            if (nameToLower == "string")
            {
                return attribute.Value;
            }
            else if (nameToLower == "bool")
            {
                bool.TryParse(attribute.Value, out bool success);
                return success;
            }
            else if (nameToLower == "byte")
            {
                byte.TryParse(attribute.Value, out byte success);
                return success;
            }
            else if (nameToLower == "sbyte")
            {
                sbyte.TryParse(attribute.Value, out sbyte success);
                return success;
            }
            else if (nameToLower == "char")
            {
                char.TryParse(attribute.Value, out char success);
                return success;
            }
            else if (nameToLower == "short")
            {
                short.TryParse(attribute.Value, out short success);
                return success;
            }
            else if (nameToLower == "ushort")
            {
                ushort.TryParse(attribute.Value, out ushort success);
                return success;
            }
            else if (nameToLower == "int")
            {
                int.TryParse(attribute.Value, out int success);
                return success;
            }
            else if (nameToLower == "uint")
            {
                uint.TryParse(attribute.Value, out uint success);
                return success;
            }
            else if (nameToLower == "long")
            {
                long.TryParse(attribute.Value, out long success);
                return success;
            }
            else if (nameToLower == "ulong")
            {
                ulong.TryParse(attribute.Value, out ulong success);
                return success;
            }
            else if (nameToLower == "float")
            {
                float.TryParse(attribute.Value, out float success);
                return success;
            }
            else if (nameToLower == "decimal")
            {
                decimal.TryParse(attribute.Value, out decimal success);
                return success;
            }
            else if (nameToLower == "double")
            {
                double.TryParse(attribute.Value, out double success);
                return success;
            }
            
            return null;
        }
    }
}
