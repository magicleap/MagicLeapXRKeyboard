using System;

namespace MagicLeap.XRKeyboard.Model
{

    /// <summary>
    /// Mapping obtained from https://github.com/microsoft/PowerToys/src/modules/poweraccent/PowerAccent.Core/Languages.cs
    /// </summary>
    public class AccentMap
    {
        public enum Language
        {
            ALL,
            CA,
            CUR,
            CY,
            CZ,
            GA,
            GD,
            DE,
            EST,
            FR,
            HR,
            HE,
            HU,
            IS,
            IT,
            KU,
            LT,
            MK,
            MI,
            NL,
            NO,
            PI,
            PL,
            PT,
            RO,
            SK,
            SP,
            SR,
            SV,
            TK,
        }

            public static string[] GetAndroidEnglishLetterKey(string letter)
            {
                return letter switch
                       {
                           "a" => new string[] { "@", "æ", "ã", "å", "ā", "à", "á", "â", "ä" },

                           "b" => new string[] { ";" },

                           "c" => new string[] { @"'", "ç" },

                           "d" => new string[] { "$" },

                           "e" => new string[] { "3", "ē", "è", "é", "ê", "ë" },

                           "f" => new string[] { "_" },

                           "g" => new string[] { "&" },

                           "h" => new string[] { "-" },

                           "i" => new string[] { "8", "í", "ì", "î", "î", "ï" },

                           "j" => new string[] { "+" },

                           "k" => new string[] { "(" },

                           "l" => new string[] { ")" },

                           "m" => new string[] { "?" },

                           "n" => new string[] { "!", "ñ" },

                           "o" => new string[] { "9", "œ", "õ", "ø", "о̄", "ò", "ó", "ô", "ö" },

                           "p" => new string[] { "0" },

                           "q" => new string[] { "1" },

                           "r" => new string[] { "4" },

                           "s" => new string[] { "#", "ß" },

                           "t" => new string[] { "5" },

                           "u" => new string[] { "7", "ū", "ù", "ú", "û", "ü", },

                           "v" => new string[] { ":" },

                           "w" => new string[] { "2" },

                           "x" => new string[] { "\"" },

                           "y" => new string[] { "6" },
                           "z" => new string[] { "*" },
                           _   => new string[] { }
                       };
            }
     
     
            public static string[] GetDefaultLetterKey(char letter, Language lang)
            {
                return lang switch
                       {
                           Language.ALL => GetDefaultLetterKeyALL(letter), // ALL
                           Language.CA  => GetDefaultLetterKeyCA(letter),  // Catalan
                           Language.CUR => GetDefaultLetterKeyCUR(letter), // Currency
                           Language.CY  => GetDefaultLetterKeyCY(letter),  // Welsh
                           Language.CZ  => GetDefaultLetterKeyCZ(letter),  // Czech
                           Language.GA  => GetDefaultLetterKeyGA(letter),  // Gaeilge (Irish Gaelic)
                           Language.GD  => GetDefaultLetterKeyGD(letter),  // Gàidhlig (Scottish Gaelic)
                           Language.DE  => GetDefaultLetterKeyDE(letter),  // German
                           Language.EST => GetDefaultLetterKeyEST(letter), // Estonian
                           Language.FR  => GetDefaultLetterKeyFR(letter),  // French
                           Language.HR  => GetDefaultLetterKeyHR(letter),  // Croatian
                           Language.HE  => GetDefaultLetterKeyHE(letter),  // Hebrew
                           Language.HU  => GetDefaultLetterKeyHU(letter),  // Hungarian
                           Language.IS  => GetDefaultLetterKeyIS(letter),  // Iceland
                           Language.IT  => GetDefaultLetterKeyIT(letter),  // Italian
                           Language.KU  => GetDefaultLetterKeyKU(letter),  // Kurdish
                           Language.LT  => GetDefaultLetterKeyLT(letter),  // Lithuanian
                           Language.MK  => GetDefaultLetterKeyMK(letter),  // Macedonian
                           Language.MI  => GetDefaultLetterKeyMI(letter),  // Maori
                           Language.NL  => GetDefaultLetterKeyNL(letter),  // Dutch
                           Language.NO  => GetDefaultLetterKeyNO(letter),  // Norwegian
                           Language.PI  => GetDefaultLetterKeyPI(letter),  // Pinyin
                           Language.PL  => GetDefaultLetterKeyPL(letter),  // Polish
                           Language.PT  => GetDefaultLetterKeyPT(letter),  // Portuguese
                           Language.RO  => GetDefaultLetterKeyRO(letter),  // Romanian
                           Language.SK  => GetDefaultLetterKeySK(letter),  // Slovak
                           Language.SP  => GetDefaultLetterKeySP(letter),  // Spain
                           Language.SR  => GetDefaultLetterKeySR(letter),  // Serbian
                           Language.SV  => GetDefaultLetterKeySV(letter),  // Swedish
                           Language.TK  => GetDefaultLetterKeyTK(letter),  // Turkish
                           _            => throw new ArgumentException("The language {0} is not know in this context", lang.ToString()),
                       };
            }

