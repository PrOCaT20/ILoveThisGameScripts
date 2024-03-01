using UnityEngine;

public class LocaleHelper
{ 
    public static string GetSupportedLanguageCode()
    {
        SystemLanguage lang = Application.systemLanguage;

        switch (lang)
        {
            case SystemLanguage.English:
                return LocaleApplication.EN;
            case SystemLanguage.Russian:
                return LocaleApplication.RU;
            case SystemLanguage.Catalan:
                return LocaleApplication.CA;
            case SystemLanguage.Ukrainian:
                return LocaleApplication.UA;
            case SystemLanguage.Spanish:
                return LocaleApplication.ES;
            default:
                return LocaleApplication.EN;
        }
    }
}