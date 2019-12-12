using Microsoft.ApplicationBlocks.Data;
using SpiritMeter.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SpiritMeter.Data
{
    public class dbConnections
    {
        //<-----------------Pass Parameters In This Way--------->

        //var myList = new List<KeyValuePair<string, dynamic>>();
        //myList.Add(new KeyValuePair<string, dynamic>("@roundId", roundId));

        //DataSet ds = Data.dbConnections.GetDataSetByID("spSelectRoundById", myList);

        #region Get Data in DataTable
        public static DataTable GetDataTable(string procedureName)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                //Execute the query
                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, procedureName).Tables[0])
                {
                    return dt;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        #endregion

        #region Get Data in Dataset
        public static DataSet GetDataSet(string procedureName)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                //Execute the query
                using (DataSet dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, procedureName))
                {
                    return dt;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        #endregion

        #region GetData By ID
        public static DataTable GetDataSetByID(string procedureName, List<SqlParameter> parameters)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                
                //Execute the query
                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, procedureName, parameters.ToArray()).Tables[0])
                {
                    return dt;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        #endregion

        #region Delete 
        public static string Delete(string procedureName, List<SqlParameter> parameters)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
               

                using (DataSet dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, procedureName, parameters.ToArray()))
                {
                    string rowsAffected = dt.Tables[0].Rows[0]["Status"].ToString();
                    return rowsAffected;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region Save data
        public static DataSet save(string procedureName, List<SqlParameter> parameters)
        {
            try
            {
                string connectionstring = Common.GetConnectionString();
                //List<SqlParameter> parameters = new List<SqlParameter>();
                //foreach (dynamic i in myList)
                //{
                //    parameters.Add(new SqlParameter(i.Key, i.Value));
                //}

                using (DataSet dt = SqlHelper.ExecuteDataset(connectionstring, CommandType.StoredProcedure, procedureName, parameters.ToArray()))
                {
                    return dt;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        //#region Update Database
        //public static string update(string procedureName, List<KeyValuePair<string, dynamic>> myList)
        //{
        //    try
        //    {
        //        string connectionstring = Common.GetConnectionString();
        //        List<SqlParameter> parameters = new List<SqlParameter>();
        //        foreach (dynamic i in myList)
        //        {
        //            parameters.Add(new SqlParameter(i.Key, i.Value));
        //        }

        //        string rowsAffected = SqlHelper.ExecuteScalar(connectionstring, CommandType.StoredProcedure, procedureName, parameters.ToArray()).ToString();
        //        return rowsAffected;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}
        //#endregion
    }
}
