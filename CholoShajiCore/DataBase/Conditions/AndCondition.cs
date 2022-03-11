using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace CholoShajiCore.DataBase.Conditions
{
    public class AndCondition : ICondition
    {
        public IList<FieldCondition> AndConditions { get; }
        public AndCondition()
        {
            AndConditions = new List<FieldCondition>();
        }
        public AndCondition(FieldCondition c1, FieldCondition c2)
        {
            AndConditions = new List<FieldCondition> {c1, c2};
        }

        public void Add(FieldCondition condition)
        {
            AndConditions.Add(condition);
        }

        public dynamic GetMongoQuery()
        {
            dynamic obj = new ExpandoObject();
            var andList = new ArrayList();
            foreach (var condition in AndConditions)
            {
                andList.Add(condition.GetMongoQuery());
            }
            ((IDictionary<string, object>)obj).Add("$and", andList);
            return obj;
        }
    }
}
