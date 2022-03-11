namespace CholoShajiCore.DataBase.Conditions
{
    public interface ICondition
    {
        dynamic GetMongoQuery();
    }
}
