using Amazon.DynamoDBv2.DataModel;
namespace WeatherWise_api.Modal
{
    [DynamoDBTable("favorites-db")]
    public class Favorite
    {
        [DynamoDBHashKey("pk")]
        public string Email { get; set; }
        [DynamoDBRangeKey("sk")]
        public string CountryId { get; set; }
    }
}