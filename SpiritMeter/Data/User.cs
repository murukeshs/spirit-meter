using Microsoft.ApplicationBlocks.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SpiritMeter.Models;
using HolidayApp.Models;

namespace SpiritMeter.Data
{
    public class User
    {
        #region   login
        public static DataSet login([FromBody]Login userlogin)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                var encryptPassword = Common.EncryptData(userlogin.password);
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@Phone", userlogin.phoneNo));
                parameters.Add(new SqlParameter("@Password", encryptPassword));

                DataSet ds = new DataSet();
                using (ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spLogin", parameters.ToArray()))
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

        #region createUser
        public static DataTable createUser([FromBody] createUser createUser)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();

                var encryptPassword = Common.EncryptData(createUser.password);
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@userId", createUser.userId));
                parameters.Add(new SqlParameter("@firstName", createUser.firstName));
                parameters.Add(new SqlParameter("@lastName", createUser.lastName));
                parameters.Add(new SqlParameter("@phoneNumber", createUser.phoneNumber));
                parameters.Add(new SqlParameter("@profileImage", createUser.profileImage));
                parameters.Add(new SqlParameter("@gender", createUser.gender));
                parameters.Add(new SqlParameter("@role", createUser.role));
                parameters.Add(new SqlParameter("@latitude", createUser.latitude));
                parameters.Add(new SqlParameter("@longitude", createUser.longitude));
                parameters.Add(new SqlParameter("@country", createUser.country));
                parameters.Add(new SqlParameter("@state", createUser.state));
                parameters.Add(new SqlParameter("@cityName", createUser.cityName));
                parameters.Add(new SqlParameter("@address", createUser.address));
                parameters.Add(new SqlParameter("@action", "add"));
                parameters.Add(new SqlParameter("@password", encryptPassword));

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spsaveuser", parameters.ToArray()).Tables[0])
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

        #region updateUser
        public static DataTable updateUser([FromBody]createUser updateUser)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@userId", updateUser.userId));
                parameters.Add(new SqlParameter("@firstName", updateUser.firstName));
                parameters.Add(new SqlParameter("@lastName", updateUser.lastName));
                parameters.Add(new SqlParameter("@phoneNumber", updateUser.phoneNumber));
                parameters.Add(new SqlParameter("@profileImage", updateUser.profileImage));
                parameters.Add(new SqlParameter("@gender", updateUser.gender));
                parameters.Add(new SqlParameter("@role", updateUser.role));
                parameters.Add(new SqlParameter("@latitude", updateUser.latitude));
                parameters.Add(new SqlParameter("@longitude", updateUser.longitude));
                parameters.Add(new SqlParameter("@country", updateUser.country));
                parameters.Add(new SqlParameter("@state", updateUser.state));
                parameters.Add(new SqlParameter("@cityName", updateUser.cityName));
                parameters.Add(new SqlParameter("@address", updateUser.address));
                parameters.Add(new SqlParameter("@action", ""));
                parameters.Add(new SqlParameter("@password", ""));

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spsaveuser", parameters.ToArray()).Tables[0])
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

        #region deleteUser
        public static DataTable deleteUser(int UserID)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@userId", UserID));
                parameters.Add(new SqlParameter("@action", "delete"));

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSelectDeleteUserById", parameters.ToArray()).Tables[0])
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

        #region listUser
        public static DataTable listUser(string Search)
        {
            try
            {
               
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@Search", Search == null ? "" : Search));


                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spGetUsers", parameters.ToArray()).Tables[0])
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
        #region ListCharity
        public static DataTable ListCharity()
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();


                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spListCharity").Tables[0])
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

        #region selectUserById
        public static DataTable selectUserById(int UserID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@userID", UserID));
            try
            {
                string ConnectionString = Common.GetConnectionString();
                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSelectDeleteUserById", parameters.ToArray()).Tables[0])
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

        #region GenerateOTP
        public static string GenerateOTP(string OTPValue, [FromBody]GenerateOTP otp)
        {

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@OTPValue", OTPValue));
            parameters.Add(new SqlParameter("@phone", otp.phone));
            parameters.Add(new SqlParameter("@otpType", otp.otpType));
            parameters.Add(new SqlParameter("@role", otp.role));
            try
            {
                string ConnectionString = Common.GetConnectionString();

                string rowsAffected = SqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, "spGenerateOTP", parameters.ToArray()).ToString();
                return rowsAffected;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        #endregion

        #region forgotPassword
        public static string forgotPassword([FromBody]forgotPassword forgotPassword)
        {
            var encryptPassword = Common.EncryptData(forgotPassword.password);

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@OTPValue", forgotPassword.OTPValue));
            parameters.Add(new SqlParameter("@phone", forgotPassword.phone));
            parameters.Add(new SqlParameter("@password", encryptPassword));

            try
            {
                string ConnectionString = Common.GetConnectionString();

                string rowsAffected = SqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, "spForgetPassword", parameters.ToArray()).ToString();
                return rowsAffected;
            }
            catch (Exception e)
            {

                throw e;
            }

        }
        #endregion
        #region forgotPassword
        public static string phoneVerify([FromBody]phoneVerify phoneVerify)
        {

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@OTPValue", phoneVerify.OTPValue));
            parameters.Add(new SqlParameter("@phoneNumber", phoneVerify.phone));

            try
            {
                string ConnectionString = Common.GetConnectionString();

                string rowsAffected = SqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, "spPhoneVerify", parameters.ToArray()).ToString();
                return rowsAffected;
            }
            catch (Exception e)
            {

                throw e;
            }

        }
        #endregion

        #region spiritMeter
        public static DataTable spiritMeter()
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();


                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSpiritMeter").Tables[0])
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

        #region selectBasicUserById
        public static DataSet selectBasicUserById(int userId)
        {
            

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@userId", userId));
            try
            {
                string ConnectionString = Common.GetConnectionString();

                using (DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spGetUserByUserId", parameters.ToArray()))
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


        #region getProfile
        public static DataSet getProfile(int userId)
        {


            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@userId", userId));
            try
            {
                string ConnectionString = Common.GetConnectionString();

                using (DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spGetProfile", parameters.ToArray()))
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
        #region dashBoardData
        public static DataSet dashBoardData()
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();

                using (DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spDashBoard"))
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

        #region CreateUserDeviceDetails
        public static DataTable CreateUserDeviceDetails([FromBody] UserDevice userDevice)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@userId", userDevice.userId));
                parameters.Add(new SqlParameter("@firebaseRegID", userDevice.firebaseRegID));


                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSaveUserDeviceDetails", parameters.ToArray()).Tables[0])
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
         
        #region getProfile
        public static DataSet GetRouteNotification(RouteNotification routeNotification)
        {


            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@userId", routeNotification.userId));
            parameters.Add(new SqlParameter("@routeId", routeNotification.routeId));
            parameters.Add(new SqlParameter("@lat", routeNotification.latitude));
            parameters.Add(new SqlParameter("@long", routeNotification.longitude));
            try
            {
                string ConnectionString = Common.GetConnectionString();

                using (DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSendRouteNotification", parameters.ToArray()))
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
        #region getProfile
        public static DataSet SaveUserNotification(int userId, int routeId, int displayId)
        {


            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@userId", userId));
            parameters.Add(new SqlParameter("@routeId", routeId));
            parameters.Add(new SqlParameter("@displayId", displayId));
            try
            {
                string ConnectionString = Common.GetConnectionString();

                using (DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "saveUserNotification", parameters.ToArray()))
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
    }

}
