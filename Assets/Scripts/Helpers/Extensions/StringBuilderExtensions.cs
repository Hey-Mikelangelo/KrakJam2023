using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public static class StringExtensions
{
    public static int IndexOf(this string stringValue, string sequence)
    {
        int length = stringValue.Length;
        bool isMatch = false;
        int firstMatchIndex = -1;
        int searchedCharIndex = 0;
        for (int i = 0; i < length; i++)
        {
            if (stringValue[i] == sequence[searchedCharIndex])
            {
                if (searchedCharIndex == sequence.Length - 1)
                {
                    break;
                }
                if (isMatch == false)
                {
                    isMatch = true;
                    firstMatchIndex = i;
                }
                searchedCharIndex++;
            }
            else
            {
                isMatch = false;
                firstMatchIndex = -1;
                searchedCharIndex = 0;
            }
        }
        return firstMatchIndex;
    }

    public static int IndexOf(this string stringValue, char character)
    {
        int length = stringValue.Length;
        for (int i = 0; i < length; i++)
        {
            if (stringValue[i] == character)
            {
                return i;
            }
        }
        return -1;
    }


}

public static class StringBuilderExtensions
{
    public static StringBuilder Tab(this StringBuilder sb, int count = 1)
    {
        return AppendChar(sb, '\t', count);
    }

    public static StringBuilder Space(this StringBuilder sb, int count = 1)
    {
        return AppendChar(sb, ' ', count);
    }

    public static StringBuilder OpenBracket(this StringBuilder sb, int tabs)
    {
        return sb.NewTabbedLine(tabs - 1).Append('{').NewTabbedLine(tabs);
    }

    public static StringBuilder NewTabbedLine(this StringBuilder sb, int tabs)
    {
        return sb.Append('\n').Tab(tabs);
    }

    private static StringBuilder AppendChar(StringBuilder sb, char c, int count = 1)
    {
        count = Mathf.Clamp(count, 0, int.MaxValue);
        if (count == 1)
        {
            return sb.Append(c);
        }
        return sb.Append(new string(c, count));
    }

    public static int IndexOf(this StringBuilder sb, string sequence)
    {
        int length = sb.Length;
        bool isMatch = false;
        int firstMatchIndex = -1;
        int searchedCharIndex = 0;
        for (int i = 0; i < length; i++)
        {
            if (sb[i] == sequence[searchedCharIndex])
            {
                if (searchedCharIndex == sequence.Length - 1)
                {
                    break;
                }
                if (isMatch == false)
                {
                    isMatch = true;
                    firstMatchIndex = i;
                }
                searchedCharIndex++;
            }
            else
            {
                isMatch = false;
                firstMatchIndex = -1;
                searchedCharIndex = 0;
            }
        }
        return firstMatchIndex;
    }

    public static int IndexOf(this StringBuilder sb, char character, int searchStartIndex = 0)
    {
        int length = sb.Length;
        for (int i = searchStartIndex; i < length; i++)
        {
            if (sb[i] == character)
            {
                return i;
            }
        }
        return -1;
    }

    public static string UnionList(this string firstString, string secondString, char separator)
    {
        if (string.IsNullOrEmpty(secondString))
        {
            return firstString;
        }
        if (string.IsNullOrEmpty(firstString))
        {
            return secondString;
        }
        string[] fistList = firstString.Split(separator);
        string[] secondList = secondString.Split(separator);

        var unionList = fistList.Union(secondList);
        string unionString = string.Join(separator, unionList);
        return unionString;
    }

    public static StringBuilder RemoveSpansFromTo(this StringBuilder stringBuilder, char startSeparator, char endSeparator, bool removeSeparators = true)
    {
        int startCharIndex = stringBuilder.IndexOf(startSeparator);
        int endCharIndex = stringBuilder.IndexOf(endSeparator, startCharIndex + 1);
        while (startCharIndex != -1 && endCharIndex != -1)
        {
            int removeStartIndex = removeSeparators ? startCharIndex : Mathf.Clamp(startCharIndex + 1, 0, endCharIndex-1);
            int removeLength = removeSeparators ? (endCharIndex - removeStartIndex + 1) : endCharIndex - removeStartIndex;
            stringBuilder.Remove(removeStartIndex, removeLength);
            startCharIndex = stringBuilder.IndexOf(startSeparator, removeStartIndex);
            endCharIndex = stringBuilder.IndexOf(endSeparator, startCharIndex + 1);
        }
        return stringBuilder;
    }

    public static int CountOf(this StringBuilder stringBuilder, char countedChar)
    {
        int count = 0;
        int indexOfChar = stringBuilder.IndexOf(countedChar, 0);
        while(indexOfChar != -1)
        {
            count++;
            indexOfChar = stringBuilder.IndexOf(countedChar, indexOfChar + 1);
        }
        return count;
    }

    public static string AddSpacesToSentence(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";
        StringBuilder newText = new StringBuilder(text.Length * 2);
        newText.Append(text[0]);
        for (int i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                newText.Append(' ');
            newText.Append(text[i]);
        }
        return newText.ToString();
    }

}