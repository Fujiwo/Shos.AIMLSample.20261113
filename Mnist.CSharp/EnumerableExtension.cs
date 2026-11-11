// 汎用拡張メソッド
using System.Collections.Generic;

namespace Mnist.CSharp;

static class EnumerableExtension
{
    extension(IEnumerable<float> source)
    {
        public int MaximumIndex
        {
            get {
                var maximum = float.MinValue;
                var maximumIndex = -1;
                var index = 0;
                foreach (var element in source) {
                    if (element > maximum) {
                        maximum = element;
                        maximumIndex = index;
                    }

                    index++;
                }

                return maximumIndex;
            }
        }
    }
}
