using System.Runtime.CompilerServices;

namespace Aoc.Y2024.D2;

public class Solution
{
    public async Task SolveStep1()
    {
        var input1Path = "../../../Y2024/D2/input.txt";
        var safeLines = 0;
        var total = 0;
        foreach (var line in await File.ReadAllLinesAsync(input1Path))
        {
            total++;
            var lineNumbers = line.Split(' ').Select(int.Parse).ToList();
            if (!lineNumbers.SequenceEqual(lineNumbers.OrderBy(x => x)) && !lineNumbers.SequenceEqual(lineNumbers.OrderByDescending(x => x)))
            {
                continue;
            }
            var isSafe = true;
            for (int i = 0; i < lineNumbers.Count - 1; i++)
            {
                var diff = Math.Abs(lineNumbers[i] - lineNumbers[i + 1]);
                if (diff < 1 || diff > 3)
                {
                    isSafe = false;
                    break;
                }
            }
            if (isSafe)
            {
                safeLines++;
            }
        }

        Console.WriteLine($"Total: {total}, Safe lines: {safeLines}");
    }

    public record NumberPair(int Number1Index, int Number1, int Number2Index, int Number2)
    {
        public int Diff => (Number2 - Number1);
        public int DiffAbs => Math.Abs(Diff);

    }
    public async Task SolveStep2()
    {
        var input1Path = "../../../Y2024/D2/input.txt";
        var safeLines = 0;
        var total = 0;
        foreach (var line in await File.ReadAllLinesAsync(input1Path))
        {
            total++;
            var lineNumbers = line.Split(' ').Select(int.Parse).ToList();
            if (IsSafe(lineNumbers, true))
            {
                safeLines++;
            }
        }

        Console.WriteLine($"Total: {total}, Safe lines: {safeLines}");
    }

    public bool IsSafe(List<int> lineNumbers, bool tryRemoveAnomaly)
    {
        var lineNumbersCount = lineNumbers.Count;
        var linePairs = GetNumberPairs(lineNumbers);
        var lineDiffsCountPositives = linePairs.Count(x => x.Diff > 0);
        var lineDiffsCountNegatives = linePairs.Count(x => x.Diff < 0);

        var triedToRemoveAnomaly = false;
        var sequentialOrdered = false;
        if (lineDiffsCountPositives == lineNumbersCount - 1 || lineDiffsCountNegatives == lineNumbersCount - 1)
        {
            sequentialOrdered = true;
        }
        if (!sequentialOrdered && !tryRemoveAnomaly)
        {
            return false;
        }

        if (!sequentialOrdered && lineDiffsCountPositives == lineNumbersCount - 2)
        {
            triedToRemoveAnomaly = true;
            var badPair = linePairs.First(x => x.Diff <= 0);
            var newLineNumbers = TryRemoveAnomaly(lineNumbers, badPair);
            if (newLineNumbers == null)
            {
                return false;
            }
            sequentialOrdered = true;
            lineNumbers = newLineNumbers;
        }
        else if (!sequentialOrdered && lineDiffsCountNegatives == lineNumbersCount - 2)
        {
            triedToRemoveAnomaly = true;
            var badPair = linePairs.First(x => x.Diff >= 0);
            var newLineNumbers = TryRemoveAnomaly(lineNumbers, badPair);
            if (newLineNumbers == null)
            {
                return false;
            }
            sequentialOrdered = true;
            lineNumbers = newLineNumbers;
        }

        if (!sequentialOrdered)
        {
            return false;
        }

        if (triedToRemoveAnomaly)
        {
            linePairs = GetNumberPairs(lineNumbers);
        }

        var linePairsWithOutliers = linePairs.Where(x => x.DiffAbs > 3 || x.DiffAbs < 1).ToList();
        if (linePairsWithOutliers.Count == 0)
        {
            return true;
        }
        if (!tryRemoveAnomaly)
        {
            return false;
        }
        if (!triedToRemoveAnomaly)
        {
            triedToRemoveAnomaly = true;
            var badPair = linePairsWithOutliers.First();
            var newLineNumbers = TryRemoveAnomaly(lineNumbers, badPair);
            if (newLineNumbers == null)
            {
                return false;
            }
            lineNumbers = newLineNumbers;
            linePairs = GetNumberPairs(lineNumbers);
        }
        linePairsWithOutliers = linePairs.Where(x => x.DiffAbs > 3 || x.DiffAbs < 1).ToList();
        return (linePairsWithOutliers.Count == 0);
    }

    private List<NumberPair> GetNumberPairs(List<int> lineNumbers)
    {
        var linePairs = new List<NumberPair>();
        for (int i = 0; i < lineNumbers.Count - 1; i++)
        {
            linePairs.Add(new NumberPair(i, lineNumbers[i], i + 1, lineNumbers[i + 1]));
        }
        return linePairs;
    }

    private List<int>? TryRemoveAnomaly(List<int> lineNumbers, NumberPair badPair)
    {
        var newLineNumbers = lineNumbers.ToList();
        newLineNumbers.RemoveAt(badPair.Number1Index);
        if (IsSafe(newLineNumbers, false))
        {
            return newLineNumbers;
        }

        newLineNumbers = lineNumbers.ToList();
        newLineNumbers.RemoveAt(badPair.Number2Index);
        if (IsSafe(newLineNumbers, false))
        {
            return newLineNumbers;
        }

        return null;
    }
}
