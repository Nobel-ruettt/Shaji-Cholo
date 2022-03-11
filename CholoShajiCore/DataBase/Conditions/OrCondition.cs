using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace CholoShajiCore.DataBase.Conditions
{
    public class OrCondition : ICondition
    {
        public IList<FieldCondition> OrConditions { get; set; }
        public OrCondition()
        {
            OrConditions = new List<FieldCondition>();
        }
        public OrCondition(FieldCondition c1, FieldCondition c2)
        {
            OrConditions = new List<FieldCondition> { c1, c2 };
        }

        public void Add(FieldCondition condition)
        {
            OrConditions.Add(condition);
        }

        public dynamic GetMongoQuery()
        {
            dynamic obj = new ExpandoObject();
            var orList = new ArrayList();
            foreach (var condition in OrConditions)
            {
                orList.Add(condition.GetMongoQuery());
            }
            ((IDictionary<string, object>)obj).Add("$or", orList);
            return obj;
        }
    }
}
