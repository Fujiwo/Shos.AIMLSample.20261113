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
                // 推論結果のうち最大値の位置を返し、最も確からしい数字を選ぶ。
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
