using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace CholoShajiCore.DataBase.Conditions
{
    public enum Operator
    {
        Equal = 0,
        NotEqual = 1,
        Any = 2,
        All = 3,
        Contains = 4,
        Between = 5,
        LessThen = 6,
        LessThenOrEqual = 7,
        GreaterThen = 8,
        GreaterThenOrEqual = 9,
        Exists = 10,
        NotExists = 11,
        None = 12,
        HasValue = 13,
        StartsWith = 14,
        EndsWith = 15,
        RelativeTo = 16
    }
    public class FieldCondition : ICondition
    {
        public Operator Operator { get; set; }
        public string FieldKey { get; set; }
        public object[] Values { get; set; }
        public FieldCondition(string fieldKey, Operator @operator, params object[] values)
        {
            Operator = @operator;
            FieldKey = fieldKey;
            Values = values;
        }
        public dynamic GetMongoQuery()
        {
            dynamic obj = new ExpandoObject();
            switch (Operator)
            {
                case Operator.Equal:
                    ((IDictionary<string, object>)obj).Add(FieldKey, Values == null ? null : ValueParse(Values[0]));
                    break;
                case Operator.NotEqual:
                    dynamic neObj = new ExpandoObject();
                    ((IDictionary<string, object>)neObj).Add("$ne", ValueParse(Values[0]));
                    ((IDictionary<string, object>)obj).Add(FieldKey, neObj);
                    break;
                case Operator.Any:
                    dynamic inObj = new ExpandoObject();
                    ((IDictionary<string, object>)inObj).Add("$in", ValueParse(Values));
                    ((IDictionary<string, object>)obj).Add(FieldKey, inObj);
                    break;
                case Operator.All:
                    dynamic allObj = new ExpandoObject();
                    ((IDictionary<string, object>)allObj).Add("$eq", ValueParse(Values));
                    ((IDictionary<string, object>)obj).Add(FieldKey, allObj);
                    break;
                case Operator.Contains:
                    dynamic containsObj = new ExpandoObject();
                    ((IDictionary<string, object>)containsObj).Add("$regex", $".*{ValueParse(Values[0])}.*");
                    ((IDictionary<string, object>)obj).Add(FieldKey, containsObj);

                    break;
                case Operator.StartsWith:
                    dynamic startsWithObj = new ExpandoObject();
                    ((IDictionary<string, object>)startsWithObj).Add("$regex", $"^{ValueParse(Values[0])}.*");
                    ((IDictionary<string, object>)obj).Add(FieldKey, startsWithObj);
                    break;
                case Operator.EndsWith:
                    dynamic endsWithObj = new ExpandoObject();
                    ((IDictionary<string, object>)endsWithObj).Add("$regex", $".*{ValueParse(Values[0])}^");
                    ((IDictionary<string, object>)obj).Add(FieldKey, endsWithObj);
                    break;
                case Operator.Between:
                    var betweenObj = new ExpandoObject();
                    ((IDictionary<string, object>)betweenObj).Add("$gte", ValueParse(Values[0]));
                    ((IDictionary<string, object>)betweenObj).Add("$lte", ValueParse(Values[1]));
                    ((IDictionary<string, object>)obj).Add(FieldKey, betweenObj);
                    break;
                case Operator.LessThen:
                    var lessThenObj = new ExpandoObject();
                    ((IDictionary<string, object>)lessThenObj).Add("$lt", ValueParse(Values[0]));
                    ((IDictionary<string, object>)obj).Add(FieldKey, lessThenObj);
                    break;
                case Operator.LessThenOrEqual:
                    var lessThenOrEqualObj = new ExpandoObject();
                    ((IDictionary<string, object>)lessThenOrEqualObj).Add("$lte", ValueParse(Values[0]));
                    ((IDictionary<string, object>)obj).Add(FieldKey, lessThenOrEqualObj);
                    break;
                case Operator.GreaterThen:
                    var greaterThenObj = new ExpandoObject();
                    ((IDictionary<string, object>)greaterThenObj).Add("$gt", ValueParse(Values[0]));
                    ((IDictionary<string, object>)obj).Add(FieldKey, greaterThenObj);
                    break;
                case Operator.GreaterThenOrEqual:
                    var greaterThenOrEqualObj = new ExpandoObject();
                    ((IDictionary<string, object>)greaterThenOrEqualObj).Add("$gte", ValueParse(Values[0]));
                    ((IDictionary<string, object>)obj).Add(FieldKey, greaterThenOrEqualObj);
                    break;
                case Operator.Exists:
                    var exist = new ExpandoObject();
                    ((IDictionary<string, object>)exist).Add("$exists", true);
                    ((IDictionary<string, object>)obj).Add(FieldKey, exist);
                    break;
                case Operator.NotExists:
                    var notExists = new ExpandoObject();
                    ((IDictionary<string, object>)notExists).Add("$exists", false);
                    ((IDictionary<string, object>)obj).Add(FieldKey, notExists);
                    break;
                case Operator.None:
                    var none = new ExpandoObject();
                    ((IDictionary<string, object>)none).Add("$nin", ValueParse(Values));
                    ((IDictionary<string, object>)obj).Add(FieldKey, none);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return obj;
        }
        public static object[] ValueParse(object[] values)
        {
            return values.Select(ValueParse).ToArray();
        }
        public static object ValueParse(object value)
        {
            if (!(value is DateTime)) return value;
            var utcDate = ((DateTime)value).ToUniversalTime();
            return utcDate;
        }
    }
}
