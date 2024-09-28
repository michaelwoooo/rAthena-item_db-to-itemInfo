﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Text;

public class Converter : MonoBehaviour
{
    const string CREATOR_URL = "https://kanintemsrisukgames.wordpress.com/2019/04/05/support-kt-games/";
    const float ONE_SECOND = 1;
    const int ITEM_DEBUG_PRICE = 10000000;
    const int PET_TAME_PRICE = 500000;
    const int PET_EGG_PRICE = 1000000;
    const int PET_ARMOR_PRICE = 10000000;
    const int FASHION_PRICE = 50000000;
    const int BUFF_PRICE = 1000000;

    const string CLASS_2_SKILL_ITEM_REQ_PRICE = "3000";
    const string CLASS_3_SKILL_ITEM_REQ_PRICE = "9000";
    const string CLASS_4_SKILL_ITEM_REQ_PRICE = "25000";
    const string STONE_PRICE = "5";
    const string YGGDRASIL_SEED_PRICE = "9000000";
    const string YGGDRASIL_BERRY_PRICE = "90000000";
    const string ELEMENT_CONVERTER_PRICE = "15000";
    const string PILE_BUNKER_PRICE = "300000";
    const string POISON_BOTTLE_PRICE = "15000";
    const string SPELL_BOOK_TIER_1_PRICE = "100000";
    const string SPELL_BOOK_TIER_2_PRICE = "500000";
    const string SPELL_BOOK_TIER_3_PRICE = "1000000";
    const string POTION_BOOK_PRICE = "1000";
    const string LUN_ANIMA_RUNESTONE_PRICE = "100000";

    [SerializeField] bool _isItemLink = false;
    public bool IsItemLink { set { _isItemLink = value; } }
    [SerializeField] bool _isItemUnconditional = false;
    public bool IsItemUnconditional { set { _isItemUnconditional = value; } }
    [SerializeField] bool _isSkipEquipLevel = false;
    public bool IsSkipEquipLevel { set { _isSkipEquipLevel = value; } }
    [SerializeField] bool _isSkipNormalEquipEtcCombo = false;
    public bool IsSkipNormalEquipEtcCombo { set { _isSkipNormalEquipEtcCombo = value; } }
    [SerializeField] bool _isRemoveBrackets = false;
    public bool IsRemoveBrackets { set { _isRemoveBrackets = value; } }

    [Serializable]
    public class ReplaceVariable
    {
        public string variableName;
        public string descriptionConverted;
    }

    bool _isFastConvert;
    bool _isFilesError;
    string _errorLog;

    /// <summary>
    /// Button to open item generator
    /// </summary>
    [SerializeField] Button _btnItemGenerator;
    /// <summary>
    /// Button to preview item information
    /// </summary>
    [SerializeField] Button _btnItemPreview;
    /// <summary>
    /// Button to generate captcha
    /// </summary>
    [SerializeField] Button _btnCaptchaGenerator;
    /// <summary>
    /// Button to start convert
    /// </summary>
    [SerializeField] Button _btnConvert;
    [SerializeField] Button _btnFastConvert;
    /// <summary>
    /// Button to see creator
    /// </summary>
    [SerializeField] Button _btnCreator;
    /// <summary>
    /// Convert button gameObject
    /// </summary>
    [SerializeField] GameObject[] _objectsToHideWhenConverterStart;
    /// <summary>
    /// Progress gameObject
    /// </summary>
    [SerializeField] GameObject _objConvertProgression;
    /// <summary>
    /// Text convert progression
    /// </summary>
    [SerializeField] Text _txtConvertProgression;
    /// <summary>
    /// Hardcode item scripts description
    /// </summary>
    [SerializeField] HardcodeItemScripts _hardcodeItemScripts;
    /// <summary>
    /// Localization
    /// </summary>
    [SerializeField] Localization _localization;
    /// <summary>
    /// Captcha Generator
    /// </summary>
    [SerializeField] CaptchaGenerator _captchaGenerator;
    /// <summary>
    /// Item Preview
    /// </summary>
    [SerializeField] ItemPreview _itemPreview;
    /// <summary>
    /// Item Generator
    /// </summary>
    [SerializeField] ItemGenerator _itemGenerator;

    // Settings

    /// <summary>
    /// Is print out zero value? (Example: Attack: 0)
    /// </summary>
    [SerializeField] bool _isZeroValuePrintable;
    public bool IsZeroValuePrintAble { set { _isZeroValuePrintable = value; } }
    /// <summary>
    /// Only read text asset from 'item_db_test.txt'
    /// </summary>
    [SerializeField] bool _isOnlyUseTestTextAsset;
    public bool IsOnlyUseTestTextAsset { set { _isOnlyUseTestTextAsset = value; } }
    /// <summary>
    /// Only read text asset from 'item_db_custom.txt'
    /// </summary>
    [SerializeField] bool _isOnlyUseCustomTextAsset;
    public bool IsOnlyUseCustomTextAsset { set { _isOnlyUseCustomTextAsset = value; } }
    /// <summary>
    /// Is random resource name for all item?
    /// </summary>
    [SerializeField] bool _isRandomResourceName;
    public bool IsRandomResourceName { set { _isRandomResourceName = value; } }
    /// <summary>
    /// Is only random resource name for custom item?
    /// </summary>
    [SerializeField] bool _isRandomResourceNameForCustomTextAssetOnly;
    public bool IsRandomResourceNameForCustomTextAssetOnly { set { _isRandomResourceNameForCustomTextAssetOnly = value; } }
    /// <summary>
    /// Is use new line instead of , for available job?
    /// </summary>
    [SerializeField] bool _isUseNewLineInsteadOfCommaForAvailableJob;
    public bool IsUseNewLineInsteadOfCommaForAvailableJob { set { _isUseNewLineInsteadOfCommaForAvailableJob = value; } }
    /// <summary>
    /// Is use new line instead of , for available class?
    /// </summary>
    [SerializeField] bool _isUseNewLineInsteadOfCommaForAvailableClass;
    public bool IsUseNewLineInsteadOfCommaForAvailableClass { set { _isUseNewLineInsteadOfCommaForAvailableClass = value; } }
    /// <summary>
    /// Is print item id?
    /// </summary>
    [SerializeField] bool _isHideItemId;
    public bool IsHideItemId { set { _isHideItemId = value; } }
    /// <summary>
    /// Is print sub-type?
    /// </summary>
    [SerializeField] bool _isHideSubType;
    public bool IsHideSubType { set { _isHideSubType = value; } }
    /// <summary>
    /// Is equipment type will print as not have any value?
    /// </summary>
    [SerializeField] bool _isEquipmentNoValue;
    public bool IsEquipmentNoValue { set { _isEquipmentNoValue = value; } }
    /// <summary>
    /// Is non-usable item will print as not have any bonuses and combos?
    /// </summary>
    [SerializeField] bool _isItemNoBonus;
    public bool IsItemNoBonus { set { _isItemNoBonus = value; } }
    /// <summary>
    /// Is enchantment able to use?
    /// </summary>
    [SerializeField] bool _isEnchantmentAbleToUse;
    public bool IsEnchantmentAbleToUse { set { _isEnchantmentAbleToUse = value; } }
    /// <summary>
    /// Is hide refinable?
    /// </summary>
    [SerializeField] bool _isHideRefinable;
    public bool IsHideRefinable { set { _isHideRefinable = value; } }
    /// <summary>
    /// Is hide gradable?
    /// </summary>
    [SerializeField] bool _isHideGradable;
    public bool IsHideGradable { set { _isHideGradable = value; } }

    /// <summary>
    /// Container for 1 item, use while parsing from item_db
    /// </summary>
    ItemContainer _itemContainer = new ItemContainer();
    /// <summary>
    /// Container that hold all item ids (Split by item type)
    /// </summary>
    ItemListContainer _itemListContainer = new ItemListContainer();
    /// <summary>
    /// Container that hold all resource names (Spilt by equipment type)
    /// </summary>
    ResourceContainer _resourceContainer = new ResourceContainer();
    /// <summary>
    /// Class numbers holder
    /// </summary>
    Dictionary<int, string> _classNumberDatabases = new Dictionary<int, string>();
    /// <summary>
    /// Combos holder
    /// </summary>
    List<ComboDatabase> _comboDatabases = new List<ComboDatabase>();
    /// <summary>
    /// Items holder
    /// </summary>
    Dictionary<int, ItemDatabase> _itemDatabases = new Dictionary<int, ItemDatabase>();
    /// <summary>
    /// Aegis name holder
    /// </summary>
    Dictionary<string, int> _aegisNameDatabases = new Dictionary<string, int>();
    /// <summary>
    /// Monsters holder
    /// </summary>
    Dictionary<int, MonsterDatabase> _monsterDatabases = new Dictionary<int, MonsterDatabase>();
    List<int> _monsterIds = new List<int>();
    /// <summary>
    /// Monsters holder
    /// </summary>
    Dictionary<string, int> _monsterIdsByAegisName = new Dictionary<string, int>();
    /// <summary>
    /// Resources holder
    /// </summary>
    Dictionary<int, string> _resourceDatabases = new Dictionary<int, string>();
    /// <summary>
    /// Skills holder
    /// </summary>
    Dictionary<int, SkillDatabase> _skillDatabases = new Dictionary<int, SkillDatabase>();
    Dictionary<string, SkillDatabase> _skillDatabasesByName = new Dictionary<string, SkillDatabase>();
    /// <summary>
    /// Skill name holder
    /// </summary>
    Dictionary<string, int> _skillNameDatabases = new Dictionary<string, int>();
    /// <summary>
    /// Container that hold all item information to print out (In order)
    /// </summary>
    List<ItemContainer> _itemContainers = new List<ItemContainer>();
    /// <summary>
    /// Item script copier holder
    /// </summary>
    Dictionary<int, int> _itemScriptCopierDatabases = new Dictionary<int, int>();
    /// <summary>
    /// Item container holder (To get script directly instead of loop all item)
    /// </summary>
    Dictionary<int, ItemContainer> _itemContaianerDatabases = new Dictionary<int, ItemContainer>();

    List<ReplaceVariable> _replaceVariables = new List<ReplaceVariable>();
    List<string> _arrayNames = new List<string>();

    List<SkillDatabase> _allSkillDatabases = new List<SkillDatabase>();
    List<string> _allLearnableSkillDatabases = new List<string>();

    EasyItemBuilderDatabase _easyItemBuilderDatabase = new EasyItemBuilderDatabase();

    string _currentCombo = string.Empty;

    List<string> errorResourceNames = new List<string>();

    List<string> _petTamingItemIds = new List<string>();

    void Start()
    {
        _isFastConvert = false;

        _btnItemGenerator.onClick.AddListener(OnItemGeneratorButtonTap);
        _btnItemPreview.onClick.AddListener(OnItemPreviewButtonTap);
        _btnCaptchaGenerator.onClick.AddListener(OnCaptchaGeneratorButtonTap);
        _btnConvert.onClick.AddListener(OnConvertButtonTap);
        _btnFastConvert.onClick.AddListener(OnFastConvertButtonTap);
        _btnCreator.onClick.AddListener(OnCreatorButtonTap);

        _objConvertProgression.SetActive(false);
        for (int i = 0; i < _objectsToHideWhenConverterStart.Length; i++)
            _objectsToHideWhenConverterStart[i].SetActive(true);
    }
    /// <summary>
    /// Call when item generator button has been tap
    /// </summary>
    void OnItemGeneratorButtonTap()
    {
        _itemGenerator.Show();
    }
    /// <summary>
    /// Call when item preview button has been tap
    /// </summary>
    void OnItemPreviewButtonTap()
    {
        _itemPreview.Setup();
    }
    /// <summary>
    /// Call when captcha generator button has been tap
    /// </summary>
    void OnCaptchaGeneratorButtonTap()
    {
        _captchaGenerator.Generate();
    }
    /// <summary>
    /// Call when creator button has been tap
    /// </summary>
    void OnCreatorButtonTap()
    {
        Application.OpenURL(CREATOR_URL);
    }
    void OnFastConvertButtonTap()
    {
        _isFastConvert = true;

        OnConvertButtonTap();
    }
    /// <summary>
    /// Call when convert button has been tap
    /// </summary>
    void OnConvertButtonTap()
    {
        _petTamingItemIds = new List<string>();

        errorResourceNames = new List<string>();

        _easyItemBuilderDatabase = new EasyItemBuilderDatabase();

        for (int i = 0; i < _objectsToHideWhenConverterStart.Length; i++)
            _objectsToHideWhenConverterStart[i].SetActive(false);

        _objConvertProgression.SetActive(true);

        _isFilesError = false;

        _txtConvertProgression.text = _localization.GetTexts(Localization.CONVERT_PROGRESSION_START) + "..";

        Debug.Log(DateTime.Now);

        Invoke("FetchingData", 0.1f);
    }
    void FetchingData()
    {
        _txtConvertProgression.text = _localization.GetTexts(Localization.CONVERT_PROGRESSION_FETCHING_ITEM) + "..";

        FetchItem();

        if (_isFilesError)
        {
            _txtConvertProgression.text = "<color=red>" + _localization.GetTexts(Localization.ERROR) + "</color>: " + _errorLog;

            return;
        }

        _txtConvertProgression.text = _localization.GetTexts(Localization.CONVERT_PROGRESSION_FETCHING_RESOURCE_NAME) + "..";

        FetchResourceName();

        if (_isFilesError)
        {
            _txtConvertProgression.text = "<color=red>" + _localization.GetTexts(Localization.ERROR) + "</color>: " + _errorLog;

            return;
        }

        _txtConvertProgression.text = _localization.GetTexts(Localization.CONVERT_PROGRESSION_FETCHING_SKILL) + "..";

        FetchSkill();

        if (_isFilesError)
        {
            _txtConvertProgression.text = "<color=red>" + _localization.GetTexts(Localization.ERROR) + "</color>: " + _errorLog;

            return;
        }

        if (!_isFastConvert)
            FetchLearnableSkill();

        if (_isFilesError)
        {
            _txtConvertProgression.text = "<color=red>" + _localization.GetTexts(Localization.ERROR) + "</color>: " + _errorLog;

            return;
        }

        _txtConvertProgression.text = _localization.GetTexts(Localization.CONVERT_PROGRESSION_FETCHING_CLASS_NUMBER) + "..";

        FetchClassNumber();

        if (_isFilesError)
        {
            _txtConvertProgression.text = "<color=red>" + _localization.GetTexts(Localization.ERROR) + "</color>: " + _errorLog;

            return;
        }

        _txtConvertProgression.text = _localization.GetTexts(Localization.CONVERT_PROGRESSION_FETCHING_CLASS_MONSTER) + "..";

        FetchMonster();

        if (_isFilesError)
        {
            _txtConvertProgression.text = "<color=red>" + _localization.GetTexts(Localization.ERROR) + "</color>: " + _errorLog;

            return;
        }

        _txtConvertProgression.text = _localization.GetTexts(Localization.CONVERT_PROGRESSION_FETCHING_ITEM_COMBO) + "..";

        FetchCombo();

        if (_isFilesError)
        {
            _txtConvertProgression.text = "<color=red>" + _localization.GetTexts(Localization.ERROR) + "</color>: " + _errorLog;

            return;
        }

        _txtConvertProgression.text = _localization.GetTexts(Localization.CONVERT_PROGRESSION_FETCHING_RESOURCE_NAME_WITH_TYPE) + "..";

        FetchResourceNameWithType();

        if (_isFilesError)
        {
            _txtConvertProgression.text = "<color=red>" + _localization.GetTexts(Localization.ERROR) + "</color>: " + _errorLog;

            return;
        }

        _txtConvertProgression.text = _localization.GetTexts(Localization.CONVERT_PROGRESSION_FETCHING_PET) + "..";

        FetchPet();

        if (_isFilesError)
        {
            _txtConvertProgression.text = "<color=red>" + _localization.GetTexts(Localization.ERROR) + "</color>: " + _errorLog;

            return;
        }

        _txtConvertProgression.text = _localization.GetTexts(Localization.CONVERT_PROGRESSION_FETCHING_ITEM_SCRIPT_COPIER) + "..";

        FetchItemScriptCopier();

        if (_isFilesError)
        {
            _txtConvertProgression.text = "<color=red>" + _localization.GetTexts(Localization.ERROR) + "</color>: " + _errorLog;

            return;
        }

        _txtConvertProgression.text = _localization.GetTexts(Localization.CONVERT_PROGRESSION_PLEASE_WAIT) + ".. (0%)";

        Debug.Log(DateTime.Now);

        Invoke("Convert", 0.1f);
    }

    // Exporting

    /// <summary>
    /// Export item lists that can be use in game
    /// </summary>
    void ExportItemLists()
    {
        StringBuilder builder = new StringBuilder();

        ExportingItemLists(builder, "weaponIds", _itemListContainer.weaponIds);
        ExportingItemLists(builder, "equipmentIds", _itemListContainer.equipmentIds);
        ExportingItemLists(builder, "costumeIds", _itemListContainer.costumeIds);
        ExportingItemLists(builder, "cardIds", _itemListContainer.cardIds);
        ExportingItemLists(builder, "card2Ids", _itemListContainer.card2Ids);
        ExportingItemLists(builder, "enchantIds", _itemListContainer.enchantIds);
        ExportingItemLists(builder, "enchant2Ids", _itemListContainer.enchant2Ids);
        ExportingItemLists(builder, "itemGroupIds", _itemListContainer.itemGroupIds);

        File.WriteAllText("global_item_ids.txt", RemoveGodItem(builder.ToString()), Encoding.UTF8);

        Debug.Log("'global_item_ids.txt' has been successfully created.");

        TurnSkillNameIntoDescription();

        _easyItemBuilderDatabase.datas.Sort((a, b) => a.bonus.CompareTo(b.bonus));

        builder = new StringBuilder();
        for (int i = 0; i < _easyItemBuilderDatabase.datas.Count; i++)
        {
            builder.Append(_easyItemBuilderDatabase.datas[i].bonus + "\n");

            _easyItemBuilderDatabase.datas[i].values.Sort((a, b) => a.itemValue.CompareTo(b.itemValue));
            _easyItemBuilderDatabase.datas[i].values.Reverse();

            for (int j = 0; j < _easyItemBuilderDatabase.datas[i].values.Count; j++)
                builder.Append(_easyItemBuilderDatabase.datas[i].values[j].itemName + " == " + _easyItemBuilderDatabase.datas[i].values[j].itemValue + "\n");

            builder.Append("\n");
        }
        File.WriteAllText("easy_item_builder.txt", builder.ToString(), Encoding.UTF8);
    }
    /// <summary>
    /// Exporting item lists to StringBuilder
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="listName"></param>
    /// <param name="items"></param>
    void ExportingItemLists(StringBuilder builder, string listName, List<string> items)
    {
        builder.Append("deletearray $" + listName + "[0],getarraysize($" + listName + ");\n");
        builder.Append("setarray $" + listName + "[0],");

        items.RemoveAll((item) => string.IsNullOrEmpty(item) || string.IsNullOrWhiteSpace(item) || (item == null));

        foreach (var item in items)
            builder.Append(item + ",");

        builder.Remove(builder.Length - 1, 1);

        builder.Append(";\n");
    }
    /// <summary>
    /// Export all item to item mall (NPC) that can be use in game
    /// </summary>
    void ExportItemMall()
    {
        // All item id

        int shopNumber = 0;

        StringBuilder builder = new StringBuilder();
        StringBuilder builder2 = new StringBuilder();

        for (int i = 0; i < _itemListContainer.allItemIds.Count; i++)
        {
            if ((i == 0)
                || ((i % 100) == 0))
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                    builder.Remove(builder.Length - 1, 1);

                shopNumber++;

                builder.Append("\n-	shop	ItemMall" + shopNumber + "	-1,no,");
            }

            builder.Append(_itemListContainer.allItemIds[i] + ":" + ITEM_DEBUG_PRICE + ",");
        }

        // Pet taming item id

        shopNumber = 0;

        for (int i = 0; i < _petTamingItemIds.Count; i++)
        {
            if ((i == 0)
                || (i % 100 == 0))
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                    builder.Remove(builder.Length - 1, 1);

                shopNumber++;

                builder.Append("\n-	shop	PetTame" + shopNumber + "	-1,");
            }

