using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NetP3DLib.Comparer;
class NaturalComparer : IComparer<string>
{
    private readonly StringComparison _comparison;
    private static readonly Regex _regex = new(@"\d+", RegexOptions.Compiled);

    public NaturalComparer(bool caseInsensitive)
    {
        _comparison = caseInsensitive
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
    }

    public int Compare(string x, string y)
    {
        if (x == null) return y == null ? 0 : -1;
        if (y == null) return 1;

        var xParts = _regex.Split(x);
        var yParts = _regex.Split(y);
        var xNums = _regex.Matches(x);
        var yNums = _regex.Matches(y);

        int i = 0;
        for (; i < Math.Min(xParts.Length, yParts.Length); i++)
        {
            int cmp = string.Compare(xParts[i], yParts[i], _comparison);
            if (cmp != 0)
                return cmp;

            if (i < xNums.Count && i < yNums.Count)
            {
                int xNum = int.Parse(xNums[i].Value);
                int yNum = int.Parse(yNums[i].Value);
                if (xNum != yNum)
                    return xNum.CompareTo(yNum);
            }
        }

        return x.Length.CompareTo(y.Length);
    }
}