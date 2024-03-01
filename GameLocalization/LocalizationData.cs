using System.Collections.Generic;

public class LocalizationData
{
    public List<LocalizationItems> items;
}

[System.Serializable]
public class LocalizationItems
{
    public string key;
    public string value;
}
