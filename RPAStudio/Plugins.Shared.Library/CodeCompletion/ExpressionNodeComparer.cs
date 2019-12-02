using System;
using System.Collections.Generic;

namespace Plugins.Shared.Library.CodeCompletion
{
    internal class ExpressionNodeComparer : IComparer<ExpressionNode>
    {
        public int Compare(ExpressionNode x, ExpressionNode y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
}