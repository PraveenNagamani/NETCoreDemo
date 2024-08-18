using Microsoft.EntityFrameworkCore;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc.Rendering;

public class DBConnect : DbContext
{
     public DBConnect(DbContextOptions<DBConnect> options ) : base(options)
     {
        
     }

     public DBConnect(){
        
     }

    public DbSet<Profile> Profiles{get; set;}
    public DataTable GetDatatable(string query, string connstring, ref string ErrMsg)
    {
       DataTable dt = new DataTable();
       OracleConnection conn = new OracleConnection(connstring);
       try
       {

           if (conn.State != ConnectionState.Open)
               conn.Open();

           OracleDataAdapter da = new OracleDataAdapter(query, conn);

           da.Fill(dt);
           if (conn.State == ConnectionState.Open)
               conn.Close();
       }
       catch (Exception e)
       {
           if (conn.State == ConnectionState.Open)
               conn.Close();
           ErrMsg += e.Message;
       }

       return dt;
   }

   public string GetNoSQLBson(string Connstring,string TableName, ref string ErrMsg,string[,] filterparams = null,string DBName = "local")
   {
        
       var filterBuilder = Builders<BsonDocument>.Filter;
        FilterDefinition<BsonDocument> combinedFilter = filterBuilder.Empty;

        for (int i = 0; i < filterparams.GetLength(0); i++)
        {
            var field = filterparams[i, 0];
            var op = filterparams[i, 1];
            var value = filterparams[i, 2];

            // Parse the value to the appropriate BSON type if necessary
            BsonValue bsonValue;
            if (int.TryParse(value, out int intValue))
            {
                bsonValue = intValue;
            }
            else if (bool.TryParse(value, out bool boolValue))
            {
                bsonValue = boolValue;
            }
            else
            {
                bsonValue = value;
            }

            FilterDefinition<BsonDocument> filter = op switch
            {
                "Eq" => filterBuilder.Eq(field, bsonValue),
                "Ne" => filterBuilder.Ne(field, bsonValue),
                "Gt" => filterBuilder.Gt(field, bsonValue),
                "Gte" => filterBuilder.Gte(field, bsonValue),
                "Lt" => filterBuilder.Lt(field, bsonValue),
                "Lte" => filterBuilder.Lte(field, bsonValue),
                _ => throw new ArgumentException($"Unknown operator: {op}")
            };

            combinedFilter = combinedFilter & filter;
        }

       
       if (Connstring == "")  Connstring = "mongodb://localhost:27017/";
       MongoClient conn = new MongoClient(Connstring);
       IMongoDatabase db = conn.GetDatabase(DBName);
       try
       {
           
           var resultlist = db.GetCollection<BsonDocument>(TableName);
           
           BsonDocument doc = resultlist.Find(combinedFilter).FirstOrDefault();
           if (doc != null)
                {
                    string bsonresult = doc.ToString();
                    return bsonresult;
                }
                else
                {
                    ErrMsg = "No document found";
                    return "";
                }     

       }
       catch (Exception e)
       {
        
           ErrMsg += e.Message;
       }

       return "";
   }


  

   public void BindDropDown(DataTable dt,ref List<SelectListItem> TargetList, ref String ErrMsg)
   {
   
       SelectListItem Autobounditem = new SelectListItem() { Text = "--Select--", Value = "" };
       TargetList.Add(Autobounditem);
       foreach (DataRow dr in dt.Rows)
       {

           SelectListItem country = new SelectListItem();
           country.Text = (string)  dr.ItemArray[0];
           country.Value = (string)dr.ItemArray[1];
           if (dt.Columns.Contains("defaultvalue"))
           {
               if ((String)dr.ItemArray[3] == "1")
               {
                   country.Selected = true;
               }
           }
           TargetList.Add(country);
       }

   }

   public void BindTextBox(DataTable dt,ref String BindValue, ref String ErrMsg)
   {
       BindValue = (String)dt.Rows[0][0];
   }
   public void BindTextBox(String SourceValue, ref String BindValue, ref String ErrMsg)
   {
       BindValue = SourceValue;
   }

}