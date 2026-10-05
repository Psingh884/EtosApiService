using EtosApiServices.Hotel.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.MongoDb.Library
{
    public class MongoDbCRUD
    {
        public static string MongoDbConnection = AppConfig.GetMongoDBConnectionString();
        public static string MongoDataBase = AppConfig.GetConfigValue("DataBase:NoSQL:MongoDB:MongoDataBase");
        private IMongoDatabase db;

        public MongoDbCRUD(string databasename)
        {
            var client = new MongoClient(MongoDbConnection);
            db = client.GetDatabase(databasename);
        }

        public void InsertRecord<T>(string table, T record)
        {
            var collection = db.GetCollection<T>(table);
            collection.InsertOneAsync(record);
        }

        public void InsertManyRecord<T>(string table, List<T> record)
        {
            var collection = db.GetCollection<T>(table);
            collection.InsertManyAsync(record);
        }

        public Task<List<T>> LoadHotelData<T>(string table, string FieldName, string FieldValues)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq(FieldName, FieldValues);
            var Data = collection.Find(filter).ToListAsync<T>();
            return Data;

        }

        public Task<List<T>> LoadHotelDataWithInHotelBeds<T>(string table, string FieldName, List<int> HotelCodes)
        {

            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.In(FieldName, HotelCodes);
            var data = collection.Find(filter).ToListAsync<T>();
            return data;

        }

        public Task<List<TBOModelClass.Tbov7HotelGiataHotelCodeList.Hotel>> GetTBOHotelInfoByLatLngV1(string table, double lat, double lng, double maxdistance)
        {
            return Task.Run(() =>
            {
                var Result = new List<TBOModelClass.Tbov7HotelGiataHotelCodeList.Hotel>();
                try
                {
                    var collection = db.GetCollection<BsonDocument>(table);

                    //Here we are forming the Geographical Coordinate point from where we have to do the search
                    BsonDocument geoPoint = new BsonDocument
                    {
                        {"type","Point"},
                        {"coordinates",new BsonArray(new double[]{ lng,lat})}
                    };

                    //Here we are forming the Geo Near Options that consists of Maximum Distance that we have to search till, number of records to be fetched, earth Spherical property to True, calculate the distance from starting point.
                    BsonDocument geoNearOptions = new BsonDocument
                    {
                        {"near", geoPoint},
                        {"distanceField", "Distance"},
                        {"maxDistance", maxdistance},
                        {"spherical", true},
                        {"$limit", 10000},

                    };

                    //MongoDB requires $geoNear as the first stage of a pipeline.
                    var stage = new BsonDocumentPipelineStageDefinition<BsonDocument, TBOModelClass.Tbov7HotelGiataHotelCodeList.Hotel>(new BsonDocument { { "$geoNear", geoNearOptions } });

                    Result = collection.Aggregate().AppendStage(stage).ToList();

                }
                catch (Exception ex)
                {

                }
                return Result;
            });
        }
    }
}
