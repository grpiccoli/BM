using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace BiblioMit.Models
{
    public static class StringManipulations
    {
        private readonly static List<string> romanNumerals = 
            new List<string> { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
        private readonly static List<int> numerals = 
            new List<int> { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };

        public static string CheckSRI(string local, Uri url)
        {
            var path = Directory.GetCurrentDirectory();
            var file = Path.Combine(path, "wwwroot", local);
            using (FileStream fileStream = File.OpenRead(file))
            using (var sha = SHA384.Create())
            {
                var localHash = sha.ComputeHash(fileStream);
                var req = WebRequest.Create(url);
                Stream urlStream = req.GetResponse().GetResponseStream();
                var urlHash = sha.ComputeHash(urlStream);
                if (urlHash == localHash) return Convert.ToBase64String(localHash);
                //Console.WriteLine("Error: {0}", $"local and source hash differ in file {local}");
                return null;
            }
        }
        public static string HtmlToPlainText(this string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);
            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);
            text = Regex.Replace(text, @"<[^>]+>|&nbsp;", "").Trim();
            text = Regex.Replace(text, @"\s{2,}", " ");
            text = Regex.Replace(text, @">", "");
            return text;
        }

        public static string GetDigit(this int Id)
        {
            int Digito;
            int Contador;
            int Multiplo;
            int Acumulador;
            string RutDigito;

            Contador = 2;
            Acumulador = 0;

            while (Id != 0)
            {
                Multiplo = (Id % 10) * Contador;
                Acumulador += Multiplo;
                Id /= 10;
                Contador += 1;
                if (Contador == 8)
                {
                    Contador = 2;
                }
            }
            Digito = 11 - (Acumulador % 11);
            RutDigito = Digito.ToString(CultureInfo.InvariantCulture).Trim();
            if (Digito == 10)
            {
                RutDigito = "K";
            }
            if (Digito == 11)
            {
                RutDigito = "0";
            }
            return (RutDigito);
        }

        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (string.IsNullOrEmpty(value))
                //throw new ArgumentException("El texto a buscar no puede estar vacío", nameof(value));
                return null;
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index, StringComparison.InvariantCultureIgnoreCase);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }
        public static string ToRomanNumeral(this int number)
        {
            var romanNumeral = string.Empty;
            while (number > 0)
            {
                // find biggest numeral that is less than equal to number
                var index = numerals.FindIndex(x => x <= number);
                // subtract it's value from your number
                number -= numerals[index];
                // tack it onto the end of your roman numeral
                romanNumeral += romanNumerals[index];
            }
            return romanNumeral;
        }

        public static string GetSeason(this DateTime Date)
        {
            float value = (float)Date.Month + (float)Date.Day / 100;
            if (value >= 12.21 || value <= 3.20) return "Summer"; //summer
            if (value >= 3.21 && value <= 6.20) return "Autumn"; // autumn
            if (value >= 6.21 && value <= 9.20) return "Winter"; // winter
            if (value >= 9.21 && value <= 12.20) return "Spring";
            return "Date error";
        }

        public static string TranslateText(
            this string input,
            string targetLanguage,
            string languagePair)
        {
            //return Task.Run(() =>  
            //{
            //    string url = String.Format("http://www.google.com/translate_t?hl={0}&ie=UTF8&text={1}&langpair={2}", targetLanguage, input, languagePair);
            //    HttpClient hc = new HttpClient();
            //    HttpResponseMessage result = hc.GetAsync(url).Result;
            //    HtmlDocument doc = new HtmlDocument() { OptionReadEncoding = true };
            //    doc.Load(result.Content.ReadAsStreamAsync().Result);
            //    string resultado = "bla";
            //    try
            //    {
            //        resultado = HtmlToPlainText(doc.DocumentNode.SelectSingleNode("//span[@id='result_box']/span").InnerHtml);
            //    }
            //    catch { }
            //    return resultado;
            //}).Result;

            var url = new Uri(string.Format(CultureInfo.InvariantCulture,
                "http://www.google.com/translate_t?hl={0}&ie=UTF8&text={1}&langpair={2}", targetLanguage, input, languagePair));
            using (HttpClient hc = new HttpClient())
            {
                HttpResponseMessage result = hc.GetAsync(url).Result;
                HtmlDocument doc = new HtmlDocument() { OptionReadEncoding = true };
                doc.Load(result.Content.ReadAsStreamAsync().Result);
                string resultado = "bla";
                var node = doc.DocumentNode.SelectSingleNode("//span[@id='result_box']/span");
                if(node != null && !string.IsNullOrWhiteSpace(node.InnerHtml))
                {
                    resultado = HtmlToPlainText(node.InnerHtml);
                }
                return resultado;
            }
        }
    }
}