            // All
            private static string[] GetDefaultLetterKeyALL(char letter)
            {
                return letter switch
                       {
                           '0'      => new string[] { "₀", "⁰" },
                           '1'      => new string[] { "₁", "¹" },
                           '2'      => new string[] { "₂", "²" },
                           '3'      => new string[] { "₃", "³" },
                           '4'      => new string[] { "₄", "⁴" },
                           '5'      => new string[] { "₅", "⁵" },
                           '6'      => new string[] { "₆", "⁶" },
                           '7'      => new string[] { "₇", "⁷" },
                           '8'      => new string[] { "₈", "⁸" },
                           '9'      => new string[] { "₉", "⁹" },
                           'a'      => new string[] { "á", "à", "ä", "â", "ă", "å", "α", "ā", "ą", "ȧ", "ã", "æ" },
                           'B'      => new string[] { "ḃ", "β" },
                           'C'      => new string[] { "ç", "ć", "ĉ", "č", "ċ", "¢", "χ" },
                           'D'      => new string[] { "ď", "ḋ", "đ", "δ", "ð" },
                           'E'      => new string[] { "é", "è", "ê", "ë", "ě", "ē", "ę", "ė", "ε", "η", "€" },
                           'F'      => new string[] { "ƒ", "ḟ" },
                           'G'      => new string[] { "ğ", "ģ", "ǧ", "ġ", "ĝ", "ǥ", "γ" },
                           'H'      => new string[] { "ḣ", "ĥ", "ħ" },
                           'I'      => new string[] { "ï", "î", "í", "ì", "ī", "į", "i", "ı", "İ", "ι" },
                           'J'      => new string[] { "ĵ" },
                           'K'      => new string[] { "ķ", "ǩ", "κ" },
                           'L'      => new string[] { "ĺ", "ľ", "ļ", "ł", "₺", "λ" },
                           'M'      => new string[] { "ṁ", "μ" },
                           'N'      => new string[] { "ñ", "ń", "ŋ", "ň", "ņ", "ṅ", "ⁿ", "ν" },
                           'O'      => new string[] { "ô", "ó", "ö", "ő", "ò", "ō", "ȯ", "ø", "õ", "œ", "ω", "ο" },
                           'P'      => new string[] { "ṗ", "₽", "π", "φ", "ψ" },
                           'R'      => new string[] { "ŕ", "ř", "ṙ", "₹", "ρ" },
                           'S'      => new string[] { "ś", "ş", "š", "ș", "ṡ", "ŝ", "ß", "σ", "$" },
                           'T'      => new string[] { "ţ", "ť", "ț", "ṫ", "ŧ", "θ", "τ", "þ" },
                           'U'      => new string[] { "û", "ú", "ü", "ŭ", "ű", "ù", "ů", "ū", "ų", "υ" },
                           'W'      => new string[] { "ẇ", "ŵ", "₩" },
                           'X'      => new string[] { "ẋ", "ξ" },
                           'Y'      => new string[] { "ÿ", "ŷ", "ý", "ẏ" },
                           'Z'      => new string[] { "ź", "ž", "ż", "ʒ", "ǯ", "ζ" },
                           ','      => new string[] { "¿", "¡", "∙", "₋", "⁻", "–", "≤", "≥", "≠", "≈", "≙", "±", "₊", "⁺" },
                           '.' => new string[] { "\u0300", "\u0301", "\u0302", "\u0303", "\u0304", "\u0308", "\u030C" },
                           '-'  => new string[] { "~", "‐", "‑", "‒", "–", "—", "―", "⁓", "−", "⸺", "⸻" },
                           _        => Array.Empty<string>(),
                       };
            }

