using Microsoft.ApplicationBlocks.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static SpiritMeter.Models.AdsModule;

namespace SpiritMeter.Data
{ 
    public class Ads
    {
        #region createAd
        public static DataTable createAd([FromBody] CreateAd createAd)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();

                //var encryptPassword = Common.EncryptData(createCharity.password);
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@name", createAd.name));
                parameters.Add(new SqlParameter("@description", createAd.description));
                parameters.Add(new SqlParameter("@navigatioURL", createAd.navigationURL));
                parameters.Add(new SqlParameter("@image", createAd.image));
                parameters.Add(new SqlParameter("@priority", createAd.priority));
                parameters.Add(new SqlParameter("@expiryDate", createAd.expiryDate));
                parameters.Add(new SqlParameter("@adStatus", createAd.adStatus));
                parameters.Add(new SqlParameter("@action", "add"));

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSaveAd", parameters.ToArray()).Tables[0])
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

        #region updateAd
        public static DataTable updateAd([FromBody]UpdateAd updateAd)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@adId", updateAd.adId));
                parameters.Add(new SqlParameter("@name", updateAd.name));
                parameters.Add(new SqlParameter("@description", updateAd.description));
                parameters.Add(new SqlParameter("@navigatioURL", updateAd.navigationURL));
                parameters.Add(new SqlParameter("@image", updateAd.image));
                parameters.Add(new SqlParameter("@priority", updateAd.priority));
                parameters.Add(new SqlParameter("@expiryDate", updateAd.expiryDate));
                parameters.Add(new SqlParameter("@adStatus", updateAd.adStatus));
                parameters.Add(new SqlParameter("@action", null));

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSaveAd", parameters.ToArray()).Tables[0])
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

        #region deleteAd
        public static DataTable deleteAd(int adId)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@adId", adId));
                parameters.Add(new SqlParameter("@action", "delete"));

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSelectDeleteAdById", parameters.ToArray()).Tables[0])
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

        #region listAd
        public static DataTable listAd(string Search)
        {
            try
            {

                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@Search", Search == null ? "" : Search));


                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spAdlist", parameters.ToArray()).Tables[0])
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

        #region selectAdById
        public static DataTable selectAdById(int adId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@adId", adId));
            try
            {
                string ConnectionString = Common.GetConnectionString();
                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSelectDeleteAdById", parameters.ToArray()).Tables[0])
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

        #region GetPublishedAds
        public static DataTable GetPublishedAds()
        {
           
            try
            {
                string ConnectionString = Common.GetConnectionString();
                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spGetPublishedAds").Tables[0])
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
    }
}
