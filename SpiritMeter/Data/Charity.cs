using Microsoft.ApplicationBlocks.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static SpiritMeter.Models.CharityModel;

namespace SpiritMeter.Data
{
    public class Charity
    {
        #region createCharity
        public static DataTable createCharity([FromBody] CreateCharity createCharity)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();

                //var encryptPassword = Common.EncryptData(createCharity.password);
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@firstName", createCharity.firstName));
                parameters.Add(new SqlParameter("@lastName", createCharity.lastName));
                parameters.Add(new SqlParameter("@phoneNumber", createCharity.phoneNumber));
                parameters.Add(new SqlParameter("@password", null));
                parameters.Add(new SqlParameter("@email", createCharity.email));
                parameters.Add(new SqlParameter("@gender", createCharity.gender));
                parameters.Add(new SqlParameter("@profileImage", createCharity.profileImage));
                parameters.Add(new SqlParameter("@address", createCharity.address));
                parameters.Add(new SqlParameter("@action", "add"));

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSaveCharity", parameters.ToArray()).Tables[0])
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

        #region updateCharity
        public static DataTable updateCharity([FromBody]UpdateCharity updateCharity)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@charityId", updateCharity.charityId));
                parameters.Add(new SqlParameter("@firstName", updateCharity.firstName));
                parameters.Add(new SqlParameter("@lastName", updateCharity.lastName));
                parameters.Add(new SqlParameter("@phoneNumber", updateCharity.phoneNumber));
                parameters.Add(new SqlParameter("@email", updateCharity.email));
                parameters.Add(new SqlParameter("@gender", updateCharity.gender));
                parameters.Add(new SqlParameter("@profileImage", updateCharity.profileImage));
                parameters.Add(new SqlParameter("@address", updateCharity.address));
                parameters.Add(new SqlParameter("@action", null));

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSaveCharity", parameters.ToArray()).Tables[0])
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

        #region deleteCharity
        public static DataTable deleteCharity(int charityId)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@charityId", charityId));
                parameters.Add(new SqlParameter("@action", "delete"));

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSelectDeleteCharityById", parameters.ToArray()).Tables[0])
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

        #region listCharity
        public static DataTable listCharity(string Search)
        {
            try
            {

                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@Search", Search == null ? "" : Search));


                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spCharitylist", parameters.ToArray()).Tables[0])
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

        #region selectByCharityId
        public static DataTable selectByCharityId(int charityId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@charityId", charityId));
            try
            {
                string ConnectionString = Common.GetConnectionString();
                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSelectDeleteCharityById", parameters.ToArray()).Tables[0])
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