            // Currencies (source: https://www.eurochange.co.uk/travel-money/world-currency-abbreviations-symbols-and-codes-travel-money)
            private static string[] GetDefaultLetterKeyCUR(char letter)
            {
                return letter switch
                       {
                           'B' => new string[] { "฿", "в" },
                           'C' => new string[] { "¢", "₡", "č" },
                           'D' => new string[] { "₫" },
                           'E' => new string[] { "€" },
                           'F' => new string[] { "ƒ" },
                           'H' => new string[] { "₴" },
                           'K' => new string[] { "₭" },
                           'L' => new string[] { "ł" },
                           'N' => new string[] { "л" },
                           'M' => new string[] { "₼" },
                           'P' => new string[] { "£", "₽" },
                           'R' => new string[] { "₹", "៛", "﷼" },
                           'S' => new string[] { "$", "₪" },
                           'T' => new string[] { "₮", "₺" },
                           'W' => new string[] { "₩" },
                           'Y' => new string[] { "¥" },
                           'Z' => new string[] { "z" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Croatian
            private static string[] GetDefaultLetterKeyHR(char letter)
            {
                return letter switch
                       {
                           'C' => new string[] { "ć", "č" },
                           'D' => new string[] { "đ" },
                           'S' => new string[] { "š" },
                           'Z' => new string[] { "ž" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Estonian
            private static string[] GetDefaultLetterKeyEST(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "ä" },
                           'E' => new string[] { "€" },
                           'O' => new string[] { "ö", "õ" },
                           'U' => new string[] { "ü" },
                           'Z' => new string[] { "ž" },
                           'S' => new string[] { "š" },
                           _   => Array.Empty<string>(),
                       };
            }

            // French
            private static string[] GetDefaultLetterKeyFR(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "à", "â", "á", "ä", "ã", "æ" },
                           'C' => new string[] { "ç" },
                           'E' => new string[] { "é", "è", "ê", "ë", "€" },
                           'I' => new string[] { "î", "ï", "í", "ì" },
                           'O' => new string[] { "ô", "ö", "ó", "ò", "õ", "œ" },
                           'U' => new string[] { "û", "ù", "ü", "ú" },
                           'Y' => new string[] { "ÿ", "ý" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Iceland
            private static string[] GetDefaultLetterKeyIS(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "á", "æ" },
                           'D' => new string[] { "ð" },
                           'E' => new string[] { "é" },
                           'O' => new string[] { "ó", "ö" },
                           'U' => new string[] { "ú" },
                           'Y' => new string[] { "ý" },
                           'T' => new string[] { "þ" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Spain
            private static string[] GetDefaultLetterKeySP(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "á" },
                           'E' => new string[] { "é", "€" },
                           'I' => new string[] { "í" },
                           'N' => new string[] { "ñ" },
                           'O' => new string[] { "ó" },
                           'U' => new string[] { "ú", "ü" },
                           ',' => new string[] { "¿", "?" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Catalan
            private static string[] GetDefaultLetterKeyCA(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "à", "á" },
                           'C' => new string[] { "ç" },
                           'E' => new string[] { "è", "é", "€" },
                           'I' => new string[] { "ì", "í", "ï" },
                           'N' => new string[] { "ñ" },
                           'O' => new string[] { "ò", "ó" },
                           'U' => new string[] { "ù", "ú", "ü" },
                           'L' => new string[] { "·" },
                           ',' => new string[] { "¿", "?" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Maori
            private static string[] GetDefaultLetterKeyMI(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "ā" },
                           'E' => new string[] { "ē" },
                           'I' => new string[] { "ī" },
                           'O' => new string[] { "ō" },
                           'S' => new string[] { "$" },
                           'U' => new string[] { "ū" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Dutch
            private static string[] GetDefaultLetterKeyNL(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "á", "à", "ä" },
                           'C' => new string[] { "ç" },
                           'E' => new string[] { "é", "è", "ë", "ê", "€" },
                           'I' => new string[] { "í", "ï", "î" },
                           'N' => new string[] { "ñ" },
                           'O' => new string[] { "ó", "ö", "ô" },
                           'U' => new string[] { "ú", "ü", "û" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Pinyin
            private static string[] GetDefaultLetterKeyPI(char letter)
            {
                return letter switch
                       {
                           '1' => new string[] { "̄", "ˉ", "1" },
                           '2' => new string[] { "́", "ˊ", "2" },
                           '3' => new string[] { "̌", "ˇ", "3" },
                           '4' => new string[] { "̀", "ˋ", "4" },
                           '5' => new string[] { "˙", "5" },
                           'a' => new string[] { "ā", "á", "ǎ", "à", "a", "ɑ", "ɑ̄", "ɑ́", "ɑ̌", "ɑ̀" },
                           'C' => new string[] { "ĉ", "c" },
                           'E' => new string[] { "ē", "é", "ě", "è", "e", "ê", "ê̄", "ế", "ê̌", "ề" },
                           'I' => new string[] { "ī", "í", "ǐ", "ì", "i" },
                           'M' => new string[] { "m̄", "ḿ", "m̌", "m̀", "m" },
                           'N' => new string[] { "n̄", "ń", "ň", "ǹ", "n", "ŋ", "ŋ̄", "ŋ́", "ŋ̌", "ŋ̀" },
                           'O' => new string[] { "ō", "ó", "ǒ", "ò", "o" },
                           'S' => new string[] { "ŝ", "s" },
                           'U' => new string[] { "ū", "ú", "ǔ", "ù", "u", "ü", "ǖ", "ǘ", "ǚ", "ǜ" },
                           'V' => new string[] { "ǖ", "ǘ", "ǚ", "ǜ", "ü" },
                           'Y' => new string[] { "¥", "y" },
                           'Z' => new string[] { "ẑ", "z" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Turkish
            private static string[] GetDefaultLetterKeyTK(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "â" },
                           'C' => new string[] { "ç" },
                           'E' => new string[] { "ë", "€" },
                           'G' => new string[] { "ğ" },
                           'I' => new string[] { "ı", "İ", "î", },
                           'O' => new string[] { "ö", "ô" },
                           'S' => new string[] { "ş" },
                           'T' => new string[] { "₺" },
                           'U' => new string[] { "ü", "û" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Polish
            private static string[] GetDefaultLetterKeyPL(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "ą" },
                           'C' => new string[] { "ć" },
                           'E' => new string[] { "ę", "€" },
                           'L' => new string[] { "ł" },
                           'N' => new string[] { "ń" },
                           'O' => new string[] { "ó" },
                           'S' => new string[] { "ś" },
                           'Z' => new string[] { "ż", "ź" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Portuguese
            private static string[] GetDefaultLetterKeyPT(char letter)
            {
                return letter switch
                       {
                           '0' => new string[] { "₀", "⁰" },
                           '1' => new string[] { "₁", "¹" },
                           '2' => new string[] { "₂", "²" },
                           '3' => new string[] { "₃", "³" },
                           '4' => new string[] { "₄", "⁴" },
                           '5' => new string[] { "₅", "⁵" },
                           '6' => new string[] { "₆", "⁶" },
                           '7' => new string[] { "₇", "⁷" },
                           '8' => new string[] { "₈", "⁸" },
                           '9' => new string[] { "₉", "⁹" },
                           'a' => new string[] { "á", "à", "â", "ã" },
                           'C' => new string[] { "ç" },
                           'E' => new string[] { "é", "ê", "€" },
                           'I' => new string[] { "í" },
                           'O' => new string[] { "ô", "ó", "õ" },
                           'P' => new string[] { "π" },
                           'S' => new string[] { "$" },
                           'U' => new string[] { "ú" },
                           ',' => new string[] { "≤", "≥", "≠", "≈", "≙", "±", "₊", "⁺" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Slovak
            private static string[] GetDefaultLetterKeySK(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "á", "ä" },
                           'C' => new string[] { "č" },
                           'D' => new string[] { "ď" },
                           'E' => new string[] { "é", "€" },
                           'I' => new string[] { "í" },
                           'L' => new string[] { "ľ", "ĺ" },
                           'N' => new string[] { "ň" },
                           'O' => new string[] { "ó", "ô" },
                           'R' => new string[] { "ŕ" },
                           'S' => new string[] { "š" },
                           'T' => new string[] { "ť" },
                           'U' => new string[] { "ú" },
                           'Y' => new string[] { "ý" },
                           'Z' => new string[] { "ž" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Gaeilge (Irish language)
            private static string[] GetDefaultLetterKeyGA(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "á" },
                           'E' => new string[] { "é" },
                           'I' => new string[] { "í" },
                           'O' => new string[] { "ó" },
                           'U' => new string[] { "ú" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Gàidhlig (Scottish Gaelic)
            private static string[] GetDefaultLetterKeyGD(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "à" },
                           'E' => new string[] { "è" },
                           'I' => new string[] { "ì" },
                           'O' => new string[] { "ò" },
                           'U' => new string[] { "ù" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Czech
            private static string[] GetDefaultLetterKeyCZ(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "á" },
                           'C' => new string[] { "č" },
                           'D' => new string[] { "ď" },
                           'E' => new string[] { "ě", "é" },
                           'I' => new string[] { "í" },
                           'N' => new string[] { "ň" },
                           'O' => new string[] { "ó" },
                           'R' => new string[] { "ř" },
                           'S' => new string[] { "š" },
                           'T' => new string[] { "ť" },
                           'U' => new string[] { "ů", "ú" },
                           'Y' => new string[] { "ý" },
                           'Z' => new string[] { "ž" },
                           _   => Array.Empty<string>(),
                       };
            }

            // German
            private static string[] GetDefaultLetterKeyDE(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "ä" },
                           'E' => new string[] { "€" },
                           'O' => new string[] { "ö" },
                           'S' => new string[] { "ß" },
                           'U' => new string[] { "ü" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Hebrew
            private static string[] GetDefaultLetterKeyHE(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "שׂ", "שׁ", "ְ" },
                           'B' => new string[] { "׆" },
                           'E' => new string[] { "ָ", "ֳ", "ֻ" },
                           'G' => new string[] { "ױ" },
                           'H' => new string[] { "ײ", "ײַ", "ׯ", "ִ" },
                           'M' => new string[] { "ֵ" },
                           'P' => new string[] { "ַ", "ֲ" },
                           'S' => new string[] { "ּ" },
                           'T' => new string[] { "ﭏ" },
                           'U' => new string[] { "וֹ", "וּ", "װ", "ֹ" },
                           'X' => new string[] { "ֶ", "ֱ" },
                           'Y' => new string[] { "ױ" },
                           ',' => new string[] { "”", "’", "״", "׳" },
                           '.' => new string[] { "֫", "ֽ", "ֿ" },
                           '-' => new string[] { "–", "־" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Hungarian
            private static string[] GetDefaultLetterKeyHU(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "á" },
                           'E' => new string[] { "é" },
                           'I' => new string[] { "í" },
                           'O' => new string[] { "ó", "ő", "ö" },
                           'U' => new string[] { "ú", "ű", "ü" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Romanian
            private static string[] GetDefaultLetterKeyRO(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "ă", "â" },
                           'I' => new string[] { "î" },
                           'S' => new string[] { "ș" },
                           'T' => new string[] { "ț" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Italian
            private static string[] GetDefaultLetterKeyIT(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "à" },
                           'E' => new string[] { "è", "é", "€" },
                           'I' => new string[] { "ì", "í" },
                           'O' => new string[] { "ò", "ó" },
                           'U' => new string[] { "ù", "ú" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Kurdish
            private static string[] GetDefaultLetterKeyKU(char letter)
            {
                return letter switch
                       {
                           'C' => new string[] { "ç" },
                           'E' => new string[] { "ê", "€" },
                           'I' => new string[] { "î" },
                           'O' => new string[] { "ö", "ô" },
                           'L' => new string[] { "ł" },
                           'N' => new string[] { "ň" },
                           'R' => new string[] { "ř" },
                           'S' => new string[] { "ş" },
                           'U' => new string[] { "û", "ü" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Welsh
            private static string[] GetDefaultLetterKeyCY(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "â" },
                           'E' => new string[] { "ê" },
                           'I' => new string[] { "î" },
                           'O' => new string[] { "ô" },
                           'U' => new string[] { "û" },
                           'Y' => new string[] { "ŷ" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Swedish
            private static string[] GetDefaultLetterKeySV(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "å", "ä" },
                           'O' => new string[] { "ö" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Serbian
            private static string[] GetDefaultLetterKeySR(char letter)
            {
                return letter switch
                       {
                           'C' => new string[] { "ć", "č" },
                           'D' => new string[] { "đ" },
                           'S' => new string[] { "š" },
                           'Z' => new string[] { "ž" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Macedonian
            private static string[] GetDefaultLetterKeyMK(char letter)
            {
                return letter switch
                       {
                           'E' => new string[] { "ѐ" },
                           'I' => new string[] { "ѝ" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Norwegian
            private static string[] GetDefaultLetterKeyNO(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "å", "æ" },
                           'E' => new string[] { "€" },
                           'O' => new string[] { "ø" },
                           'S' => new string[] { "$" },
                           _   => Array.Empty<string>(),
                       };
            }

            // Lithuanian
            private static string[] GetDefaultLetterKeyLT(char letter)
            {
                return letter switch
                       {
                           'a' => new string[] { "ą" },
                           'C' => new string[] { "č" },
                           'E' => new string[] { "ę", "ė", "€" },
                           'I' => new string[] { "į" },
                           'S' => new string[] { "š" },
                           'U' => new string[] { "ų", "ū" },
                           'Z' => new string[] { "ž" },
                           _   => Array.Empty<string>(),
                       };
            }
        }

    }

