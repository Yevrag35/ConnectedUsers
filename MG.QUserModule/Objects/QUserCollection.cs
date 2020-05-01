using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MG.QUserModule
{
    public class QUserCollection : IReadOnlyList<IQUserObject>
    {
        private List<IQUserObject> _list;

        public IQUserObject this[int index] => _list[index];

        public int Count => _list.Count;

        public QUserCollection(IEnumerable<IQUserObject> userObjects)
        {
            _list = new List<IQUserObject>(userObjects);
        }

        public IEnumerator<IQUserObject> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }
}
