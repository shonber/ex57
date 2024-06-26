using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Collections;
using System;
using System.Text;
using System.ComponentModel;

namespace MyProject;

public class NumericalExpression
{
    private static readonly Dictionary<string, string[]> unitsArrayTranslations = new (){
        {"en", new string[]
            {"Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" }
        },
        {"he", new string[]
            {"אפס", "אחת", "שתיים", "שלוש", "ארבע", "חמש", "שש", "שבע", "שמונה", "תשע", "עשר", "אחת עשרה", "שתיים עשרה", "שלוש עשרה", "ארבע עשרה", "חמש עשרה", "שש עשרה", "שבע עשרה", "שמונה עשרה", "תשע עשרה" }
        },
    };

    private static readonly Dictionary<string, string[]> tensArrayTranslations = new (){
        {"en", new string[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" }},
        {"he", new string[]
            {"אפס", "עשר", "עשרים", "שלושים", "ארבעים", "חמישים", "שישים", "שבעים", "שמונים", "תשעים" }
        },
    };

    private static readonly Dictionary<string, string[]> multiplierTextsTranslations = new (){
        {"en", new string[] { "Hundred", "Thousand", "Million", "Billion", "Trillion" }},
        {"he", new string[]
            {"מאה", "אלף", "מיליון", "מיליארד", "טריליון"}
        },
    };

    private static ulong[] multipliers =  [ 100, 1000, 1000000, 1000000000, 1000000000000 ];
    private static ulong maxNumber = 999000000000000;
                                    
    private static string[] supportedLanguages = ["en", "he"];
    private static string selectedLanguage = supportedLanguages[0];

    private static readonly Func<string, string[]> GetUnitsArray = str => unitsArrayTranslations[str];
    private static readonly Func<string, string[]> GetTensArray = str => tensArrayTranslations[str];
    private static readonly Func<string, string[]> GetMultiplierTextsTranslations = str => multiplierTextsTranslations[str];
    private static readonly Func<string, string[], string[]> SetMultiplierTextsTranslations = (str, newArr) => multiplierTextsTranslations[str] = newArr;

    private readonly ulong number;
    private static readonly string encodingFormat = "UTF-16";

    public NumericalExpression (ulong n){
        this.number = n;
    }

    public ulong GetValue (){
        // The method returns the number attribute value.

        return this.number;
    } 

    public static int SumLetters (ulong n){
        // The method calls ConvertToString() method on numbers from 0 to @n: ulong and counts how many letters are needed without spaces.

        if (n > maxNumber){
            Console.WriteLine("[-] Number is too big.");
            return 0;
        }

        int charCounter = 0;
        for (ulong i = 0; i <= n; i++)
        {
            string word = ConvertToString(i).Replace(" ", "");
            charCounter += word.Length;
        }

        return charCounter;
    } 

    // Method Overloading (Compile-Time Polymorphism) is the OOP principle being used.
    public static int SumLetters (NumericalExpression n){
        // The method calls ConvertToString() method on numbers from 0 to @n: NumericalExpression and counts how many letters are needed without spaces.

        if (n.number > maxNumber){
            Console.WriteLine("[-] Number is too big.");
            return 0;
        }

        int charCounter = 0;
        for (ulong i = 0; i <= n.number; i++)
        {
            string word = ConvertToString(i).Replace(" ", "");
            charCounter += word.Length;
        }

        return charCounter;
    } 

    private static string ConvertToString (ulong n){
        // The method uses recursion technique to convert a @number: ulong to its word form.

        // Get translations based on selected language.
        string[] unitsArray = GetUnitsArray(selectedLanguage);
        string[] tensArray = GetTensArray(selectedLanguage);
        string[] multiplierTexts = GetMultiplierTextsTranslations(selectedLanguage);

        if (n > maxNumber)
            return "[-] Number is too big.";

        if (n == 0)
            return unitsArray[0];

        if (n < 0)
            return "[-] Negative numbers are yet to be supported.";

        StringBuilder retVal = new();

        // Goes over each multiplier in the list and runs recursion on it if the condition returns True.
        for (int i = multipliers.Length - 1; i >= 0 ; i--)
        {
            if ((n / multipliers[i]) > 0)
            {
                try
                {
                    if (multiplierTexts[i] == null){
                        Console.WriteLine("MISSING TRANSLATION");
                    } 
                }
                catch (System.IndexOutOfRangeException)
                {
                    Console.Write($"[!] Missing Translation, insert a name for the number {multipliers[i]} - >> ");
                    string? newNumberName = UTF8ReadLine();

                    if ( multiplierTexts.Contains(newNumberName) || (newNumberName == "") || (newNumberName == " ") || (newNumberName == null) ){
                        Console.WriteLine("[-] That name is already being used or invalid.");
                    }
                    Array.Resize(ref multiplierTexts, multiplierTexts.Length + 1);

                    multiplierTexts[^1] = newNumberName;
                    SetMultiplierTextsTranslations(selectedLanguage, multiplierTexts);
                } finally{
                    retVal.Append(ConvertToString(n / multipliers[i]) + $" {multiplierTexts[i]} ");
                    n %= multipliers[i];
                }
            }            
        }

        // Checks if there are any Tens or Units.
        if (n > 0){
            if (n < 20)
                retVal.Append(unitsArray[n]);
            else{
                retVal.Append(tensArray[(n / 10)]);
                if ((n % 10) > 0)
                    retVal.Append(" ");
                    retVal.Append(unitsArray[(n % 10)]);
            }
        }

        // Returns the result and remove unnecessary double spaces.
        return retVal.ToString().Replace("  ", " ");
    } 

    public static void SetMaxNumber(){
        // The method allows the client to change the max number.

        Console.WriteLine($"[!] The current max number is >> {maxNumber}");

        Console.Write("[!] New max number (100, 1000, 1000000, etc.) >>> ");
        ulong newMaxNumber = ulong.Parse(Console.ReadLine());

        if ( (newMaxNumber / multipliers[^1] != 1000) || (newMaxNumber % 10 != 0) ){
            Console.WriteLine("[-] Insert a number that is bigger by 1000.");
            return;
        }

        Console.Write("[!] New max number name (Hundred, Thousand, Million, etc.) >>> ");
        string? newMaxNumberName = UTF8ReadLine();

        string[] multiplierTexts = GetMultiplierTextsTranslations(selectedLanguage);

        if ( multiplierTexts.Contains(newMaxNumberName) || (newMaxNumberName == "") || (newMaxNumberName == " ") || (newMaxNumberName == null) ){
            Console.WriteLine("[-] That name is already being used or invalid.");
            return;
        }

        Array.Resize(ref multiplierTexts, multiplierTexts.Length + 1);
        Array.Resize(ref multipliers, multipliers.Length + 1);

        maxNumber = 999 * newMaxNumber;
        multipliers[^1] = newMaxNumber;

        multiplierTexts[^1] = newMaxNumberName;
        SetMultiplierTextsTranslations(selectedLanguage, multiplierTexts);

        string[] multiplierTextsEnglish = GetMultiplierTextsTranslations("en");
        if ( !multiplierTextsEnglish.Contains(newMaxNumberName) ){
            Array.Resize(ref multiplierTextsEnglish, multiplierTextsEnglish.Length + 1);
            multiplierTextsEnglish[^1] = newMaxNumberName;
            SetMultiplierTextsTranslations(selectedLanguage, multiplierTextsEnglish);
        }

        Console.WriteLine("[+] Updated the max number successfully.");
    }

    public static void SelectLanguage(){
        // The method allows the user to switch between supported languages.

        StringBuilder supportedLanguagesRetVal = new();

        for (int i = 0; i < supportedLanguages.Length; i++)
        {
            if (i == supportedLanguages.Length - 1){
                supportedLanguagesRetVal.Append("and ");
                supportedLanguagesRetVal.Append(supportedLanguages[i]);
                supportedLanguagesRetVal.Append('.');
            }else{
                supportedLanguagesRetVal.Append(supportedLanguages[i]);
                supportedLanguagesRetVal.Append(", ");
            }
        }

        Console.WriteLine($"[!] Current supported languages >> {supportedLanguagesRetVal.ToString()}");
        Console.Write("[!] Select a language >>> ");
        string newSelected = UTF8ReadLine();

        if (!supportedLanguages.Contains(newSelected)){
            Console.WriteLine("[-] The selected language is yet to be supported.");
            return;
        }

        selectedLanguage = newSelected;

    }

    public static string GetSelectedLanguage(){
        // The method returns the selected language.

        return selectedLanguage;
    }

    public static void AddLanguage(){
        // The method allows the client to add additional languages to the class

        StringBuilder supportedLanguagesRetVal = new();

        for (int i = 0; i < supportedLanguages.Length; i++)
        {
            if (i == supportedLanguages.Length - 1){
                supportedLanguagesRetVal.Append("and ");
                supportedLanguagesRetVal.Append(supportedLanguages[i]);
                supportedLanguagesRetVal.Append('.');
            }else{
                supportedLanguagesRetVal.Append(supportedLanguages[i]);
                supportedLanguagesRetVal.Append(", ");
            }
        }

        Console.WriteLine($"[!] Current supported languages >> {supportedLanguagesRetVal.ToString()}");
        Console.Write("[!] New language name >>> ");
        string languageName = UTF8ReadLine();

        if (supportedLanguages.Contains(languageName) || languageName == "" || languageName == " "){
            Console.WriteLine("[!] The selected language is already supported or not valid.");
            return;
        }

        Console.WriteLine("[!] Insert a name for the following fields");
        Console.WriteLine("__________________________________________");

        // Get new translations for the new language.
        string[] unitsArray = GetUnitsArray("en");
        string[] tensArray = GetTensArray("en");
        string[] multiplierTexts = GetMultiplierTextsTranslations("en");

        string[] newUnitsArray = new string[unitsArray.Length];
        string[] newTensArray = new string[tensArray.Length];
        string[] newMultiplierTexts = new string[multiplierTexts.Length];

        Console.WriteLine("****Units****");
        for (int i = 0; i < unitsArray.Length; i++)
        {
            Console.Write($"\t[!] {unitsArray[i]} = ");
            newUnitsArray[i] = UTF8ReadLine();
        }

        Console.WriteLine("\n****Tens****");
        for (int i = 0; i < tensArray.Length; i++)
        {
            Console.Write($"\t[!] {tensArray[i]} = ");
            newTensArray[i] = UTF8ReadLine();
        }

        Console.WriteLine("\n****Multipliers****");
        for (int i = 0; i < multiplierTexts.Length; i++)
        {
            Console.Write($"\t[!] {multiplierTexts[i]} = ");
            newMultiplierTexts[i] = UTF8ReadLine();
        }

        Array.Resize(ref supportedLanguages, supportedLanguages.Length + 1);
        supportedLanguages[^1] = languageName;

        unitsArrayTranslations.Add(languageName, newUnitsArray);
        tensArrayTranslations.Add(languageName, newTensArray);
        multiplierTextsTranslations.Add(languageName, newMultiplierTexts);

        Console.WriteLine("[+] Added language successfully.");
    }

    public static string UTF8ReadLine(){
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        Console.OutputEncoding = Encoding.GetEncoding(encodingFormat);
        Console.InputEncoding = Encoding.GetEncoding(encodingFormat);

        StringBuilder sb = new();
        sb.Append(Console.ReadLine());

        Console.WriteLine();
        return sb.ToString();
    }

    public override string ToString()
    {  
        // Allows UTF8 chars in the Program Console.
        Console.OutputEncoding = Encoding.GetEncoding(encodingFormat);
        Console.InputEncoding = Encoding.GetEncoding(encodingFormat);

        return ConvertToString(this.number);
    }
}