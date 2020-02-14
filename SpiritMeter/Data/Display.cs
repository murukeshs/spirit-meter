using Microsoft.ApplicationBlocks.Data;
using Microsoft.AspNetCore.Mvc;
using SpiritMeter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;


namespace SpiritMeter.Data
{
    public class Display
    {
        #region listCategory
        public static DataTable listCategory()
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spGetCategory").Tables[0])
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

        #region createDisplay
        public static DataSet createDisplay(createDisplay createDisplay)
        {
            try
            {

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@name", createDisplay.name));
                parameters.Add(new SqlParameter("@notes", createDisplay.notes));
                parameters.Add(new SqlParameter("@markerType", createDisplay.markerType));
                parameters.Add(new SqlParameter("@isPrivate", createDisplay.isPrivate));
                parameters.Add(new SqlParameter("@createdBy", createDisplay.createdBy));
                parameters.Add(new SqlParameter("@markerUrl", createDisplay.markerUrl));
                parameters.Add(new SqlParameter("@action", "add"));
                string ConnectionString = Common.GetConnectionString();

                using (DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSaveDisplay", parameters.ToArray()))
                {
                    return ds;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        #endregion

        #region createDisplayFiles
        public static int createDisplayFiles(createDisplayFiles createDisplayFiles)
        {
            try
            {

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@displayId", createDisplayFiles.displayId));
                parameters.Add(new SqlParameter("@filePath", createDisplayFiles.filePath));
                parameters.Add(new SqlParameter("@latitude", createDisplayFiles.latitude));
                parameters.Add(new SqlParameter("@longitude", createDisplayFiles.longitude));
                parameters.Add(new SqlParameter("@country", createDisplayFiles.country));
                parameters.Add(new SqlParameter("@state", createDisplayFiles.state));
                parameters.Add(new SqlParameter("@cityName", createDisplayFiles.cityName));
                parameters.Add(new SqlParameter("@address", createDisplayFiles.address));
                string ConnectionString = Common.GetConnectionString();

                int rowsAffected = SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "spSaveDisplayFiles", parameters.ToArray());

                return rowsAffected;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        #endregion
            
        #region updateDisplay
        public static int updateDisplay(updateDisplay createDisplay)
        {
            try
            {

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@displayId", createDisplay.displayId));
                parameters.Add(new SqlParameter("@name", createDisplay.name));
                parameters.Add(new SqlParameter("@notes", createDisplay.notes));
                parameters.Add(new SqlParameter("markerType", createDisplay.markerType));
                parameters.Add(new SqlParameter("@isPrivate", createDisplay.isPrivate));
                parameters.Add(new SqlParameter("@longitude", createDisplay.longitude));
                parameters.Add(new SqlParameter("@latitude", createDisplay.latitude));
                parameters.Add(new SqlParameter("@country", createDisplay.country));
                parameters.Add(new SqlParameter("@state", createDisplay.state));
                parameters.Add(new SqlParameter("@cityName", createDisplay.cityName));
                parameters.Add(new SqlParameter("@address", createDisplay.address));
                parameters.Add(new SqlParameter("@filePath", createDisplay.filePath));
                parameters.Add(new SqlParameter("@markerUrl", createDisplay.markerUrl));

                string ConnectionString = Common.GetConnectionString();

                int rowsAffected = SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "spSaveDisplay", parameters.ToArray());
                
                return rowsAffected;
               
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        #endregion

        #region deleteDisplayFiles
        public static int deleteDisplayFiles(int displayFileId)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@displayFileId", displayFileId));

                int rowsAffected = SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "spDeleteDisplayFiles", parameters.ToArray());

                return rowsAffected;
            }
            catch (Exception e)
            {
                //loggerErr.Error(e.Message + " - " + e.StackTrace);
                throw e;
            }
        }

        #endregion

        #region selectDisplay
        public static DataSet selectDisplay(int displayId)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@displayId", displayId));
                string ConnectionString = Common.GetConnectionString();

                using (DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSelectDeleteDisplayById", parameters.ToArray()))
                {
                    return ds;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        #endregion

        #region listDisplay
        public static DataTable listDisplay(string Search)
        {
            try
            {
                if (Search == "" || Search == null)
                {
                    Search = "";
                }
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@Search", Search));
                string ConnectionString = Common.GetConnectionString();

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spGetDisplay", parameters.ToArray()).Tables[0])
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

        #region deleteDisplay
        public static int deleteDisplay(int displayId)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@displayId", displayId));
                parameters.Add(new SqlParameter("@action", "delete"));

                int rowsAffected = SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "spSelectDeleteDisplayById", parameters.ToArray());

                return rowsAffected;
            }
            catch (Exception e)
            {
                //loggerErr.Error(e.Message + " - " + e.StackTrace);
                throw e;
            }
        }

        #endregion
        #region listDisplayByUserId
        public static DataTable listDisplayByUserId(int userId)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@userId", userId));
                string ConnectionString = Common.GetConnectionString();

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spGetDisplayByUserId", parameters.ToArray()).Tables[0])
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
        #region PopularDisplay
        public static DataTable PopularDisplay( int userId)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@userId", userId));
                string ConnectionString = Common.GetConnectionString();

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spPopularDisplayUser", parameters.ToArray()).Tables[0])
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

        #region createDisplayCharity
        public static DataTable createDisplayCharity([FromBody]CreatelDisplayCharity createDisplayCharity)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();

                //var encryptPassword = Common.EncryptData(createCharity.password);
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@displayId", createDisplayCharity.displayId));
                parameters.Add(new SqlParameter("@charityId", createDisplayCharity.charityId));
                parameters.Add(new SqlParameter("@action", "add"));

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSaveDisplayCharity", parameters.ToArray()).Tables[0])
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

        #region updateDisplayCharity
        public static DataTable updateDisplayCharity([FromBody]UpdateDisplayCharity updateDisplayCharity)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@displayCharityId", updateDisplayCharity.displayCharityId));
                parameters.Add(new SqlParameter("@displayId", updateDisplayCharity.displayId));
                parameters.Add(new SqlParameter("@charityId", updateDisplayCharity.charityId));
                parameters.Add(new SqlParameter("@action", null));

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSaveDisplayCharity", parameters.ToArray()).Tables[0])
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
