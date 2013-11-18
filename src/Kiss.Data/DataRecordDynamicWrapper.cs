using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Data;

namespace Kiss.Data
{
    public class DataRecordDynamicWrapper : DynamicObject
    {
        private IDataRecord data;
        public DataRecordDynamicWrapper(IDataRecord record)
        {
            data = record;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = data[binder.Name];
            return result != null;
        }
    }
}
