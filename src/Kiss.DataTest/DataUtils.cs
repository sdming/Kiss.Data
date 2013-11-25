using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiss.DataTest.Entity;
using Kiss.Data;
using Kiss.Data.Entity;

namespace Kiss.DataTest
{
    public class DataUtils
    {
        public static int NextInt32()
        {
            return new Random(Guid.NewGuid().GetHashCode()).Next(1, 10000);
        }

        public static List<CEntity> BuildTestData(string dbName, int count)
        {
            if(count <= 0)
            {
                count = 1;
            }

            List<CEntity> list = new List<CEntity>();

            using (DbContent db = new DbContent(dbName))
            {
                ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);
                ae.Delete(null);

                for (var i = 0; i < count; i++)
                {
                    var data = CEntity.NewEntity();
                    var key = ae.Add(data);
                    data.PK = Convert.ToInt32(key);
                    list.Add(data);
                }
            }
            return list;
        }
    }
}