            builder.Append(_petTamingItemIds[i] + ":" + PET_TAME_PRICE + ",");
        }

        // Pet eggs item id

        shopNumber = 0;

        for (int i = 0; i < _itemListContainer.petEggIds.Count; i++)
        {
            if ((i == 0)
                || (i % 100 == 0))
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                    builder.Remove(builder.Length - 1, 1);

                shopNumber++;

                builder.Append("\n-	shop	PetEgg" + shopNumber + "	-1,no,");
            }

            builder.Append(_itemListContainer.petEggIds[i] + ":" + PET_EGG_PRICE + ",");
        }

        // Pet armors item id

        shopNumber = 0;

        for (int i = 0; i < _itemListContainer.petArmorIds.Count; i++)
        {
            if ((i == 0)
                || (i % 100 == 0))
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                    builder.Remove(builder.Length - 1, 1);

                shopNumber++;

                builder.Append("\n-	shop	PetArmor" + shopNumber + "	-1,no,");
            }

            builder.Append(_itemListContainer.petArmorIds[i] + ":" + PET_ARMOR_PRICE + ",");
        }

        // Fasion costumes item id

        shopNumber = 0;

        for (int i = 0; i < _itemListContainer.fashionCostumeIds.Count; i++)
        {
            if ((i == 0)
                || (i % 100 == 0))
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                    builder.Remove(builder.Length - 1, 1);

                shopNumber++;

                builder.Append("\n-	shop	FashionCostume" + shopNumber + "	-1,no,");
            }

            builder.Append(_itemListContainer.fashionCostumeIds[i] + ":" + FASHION_PRICE + ",");
        }

        // Buffs item id

        shopNumber = 0;

        for (int i = 0; i < _itemListContainer.buffItemIds.Count; i++)
        {
            if ((i == 0)
                || (i % 100 == 0))
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                    builder.Remove(builder.Length - 1, 1);

                shopNumber++;

                builder.Append("\n-	shop	BuffItem" + shopNumber + "	-1,no,");
            }

            builder.Append(_itemListContainer.buffItemIds[i] + ":" + BUFF_PRICE + ",");
        }

        // SubType item id

        List<string> subTypes = new List<string>();
        List<int> maxPages = new List<int>();
        foreach (var item in _itemListContainer.subTypeDatas)
        {
            subTypes.Add(item.subType);
            int maxPage = 0;

            builder2.Append("deletearray $" + item.subType + "[0],getarraysize($" + item.subType + ");\n");
            builder2.Append("setarray $" + item.subType + "[0],");

            for (int i = 0; i < item.id.Count; i++)
            {
                if ((i == 0)
                    || (i % 100 == 0))
                {
                    if (!string.IsNullOrEmpty(builder.ToString()))
                        builder.Remove(builder.Length - 1, 1);

                    maxPage++;
                    builder.Append("\n-	shop	Debug" + item.subType + "" + maxPage + "	-1,no,");
                }

                builder.Append(item.id[i] + ":" + ITEM_DEBUG_PRICE + ",");
                builder2.Append(item.id[i] + ",");
            }

            builder2.Remove(builder2.Length - 1, 1);
            builder2.Append(";\n");

            maxPages.Add(maxPage);
        }

        if (!string.IsNullOrEmpty(builder.ToString()))
            builder.Remove(builder.Length - 1, 1);

        builder.Append("\nsetarray .subType$[0],");
        for (int i = 0; i < subTypes.Count; i++)
            builder.Append("\"" + subTypes[i] + "\",");

        if (!string.IsNullOrEmpty(builder.ToString()))
            builder.Remove(builder.Length - 1, 1);

        builder.Append(";");

        builder.Append("\nsetarray .subTypeMaxPage[0],");
        for (int i = 0; i < maxPages.Count; i++)
            builder.Append("" + maxPages[i] + ",");

        if (!string.IsNullOrEmpty(builder.ToString()))
            builder.Remove(builder.Length - 1, 1);

        builder.Append(";");
        builder.Append(";");

        // Location item id

        List<string> locations = new List<string>();
        maxPages = new List<int>();
        foreach (var item in _itemListContainer.locationDatas)
        {
            locations.Add(item.location);
            int maxPage = 0;

            builder2.Append("deletearray $" + item.location + "[0],getarraysize($" + item.location + ");\n");
            builder2.Append("setarray $" + item.location + "[0],");

            for (int i = 0; i < item.id.Count; i++)
            {
                if ((i == 0)
                    || (i % 100 == 0))
                {
                    if (!string.IsNullOrEmpty(builder.ToString()))
                        builder.Remove(builder.Length - 1, 1);

                    maxPage++;
                    builder.Append("\n-	shop	Debug" + item.location + "" + maxPage + "	-1,no,");
                }

                builder.Append(item.id[i] + ":" + ITEM_DEBUG_PRICE + ",");
                builder2.Append(item.id[i] + ",");
            }

            builder2.Remove(builder2.Length - 1, 1);
            builder2.Append(";\n");

            maxPages.Add(maxPage);
        }

        locations.Add("CARD1");
        int cardMaxPage = 0;
        for (int i = 0; i < _itemListContainer.cardIds.Count; i++)
        {
            if ((i == 0)
                 || (i % 100 == 0))
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                    builder.Remove(builder.Length - 1, 1);

                cardMaxPage++;
                builder.Append("\n-	shop	DebugCARD1" + cardMaxPage + "	-1,no,");
            }

            builder.Append(_itemListContainer.cardIds[i] + ":" + ITEM_DEBUG_PRICE + ",");
        }
        maxPages.Add(cardMaxPage);
        locations.Add("CARD2");
        cardMaxPage = 0;
        for (int i = 0; i < _itemListContainer.card2Ids.Count; i++)
        {
            if ((i == 0)
                 || (i % 100 == 0))
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                    builder.Remove(builder.Length - 1, 1);

                cardMaxPage++;
                builder.Append("\n-	shop	DebugCARD2" + cardMaxPage + "	-1,no,");
            }

            builder.Append(_itemListContainer.card2Ids[i] + ":" + ITEM_DEBUG_PRICE + ",");
        }
        maxPages.Add(cardMaxPage);

        if (!string.IsNullOrEmpty(builder.ToString()))
            builder.Remove(builder.Length - 1, 1);

        builder.Append("\nsetarray .location$[0],");
        for (int i = 0; i < locations.Count; i++)
            builder.Append("\"" + locations[i] + "\",");

        if (!string.IsNullOrEmpty(builder.ToString()))
            builder.Remove(builder.Length - 1, 1);

        builder.Append(";");

        builder.Append("\nsetarray .locationMaxPage[0],");
        for (int i = 0; i < maxPages.Count; i++)
            builder.Append("" + maxPages[i] + ",");

        if (!string.IsNullOrEmpty(builder.ToString()))
            builder.Remove(builder.Length - 1, 1);

        builder.Append(";");

        // Print

        var builderDebug = builder.ToString();

        if (!string.IsNullOrEmpty(builderDebug)
            && (builderDebug[builderDebug.Length - 1] == ','))
            builder.Remove(builder.Length - 1, 1);

        File.WriteAllText("item_mall.txt", RemoveGodItem(builder.ToString() + "\n" + builder2.ToString()), Encoding.UTF8);

        Debug.Log("'item_mall.txt' has been successfully created.");
    }

    /// <summary>
    /// Export monster list
    /// </summary>
    void ExportMonsterLists()
    {
        List<string> monsterIds = new List<string>();
        List<string> attackableMonsterIds = new List<string>();
        List<string> attackableMonsterTier1Ids = new List<string>();
        List<string> attackableMonsterTier2Ids = new List<string>();
        List<string> attackableMonsterTier3Ids = new List<string>();
        List<string> attackableMonsterTier4Ids = new List<string>();
        List<string> attackableMonsterTier5Ids = new List<string>();
        List<string> attackableMonsterTier6Ids = new List<string>();
        List<string> attackableMonsterTier7Ids = new List<string>();
        List<string> attackableMonsterTier8Ids = new List<string>();
        List<string> attackableMonsterTier9Ids = new List<string>();
        List<string> attackableMonsterTier10Ids = new List<string>();
        List<string> mvpIds = new List<string>();

        List<int> errorMonsterIds = new List<int>
        {
            1288,
            1324,
            1325,
            1326,
            1327,
            1328,
            1329,
            1330,
            1331,
            1332,
            1333,
            1334,
            1335,
            1336,
            1337,
            1338,
            1339,
            1340,
            1341,
            1342,
            1343,
            1344,
            1345,
            1346,
            1347,
            1348,
            1349,
            1350,
            1351,
            1352,
            1353,
            1354,
            1355,
            1356,
            1357,
            1358,
            1359,
            1360,
            1361,
            1362,
            1363,
            1732,
            1798,
            1845,
            1902,
            1903,
            1905,
            1906,
            1911,
            1912,
            1913,
            1938,
            1939,
            1940,
            1941,
            1942,
            1943,
            1944,
            1945,
            1946,
            1955,
            2245,
            2288,
            2328,
            2335,
            2337,
            2343,
            2408,
            2409,
            2410,
            2411,
            2413,
            2452,
            2453,
            2454,
            2455,
            2456,
            2457,
            2458,
            2459,
            2460,
            2461,
            2462,
            2536,
            2537,
            2539,
            3007,
            3008,
            3038,
            3075,
            3086,
            20269,
            20846,
            20847,
            20848,
            20849,
            20850,
            20851,
            21064,
            21065,
            21066,
            21067,
            21068,
            21069,
            21070,
            21071,
            21072,
            21073,
            21074,
            21075,
            21076,
            21077,
            21078,
            21079,
            21080,
            21081,
            21082,
            21083,
            21084,
            21085,
            21086
        };

        for (int i = 0; i < _monsterIds.Count; i++)
        {
            var monsterDatabase = _monsterDatabases[_monsterIds[i]];

            if (errorMonsterIds.Contains(monsterDatabase.id) || (monsterDatabase.clientAttackMotion <= 0))
                continue;

            var monsterId = monsterDatabase.id.ToString("f0");

            monsterIds.Add(monsterId);

            // MvP
            if (monsterDatabase.isMvp)
                mvpIds.Add(monsterId);
            else
            {
                attackableMonsterIds.Add(monsterId);
                if (monsterDatabase.level <= 15)
                    attackableMonsterTier1Ids.Add(monsterId);
                else if (monsterDatabase.level <= 30)
                    attackableMonsterTier2Ids.Add(monsterId);
                else if (monsterDatabase.level <= 50)
                    attackableMonsterTier3Ids.Add(monsterId);
                else if (monsterDatabase.level <= 75)
                    attackableMonsterTier4Ids.Add(monsterId);
                else if (monsterDatabase.level <= 99)
                    attackableMonsterTier5Ids.Add(monsterId);
                else if (monsterDatabase.level <= 125)
                    attackableMonsterTier6Ids.Add(monsterId);
                else if (monsterDatabase.level <= 145)
                    attackableMonsterTier7Ids.Add(monsterId);
                else if (monsterDatabase.level <= 165)
                    attackableMonsterTier8Ids.Add(monsterId);
                else if (monsterDatabase.level <= 185)
                    attackableMonsterTier9Ids.Add(monsterId);
                else
                    attackableMonsterTier10Ids.Add(monsterId);
            }
        }

        StringBuilder builder = new StringBuilder();

        ExportingItemLists(builder, "monsterIds", monsterIds);
        ExportingItemLists(builder, "attackableMonsterIds", attackableMonsterIds);
        ExportingItemLists(builder, "attackableMonsterTier1Ids", attackableMonsterTier1Ids);
        ExportingItemLists(builder, "attackableMonsterTier2Ids", attackableMonsterTier2Ids);
        ExportingItemLists(builder, "attackableMonsterTier3Ids", attackableMonsterTier3Ids);
        ExportingItemLists(builder, "attackableMonsterTier4Ids", attackableMonsterTier4Ids);
        ExportingItemLists(builder, "attackableMonsterTier5Ids", attackableMonsterTier5Ids);
        ExportingItemLists(builder, "attackableMonsterTier6Ids", attackableMonsterTier6Ids);
        ExportingItemLists(builder, "attackableMonsterTier7Ids", attackableMonsterTier7Ids);
        ExportingItemLists(builder, "attackableMonsterTier8Ids", attackableMonsterTier8Ids);
        ExportingItemLists(builder, "attackableMonsterTier9Ids", attackableMonsterTier9Ids);
        ExportingItemLists(builder, "attackableMonsterTier10Ids", attackableMonsterTier10Ids);
        ExportingItemLists(builder, "mvpIds", mvpIds);

        File.WriteAllText("global-monster-list.txt", builder.ToString(), Encoding.UTF8);

        Debug.Log("'global-monster-list.txt' has been successfully created.");
        Debug.Log("monsterIds.Count:" + monsterIds.Count);
        Debug.Log("mvpIds.Count:" + mvpIds.Count);
    }

    void ExportSkillLists()
    {
        List<string> criticalSkillNames = new List<string>();

        for (int i = 0; i < _allSkillDatabases.Count; i++)
        {
            var skillDatabase = _allSkillDatabases[i];
            if (skillDatabase.isCritical)
                criticalSkillNames.Add(skillDatabase.description);
        }

        StringBuilder builder = new StringBuilder();

        foreach (var item in criticalSkillNames)
            builder.Append(item + "\n");

        File.WriteAllText("critical-skill-list.txt", builder.ToString(), Encoding.UTF8);

        Debug.Log("'critical-skill-list.txt' has been successfully created.");
        Debug.Log("criticalSkillNames.Count:" + criticalSkillNames.Count);

        builder = new StringBuilder();

        builder.Append("setarray $learnableSkills$[0],");
        foreach (var item in _allLearnableSkillDatabases)
            builder.Append("\"" + item + "\",");
        builder.Remove(builder.Length - 1, 1);
        builder.Append(";");

        File.WriteAllText("learnable-skill-list.txt", builder.ToString(), Encoding.UTF8);

        Debug.Log("'learnable-skill-list.txt' has been successfully created.");
        Debug.Log("learnable.Count:" + _allLearnableSkillDatabases.Count);

        int shopNumber = 0;
        int addedItem = 0;
        builder = new StringBuilder();
        builder.Append("\n-	shop	ItemMallSkill" + shopNumber + "	-1,");
        List<int> itemSkillAddedList = new List<int>();
        for (int i = 0; i < _allSkillDatabases.Count; i++)
        {
            if (_allSkillDatabases[i].requiredItems.Count > 0)
            {
                for (int j = 0; j < _allSkillDatabases[i].requiredItems.Count; j++)
                {
                    if (IsItemMallSkillError(_allSkillDatabases[i].requiredItems[j]))
                        continue;

                    var itemId = GetItemIdFromAegisName(_allSkillDatabases[i].requiredItems[j]);
                    if (itemId <= 0)
                    {
                        Debug.LogWarning("Found wrong Item Id for skill ID: " + _allSkillDatabases[i].id);

                        continue;
                    }

                    if (itemSkillAddedList.Contains(itemId))
                        continue;

                    itemSkillAddedList.Add(itemId);
                    builder.Append(itemId + ":" + GetItemMallSkillPrice(itemId) + ",");
                    addedItem++;

                    if ((addedItem % 100) == 0)
                    {
                        if (!string.IsNullOrEmpty(builder.ToString()))
                            builder.Remove(builder.Length - 1, 1);

                        shopNumber++;

                        builder.Append("\n-	shop	ItemMallSkill" + shopNumber + "	-1,");
                    }
                }
            }

            if (_allSkillDatabases[i].requiredEquipments.Count > 0)
            {
                for (int j = 0; j < _allSkillDatabases[i].requiredEquipments.Count; j++)
                {
                    if (IsItemMallSkillError(_allSkillDatabases[i].requiredEquipments[j]))
                        continue;

                    var itemId = GetItemIdFromAegisName(_allSkillDatabases[i].requiredEquipments[j]);
                    if (itemId <= 0)
                    {
                        Debug.LogWarning("Found wrong Item Id for skill ID: " + _allSkillDatabases[i].id);

                        continue;
                    }

                    if (itemSkillAddedList.Contains(itemId))
                        continue;

                    itemSkillAddedList.Add(itemId);
                    builder.Append(itemId + ":" + GetItemMallSkillPrice(itemId) + ",");
                    addedItem++;

                    if ((addedItem % 100) == 0)
                    {
                        if (!string.IsNullOrEmpty(builder.ToString()))
                            builder.Remove(builder.Length - 1, 1);

                        shopNumber++;

                        builder.Append("\n-	shop	ItemMallSkill" + shopNumber + "	-1,");
                    }
                }
            }
        }

        List<int> missingItemId = new List<int>();
        missingItemId.Add(22549);
        missingItemId.Add(12717);
        missingItemId.Add(12722);
        missingItemId.Add(12720);
        missingItemId.Add(12718);
        missingItemId.Add(12724);
        missingItemId.Add(12723);
        missingItemId.Add(12721);
        missingItemId.Add(12719);
        missingItemId.Add(100065);
        missingItemId.Add(100066);
        missingItemId.Add(100067);
        missingItemId.Add(100068);
        missingItemId.Add(100069);
        missingItemId.Add(100070);
        missingItemId.Add(100071);
        missingItemId.Add(100072);
        missingItemId.Add(100073);
        missingItemId.Add(100074);
        missingItemId.Add(6284);
        missingItemId.Add(6285);
        missingItemId.Add(11022);
        missingItemId.Add(11023);
        missingItemId.Add(11024);
        missingItemId.Add(6248);
        missingItemId.Add(6250);
        missingItemId.Add(6251);
        missingItemId.Add(6253);
        missingItemId.Add(6255);
        missingItemId.Add(6258);
        missingItemId.Add(6261);
        missingItemId.Add(6262);
        missingItemId.Add(1000275);
        missingItemId.Add(7433);
        missingItemId.Add(12731);
        missingItemId.Add(12728);
        missingItemId.Add(12732);
        missingItemId.Add(12733);
        missingItemId.Add(12729);
        missingItemId.Add(12730);
        missingItemId.Add(12726);
        missingItemId.Add(12725);
        missingItemId.Add(12727);
        missingItemId.Add(22540);
        missingItemId.Add(23277);

        for (int i = 0; i < missingItemId.Count; i++)
        {
            var itemId = missingItemId[i];
            if (itemSkillAddedList.Contains(itemId))
                continue;

            itemSkillAddedList.Add(itemId);
            builder.Append(itemId + ":" + GetItemMallSkillPrice(itemId) + ",");
            addedItem++;

            if ((addedItem % 100) == 0)
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                    builder.Remove(builder.Length - 1, 1);

                shopNumber++;

                builder.Append("\n-	shop	ItemMallSkill" + shopNumber + "	-1,");
            }


        }
        if (!string.IsNullOrEmpty(builder.ToString()))
            builder.Remove(builder.Length - 1, 1);

        File.WriteAllText("item-mall-skill-list.txt", builder.ToString(), Encoding.UTF8);

        Debug.Log("'item-mall-skill-list.txt' has been successfully created.");
    }

    void ExportErrorLists()
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < errorResourceNames.Count; i++)
            builder.Append(errorResourceNames[i] + "\n");
        File.WriteAllText("Assets/Assets/error_resource_names.txt", builder.ToString(), Encoding.UTF8);
    }

    // Parsing

    /// <summary>
    /// Parse monster database file into converter (Only store ID, Name)
    /// </summary>
    void FetchItemScriptCopier()
    {
        var path = Application.dataPath + "/Assets/itemScriptCopier.txt";

        // Is file exists?
        if (!File.Exists(path))
        {
            _errorLog = path + " " + _localization.GetTexts(Localization.NOT_FOUND);

            Debug.Log(_errorLog);

            _isFilesError = true;

            return;
        }

        var itemScriptCopierFile = File.ReadAllText(path);

        var itemScriptCopierDatabases = itemScriptCopierFile.Split('\n');

        _itemScriptCopierDatabases = new Dictionary<int, int>();

        for (int i = 0; i < itemScriptCopierDatabases.Length; i++)
        {
            var text = itemScriptCopierDatabases[i];

            text = CommentRemover.Fix(text);

            text = LineEndingsRemover.Fix(text);

            if (string.IsNullOrEmpty(text)
                || string.IsNullOrWhiteSpace(text))
                continue;

            var texts = text.Split('=');

            var itemId = int.Parse(texts[0]);
            var copyFromItemId = int.Parse(texts[1]);

            if (!_itemScriptCopierDatabases.ContainsKey(itemId))
                _itemScriptCopierDatabases.Add(itemId, copyFromItemId);
        }

        Debug.Log("There are " + _itemScriptCopierDatabases.Count + " item script copier database.");
    }
    /// <summary>
    /// Fetch resource name from all equipment (Split into list by equipment type)
    /// </summary>
    void FetchResourceNameWithType()
    {
        var path = Application.dataPath + "/Assets/item_db_equip.yml";
        var path2 = Application.dataPath + "/Assets/item_db_etc.yml";

        // Is file exists?
        if (!File.Exists(path))
        {
            _errorLog = path + " " + _localization.GetTexts(Localization.NOT_FOUND);

            Debug.Log(_errorLog);

            return;
        }

        // Is file exists?
        if (!File.Exists(path2))
        {
            _errorLog = path2 + " " + _localization.GetTexts(Localization.NOT_FOUND);

            Debug.Log(_errorLog);

            return;
        }

        var equipmentsFile = File.ReadAllText(path);
        var enchantmentFile = File.ReadAllText(path2);

        var equipments = equipmentsFile.Split('\n');
        var enchantments = enchantmentFile.Split('\n');

        _resourceContainer = new ResourceContainer();

        string id = string.Empty;

        bool isArmor = false;

        for (int i = 0; i < equipments.Length; i++)
        {
            var text = equipments[i];

            text = CommentRemover.Fix(text);

            text = LineEndingsRemover.Fix(text);

            // Skip
            if (text.Contains("    Buy:")
                || text.Contains("    Sell:")
                || text.Contains("    Jobs:")
                || text.Contains("    Classes:")
                || text.Contains("    AliasName:")
                || text.Contains("    Flags:")
                || text.Contains("    BuyingStore:")
                || text.Contains("    DeadBranch:")
                || text.Contains("    Container:")
                || text.Contains("    UniqueId:")
                || text.Contains("    BindOnEquip:")
                || text.Contains("    DropAnnounce:")
                || text.Contains("    NoConsume:")
                || text.Contains("    DropEffect:")
                || text.Contains("    Delay:")
                || text.Contains("    Duration:")
                || text.Contains("    Status:")
                || text.Contains("    Stack:")
                || text.Contains("    Amount:")
                || text.Contains("    Inventory:")
                || text.Contains("    Cart:")
                || text.Contains("    Storage:")
                || text.Contains("    GuildStorage:")
                || text.Contains("    NoUse:")
                || text.Contains("    Override:")
                || text.Contains("    Sitting:")
                || text.Contains("    Trade:")
                || text.Contains("    NoDrop:")
                || text.Contains("    NoTrade:")
                || text.Contains("    TradePartner:")
                || text.Contains("    NoSell:")
                || text.Contains("    NoCart:")
                || text.Contains("    NoStorage:")
                || text.Contains("    NoGuildStorage:")
                || text.Contains("    NoMail:")
                || text.Contains("    NoAuction:")
                || text.Contains("    Script:")
                || text.Contains("    OnEquip_Script:")
                || text.Contains("    OnUnequip_Script:")
                || string.IsNullOrEmpty(text)
                || string.IsNullOrWhiteSpace(text))
                text = string.Empty;

            // Id
            if (text.Contains("  - Id:"))
            {
                text = QuoteRemover.Remove(text);

                text = SpacingRemover.Remove(text);

                id = text.Replace("-Id:", string.Empty);
            }
            // Type
            else if (text.Contains("    Type:"))
            {
                text = QuoteRemover.Remove(text);

                text = SpacingRemover.Remove(text);

                text = text.Replace("Type:", string.Empty);

                if ((text.ToLower() == "armor")
                    || (text.ToLower() == "shadowgear"))
                    isArmor = true;
                else
                    isArmor = false;
            }
            // Locations
            else if (isArmor)
            {
                text = QuoteRemover.Remove(text);

                text = SpacingRemover.Remove(text);

                if (!_isFastConvert || (_isRandomResourceNameForCustomTextAssetOnly && _isRandomResourceName))
                {
                    if (text.ToLower().Contains("head_top"))
                        _resourceContainer.topHeadgears.Add(id);
                    else if (text.ToLower().Contains("head_mid"))
                        _resourceContainer.middleHeadgears.Add(id);
                    else if (text.ToLower().Contains("head_low"))
                        _resourceContainer.lowerHeadgears.Add(id);
                    else if (text.ToLower().Contains("garment"))
                        _resourceContainer.garments.Add(id);
                    else if (text.ToLower().Contains("armor"))
                        _resourceContainer.armors.Add(id);
                    else if (text.ToLower().Contains("shadow_weapon")
                        || text.ToLower().Contains("shadow_shield"))
                        _resourceContainer.shields.Add(id);
                    else if (text.ToLower().Contains("shoes"))
                        _resourceContainer.shoes.Add(id);
                    else if (text.ToLower().Contains("accessory"))
                        _resourceContainer.accessorys.Add(id);
                }
            }
            else
            {
                text = QuoteRemover.Remove(text);

                text = SpacingRemover.Remove(text);

                if (!_isFastConvert || (_isRandomResourceNameForCustomTextAssetOnly && _isRandomResourceName))
                {
                    if (text.ToLower().Contains("dagger"))
                        _resourceContainer.daggers.Add(id);
                    else if (text.ToLower().Contains("1hsword"))
                        _resourceContainer.oneHandedSwords.Add(id);
                    else if (text.ToLower().Contains("2hsword"))
                        _resourceContainer.twoHandedSwords.Add(id);
                    else if (text.ToLower().Contains("1hspear"))
                        _resourceContainer.oneHandedSpears.Add(id);
                    else if (text.ToLower().Contains("2hspear"))
                        _resourceContainer.twoHandedSpears.Add(id);
                    else if (text.ToLower().Contains("1haxe"))
                        _resourceContainer.oneHandedAxes.Add(id);
                    else if (text.ToLower().Contains("2haxe"))
                        _resourceContainer.twoHandedAxes.Add(id);
                    else if (text.ToLower().Contains("2hmace"))
                        _resourceContainer.twoHandedMaces.Add(id);
                    else if (text.ToLower().Contains("mace"))
                        _resourceContainer.oneHandedMaces.Add(id);
                    else if (text.ToLower().Contains("2hstaff"))
                        _resourceContainer.twoHandedStaffs.Add(id);
                    else if (text.ToLower().Contains("staff"))
                        _resourceContainer.oneHandedStaffs.Add(id);
                    else if (text.ToLower().Contains("bow"))
                        _resourceContainer.bows.Add(id);
                    else if (text.ToLower().Contains("knuckle"))
                        _resourceContainer.knuckles.Add(id);
                    else if (text.ToLower().Contains("musical"))
                        _resourceContainer.musicals.Add(id);
                    else if (text.ToLower().Contains("whip"))
                        _resourceContainer.whips.Add(id);
                    else if (text.ToLower().Contains("book"))
                        _resourceContainer.books.Add(id);
                    else if (text.ToLower().Contains("katar"))
                        _resourceContainer.katars.Add(id);
                    else if (text.ToLower().Contains("revolver"))
                        _resourceContainer.revolvers.Add(id);
                    else if (text.ToLower().Contains("rifle"))
                        _resourceContainer.rifles.Add(id);
                    else if (text.ToLower().Contains("gatling"))
                        _resourceContainer.gatlings.Add(id);
                    else if (text.ToLower().Contains("shotgun"))
                        _resourceContainer.shotguns.Add(id);
                    else if (text.ToLower().Contains("grenade"))
                        _resourceContainer.grenades.Add(id);
                    else if (text.ToLower().Contains("huuma"))
                        _resourceContainer.huumas.Add(id);
                }
            }
        }
        for (int i = 0; i < enchantments.Length; i++)
        {
            var text = enchantments[i];

            text = CommentRemover.Fix(text);

            text = LineEndingsRemover.Fix(text);

            // Skip
            if (text.Contains("    Buy:")
                || text.Contains("    Sell:")
                || text.Contains("    Jobs:")
                || text.Contains("    Classes:")
                || text.Contains("    AliasName:")
                || text.Contains("    Flags:")
                || text.Contains("    BuyingStore:")
                || text.Contains("    DeadBranch:")
                || text.Contains("    Container:")
                || text.Contains("    UniqueId:")
                || text.Contains("    BindOnEquip:")
                || text.Contains("    DropAnnounce:")
                || text.Contains("    NoConsume:")
                || text.Contains("    DropEffect:")
                || text.Contains("    Delay:")
                || text.Contains("    Duration:")
                || text.Contains("    Status:")
                || text.Contains("    Stack:")
                || text.Contains("    Amount:")
                || text.Contains("    Inventory:")
                || text.Contains("    Cart:")
                || text.Contains("    Storage:")
                || text.Contains("    GuildStorage:")
                || text.Contains("    NoUse:")
                || text.Contains("    Override:")
                || text.Contains("    Sitting:")
                || text.Contains("    Trade:")
                || text.Contains("    NoDrop:")
                || text.Contains("    NoTrade:")
                || text.Contains("    TradePartner:")
                || text.Contains("    NoSell:")
                || text.Contains("    NoCart:")
                || text.Contains("    NoStorage:")
                || text.Contains("    NoGuildStorage:")
                || text.Contains("    NoMail:")
                || text.Contains("    NoAuction:")
                || text.Contains("    Script:")
                || text.Contains("    OnEquip_Script:")
                || text.Contains("    OnUnequip_Script:")
                || string.IsNullOrEmpty(text)
                || string.IsNullOrWhiteSpace(text))
                text = string.Empty;

            // Id
            if (text.Contains("  - Id:"))
            {
                text = QuoteRemover.Remove(text);

                text = SpacingRemover.Remove(text);

                id = text.Replace("-Id:", string.Empty);
            }
            else
            {
                text = QuoteRemover.Remove(text);

                text = SpacingRemover.Remove(text);

                if (!_isFastConvert && text.ToLower().Contains("enchant"))
                    _resourceContainer.enchantments.Add(id);
            }
        }
    }
    /// <summary>
    /// Parse monster database file into converter (Only store ID, Name)
    /// </summary>
    void FetchMonster()
    {
        var path = Application.dataPath + "/Assets/mob_db.yml";

        // Is file exists?
        if (!File.Exists(path))
        {
            _errorLog = path + " " + _localization.GetTexts(Localization.NOT_FOUND);

            Debug.Log(_errorLog);

            _isFilesError = true;

            return;
        }

        var monsterDatabasesFile = File.ReadAllText(path);

        var monsterDatabases = monsterDatabasesFile.Split('\n');

        _monsterDatabases = new Dictionary<int, MonsterDatabase>();

        _monsterIds = new List<int>();

        _monsterIdsByAegisName = new Dictionary<string, int>();

        MonsterDatabase monsterDatabase = new MonsterDatabase();

        for (int i = 0; i < monsterDatabases.Length; i++)
        {
            var text = monsterDatabases[i];

            if (string.IsNullOrEmpty(text)
                || string.IsNullOrWhiteSpace(text))
                continue;

            text = CommentRemover.Fix(text);

            text = LineEndingsRemover.Fix(text);

            if (text.Contains("  - Id:"))
            {
                monsterDatabase = new MonsterDatabase();

                monsterDatabase.id = int.Parse(SpacingRemover.Remove(text).Replace("-Id:", string.Empty));
            }
            else if (text.Contains("    AegisName:"))
            {
                monsterDatabase.aegisName = QuoteRemover.Remove(text.Replace("    AegisName: ", string.Empty));

                if (_monsterIdsByAegisName.ContainsKey(monsterDatabase.aegisName))
                    Debug.LogWarning("Found duplicated monster aegis name: " + monsterDatabase.id + " Please tell rAthena about this.");
                else
                {
                    _monsterIdsByAegisName.Add(monsterDatabase.aegisName, monsterDatabase.id);
                    _monsterIds.Add(monsterDatabase.id);
                }
            }
            else if (text.Contains("    Name:"))
            {
                monsterDatabase.name = QuoteRemover.Remove(text.Replace("    Name: ", string.Empty));

                if (_monsterDatabases.ContainsKey(monsterDatabase.id))
                    Debug.LogWarning("Found duplicated monster ID: " + monsterDatabase.id + " Please tell rAthena about this.");
                else
                    _monsterDatabases.Add(monsterDatabase.id, monsterDatabase);
            }
            else if ((text.Contains("    MvpExp: ") || text.Contains("    MvpDrops:") || text.Contains("      Mvp: true"))
                && !monsterDatabase.aegisName.Contains("DUMMY")
                && (monsterDatabase.id != 1288)
                && (monsterDatabase.id != 1289)
                && (monsterDatabase.id != 1816)
                && (monsterDatabase.id != 1847)
                && (monsterDatabase.id != 1907)
                && (monsterDatabase.id != 1908)
                && (monsterDatabase.id != 1916)
                && (monsterDatabase.id != 1935)
                && (monsterDatabase.id != 1936)
                && (monsterDatabase.id != 1947)
                && (monsterDatabase.id != 2139)
                && (monsterDatabase.id != 2140)
                && (monsterDatabase.id != 2141)
                && (monsterDatabase.id != 2142)
                && (monsterDatabase.id != 2143)
                && (monsterDatabase.id != 2146)
                && (monsterDatabase.id != 2156)
                && (monsterDatabase.id != 2192)
                && (monsterDatabase.id != 2193)
                && (monsterDatabase.id != 2194)
                && (monsterDatabase.id != 2332)
                && (monsterDatabase.id != 20353))
                monsterDatabase.isMvp = true;
            else if (text.Contains("    Level: "))
                monsterDatabase.level = int.Parse(SpacingRemover.Remove(text).Replace("Level:", string.Empty));
            else if (text.Contains("    Hp: "))
                monsterDatabase.hp = int.Parse(SpacingRemover.Remove(text).Replace("Hp:", string.Empty));
            else if (text.Contains("    BaseExp: "))
                monsterDatabase.baseExp = int.Parse(SpacingRemover.Remove(text).Replace("BaseExp:", string.Empty));
            else if (text.Contains("    Attack: "))
                monsterDatabase.attack = int.Parse(SpacingRemover.Remove(text).Replace("Attack:", string.Empty));
            else if (text.Contains("    Attack2: "))
                monsterDatabase.attack2 = int.Parse(SpacingRemover.Remove(text).Replace("Attack2:", string.Empty));
            else if (text.Contains("    WalkSpeed: "))
                monsterDatabase.walkSpeed = int.Parse(SpacingRemover.Remove(text).Replace("WalkSpeed:", string.Empty));
            else if (text.Contains("    AttackMotion: "))
                monsterDatabase.attackMotion = int.Parse(SpacingRemover.Remove(text).Replace("AttackMotion:", string.Empty));
            else if (text.Contains("    ClientAttackMotion: "))
                monsterDatabase.clientAttackMotion = int.Parse(SpacingRemover.Remove(text).Replace("ClientAttackMotion:", string.Empty));
            else if (text.Contains("    DamageTaken:"))
                monsterDatabase.damageTaken = int.Parse(SpacingRemover.Remove(text).Replace("DamageTaken:", string.Empty));
            else if (text.Contains("    Class:"))
                monsterDatabase._class = SpacingRemover.Remove(text).Replace("Class:", string.Empty);
        }

        Debug.Log("There are " + _monsterDatabases.Count + " monster database.");
    }
    /// <summary>
    /// Parse class number file into converter
    /// </summary>
    void FetchClassNumber()
    {
        var path = Application.dataPath + "/Assets/classNum.txt";

        // Is file exists?
        if (!File.Exists(path))
        {
            _errorLog = path + " " + _localization.GetTexts(Localization.NOT_FOUND);

            Debug.Log(_errorLog);

            _isFilesError = true;

            return;
        }

        var classNumbersFile = File.ReadAllText(path);

        var classNumbers = classNumbersFile.Split('\n');

        _classNumberDatabases = new Dictionary<int, string>();

        var id = 0;
        var classNumber = string.Empty;

        for (int i = 0; i < classNumbers.Length; i++)
        {
            var text = classNumbers[i];

            if (string.IsNullOrEmpty(text)
                || string.IsNullOrWhiteSpace(text))
                continue;

            text = LineEndingsRemover.Fix(text);

            var texts = text.Split('=');

            id = int.Parse(texts[0]);
            classNumber = texts[1];

            if (!_classNumberDatabases.ContainsKey(id))
                _classNumberDatabases.Add(id, classNumber);
        }

        path = Application.dataPath + "/Assets/item_db_custom.txt";

        // Is file exists?
        if (!File.Exists(path))
        {
            _errorLog = path + " " + _localization.GetTexts(Localization.NOT_FOUND);

            Debug.Log(_errorLog);

            _isFilesError = true;

            return;
        }

        classNumbersFile = File.ReadAllText(path);

        classNumbers = classNumbersFile.Split('\n');

        for (int i = 0; i < classNumbers.Length; i++)
        {
            var text = classNumbers[i];

            if (string.IsNullOrEmpty(text)
                || string.IsNullOrWhiteSpace(text))
                continue;

            text = CommentRemover.Fix(text);

            text = LineEndingsRemover.Fix(text);

            if (text.Contains("- Id:"))
                id = int.Parse(SpacingRemover.Remove(text).Replace("-Id:", string.Empty));
            else if (text.Contains("    View:"))
            {
                classNumber = SpacingRemover.Remove(text).Replace("View:", string.Empty);

                if (!_classNumberDatabases.ContainsKey(id))
                    _classNumberDatabases.Add(id, classNumber);
            }
        }

        Debug.Log("There are " + _classNumberDatabases.Count + " class number database.");
    }
    /// <summary>
    /// Parse skill database file into converter (Only store ID, Name)
    /// </summary>
    void FetchSkill()
    {
        var path = Application.dataPath + "/Assets/skill_db.yml";

        // Is file exists?
        if (!File.Exists(path))
        {
            _errorLog = path + " " + _localization.GetTexts(Localization.NOT_FOUND);

            Debug.Log(_errorLog);

            _isFilesError = true;

            return;
        }

        var skillDatabasesFile = File.ReadAllText(path);

        var skillDatabases = skillDatabasesFile.Split('\n');

        _skillDatabases = new Dictionary<int, SkillDatabase>();
        _skillDatabasesByName = new Dictionary<string, SkillDatabase>();
        _skillNameDatabases = new Dictionary<string, int>();
        _allSkillDatabases = new List<SkillDatabase>();

        SkillDatabase skillDatabase = new SkillDatabase();

        bool isRequiredEquipmentLines = false;

        for (int i = 0; i < skillDatabases.Length; i++)
        {
            var text = skillDatabases[i];

            if (string.IsNullOrEmpty(text)
                || string.IsNullOrWhiteSpace(text))
                continue;

            text = CommentRemover.Fix(text);

            text = LineEndingsRemover.Fix(text);

            if (text.Contains("  - Id:"))
            {
                isRequiredEquipmentLines = false;
                skillDatabase = new SkillDatabase();
                _allSkillDatabases.Add(skillDatabase);
                skillDatabase.id = int.Parse(SpacingRemover.Remove(text).Replace("-Id:", string.Empty));
            }
            else if (text.Contains("    Name:"))
                skillDatabase.name = QuoteRemover.Remove(text.Replace("    Name: ", string.Empty));
            else if (text.Contains("Critical: true"))
                skillDatabase.isCritical = true;
            else if (text.Contains("- Item:"))
                skillDatabase.requiredItems.Add(SpacingRemover.Remove(text).Replace("-Item:", string.Empty));
            else if (text.Contains("Equipment:"))
                isRequiredEquipmentLines = true;
            else if (text.Contains("Unit:") || text.Contains("Status:"))
                isRequiredEquipmentLines = false;
            else if (isRequiredEquipmentLines)
                skillDatabase.requiredEquipments.Add(SpacingRemover.Remove(text).Replace(":true", string.Empty));
            else if (text.Contains("    Description:"))
            {
                skillDatabase.description = QuoteRemover.Remove(text.Replace("    Description: ", string.Empty));

                skillDatabase.nameWithQuote = "\"" + skillDatabase.name + "\"";

                if (_skillDatabases.ContainsKey(skillDatabase.id))
                    Debug.LogWarning("Found duplicated skill ID: " + skillDatabase.id + " (Old: " + _skillDatabases[skillDatabase.id] + " vs New: " + skillDatabase.id + ")");
                else
                {
                    _skillDatabases.Add(skillDatabase.id, skillDatabase);
                    _skillDatabasesByName.Add(skillDatabase.name, skillDatabase);
                }

                if (_skillNameDatabases.ContainsKey(skillDatabase.name))
                    Debug.LogWarning("Found duplicated skill name: " + skillDatabase.name + " (Old: " + _skillNameDatabases[skillDatabase.name] + " vs New: " + skillDatabase.name + ")");
                else
                    _skillNameDatabases.Add(skillDatabase.name, skillDatabase.id);
            }
        }

        Debug.Log("There are " + _skillDatabases.Count + " skill database.");
    }
    void FetchLearnableSkill()
    {
        var path = Application.dataPath + "/Assets/all_learnable_skill.txt";

        // Is file exists?
        if (!File.Exists(path))
        {
            _errorLog = path + " " + _localization.GetTexts(Localization.NOT_FOUND);

            Debug.Log(_errorLog);

            _isFilesError = true;

            return;
        }

        var allLearnableSkillDatabasesFile = File.ReadAllText(path);

        var learnableSkillDatabases = allLearnableSkillDatabasesFile.Split('\n');

        _allLearnableSkillDatabases = new List<string>();

        for (int i = 0; i < learnableSkillDatabases.Length; i++)
        {
            var text = learnableSkillDatabases[i];

            if (string.IsNullOrEmpty(text)
                || string.IsNullOrWhiteSpace(text))
                continue;

            text = CommentRemover.Fix(text);

            text = LineEndingsRemover.Fix(text);

            if (!_allLearnableSkillDatabases.Contains(text))
            {
                if (!string.IsNullOrEmpty(GetSkillName(text, false, true, true)))
                    _allLearnableSkillDatabases.Add(text);
            }
        }

        Debug.Log("There are " + _allLearnableSkillDatabases.Count + " learnable skill.");
    }
    /// <summary>
    /// Parse resource name file into converter
    /// </summary>
    void FetchResourceName()
    {
        var path = Application.dataPath + "/Assets/resourceName.txt";

        // Is file exists?
        if (!File.Exists(path))
        {
            _errorLog = path + " " + _localization.GetTexts(Localization.NOT_FOUND);

            Debug.Log(_errorLog);

            _isFilesError = true;

            return;
        }

        var resourceNamesFile = File.ReadAllText(path, Encoding.UTF8);

        var resourceNames = resourceNamesFile.Split('\n');

        _resourceDatabases = new Dictionary<int, string>();

        for (int i = 0; i < resourceNames.Length; i++)
        {
            var text = resourceNames[i];

            if (string.IsNullOrEmpty(text)
                || string.IsNullOrWhiteSpace(text))
                continue;

            text = LineEndingsRemover.Fix(text);

            if (text[0] == '/')
                continue;

            var texts = text.Split('=');

            var id = int.Parse(texts[0]);
            var name = texts[1];

            if (!string.IsNullOrEmpty(name)
                && (name != "\"\""))
            {
                if (_resourceDatabases.ContainsKey(id))
                    Debug.LogWarning("Found duplicated resource name ID: " + id + " (Old: " + _resourceDatabases[id] + " vs New: " + name + ") (Will using a old one)");
                else
                    _resourceDatabases.Add(id, name);
            }
            else
                Debug.LogWarning("Found empty resource name ID: " + id + ") (Skip adding these)");
        }

        Debug.Log("There are " + _resourceDatabases.Count + " resource name database.");
    }
    /// <summary>
    /// Parse combo database file into converter
    /// </summary>
    void FetchCombo()
    {
        var path = Application.dataPath + "/Assets/item_combos.yml";

        // Is file exists?
        if (!File.Exists(path))
        {
            _errorLog = path + " " + _localization.GetTexts(Localization.NOT_FOUND);

            Debug.Log(_errorLog);

            _isFilesError = true;

            return;
        }

        var comboDatabasesFile = _isSkipNormalEquipEtcCombo ? string.Empty : File.ReadAllText(path);

        var comboDatabases = comboDatabasesFile.Split('\n');

        _comboDatabases = new List<ComboDatabase>();

        ComboDatabase comboDatabase = new ComboDatabase();

        bool isScript = false;

        string script = string.Empty;

        // Comment remover

        for (int i = 0; i < comboDatabases.Length; i++)
            comboDatabases[i] = CommentRemover.Fix(comboDatabases[i]);

        // Nowaday rAthena use YAML for combo database, but it still had /* and */
        // Then just keep these for unexpected error

        for (int i = 0; i < comboDatabases.Length; i++)
        {
            var text = CommentRemover.FixCommentSeperateLine(comboDatabases, i);

            if (text.Contains("- Combos:"))
            {
                ResetRefineGrade();

                comboDatabase = new ComboDatabase();

                _comboDatabases.Add(comboDatabase);

                isScript = false;

                script = string.Empty;
            }
            else if (text.Contains("- Combo:"))
                comboDatabase.sameComboDatas.Add(new ComboDatabase.SameComboData());
            else if (text.Contains("          - "))
                comboDatabase.sameComboDatas[comboDatabase.sameComboDatas.Count - 1].aegisNames.Add(SpacingRemover.Remove(QuoteRemover.Remove(text.Replace("          - ", string.Empty))).ToLower());
            else if (text.Contains("Script: |"))
                isScript = true;
            else if (isScript)
            {
                _currentCombo = string.Empty;
                for (int j = 0; j < comboDatabase.sameComboDatas[comboDatabase.sameComboDatas.Count - 1].aegisNames.Count; j++)
                    _currentCombo += GetItemIdFromAegisName(comboDatabase.sameComboDatas[comboDatabase.sameComboDatas.Count - 1].aegisNames[j]) + "+";
                if (!string.IsNullOrEmpty(_currentCombo))
                    _currentCombo = _currentCombo.Substring(0, _currentCombo.Length - 1);
                var comboScript = ConvertItemScripts(text);
                _currentCombo = string.Empty;
                if (!string.IsNullOrEmpty(comboScript))
                    script += "			\"" + comboScript + "\",\n";

                // Is reach last line or next line is new combo database?
                // Cutoff and add combo scripts now
                if ((i + 1) >= comboDatabases.Length
                    || comboDatabases[i + 1].Contains("- Combos:"))
                    comboDatabase.descriptions.Add(script);
            }
        }

        Debug.Log("There are " + _comboDatabases.Count + " combo database.");
    }
    /// <summary>
    /// Parse item database file into converter (Only store ID, Name), also parse into item list
    /// </summary>
    void FetchItem()
    {
        var path = Application.dataPath + "/Assets/item_db_equip.yml";
        var path2 = Application.dataPath + "/Assets/item_db_usable.yml";
        var path3 = Application.dataPath + "/Assets/item_db_etc.yml";
        var path4 = Application.dataPath + "/Assets/item_db_custom.txt";

        // Is file exists?
        if (!File.Exists(path))
        {
            _errorLog = path + " " + _localization.GetTexts(Localization.NOT_FOUND);

            Debug.Log(_errorLog);

            _isFilesError = true;

            return;
        }
        else if (!File.Exists(path2))
        {
            _errorLog = path2 + " " + _localization.GetTexts(Localization.NOT_FOUND);

            Debug.Log(_errorLog);

            _isFilesError = true;

            return;
        }
        else if (!File.Exists(path3))
        {
            _errorLog = path3 + " " + _localization.GetTexts(Localization.NOT_FOUND);

            Debug.Log(_errorLog);

            _isFilesError = true;

            return;
        }
        else if (!File.Exists(path4))
        {
            _errorLog = path4 + " " + _localization.GetTexts(Localization.NOT_FOUND);

            Debug.Log(_errorLog);

            _isFilesError = true;

            return;
        }

        var itemDatabasesFile = (_isSkipNormalEquipEtcCombo ? string.Empty : File.ReadAllText(path)) + "\n"
            + File.ReadAllText(path2) + "\n"
            + File.ReadAllText(path3) + "\n"
            + File.ReadAllText(path4);

        var itemDatabases = itemDatabasesFile.Split('\n');

        _itemDatabases = new Dictionary<int, ItemDatabase>();
        _aegisNameDatabases = new Dictionary<string, int>();

        _itemListContainer = new ItemListContainer();

        ItemDatabase itemDatabase = new ItemDatabase();

        string _id = string.Empty;

        string _name = string.Empty;

        bool isArmor = false;

        for (int i = 0; i < itemDatabases.Length; i++)
        {
            var text = itemDatabases[i];

            text = CommentRemover.Fix(text);

            text = LineEndingsRemover.Fix(text);

            // Skip
            if (text.Contains("    Buy:")
                || text.Contains("    Sell:")
                || text.Contains("    Jobs:")
                || text.Contains("    Classes:")
                || text.Contains("    AliasName:")
                || text.Contains("    Flags:")
                || text.Contains("    BuyingStore:")
                || text.Contains("    DeadBranch:")
                || text.Contains("    Container:")
                || text.Contains("    UniqueId:")
                || text.Contains("    BindOnEquip:")
                || text.Contains("    DropAnnounce:")
                || text.Contains("    NoConsume:")
                || text.Contains("    DropEffect:")
                || text.Contains("    Delay:")
                || text.Contains("    Duration:")
                || text.Contains("    Status:")
                || text.Contains("    Stack:")
                || text.Contains("    Amount:")
                || text.Contains("    Inventory:")
                || text.Contains("    Cart:")
                || text.Contains("    Storage:")
                || text.Contains("    GuildStorage:")
                || text.Contains("    NoUse:")
                || text.Contains("    Override:")
                || text.Contains("    Sitting:")
                || text.Contains("    Trade:")
                || text.Contains("    NoDrop:")
                || text.Contains("    NoTrade:")
                || text.Contains("    TradePartner:")
                || text.Contains("    NoSell:")
                || text.Contains("    NoCart:")
                || text.Contains("    NoStorage:")
                || text.Contains("    NoGuildStorage:")
                || text.Contains("    NoMail:")
                || text.Contains("    NoAuction:")
                || text.Contains("    Script:")
                || text.Contains("    OnEquip_Script:")
                || text.Contains("    OnUnequip_Script:"))
                text = string.Empty;

            if (text.Contains("  - Id:"))
            {
                text = SpacingRemover.Remove(text);

                _id = text.Replace("-Id:", string.Empty);

                itemDatabase.id = int.Parse(_id);

                if (!_isFastConvert)
                    _itemListContainer.allItemIds.Add(_id);
            }
            else if (text.Contains("    AegisName:"))
            {
                text = QuoteRemover.Remove(text);

                _name = text.Replace("    AegisName: ", string.Empty);

                // To prevent some aegis name that had double spacebar format
                _name = SpacingRemover.Remove(_name);

                itemDatabase.aegisName = _name.ToLower();
            }
            else if (text.Contains("    Name:"))
            {
                text = QuoteRemover.Remove(text);

                _name = text.Replace("    Name: ", string.Empty);

                itemDatabase.name = _name;

                if (_itemDatabases.ContainsKey(itemDatabase.id))
                    Debug.LogWarning("Found duplicated item ID: " + itemDatabase.id + " Please tell rAthena about this.");
                else
                    _itemDatabases.Add(itemDatabase.id, itemDatabase);

                if (_aegisNameDatabases.ContainsKey(itemDatabase.aegisName))
                    Debug.LogWarning("Found duplicated item aegis name: " + itemDatabase.aegisName + " Please tell rAthena about this.");
                else
                    _aegisNameDatabases.Add(itemDatabase.aegisName, itemDatabase.id);

                itemDatabase = new ItemDatabase();
            }
            else if (text.Contains("    Type:"))
            {
                text = QuoteRemover.Remove(text);

                text = SpacingRemover.Remove(text);

                text = text.Replace("Type:", string.Empty);

                isArmor = false;

                if (text.ToLower() == "weapon")
                {
                    if (!_isFastConvert)
                        _itemListContainer.weaponIds.Add(_id);
                }
                else if (text.ToLower() == "armor")
                    isArmor = true;
            }
            // Locations
            else if (isArmor)
            {
                text = QuoteRemover.Remove(text);

                text = SpacingRemover.Remove(text);

                if (text.ToLower().Contains("costume_head_top")
                    || text.ToLower().Contains("costume_head_mid")
                    || text.ToLower().Contains("costume_head_low")
                    || text.ToLower().Contains("costume_garment")
                    || text.ToLower().Contains("shadow_armor")
                    || text.ToLower().Contains("shadow_weapon")
                    || text.ToLower().Contains("shadow_shield")
                    || text.ToLower().Contains("shadow_shoes")
                    || text.ToLower().Contains("shadow_right_accessory")
                    || text.ToLower().Contains("shadow_left_accessory"))
                {
                    if (!_isFastConvert)
                        _itemListContainer.costumeIds.Add(_id);

                    // Always clear isArmor
                    isArmor = false;
                }
                else if (text.ToLower().Contains("head_top")
                    || text.ToLower().Contains("head_mid")
                    || text.ToLower().Contains("head_low")
                    || text.ToLower().Contains("armor")
                    || text.ToLower().Contains("left_hand")
                    || text.ToLower().Contains("garment")
                    || text.ToLower().Contains("shoes")
                    || text.ToLower().Contains("right_accessory")
                    || text.ToLower().Contains("left_accessory")
                    || text.ToLower().Contains("both_accessory"))
                {
                    if (!_isFastConvert)
                        _itemListContainer.equipmentIds.Add(_id);

                    // Always clear isArmor
                    isArmor = false;
                }
            }
            // Locations
            else
            {
                text = QuoteRemover.Remove(text);

                text = SpacingRemover.Remove(text);

                if (!_isFastConvert
                    &&
                    (text.ToLower().Contains("costume_head_top")
                    || text.ToLower().Contains("costume_head_mid")
                    || text.ToLower().Contains("costume_head_low")
                    || text.ToLower().Contains("costume_garment")
                    || text.ToLower().Contains("shadow_armor")
                    || text.ToLower().Contains("shadow_weapon")
                    || text.ToLower().Contains("shadow_shield")
                    || text.ToLower().Contains("shadow_shoes")
                    || text.ToLower().Contains("shadow_right_accessory")
                    || text.ToLower().Contains("shadow_left_accessory")))
                    _itemListContainer.costumeIds.Add(_id);
            }
        }

        Debug.Log("There are " + _itemDatabases.Count + " item database.");
        Debug.Log("There are " + _itemListContainer.weaponIds.Count + " weapon database.");
        Debug.Log("There are " + _itemListContainer.equipmentIds.Count + " equipment database.");
        Debug.Log("There are " + _itemListContainer.costumeIds.Count + " costume database.");
        Debug.Log("There are " + _itemListContainer.cardIds.Count + " card database.");
        Debug.Log("There are " + _itemListContainer.card2Ids.Count + " card2 database.");
        Debug.Log("There are " + _itemListContainer.enchantIds.Count + " enchant database.");
        Debug.Log("There are " + _itemListContainer.enchant2Ids.Count + " enchant2 database.");
    }
    /// <summary>
    /// Parse pet database file into converter
    /// </summary>
    void FetchPet()
    {
        var path = Application.dataPath + "/Assets/pet_db.yml";

        // Is file exists?
        if (!File.Exists(path))
        {
            _errorLog = path + " " + _localization.GetTexts(Localization.NOT_FOUND);

            Debug.Log(_errorLog);

            _isFilesError = true;

            return;
        }

        var petDatabasesFile = File.ReadAllText(path);

        var petDatabases = petDatabasesFile.Split('\n');

        var monsterDatabase = new MonsterDatabase();

        for (int i = 0; i < petDatabases.Length; i++)
        {
            var text = petDatabases[i];

            if (string.IsNullOrEmpty(text)
                || string.IsNullOrWhiteSpace(text))
                continue;

            text = CommentRemover.Fix(text);

            text = LineEndingsRemover.Fix(text);

            if (text.Contains("  - Mob:"))
            {
                monsterDatabase = new MonsterDatabase();

                monsterDatabase.name = SpacingRemover.Remove(text).Replace("-Mob:", string.Empty);
            }
            else if (text.Contains("    CaptureRate:"))
            {
                if (_monsterIdsByAegisName.ContainsKey(monsterDatabase.name))
                    _monsterDatabases[_monsterIdsByAegisName[monsterDatabase.name]].captureRate = int.Parse(QuoteRemover.Remove(text.Replace("    CaptureRate: ", string.Empty))) / 100;
            }
        }
    }

    /// <summary>
    /// For my own purpose used only
    /// </summary>
    void TurnSkillNameIntoDescription()
    {
        var path = Application.dataPath + "/Assets/skill_tree.txt";

        // Is file exists?
        if (!File.Exists(path))
        {
            _errorLog = path + " " + _localization.GetTexts(Localization.NOT_FOUND);

            Debug.Log(_errorLog);

            _isFilesError = true;

            return;
        }

        var skillNameDatabasesFile = File.ReadAllText(path);

        var skillNameDatabases = skillNameDatabasesFile.Split('\n');

        List<string> availableSkills = new List<string>();
        for (int i = 0; i < skillNameDatabases.Length; i++)
        {
            if (!availableSkills.Contains(skillNameDatabases[i]))
                availableSkills.Add(skillNameDatabases[i]);
        }

        StringBuilder builder = new StringBuilder();
        StringBuilder builder2 = new StringBuilder();

        builder.Append("setarray $skillDesc$[0],");
        builder2.Append("setarray $allSkills$[0],");

        for (int i = 0; i < availableSkills.Count; i++)
        {
            var text = availableSkills[i];

            text = CommentRemover.Fix(text);

            text = LineEndingsRemover.Fix(text);

            if (!_skillDatabasesByName.ContainsKey(text))
                continue;

            builder.Append("\"" + _skillDatabasesByName[text].description + "\",");
            builder2.Append("\"" + _skillDatabasesByName[text].name + "\",");
        }

        builder.Remove(builder.Length - 1, 1);
        builder2.Remove(builder2.Length - 1, 1);

        builder.Append(";\n");
        builder2.Append(";\n");

        File.WriteAllText("skill_desc.txt", builder.ToString(), Encoding.UTF8);
        File.WriteAllText("skill_name.txt", builder2.ToString(), Encoding.UTF8);

        Debug.Log("'skill_desc.txt' has been successfully created.");
        Debug.Log("'skill_name.txt' has been successfully created.");

    }

    // Converting

    /// <summary>
    /// Start converting process
    /// </summary>
    void Convert()
    {
        StartCoroutine(ConvertCoroutine());
    }

    IEnumerator ConvertCoroutine()
    {
        if (File.Exists("itemInfo_Sak.lub"))
            File.Delete("itemInfo_Sak.lub");
        if (File.Exists("itemInfo_true.lub"))
            File.Delete("itemInfo_true.lub");
        if (File.Exists("itemInfo_Debug.txt"))
            File.Delete("itemInfo_Debug.txt");

        var path = Application.dataPath + "/Assets/item_db_equip.yml";
        var path2 = Application.dataPath + "/Assets/item_db_usable.yml";
        var path3 = Application.dataPath + "/Assets/item_db_etc.yml";
        var path4 = Application.dataPath + "/Assets/item_db_custom.txt";
        var path5 = Application.dataPath + "/Assets/item_db_test.txt";

        var itemDatabasesFile = (_isSkipNormalEquipEtcCombo ? string.Empty : File.ReadAllText(path)) + "\n"
            + File.ReadAllText(path2) + "\n"
            + File.ReadAllText(path3) + "\n"
            + File.ReadAllText(path4);

        if (_isOnlyUseTestTextAsset
            && File.Exists(path5))
            itemDatabasesFile = File.ReadAllText(path5);

        if (_isOnlyUseCustomTextAsset)
            itemDatabasesFile = File.ReadAllText(path4);

        var itemDatabases = itemDatabasesFile.Split('\n');

        _itemContaianerDatabases = new Dictionary<int, ItemContainer>();

        _itemContainers = new List<ItemContainer>();

        _itemContainer = new ItemContainer();

        StringBuilder builder = new StringBuilder();

        bool isDelay = false;

        for (int i = 0; i < itemDatabases.Length; i++)
        {
            if (i % 1000 == 0)
            {
                _txtConvertProgression.text = _localization.GetTexts(Localization.CONVERT_PROGRESSION_PLEASE_WAIT) + ".. (Input " + ((float)i / (float)itemDatabases.Length).ToString("f0") + "%)";

                yield return null;
            }

            var text = CommentRemover.FixCommentSeperateLine(itemDatabases, i);

            var nextText = ((i + 1) < itemDatabases.Length) ? itemDatabases[i + 1] : string.Empty;

            var nextNextText = ((i + 2) < itemDatabases.Length) ? itemDatabases[i + 2] : string.Empty;

            // Skip
            if (text.Contains("    AegisName:")
                || text.Contains("    Sell:")
                || text.Contains("    Jobs:")
                || text.Contains("    Classes:")
                || text.Contains("    Locations:")
                || text.Contains("    AliasName:")
                || text.Contains("    Flags:")
                || text.Contains("    BuyingStore:")
                || text.Contains("    DeadBranch:")
                || text.Contains("    Container:")
                || text.Contains("    UniqueId:")
                || text.Contains("    BindOnEquip:")
                || text.Contains("    DropAnnounce:")
                || text.Contains("    NoConsume:")
                || text.Contains("    DropEffect:")
                || text.Contains("    Status:")
                || text.Contains("    Stack:")
                || text.Contains("    Amount:")
                || text.Contains("    Inventory:")
                || text.Contains("    Cart:")
                || text.Contains("    Storage:")
                || text.Contains("    GuildStorage:")
                || text.Contains("    NoUse:")
                || text.Contains("    Override:")
                || text.Contains("    Sitting:")
                || text.Contains("    Trade:")
                || text.Contains("    NoDrop:")
                || text.Contains("    NoTrade:")
                || text.Contains("    TradePartner:")
                || text.Contains("    NoSell:")
                || text.Contains("    NoCart:")
                || text.Contains("    NoStorage:")
                || text.Contains("    NoGuildStorage:")
                || text.Contains("    NoMail:")
                || text.Contains("    NoAuction:")
                || text.Contains("    Script:")
                || text.Contains("    OnEquip_Script:")
                || text.Contains("    OnUnequip_Script:"))
            {
                if (text.Contains("    Jobs:"))
                {
                    _itemContainer.isJob = true;
                    _itemContainer.isClass = false;
                    _itemContainer.isScript = false;
                    _itemContainer.isEquipScript = false;
                    _itemContainer.isUnequipScript = false;
                }
                else if (text.Contains("    Classes:"))
                {
                    _itemContainer.isJob = false;
                    _itemContainer.isClass = true;
                    _itemContainer.isScript = false;
                    _itemContainer.isEquipScript = false;
                    _itemContainer.isUnequipScript = false;
                }
                else if (text.Contains("    Script:"))
                {
                    ResetRefineGrade();
                    _itemContainer.isJob = false;
                    _itemContainer.isClass = false;
                    _itemContainer.isScript = true;
                    _itemContainer.isEquipScript = false;
                    _itemContainer.isUnequipScript = false;
                }
                else if (text.Contains("    OnEquip_Script:"))
                {
                    ResetRefineGrade();
                    _itemContainer.isJob = false;
                    _itemContainer.isClass = false;
                    _itemContainer.isScript = false;
                    _itemContainer.isEquipScript = true;
                    _itemContainer.isUnequipScript = false;
                }
                else if (text.Contains("    OnUnequip_Script:"))
                {
                    ResetRefineGrade();
                    _itemContainer.isJob = false;
                    _itemContainer.isClass = false;
                    _itemContainer.isScript = false;
                    _itemContainer.isEquipScript = false;
                    _itemContainer.isUnequipScript = true;
                }

                text = string.Empty;
            }

            // Id
            if (text.Contains("  - Id:"))
            {
                text = SpacingRemover.Remove(text);

                _itemContainer.id = text.Replace("-Id:", string.Empty);
            }
            // Name
            else if (text.Contains("    Name:"))
            {
                _itemContainer.name = text.Replace("    Name: ", string.Empty);

                _itemContainer.name = QuoteRemover.Remove(_itemContainer.name);

                // Hotfix for →
                _itemContainer.name = _itemContainer.name.Replace("→", " to ");
            }
            // Delay
            else if (text.Contains("    Delay:"))
                isDelay = true;
            // Delay
            else if (isDelay
                && text.Contains("      Duration:"))
            {
                isDelay = false;

                text = QuoteRemover.Remove(text);

                text = SpacingRemover.Remove(text);

                text = text.Replace("Duration:", string.Empty);

                _itemContainer.delay = int.Parse(text);
            }
            // Type
            else if (text.Contains("    Type:"))
            {
                _itemContainer.type = text.Replace("    Type: ", string.Empty);

                if (!_isFastConvert && !string.IsNullOrEmpty(_itemContainer.type))
                {
                    if ((_itemContainer.type.ToLower() == "petegg")
                        && !_itemListContainer.petEggIds.Contains(_itemContainer.id))
                        _itemListContainer.petEggIds.Add(_itemContainer.id);
                    else if ((_itemContainer.type.ToLower() == "petarmor")
                        && !_itemListContainer.petArmorIds.Contains(_itemContainer.id))
                        _itemListContainer.petArmorIds.Add(_itemContainer.id);
                }
            }
            // SubType
            else if (text.Contains("    SubType:"))
                _itemContainer.subType = text.Replace("    SubType: ", string.Empty).Replace("Enchant    ", "Enchant");
            // Buy
            else if (text.Contains("    Buy:"))
            {
                text = text.Replace("    Buy: ", string.Empty);

                int buy = 0;

                if (int.TryParse(text, out buy))
                    _itemContainer.buy = buy.ToString("n0");
                else
                    _itemContainer.buy = TryParseInt(text);
            }
            // Weight
            else if (text.Contains("    Weight:"))
            {
                text = text.Replace("    Weight: ", string.Empty);

                _itemContainer.weight = TryParseInt(text, 10);
            }
            // Attack
            else if (text.Contains("    Attack:"))
                _itemContainer.attack = text.Replace("    Attack: ", string.Empty);
            // Magic Attack
            else if (text.Contains("    MagicAttack:"))
                _itemContainer.magicAttack = text.Replace("    MagicAttack: ", string.Empty);
            // Defense
            else if (text.Contains("    Defense:"))
                _itemContainer.defense = text.Replace("    Defense: ", string.Empty);
            // Range
            else if (text.Contains("    Range:"))
                _itemContainer.range = text.Replace("    Range: ", string.Empty);
            // Slots
            else if (text.Contains("    Slots:"))
                _itemContainer.slots = text.Replace("    Slots: ", string.Empty);
            // Jobs
            else if (_itemContainer.isJob && text.Contains("      All: true"))
                _itemContainer.jobs += GetAvailableJobBullet + _localization.GetTexts(Localization.JOBS_ALL_JOB) + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      All: false"))
                _itemContainer.jobs += GetAvailableJobBullet + _localization.GetTexts(Localization.JOBS_ALL_JOB) + " [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Acolyte: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Acolyte" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Acolyte: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Acolyte [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Alchemist: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Alchemist" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Alchemist: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Alchemist [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Archer: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Archer" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Archer: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Archer [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Assassin: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Assassin" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Assassin: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Assassin [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      BardDancer: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Bard & Dancer" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      BardDancer: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Bard & Dancer [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Blacksmith: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Blacksmith" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Blacksmith: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Blacksmith [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Crusader: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Crusader" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Crusader: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Crusader [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Gunslinger: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Gunslinger" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Gunslinger: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Gunslinger [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Hunter: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Hunter" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Hunter: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Hunter [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      KagerouOboro: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Kagerou & Oboro" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      KagerouOboro: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Kagerou & Oboro [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Shinkiro: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Shinkiro" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Shinkiro: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Shinkiro [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Shiranui: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Shiranui" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Shiranui: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Shiranui [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Knight: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Knight" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Knight: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Knight [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Mage: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Mage" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Mage: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Mage [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Merchant: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Merchant" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Merchant: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Merchant [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Monk: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Monk" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Monk: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Monk [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Ninja: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Ninja" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Ninja: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Ninja [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Novice: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Novice" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Novice: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Novice [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Priest: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Priest" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Priest: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Priest [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Rebellion: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Rebellion" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Rebellion: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Rebellion [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Night_Watch: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Night Watch" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Night_Watch: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Night Watch [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Rogue: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Rogue" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Rogue: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Rogue [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Sage: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Sage" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Sage: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Sage [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      SoulLinker: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Soul Linker" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      SoulLinker: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Soul Linker [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Soul_Ascetic: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Soul Ascetic" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Soul_Ascetic: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Soul Ascetic [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      StarGladiator: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Star Gladiator" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      StarGladiator: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Star Gladiator [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Sky_Emperor: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Sky Emperor" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Sky_Emperor: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Sky Emperor [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Summoner: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Summoner" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Summoner: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Summoner [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Spirit_Handler: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Spirit Handler" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Spirit_Handler: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Spirit Handler [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      SuperNovice: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Super Novice" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      SuperNovice: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Super Novice [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Hyper_Novice: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Hyper Novice" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Hyper_Novice: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Hyper Novice [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Swordman: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Swordman" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Swordman: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Swordman [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Taekwon: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Taekwon" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Taekwon: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Taekwon [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Thief: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Thief" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Thief: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Thief [x]" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Wizard: true"))
                _itemContainer.jobs += GetAvailableJobBullet + "Wizard" + GetAvailableJobSeperator;
            else if (_itemContainer.isJob && text.Contains("      Wizard: false"))
                _itemContainer.jobs += GetAvailableJobBullet + "Wizard [x]" + GetAvailableJobSeperator;
            // Classes
            else if (_itemContainer.isClass && text.Contains("      All: true"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASSES_ALL_CLASS) + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      All: false"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASSES_ALL_CLASS) + " [x]" + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      Normal: true"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 1" + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      Normal: false"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 1 [x]" + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      Upper: true"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 2" + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      Upper: false"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 2 [x]" + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      Baby: true"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 1 " + _localization.GetTexts(Localization.OR) + " 2 " + _localization.GetTexts(Localization.CLASSES_BABY) + "" + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      Baby: false"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 1 " + _localization.GetTexts(Localization.OR) + " 2 " + _localization.GetTexts(Localization.CLASSES_BABY) + " [x]" + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      Third: true"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 3" + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      Third: false"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 3 [x]" + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      Third_Upper: true"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 3 " + _localization.GetTexts(Localization.CLASSES_TRANS) + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      Third_Upper: false"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 3 " + _localization.GetTexts(Localization.CLASSES_TRANS) + " [x]" + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      Third_Baby: true"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 3 " + _localization.GetTexts(Localization.CLASSES_BABY) + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      Third_Baby: false"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 3 " + _localization.GetTexts(Localization.CLASSES_BABY) + " [x]" + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      All_Upper: true"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 2 " + _localization.GetTexts(Localization.OR) + _localization.GetTexts(Localization.CLASS) + " 3 " + _localization.GetTexts(Localization.CLASSES_TRANS) + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      All_Upper: false"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 2 " + _localization.GetTexts(Localization.OR) + _localization.GetTexts(Localization.CLASS) + " 3 " + _localization.GetTexts(Localization.CLASSES_TRANS) + " [x]" + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      All_Baby: true"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " " + _localization.GetTexts(Localization.CLASSES_BABY) + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      All_Baby: false"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " " + _localization.GetTexts(Localization.CLASSES_BABY) + " [x]" + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      All_Third: true"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 3" + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      All_Third: false"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 3 [x]" + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      Fourth: true"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 4" + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      Fourth: false"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 4 [x]" + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      Fourth_Baby: true"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 4 " + _localization.GetTexts(Localization.CLASSES_BABY) + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      Fourth_Baby: false"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 4 " + _localization.GetTexts(Localization.CLASSES_BABY) + " [x]" + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      All_Fourth: true"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 4" + GetAvailableClassSeperator;
            else if (_itemContainer.isClass && text.Contains("      All_Fourth: false"))
                _itemContainer.classes += GetAvailableClassBullet + _localization.GetTexts(Localization.CLASS) + " 4 [x]" + GetAvailableClassSeperator;
            // Gender
            else if (text.Contains("    Gender: Female"))
                _itemContainer.gender += _localization.GetTexts(Localization.GENDER_FEMALE) + ", ";
            else if (text.Contains("    Gender: Male"))
                _itemContainer.gender += _localization.GetTexts(Localization.GENDER_MALE) + ", ";
            else if (text.Contains("    Gender: Both"))
                _itemContainer.gender += _localization.GetTexts(Localization.GENDER_ALL) + ", ";
            // Location
            else if (text.Contains("      Head_Top: true"))
            {
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_HEAD_TOP) + ", ";
                _itemContainer.debugLocations += "TOP";
            }
            else if (text.Contains("      Head_Top: false"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_HEAD_TOP) + " [x], ";
            else if (text.Contains("      Head_Mid: true"))
            {
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_HEAD_MID) + ", ";
                _itemContainer.debugLocations += "MID";
            }
            else if (text.Contains("      Head_Mid: false"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_HEAD_MID) + " [x], ";
            else if (text.Contains("      Head_Low: true"))
            {
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_HEAD_LOW) + ", ";
                _itemContainer.debugLocations += "LOW";
            }
            else if (text.Contains("      Head_Low: false"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_HEAD_LOW) + " [x], ";
            else if (text.Contains("      Armor: true"))
            {
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_ARMOR) + ", ";
                _itemContainer.debugLocations += "ARMOR";
            }
            else if (text.Contains("      Armor: false"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_ARMOR) + " [x], ";
            else if (text.Contains("      Right_Hand: true"))
                _itemContainer.locations += _localization.GetTexts((_itemContainer.type == "Card") ? Localization.WEAPON : Localization.LOCATION_RIGHT_HAND) + ", ";
            else if (text.Contains("      Right_Hand: false"))
                _itemContainer.locations += _localization.GetTexts((_itemContainer.type == "Card") ? Localization.WEAPON : Localization.LOCATION_RIGHT_HAND) + " [x], ";
            else if (text.Contains("      Left_Hand: true"))
            {
                _itemContainer.locations += _localization.GetTexts((_itemContainer.type == "Card") ? Localization.SHIELD : Localization.LOCATION_LEFT_HAND) + ", ";
                if (_itemContainer.type == "Armor")
                    _itemContainer.debugLocations += "SHIELD";
            }
            else if (text.Contains("      Left_Hand: false"))
                _itemContainer.locations += _localization.GetTexts((_itemContainer.type == "Card") ? Localization.SHIELD : Localization.LOCATION_LEFT_HAND) + " [x], ";
            else if (text.Contains("      Garment: true"))
            {
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_GARMENT) + ", ";
                _itemContainer.debugLocations += "GARMENT";
            }
            else if (text.Contains("      Garment: false"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_GARMENT) + " [x], ";
            else if (text.Contains("      Shoes: true"))
            {
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_SHOES) + ", ";
                _itemContainer.debugLocations += "SHOES";
            }
            else if (text.Contains("      Shoes: false"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_SHOES) + " [x], ";
            else if (text.Contains("      Right_Accessory: true"))
            {
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_RIGHT_ACCESSORY) + ", ";
                _itemContainer.debugLocations += "RA";
            }
            else if (text.Contains("      Right_Accessory: false"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_RIGHT_ACCESSORY) + " [x], ";
            else if (text.Contains("      Left_Accessory: true"))
            {
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_LEFT_ACCESSORY) + ", ";
                _itemContainer.debugLocations += "LA";
            }
            else if (text.Contains("      Left_Accessory: false"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_LEFT_ACCESSORY) + " [x], ";
            else if (text.Contains("      Costume_Head_Top: true"))
            {
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_COSTUME_HEAD_TOP) + ", ";
                _itemContainer.debugLocations += "CTOP";

                if (!_isFastConvert && !_itemListContainer.fashionCostumeIds.Contains(_itemContainer.id))
                    _itemListContainer.fashionCostumeIds.Add(_itemContainer.id);
            }
            else if (text.Contains("      Costume_Head_Top: false"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_COSTUME_HEAD_TOP) + " [x], ";
            else if (text.Contains("      Costume_Head_Mid: true"))
            {
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_COSTUME_HEAD_MID) + ", ";
                _itemContainer.debugLocations += "CMID";

                if (!_isFastConvert && !_itemListContainer.fashionCostumeIds.Contains(_itemContainer.id))
                    _itemListContainer.fashionCostumeIds.Add(_itemContainer.id);
            }
            else if (text.Contains("      Costume_Head_Mid: false"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_COSTUME_HEAD_MID) + " [x], ";
            else if (text.Contains("      Costume_Head_Low: true"))
            {
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_COSTUME_HEAD_LOW) + ", ";
                _itemContainer.debugLocations += "CLOW";

                if (!_isFastConvert && !_itemListContainer.fashionCostumeIds.Contains(_itemContainer.id))
                    _itemListContainer.fashionCostumeIds.Add(_itemContainer.id);
            }
            else if (text.Contains("      Costume_Head_Low: false"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_COSTUME_HEAD_LOW) + " [x], ";
            else if (text.Contains("      Costume_Garment: true"))
            {
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_COSTUME_GARMENT) + ", ";
                _itemContainer.debugLocations += "CGARMENT";

                if (!_isFastConvert && !_itemListContainer.fashionCostumeIds.Contains(_itemContainer.id))
                    _itemListContainer.fashionCostumeIds.Add(_itemContainer.id);
            }
            else if (text.Contains("      Costume_Garment: false"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_COSTUME_GARMENT) + " [x], ";
            else if (text.Contains("      Ammo: true"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_AMMO) + ", ";
            else if (text.Contains("      Ammo: false"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_AMMO) + " [x], ";
            else if (text.Contains("      Shadow_Armor: true"))
            {
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_SHADOW_ARMOR) + ", ";
                _itemContainer.debugLocations += "SARMOR";
            }
            else if (text.Contains("      Shadow_Armor: false"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_SHADOW_ARMOR) + " [x], ";
            else if (text.Contains("      Shadow_Weapon: true"))
            {
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_SHADOW_WEAPON) + ", ";
                _itemContainer.debugLocations += "SWEAPON";
            }
            else if (text.Contains("      Shadow_Weapon: false"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_SHADOW_WEAPON) + " [x], ";
            else if (text.Contains("      Shadow_Shield: true"))
            {
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_SHADOW_SHIELD) + ", ";
                _itemContainer.debugLocations += "SSHIELD";
            }
            else if (text.Contains("      Shadow_Shield: false"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_SHADOW_SHIELD) + " [x], ";
            else if (text.Contains("      Shadow_Shoes: true"))
            {
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_SHADOW_SHOES) + ", ";
                _itemContainer.debugLocations += "SSHOES";
            }
            else if (text.Contains("      Shadow_Shoes: false"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_SHADOW_SHOES) + " [x], ";
            else if (text.Contains("      Shadow_Right_Accessory: true"))
            {
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_SHADOW_RIGHT_ACCESSORY) + ", ";
                _itemContainer.debugLocations += "SRA";
            }
            else if (text.Contains("      Shadow_Right_Accessory: false"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_SHADOW_RIGHT_ACCESSORY) + " [x], ";
            else if (text.Contains("      Shadow_Left_Accessory: true"))
            {
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_SHADOW_LEFT_ACCESSORY) + ", ";
                _itemContainer.debugLocations += "SLA";
            }
            else if (text.Contains("      Shadow_Left_Accessory: false"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_SHADOW_LEFT_ACCESSORY) + " [x], ";
            else if (text.Contains("      Both_Hand: true"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_BOTH_HAND) + ", ";
            else if (text.Contains("      Both_Hand: false"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_BOTH_HAND) + " [x], ";
            else if (text.Contains("      Both_Accessory: true"))
            {
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_BOTH_ACCESSORY) + ", ";
                _itemContainer.debugLocations += "A";
            }
            else if (text.Contains("      Both_Accessory: false"))
                _itemContainer.locations += _localization.GetTexts(Localization.LOCATION_BOTH_ACCESSORY) + " [x], ";
            // Weapon Level
            else if (text.Contains("    WeaponLevel:"))
                _itemContainer.weaponLevel = text.Replace("    WeaponLevel: ", string.Empty);
            // Armor Level
            else if (text.Contains("    ArmorLevel:"))
                _itemContainer.armorLevel = text.Replace("    ArmorLevel: ", string.Empty);
            // Equip Level Min
            else if (text.Contains("    EquipLevelMin:"))
                _itemContainer.equipLevelMinimum = text.Replace("    EquipLevelMin: ", string.Empty);
            // Equip Level Max
            else if (text.Contains("    EquipLevelMax:"))
                _itemContainer.equipLevelMaximum = text.Replace("    EquipLevelMax: ", string.Empty);
            // Refineable
            else if (text.Contains("    Refineable: true"))
                _itemContainer.refinable = _localization.GetTexts(Localization.CAN);
            else if (text.Contains("    Refineable: false"))
                _itemContainer.refinable = _localization.GetTexts(Localization.CANNOT);
            // Gradable
            else if (text.Contains("    Gradable: true"))
                _itemContainer.grable = _localization.GetTexts(Localization.CAN);
            else if (text.Contains("    Gradable: false"))
                _itemContainer.grable = _localization.GetTexts(Localization.CANNOT);
            // View
            else if (text.Contains("    View:"))
            {
                _itemContainer.view = text.Replace("    View: ", string.Empty);

                var itemId = int.Parse(_itemContainer.id);
                if (!_classNumberDatabases.ContainsKey(itemId))
                    _classNumberDatabases.Add(itemId, _itemContainer.view);
                else
                    _classNumberDatabases[itemId] = _itemContainer.view;
            }
            // Script
            else if (_itemContainer.isScript)
            {
                var script = ConvertItemScripts(text);

                if (!string.IsNullOrEmpty(script))
                    _itemContainer.script += "			\"" + script + "\",\n";
            }
            // Equip Script
            else if (_itemContainer.isEquipScript)
            {
                var equipScript = ConvertItemScripts(text);

                if (!string.IsNullOrEmpty(equipScript))
                    _itemContainer.equipScript += "			\"" + equipScript + "\",\n";
            }
            // Unequip Script
            else if (_itemContainer.isUnequipScript)
            {
                var unequipScript = ConvertItemScripts(text);

                if (!string.IsNullOrEmpty(unequipScript))
                    _itemContainer.unequipScript += "			\"" + unequipScript + "\",\n";
            }

            // Store in container
            if (nextText.Contains("- Id:")
                && !string.IsNullOrEmpty(_itemContainer.id)
                && !string.IsNullOrWhiteSpace(_itemContainer.id)
                || ((i + 1) >= itemDatabases.Length))
            {
                if ((_itemContainer.subType == "Bow")
                    && (_itemContainer.locations.Contains(_localization.GetTexts(Localization.LOCATION_RIGHT_HAND))
                    || _itemContainer.locations.Contains(_localization.GetTexts(Localization.LOCATION_LEFT_HAND))))
                    Debug.Log("Item ID: " + _itemContainer.id + " wrong location");

                bool isSkipThisItem = false;
                // Just pick ammo
                if (_isSkipNormalEquipEtcCombo
                    && (int.Parse(_itemContainer.id) < ItemGenerator.Instance.StartId)
                    && (_itemContainer.type == "Etc" || _itemContainer.type == "Card"))
                    isSkipThisItem = true;

                if (!isSkipThisItem)
                {
                    _itemContaianerDatabases.Add(int.Parse(_itemContainer.id), _itemContainer);

                    _itemContainers.Add(_itemContainer);
                }

                _itemContainer = new ItemContainer();

                continue;
            }
        }

        _itemContainers.Sort((a, b) => int.Parse(a.id).CompareTo(int.Parse(b.id)));

        for (int i = 0; i < _itemContainers.Count; i++)
        {
            if (i % 1000 == 0)
            {
                _txtConvertProgression.text = _localization.GetTexts(Localization.CONVERT_PROGRESSION_PLEASE_WAIT) + ".. (Output " + ((float)i / (float)_itemContainers.Count).ToString("f0") + "%)";

                yield return null;
            }

            _itemContainer = _itemContainers[i];

            bool isEquipment = !string.IsNullOrEmpty(_itemContainer.weaponLevel) || !string.IsNullOrEmpty(_itemContainer.armorLevel);
            bool isUsable = IsItemTypeUsable(_itemContainer.type);

            if (_itemContainer.type == "Card")
            {
                if (_itemContainer.subType == "Enchant")
                {
                    if (!string.IsNullOrEmpty(_itemContainer.locations))
                        Debug.Log("Enchantment " + _itemContainer.id + " had location.." + _itemContainer.locations);

                    if (!_isFastConvert)
                    {
                        if (IsContainScripts(_itemContainer))
                            _itemListContainer.enchant2Ids.Add(_itemContainer.id);
                        else
                            _itemListContainer.enchantIds.Add(_itemContainer.id);
                    }
                }
                else if (!_isFastConvert)
                {
                    if (IsContainScripts(_itemContainer))
                        _itemListContainer.card2Ids.Add(_itemContainer.id);
                    else
                        _itemListContainer.cardIds.Add(_itemContainer.id);
                }
            }

            if (!_isFastConvert)
            {
                _itemListContainer.AddSubType(_itemContainer.subType.Replace("CannonBall", "Cannonball"), _itemContainer.id);

                if ((_itemContainer.type.ToLower() == "armor")
                    || (_itemContainer.type.ToLower() == "shadowgear"))
                    _itemListContainer.AddLocation(_itemContainer.debugLocations, _itemContainer.id);
            }

            var itemId = int.Parse(_itemContainer.id);

            var itemIdToGetCombo = itemId;

            if (_itemScriptCopierDatabases.ContainsKey(itemId))
            {
                _itemContainer.script = _itemContaianerDatabases[_itemScriptCopierDatabases[itemId]].script;
                _itemContainer.equipScript = _itemContaianerDatabases[_itemScriptCopierDatabases[itemId]].equipScript;
                _itemContainer.unequipScript = _itemContaianerDatabases[_itemScriptCopierDatabases[itemId]].unequipScript;

                itemIdToGetCombo = _itemScriptCopierDatabases[itemId];
            }

            var resourceName = GetResourceNameFromId(itemId
                , _itemContainer.type
                , _itemContainer.subType
                , !string.IsNullOrEmpty(_itemContainer.locations) ? _itemContainer.locations.Substring(0, _itemContainer.locations.Length - 2) : string.Empty);

            if (_isEquipmentNoValue)
            {
                var itemType = _itemContainer.type.ToLower();
                if ((itemType == "weapon")
                    || (itemType == "armor")
                    || (itemType == "shadowgear")
                    || (itemType == "ammo"))
                {
                    if (itemType != "weapon")
                        _itemContainer.jobs = GetAvailableJobBullet + _localization.GetTexts(Localization.JOBS_ALL_JOB) + GetAvailableJobSeperator;
                    _itemContainer.classes = GetAvailableClassBullet + _localization.GetTexts(Localization.CLASSES_ALL_CLASS) + GetAvailableClassSeperator;
                    _itemContainer.slots = "0";
                    _itemContainer.script = string.Empty;
                    _itemContainer.equipScript = string.Empty;
                    _itemContainer.unequipScript = string.Empty;
                    _itemContainer.attack = string.Empty;
                    _itemContainer.magicAttack = string.Empty;
                    _itemContainer.defense = string.Empty;
                    _itemContainer.equipLevelMinimum = string.Empty;
                    _itemContainer.equipLevelMaximum = string.Empty;
                    _itemContainer.refinable = _localization.GetTexts(Localization.CAN);
                    _itemContainer.grable = _localization.GetTexts(Localization.CAN);
                    _itemContainer.buy = string.Empty;
                }
            }
            else if (_isItemNoBonus)
            {
                var itemType = _itemContainer.type.ToLower();
                if ((itemType == "weapon")
                    || (itemType == "armor")
                    || (itemType == "shadowgear")
                    || (itemType == "ammo")
                    || (itemType == "etc")
                    || (itemType == "card"))
                {
                    if ((itemType != "etc")
                        && (itemType != "card"))
                    {
                        _itemContainer.jobs = GetAvailableJobBullet + _localization.GetTexts(Localization.JOBS_ALL_JOB) + GetAvailableJobSeperator;
                        _itemContainer.classes = GetAvailableClassBullet + _localization.GetTexts(Localization.CLASSES_ALL_CLASS) + GetAvailableClassSeperator;
                        _itemContainer.refinable = _localization.GetTexts(Localization.CAN);
                        _itemContainer.grable = _localization.GetTexts(Localization.CAN);
                    }
                    _itemContainer.slots = "0";
                    _itemContainer.script = string.Empty;
                    _itemContainer.equipScript = string.Empty;
                    _itemContainer.unequipScript = string.Empty;
                    _itemContainer.equipLevelMinimum = string.Empty;
                    _itemContainer.equipLevelMaximum = string.Empty;
                    _itemContainer.buy = string.Empty;
                }
            }

            // Id
            builder.Append("	[" + _itemContainer.id + "] = {\n");
            // Unidentified display name
            builder.Append("		unidentifiedDisplayName = \"" + _itemContainer.name
                +
                (isEquipment
                ? " [" + (!string.IsNullOrEmpty(_itemContainer.slots) ? _itemContainer.slots : "0") + "]"
                : string.Empty) + "\",\n");
            // Unidentified resource name
            builder.Append("		unidentifiedResourceName = " + resourceName + ",\n");
            // Unidentified description
            builder.Append("		unidentifiedDescriptionName = {\n");
            builder.Append("			\"" + (isEquipment
                ? _localization.GetTexts(Localization.UNIDENTIFIED_DESC)
                : string.Empty) + "\"\n");
            builder.Append("		},\n");
            // Identified display name
            builder.Append("		identifiedDisplayName = \"" + _itemContainer.name + "\",\n");
            // Identified resource name
            builder.Append("		identifiedResourceName = " + resourceName + ",\n");
            // Identified description
            builder.Append("		identifiedDescriptionName = {\n");
            // Description
            var comboData = GetItemDatabase(itemIdToGetCombo);
            var comboBonuses = GetCombo((comboData != null) ? comboData.aegisName : string.Empty);

            string hardcodeBonuses = _hardcodeItemScripts.GetHardcodeItemScript(itemId);

            var bonuses = !string.IsNullOrEmpty(hardcodeBonuses)
                ? hardcodeBonuses
                : !string.IsNullOrEmpty(_itemContainer.script)
                ? _itemContainer.script
                : string.Empty;

            var equipBonuses = !string.IsNullOrEmpty(_itemContainer.equipScript)
                ? "			\"^666478[" + _localization.GetTexts(Localization.WHEN_EQUIP) + "]^000000\",\n"
                + _itemContainer.equipScript
                : string.Empty;

            var unequipBonuses = !string.IsNullOrEmpty(_itemContainer.unequipScript)
                ? "			\"^666478[" + _localization.GetTexts(Localization.WHEN_UNEQUIP) + "]^000000\",\n"
                + _itemContainer.unequipScript
                : string.Empty;

            var description = (!_isHideItemId
                ? "			\"^3F28FFID:^000000 " + _itemContainer.id + "\",\n"
                : string.Empty)
                + "			\"^3F28FF" + _localization.GetTexts(Localization.TYPE) + ":^000000 " + _itemContainer.type + "\",\n";


            if (!_isHideSubType)
            {
                if (!string.IsNullOrEmpty(_itemContainer.subType))
                    description += "			\"^3F28FF" + _localization.GetTexts(Localization.SUB_TYPE) + ":^000000 " + _itemContainer.subType + "\",\n";
                else if (_isZeroValuePrintable && isEquipment)
                    description += "			\"^3F28FF" + _localization.GetTexts(Localization.SUB_TYPE) + ":^000000 -\",\n";
            }

            if (!string.IsNullOrEmpty(_itemContainer.locations))
                description += "			\"^3F28FF" + _localization.GetTexts(Localization.LOCATION) + ":^000000 " + _itemContainer.locations.Substring(0, _itemContainer.locations.Length - 2) + "\",\n";
            else if (_isZeroValuePrintable && isEquipment)
                description += "			\"^3F28FF" + _localization.GetTexts(Localization.LOCATION) + ":^000000 -\",\n";

            bool isMultipleJob = _itemContainer.jobs.Split("[NEW_LINE]").Length > 2;
            if (!isMultipleJob)
            {
                _itemContainer.jobs = _itemContainer.jobs.Replace("- ", string.Empty);

                _itemContainer.jobs = _itemContainer.jobs.Replace("— ", string.Empty);
            }
            if (!_isItemUnconditional)
            {
                if (!string.IsNullOrEmpty(_itemContainer.jobs))
                    description += "			\"^3F28FF" + _localization.GetTexts(Localization.JOB) + ":^000000 " + (_isUseNewLineInsteadOfCommaForAvailableJob ? (isMultipleJob ? "[NEW_LINE]" : string.Empty) + _itemContainer.jobs.Substring(0, _itemContainer.jobs.Length - 10) : _itemContainer.jobs.Substring(0, _itemContainer.jobs.Length - 2)) + "\",\n";
                else if (_isZeroValuePrintable && isEquipment)
                    description += "			\"^3F28FF" + _localization.GetTexts(Localization.JOB) + ":^000000 -\",\n";
            }

            bool isMultipleClass = _itemContainer.classes.Split("[NEW_LINE]").Length > 2;
            if (!isMultipleClass)
            {
                _itemContainer.classes = _itemContainer.classes.Replace("- ", string.Empty);

                _itemContainer.classes = _itemContainer.classes.Replace("— ", string.Empty);
            }
            if (!_isItemUnconditional)
            {
                if (!string.IsNullOrEmpty(_itemContainer.classes))
                    description += "			\"^3F28FF" + _localization.GetTexts(Localization.CLASS) + ":^000000 " + (_isUseNewLineInsteadOfCommaForAvailableClass ? (isMultipleClass ? "[NEW_LINE]" : string.Empty) + _itemContainer.classes.Substring(0, _itemContainer.classes.Length - 10) : _itemContainer.classes.Substring(0, _itemContainer.classes.Length - 2)) + "\",\n";
                else if (_isZeroValuePrintable && isEquipment)
                    description += "			\"^3F28FF" + _localization.GetTexts(Localization.CLASS) + ":^000000 -\",\n";

                if (!string.IsNullOrEmpty(_itemContainer.gender))
                    description += "			\"^3F28FF" + _localization.GetTexts(Localization.GENDER) + ":^000000 " + _itemContainer.gender.Substring(0, _itemContainer.gender.Length - 2) + "\",\n";
                else if (_isZeroValuePrintable && isEquipment)
                    description += "			\"^3F28FF" + _localization.GetTexts(Localization.GENDER) + ":^000000 -\",\n";
            }

            if (!string.IsNullOrEmpty(_itemContainer.attack))
                description += "			\"^3F28FF" + _localization.GetTexts(Localization.ATTACK) + ":^000000 " + _itemContainer.attack + "\",\n";
            else if (_isZeroValuePrintable && isEquipment)
                description += "			\"^3F28FF" + _localization.GetTexts(Localization.ATTACK) + ":^000000 -\",\n";

            if (!string.IsNullOrEmpty(_itemContainer.magicAttack))
                description += "			\"^3F28FF" + _localization.GetTexts(Localization.MAGIC_ATTACK) + ":^000000 " + _itemContainer.magicAttack + "\",\n";
            else if (_isZeroValuePrintable && isEquipment)
                description += "			\"^3F28FF" + _localization.GetTexts(Localization.MAGIC_ATTACK) + ":^000000 -\",\n";

            if (!string.IsNullOrEmpty(_itemContainer.defense))
                description += "			\"^3F28FF" + _localization.GetTexts(Localization.DEFENSE) + ":^000000 " + _itemContainer.defense + "\",\n";
            else if (_isZeroValuePrintable && isEquipment)
                description += "			\"^3F28FF" + _localization.GetTexts(Localization.DEFENSE) + ":^000000 -\",\n";

            if (!string.IsNullOrEmpty(_itemContainer.range))
                description += "			\"^3F28FF" + _localization.GetTexts(Localization.RANGE) + ":^000000 " + _itemContainer.range + "\",\n";
            else if (_isZeroValuePrintable && isEquipment)
                description += "			\"^3F28FF" + _localization.GetTexts(Localization.RANGE) + ":^000000 -\",\n";

            if (!string.IsNullOrEmpty(_itemContainer.weaponLevel))
                description += "			\"^3F28FF" + _localization.GetTexts(Localization.WEAPON_LEVEL) + ":^000000 " + _itemContainer.weaponLevel + "\",\n";
            else if (_isZeroValuePrintable && isEquipment)
                description += "			\"^3F28FF" + _localization.GetTexts(Localization.WEAPON_LEVEL) + ":^000000 -\",\n";

            if (!string.IsNullOrEmpty(_itemContainer.armorLevel))
                description += "			\"^3F28FF" + _localization.GetTexts(Localization.ARMOR_LEVEL) + ":^000000 " + _itemContainer.armorLevel + "\",\n";
            else if (_isZeroValuePrintable && isEquipment)
                description += "			\"^3F28FF" + _localization.GetTexts(Localization.ARMOR_LEVEL) + ":^000000 -\",\n";

            if (!_isSkipEquipLevel)
            {
                if (!string.IsNullOrEmpty(_itemContainer.equipLevelMinimum))
                    description += "			\"^3F28FF" + _localization.GetTexts(Localization.MINIMUM_EQUIP_LEVEL) + ":^000000 " + _itemContainer.equipLevelMinimum + "\",\n";
                else if (_isZeroValuePrintable && isEquipment)
                    description += "			\"^3F28FF" + _localization.GetTexts(Localization.MINIMUM_EQUIP_LEVEL) + ":^000000 -\",\n";

                if (!string.IsNullOrEmpty(_itemContainer.equipLevelMaximum))
                    description += "			\"^3F28FF" + _localization.GetTexts(Localization.MAXIMUM_EQUIP_LEVEL) + ":^000000 " + _itemContainer.equipLevelMaximum + "\",\n";
                else if (_isZeroValuePrintable && isEquipment)
                    description += "			\"^3F28FF" + _localization.GetTexts(Localization.MAXIMUM_EQUIP_LEVEL) + ":^000000 -\",\n";
            }

            if (!_isHideRefinable)
            {
                if (!string.IsNullOrEmpty(_itemContainer.refinable))
                    description += "			\"^3F28FF" + _localization.GetTexts(Localization.REFINABLE) + ":^000000 " + _itemContainer.refinable + "\",\n";
                else if (_isZeroValuePrintable && isEquipment)
                    description += "			\"^3F28FF" + _localization.GetTexts(Localization.REFINABLE) + ":^000000 -\",\n";
            }

            if (!_isHideGradable)
            {
                if (!string.IsNullOrEmpty(_itemContainer.grable))
                    description += "			\"^3F28FF" + _localization.GetTexts(Localization.GRADABLE) + ":^000000 " + _itemContainer.grable + "\",\n";
                else if (_isZeroValuePrintable && isEquipment)
                    description += "			\"^3F28FF" + _localization.GetTexts(Localization.GRADABLE) + ":^000000 -\",\n";
            }

            if (!string.IsNullOrEmpty(_itemContainer.weight))
                description += "			\"^3F28FF" + _localization.GetTexts(Localization.WEIGHT) + ":^000000 " + _itemContainer.weight + "\",\n";
            else if (_isZeroValuePrintable)
                description += "			\"^3F28FF" + _localization.GetTexts(Localization.WEIGHT) + ":^000000 -\",\n";

            if (_itemContainer.delay > 0)
                description += "			\"^3F28FF" + _localization.GetTexts(Localization.DELAY) + ":^000000 " + TryParseTimer((_itemContainer.delay / 1000).ToString()) + "\",\n";
            else if (_isZeroValuePrintable && isUsable)
                description += "			\"^3F28FF" + _localization.GetTexts(Localization.DELAY) + ":^000000 -\",\n";

            if (!string.IsNullOrEmpty(_itemContainer.buy))
                description += "			\"^3F28FF" + _localization.GetTexts(Localization.PRICE) + ":^000000 " + _itemContainer.buy + "\",\n";
            else if (_isZeroValuePrintable)
                description += "			\"^3F28FF" + _localization.GetTexts(Localization.PRICE) + ":^000000 -\",\n";

            if (_isEnchantmentAbleToUse
                && (_itemContainer.subType == "Enchant"))
                builder.Append("			\"^9390C5กดใช้เพื่อ ผนึกใส่ อุปกรณ์สวมใส่ที่ไม่มีรู^000000\",\n");

            builder.Append(bonuses);

            if (!string.IsNullOrEmpty(bonuses)
                && !string.IsNullOrWhiteSpace(bonuses))
                builder.Append("			\"————————————\",\n");

            if (!_isEquipmentNoValue && !_isItemNoBonus)
                builder.Append(comboBonuses);

            builder.Append(equipBonuses);

            builder.Append(unequipBonuses);

            builder.Append(description);

            builder.Append("			\"\"\n");

            builder.Append("		},\n");

            // Slot Count
            if (!string.IsNullOrEmpty(_itemContainer.slots))
                builder.Append("		slotCount = " + _itemContainer.slots + ",\n");
            else
                builder.Append("		slotCount = 0,\n");

            // View / Class Number
            builder.Append("		ClassNum = " + GetClassNumFromId(_itemContainer) + ",\n");

            // Costume
            builder.Append("		costume = " + IsCostumeFromId(_itemContainer) + "\n");

            builder.Append("	},\n");
        }

        builder.Remove(builder.Length - 2, 1);

        var prefix = "tbl = {\n";
        var postfix = "}\n\n" +
            "-- Function #0\n" +
            "main = function()\n" +
            "	for ItemID, DESC in pairs(tbl) do\n" +
            "		result, msg = AddItem(ItemID, DESC.unidentifiedDisplayName, DESC.unidentifiedResourceName, DESC.identifiedDisplayName, DESC.identifiedResourceName, DESC.slotCount, DESC.ClassNum)\n" +
            "		if not result == true then\n" +
            "			return false, msg\n" +
            "		end\n" +
            "		for k, v in pairs(DESC.unidentifiedDescriptionName) do\n" +
            "			result, msg = AddItemUnidentifiedDesc(ItemID, v)\n" +
            "			if not result == true then\n" +
            "				return false, msg\n" +
            "			end\n" +
            "		end\n" +
            "		for k, v in pairs(DESC.identifiedDescriptionName) do\n" +
            "			result, msg = AddItemIdentifiedDesc(ItemID, v)\n" +
            "			if not result == true then\n" +
            "				return false, msg\n" +
            "			end\n" +
            "		end\n" +
            "		if nil ~= DESC.EffectID then\n" +
            "			result, msg = AddItemEffectInfo(ItemID, DESC.EffectID)\n" +
            "			if not result == true then\n" +
            "				return false, msg\n" +
            "			end\n" +
            "		end\n" +
            "		if nil ~= DESC.costume then\n" +
            "			result, msg = AddItemIsCostume(ItemID, DESC.costume)\n" +
            "			if not result == true then\n" +
            "				return false, msg\n" +
            "			end\n" +
            "		end\n" +
            "		k = DESC.unidentifiedResourceName\n" +
            "		v = DESC.identifiedDisplayName\n" +
            "	end\n" +
            "	return true, \"good\"\n" +
            "end\n" +
            "\n" +
            "-- Function #1\n" +
            "main_server = function()\n" +
            "	for ItemID, DESC in pairs(tbl) do\n" +
            "		result, msg = AddItem(ItemID, DESC.identifiedDisplayName, DESC.slotCount)\n" +
            "		if not result == true then\n" +
            "			return false, msg\n" +
            "		end\n" +
            "	end\n" +
            "	return true, \"good\"\n" +
            "end\n";

        string finalize = prefix + builder.ToString() + postfix;

        // New line
        finalize = finalize.Replace("[NEW_LINE]", "\",\n			\"");

        // TODO: Fix it properly
        finalize = finalize.Replace("ID ถ้าไม่ใช่", "ID:");
        finalize = finalize.Replace(_localization.GetTexts(Localization.INFINITE) + _localization.GetTexts(Localization.SECOND_ABBREVIATION), _localization.GetTexts(Localization.INFINITE));
        finalize = finalize.Replace(_localization.GetTexts(Localization.WITH) + " 11)", _localization.GetTexts(Localization.TYPE) + ")");
        finalize = finalize.Replace(_localization.GetTexts(Localization.WITH) + " 11 )", _localization.GetTexts(Localization.TYPE) + ")");
        finalize = finalize.Replace(_localization.GetTexts(Localization.WITH) + " II_VIEW)", _localization.GetTexts(Localization.TYPE) + ")");
        finalize = finalize.Replace(_localization.GetTexts(Localization.WITH) + " II_VIEW )", _localization.GetTexts(Localization.TYPE) + ")");
        finalize = finalize.Replace(_localization.GetTexts(Localization.WITH) + " ITEMINFO_VIEW)", _localization.GetTexts(Localization.TYPE) + ")");
        finalize = finalize.Replace(_localization.GetTexts(Localization.WITH) + " ITEMINFO_VIEW )", _localization.GetTexts(Localization.TYPE) + ")");

        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 23"
            , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " Two-handed Staff");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 22"
            , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " Huuma");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 21"
           , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " Grenade Launcher");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 20"
          , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " Shotgun");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 19"
        , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " Gatling Gun");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 18"
         , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " Rifle");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 17"
            , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " Revolver");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 16"
            , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " Katar");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 15"
            , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " Book");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 14"
            , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " Whip");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 13"
            , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " Instrument");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 12"
            , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " Knuckle");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 11"
            , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " Bow");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 10"
            , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " One-handed Staff");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 9"
            , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " Two-handed Mace");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 8"
            , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " One-handed Mace");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 7"
            , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " Two-handed Axe");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 6"
            , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " One-handed Axe");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 5"
            , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " Two-handed Spear");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 4"
            , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " One-handed Spear");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 3"
            , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " Two-handed Sword");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 2"
            , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " One-handed Sword");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 1"
          , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " Dagger");
        finalize = finalize.Replace(_localization.GetTexts(Localization.TYPE) + ") " + _localization.GetTexts(Localization.EQUAL) + " 0"
           , _localization.GetTexts(Localization.TYPE) + " " + _localization.GetTexts(Localization.EQUAL) + " Fist");

        // Spacing fix
        finalize = finalize.Replace("     •", "•");
        finalize = finalize.Replace("    •", "•");
        finalize = finalize.Replace("   •", "•");
        finalize = finalize.Replace("  •", "•");
        finalize = finalize.Replace(" •", "•");
        finalize = finalize.Replace("\"     ^FF2525" + _localization.GetTexts(Localization.IF) + "^000000 (", "\"^FF2525" + _localization.GetTexts(Localization.IF) + "^000000(");
        finalize = finalize.Replace("\"    ^FF2525" + _localization.GetTexts(Localization.IF) + "^000000 (", "\"^FF2525" + _localization.GetTexts(Localization.IF) + "^000000(");
        finalize = finalize.Replace("\"   ^FF2525" + _localization.GetTexts(Localization.IF) + "^000000 (", "\"^FF2525" + _localization.GetTexts(Localization.IF) + "^000000(");
        finalize = finalize.Replace("\"  ^FF2525" + _localization.GetTexts(Localization.IF) + "^000000 (", "\"^FF2525" + _localization.GetTexts(Localization.IF) + "^000000(");
        finalize = finalize.Replace("\" ^FF2525" + _localization.GetTexts(Localization.IF) + "^000000 (", "\"^FF2525" + _localization.GetTexts(Localization.IF) + "^000000(");
        finalize = finalize.Replace("\"     ^FF2525" + _localization.GetTexts(Localization.IF) + "^000000(", "\"^FF2525" + _localization.GetTexts(Localization.IF) + "^000000(");
        finalize = finalize.Replace("\"    ^FF2525" + _localization.GetTexts(Localization.IF) + "^000000(", "\"^FF2525" + _localization.GetTexts(Localization.IF) + "^000000(");
        finalize = finalize.Replace("\"   ^FF2525" + _localization.GetTexts(Localization.IF) + "^000000(", "\"^FF2525" + _localization.GetTexts(Localization.IF) + "^000000(");
        finalize = finalize.Replace("\"  ^FF2525" + _localization.GetTexts(Localization.IF) + "^000000(", "\"^FF2525" + _localization.GetTexts(Localization.IF) + "^000000(");
        finalize = finalize.Replace("\" ^FF2525" + _localization.GetTexts(Localization.IF) + "^000000(", "\"^FF2525" + _localization.GetTexts(Localization.IF) + "^000000(");

        // Write it out
        File.WriteAllText("itemInfo_Sak.lub", finalize, _localization.GetCurrentEncoding);
        File.WriteAllText("itemInfo_true.lub", finalize, _localization.GetCurrentEncoding);
        File.WriteAllText("itemInfo_Debug.txt", finalize, _localization.GetCurrentEncoding);

        Debug.Log("Files has been successfully created.");

        if (!_isFastConvert)
        {
            ExportItemLists();

            ExportItemMall();

            ExportMonsterLists();

            ExportSkillLists();

            ExportErrorLists();
        }

        Debug.Log(DateTime.Now);

        _txtConvertProgression.text = _localization.GetTexts(Localization.CONVERT_PROGRESSION_DONE) + "!!";
        yield return null;
    }

    /// <summary>
    /// Convert item scripts
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    string ConvertItemScripts(string text)
    {
        // Comment fix
        int commentFixRetry = 30;
        while (text.Contains("/*") && (commentFixRetry > 0))
        {
            var copier = text;
            if (!copier.Contains("*/"))
                text = copier.Substring(0, copier.IndexOf("/*"));
            else
                text = copier.Substring(0, copier.IndexOf("/*")) + copier.Substring(copier.IndexOf("*/") + 2);

            commentFixRetry--;
        }

        commentFixRetry = 30;
        while (text.Contains("*/") && (commentFixRetry > 0))
        {
            text = text.Replace("*/", string.Empty);

            commentFixRetry--;
        }

        text = text.ToLower();

        if (text.Contains(".@")
            && text.Contains("=")
            && text.Contains(";"))
        {
            bool isFirstTextDot = true;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ')
                    continue;

                if (text[i] != '.')
                {
                    isFirstTextDot = false;
                    break;
                }
                else
                    break;
            }

            bool isFoundOldVariables = false;
            for (int i = 0; i < _replaceVariables.Count; i++)
            {
                if (text.Contains(_replaceVariables[i].variableName))
                {
                    isFoundOldVariables = true;
                    break;
                }
            }

            if (!isFoundOldVariables
                && isFirstTextDot)
            {
                ReplaceVariable replaceVariable = new ReplaceVariable();
                replaceVariable.variableName = text.Substring(0, text.IndexOf("="));
                var description = text.Substring(text.IndexOf("=") + 1);

                while (replaceVariable.variableName.Contains(" "))
                    replaceVariable.variableName = replaceVariable.variableName.Replace(" ", string.Empty);
                while (description.Contains(" "))
                    description = description.Replace(" ", string.Empty);

                // Don't replace too long or too complex variable
                if (!description.Contains("getiteminfo")
                    && !description.Contains("+")
                    && !description.Contains("-")
                    && !description.Contains("*")
                    && !description.Contains("/"))
                {
                    replaceVariable.descriptionConverted = AllInOneParse(description).Replace(";", string.Empty);

                    // Only add string to replace variable list
                    if (!int.TryParse(replaceVariable.descriptionConverted, out _)
                        && !replaceVariable.descriptionConverted.Contains(_localization.GetTexts(Localization.MIN))
                        && !replaceVariable.descriptionConverted.Contains(_localization.GetTexts(Localization.MAX))
                        && !replaceVariable.descriptionConverted.Contains(_localization.GetTexts(Localization.POW)))
                    {
                        _replaceVariables.Add(replaceVariable);
                        _replaceVariables.Sort((a, b) => a.variableName.Length.CompareTo(b.variableName.Length));
                        _replaceVariables.Reverse();

                        text = string.Empty;
                    }
                }
            }
        }

        // End wrong wording fix

        // Comma fix
        int commaFixRetry = 300;
        while (text.Contains("(") && text.Contains(")") && commaFixRetry > 0)
        {
            commaFixRetry--;

            int commaOpenCount = 0;
            int commaCloseCount = 0;
            int currentCommaFixerStartIndex = text.IndexOf('(');

            //Debug.Log("text#1:" + text);

            for (int i = currentCommaFixerStartIndex; i < text.Length; i++)
            {
                if (text[i] == '(')
                    commaOpenCount++;
                else if (text[i] == ')')
                {
                    commaCloseCount++;

                    if (commaCloseCount == commaOpenCount)
                    {
                        var testCommaFixer = text.Substring(currentCommaFixerStartIndex, i - currentCommaFixerStartIndex + 1);
                        var testCommaFixer2 = text.Substring(currentCommaFixerStartIndex, i - currentCommaFixerStartIndex + 1);
                        testCommaFixer2 = testCommaFixer2.Replace('(', '†');
                        testCommaFixer2 = testCommaFixer2.Replace(')', '‡');
                        testCommaFixer2 = testCommaFixer2.Replace(",", " " + _localization.GetTexts(Localization.WITH) + " ");
                        text = text.Replace(testCommaFixer, testCommaFixer2);

                        break;
                    }
                }
            }

            //Debug.Log("text#2:" + text);
        }

        text = text.Replace('†', '(');
        text = text.Replace('‡', ')');

        //Debug.Log("text#3:" + text);

        // End comma fix

        text = text.Replace("      ", string.Empty);

        // autobonus3
        if (text.Contains("autobonus3 \"{"))
        {
            string duplicate = string.Empty;

            var temp = text.Replace("autobonus3 \"{", string.Empty);
            if (temp.IndexOf("}\"") > 0)
            {
                duplicate = temp.Substring(temp.IndexOf("}\""));

                temp = temp.Substring(0, temp.IndexOf("}\""));

                text = text.Replace(temp + "}\"", string.Empty);
            }
            var duplicates = duplicate.Split(',');
            var bonuses = string.Empty;
            var allBonus = temp.Split(';');
            for (int i = 0; i < allBonus.Length; i++)
            {
                if (!string.IsNullOrEmpty(allBonus[i]) && !string.IsNullOrWhiteSpace(allBonus[i]))
                {
                    text = text.Replace(allBonus[i] + ";", string.Empty);
                    bonuses += ConvertItemScripts(allBonus[i]);
                }
            }

            // Find first "{ for other script
            if (duplicate.IndexOf("\"{") > 0)
            {
                duplicate = duplicate.Substring(duplicate.IndexOf("\"{") + 2);
                temp = duplicate.Substring(0, duplicate.IndexOf("}\""));

                while (!string.IsNullOrEmpty(temp))
                {
                    if (temp.IndexOf(';') > 0)
                    {
                        var sumBonus = temp.Substring(0, temp.IndexOf(';'));
                        bonuses += ConvertItemScripts(sumBonus);
                        if (temp.Length > temp.IndexOf(';') + 1)
                            temp = temp.Substring(temp.IndexOf(';') + 1);
                        else
                            temp = string.Empty;
                    }
                    else
                        temp = string.Empty;
                }
            }

            if (!string.IsNullOrEmpty(bonuses) || !string.IsNullOrWhiteSpace(bonuses))
            {
                bonuses = bonuses.Replace("•", "[NEW_LINE]•");
                bonuses = bonuses.Replace("^FF2525", "[NEW_LINE]^FF2525");

                int number = 1;
                while (bonuses.Contains("•"))
                {
                    bonuses = ReplaceOneTime.ReplaceNow(bonuses, "•", number.ToString("f0") + ".)");

                    number++;
                }

                var skillName = QuoteRemover.Remove(duplicates.Length >= 4 ? duplicates[3] : string.Empty);
                var duration = QuoteRemover.Remove(duplicates.Length >= 3 ? duplicates[2] : string.Empty);
                var rate = QuoteRemover.Remove(duplicates.Length >= 2 ? duplicates[1] : string.Empty);
                text = string.Format(_localization.GetTexts(Localization.AUTO_BONUS_3), bonuses, GetSkillName(skillName), TryParseInt(rate, 10), TryParseTimer(TryParseInt(duration, 1000)));
            }
            else
                text = string.Empty;
        }
        // autobonus2
        if (text.Contains("autobonus2 \"{"))
        {
            var duplicate = string.Empty;

            var temp = text.Replace("autobonus2 \"{", string.Empty);
            var flag = GetAllAtf(temp);
            if (temp.IndexOf("}\"") > 0)
            {
                duplicate = temp.Substring(temp.IndexOf("}\"") + 2);

                temp = temp.Substring(0, temp.IndexOf("}\""));

                text = text.Replace(temp + "}\"", string.Empty);
            }
            var duplicates = duplicate.Split(',');
            var bonuses = string.Empty;
            var allBonus = temp.Split(';');
            for (int i = 0; i < allBonus.Length; i++)
            {
                if (!string.IsNullOrEmpty(allBonus[i]) && !string.IsNullOrWhiteSpace(allBonus[i]))
                {
                    text = text.Replace(allBonus[i] + ";", string.Empty);
                    bonuses += ConvertItemScripts(allBonus[i]);
                }
            }

            // Find first "{ for other script
            if (duplicate.IndexOf("\"{") > 0)
            {
                duplicate = duplicate.Substring(duplicate.IndexOf("\"{") + 2);
                temp = duplicate.Substring(0, duplicate.IndexOf("}\""));

                while (!string.IsNullOrEmpty(temp))
                {
                    if (temp.IndexOf(';') > 0)
                    {
                        var sumBonus = temp.Substring(0, temp.IndexOf(';'));
                        bonuses += ConvertItemScripts(sumBonus);
                        if (temp.Length > temp.IndexOf(';') + 1)
                            temp = temp.Substring(temp.IndexOf(';') + 1);
                        else
                            temp = string.Empty;
                    }
                    else
                        temp = string.Empty;
                }
            }

            if (!string.IsNullOrEmpty(bonuses) || !string.IsNullOrWhiteSpace(bonuses))
            {
                bonuses = bonuses.Replace("•", "[NEW_LINE]•");
                bonuses = bonuses.Replace("^FF2525", "[NEW_LINE]^FF2525");

                int number = 1;
                while (bonuses.Contains("•"))
                {
                    bonuses = ReplaceOneTime.ReplaceNow(bonuses, "•", number.ToString("f0") + ".)");

                    number++;
                }

                var duration = QuoteRemover.Remove(duplicates.Length >= 3 ? duplicates[2] : string.Empty);
                var rate = QuoteRemover.Remove(duplicates.Length >= 2 ? duplicates[1] : string.Empty);
                text = string.Format(_localization.GetTexts(Localization.AUTO_BONUS_2), bonuses, flag, TryParseInt(rate, 10), TryParseTimer(TryParseInt(duration, 1000)));
            }
            else
                text = string.Empty;
        }
        // autobonus
        if (text.Contains("autobonus \"{"))
        {
            var duplicate = string.Empty;

            var temp = text.Replace("autobonus \"{", string.Empty);
            var flag = GetAllAtf(temp);
            if (temp.IndexOf("}\"") > 0)
            {
                duplicate = temp.Substring(temp.IndexOf("}\"") + 2);

                temp = temp.Substring(0, temp.IndexOf("}\""));

                text = text.Replace(temp + "}\"", string.Empty);
            }
            else
                duplicate = text;

            var duplicates = duplicate.Split(',');
            var bonuses = string.Empty;
            var allBonus = temp.Split(';');
            for (int i = 0; i < allBonus.Length; i++)
            {
                var currentBonus = allBonus[i];
                if (currentBonus.IndexOf("bonus") > 0)
                {
                    var checkEmpty = currentBonus.Substring(0, currentBonus.IndexOf("bonus"));
                    if (!string.IsNullOrEmpty(checkEmpty)
                        && !string.IsNullOrWhiteSpace(checkEmpty)
                        && (checkEmpty != " "))
                        bonuses += "[NEW_LINE]" + checkEmpty;
                    currentBonus = currentBonus.Substring(currentBonus.IndexOf("bonus"));
                }
                // { Fix
                if (currentBonus.IndexOf("\"{") > 0)
                    currentBonus = currentBonus.Substring(currentBonus.IndexOf("\"{") + 2);

                currentBonus = currentBonus.Replace("}\"", string.Empty);

                if (!string.IsNullOrEmpty(currentBonus) && !string.IsNullOrWhiteSpace(currentBonus))
                {
                    text = text.Replace(currentBonus + ";", string.Empty);
                    bonuses += ConvertItemScripts(currentBonus);
                }
            }

            // Find first "{ for other script
            if (duplicate.IndexOf("\"{") > 0)
            {
                duplicate = duplicate.Substring(duplicate.IndexOf("\"{") + 2);
                temp = duplicate.Substring(0, duplicate.IndexOf("}\""));

                while (!string.IsNullOrEmpty(temp))
                {
                    if (temp.IndexOf(';') > 0)
                    {
                        var sumBonus = temp.Substring(0, temp.IndexOf(';'));
                        bonuses += ConvertItemScripts(sumBonus);
                        if (temp.Length > temp.IndexOf(';') + 1)
                            temp = temp.Substring(temp.IndexOf(';') + 1);
                        else
                            temp = string.Empty;
                    }
                    else
                        temp = string.Empty;
                }
            }

            if (!string.IsNullOrEmpty(bonuses) || !string.IsNullOrWhiteSpace(bonuses))
            {
                bonuses = bonuses.Replace("•", "[NEW_LINE]•");
                bonuses = bonuses.Replace("^FF2525", "[NEW_LINE]^FF2525");

                int number = 1;
                while (bonuses.Contains("•"))
                {
                    bonuses = ReplaceOneTime.ReplaceNow(bonuses, "•", number.ToString("f0") + ".)");

                    number++;
                }

                var duration = QuoteRemover.Remove(duplicates.Length >= 3 ? duplicates[2] : string.Empty);
                var rate = QuoteRemover.Remove(duplicates.Length >= 2 ? duplicates[1] : string.Empty);
                text = string.Format(_localization.GetTexts(Localization.AUTO_BONUS_1), bonuses, flag, TryParseInt(rate, 10), TryParseTimer(TryParseInt(duration, 1000)));
            }
            else
                text = string.Empty;
        }
        // bonus_script
        if (text.Contains("bonus_script \"{"))
        {
            var duplicate = string.Empty;

            var temp = text.Replace("bonus_script \"{", string.Empty);
            if (temp.IndexOf("}\"") > 0)
            {
                duplicate = temp.Substring(temp.IndexOf("}\"") + 2);

                temp = temp.Substring(0, temp.IndexOf("}\""));

                text = text.Replace(temp + "}\"", string.Empty);
            }
            var duplicates = duplicate.Split(',');
            var bonuses = string.Empty;
            var allBonus = temp.Split(';');
            for (int i = 0; i < allBonus.Length; i++)
            {
                if (!string.IsNullOrEmpty(allBonus[i]) && !string.IsNullOrWhiteSpace(allBonus[i]))
                {
                    text = text.Replace(allBonus[i] + ";", string.Empty);
                    bonuses += ConvertItemScripts(allBonus[i]);
                }
            }

            if (!string.IsNullOrEmpty(bonuses) || !string.IsNullOrWhiteSpace(bonuses))
            {
                bonuses = bonuses.Replace("•", "[NEW_LINE]•");
                bonuses = bonuses.Replace("^FF2525", "[NEW_LINE]^FF2525");

                int number = 1;
                while (bonuses.Contains("•"))
                {
                    bonuses = ReplaceOneTime.ReplaceNow(bonuses, "•", number.ToString("f0") + ".)");

                    number++;
                }

                var duration = QuoteRemover.Remove(duplicates.Length >= 2 ? duplicates[1] : string.Empty);
                text = string.Format(_localization.GetTexts(Localization.BONUS_SCRIPT), bonuses, TryParseTimer(TryParseInt(duration)));
            }
            else
                text = string.Empty;

            ParseStatusChangeStartIntoItemId();
        }

        text = text.Replace("break;", string.Empty);
        text = text.Replace(";", string.Empty);

        text = text.Replace("unequipscript: |", "^666478[" + _localization.GetTexts(Localization.WHEN_UNEQUIP) + "]^000000");
        text = text.Replace("equipscript: |", "^666478[" + _localization.GetTexts(Localization.WHEN_EQUIP) + "]^000000");

        text = text.Replace("bonus bstr,", "• Str +");
        text = text.Replace("bonus bagi,", "• Agi +");
        text = text.Replace("bonus bvit,", "• Vit +");
        text = text.Replace("bonus bint,", "• Int +");
        text = text.Replace("bonus bdex,", "• Dex +");
        text = text.Replace("bonus bluk,", "• Luk +");
        text = text.Replace("bonus ballstats,", "• All Status +");
        text = text.Replace("bonus bagivit,", "• Agi, Vit +");
        text = text.Replace("bonus bagidexstr,", "• Agi, Dex, Str +");

        text = text.Replace("bonus bpow,", "• Pow +");
        text = text.Replace("bonus bsta,", "• Sta +");
        text = text.Replace("bonus bwis,", "• Wis +");
        text = text.Replace("bonus bspl,", "• Spl +");
        text = text.Replace("bonus bcon,", "• Con +");
        text = text.Replace("bonus bcrt,", "• Crt +");
        text = text.Replace("bonus balltraitstats,", "• All Trait +");

        text = text.Replace("bonus bmaxhp,", "• MaxHP +");
        if (text.Contains("bonus bmaxhprate,"))
        {
            var temp = text.Replace("bonus bmaxhprate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format("• MaxHP +{0}%", TryParseInt(temps[0]));
        }
        text = text.Replace("bonus bmaxsp,", "• MaxSP +");
        if (text.Contains("bonus bmaxsprate,"))
        {
            var temp = text.Replace("bonus bmaxsprate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format("• MaxSP +{0}%", TryParseInt(temps[0]));
        }
        text = text.Replace("bonus bmaxap,", "• MaxAP +");
        if (text.Contains("bonus bmaxaprate,"))
        {
            var temp = text.Replace("bonus bmaxaprate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format("• MaxAP +{0}%", TryParseInt(temps[0]));
        }

        text = text.Replace("bonus bbaseatk,", _localization.GetTexts(Localization.BONUS_BASE_ATK));
        text = text.Replace("bonus batk,", "• Atk +");
        text = text.Replace("bonus batk2,", "• Atk +");
        if (text.Contains("bonus batkrate,"))
        {
            var temp = text.Replace("bonus batkrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format("• Atk +{0}%", TryParseInt(temps[0]));
        }
        if (text.Contains("bonus bweaponatkrate,"))
        {
            var temp = text.Replace("bonus bweaponatkrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_WEAPON_ATK_RATE), TryParseInt(temps[0]));
        }
        text = text.Replace("bonus bmatk,", "• MAtk +");
        if (text.Contains("bonus bmatkrate,"))
        {
            var temp = text.Replace("bonus bmatkrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format("• MAtk +{0}%", TryParseInt(temps[0]));
        }
        if (text.Contains("bonus bweaponmatkrate,"))
        {
            var temp = text.Replace("bonus bweaponmatkrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_WEAPON_MATK_RATE), TryParseInt(temps[0]));
        }
        text = text.Replace("bonus bdef,", "• Def +");
        if (text.Contains("bonus bdefrate,"))
        {
            var temp = text.Replace("bonus bdefrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format("• Def +{0}%", TryParseInt(temps[0]));
        }
        text = text.Replace("bonus bdef2,", _localization.GetTexts(Localization.BONUS_DEF2));
        if (text.Contains("bonus bdef2rate,"))
        {
            var temp = text.Replace("bonus bdef2rate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_DEF2_RATE), TryParseInt(temps[0]));
        }
        text = text.Replace("bonus bmdef,", "• MDef +");
        if (text.Contains("bonus bmdefrate,"))
        {
            var temp = text.Replace("bonus bmdefrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format("• MDef +{0}%", TryParseInt(temps[0]));
        }
        text = text.Replace("bonus bmdef2,", _localization.GetTexts(Localization.BONUS_MDEF2));
        if (text.Contains("bonus bmdef2rate,"))
        {
            var temp = text.Replace("bonus bmdef2rate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_MDEF2_RATE), TryParseInt(temps[0]));
        }
        text = text.Replace("bonus bhit,", "• Hit +");
        if (text.Contains("bonus bhitrate,"))
        {
            var temp = text.Replace("bonus bhitrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format("• Hit +{0}%", TryParseInt(temps[0]));
        }
        text = text.Replace("bonus bcritical,", "• Critical +");
        text = text.Replace("bonus bcriticallong,", _localization.GetTexts(Localization.BONUS_CRITICAL_LONG));
        if (text.Contains("bonus2 bcriticaladdrace,"))
        {
            var temp = text.Replace("bonus2 bcriticaladdrace,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_CRITICAL_ADD_RACE), ParseRace(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus bcriticalrate,"))
        {
            var temp = text.Replace("bonus bcriticalrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format("• Critical +{0}%", TryParseInt(temps[0]));
        }
        text = text.Replace("bonus bflee,", "• Flee +");
        if (text.Contains("bonus bfleerate,"))
        {
            var temp = text.Replace("bonus bfleerate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format("• Flee +{0}%", TryParseInt(temps[0]));
        }
        text = text.Replace("bonus bflee2,", "• Perfect Dodge +");
        if (text.Contains("bonus bflee2rate,"))
        {
            var temp = text.Replace("bonus bflee2rate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format("• Perfect Dodge +{0}%", TryParseInt(temps[0]));
        }
        if (text.Contains("bonus bperfecthitrate,"))
        {
            var temp = text.Replace("bonus bperfecthitrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_PERFECT_HIT_RATE), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus bperfecthitaddrate,"))
        {
            var temp = text.Replace("bonus bperfecthitaddrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format("• Perfect Hit +{0}%", TryParseInt(temps[0]));
        }
        if (text.Contains("bonus bspeedrate,"))
        {
            var temp = text.Replace("bonus bspeedrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_SPEED_RATE), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus bspeedaddrate,"))
        {
            var temp = text.Replace("bonus bspeedaddrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_SPEED_ADD_RATE), TryParseInt(temps[0]));
        }
        text = text.Replace("bonus baspd,", "• ASPD +");
        if (text.Contains("bonus baspdrate,"))
        {
            var temp = text.Replace("bonus baspdrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format("• ASPD +{0}%", TryParseInt(temps[0]));
        }
        text = text.Replace("bonus batkrange,", _localization.GetTexts(Localization.BONUS_ATK_RANGE));
        if (text.Contains("bonus baddmaxweight,"))
        {
            var temp = text.Replace("bonus baddmaxweight,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_ADD_MAX_WEIGHT), TryParseInt(temps[0], 10));
        }

        text = text.Replace("bonus bpatk,", "• P.Atk +");
        if (text.Contains("bonus bpatkrate,"))
        {
            var temp = text.Replace("bonus bpatkrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format("• P.Atk +{0}%", TryParseInt(temps[0]));
        }
        text = text.Replace("bonus bsmatk,", "• S.MAtk +");
        if (text.Contains("bonus bsmatkrate,"))
        {
            var temp = text.Replace("bonus bsmatkrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format("• S.MAtk +{0}%", TryParseInt(temps[0]));
        }
        text = text.Replace("bonus bres,", "• Res +");
        if (text.Contains("bonus bresrate,"))
        {
            var temp = text.Replace("bonus bresrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format("• Res +{0}%", TryParseInt(temps[0]));
        }
        text = text.Replace("bonus bmres,", "• M.Res +");
        if (text.Contains("bonus bmresrate,"))
        {
            var temp = text.Replace("bonus bmresrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format("• M.Res +{0}%", TryParseInt(temps[0]));
        }
        text = text.Replace("bonus bhplus,", "• H.Plus +");
        if (text.Contains("bonus bhplusrate,"))
        {
            var temp = text.Replace("bonus bhplusrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format("• H.Plus +{0}%", TryParseInt(temps[0]));
        }
        text = text.Replace("bonus bcrate,", "• C.Rate +");
        if (text.Contains("bonus bcraterate,"))
        {
            var temp = text.Replace("bonus bcraterate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format("• C.Rate +{0}%", TryParseInt(temps[0]));
        }
        if (text.Contains("bonus bhprecovrate,"))
        {
            var temp = text.Replace("bonus bhprecovrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_HP_RECOV_RATE), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus bsprecovrate,"))
        {
            var temp = text.Replace("bonus bsprecovrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_SP_RECOV_RATE), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus2 bhpregenrate,"))
        {
            var temp = text.Replace("bonus2 bhpregenrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_HP_REGEN_RATE), TryParseInt(temps[0]), TryParseTimer(TryParseInt(temps[1], 1000)));
        }
        if (text.Contains("bonus2 bhplossrate,"))
        {
            var temp = text.Replace("bonus2 bhplossrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_HP_LOSS_RATE), TryParseInt(temps[0]), TryParseTimer(TryParseInt(temps[1], 1000)));
        }
        if (text.Contains("bonus2 bspregenrate,"))
        {
            var temp = text.Replace("bonus2 bspregenrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SP_REGEN_RATE), TryParseInt(temps[0]), TryParseTimer(TryParseInt(temps[1], 1000)));
        }
        if (text.Contains("bonus2 bsplossrate,"))
        {
            var temp = text.Replace("bonus2 bsplossrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SP_LOSS_RATE), TryParseInt(temps[0]), TryParseTimer(TryParseInt(temps[1], 1000)));
        }
        if (text.Contains("bonus2 bregenpercenthp,"))
        {
            var temp = text.Replace("bonus2 bregenpercenthp,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_REGEN_PERCENT_HP), TryParseInt(temps[0]), TryParseTimer(TryParseInt(temps[1], 1000)));
        }
        if (text.Contains("bonus2 bregenpercentsp,"))
        {
            var temp = text.Replace("bonus2 bregenpercentsp,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_REGEN_PERCENT_SP), TryParseInt(temps[0]), TryParseTimer(TryParseInt(temps[1], 1000)));
        }
        text = text.Replace("bonus bnoregen,1", _localization.GetTexts(Localization.BONUS_STOP_HP_REGEN));
        text = text.Replace("bonus bnoregen,2", _localization.GetTexts(Localization.BONUS_STOP_SP_REGEN));
        if (text.Contains("bonus busesprate,"))
        {
            var temp = text.Replace("bonus busesprate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_USE_SP_RATE), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus2 bskillusesp,"))
        {
            var temp = text.Replace("bonus2 bskillusesp,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SKILL_USE_SP), GetSkillName(QuoteRemover.Remove(temps[0])), TryParseInt(temps[1]));
            text = text.Replace("--", "+");
        }
        if (text.Contains("bonus2 bskillusesprate,"))
        {
            var temp = text.Replace("bonus2 bskillusesprate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SKILL_USE_SP_RATE), GetSkillName(QuoteRemover.Remove(temps[0])), TryParseInt(temps[1]));
            text = text.Replace("--", "+");
        }
        if (text.Contains("bonus2 bskillatk,"))
        {
            var temp = text.Replace("bonus2 bskillatk,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SKILL_ATK), GetSkillName(QuoteRemover.Remove(temps[0])), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus bshortatkrate,"))
        {
            var temp = text.Replace("bonus bshortatkrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_SHORT_ATK_RATE), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus blongatkrate,"))
        {
            var temp = text.Replace("bonus blongatkrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_LONG_ATK_RATE), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus bcritatkrate,"))
        {
            var temp = text.Replace("bonus bcritatkrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_CRIT_ATK_RATE), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus bcritdefrate,"))
        {
            var temp = text.Replace("bonus bcritdefrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_CRIT_DEF_RATE), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus bcriticaldef,"))
        {
            var temp = text.Replace("bonus bcriticaldef,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_CRITICAL_DEF), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus2 bweaponatk,"))
        {
            var temp = text.Replace("bonus2 bweaponatk,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_WEAPON_ATK), QuoteRemover.Remove(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bweapondamagerate,"))
        {
            var temp = text.Replace("bonus2 bweapondamagerate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_WEAPON_DAMAGE_RATE), QuoteRemover.Remove(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus bnearatkdef,"))
        {
            var temp = text.Replace("bonus bnearatkdef,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_NEAR_ATK_DEF), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus blongatkdef,"))
        {
            var temp = text.Replace("bonus blongatkdef,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_LONG_ATK_DEF), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus bmagicatkdef,"))
        {
            var temp = text.Replace("bonus bmagicatkdef,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_MAGIC_ATK_DEF), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus bmiscatkdef,"))
        {
            var temp = text.Replace("bonus bmiscatkdef,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_MISC_ATK_DEF), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus bnoweapondamage,"))
        {
            var temp = text.Replace("bonus bnoweapondamage,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_NO_WEAPON_DAMAGE), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus bnomagicdamage,"))
        {
            var temp = text.Replace("bonus bnomagicdamage,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_NO_MAGIC_DAMAGE), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus bnomiscdamage,"))
        {
            var temp = text.Replace("bonus bnomiscdamage,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_NO_MISC_DAMAGE), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus bhealpower,"))
        {
            var temp = text.Replace("bonus bhealpower,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_HEAL_POWER), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus bhealpower2,"))
        {
            var temp = text.Replace("bonus bhealpower2,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_HEAL_POWER_2), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus2 bskillheal,"))
        {
            var temp = text.Replace("bonus2 bskillheal,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SKILL_HEAL), GetSkillName(QuoteRemover.Remove(temps[0])), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bskillheal2,"))
        {
            var temp = text.Replace("bonus2 bskillheal2,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SKILL_HEAL_2), GetSkillName(QuoteRemover.Remove(temps[0])), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus badditemhealrate,"))
        {
            var temp = text.Replace("bonus badditemhealrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_ADD_ITEM_HEAL_RATE), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus2 badditemhealrate,"))
        {
            var temp = text.Replace("bonus2 badditemhealrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_ADD_ITEM_HEAL_RATE), GetItemName(QuoteRemover.Remove(temps[0])), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 badditemgrouphealrate,"))
        {
            var temp = text.Replace("bonus2 badditemgrouphealrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_ADD_ITEM_GROUP_HEAL_RATE), QuoteRemover.Remove(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus badditemsphealrate,"))
        {
            var temp = text.Replace("bonus badditemsphealrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_ADD_ITEM_SP_HEAL_RATE), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus2 badditemsphealrate,"))
        {
            var temp = text.Replace("bonus2 badditemsphealrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_ADD_ITEM_SP_HEAL_RATE), GetItemName(QuoteRemover.Remove(temps[0])), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 badditemgroupsphealrate,"))
        {
            var temp = text.Replace("bonus2 badditemgroupsphealrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_ADD_ITEM_GROUP_SP_HEAL_RATE), QuoteRemover.Remove(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus bcastrate,"))
        {
            var temp = text.Replace("bonus bcastrate,", string.Empty);
            var temps = temp.Split(',');
            var value = TryParseInt(temps[0]);
            text = string.Format(_localization.GetTexts(Localization.BONUS_CAST_RATE), value);

            if (!_isFastConvert)
                _easyItemBuilderDatabase.Add("Cast % (All)", GetCurrentItemIdOrCombo(), value);
        }
        if (text.Contains("bonus2 bcastrate,"))
        {
            var temp = text.Replace("bonus2 bcastrate,", string.Empty);
            var temps = temp.Split(',');
            var skillName = GetSkillName(QuoteRemover.Remove(temps[0]));
            var value = TryParseInt(temps[1]);
            text = string.Format(_localization.GetTexts(Localization.BONUS2_CAST_RATE), skillName, value);

            if (!_isFastConvert)
                _easyItemBuilderDatabase.Add("Cast % | " + skillName, GetCurrentItemIdOrCombo(), value);
        }
        if (text.Contains("bonus bfixedcastrate,"))
        {
            var temp = text.Replace("bonus bfixedcastrate,", string.Empty);
            var temps = temp.Split(',');
            var value = TryParseInt(temps[0]);
            text = string.Format(_localization.GetTexts(Localization.BONUS_FIXED_CAST_RATE), value);

            if (!_isFastConvert)
                _easyItemBuilderDatabase.Add("F. Cast % (All)", GetCurrentItemIdOrCombo(), value);
        }
        if (text.Contains("bonus2 bfixedcastrate,"))
        {
            var temp = text.Replace("bonus2 bfixedcastrate,", string.Empty);
            var temps = temp.Split(',');
            var skillName = GetSkillName(QuoteRemover.Remove(temps[0]));
            var value = TryParseInt(temps[1]);
            text = string.Format(_localization.GetTexts(Localization.BONUS2_FIXED_CAST_RATE), skillName, value);

            if (!_isFastConvert)
                _easyItemBuilderDatabase.Add("F. Cast % | " + skillName, GetCurrentItemIdOrCombo(), value);
        }
        if (text.Contains("bonus bvariablecastrate,"))
        {
            var temp = text.Replace("bonus bvariablecastrate,", string.Empty);
            var temps = temp.Split(',');
            var value = TryParseInt(temps[0]);
            text = string.Format(_localization.GetTexts(Localization.BONUS_VARIABLE_CAST_RATE), value);

            if (!_isFastConvert)
                _easyItemBuilderDatabase.Add("V. Cast % (All)", GetCurrentItemIdOrCombo(), value);
        }
        if (text.Contains("bonus2 bvariablecastrate,"))
        {
            var temp = text.Replace("bonus2 bvariablecastrate,", string.Empty);
            var temps = temp.Split(',');
            var skillName = GetSkillName(QuoteRemover.Remove(temps[0]));
            var value = TryParseInt(temps[1]);
            text = string.Format(_localization.GetTexts(Localization.BONUS2_VARIABLE_CAST_RATE), skillName, value);

            if (!_isFastConvert)
                _easyItemBuilderDatabase.Add("V. Cast % | " + skillName, GetCurrentItemIdOrCombo(), value);
        }
        if (text.Contains("bonus bfixedcast,"))
        {
            var temp = text.Replace("bonus bfixedcast,", string.Empty);
            var temps = temp.Split(',');
            var value = TryParseTimer(TryParseInt(temps[0], 1000));
            text = string.Format(_localization.GetTexts(Localization.BONUS_FIXED_CAST), value);

            if (!_isFastConvert)
                _easyItemBuilderDatabase.Add("F. Cast (All)", GetCurrentItemIdOrCombo(), value);
        }
        if (text.Contains("bonus2 bskillfixedcast,"))
        {
            var temp = text.Replace("bonus2 bskillfixedcast,", string.Empty);
            var temps = temp.Split(',');
            var skillName = GetSkillName(QuoteRemover.Remove(temps[0]));
            var value = TryParseTimer(TryParseInt(temps[1], 1000));
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SKILL_FIXED_CAST), skillName, value);

            if (!_isFastConvert)
                _easyItemBuilderDatabase.Add("F. Cast | " + skillName, GetCurrentItemIdOrCombo(), value);
        }
        if (text.Contains("bonus bvariablecast,"))
        {
            var temp = text.Replace("bonus bvariablecast,", string.Empty);
            var temps = temp.Split(',');
            var value = TryParseTimer(TryParseInt(temps[0], 1000));
            text = string.Format(_localization.GetTexts(Localization.BONUS_VARIABLE_CAST), value);

            if (!_isFastConvert)
                _easyItemBuilderDatabase.Add("V. Cast (All)", GetCurrentItemIdOrCombo(), value);
        }
        if (text.Contains("bonus2 bskillvariablecast,"))
        {
            var temp = text.Replace("bonus2 bskillvariablecast,", string.Empty);
            var temps = temp.Split(',');
            var skillName = GetSkillName(QuoteRemover.Remove(temps[0]));
            var value = TryParseTimer(TryParseInt(temps[1], 1000));
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SKILL_VARIABLE_CAST), skillName, value);

            if (!_isFastConvert)
                _easyItemBuilderDatabase.Add("V. Cast | " + skillName, GetCurrentItemIdOrCombo(), value);
        }
        text = text.Replace("bonus bnocastcancel2", _localization.GetTexts(Localization.BONUS_NO_CAST_CANCEL_2));
        text = text.Replace("bonus bnocastcancel", _localization.GetTexts(Localization.BONUS_NO_CAST_CANCEL));
        if (text.Contains("bonus bdelayrate,"))
        {
            var temp = text.Replace("bonus bdelayrate,", string.Empty);
            var temps = temp.Split(',');
            var value = TryParseInt(temps[0]);
            text = string.Format(_localization.GetTexts(Localization.BONUS_DELAY_RATE), value);

            if (!_isFastConvert)
                _easyItemBuilderDatabase.Add("Delay % (All)", GetCurrentItemIdOrCombo(), value);
        }
        if (text.Contains("bonus2 bskilldelay,"))
        {
            var temp = text.Replace("bonus2 bskilldelay,", string.Empty);
            var temps = temp.Split(',');
            var skillName = GetSkillName(QuoteRemover.Remove(temps[0]));
            var value = TryParseTimer(TryParseInt(temps[1], 1000));
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SKILL_DELAY), skillName, value);

            if (!_isFastConvert)
                _easyItemBuilderDatabase.Add("Delay % | " + skillName, GetCurrentItemIdOrCombo(), value);
        }
        if (text.Contains("bonus2 bskillcooldown,"))
        {
            var temp = text.Replace("bonus2 bskillcooldown,", string.Empty);
            var temps = temp.Split(',');
            var skillName = GetSkillName(QuoteRemover.Remove(temps[0]));
            var value = TryParseTimer(TryParseInt(temps[1], 1000));
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SKILL_COOLDOWN), skillName, value);

            if (!_isFastConvert)
                _easyItemBuilderDatabase.Add("Cooldown | " + skillName, GetCurrentItemIdOrCombo(), value);
        }
        if (text.Contains("bonus2 baddele,"))
        {
            var temp = text.Replace("bonus2 baddele,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_ADD_ELE), ParseElement(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus3 baddele,"))
        {
            var temp = text.Replace("bonus3 baddele,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS3_ADD_ELE), ParseElement(temps[0]), TryParseInt(temps[1]), ParseAtf(temps[2]));
        }
        if (text.Contains("bonus2 bmagicaddele,"))
        {
            var temp = text.Replace("bonus2 bmagicaddele,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_MAGIC_ADD_ELE), ParseElement(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bsubele,"))
        {
            var temp = text.Replace("bonus2 bsubele,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SUB_ELE), ParseElement(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus3 bsubele,"))
        {
            var temp = text.Replace("bonus3 bsubele,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS3_SUB_ELE), ParseElement(temps[0]), TryParseInt(temps[1]), ParseAtf(temps[2]));
        }
        if (text.Contains("bonus2 bsubdefele,"))
        {
            var temp = text.Replace("bonus2 bsubdefele,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SUB_DEF_ELE), ParseElement(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bmagicsubdefele,"))
        {
            var temp = text.Replace("bonus2 bmagicsubdefele,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_MAGIC_SUB_DEF_ELE), ParseElement(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 baddrace,"))
        {
            var temp = text.Replace("bonus2 baddrace,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_ADD_RACE), ParseRace(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bmagicaddrace,"))
        {
            var temp = text.Replace("bonus2 bmagicaddrace,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_MAGIC_ADD_RACE), ParseRace(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bsubrace,"))
        {
            var temp = text.Replace("bonus2 bsubrace,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SUB_RACE), ParseRace(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus3 bsubrace,"))
        {
            var temp = text.Replace("bonus3 bsubrace,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS3_SUB_RACE), ParseRace(temps[0]), TryParseInt(temps[1]), ParseAtf(temps[2]));
        }
        if (text.Contains("bonus2 baddclass,"))
        {
            var temp = text.Replace("bonus2 baddclass,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_ADD_CLASS), ParseClass(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bmagicaddclass,"))
        {
            var temp = text.Replace("bonus2 bmagicaddclass,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_MAGIC_ADD_CLASS), ParseClass(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bsubclass,"))
        {
            var temp = text.Replace("bonus2 bsubclass,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SUB_CLASS), ParseClass(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 baddsize,"))
        {
            var temp = text.Replace("bonus2 baddsize,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_ADD_SIZE), ParseSize(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bmagicaddsize,"))
        {
            var temp = text.Replace("bonus2 bmagicaddsize,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_MAGIC_ADD_SIZE), ParseSize(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bsubsize,"))
        {
            var temp = text.Replace("bonus2 bsubsize,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SUB_SIZE), ParseSize(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bweaponsubsize,"))
        {
            var temp = text.Replace("bonus2 bweaponsubsize,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_WEAPON_SUB_SIZE), ParseSize(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bmagicsubsize,"))
        {
            var temp = text.Replace("bonus2 bmagicsubsize,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_MAGIC_SUB_SIZE), ParseSize(temps[0]), TryParseInt(temps[1]));
        }
        text = text.Replace("bonus bnosizefix", _localization.GetTexts(Localization.BONUS_NO_SIZE_FIX));
        if (text.Contains("bonus2 badddamageclass,"))
        {
            var temp = text.Replace("bonus2 badddamageclass,", string.Empty);
            var temps = temp.Split(',');
            var monsterDatabase = GetMonsterDatabase(TryParseInt(temps[0]));
            text = string.Format(_localization.GetTexts(Localization.BONUS2_ADD_DAMAGE_CLASS), (monsterDatabase != null) ? "^FF0000" + monsterDatabase.name + "^000000" : temps[0], TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 baddmagicdamageclass,"))
        {
            var temp = text.Replace("bonus2 baddmagicdamageclass,", string.Empty);
            var temps = temp.Split(',');
            var monsterDatabase = GetMonsterDatabase(TryParseInt(temps[0]));
            text = string.Format(_localization.GetTexts(Localization.BONUS2_ADD_MAGIC_DAMAGE_CLASS), (monsterDatabase != null) ? "^FF0000" + monsterDatabase.name + "^000000" : temps[0], TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 badddefmonster,"))
        {
            var temp = text.Replace("bonus2 badddefmonster,", string.Empty);
            var temps = temp.Split(',');
            var monsterDatabase = GetMonsterDatabase(TryParseInt(temps[0]));
            text = string.Format(_localization.GetTexts(Localization.BONUS2_ADD_DEF_MONSTER), (monsterDatabase != null) ? "^FF0000" + monsterDatabase.name + "^000000" : temps[0], TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 baddmdefmonster,"))
        {
            var temp = text.Replace("bonus2 baddmdefmonster,", string.Empty);
            var temps = temp.Split(',');
            var monsterDatabase = GetMonsterDatabase(TryParseInt(temps[0]));
            text = string.Format(_localization.GetTexts(Localization.BONUS2_ADD_MDEF_MONSTER), (monsterDatabase != null) ? "^FF0000" + monsterDatabase.name + "^000000" : temps[0], TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 baddrace2,"))
        {
            var temp = text.Replace("bonus2 baddrace2,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_ADD_RACE_2), ParseRace2(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bsubrace2,"))
        {
            var temp = text.Replace("bonus2 bsubrace2,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SUB_RACE_2), ParseRace2(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bmagicaddrace2,"))
        {
            var temp = text.Replace("bonus2 bmagicaddrace2,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_MAGIC_ADD_RACE_2), ParseRace2(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bsubskill,"))
        {
            var temp = text.Replace("bonus2 bsubskill,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SUB_SKILL), GetSkillName(QuoteRemover.Remove(temps[0])), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus babsorbdmgmaxhp,"))
        {
            var temp = text.Replace("bonus babsorbdmgmaxhp,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_ABSORB_DMG_MAX_HP), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus babsorbdmgmaxhp2,"))
        {
            var temp = text.Replace("bonus babsorbdmgmaxhp2,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_ABSORB_DMG_MAX_HP_2), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus batkele,"))
        {
            var temp = text.Replace("bonus batkele,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_ATK_ELE), ParseElement(temps[0]));
        }
        if (text.Contains("bonus bdefele,"))
        {
            var temp = text.Replace("bonus bdefele,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_DEF_ELE), ParseElement(temps[0]));
        }
        if (text.Contains("bonus2 bmagicatkele,"))
        {
            var temp = text.Replace("bonus2 bmagicatkele,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_MAGIC_ATK_ELE), ParseElement(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus bdefratioatkrace,"))
        {
            var temp = text.Replace("bonus bdefratioatkrace,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_DEF_RATIO_ATK_RACE), ParseRace(temps[0]));
        }
        if (text.Contains("bonus bdefratioatkele,"))
        {
            var temp = text.Replace("bonus bdefratioatkele,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_DEF_RATIO_ATK_ELE), ParseElement(temps[0]));
        }
        if (text.Contains("bonus bdefratioatkclass,"))
        {
            var temp = text.Replace("bonus bdefratioatkclass,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_DEF_RATIO_ATK_CLASS), ParseClass(temps[0]));
        }
        if (text.Contains("bonus4 bsetdefrace,"))
        {
            var temp = text.Replace("bonus4 bsetdefrace,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS4_SET_DEF_RACE), ParseRace(temps[0]), TryParseInt(temps[1]), TryParseTimer(TryParseInt(temps[2], 1000)), TryParseInt(temps[3]));
        }
        if (text.Contains("bonus4 bsetmdefrace,"))
        {
            var temp = text.Replace("bonus4 bsetmdefrace,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS4_SET_MDEF_RACE), ParseRace(temps[0]), TryParseInt(temps[1]), TryParseTimer(TryParseInt(temps[2], 1000)), TryParseInt(temps[3]));
        }
        if (text.Contains("bonus bignoredefele,"))
        {
            var temp = text.Replace("bonus bignoredefele,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_IGNORE_DEF_ELE), ParseElement(temps[0]));
        }
        if (text.Contains("bonus bignoredefrace,"))
        {
            var temp = text.Replace("bonus bignoredefrace,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_IGNORE_DEF_RACE), ParseRace(temps[0]));
        }
        if (text.Contains("bonus bignoredefclass,"))
        {
            var temp = text.Replace("bonus bignoredefclass,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_IGNORE_DEF_CLASS), ParseClass(temps[0]));
        }
        if (text.Contains("bonus bignoremdefrace,"))
        {
            var temp = text.Replace("bonus bignoremdefrace,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_IGNORE_MDEF_RACE), ParseRace(temps[0]));
        }
        if (text.Contains("bonus2 bignoredefracerate,"))
        {
            var temp = text.Replace("bonus2 bignoredefracerate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_IGNORE_DEF_RACE_RATE), ParseRace(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bignoremdefracerate,"))
        {
            var temp = text.Replace("bonus2 bignoremdefracerate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_IGNORE_MDEF_RACE_RATE), ParseRace(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bignoremdefrace2rate,"))
        {
            var temp = text.Replace("bonus2 bignoremdefrace2rate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_IGNORE_MDEF_RACE_2_RATE), ParseRace2(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus bignoremdefele,"))
        {
            var temp = text.Replace("bonus bignoremdefele,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_IGNORE_MDEF_ELE), ParseElement(temps[0]));
        }
        if (text.Contains("bonus2 bignoredefclassrate,"))
        {
            var temp = text.Replace("bonus2 bignoredefclassrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_IGNORE_DEF_CLASS_RATE), ParseClass(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bignoremdefclassrate,"))
        {
            var temp = text.Replace("bonus2 bignoremdefclassrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_IGNORE_MDEF_CLASS_RATE), ParseClass(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bignoreresracerate,"))
        {
            var temp = text.Replace("bonus2 bignoreresracerate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_IGNORE_RES_RACE_RATE), ParseRace(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bignoremresracerate,"))
        {
            var temp = text.Replace("bonus2 bignoremresracerate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_IGNORE_MRES_RACE_RATE), ParseRace(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bexpaddrace,"))
        {
            var temp = text.Replace("bonus2 bexpaddrace,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_EXP_ADD_RACE), ParseRace(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bexpaddclass,"))
        {
            var temp = text.Replace("bonus2 bexpaddclass,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_EXP_ADD_CLASS), ParseClass(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 baddeff,"))
        {
            var temp = text.Replace("bonus2 baddeff,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_ADD_EFF), ParseEffect(temps[0]), TryParseInt(temps[1], 100));
        }
        if (text.Contains("bonus2 baddeff2,"))
        {
            var temp = text.Replace("bonus2 baddeff2,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_ADD_EFF_2), ParseEffect(temps[0]), TryParseInt(temps[1], 100));
        }
        if (text.Contains("bonus2 baddeffwhenhit,"))
        {
            var temp = text.Replace("bonus2 baddeffwhenhit,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_ADD_EFF_WHEN_HIT), ParseEffect(temps[0]), TryParseInt(temps[1], 100));
        }
        if (text.Contains("bonus2 breseff,"))
        {
            var temp = text.Replace("bonus2 breseff,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_RES_EFF), ParseEffect(temps[0]), TryParseInt(temps[1], 100));
        }
        if (text.Contains("bonus3 baddeff,"))
        {
            var temp = text.Replace("bonus3 baddeff,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS3_ADD_EFF), ParseEffect(temps[0]), TryParseInt(temps[1], 100), ParseAtf(temps[2]));
        }
        if (text.Contains("bonus4 baddeff,"))
        {
            var temp = text.Replace("bonus4 baddeff,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS4_ADD_EFF), ParseEffect(temps[0]), TryParseInt(temps[1], 100), ParseAtf(temps[2]), TryParseTimer(TryParseInt(temps[3], 1000)));
        }
        if (text.Contains("bonus3 baddeffwhenhit,"))
        {
            var temp = text.Replace("bonus3 baddeffwhenhit,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS3_ADD_EFF_WHEN_HIT), ParseEffect(temps[0]), TryParseInt(temps[1], 100), ParseAtf(temps[2]));
        }
        if (text.Contains("bonus4 baddeffwhenhit,"))
        {
            var temp = text.Replace("bonus4 baddeffwhenhit,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS4_ADD_EFF_WHEN_HIT), ParseEffect(temps[0]), TryParseInt(temps[1], 100), ParseAtf(temps[2]), TryParseTimer(TryParseInt(temps[3], 1000)));
        }
        if (text.Contains("bonus3 baddeffonskill,"))
        {
            var temp = text.Replace("bonus3 baddeffonskill,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS3_ADD_EFF_ON_SKILL), GetSkillName(QuoteRemover.Remove(temps[0])), ParseEffect(temps[1]), TryParseInt(temps[2], 100));
        }
        if (text.Contains("bonus4 baddeffonskill,"))
        {
            var temp = text.Replace("bonus4 baddeffonskill,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS4_ADD_EFF_ON_SKILL), GetSkillName(QuoteRemover.Remove(temps[0])), ParseEffect(temps[1]), TryParseInt(temps[2], 100), ParseAtf(temps[3]));
        }
        if (text.Contains("bonus5 baddeffonskill,"))
        {
            var temp = text.Replace("bonus5 baddeffonskill,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS5_ADD_EFF_ON_SKILL), GetSkillName(QuoteRemover.Remove(temps[0])), ParseEffect(temps[1]), TryParseInt(temps[2], 100), ParseAtf(temps[3]), TryParseTimer(TryParseInt(temps[4], 1000)));
        }
        if (text.Contains("bonus2 bcomaclass,"))
        {
            var temp = text.Replace("bonus2 bcomaclass,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_COMA_CLASS), ParseClass(temps[0]), TryParseInt(temps[1], 100));
        }
        if (text.Contains("bonus2 bcomarace,"))
        {
            var temp = text.Replace("bonus2 bcomarace,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_COMA_RACE), ParseRace(temps[0]), TryParseInt(temps[1], 100));
        }
        if (text.Contains("bonus2 bweaponcomaele,"))
        {
            var temp = text.Replace("bonus2 bweaponcomaele,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_WEAPON_COMA_ELE), ParseElement(temps[0]), TryParseInt(temps[1], 100));
        }
        if (text.Contains("bonus2 bweaponcomaclass,"))
        {
            var temp = text.Replace("bonus2 bweaponcomaclass,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_WEAPON_COMA_CLASS), ParseClass(temps[0]), TryParseInt(temps[1], 100));
        }
        if (text.Contains("bonus2 bweaponcomarace,"))
        {
            var temp = text.Replace("bonus2 bweaponcomarace,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_WEAPON_COMA_RACE), ParseRace(temps[0]), TryParseInt(temps[1], 100));
        }
        if (text.Contains("bonus3 bautospell,"))
        {
            var temp = text.Replace("bonus3 bautospell,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS3_AUTO_SPELL), GetSkillName(QuoteRemover.Remove(temps[0])), TryParseInt(temps[1]), TryParseInt(temps[2], 10));
        }
        if (text.Contains("bonus3 bautospellwhenhit,"))
        {
            var temp = text.Replace("bonus3 bautospellwhenhit,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS3_AUTO_SPELL_WHEN_HIT), GetSkillName(QuoteRemover.Remove(temps[0])), TryParseInt(temps[1]), TryParseInt(temps[2], 10));
        }
        if (text.Contains("bonus4 bautospell,"))
        {
            var temp = text.Replace("bonus4 bautospell,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS4_AUTO_SPELL), GetSkillName(QuoteRemover.Remove(temps[0])), TryParseInt(temps[1]), TryParseInt(temps[2], 10), ParseI(temps[3]));
        }
        if (text.Contains("bonus5 bautospell,"))
        {
            var temp = text.Replace("bonus5 bautospell,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS5_AUTO_SPELL), GetSkillName(QuoteRemover.Remove(temps[0])), TryParseInt(temps[1]), TryParseInt(temps[2], 10), ParseAtf(temps[3]), ParseI(temps[4]));
        }
        if (text.Contains("bonus4 bautospellwhenhit,"))
        {
            var temp = text.Replace("bonus4 bautospellwhenhit,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS4_AUTO_SPELL_WHEN_HIT), GetSkillName(QuoteRemover.Remove(temps[0])), TryParseInt(temps[1]), TryParseInt(temps[2], 10), ParseI(temps[3]));
        }
        if (text.Contains("bonus5 bautospellwhenhit,"))
        {
            var temp = text.Replace("bonus5 bautospellwhenhit,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS5_AUTO_SPELL_WHEN_HIT), GetSkillName(QuoteRemover.Remove(temps[0])), TryParseInt(temps[1]), TryParseInt(temps[2], 10), ParseAtf(temps[3]), ParseI(temps[4]));
        }
        if (text.Contains("bonus4 bautospellonskill,"))
        {
            var temp = text.Replace("bonus4 bautospellonskill,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS4_AUTO_SPELL_ON_SKILL), GetSkillName(QuoteRemover.Remove(temps[0])), GetSkillName(QuoteRemover.Remove(temps[1])), TryParseInt(temps[2]), TryParseInt(temps[3], 10));
        }
        if (text.Contains("bonus5 bautospellonskill,"))
        {
            var temp = text.Replace("bonus5 bautospellonskill,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS5_AUTO_SPELL_ON_SKILL), GetSkillName(QuoteRemover.Remove(temps[0])), GetSkillName(QuoteRemover.Remove(temps[1])), TryParseInt(temps[2]), TryParseInt(temps[3], 10), ParseI(temps[4]));
        }
        text = text.Replace("bonus bhpdrainvalue,", _localization.GetTexts(Localization.BONUS_HP_DRAIN_VALUE));
        if (text.Contains("bonus2 bhpdrainvaluerace,"))
        {
            var temp = text.Replace("bonus2 bhpdrainvaluerace,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_HP_DRAIN_VALUE_RACE), ParseRace(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bhpdrainvalueclass,"))
        {
            var temp = text.Replace("bonus2 bhpdrainvalueclass,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_HP_DRAIN_VALUE_CLASS), ParseClass(temps[0]), TryParseInt(temps[1]));
        }
        text = text.Replace("bonus bspdrainvalue,", _localization.GetTexts(Localization.BONUS_SP_DRAIN_VALUE));
        if (text.Contains("bonus2 bspdrainvaluerace,"))
        {
            var temp = text.Replace("bonus2 bspdrainvaluerace,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SP_DRAIN_VALUE_RACE), ParseRace(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bspdrainvalueclass,"))
        {
            var temp = text.Replace("bonus2 bspdrainvalueclass,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SP_DRAIN_VALUE_CLASS), ParseClass(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bhpdrainrate,"))
        {
            var temp = text.Replace("bonus2 bhpdrainrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_HP_DRAIN_RATE), TryParseInt(temps[0], 10), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bspdrainrate,"))
        {
            var temp = text.Replace("bonus2 bspdrainrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SP_DRAIN_RATE), TryParseInt(temps[0], 10), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bhpvanishrate,"))
        {
            var temp = text.Replace("bonus2 bhpvanishrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_HP_VANISH_RATE), TryParseInt(temps[0], 10), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus3 bhpvanishracerate,"))
        {
            var temp = text.Replace("bonus3 bhpvanishracerate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS3_HP_VANISH_RACE_RATE), ParseRace(temps[0]), TryParseInt(temps[1], 10), TryParseInt(temps[2]));
        }
        if (text.Contains("bonus3 bhpvanishrate,"))
        {
            var temp = text.Replace("bonus3 bhpvanishrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS3_HP_VANISH_RATE), TryParseInt(temps[0], 10), TryParseInt(temps[1]), ParseAtf(temps[2]));
        }
        if (text.Contains("bonus2 bspvanishrate,"))
        {
            var temp = text.Replace("bonus2 bspvanishrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SP_VANISH_RATE), TryParseInt(temps[0], 10), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus3 bspvanishracerate,"))
        {
            var temp = text.Replace("bonus3 bspvanishracerate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS3_SP_VANISH_RACE_RATE), ParseRace(temps[0]), TryParseInt(temps[1], 10), TryParseInt(temps[2]));
        }
        if (text.Contains("bonus3 bspvanishrate,"))
        {
            var temp = text.Replace("bonus3 bspvanishrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS3_SP_VANISH_RATE), TryParseInt(temps[0], 10), TryParseInt(temps[1]), ParseAtf(temps[2]));
        }
        if (text.Contains("bonus3 bstatenorecoverrace,"))
        {
            var temp = text.Replace("bonus3 bstatenorecoverrace,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS3_STATE_NO_RECOVER_RACE), ParseRace(temps[0]), TryParseInt(temps[1], 100), TryParseTimer(TryParseInt(temps[2], 1000)));
        }
        text = text.Replace("bonus bhpgainvalue,", _localization.GetTexts(Localization.BONUS_HP_GAIN_VALUE));
        text = text.Replace("bonus bspgainvalue,", _localization.GetTexts(Localization.BONUS_SP_GAIN_VALUE));
        if (text.Contains("bonus2 bspgainrace,"))
        {
            var temp = text.Replace("bonus2 bspgainrace,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_SP_GAIN_RACE), ParseRace(temps[0]), TryParseInt(temps[1]));
        }
        text = text.Replace("bonus blonghpgainvalue,", _localization.GetTexts(Localization.BONUS_LONG_HP_GAIN_VALUE));
        text = text.Replace("bonus blongspgainvalue,", _localization.GetTexts(Localization.BONUS_LONG_SP_GAIN_VALUE));
        text = text.Replace("bonus bmagichpgainvalue,", _localization.GetTexts(Localization.BONUS_MAGIC_HP_GAIN_VALUE));
        text = text.Replace("bonus bmagicspgainvalue,", _localization.GetTexts(Localization.BONUS_MAGIC_SP_GAIN_VALUE));
        if (text.Contains("bonus bshortweapondamagereturn,"))
        {
            var temp = text.Replace("bonus bshortweapondamagereturn,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_SHORT_WEAPON_DAMAGE_RETURN), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus blongweapondamagereturn,"))
        {
            var temp = text.Replace("bonus blongweapondamagereturn,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_LONG_WEAPON_DAMAGE_RETURN), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus bmagicdamagereturn,"))
        {
            var temp = text.Replace("bonus bmagicdamagereturn,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_MAGIC_DAMAGE_RETURN), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus breducedamagereturn,"))
        {
            var temp = text.Replace("bonus breducedamagereturn,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_REDUCE_DAMAGE_RETURN), TryParseInt(temps[0]));
        }
        text = text.Replace("bonus bunstripableweapon", _localization.GetTexts(Localization.BONUS_UNSTRIPABLE_WEAPON));
        text = text.Replace("bonus bunstripablearmor", _localization.GetTexts(Localization.BONUS_UNSTRIPABLE_ARMOR));
        text = text.Replace("bonus bunstripablehelm", _localization.GetTexts(Localization.BONUS_UNSTRIPABLE_HELM));
        text = text.Replace("bonus bunstripableshield", _localization.GetTexts(Localization.BONUS_UNSTRIPABLE_SHIELD));
        text = text.Replace("bonus bunstripable", _localization.GetTexts(Localization.BONUS_UNSTRIPABLE));
        text = text.Replace("bonus bunbreakablegarment", _localization.GetTexts(Localization.BONUS_UNBREAKABLE_GARMENT));
        text = text.Replace("bonus bunbreakableweapon", _localization.GetTexts(Localization.BONUS_UNBREAKABLE_WEAPON));
        text = text.Replace("bonus bunbreakablearmor", _localization.GetTexts(Localization.BONUS_UNBREAKABLE_ARMOR));
        text = text.Replace("bonus bunbreakablehelm", _localization.GetTexts(Localization.BONUS_UNBREAKABLE_HELM));
        text = text.Replace("bonus bunbreakableshield", _localization.GetTexts(Localization.BONUS_UNBREAKABLE_SHIELD));
        text = text.Replace("bonus bunbreakableshoes", _localization.GetTexts(Localization.BONUS_UNBREAKABLE_SHOES));
        if (text.Contains("bonus bunbreakable,"))
        {
            var temp = text.Replace("bonus bunbreakable,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_UNBREAKABLE), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus bbreakweaponrate,"))
        {
            var temp = text.Replace("bonus bbreakweaponrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_BREAK_WEAPON_RATE), TryParseInt(temps[0], 100));
        }
        if (text.Contains("bonus bbreakarmorrate,"))
        {
            var temp = text.Replace("bonus bbreakarmorrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_BREAK_ARMOR_RATE), TryParseInt(temps[0], 100));
        }
        if (text.Contains("bonus2 bdropaddrace,"))
        {
            var temp = text.Replace("bonus2 bdropaddrace,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_DROP_ADD_RACE), ParseRace(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 bdropaddclass,"))
        {
            var temp = text.Replace("bonus2 bdropaddclass,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_DROP_ADD_CLASS), ParseClass(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus3 baddmonsteriddropitem,"))
        {
            var temp = text.Replace("bonus3 baddmonsteriddropitem,", string.Empty);
            var temps = temp.Split(',');
            var itemId = QuoteRemover.Remove(temps[0]);
            var monsterDatabase = GetMonsterDatabase(TryParseInt(temps[1]));
            text = string.Format(_localization.GetTexts(Localization.BONUS3_ADD_MONSTER_ID_DROP_ITEM), GetItemName(itemId), (monsterDatabase != null) ? monsterDatabase.name : temps[1], TryParseInt(temps[2], 100), itemId);
        }
        if (text.Contains("bonus2 baddmonsterdropitem,"))
        {
            var temp = text.Replace("bonus2 baddmonsterdropitem,", string.Empty);
            var temps = temp.Split(',');
            var itemId = QuoteRemover.Remove(temps[0]);
            text = string.Format(_localization.GetTexts(Localization.BONUS2_ADD_MONSTER_DROP_ITEM), GetItemName(itemId), TryParseInt(temps[1], 100), itemId);
        }
        if (text.Contains("bonus3 baddmonsterdropitem,"))
        {
            var temp = text.Replace("bonus3 baddmonsterdropitem,", string.Empty);
            var temps = temp.Split(',');
            var itemId = QuoteRemover.Remove(temps[0]);
            text = string.Format(_localization.GetTexts(Localization.BONUS3_ADD_MONSTER_DROP_ITEM), GetItemName(itemId), ParseRace(temps[1]), TryParseInt(temps[2], 100), itemId);
        }
        if (text.Contains("bonus3 baddclassdropitem,"))
        {
            var temp = text.Replace("bonus3 baddclassdropitem,", string.Empty);
            var temps = temp.Split(',');
            var itemId = QuoteRemover.Remove(temps[0]);
            text = string.Format(_localization.GetTexts(Localization.BONUS3_ADD_CLASS_DROP_ITEM), GetItemName(itemId), ParseClass(temps[1]), TryParseInt(temps[2], 100), itemId);
        }
        if (text.Contains("bonus2 baddmonsterdropitemgroup,"))
        {
            var temp = text.Replace("bonus2 baddmonsterdropitemgroup,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_ADD_MONSTER_DROP_ITEM_GROUP), QuoteRemover.Remove(temps[0]), TryParseInt(temps[1], 100));
        }
        if (text.Contains("bonus3 baddmonsterdropitemgroup,"))
        {
            var temp = text.Replace("bonus3 baddmonsterdropitemgroup,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS3_ADD_MONSTER_DROP_ITEM_GROUP), QuoteRemover.Remove(temps[0]), ParseRace(temps[1]), TryParseInt(temps[2], 100));
        }
        if (text.Contains("bonus3 baddclassdropitemgroup,"))
        {
            var temp = text.Replace("bonus3 baddclassdropitemgroup,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS3_ADD_CLASS_DROP_ITEM_GROUP), QuoteRemover.Remove(temps[0]), ParseClass(temps[1]), TryParseInt(temps[2], 100));
        }
        if (text.Contains("bonus2 bgetzenynum,"))
        {
            var temp = text.Replace("bonus2 bgetzenynum,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_GET_ZENY_NUM), TryParseInt(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus2 baddgetzenynum,"))
        {
            var temp = text.Replace("bonus2 baddgetzenynum,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_ADD_GET_ZENY_NUM), TryParseInt(temps[0]), TryParseInt(temps[1]));
        }
        if (text.Contains("bonus bdoublerate,"))
        {
            var temp = text.Replace("bonus bdoublerate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_DOUBLE_RATE), TryParseInt(temps[0]));
        }
        if (text.Contains("bonus bdoubleaddrate,"))
        {
            var temp = text.Replace("bonus bdoubleaddrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_DOUBLE_ADD_RATE), TryParseInt(temps[0]));
        }
        text = text.Replace("bonus bsplashrange,", _localization.GetTexts(Localization.BONUS_SPLASH_RANGE));
        text = text.Replace("bonus bsplashaddrange,", _localization.GetTexts(Localization.BONUS_SPLASH_ADD_RANGE));
        if (text.Contains("bonus2 baddskillblow,"))
        {
            var temp = text.Replace("bonus2 baddskillblow,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS2_ADD_SKILL_BLOW), GetSkillName(QuoteRemover.Remove(temps[0])), TryParseInt(temps[1]));
        }
        text = text.Replace("bonus bnoknockback", _localization.GetTexts(Localization.BONUS_NO_KNOCKBACK));
        text = text.Replace("bonus bnogemstone", _localization.GetTexts(Localization.BONUS_NO_GEM_STONE));
        text = text.Replace("bonus bintravision", _localization.GetTexts(Localization.BONUS_INTRAVISION));
        text = text.Replace("bonus bperfecthide", _localization.GetTexts(Localization.BONUS_PERFECT_HIDE));
        text = text.Replace("bonus brestartfullrecover", _localization.GetTexts(Localization.BONUS_RESTART_FULL_RECOVER));
        if (text.Contains("bonus bclasschange,"))
        {
            var temp = text.Replace("bonus bclasschange,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_CLASS_CHANGE), TryParseInt(temps[0], 100));
        }
        if (text.Contains("bonus baddstealrate,"))
        {
            var temp = text.Replace("bonus baddstealrate,", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.BONUS_ADD_STEAL_RATE), TryParseInt(temps[0]));
        }
        text = text.Replace("bonus bnomadofuel", _localization.GetTexts(Localization.BONUS_NO_MADO_FUEL));
        text = text.Replace("bonus bnowalkdelay", _localization.GetTexts(Localization.BONUS_NO_WALK_DELAY));
        // Special Effect 2
        if (text.Contains("specialeffect2 "))
        {
            var temp = text.Replace("specialeffect2 ", string.Empty);
            text = _localization.GetTexts(Localization.SPECIAL_EFFECT_2) + " " + temp.Replace("EF_", string.Empty);
        }
        // Special Effect
        if (text.Contains("specialeffect "))
        {
            var temp = text.Replace("specialeffect ", string.Empty);
            text = _localization.GetTexts(Localization.SPECIAL_EFFECT) + " " + temp.Replace("EF_", string.Empty);
        }
        // Show Script
        if (text.Contains("showscript "))
        {
            var temp = text.Replace("showscript ", string.Empty);
            text = _localization.GetTexts(Localization.SHOW_SCRIPT) + " " + temp;
        }
        // Unit Skill Use Id
        if (text.Contains("unitskilluseid "))
        {
            var temp = text.Replace("unitskilluseid ", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.UNIT_SKILL_USE_ID), QuoteRemover.Remove(temps[0]), GetSkillName(QuoteRemover.Remove(temps[1])), TryParseInt(temps[2]));
        }
        // Item Skill
        if (text.Contains("itemskill "))
        {
            var temp = text.Replace("itemskill ", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.ITEM_SKILL), GetSkillName(QuoteRemover.Remove(temps[0])), TryParseInt(temps[1]));
        }
        // Skill
        if (text.Contains("skill "))
        {
            var temp = text.Replace("skill ", string.Empty);
            var temps = temp.Split(',');
            if (temps.Length > 1)
                text = string.Format(_localization.GetTexts(Localization.SKILL), GetSkillName(QuoteRemover.Remove(temps[0])), TryParseInt(temps[1]));
        }
        // itemheal
        if (text.Contains("percentheal "))
        {
            var temp = text.Replace("percentheal ", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.PERCENT_HEAL), TryParseInt(temps[0]), TryParseInt(temps[1]));
        }
        // itemheal
        if (text.Contains("itemheal "))
        {
            var temp = text.Replace("itemheal ", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.ITEM_HEAL), TryParseInt(temps[0]), TryParseInt(temps[1]));
        }
        // heal
        if (((!string.IsNullOrEmpty(text) && (text[0] == 'h')) || text.Contains(" h")) && text.Contains("heal "))
        {
            var temp = text.Replace("heal ", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.HEAL), TryParseInt(temps[0]), TryParseInt(temps[1]));
        }
        // sc_start4
        if (text.Contains("sc_start4 "))
        {
            var temp = text.Replace("sc_start4 ", string.Empty);
            var temps = temp.Split(',');
            var duration = (temps.Length > 1) ? TryParseInt(temps[1], 1000) : "0";
            if (duration == "0")
                text = string.Format(_localization.GetTexts(Localization.SC_START_4_NO_DURATION), QuoteRemover.Remove(temps[0]));
            else
                text = string.Format(_localization.GetTexts(Localization.SC_START_4), QuoteRemover.Remove(temps[0]), TryParseTimer(TryParseInt(temps[1], 1000)));

            text = text.Replace("sc_", string.Empty);

            ParseStatusChangeStartIntoItemId();
        }
        // sc_start2
        if (text.Contains("sc_start2 "))
        {
            var temp = text.Replace("sc_start2 ", string.Empty);
            var temps = temp.Split(',');
            var duration = (temps.Length > 1) ? TryParseInt(temps[1], 1000) : "0";
            if (duration == "0")
                text = string.Format(_localization.GetTexts(Localization.SC_START_2_NO_DURATION), QuoteRemover.Remove(temps[0]));
            else
                text = string.Format(_localization.GetTexts(Localization.SC_START_2), QuoteRemover.Remove(temps[0]), TryParseTimer(TryParseInt(temps[1], 1000)));

            text = text.Replace("sc_", string.Empty);

            ParseStatusChangeStartIntoItemId();
        }
        // sc_start
        if (text.Contains("sc_start "))
        {
            var temp = text.Replace("sc_start ", string.Empty);
            var temps = temp.Split(',');
            var duration = (temps.Length > 1) ? TryParseInt(temps[1], 1000) : "0";
            if (duration == "0")
                text = string.Format(_localization.GetTexts(Localization.SC_START_NO_DURATION), QuoteRemover.Remove(temps[0]));
            else
                text = string.Format(_localization.GetTexts(Localization.SC_START), QuoteRemover.Remove(temps[0]), (temps.Length > 1) ? TryParseTimer(TryParseInt(temps[1], 1000)) : "0");

            text = text.Replace("sc_", string.Empty);

            ParseStatusChangeStartIntoItemId();
        }
        // sc_end
        if (text.Contains("sc_end "))
        {
            var temp = text.Replace("sc_end ", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.SC_END), QuoteRemover.Remove(temps[0]));

            text = text.Replace("sc_", string.Empty);
        }
        // active_transform
        if (text.Contains("active_transform "))
        {
            var temp = text.Replace("active_transform ", string.Empty);
            var temps = temp.Split(',');
            var monsterDatabase = GetMonsterDatabase(QuoteRemover.Remove(temps[0]));
            text = string.Format(_localization.GetTexts(Localization.ACTIVE_TRANSFORM), (monsterDatabase != null) ? monsterDatabase.name : QuoteRemover.Remove(temps[0]), TryParseTimer(TryParseInt(temps[1], 1000)));
        }
        // transform
        if (text.Contains("transform "))
        {
            var temp = text.Replace("transform ", string.Empty);
            var temps = temp.Split(',');
            var monsterDatabase = GetMonsterDatabase(QuoteRemover.Remove(temps[0]));
            text = string.Format(_localization.GetTexts(Localization.ACTIVE_TRANSFORM), (monsterDatabase != null) ? monsterDatabase.name : QuoteRemover.Remove(temps[0]), TryParseTimer(TryParseInt(temps[1], 1000)));
        }
        // getitem
        if (text.Contains("getitem "))
        {
            var temp = text.Replace("getitem ", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.GET_ITEM), GetItemName(QuoteRemover.Remove(temps[0])), TryParseInt(temps[1]));
        }
        // groupranditem
        if (text.Contains("groupranditem"))
        {
            var temp = text.Replace("groupranditem", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.GET_GROUP_ITEM), QuoteRemover.Remove(temps[0]).Replace("IG_", string.Empty), TryParseInt((temps.Length > 1) ? temps[1] : null, 1, 1));

            if (!_isFastConvert && !_itemListContainer.itemGroupIds.Contains(_itemContainer.id))
                _itemListContainer.itemGroupIds.Add(_itemContainer.id);
        }
        // getrandgroupitem
        if (text.Contains("getrandgroupitem"))
        {
            var temp = text.Replace("getrandgroupitem", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.GET_GROUP_ITEM), QuoteRemover.Remove(temps[0]).Replace("IG_", string.Empty), TryParseInt((temps.Length > 1) ? temps[1] : null, 1, 1));

            if (!_isFastConvert && !_itemListContainer.itemGroupIds.Contains(_itemContainer.id))
                _itemListContainer.itemGroupIds.Add(_itemContainer.id);
        }
        // getgroupitem
        if (text.Contains("getgroupitem"))
        {
            var temp = text.Replace("getgroupitem", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.GET_GROUP_ITEM), QuoteRemover.Remove(temps[0]).Replace("IG_", string.Empty), TryParseInt((temps.Length > 1) ? temps[1] : null, 1, 1));

            if (!_isFastConvert && !_itemListContainer.itemGroupIds.Contains(_itemContainer.id))
                _itemListContainer.itemGroupIds.Add(_itemContainer.id);
        }
        // pet
        if (text.Contains("pet "))
        {
            var temp = text.Replace("pet ", string.Empty);
            var temps = temp.Split(',');
            var monsterDatabase = GetMonsterDatabase(QuoteRemover.Remove(temps[0]));
            if ((monsterDatabase != null)
                && (monsterDatabase.captureRate <= 0))
                text = string.Format(_localization.GetTexts(Localization.PET), (monsterDatabase != null) ? "^FF0000" + monsterDatabase.name + "^000000" : temps[0]);
            else
                text = string.Format(_localization.GetTexts(Localization.PET_WITH_CHANCE), (monsterDatabase != null) ? "^FF0000" + monsterDatabase.name + "^000000" : temps[0], (monsterDatabase != null) ? monsterDatabase.captureRate : "0");

            if (!_isFastConvert && !_petTamingItemIds.Contains(_itemContainer.id))
                _petTamingItemIds.Add(_itemContainer.id);
        }
        // hateffect
        if (text.Contains("hateffect "))
        {
            var temp = text.Replace("hateffect ", string.Empty);
            var temps = temp.Split(',');
            text = string.Format(_localization.GetTexts(Localization.HAT_EFFECT)
                , (temps[1] == "true") ? _localization.GetTexts(Localization.HAT_EFFECT_TRUE) : _localization.GetTexts(Localization.HAT_EFFECT_FALSE)
                , temps[0].Replace("hat_ef_", string.Empty));
        }
        // switch
        if (text.Contains("switch("))
            text = text.Replace("switch(", _localization.GetTexts(Localization.IF) + "(");
        if (text.Contains("switch "))
            text = text.Replace("switch ", _localization.GetTexts(Localization.IF));
        // case
        if (text.Contains("case "))
        {
            text = text.Replace("case ", _localization.GetTexts(Localization.WAS));
            text = text.Replace(":", string.Empty);
            // Remove //
            if (text.Contains("/"))
                text = text.Substring(0, text.IndexOf("/"));
            text = "[" + GetItemName(text) + "]";
        }
        // setarray
        if (text.Contains("setarray "))
        {
            text = text.Replace("setarray ", "• ");
            var arrayName = text.Substring(text.IndexOf("."), text.IndexOf(",") - text.IndexOf("."));
            _arrayNames.Add(arrayName);
            text = text.Replace(arrayName + " , ", string.Empty);
            text = text.Replace(arrayName + " ,", string.Empty);
            text = text.Replace(arrayName + ", ", string.Empty);
            text = text.Replace(arrayName + ",", string.Empty);
        }
        // full for loop (Only remove it, no translation)
        if ((text.Contains("for (")
            || text.Contains("for("))
            && text.Contains(")"))
            text = string.Empty;

        text = text.Replace("sc_end_class", _localization.GetTexts(Localization.SC_END_CLASS));
        text = text.Replace("setmounting()", _localization.GetTexts(Localization.SET_MOUNTING));
        text = text.Replace("laphine_upgrade()", _localization.GetTexts(Localization.LAPHINE_UPGRADE));
        text = text.Replace("laphine_synthesis()", _localization.GetTexts(Localization.LAPHINE_SYNTHESIS));
        text = text.Replace("openstylist()", _localization.GetTexts(Localization.OPEN_STYLIST));
        text = text.Replace("refineui()", _localization.GetTexts(Localization.REFINE_UI));

        // All in one parse...
        text = AllInOneParse(text);

        // Negative value
        text = text.Replace("+-", "-");
        // Positive value
        text = text.Replace(" ++", " +");
        // autobonus string fix
        text = text.Replace("+%", "%");
        text = text.Replace("⁞", ":");

        // Whitespace
        text = text.Replace("    ", " ");
        text = text.Replace("   ", " ");
        text = text.Replace("  ", " ");
        text = text.Replace("\\", string.Empty);

        // Spacing fix
        text = text.Replace("     •", "•");
        text = text.Replace("    •", "•");
        text = text.Replace("   •", "•");
        text = text.Replace("  •", "•");
        text = text.Replace(" •", "•");

        return text;
    }

    string ParseRace(string text)
    {
        if (!text.Contains("rc_", StringComparison.CurrentCultureIgnoreCase))
            Debug.Log("Found wrong race: " + text + GetCurrentItemIdOrCombo());

        text = text.Replace("rc_angel", "^AC6523(Angel)^000000");
        text = text.Replace("rc_brute", "^AC6523(Brute)^000000");
        text = text.Replace("rc_demihuman", "^AC6523(Demi-Human)^000000");
        text = text.Replace("rc_demon", "^AC6523(Demon)^000000");
        text = text.Replace("rc_dragon", "^AC6523(Dragon)^000000");
        text = text.Replace("rc_fish", "^AC6523(Fish)^000000");
        text = text.Replace("rc_formless", "^AC6523(Formless)^000000");
        text = text.Replace("rc_insect", "^AC6523(Insect)^000000");
        text = text.Replace("rc_plant", "^AC6523(Plant)^000000");
        text = text.Replace("rc_player_human", "^AC6523(Human)^000000");
        text = text.Replace("rc_player_doram", "^AC6523(Doram)^000000");
        text = text.Replace("rc_undead", "^AC6523(Undead)^000000");
        text = text.Replace("rc_all", "^AC6523(" + _localization.GetTexts(Localization.ALL_RACE) + ")^000000");
        return text;
    }

    string ParseRace2(string text)
    {
        if (!text.Contains("rc2_", StringComparison.CurrentCultureIgnoreCase))
            Debug.Log("Found wrong race2: " + text + GetCurrentItemIdOrCombo());

        text = text.ToUpper();
        text = text.Replace("rc2_goblin", "^AC6523(Goblin)^000000");
        text = text.Replace("rc2_kobold", "^AC6523(Kobold)^000000");
        text = text.Replace("rc2_orc", "^AC6523(Orc)^000000");
        text = text.Replace("rc2_golem", "^AC6523(Golem)^000000");
        text = text.Replace("rc2_guardian", "^AC6523(Guardian)^000000");
        text = text.Replace("rc2_ninja", "^AC6523(Ninja)^000000");
        text = text.Replace("rc2_gvg", "^AC6523(GvG)^000000");
        text = text.Replace("rc2_battlefield", "^AC6523(Battlefield)^000000");
        text = text.Replace("rc2_treasure", "^AC6523(Treasure)^000000");
        text = text.Replace("rc2_biolab", "^AC6523(Biolab)^000000");
        text = text.Replace("rc2_manuk", "^AC6523(Manuk)^000000");
        text = text.Replace("rc2_splendide", "^AC6523(Splendide)^000000");
        text = text.Replace("rc2_scaraba", "^AC6523(Scaraba)^000000");
        text = text.Replace("rc2_ogh_atk_def", "^AC6523(Old Glast Heim)^000000");
        text = text.Replace("rc2_ogh_hidden", "^AC6523(Hidden Old Glast Heim)^000000");
        text = text.Replace("rc2_bio5_swordman_thief", "^AC6523(Biolab 5 Swordman & Theif)^000000");
        text = text.Replace("rc2_bio5_acolyte_merchant", "^AC6523(Biolab 5 Acolyte & Merchant)^000000");
        text = text.Replace("rc2_bio5_mage_archer", "^AC6523(Biolab 5 Mage & Archer)^000000");
        text = text.Replace("rc2_bio5_mvp", "^AC6523(Biolab 5 MvP)^000000");
        text = text.Replace("rc2_clocktower", "^AC6523(Clocktower)^000000");
        text = text.Replace("rc2_thanatos", "^AC6523(Thanatos)^000000");
        text = text.Replace("rc2_faceworm", "^AC6523(Faceworm)^000000");
        text = text.Replace("rc2_hearthunter", "^AC6523(Heart Hunter)^000000");
        text = text.Replace("rc2_rockridge", "^AC6523(Rockridge)^000000");
        text = text.Replace("rc2_werner_lab", "^AC6523(Werner's Laboratory)^000000");
        text = text.Replace("rc2_temple_demon", "^AC6523(Temple of Demon God)^000000");
        text = text.Replace("rc2_illusion_vampire", "^AC6523(Illusion Vampire)^000000");
        text = text.Replace("rc2_malangdo", "^AC6523(Malangdo)^000000");
        text = text.Replace("rc2_ep172alpha", "^AC6523(EP 17.2 Alpha)^000000");
        text = text.Replace("rc2_ep172beta", "^AC6523(EP 17.2 Beta)^000000");
        text = text.Replace("rc2_ep172bath", "^AC6523(EP 17.2 Bath)^000000");
        text = text.Replace("rc2_illusion_turtle", "^AC6523(Illusion Turtle)^000000");
        text = text.Replace("rc2_rachel_sanctuary", "^AC6523(Rachel Sanctuary)^000000");
        text = text.Replace("rc2_illusion_luanda", "^AC6523(Illusion Luanda)^000000");
        return text;
    }

    string ParseClass(string text)
    {
        if (!text.Contains("class_", StringComparison.CurrentCultureIgnoreCase))
            Debug.Log("Found wrong class: " + text + GetCurrentItemIdOrCombo());

        text = text.Replace("class_normal", "^0040B6(Normal)^000000");
        text = text.Replace("class_boss", "^0040B6(Boss)^000000");
        text = text.Replace("class_guardian", "^0040B6(Guardian)^000000");
        text = text.Replace("class_all", "^0040B6(" + _localization.GetTexts(Localization.ALL_CLASS) + ")^000000");
        return text;
    }

    string ParseSize(string text)
    {
        if (!text.Contains("size_", StringComparison.CurrentCultureIgnoreCase))
            Debug.Log("Found wrong size: " + text + GetCurrentItemIdOrCombo());

        text = text.Replace("size_small", "^FF26F5(" + _localization.GetTexts(Localization.SIZE_SMALL) + ")^000000");
        text = text.Replace("size_medium", "^FF26F5(" + _localization.GetTexts(Localization.SIZE_MEDIUM) + ")^000000");
        text = text.Replace("size_large", "^FF26F5(" + _localization.GetTexts(Localization.SIZE_LARGE) + ")^000000");
        text = text.Replace("size_all", "^FF26F5(" + _localization.GetTexts(Localization.ALL_SIZE) + ")^000000");
        return text;
    }

    string ParseElement(string text)
    {
        if (!text.Contains("ele_", StringComparison.CurrentCultureIgnoreCase))
            Debug.Log("Found wrong element: " + text + GetCurrentItemIdOrCombo());

        text = text.Replace("ele_dark", "^C426FF(Dark)^000000");
        text = text.Replace("ele_earth", "^C426FF(Earth)^000000");
        text = text.Replace("ele_fire", "^C426FF(Fire)^000000");
        text = text.Replace("ele_ghost", "^C426FF(Ghost)^000000");
        text = text.Replace("ele_holy", "^C426FF(Holy)^000000");
        text = text.Replace("ele_neutral", "^C426FF(Neutral)^000000");
        text = text.Replace("ele_poison", "^C426FF(Poison)^000000");
        text = text.Replace("ele_undead", "^C426FF(Undead)^000000");
        text = text.Replace("ele_water", "^C426FF(Water)^000000");
        text = text.Replace("ele_wind", "^C426FF(Wind)^000000");
        text = text.Replace("ele_all", "^C426FF(" + _localization.GetTexts(Localization.ALL_ELEMENT) + ")^000000");
        return text;
    }

    string ParseEffect(string text)
    {
        if (!text.Contains("eff_", StringComparison.CurrentCultureIgnoreCase))
            Debug.Log("Found wrong effect: " + text + GetCurrentItemIdOrCombo());

        text = text.Replace("eff_stone", "^EC1B3AStone^000000");
        text = text.Replace("eff_freeze", "^EC1B3AFreeze^000000");
        text = text.Replace("eff_stun", "^EC1B3AStun^000000");
        text = text.Replace("eff_sleep", "^EC1B3ASleep^000000");
        text = text.Replace("eff_poison", "^EC1B3APoison^000000");
        text = text.Replace("eff_curse", "^EC1B3ACurse^000000");
        text = text.Replace("eff_silence", "^EC1B3ASilence^000000");
        text = text.Replace("eff_confusion", "^EC1B3AConfusion^000000");
        text = text.Replace("eff_blind", "^EC1B3ABlind^000000");
        text = text.Replace("eff_bleeding", "^EC1B3ABleeding^000000");
        text = text.Replace("eff_dpoison", "^EC1B3ADeadly Poison^000000");
        text = text.Replace("eff_fear", "^EC1B3AFear^000000");
        text = text.Replace("eff_burning", "^EC1B3ABurning^000000");
        text = text.Replace("eff_crystalize", "^EC1B3ACrystalize^000000");
        text = text.Replace("eff_freezing", "^EC1B3AFreezing^000000");
        text = text.Replace("eff_heat", "^EC1B3AHeat^000000");
        text = text.Replace("eff_deepsleep", "^EC1B3ADeep Sleep^000000");
        text = text.Replace("eff_whiteimprison", "^EC1B3AWhite Imprison^000000");
        text = text.Replace("eff_hallucination", "^EC1B3AHallucination^000000");
        return text;
    }

    string ParseAtf(string text)
    {
        text = text.Replace("atk_self", _localization.GetTexts(Localization.ATF_SELF));
        text = text.Replace("atf_target", _localization.GetTexts(Localization.ATF_TARGET));
        text = text.Replace("atk_short", _localization.GetTexts(Localization.ATF_SHORT));
        text = text.Replace("bf_short", _localization.GetTexts(Localization.BF_SHORT));
        text = text.Replace("atf_long", _localization.GetTexts(Localization.ATF_LONG));
        text = text.Replace("bf_long", _localization.GetTexts(Localization.BF_LONG));
        text = text.Replace("atf_skill", _localization.GetTexts(Localization.ATF_SKILL));
        text = text.Replace("atf_weapon", _localization.GetTexts(Localization.ATF_WEAPON));
        text = text.Replace("bf_weapon", _localization.GetTexts(Localization.BF_WEAPON));
        text = text.Replace("atf_magic", _localization.GetTexts(Localization.ATF_MAGIC));
        text = text.Replace("bf_magic", _localization.GetTexts(Localization.BF_MAGIC));
        text = text.Replace("bf_skill", _localization.GetTexts(Localization.BF_SKILL));
        text = text.Replace("atf_misc", _localization.GetTexts(Localization.ATF_MISC));
        text = text.Replace("bf_misc", _localization.GetTexts(Localization.BF_MISC));
        text = text.Replace("bf_normal", _localization.GetTexts(Localization.BF_NORMAL));
        return text;
    }

    string GetAllAtf(string text)
    {
        string atf = string.Empty;

        if (text.Contains("atf_self"))
            atf += _localization.GetTexts(Localization.AUTO_BONUS_ATF_SELF) + ", ";
        if (text.Contains("atf_target"))
            atf += _localization.GetTexts(Localization.AUTO_BONUS_ATF_TARGET) + ", ";
        if (text.Contains("atf_short"))
            atf += _localization.GetTexts(Localization.AUTO_BONUS_ATF_SHORT) + ", ";
        if (text.Contains("bf_short"))
            atf += _localization.GetTexts(Localization.AUTO_BONUS_BF_SHORT) + ", ";
        if (text.Contains("atf_long"))
            atf += _localization.GetTexts(Localization.AUTO_BONUS_ATF_LONG) + ", ";
        if (text.Contains("bf_long"))
            atf += _localization.GetTexts(Localization.AUTO_BONUS_BF_LONG) + ", ";
        if (text.Contains("atf_skill"))
            atf += _localization.GetTexts(Localization.AUTO_BONUS_ATF_SKILL) + ", ";
        if (text.Contains("atf_weapon"))
            atf += _localization.GetTexts(Localization.AUTO_BONUS_ATF_WEAPON) + ", ";
        if (text.Contains("bf_weapon"))
            atf += _localization.GetTexts(Localization.AUTO_BONUS_BF_WEAPON) + ", ";
        if (text.Contains("atf_magic"))
            atf += _localization.GetTexts(Localization.AUTO_BONUS_ATF_MAGIC) + ", ";
        if (text.Contains("bf_magic"))
            atf += _localization.GetTexts(Localization.AUTO_BONUS_BF_MAGIC) + ", ";
        if (text.Contains("bf_skill"))
            atf += _localization.GetTexts(Localization.AUTO_BONUS_BF_SKILL) + ", ";
        if (text.Contains("atf_misc"))
            atf += _localization.GetTexts(Localization.AUTO_BONUS_ATF_MISC) + ", ";
        if (text.Contains("bf_misc"))
            atf += _localization.GetTexts(Localization.AUTO_BONUS_BF_MISC) + ", ";
        if (text.Contains("bf_normal"))
            atf += _localization.GetTexts(Localization.AUTO_BONUS_BF_NORMAL) + ", ";

        // Remove leftover ,
        if (!string.IsNullOrEmpty(atf))
            atf = atf.Substring(0, atf.Length - 2);
        else
            atf = _localization.GetTexts(Localization.PHYSICAL_DAMAGE);

        return atf;
    }

    string ParseI(string text)
    {
        text = text.Replace("0", _localization.GetTexts(Localization.AUTOSPELL_I_SELF));
        text = text.Replace("1", _localization.GetTexts(Localization.AUTOSPELL_I_TARGET));
        text = text.Replace("2", _localization.GetTexts(Localization.AUTOSPELL_I_SKILL));
        text = text.Replace("3", _localization.GetTexts(Localization.AUTOSPELL_I_SKILL_TO_TARGET));
        return text;
    }

    string ParseEQI(string text)
    {
        text = text.Replace("(EQI_SHADOW_ARMOR)", _localization.GetTexts(Localization.LOCATION_SHADOW_ARMOR));
        text = text.Replace("(EQI_SHADOW_WEAPON)", _localization.GetTexts(Localization.LOCATION_SHADOW_WEAPON));
        text = text.Replace("(EQI_SHADOW_SHIELD)", _localization.GetTexts(Localization.LOCATION_SHADOW_SHIELD));
        text = text.Replace("(EQI_SHADOW_SHOES)", _localization.GetTexts(Localization.LOCATION_SHADOW_SHOES));
        text = text.Replace("(EQI_SHADOW_ACC_R)", _localization.GetTexts(Localization.LOCATION_SHADOW_RIGHT_ACCESSORY));
        text = text.Replace("(EQI_SHADOW_ACC_L)", _localization.GetTexts(Localization.LOCATION_SHADOW_LEFT_ACCESSORY));
        text = text.Replace("(EQI_COMPOUND_ON)", _localization.GetTexts(Localization.EQUIP_COMPOUND_ON));
        text = text.Replace("(EQI_ACC_L)", _localization.GetTexts(Localization.LOCATION_LEFT_ACCESSORY));
        text = text.Replace("(EQI_ACC_R)", _localization.GetTexts(Localization.LOCATION_RIGHT_ACCESSORY));
        text = text.Replace("(EQI_SHOES)", _localization.GetTexts(Localization.LOCATION_SHOES));
        text = text.Replace("(EQI_GARMENT)", _localization.GetTexts(Localization.LOCATION_GARMENT));
        text = text.Replace("(EQI_HEAD_LOW)", _localization.GetTexts(Localization.LOCATION_HEAD_LOW));
        text = text.Replace("(EQI_HEAD_MID)", _localization.GetTexts(Localization.LOCATION_HEAD_MID));
        text = text.Replace("(EQI_HEAD_TOP)", _localization.GetTexts(Localization.LOCATION_HEAD_TOP));
        text = text.Replace("(EQI_ARMOR)", _localization.GetTexts(Localization.LOCATION_ARMOR));
        text = text.Replace("(EQI_HAND_L)", _localization.GetTexts(Localization.LOCATION_LEFT_HAND));
        text = text.Replace("(EQI_HAND_R)", _localization.GetTexts(Localization.LOCATION_RIGHT_HAND));
        text = text.Replace("(EQI_COSTUME_HEAD_TOP)", _localization.GetTexts(Localization.LOCATION_COSTUME_HEAD_TOP));
        text = text.Replace("(EQI_COSTUME_HEAD_MID)", _localization.GetTexts(Localization.LOCATION_COSTUME_HEAD_MID));
        text = text.Replace("(EQI_COSTUME_HEAD_LOW)", _localization.GetTexts(Localization.LOCATION_COSTUME_HEAD_LOW));
        text = text.Replace("(EQI_COSTUME_GARMENT)", _localization.GetTexts(Localization.LOCATION_COSTUME_GARMENT));
        text = text.Replace("(EQI_AMMO)", _localization.GetTexts(Localization.LOCATION_AMMO));

        text = text.Replace("EQI_SHADOW_ARMOR", _localization.GetTexts(Localization.LOCATION_SHADOW_ARMOR));
        text = text.Replace("EQI_SHADOW_WEAPON", _localization.GetTexts(Localization.LOCATION_SHADOW_WEAPON));
        text = text.Replace("EQI_SHADOW_SHIELD", _localization.GetTexts(Localization.LOCATION_SHADOW_SHIELD));
        text = text.Replace("EQI_SHADOW_SHOES", _localization.GetTexts(Localization.LOCATION_SHADOW_SHOES));
        text = text.Replace("EQI_SHADOW_ACC_R", _localization.GetTexts(Localization.LOCATION_SHADOW_RIGHT_ACCESSORY));
        text = text.Replace("EQI_SHADOW_ACC_L", _localization.GetTexts(Localization.LOCATION_SHADOW_LEFT_ACCESSORY));
        text = text.Replace("EQI_COMPOUND_ON", _localization.GetTexts(Localization.EQUIP_COMPOUND_ON));
        text = text.Replace("EQI_ACC_L", _localization.GetTexts(Localization.LOCATION_LEFT_ACCESSORY));
        text = text.Replace("EQI_ACC_R", _localization.GetTexts(Localization.LOCATION_RIGHT_ACCESSORY));
        text = text.Replace("EQI_SHOES", _localization.GetTexts(Localization.LOCATION_SHOES));
        text = text.Replace("EQI_GARMENT", _localization.GetTexts(Localization.LOCATION_GARMENT));
        text = text.Replace("EQI_HEAD_LOW", _localization.GetTexts(Localization.LOCATION_HEAD_LOW));
        text = text.Replace("EQI_HEAD_MID", _localization.GetTexts(Localization.LOCATION_HEAD_MID));
        text = text.Replace("EQI_HEAD_TOP", _localization.GetTexts(Localization.LOCATION_HEAD_TOP));
        text = text.Replace("EQI_ARMOR", _localization.GetTexts(Localization.LOCATION_ARMOR));
        text = text.Replace("EQI_HAND_L", _localization.GetTexts(Localization.LOCATION_LEFT_HAND));
        text = text.Replace("EQI_HAND_R", _localization.GetTexts(Localization.LOCATION_RIGHT_HAND));
        text = text.Replace("EQI_COSTUME_HEAD_TOP", _localization.GetTexts(Localization.LOCATION_COSTUME_HEAD_TOP));
        text = text.Replace("EQI_COSTUME_HEAD_MID", _localization.GetTexts(Localization.LOCATION_COSTUME_HEAD_MID));
        text = text.Replace("EQI_COSTUME_HEAD_LOW", _localization.GetTexts(Localization.LOCATION_COSTUME_HEAD_LOW));
        text = text.Replace("EQI_COSTUME_GARMENT", _localization.GetTexts(Localization.LOCATION_COSTUME_GARMENT));
        text = text.Replace("EQI_AMMO", _localization.GetTexts(Localization.LOCATION_AMMO));

        return text;
    }

    string ParseWeaponType(string text)
    {
        text = text.Replace("W_FIST", _localization.GetTexts(Localization.FIST));
        text = text.Replace("W_DAGGER", "Dagger");
        text = text.Replace("W_1HSWORD", "One-handed Sword");
        text = text.Replace("W_2HSWORD", "Two-handed Sword");
        text = text.Replace("W_1HSPEAR", "One-handed Spear");
        text = text.Replace("W_2HSPEAR", "Two-handed Spear");
        text = text.Replace("W_1HAXE", "One-handed Axe");
        text = text.Replace("W_2HAXE", "Two-handed Axe");
        text = text.Replace("W_MACE", "Mace");
        text = text.Replace("W_2HMACE", "Two-handed Mace");
        text = text.Replace("W_STAFF", "Staff");
        text = text.Replace("W_BOW", "Bow");
        text = text.Replace("W_KNUCKLE", "Knuckle");
        text = text.Replace("W_MUSICAL", "Musical");
        text = text.Replace("W_WHIP", "Whip");
        text = text.Replace("W_BOOK", "Book");
        text = text.Replace("W_KATAR", "Katar");
        text = text.Replace("W_REVOLVER", "Revolver");
        text = text.Replace("W_RIFLE", "Rifle");
        text = text.Replace("W_GATLING", "Gatling");
        text = text.Replace("W_SHOTGUN", "Shotgun");
        text = text.Replace("W_GRENADE", "Grenade Launcher");
        text = text.Replace("W_HUUMA", "Huuma");
        text = text.Replace("W_2HSTAFF", "Two-handed Staff");
        text = text.Replace("W_DOUBLE_DD,", "2 Daggers");
        text = text.Replace("W_DOUBLE_SS,", "2 Swords");
        text = text.Replace("W_DOUBLE_AA,", "2 Axes");
        text = text.Replace("W_DOUBLE_DS,", "Dagger + Sword");
        text = text.Replace("W_DOUBLE_DA,", "Dagger + Axe");
        text = text.Replace("W_DOUBLE_SA,", "Sword + Axe");
        text = text.Replace("W_SHIELD", _localization.GetTexts(Localization.SHIELD));
        return text;
    }

    string AllInOneParse(string text)
    {
        text = text.Replace("DT_DAYOFMONTH", _localization.GetTexts(Localization.DAY));
        text = text.Replace("DT_DAY", _localization.GetTexts(Localization.DAY));
        text = text.Replace("DT_MONTH", _localization.GetTexts(Localization.MONTH));
        text = text.Replace("DT_YEAR", _localization.GetTexts(Localization.YEAR));
        text = text.Replace("gettime", string.Empty);
        text = text.Replace("else if (", "^FF2525" + _localization.GetTexts(Localization.CONDITION_NOT_MET) + "^000000(");
        text = text.Replace("else if(", "^FF2525" + _localization.GetTexts(Localization.CONDITION_NOT_MET) + "^000000(");
        text = text.Replace("if (", "^FF2525" + _localization.GetTexts(Localization.IF) + "^000000(");
        text = text.Replace("else (", "^FF2525" + _localization.GetTexts(Localization.CONDITION_NOT_MET) + "^000000(");
        text = text.Replace("if(", "^FF2525" + _localization.GetTexts(Localization.IF) + "^000000(");
        text = text.Replace("else(", "^FF2525" + _localization.GetTexts(Localization.CONDITION_NOT_MET) + "^000000(");
        text = text.Replace("||", _localization.GetTexts(Localization.OR));
        text = text.Replace("&&", _localization.GetTexts(Localization.AND));
        text = text.Replace("&", " " + _localization.GetTexts(Localization.WAS) + " ");
        text = text.Replace("(BaseLevel)", "Lv.");
        text = text.Replace("BaseLevel", "Lv.");
        text = text.Replace("(BaseJob)", _localization.GetTexts(Localization.BASE_JOB));
        text = text.Replace("BaseJob", _localization.GetTexts(Localization.BASE_JOB));
        text = text.Replace("(BaseClass)", _localization.GetTexts(Localization.BASE_CLASS));
        text = text.Replace("BaseClass", _localization.GetTexts(Localization.BASE_CLASS));
        text = text.Replace("(JobLevel)", "Job Lv.");
        text = text.Replace("JobLevel", "Job Lv.");
        //text = text.Replace("readparam", _localization.GetTexts(Localization.READ_PARAM));
        text = text.Replace("readparam", string.Empty);
        text = text.Replace("(bSTR)", "STR");
        text = text.Replace("(bStr)", "STR");
        text = text.Replace("(bAGI)", "AGI");
        text = text.Replace("(bAgi)", "AGI");
        text = text.Replace("(bVIT)", "VIT");
        text = text.Replace("(bVit)", "VIT");
        text = text.Replace("(bINT)", "INT");
        text = text.Replace("(bInt)", "INT");
        text = text.Replace("(bDEX)", "DEX");
        text = text.Replace("(bDex)", "DEX");
        text = text.Replace("(bLUK)", "LUK");
        text = text.Replace("(bLuk)", "LUK");
        text = text.Replace("(bPow)", "POW");
        text = text.Replace("(bSta)", "STA");
        text = text.Replace("(bWis)", "WIS");
        text = text.Replace("(bSpl)", "SPL");
        text = text.Replace("(bCon)", "CON");
        text = text.Replace("(bCrt)", "CRT");
        text = text.Replace("eaclass()", "Class");
        text = text.Replace("getitempos()", _localization.GetTexts(Localization.GET_ITEM_POS));
        text = text.Replace("EAJL_2_1", _localization.GetTexts(Localization.CLASS) + " 2-1");
        text = text.Replace("EAJL_2_2", _localization.GetTexts(Localization.CLASS) + " 2-2");
        text = text.Replace("EAJL_2", _localization.GetTexts(Localization.CLASS) + " 2");
        text = text.Replace("EAJL_UPPER", _localization.GetTexts(Localization.HI_CLASS));
        text = text.Replace("EAJL_BABY", _localization.GetTexts(Localization.CLASS) + " Baby");
        text = text.Replace("EAJL_THIRD", _localization.GetTexts(Localization.CLASS) + " 3");
        text = text.Replace("EAJL_FOURTH", _localization.GetTexts(Localization.CLASS) + " 4");
        text = text.Replace("EAJ_BASEMASK", _localization.GetTexts(Localization.CLASS) + " 1");
        text = text.Replace("EAJ_UPPERMASK", _localization.GetTexts(Localization.HI_CLASS));
        text = text.Replace("EAJ_THIRDMASK", _localization.GetTexts(Localization.CLASS) + " 3");
        text = text.Replace("EAJ_FOURTHMASK", _localization.GetTexts(Localization.CLASS) + " 4");

        #region EAJ
        text = text.Replace("EAJ_NOVICE", " Novice");
        text = text.Replace("EAJ_SWORDMAN", " Swordman");
        text = text.Replace("EAJ_MAGE", " Mage");
        text = text.Replace("EAJ_ARCHER", " Archer");
        text = text.Replace("EAJ_ACOLYTE", " Acolyte");
        text = text.Replace("EAJ_MERCHANT", " Merchant");
        text = text.Replace("EAJ_THIEF", " Thief");
        text = text.Replace("EAJ_TAEKWON", " Taekwon");
        text = text.Replace("EAJ_GUNSLINGER", " Gunslinger");
        text = text.Replace("EAJ_NINJA", " Ninja");
        text = text.Replace("EAJ_GANGSI", " Gangsi");
        text = text.Replace("EAJ_KNIGHT", " Knight");
        text = text.Replace("EAJ_WIZARD", " Wizard");
        text = text.Replace("EAJ_HUNTER", " Hunter");
        text = text.Replace("EAJ_PRIEST", " Priest");
        text = text.Replace("EAJ_BLACKSMITH", " Blacksmith");
        text = text.Replace("EAJ_ASSASSIN", " Assassin");
        text = text.Replace("EAJ_STAR_GLADIATOR", " Star Gladiator");
        text = text.Replace("EAJ_STARGLADIATOR", " Star Gladiator");
        text = text.Replace("EAJ_REBELLION", " Rebellion");
        text = text.Replace("EAJ_KAGEROUOBORO", " Kagerou & Oboro");
        text = text.Replace("EAJ_DEATH_KNIGHT", " Death Knight");
        text = text.Replace("EAJ_DEATHKNIGHT", " Death Knight");
        text = text.Replace("EAJ_CRUSADER", " Crusader");
        text = text.Replace("EAJ_SAGE", " Sage");
        text = text.Replace("EAJ_BARDDANCER", " Bard & Dancer");
        text = text.Replace("EAJ_MONK", " Monk");
        text = text.Replace("EAJ_ALCHEMIST", " Alchemist");
        text = text.Replace("EAJ_ROGUE", " Rogue");
        text = text.Replace("EAJ_SOUL_LINKER", " Soul Linker");
        text = text.Replace("EAJ_SOULLINKER", " Soul Linker");
        text = text.Replace("EAJ_DARK_COLLECTOR", " Dark Colletor");
        text = text.Replace("EAJ_DARKCOLLECTOR", " Dark Colletor");
        text = text.Replace("EAJ_NOVICE_HIGH", " High Novice");
        text = text.Replace("EAJ_SWORDMAN_HIGH", " High Swordman");
        text = text.Replace("EAJ_MAGE_HIGH", " High Mage");
        text = text.Replace("EAJ_ARCHER_HIGH", " High Archer");
        text = text.Replace("EAJ_ACOLYTE_HIGH", " High Acolyte");
        text = text.Replace("EAJ_MERCHANT_HIGH", " High Merchant");
        text = text.Replace("EAJ_THIEF_HIGH", " High Thief");
        text = text.Replace("EAJ_LORD_KNIGHT", " Lord Knight");
        text = text.Replace("EAJ_HIGH_WIZARD", " High Wizard");
        text = text.Replace("EAJ_SNIPER", " Sniper");
        text = text.Replace("EAJ_HIGH_PRIEST", " High Priest");
        text = text.Replace("EAJ_WHITESMITH", " Whitesmith");
        text = text.Replace("EAJ_ASSASSIN_CROSS", " Assassin Cross");
        text = text.Replace("EAJ_PALADIN", " Paladin");
        text = text.Replace("EAJ_PROFESSOR", " Professor");
        text = text.Replace("EAJ_CLOWNGYPSY", " Clown & Gypsy");
        text = text.Replace("EAJ_CHAMPION", " Champion");
        text = text.Replace("EAJ_CREATOR", " Creator");
        text = text.Replace("EAJ_STALKER", " Stalker");
        text = text.Replace("EAJ_BABY", " Baby");
        text = text.Replace("EAJ_BABY_SWORDMAN", " Baby Swordman");
        text = text.Replace("EAJ_BABY_MAGE", " Baby Mage");
        text = text.Replace("EAJ_BABY_ARCHER", " Baby Archer");
        text = text.Replace("EAJ_BABY_ACOLYTE", " Baby Acolyte");
        text = text.Replace("EAJ_BABY_MERCHANT", " Baby Merchant");
        text = text.Replace("EAJ_BABY_THIEF", " Baby Thief");
        text = text.Replace("EAJ_BABY_TAEKWON", " Baby Taekwon");
        text = text.Replace("EAJ_BABY_GUNSLINGER", " Baby Gunslinger");
        text = text.Replace("EAJ_BABY_NINJA", " Baby Ninja");
        text = text.Replace("EAJ_BABY_SUMMONER", " Baby Summoner");
        text = text.Replace("EAJ_BABY_KNIGHT", " Baby Knight");
        text = text.Replace("EAJ_BABY_WIZARD", " Baby Wizard");
        text = text.Replace("EAJ_BABY_HUNTER", " Baby Hunter");
        text = text.Replace("EAJ_BABY_PRIEST", " Baby Priest");
        text = text.Replace("EAJ_BABY_BLACKSMITH", " Baby Blacksmith");
        text = text.Replace("EAJ_BABY_ASSASSIN", " Baby Assassin");
        text = text.Replace("EAJ_BABY_STAR_GLADIATOR", " Baby Star Gladiator");
        text = text.Replace("EAJ_BABY_REBELLION", " Baby Rebellion");
        text = text.Replace("EAJ_BABY_KAGEROUOBORO", " Baby Kagerou & Baby Oboro");
        text = text.Replace("EAJ_BABY_CRUSADER", " Baby Crusader");
        text = text.Replace("EAJ_BABY_SAGE", " Baby Sage");
        text = text.Replace("EAJ_BABY_BARDDANCER", " Baby Bard & Baby Dancer");
        text = text.Replace("EAJ_BABY_MONK", " Baby Monk");
        text = text.Replace("EAJ_BABY_ALCHEMIST", " Baby Alchemist");
        text = text.Replace("EAJ_BABY_ROGUE", " Baby Rouge");
        text = text.Replace("EAJ_BABY_SOUL_LINKER", " Baby Soul Linker");
        text = text.Replace("EAJ_RUNE_KNIGHT_T", " Rune Knight T.");
        text = text.Replace("EAJ_WARLOCK_T", " Warlock T.");
        text = text.Replace("EAJ_RANGER_T", " Ranger T.");
        text = text.Replace("EAJ_ARCH_BISHOP_T", " Arch Bishop T.");
        text = text.Replace("EAJ_MECHANIC_T", " Mechanic T.");
        text = text.Replace("EAJ_GUILLOTINE_CROSS_T", " Guilootine Cross T.");
        text = text.Replace("EAJ_ROYAL_GUARD_T", " Royal Guard T.");
        text = text.Replace("EAJ_SORCERER_T", " Sorcerer T.");
        text = text.Replace("EAJ_MINSTRELWANDERER_T", " Minstrel T. & Wanderer T.");
        text = text.Replace("EAJ_SURA_T", " Sura T.");
        text = text.Replace("EAJ_GENETIC_T", " Genetic T.");
        text = text.Replace("EAJ_SHADOW_CHASER_T", " Shadow Chaser T.");
        text = text.Replace("EAJ_RUNE_KNIGHT", " Rune Knight");
        text = text.Replace("EAJ_WARLOCK", " Warlock");
        text = text.Replace("EAJ_RANGER", " Ranger");
        text = text.Replace("EAJ_ARCH_BISHOP", " Arch Bishop");
        text = text.Replace("EAJ_MECHANIC", " Mechanic");
        text = text.Replace("EAJ_GUILLOTINE_CROSS", " Guillotine Cross");
        text = text.Replace("EAJ_STAR_EMPEROR", " Star Emperor");
        text = text.Replace("EAJ_ROYAL_GUARD", " Royal Guard");
        text = text.Replace("EAJ_SORCERER", " Sorcerer");
        text = text.Replace("EAJ_MINSTRELWANDERER", " Minstrel & Wanderer");
        text = text.Replace("EAJ_SURA", " Sura");
        text = text.Replace("EAJ_GENETIC", " Genetic");
        text = text.Replace("EAJ_SHADOW_CHASER", " Shadow Chaser");
        text = text.Replace("EAJ_SOUL_REAPER", " Soul Reaper");
        text = text.Replace("EAJ_BABY_RUNE", " Baby Rune Knight");
        text = text.Replace("EAJ_BABY_RUNE_KNIGHT", " Baby Rune Knight");
        text = text.Replace("EAJ_BABY_CROSS", " Baby Guillotine Cross");
        text = text.Replace("EAJ_BABY_GUILLOTINE_CROSS", " Baby Guillotine Cross");
        text = text.Replace("EAJ_BABY_BISHOP", " Baby Arch Bishop");
        text = text.Replace("EAJ_BABY_ARCH_BISHOP", " Baby Arch Bishop");
        text = text.Replace("EAJ_BABY_GUARD", " Baby Royal Guard");
        text = text.Replace("EAJ_BABY_ROYAL_GUARD", " Baby Royal Guard");
        text = text.Replace("EAJ_BABY_CHASER", " Baby Shadow Chaser");
        text = text.Replace("EAJ_BABY_SHADOW_CHASER", " Baby Shadow Chaser");
        text = text.Replace("EAJ_BABY_RUNE_KNIGHT", " Baby Rune Knight");
        text = text.Replace("EAJ_BABY_WARLOCK", " Baby Warlock");
        text = text.Replace("EAJ_BABY_RANGER", " Baby Ranger");
        text = text.Replace("EAJ_BABY_ARCH_BISHOP", " Baby Arch Bishop");
        text = text.Replace("EAJ_BABY_MECHANIC", " Baby Mechanic");
        text = text.Replace("EAJ_BABY_GUILLOTINE_CROSS", " Baby Guillotine Cross");
        text = text.Replace("EAJ_BABY_STAR_EMPEROR", " Baby Star Emperor");
        text = text.Replace("EAJ_BABY_ROYAL_GUARD", " Baby Royal Guard");
        text = text.Replace("EAJ_BABY_SORCERER", " Baby Sorcerer");
        text = text.Replace("EAJ_BABY_MINSTRELWANDERER", " Baby Minstrel & Baby Wanderer");
        text = text.Replace("EAJ_BABY_SURA", " Baby Sura");
        text = text.Replace("EAJ_BABY_GENETIC", " Baby Genetic");
        text = text.Replace("EAJ_BABY_SHADOW_CHASER", " Baby Shadow Chaser");
        text = text.Replace("EAJ_BABY_SOUL_REAPER", " Baby Soul Reaper");
        text = text.Replace("EAJ_SUPER_NOVICE", " Super Novice");
        text = text.Replace("EAJ_SUPERNOVICE", " Super Novice");
        text = text.Replace("EAJ_SUPER_BABY", " Super Baby");
        text = text.Replace("EAJ_SUPER_NOVICE_E", " Expanded Super Novice");
        text = text.Replace("EAJ_SUPER_BABY_E", " Expanded Super Baby");
        text = text.Replace("EAJ_SUMMONER", " Summoner");
        text = text.Replace("EAJ_SPIRIT_HANDLER", " Spirit Handler");
        text = text.Replace("EAJ_HYPER_NOVICE", " Hyper Novice");
        text = text.Replace("EAJ_DRAGON_KNIGHT", " Dragon Knight");
        text = text.Replace("EAJ_ARCH_MAGE", " Arch Mage");
        text = text.Replace("EAJ_WINDHAWK", " Windhawk");
        text = text.Replace("EAJ_CARDINAL", " Cardinal");
        text = text.Replace("EAJ_MEISTER", " Meister");
        text = text.Replace("EAJ_SHADOW_CROSS", " Shadow Cross");
        text = text.Replace("EAJ_SKY_EMPEROR", " Sky Emperor");
        text = text.Replace("EAJ_NIGHT_WATCH", " Night Watch");
        text = text.Replace("EAJ_SHINKIRO_SHIRANUI", " Shinkiro & Shiranui");
        text = text.Replace("EAJ_IMPERIAL_GUARD", " Imperial Guard");
        text = text.Replace("EAJ_ELEMENTAL_MASTER", " Elemental Master");
        text = text.Replace("EAJ_TROUBADOURTROUVERE", " Troubador & Trouvere");
        text = text.Replace("EAJ_INQUISITOR", " Inquisitor");
        text = text.Replace("EAJ_BIOLO", " Biolo");
        text = text.Replace("EAJ_ABYSS_CHASER", " Abyss Chaser");
        text = text.Replace("EAJ_SOUL_ASCETIC", " Soul Ascetic");
        #endregion

        #region JOB
        text = text.Replace("JOB_NOVICE", " Novice", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SWORDMAN", " Swordman", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_MAGE", " Mage", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_ARCHER", " Archer", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_ACOLYTE", " Acolyte", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_MERCHANT", " Merchant", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_THIEF", " Thief", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_TAEKWON", " Taekwon", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_GUNSLINGER", " Gunslinger", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_NINJA", " Ninja", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_GANGSI", " Gangsi", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_KNIGHT", " Knight", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_WIZARD", " Wizard", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_HUNTER", " Hunter", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_PRIEST", " Priest", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BLACKSMITH", " Blacksmith", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_ASSASSIN", " Assassin", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_STAR_GLADIATOR", " Star Gladiator", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_STARGLADIATOR", " Star Gladiator", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_REBELLION", " Rebellion", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_KAGEROUOBORO", " Kagerou & Oboro", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_DEATH_KNIGHT", " Death Knight", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_DEATHKNIGHT", " Death Knight", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_CRUSADER", " Crusader", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SAGE", " Sage", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BARDDANCER", " Bard & Dancer", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_MONK", " Monk", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_ALCHEMIST", " Alchemist", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_ROGUE", " Rogue", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SOUL_LINKER", " Soul Linker", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SOULLINKER", " Soul Linker", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_DARK_COLLECTOR", " Dark Colletor", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_DARKCOLLECTOR", " Dark Colletor", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_NOVICE_HIGH", " High Novice", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SWORDMAN_HIGH", " High Swordman", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_MAGE_HIGH", " High Mage", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_ARCHER_HIGH", " High Archer", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_ACOLYTE_HIGH", " High Acolyte", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_MERCHANT_HIGH", " High Merchant", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_THIEF_HIGH", " High Thief", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_LORD_KNIGHT", " Lord Knight", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_HIGH_WIZARD", " High Wizard", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SNIPER", " Sniper", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_HIGH_PRIEST", " High Priest", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_WHITESMITH", " Whitesmith", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_ASSASSIN_CROSS", " Assassin Cross", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_PALADIN", " Paladin", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_PROFESSOR", " Professor", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_CLOWNGYPSY", " Clown & Gypsy", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_CHAMPION", " Champion", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_CREATOR", " Creator", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_STALKER", " Stalker", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_SWORDMAN", " Baby Swordman", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_MAGE", " Baby Mage", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_ARCHER", " Baby Archer", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_ACOLYTE", " Baby Acolyte", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_MERCHANT", " Baby Merchant", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_THIEF", " Baby Thief", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_TAEKWON", " Baby Taekwon", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_GUNSLINGER", " Baby Gunslinger", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_NINJA", " Baby Ninja", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_SUMMONER", " Baby Summoner", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_KNIGHT", " Baby Knight", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_WIZARD", " Baby Wizard", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_HUNTER", " Baby Hunter", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_PRIEST", " Baby Priest", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_BLACKSMITH", " Baby Blacksmith", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_ASSASSIN", " Baby Assassin", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_STAR_GLADIATOR", " Baby Star Gladiator", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_REBELLION", " Baby Rebellion", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_KAGEROUOBORO", " Baby Kagerou & Baby Oboro", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_CRUSADER", " Baby Crusader", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_SAGE", " Baby Sage", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_BARDDANCER", " Baby Bard & Baby Dancer", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_MONK", " Baby Monk", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_ALCHEMIST", " Baby Alchemist", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_ROGUE", " Baby Rouge", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_SOUL_LINKER", " Baby Soul Linker", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_RUNE_KNIGHT_T", " Rune Knight T.", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_WARLOCK_T", " Warlock T.", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_RANGER_T", " Ranger T.", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_ARCH_BISHOP_T", " Arch Bishop T.", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_MECHANIC_T", " Mechanic T.", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_GUILLOTINE_CROSS_T", " Guilootine Cross T.", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_ROYAL_GUARD_T", " Royal Guard T.", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SORCERER_T", " Sorcerer T.", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_MINSTRELWANDERER_T", " Minstrel T. & Wanderer T.", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SURA_T", " Sura T.", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_GENETIC_T", " Genetic T.", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SHADOW_CHASER_T", " Shadow Chaser T.", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_RUNE_KNIGHT", " Rune Knight", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_WARLOCK", " Warlock", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_RANGER", " Ranger", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_ARCH_BISHOP", " Arch Bishop", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_MECHANIC", " Mechanic", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_GUILLOTINE_CROSS", " Guillotine Cross", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_STAR_EMPEROR", " Star Emperor", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_ROYAL_GUARD", " Royal Guard", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SORCERER", " Sorcerer", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_MINSTRELWANDERER", " Minstrel & Wanderer", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SURA", " Sura", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_GENETIC", " Genetic", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SHADOW_CHASER", " Shadow Chaser", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SOUL_REAPER", " Soul Reaper", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_RUNE", " Baby Rune Knight", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_RUNE_KNIGHT", " Baby Rune Knight", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_CROSS", " Baby Guillotine Cross", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_GUILLOTINE_CROSS", " Baby Guillotine Cross", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_BISHOP", " Baby Arch Bishop", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_ARCH_BISHOP", " Baby Arch Bishop", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_GUARD", " Baby Royal Guard", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_ROYAL_GUARD", " Baby Royal Guard", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_CHASER", " Baby Shadow Chaser", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_SHADOW_CHASER", " Baby Shadow Chaser", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_RUNE_KNIGHT", " Baby Rune Knight", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_WARLOCK", " Baby Warlock", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_RANGER", " Baby Ranger", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_ARCH_BISHOP", " Baby Arch Bishop", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_MECHANIC", " Baby Mechanic", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_GUILLOTINE_CROSS", " Baby Guillotine Cross", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_STAR_EMPEROR", " Baby Star Emperor", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_ROYAL_GUARD", " Baby Royal Guard", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_SORCERER", " Baby Sorcerer", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_MINSTRELWANDERER", " Baby Minstrel & Baby Wanderer", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_SURA", " Baby Sura", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_GENETIC", " Baby Genetic", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_SHADOW_CHASER", " Baby Shadow Chaser", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY_SOUL_REAPER", " Baby Soul Reaper", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SUPER_NOVICE", " Super Novice", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SUPERNOVICE", " Super Novice", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SUPER_BABY", " Super Baby", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SUPER_NOVICE_E", " Expanded Super Novice", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SUPER_BABY_E", " Expanded Super Baby", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SUMMONER", " Summoner", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SPIRIT_HANDLER", " Spirit Handler", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_HYPER_NOVICE", " Hyper Novice", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_DRAGON_KNIGHT", " Dragon Knight", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_ARCH_MAGE", " Arch Mage", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_WINDHAWK", " Windhawk", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_CARDINAL", " Cardinal", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_MEISTER", " Meister", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SHADOW_CROSS", " Shadow Cross", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SKY_EMPEROR", " Sky Emperor", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_NIGHT_WATCH", " Night Watch", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SHINKIRO_SHIRANUI", " Shinkiro & Shiranui", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_IMPERIAL_GUARD", " Imperial Guard", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_ELEMENTAL_MASTER", " Elemental Master", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_TROUBADOURTROUVERE", " Troubador & Trouvere", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_INQUISITOR", " Inquisitor", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BIOLO", " Biolo", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_ABYSS_CHASER", " Abyss Chaser", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_SOUL_ASCETIC", " Soul Ascetic", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("JOB_BABY", " Baby", StringComparison.OrdinalIgnoreCase);
        #endregion

        text = text.Replace("PETINFO_ID", "Pet");
        text = text.Replace("PETINFO_EGGID", "Pet");
        text = text.Replace("PETINFO_INTIMATE", _localization.GetTexts(Localization.PET_INFO_INTIMATE));
        text = text.Replace("PET_INTIMATE_LOYAL", _localization.GetTexts(Localization.PET_INTIMATE_LOYAL));
        text = text.Replace("ENCHANTGRADE_D", "D");
        text = text.Replace("ENCHANTGRADE_C", "C");
        text = text.Replace("ENCHANTGRADE_B", "B");
        text = text.Replace("ENCHANTGRADE_A", "A");

        text = text.Replace("(EQP_HEAD_LOW)", _localization.GetTexts(Localization.LOCATION_HEAD_LOW));
        text = text.Replace("(EQP_HEAD_MID)", _localization.GetTexts(Localization.LOCATION_HEAD_MID));
        text = text.Replace("(EQP_HEAD_TOP)", _localization.GetTexts(Localization.LOCATION_HEAD_TOP));
        text = text.Replace("(EQP_HAND_R)", _localization.GetTexts(Localization.LOCATION_RIGHT_HAND));
        text = text.Replace("(EQP_HAND_L)", _localization.GetTexts(Localization.LOCATION_LEFT_HAND));
        text = text.Replace("(EQP_ARMOR)", _localization.GetTexts(Localization.LOCATION_ARMOR));
        text = text.Replace("(EQP_SHOES)", _localization.GetTexts(Localization.LOCATION_SHOES));
        text = text.Replace("(EQP_GARMENT)", _localization.GetTexts(Localization.LOCATION_GARMENT));
        text = text.Replace("(EQP_ACC_R)", _localization.GetTexts(Localization.LOCATION_RIGHT_ACCESSORY));
        text = text.Replace("(EQP_ACC_L)", _localization.GetTexts(Localization.LOCATION_LEFT_ACCESSORY));
        text = text.Replace("(EQP_COSTUME_HEAD_TOP)", _localization.GetTexts(Localization.LOCATION_COSTUME_HEAD_TOP));
        text = text.Replace("(EQP_COSTUME_HEAD_MID)", _localization.GetTexts(Localization.LOCATION_COSTUME_HEAD_MID));
        text = text.Replace("(EQP_COSTUME_HEAD_LOW)", _localization.GetTexts(Localization.LOCATION_COSTUME_HEAD_LOW));
        text = text.Replace("(EQP_COSTUME_GARMENT)", _localization.GetTexts(Localization.LOCATION_COSTUME_GARMENT));
        text = text.Replace("(EQP_AMMO)", _localization.GetTexts(Localization.LOCATION_AMMO));
        text = text.Replace("(EQP_SHADOW_ARMOR)", _localization.GetTexts(Localization.LOCATION_SHADOW_ARMOR));
        text = text.Replace("(EQP_SHADOW_WEAPON)", _localization.GetTexts(Localization.LOCATION_SHADOW_WEAPON));
        text = text.Replace("(EQP_SHADOW_SHIELD)", _localization.GetTexts(Localization.LOCATION_SHADOW_SHIELD));
        text = text.Replace("(EQP_SHADOW_SHOES)", _localization.GetTexts(Localization.LOCATION_SHADOW_SHOES));
        text = text.Replace("(EQP_SHADOW_ACC_R)", _localization.GetTexts(Localization.LOCATION_SHADOW_RIGHT_ACCESSORY));
        text = text.Replace("(EQP_SHADOW_ACC_L)", _localization.GetTexts(Localization.LOCATION_SHADOW_LEFT_ACCESSORY));
        text = text.Replace("(EQP_ACC_RL)", _localization.GetTexts(Localization.LOCATION_BOTH_ACCESSORY));
        text = text.Replace("(EQP_SHADOW_ACC_RL)", _localization.GetTexts(Localization.LOCATION_BOTH_SHADOW_ACCESSORY));

        text = text.Replace("EQP_HEAD_LOW", _localization.GetTexts(Localization.LOCATION_HEAD_LOW));
        text = text.Replace("EQP_HEAD_MID", _localization.GetTexts(Localization.LOCATION_HEAD_MID));
        text = text.Replace("EQP_HEAD_TOP", _localization.GetTexts(Localization.LOCATION_HEAD_TOP));
        text = text.Replace("EQP_HAND_R", _localization.GetTexts(Localization.LOCATION_RIGHT_HAND));
        text = text.Replace("EQP_HAND_L", _localization.GetTexts(Localization.LOCATION_LEFT_HAND));
        text = text.Replace("EQP_ARMOR", _localization.GetTexts(Localization.LOCATION_ARMOR));
        text = text.Replace("EQP_SHOES", _localization.GetTexts(Localization.LOCATION_SHOES));
        text = text.Replace("EQP_GARMENT", _localization.GetTexts(Localization.LOCATION_GARMENT));
        text = text.Replace("EQP_ACC_R", _localization.GetTexts(Localization.LOCATION_RIGHT_ACCESSORY));
        text = text.Replace("EQP_ACC_L", _localization.GetTexts(Localization.LOCATION_LEFT_ACCESSORY));
        text = text.Replace("EQP_COSTUME_HEAD_TOP", _localization.GetTexts(Localization.LOCATION_COSTUME_HEAD_TOP));
        text = text.Replace("EQP_COSTUME_HEAD_MID", _localization.GetTexts(Localization.LOCATION_COSTUME_HEAD_MID));
        text = text.Replace("EQP_COSTUME_HEAD_LOW", _localization.GetTexts(Localization.LOCATION_COSTUME_HEAD_LOW));
        text = text.Replace("EQP_COSTUME_GARMENT", _localization.GetTexts(Localization.LOCATION_COSTUME_GARMENT));
        text = text.Replace("EQP_AMMO", _localization.GetTexts(Localization.LOCATION_AMMO));
        text = text.Replace("EQP_SHADOW_ARMOR", _localization.GetTexts(Localization.LOCATION_SHADOW_ARMOR));
        text = text.Replace("EQP_SHADOW_WEAPON", _localization.GetTexts(Localization.LOCATION_SHADOW_WEAPON));
        text = text.Replace("EQP_SHADOW_SHIELD", _localization.GetTexts(Localization.LOCATION_SHADOW_SHIELD));
        text = text.Replace("EQP_SHADOW_SHOES", _localization.GetTexts(Localization.LOCATION_SHADOW_SHOES));
        text = text.Replace("EQP_SHADOW_ACC_R", _localization.GetTexts(Localization.LOCATION_SHADOW_RIGHT_ACCESSORY));
        text = text.Replace("EQP_SHADOW_ACC_L", _localization.GetTexts(Localization.LOCATION_SHADOW_LEFT_ACCESSORY));
        text = text.Replace("EQP_ACC_RL", _localization.GetTexts(Localization.LOCATION_BOTH_ACCESSORY));
        text = text.Replace("EQP_SHADOW_ACC_RL", _localization.GetTexts(Localization.LOCATION_BOTH_SHADOW_ACCESSORY));

        //text = text.Replace("ITEMINFO_BUY", _localization.GetTexts(Localization.PRICE));
        //text = text.Replace("ITEMINFO_SELL", _localization.GetTexts(Localization.SELL_PRICE));
        //text = text.Replace("ITEMINFO_TYPE", _localization.GetTexts(Localization.TYPE));
        //text = text.Replace("ITEMINFO_MAXCHANCE", _localization.GetTexts(Localization.MAX_CHANCE));
        //text = text.Replace("ITEMINFO_GENDER", _localization.GetTexts(Localization.GENDER));
        //text = text.Replace("ITEMINFO_LOCATIONS", _localization.GetTexts(Localization.LOCATION));
        //text = text.Replace("ITEMINFO_WEIGHT", _localization.GetTexts(Localization.WEIGHT));
        //text = text.Replace("ITEMINFO_ATTACK", _localization.GetTexts(Localization.ATTACK));
        //text = text.Replace("ITEMINFO_DEFENSE", _localization.GetTexts(Localization.DEFENSE));
        //text = text.Replace("ITEMINFO_RANGE", _localization.GetTexts(Localization.RANGE));
        //text = text.Replace("ITEMINFO_SLOT", _localization.GetTexts(Localization.SLOT));
        //text = text.Replace("ITEMINFO_VIEW", _localization.GetTexts(Localization.VIEW));
        //text = text.Replace("ITEMINFO_EQUIPLEVELMIN", _localization.GetTexts(Localization.MINIMUM_EQUIP_LEVEL));
        text = text.Replace("ITEMINFO_WEAPONLEVEL", _localization.GetTexts(Localization.WEAPON_LEVEL));
        //text = text.Replace("ITEMINFO_ALIASNAME", _localization.GetTexts(Localization.ALIAS_NAME));
        //text = text.Replace("ITEMINFO_EQUIPLEVELMAX", _localization.GetTexts(Localization.MAXIMUM_EQUIP_LEVEL));
        //text = text.Replace("ITEMINFO_MAGICATTACK", _localization.GetTexts(Localization.MAGIC_ATTACK));
        //text = text.Replace("ITEMINFO_ID", "ID");
        //text = text.Replace("ITEMINFO_AEGISNAME", _localization.GetTexts(Localization.AEGIS_NAME));
        text = text.Replace("ITEMINFO_ARMORLEVEL", _localization.GetTexts(Localization.ARMOR_LEVEL));
        //text = text.Replace("ITEMINFO_SUBTYPE", _localization.GetTexts(Localization.SUB_TYPE));

        text = text.Replace("getpetinfo(", "(");
        text = text.Replace("getiteminfo(", "(");
        text = text.Replace("getequipid(", " Item ");
        text = text.Replace("getequiprefinerycnt", _localization.GetTexts(Localization.REFINE_COUNT));
        text = text.Replace("getenchantgrade()", _localization.GetTexts(Localization.GRADE_COUNT));
        text = text.Replace("getenchantgrade", _localization.GetTexts(Localization.GRADE_COUNT));
        text = text.Replace("getrefine()", _localization.GetTexts(Localization.REFINE_COUNT));
        text = text.Replace("getequipweaponlv()", _localization.GetTexts(Localization.GET_WEAPON_LEVEL));
        text = text.Replace("getequipweaponlv", _localization.GetTexts(Localization.GET_WEAPON_LEVEL));
        text = text.Replace("getequiparmorlv()", _localization.GetTexts(Localization.GET_EQUIPMENT_LEVEL));
        text = text.Replace("getequiparmorlv", _localization.GetTexts(Localization.GET_EQUIPMENT_LEVEL));
        text = text.Replace("ismounting()", _localization.GetTexts(Localization.IF_MOUNTING));
        text = text.Replace("!isequipped", _localization.GetTexts(Localization.IS_NOT_EQUIPPED));
        text = text.Replace("isequipped", _localization.GetTexts(Localization.IS_EQUIPPED));
        text = text.Replace("getskilllv", "Lv.");
        text = text.Replace("duplicate_dynamic", "เรียก");
        text = text.Replace("pow (", _localization.GetTexts(Localization.POW) + "(");
        text = text.Replace("pow(", _localization.GetTexts(Localization.POW) + "(");
        text = text.Replace("min (", _localization.GetTexts(Localization.MIN) + "(");
        text = text.Replace("min(", _localization.GetTexts(Localization.MIN) + "(");
        text = text.Replace("max (", _localization.GetTexts(Localization.MAX) + "(");
        text = text.Replace("max(", _localization.GetTexts(Localization.MAX) + "(");
        text = text.Replace("rand (", _localization.GetTexts(Localization.RAND) + "(");
        text = text.Replace("rand(", _localization.GetTexts(Localization.RAND) + "(");
        text = text.Replace("==", _localization.GetTexts(Localization.EQUAL));
        text = text.Replace("!=", " " + _localization.GetTexts(Localization.NOT_EQUAL) + " ");
        text = text.Replace("JOB_", string.Empty);
        text = text.Replace("Job_", string.Empty);
        text = text.Replace("job_", string.Empty);
        text = text.Replace("EAJ_", string.Empty);
        text = text.Replace("eaj_", string.Empty);
        text = text.Replace("else", "^FF2525" + _localization.GetTexts(Localization.CONDITION_NOT_MET) + "^000000");
        text = text.Replace(" ? ", " " + _localization.GetTexts(Localization.WILL_BE) + " ");
        text = text.Replace("?", " " + _localization.GetTexts(Localization.WILL_BE) + " ");
        text = text.Replace(" : ", " " + _localization.GetTexts(Localization.IF_NOT) + " ");
        text = text.Replace(":", " " + _localization.GetTexts(Localization.IF_NOT) + " ");

        for (int i = 0; i < _replaceVariables.Count; i++)
            text = SafeReplace.ReplaceWholeWord(text, _replaceVariables[i].variableName, _replaceVariables[i].descriptionConverted);
        for (int i = 0; i < _arrayNames.Count; i++)
        {
            if (text.Contains(_arrayNames[i])
                && text.Contains("[")
                && text.Contains("]"))
            {
                // Find last index of array
                var index = text.LastIndexOf(_arrayNames[i]);
                var temptoReplace = text.Substring(index);
                // Find index of ]
                var arrayToReplace = temptoReplace.Substring(0, temptoReplace.IndexOf(']') + 1);
                text = SafeReplace.ReplaceWholeWord(text, arrayToReplace, string.Empty);
            }
        }

        text = text.Replace("Lv.Lv.", "Lv.");

        text = ParseEQI(text);
        text = ParseWeaponType(text);

        List<string> foundSkillNames = new List<string>();

        // Some normal function contains skill name, let's try parse them
        while (text.Contains("\""))
        {
            // Try to find first "
            var firstQuoteIndex = text.IndexOf("\"");

            if ((firstQuoteIndex > 0)
                && (text.Length > (firstQuoteIndex + 1)))
            {
                // Substract text
                var subText = text.Substring(firstQuoteIndex + 1);

                // Try to find second "
                var secondQuoteIndex = subText.IndexOf("\"");

                // Good! Found 2 quote
                if (secondQuoteIndex > 0)
                {
                    var finalSubText = text.Substring(firstQuoteIndex + 1, secondQuoteIndex);

                    var skillName = GetSkillName(finalSubText, true, true);
                    foundSkillNames.Add(skillName);

                    text = text.Replace("\"" + finalSubText + "\"", skillName);
                }
                // Unexpected error (Had 1 quote)
                else
                    break;
            }
            // Unexpected error (Maybe " stay on last index of string and had 1 quote?)
            else
                break;
        }

        // Instead of (Bash) it should show Bash for readability
        for (int i = 0; i < foundSkillNames.Count; i++)
            text = text.Replace("(" + foundSkillNames[i] + ")", foundSkillNames[i]);

        text = QuoteRemover.Remove(text);

        if (_isRemoveBrackets)
        {
            text = text.Replace("{", string.Empty);
            text = text.Replace("}", string.Empty);
        }

        return text;
    }

    string TryParseInt(string text, float divider = 1, int defaultValue = 0)
    {
        if (text == "INFINITE_TICK")
            return _localization.GetTexts(Localization.INFINITE);

        if (!string.IsNullOrEmpty(text))
            text = text.Replace(";", string.Empty);

        float sum = -1;

        if (float.TryParse(text, out sum))
            sum = float.Parse(text);
        else if (divider == 1)
        {
            if (string.IsNullOrEmpty(text)
                || string.IsNullOrWhiteSpace(text))
                return defaultValue.ToString("f0");
            else
                return text;
        }
        else
            return text + " ^FF0000" + _localization.GetTexts(Localization.DIVIDE) + " " + divider.ToString("f0") + "^000000";

        return ParseNumberDecimal(sum, divider);
    }

    string ParseNumberDecimal(float number, float divider)
    {
        if ((number / divider) == 0)
            return (number / divider).ToString("f0");
        else if (((number / divider) > -0.1f)
            && ((number / divider) < 0.1f))
            return (number / divider).ToString("f2");
        else if (((number / divider) % 1) != 0)
            return (number / divider).ToString("f1");
        else
            return (number / divider).ToString("f0");
    }

    string GetCombo(string aegisName)
    {
        if (string.IsNullOrEmpty(aegisName))
            return string.Empty;

        StringBuilder builder = new StringBuilder();

        // Loop all combo data
        for (int i = 0; i < _comboDatabases.Count; i++)
        {
            var currentComboData = _comboDatabases[i];

            // Found
            if (currentComboData.IsAegisNameContain(aegisName))
            {
                StringBuilder sum = new StringBuilder();

                bool isFoundNow = false;

                for (int j = 0; j < currentComboData.sameComboDatas.Count; j++)
                {
                    var currentSameComboData = currentComboData.sameComboDatas[j];

                    // Add item name
                    for (int k = 0; k < currentSameComboData.aegisNames.Count; k++)
                    {
                        if (currentSameComboData.aegisNames[k] == aegisName)
                            isFoundNow = true;
                    }

                    if (isFoundNow)
                    {
                        // Declare header
                        var same_set_name_list = "			\"^666478" + _localization.GetTexts(Localization.EQUIP_WITH);

                        // Add item name
                        for (int k = 0; k < currentSameComboData.aegisNames.Count; k++)
                        {
                            var currentAegisName = currentSameComboData.aegisNames[k].ToLower();

                            // Should not add base item name
                            if (currentAegisName == aegisName)
                                continue;
                            else
                            {
                                var itemId = GetItemIdFromAegisName(currentAegisName);

                                if (!_isItemLink)
                                {
                                    same_set_name_list += "[NEW_LINE]+ " + GetItemName(itemId.ToString("f0"));
                                    same_set_name_list += "[NEW_LINE]+ (ID:" + itemId + ")";
                                }
                                else
                                    same_set_name_list += "[NEW_LINE]+ <ITEM>" + GetItemName(itemId.ToString("f0")) + "<INFO>" + itemId + "</INFO></ITEM>";
                            }
                        }

                        // End
                        same_set_name_list += "^000000\",\n";

                        sum.Append(same_set_name_list);

                        // Add combo bonus description
                        for (int l = 0; l < currentComboData.descriptions.Count; l++)
                        {
                            if (l >= currentComboData.descriptions.Count - 1)
                                sum.Append(currentComboData.descriptions[l]);
                            else
                                sum.Append(currentComboData.descriptions[l] + "\n");
                        }

                        // End
                        sum.Append("			\"————————————\",\n");

                        isFoundNow = false;
                    }
                }

                // Finalize this combo data
                builder.Append(sum);
            }
        }

        return builder.ToString();
    }

    string GetItemName(string text)
    {
        int _int = 0;

        if (int.TryParse(text, out _int))
        {
            _int = int.Parse(text);

            if (_itemDatabases.ContainsKey(_int))
                return _itemDatabases[_int].name;
        }
        else
        {
            var itemId = 0;

            var textLower = text.ToLower();

            if (_aegisNameDatabases.ContainsKey(textLower))
            {
                itemId = _aegisNameDatabases[textLower];

                if (_itemDatabases.ContainsKey(itemId))
                    return _itemDatabases[itemId].name;
            }
        }

        return text;
    }

    int GetItemIdFromAegisName(string text)
    {
        text = SpacingRemover.Remove(text);

        var textLower = text.ToLower();

        if (_aegisNameDatabases.ContainsKey(textLower))
            return _aegisNameDatabases[textLower];
        else
        {
            Debug.LogWarning(textLower + " not found in aegisNameDatabases");

            return 0;
        }
    }

    string GetResourceNameFromId(int id, string type, string subType, string location)
    {
        if (_isRandomResourceNameForCustomTextAssetOnly)
        {
            if (_isRandomResourceName && (id >= ItemGenerator.Instance.StartId))
            {
                if (subType.ToLower().Contains("dagger"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.daggers[UnityEngine.Random.Range(0, _resourceContainer.daggers.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.daggers[UnityEngine.Random.Range(0, _resourceContainer.daggers.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("1hsword"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.oneHandedSwords[UnityEngine.Random.Range(0, _resourceContainer.oneHandedSwords.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.oneHandedSwords[UnityEngine.Random.Range(0, _resourceContainer.oneHandedSwords.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("2hsword"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.twoHandedSwords[UnityEngine.Random.Range(0, _resourceContainer.twoHandedSwords.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.twoHandedSwords[UnityEngine.Random.Range(0, _resourceContainer.twoHandedSwords.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("1hspear"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.oneHandedSpears[UnityEngine.Random.Range(0, _resourceContainer.oneHandedSpears.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.oneHandedSpears[UnityEngine.Random.Range(0, _resourceContainer.oneHandedSpears.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("2hspear"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.twoHandedSpears[UnityEngine.Random.Range(0, _resourceContainer.twoHandedSpears.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.twoHandedSpears[UnityEngine.Random.Range(0, _resourceContainer.twoHandedSpears.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("1haxe"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.oneHandedAxes[UnityEngine.Random.Range(0, _resourceContainer.oneHandedAxes.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.oneHandedAxes[UnityEngine.Random.Range(0, _resourceContainer.oneHandedAxes.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("2haxe"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.twoHandedAxes[UnityEngine.Random.Range(0, _resourceContainer.twoHandedAxes.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.twoHandedAxes[UnityEngine.Random.Range(0, _resourceContainer.twoHandedAxes.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("mace"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.oneHandedMaces[UnityEngine.Random.Range(0, _resourceContainer.oneHandedMaces.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.oneHandedMaces[UnityEngine.Random.Range(0, _resourceContainer.oneHandedMaces.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("2hmace"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.twoHandedMaces[UnityEngine.Random.Range(0, _resourceContainer.twoHandedMaces.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.twoHandedMaces[UnityEngine.Random.Range(0, _resourceContainer.twoHandedMaces.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("staff"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.oneHandedStaffs[UnityEngine.Random.Range(0, _resourceContainer.oneHandedStaffs.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.oneHandedStaffs[UnityEngine.Random.Range(0, _resourceContainer.oneHandedStaffs.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("2hstaff"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.twoHandedStaffs[UnityEngine.Random.Range(0, _resourceContainer.twoHandedStaffs.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.twoHandedStaffs[UnityEngine.Random.Range(0, _resourceContainer.twoHandedStaffs.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("bow"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.bows[UnityEngine.Random.Range(0, _resourceContainer.bows.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.bows[UnityEngine.Random.Range(0, _resourceContainer.bows.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("knuckle"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.knuckles[UnityEngine.Random.Range(0, _resourceContainer.knuckles.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.knuckles[UnityEngine.Random.Range(0, _resourceContainer.knuckles.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("musical"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.musicals[UnityEngine.Random.Range(0, _resourceContainer.musicals.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.musicals[UnityEngine.Random.Range(0, _resourceContainer.musicals.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("whip"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.whips[UnityEngine.Random.Range(0, _resourceContainer.whips.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.whips[UnityEngine.Random.Range(0, _resourceContainer.whips.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("book"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.books[UnityEngine.Random.Range(0, _resourceContainer.books.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.books[UnityEngine.Random.Range(0, _resourceContainer.books.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("katar"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.katars[UnityEngine.Random.Range(0, _resourceContainer.katars.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.katars[UnityEngine.Random.Range(0, _resourceContainer.katars.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("revolver"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.revolvers[UnityEngine.Random.Range(0, _resourceContainer.revolvers.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.revolvers[UnityEngine.Random.Range(0, _resourceContainer.revolvers.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("rifle"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.rifles[UnityEngine.Random.Range(0, _resourceContainer.rifles.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.rifles[UnityEngine.Random.Range(0, _resourceContainer.rifles.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("gatling"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.gatlings[UnityEngine.Random.Range(0, _resourceContainer.gatlings.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.gatlings[UnityEngine.Random.Range(0, _resourceContainer.gatlings.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("shotgun"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.shotguns[UnityEngine.Random.Range(0, _resourceContainer.shotguns.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.shotguns[UnityEngine.Random.Range(0, _resourceContainer.shotguns.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("grenade"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.grenades[UnityEngine.Random.Range(0, _resourceContainer.grenades.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.grenades[UnityEngine.Random.Range(0, _resourceContainer.grenades.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("huuma"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.huumas[UnityEngine.Random.Range(0, _resourceContainer.huumas.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.huumas[UnityEngine.Random.Range(0, _resourceContainer.huumas.Count)]), null, null, null);
                    return s;
                }
                else if (subType.ToLower().Contains("enchant"))
                {
                    var s = GetResourceNameFromId(int.Parse(_resourceContainer.enchantments[UnityEngine.Random.Range(0, _resourceContainer.enchantments.Count)]), null, null, null);
                    while (s == "\"Bio_Reseearch_Docu\"")
                        s = GetResourceNameFromId(int.Parse(_resourceContainer.enchantments[UnityEngine.Random.Range(0, _resourceContainer.enchantments.Count)]), null, null, null);
                    return s;
                }

                if (type.ToLower() == "armor")
                {
                    if (location == _localization.GetTexts(Localization.LOCATION_HEAD_TOP))
                    {
                        var s = GetResourceNameFromId(int.Parse(_resourceContainer.topHeadgears[UnityEngine.Random.Range(0, _resourceContainer.topHeadgears.Count)]), null, null, null);
                        while (s == "\"Bio_Reseearch_Docu\"")
                            s = GetResourceNameFromId(int.Parse(_resourceContainer.topHeadgears[UnityEngine.Random.Range(0, _resourceContainer.topHeadgears.Count)]), null, null, null);
                        return s;
                    }
                    else if (location == _localization.GetTexts(Localization.LOCATION_HEAD_MID))
                    {
                        var s = GetResourceNameFromId(int.Parse(_resourceContainer.middleHeadgears[UnityEngine.Random.Range(0, _resourceContainer.middleHeadgears.Count)]), null, null, null);
                        while (s == "\"Bio_Reseearch_Docu\"")
                            s = GetResourceNameFromId(int.Parse(_resourceContainer.middleHeadgears[UnityEngine.Random.Range(0, _resourceContainer.middleHeadgears.Count)]), null, null, null);
                        return s;
                    }
                    else if (location == _localization.GetTexts(Localization.LOCATION_HEAD_LOW))
                    {
                        var s = GetResourceNameFromId(int.Parse(_resourceContainer.lowerHeadgears[UnityEngine.Random.Range(0, _resourceContainer.lowerHeadgears.Count)]), null, null, null);
                        while (s == "\"Bio_Reseearch_Docu\"")
                            s = GetResourceNameFromId(int.Parse(_resourceContainer.lowerHeadgears[UnityEngine.Random.Range(0, _resourceContainer.lowerHeadgears.Count)]), null, null, null);
                        return s;
                    }
                    else if (location == _localization.GetTexts(Localization.LOCATION_ARMOR))
                    {
                        var s = GetResourceNameFromId(int.Parse(_resourceContainer.armors[UnityEngine.Random.Range(0, _resourceContainer.armors.Count)]), null, null, null);
                        while (s == "\"Bio_Reseearch_Docu\"")
                            s = GetResourceNameFromId(int.Parse(_resourceContainer.armors[UnityEngine.Random.Range(0, _resourceContainer.armors.Count)]), null, null, null);
                        return s;
                    }
                    else if (location == _localization.GetTexts(Localization.LOCATION_GARMENT))
                    {
                        var s = GetResourceNameFromId(int.Parse(_resourceContainer.garments[UnityEngine.Random.Range(0, _resourceContainer.garments.Count)]), null, null, null);
                        while (s == "\"Bio_Reseearch_Docu\"")
                            s = GetResourceNameFromId(int.Parse(_resourceContainer.garments[UnityEngine.Random.Range(0, _resourceContainer.garments.Count)]), null, null, null);
                        return s;
                    }
                    else if (location == _localization.GetTexts(Localization.LOCATION_SHOES))
                    {
                        var s = GetResourceNameFromId(int.Parse(_resourceContainer.shoes[UnityEngine.Random.Range(0, _resourceContainer.shoes.Count)]), null, null, null);
                        while (s == "\"Bio_Reseearch_Docu\"")
                            s = GetResourceNameFromId(int.Parse(_resourceContainer.shoes[UnityEngine.Random.Range(0, _resourceContainer.shoes.Count)]), null, null, null);
                        return s;
                    }
                    else if (location == _localization.GetTexts(Localization.LOCATION_LEFT_ACCESSORY) || location == _localization.GetTexts(Localization.LOCATION_RIGHT_ACCESSORY) || location == _localization.GetTexts(Localization.LOCATION_BOTH_ACCESSORY))
                    {
                        var s = GetResourceNameFromId(int.Parse(_resourceContainer.accessorys[UnityEngine.Random.Range(0, _resourceContainer.accessorys.Count)]), null, null, null);
                        while (s == "\"Bio_Reseearch_Docu\"")
                            s = GetResourceNameFromId(int.Parse(_resourceContainer.accessorys[UnityEngine.Random.Range(0, _resourceContainer.accessorys.Count)]), null, null, null);
                        return s;
                    }
                    else if (location == _localization.GetTexts(Localization.LOCATION_LEFT_HAND))
                    {
                        var s = GetResourceNameFromId(int.Parse(_resourceContainer.shields[UnityEngine.Random.Range(0, _resourceContainer.shields.Count)]), null, null, null);
                        while (s == "\"Bio_Reseearch_Docu\"")
                            s = GetResourceNameFromId(int.Parse(_resourceContainer.shields[UnityEngine.Random.Range(0, _resourceContainer.shields.Count)]), null, null, null);
                        return s;
                    }
                }
                List<int> keys = new List<int>(_resourceDatabases.Keys);
                return _resourceDatabases[keys[UnityEngine.Random.Range(0, keys.Count)]];
            }
        }
        else
        {
            if (_isRandomResourceName)
            {
                List<int> keys = new List<int>(_resourceDatabases.Keys);
                return _resourceDatabases[keys[UnityEngine.Random.Range(0, keys.Count)]];
            }
        }

        if (_resourceDatabases.ContainsKey(id))
            return _resourceDatabases[id];

        if (!_isFastConvert && (id != 25786))
            errorResourceNames.Add(id.ToString());

        return "\"Bio_Reseearch_Docu\"";
    }

    string GetSkillName(string text, bool isKeepSpacebar = false, bool isOnlyString = false, bool isErrorReturnNull = false)
    {
        int _int = 0;

        if (int.TryParse(text, out _int)
            && !isOnlyString)
        {
            _int = int.Parse(text);

            if (_skillDatabases.ContainsKey(_int))
                return "^990B0B" + _skillDatabases[_int].description + "^000000";
        }
        else
        {
            if (!isKeepSpacebar)
                text = SpacingRemover.Remove(text);

            text = text.Replace(";", string.Empty);

            if (_skillNameDatabases.ContainsKey(text))
            {
                if (_skillDatabases.ContainsKey(_skillNameDatabases[text]))
                    return "^990B0B" + _skillDatabases[_skillNameDatabases[text]].description + "^000000";
            }
        }

        return isErrorReturnNull ? string.Empty : text;
    }

    string GetClassNumFromId(ItemContainer itemContainer)
    {
        if (_classNumberDatabases.ContainsKey(int.Parse(itemContainer.id)))
            return _classNumberDatabases[int.Parse(itemContainer.id)];
        // Default view for weapons
        else if (!string.IsNullOrEmpty(itemContainer.subType))
        {
            if (itemContainer.subType == "Fist")
                return "0";
            else if (itemContainer.subType == "Dagger")
                return "1";
            else if (itemContainer.subType == "1hSword")
                return "2";
            else if (itemContainer.subType == "2hSword")
                return "3";
            else if (itemContainer.subType == "1hSpear")
                return "4";
            else if (itemContainer.subType == "2hSpear")
                return "5";
            else if (itemContainer.subType == "1hAxe")
                return "6";
            else if (itemContainer.subType == "2hAxe")
                return "7";
            else if (itemContainer.subType == "Mace")
                return "8";
            else if (itemContainer.subType == "Staff")
                return "10";
            else if (itemContainer.subType == "Bow")
                return "11";
            else if (itemContainer.subType == "Knuckle")
                return "12";
            else if (itemContainer.subType == "Musical")
                return "13";
            else if (itemContainer.subType == "Whip")
                return "14";
            else if (itemContainer.subType == "Book")
                return "15";
            else if (itemContainer.subType == "Katar")
                return "16";
            else if (itemContainer.subType == "Revolver")
                return "17";
            else if (itemContainer.subType == "Rifle")
                return "18";
            else if (itemContainer.subType == "Gatling")
                return "19";
            else if (itemContainer.subType == "Shotgun")
                return "20";
            else if (itemContainer.subType == "Grenade")
                return "21";
            else if (itemContainer.subType == "Huuma")
                return "22";
            else if (itemContainer.subType == "2hStaff")
                return "23";
            else
                return "0";
        }
        // Default view for shields
        else if (!string.IsNullOrEmpty(itemContainer.type)
            && string.IsNullOrEmpty(itemContainer.locations)
            && (itemContainer.locations == _localization.GetTexts(Localization.LOCATION_LEFT_HAND)))
            return "1";
        else
            return "0";
    }

    string IsCostumeFromId(ItemContainer itemContainer)
    {
        if (itemContainer.locations.Contains(_localization.GetTexts(Localization.LOCATION_COSTUME_HEAD_TOP))
            || itemContainer.locations.Contains(_localization.GetTexts(Localization.LOCATION_COSTUME_HEAD_MID))
            || itemContainer.locations.Contains(_localization.GetTexts(Localization.LOCATION_COSTUME_HEAD_LOW))
            || itemContainer.locations.Contains(_localization.GetTexts(Localization.LOCATION_SHADOW_ARMOR))
            || itemContainer.locations.Contains(_localization.GetTexts(Localization.LOCATION_SHADOW_WEAPON))
            || itemContainer.locations.Contains(_localization.GetTexts(Localization.LOCATION_SHADOW_SHIELD))
            || itemContainer.locations.Contains(_localization.GetTexts(Localization.LOCATION_COSTUME_GARMENT))
            || itemContainer.locations.Contains(_localization.GetTexts(Localization.LOCATION_SHADOW_SHOES))
            || itemContainer.locations.Contains(_localization.GetTexts(Localization.LOCATION_SHADOW_LEFT_ACCESSORY))
            || itemContainer.locations.Contains(_localization.GetTexts(Localization.LOCATION_SHADOW_RIGHT_ACCESSORY)))
            return "true";
        else
            return "false";
    }

    string GetAvailableJobSeperator { get { return _isUseNewLineInsteadOfCommaForAvailableJob ? "[NEW_LINE]" : ", "; } }
    string GetAvailableJobBullet { get { return _isUseNewLineInsteadOfCommaForAvailableJob ? "— " : string.Empty; } }
    string GetAvailableClassSeperator { get { return _isUseNewLineInsteadOfCommaForAvailableClass ? "[NEW_LINE]" : ", "; } }
    string GetAvailableClassBullet { get { return _isUseNewLineInsteadOfCommaForAvailableClass ? "— " : string.Empty; } }

    /// <summary>
    /// Get monster database by ID
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    MonsterDatabase GetMonsterDatabase(string text)
    {
        int id = 0;

        if (int.TryParse(text, out id))
        {
            id = int.Parse(text);

            if (_monsterDatabases.ContainsKey(id))
                return _monsterDatabases[id];
        }

        return null;
    }

    /// <summary>
    /// Get item {name, aegis name} database by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    ItemDatabase GetItemDatabase(int id)
    {
        if (_itemDatabases.ContainsKey(id))
            return _itemDatabases[id];
        else
            return null;
    }

    /// <summary>
    /// Parse usable items that contains sc_start bonuses into item list
    /// </summary>
    void ParseStatusChangeStartIntoItemId()
    {
        if (_isFastConvert)
            return;

        if (!string.IsNullOrEmpty(_itemContainer.id)
            && !string.IsNullOrEmpty(_itemContainer.type))
        {
            if (!_itemListContainer.buffItemIds.Contains(_itemContainer.id)
                && ((_itemContainer.type.ToLower() == "healing")
                || (_itemContainer.type.ToLower() == "usable")
                || (_itemContainer.type.ToLower() == "cash")))
                _itemListContainer.buffItemIds.Add(_itemContainer.id);
        }
    }

    /// <summary>
    /// Timer Abbreviation
    /// </summary>
    /// <param name="timerText"></param>
    /// <returns></returns>
    string TryParseTimer(string timerText)
    {
        int integer = 0;
        if (int.TryParse(timerText, out integer))
        {
            bool isMinus = integer < 0;
            if (isMinus)
                integer = -integer;
            TimeSpan timeSpan = TimeSpan.FromSeconds(integer);
            if (timeSpan.Hours > 0 && timeSpan.Minutes > 0)
                return (isMinus ? "-" : string.Empty) + timeSpan.Hours + _localization.GetTexts(Localization.HOUR_ABBREVIATION) + " " + timeSpan.Minutes + _localization.GetTexts(Localization.MINUTE_ABBREVIATION);
            else if (timeSpan.Hours > 0)
                return (isMinus ? "-" : string.Empty) + timeSpan.Hours + _localization.GetTexts(Localization.HOUR_ABBREVIATION);
            else if (timeSpan.Minutes > 0 && timeSpan.Seconds > 0)
                return (isMinus ? "-" : string.Empty) + timeSpan.Minutes + _localization.GetTexts(Localization.MINUTE_ABBREVIATION) + " " + timeSpan.Seconds + _localization.GetTexts(Localization.SECOND_ABBREVIATION);
            else if (timeSpan.Minutes > 0)
                return (isMinus ? "-" : string.Empty) + timeSpan.Minutes + _localization.GetTexts(Localization.MINUTE_ABBREVIATION);
            else
                return (isMinus ? "-" : string.Empty) + timeSpan.Seconds + _localization.GetTexts(Localization.SECOND_ABBREVIATION);
        }
        else
            return timerText + _localization.GetTexts(Localization.SECOND_ABBREVIATION);
    }

    void ResetRefineGrade()
    {
        _replaceVariables = new List<ReplaceVariable>();
        _arrayNames = new List<string>();
    }

    bool IsItemTypeUsable(string itemType)
    {
        itemType = itemType.ToLower();

        if ((itemType == "healing")
            || (itemType == "usable")
            || (itemType == "delayconsume"))
            return true;
        else
            return false;
    }

    string GetCurrentItemIdOrCombo()
    {
        return ((_itemContainer != null) && !string.IsNullOrEmpty(_itemContainer.id))
            ? (" on " + _itemContainer.id)
            : !string.IsNullOrEmpty(_currentCombo)
            ? (" combo " + _currentCombo)
            : string.Empty;
    }

    bool IsContainScripts(ItemContainer _itemContainer)
    {
        return !string.IsNullOrEmpty(_itemContainer.script)
            || !string.IsNullOrEmpty(_itemContainer.equipScript)
            || !string.IsNullOrEmpty(_itemContainer.unequipScript)
            || !string.IsNullOrEmpty(GetCombo(GetItemDatabase(int.Parse(_itemContainer.id)).aegisName));
    }

    string GetItemMallSkillPrice(int id)
    {
        if (id == 7049) // Stone
            return STONE_PRICE;
        else if (id == 7135) // Bottle Grenade
            return CLASS_2_SKILL_ITEM_REQ_PRICE;
        else if (id == 7136) // Acid Bottle
            return CLASS_2_SKILL_ITEM_REQ_PRICE;
        else if (id == 608) // Yggdrasil Seed
            return YGGDRASIL_SEED_PRICE;
        else if (id == 607) // Yggdrasil Berry
            return YGGDRASIL_BERRY_PRICE;
        else if (id == 7137) // Plant Bottle
            return CLASS_2_SKILL_ITEM_REQ_PRICE;
        else if (id == 7138) // Marine Sphere Bottle
            return CLASS_2_SKILL_ITEM_REQ_PRICE;
        else if (id == 7139) // Glistening Coat
            return CLASS_2_SKILL_ITEM_REQ_PRICE;
        else if (id == 7142) // Ebmryo
            return CLASS_2_SKILL_ITEM_REQ_PRICE;
        else if (id == 6360) // Scarlet Point
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 6361) // Indigo Point
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 6362) // Yellow Wish Point
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 6363) // Lime Green Point
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 12115) // Water Elemental Converter
            return ELEMENT_CONVERTER_PRICE;
        else if (id == 12116) // Earth Elemental Converter
            return ELEMENT_CONVERTER_PRICE;
        else if (id == 12114) // Fire Elemental Converter
            return ELEMENT_CONVERTER_PRICE;
        else if (id == 12117) // Wind Elemental Converter
            return ELEMENT_CONVERTER_PRICE;
        else if (id == 6128) // Antidote
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 12333) // Ancilla
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 16030) // Pile Bunker S
            return PILE_BUNKER_PRICE;
        else if (id == 16031) // Pile Bunker P
            return PILE_BUNKER_PRICE;
        else if (id == 16032) // Pile Bunker T
            return PILE_BUNKER_PRICE;
        else if (id == 1000352) // Device Creation Guide
            return CLASS_4_SKILL_ITEM_REQ_PRICE;
        else if (id == 1000289) // Device Capsule
            return CLASS_4_SKILL_ITEM_REQ_PRICE;
        else if (id == 1000290) // Auto Battle Capsule
            return CLASS_4_SKILL_ITEM_REQ_PRICE;
        else if (id == 1000280) // High Coating Bottle
            return CLASS_4_SKILL_ITEM_REQ_PRICE;
        else if (id == 1000279) // Icicle Acid Bottle
            return CLASS_4_SKILL_ITEM_REQ_PRICE;
        else if (id == 1000277) // Earth Acid Bottle
            return CLASS_4_SKILL_ITEM_REQ_PRICE;
        else if (id == 1000278) // Gale Acid Bottle
            return CLASS_4_SKILL_ITEM_REQ_PRICE;
        else if (id == 1000276) // Flame Acid Bottle
            return CLASS_4_SKILL_ITEM_REQ_PRICE;
        else if (id == 1000281) // High Plant Bottle
            return CLASS_4_SKILL_ITEM_REQ_PRICE;
        else if (id == 1000564) // Nw Grenade
            return CLASS_4_SKILL_ITEM_REQ_PRICE;
        else if (id == 22549) // Poison Bottle
            return POISON_BOTTLE_PRICE;
        else if (id == 12717) // Paralysis
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 12722) // Pyrexia
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 12720) // Death Hurt
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 12718) // Leech End
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 12724) // Venom Bleed
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 12723) // Magic Mushroom
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 12721) // Toxin
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 12719) // Oblivion Curse
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 100065) // Spell Book (Storm Gust)
            return SPELL_BOOK_TIER_1_PRICE;
        else if (id == 100066) // Spell Book (Lord of Vermilion)
            return SPELL_BOOK_TIER_1_PRICE;
        else if (id == 100067) // Spell Book (Meteor Storm)
            return SPELL_BOOK_TIER_1_PRICE;
        else if (id == 100068) // Spell Book (Drain Life)
            return SPELL_BOOK_TIER_2_PRICE;
        else if (id == 100069) // Spell Book (Jack Frost)
            return SPELL_BOOK_TIER_2_PRICE;
        else if (id == 100070) // Spell Book (Earth Strain)
            return SPELL_BOOK_TIER_2_PRICE;
        else if (id == 100071) // Spell Book (Crimson Rock)
            return SPELL_BOOK_TIER_2_PRICE;
        else if (id == 100072) // Spell Book (Chain Lightning)
            return SPELL_BOOK_TIER_2_PRICE;
        else if (id == 100073) // Spell Book (Comet)
            return SPELL_BOOK_TIER_3_PRICE;
        else if (id == 100074) // Spell Book (Tetra Vortex)
            return SPELL_BOOK_TIER_3_PRICE;
        else if (id == 11022) // Mix Cook Book
            return POTION_BOOK_PRICE;
        else if (id == 11023) // Increase Stamina Study
            return POTION_BOOK_PRICE;
        else if (id == 11024) // Vital Drink CB
            return POTION_BOOK_PRICE;
        else if (id == 1000293) // Flame Stone 4th
            return CLASS_4_SKILL_ITEM_REQ_PRICE;
        else if (id == 1000295) // Ice Stone 4th
            return CLASS_4_SKILL_ITEM_REQ_PRICE;
        else if (id == 1000291) // Wind Stone 4th
            return CLASS_4_SKILL_ITEM_REQ_PRICE;
        else if (id == 1000292) // Earth Stone 4th
            return CLASS_4_SKILL_ITEM_REQ_PRICE;
        else if (id == 1000294) // Poison Stone 4th
            return CLASS_4_SKILL_ITEM_REQ_PRICE;
        else if (id == 12731) // Thurisaz Rune
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 12728) // Isa Rune
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 12732) // Wyrd Rune
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 12733) // Hagalaz Rune
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 12729) // Othila Rune
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 12730) // Uruz Rune
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 12726) // Raido Rune
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 12725) // Nauthiz Rune
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 12727) // Berkana Rune
            return CLASS_3_SKILL_ITEM_REQ_PRICE;
        else if (id == 22540) // Lux Anima Runestone
            return LUN_ANIMA_RUNESTONE_PRICE;
        else
            return "-1";
    }

    bool IsItemMallSkillError(string aegisItemName)
    {
        return (aegisItemName == "Fruit_Of_Mastela") // Appeared in Potion Pitcher Lv. 6
             || (aegisItemName == "Royal_Jelly") // Appeared in Potion Pitcher Lv. 7
             || (aegisItemName == "Seed_Of_Yggdrasil") // Appeared in Potion Pitcher Lv. 8
             || (aegisItemName == "Yggdrasilberry") // Appeared in Potion Pitcher Lv. 9
             ;
    }

    string RemoveGodItem(string input)
    {
        input = input.Replace(",1599,", string.Empty);
        input = input.Replace(",2199,", string.Empty);
        input = input.Replace(",15065,", string.Empty);
        input = input.Replace(",2904,", string.Empty);
        input = input.Replace(",19429,", string.Empty);
        input = input.Replace(",5013,", string.Empty);
        return input;
    }
}
