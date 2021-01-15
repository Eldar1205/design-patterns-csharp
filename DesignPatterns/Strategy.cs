using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DesignPatterns
{
    #region Bad sort with enums and switch-case
    class BadSort
    {
        public enum Kind
        {
            CaseSensitiveAscending,
            CaseSensitiveDescending,
            CaseInsensitiveAscending,
            CaseInsensitiveDescending,
        }

        public static void Sort(string[] arr, Kind sortKind)
        {
            // Bubble sort for simplicity
            Func<string, string, int> comparisonFunc = null;

            switch (sortKind)
            {
                case Kind.CaseSensitiveAscending:
                    comparisonFunc = (x, y) => string.Compare(x, y, StringComparison.Ordinal);
                    break;

                case Kind.CaseSensitiveDescending:
                    comparisonFunc = (x, y) => string.Compare(x, y, StringComparison.Ordinal) * (-1);
                    break;

                case Kind.CaseInsensitiveAscending:
                    comparisonFunc = (x, y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
                    break;

                case Kind.CaseInsensitiveDescending:
                    comparisonFunc = (x, y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase) * (-1);
                    break;
            }

            // Tomorrow we need a 5th option, and a 6th option... => we need to keep changing code in the same sort method and re-compile it

            for (int i = 0; i < arr.Length - 1; i++)
            {
                for (int j = 0; j < arr.Length - i - 1; j++)
                {
                    if (comparisonFunc(arr[j], arr[j + 1]) > 0)
                    {
                        var temp = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = temp;
                    }
                }
            }
        }
    }

    class BadSortApp
    {
        static void Main()
        {
            var arr = new[] { "Hello", "Nice", "World" };
            BadSort.Sort(arr, BadSort.Kind.CaseInsensitiveAscending);
            BadSort.Sort(arr, BadSort.Kind.CaseSensitiveDescending);
        }
    } 
    #endregion

    #region Good sort with Strategy pattern

    interface IStringComparer
    {
        /// <summary>
        /// Returns positive number if x bigger than y, negative number if x smaller than y, 0 if x equals y
        /// </summary>
        int Compare(string x, string y);
    }

    class GoodSort
    {
        public static void Sort(string[] arr, IStringComparer comparer)
        {
            // Bubble sort for simplicity
            for (int i = 0; i < arr.Length - 1; i++)
            {
                for (int j = 0; j < arr.Length - i - 1; j++)
                {
                    if (comparer.Compare(arr[j], arr[j + 1]) > 0)
                    {
                        var temp = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = temp;
                    }
                }
            }
        }
    }

    class CaseSensitiveStringComparer : IStringComparer
    {
        public int Compare(string x, string y) => string.Compare(x, y, StringComparison.Ordinal);
    }

    class CaseInsensitiveStringComparer : IStringComparer
    {
        public int Compare(string x, string y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
    }

    class ReverseStringComparerDecorator : IStringComparer
    {
        private readonly IStringComparer _innerComparer;

        public ReverseStringComparerDecorator(IStringComparer innerComparer) => _innerComparer = innerComparer;

        public int Compare(string x, string y) => _innerComparer.Compare(x, y) * (-1);
    }

    class GoodSortApp
    {
        static void Main()
        {
            var arr = new[] { "Hello", "Nice", "World" };
            GoodSort.Sort(arr, StringComparers.CaseInsensitive);
            GoodSort.Sort(arr, StringComparers.Reverse(StringComparers.CaseSensitive));
        }
    }

    class StringComparers
    {
        public static readonly IStringComparer CaseSensitive = new CaseSensitiveStringComparer();
        public static readonly IStringComparer CaseInsensitive = new CaseInsensitiveStringComparer();
        public static IStringComparer Reverse(IStringComparer comparer) => new ReverseStringComparerDecorator(comparer);
    }
    #endregion
}
