using System.Collections;
using Tsundere.LTL;

namespace Tsundere.BA;

public class ElementarySet : IEnumerable<LtlNode>
{
    private HashSet<LtlNode> _set;

    public ElementarySet(HashSet<LtlNode> set)
    {
        _set = set;
    }

    public IEnumerator<LtlNode> GetEnumerator()
    {
        return _set.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}