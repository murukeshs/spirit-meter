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
    public class Route
    {
        #region createRoute
        public static DataSet createRoute([FromBody] createRoute createRoute )
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@routeName", createRoute.routeName));
                parameters.Add(new SqlParameter("@comments", createRoute.comments));
                parameters.Add(new SqlParameter("@userId", createRoute.designatedCharityId));
                parameters.Add(new SqlParameter("@isPrivate", createRoute.isPrivate));

                using (DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spcreateRoute", parameters.ToArray()))
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

        #region updateRoute
        public static DataTable updateRoute([FromBody]createRoute updateRoute)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@routeId", updateRoute.routeId));
                parameters.Add(new SqlParameter("@routeName", updateRoute.routeName));
                parameters.Add(new SqlParameter("@comments", updateRoute.comments));
                parameters.Add(new SqlParameter("@designatedCharityId", updateRoute.designatedCharityId));
                parameters.Add(new SqlParameter("@startingPoint", updateRoute.startingPoint));
                parameters.Add(new SqlParameter("@isPrivate", updateRoute.isPrivate));

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spupdateRoute", parameters.ToArray()).Tables[0])
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

        #region selectcoordinates
        public static DataTable selectcoordinates([FromBody] routePoints routePoints)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@displayId", routePoints.displayId));
                parameters.Add(new SqlParameter("@startingPoint", routePoints.startingPoint));

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spselectcoordinates", parameters.ToArray()).Tables[0])
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

        #region saveRoutePoints
        public static DataTable saveRoutePoints([FromBody] routePoints routePoints,string path, string routePoint)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@routeId", routePoints.routeId));
                parameters.Add(new SqlParameter("@displayId", routePoints.displayId));
                parameters.Add(new SqlParameter("@path", path));
                parameters.Add(new SqlParameter("@startingPoint", routePoints.startingPoint));
                parameters.Add(new SqlParameter("@mapRequest", routePoint));
                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spsaveRoutePoints", parameters.ToArray()).Tables[0])
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

        #region deleteRoute
        public static DataTable deleteRoute(int routeId)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@routeId", routeId));

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spdeleteRoute", parameters.ToArray()).Tables[0])
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

        #region saveRoutePsaveRoutePointStatusoints
        public static DataTable saveRoutePointStatus([FromBody] routePointStatus routePointStatus)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@routePointId", routePointStatus.routePointId));
                parameters.Add(new SqlParameter("@userId", routePointStatus.userId));

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spsaveRoutePointStatus", parameters.ToArray()).Tables[0])
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

        #region saveRouteStatus
        public static DataSet saveRouteStatus([FromBody]rideStatus rideStatus)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@routeId", rideStatus.routeId));
                parameters.Add(new SqlParameter("@userId", rideStatus.userId));
                parameters.Add(new SqlParameter("@status", rideStatus.status));

                using (DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spsaveRouteStatus", parameters.ToArray()))
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

        #region listRoutes
        public static DataTable listRoutes(string Search)
        {
            try
            {
                if (Search == "" || Search == null)
                {
                    Search = "";
                }
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@Search", Search));


                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "splistRoutes", parameters.ToArray()).Tables[0])
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
        #region listRoutesByUserId
        public static DataTable listRoutesByUserId(int userId)
        {
            try
            {
                
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@userId", userId));


                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "splistRoutesByUserId", parameters.ToArray()).Tables[0])
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
        #region selectRouteById
        public static DataSet selectRouteById(int routeId)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@routeId", routeId));


                using (DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spselectRouteById", parameters.ToArray()))
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
        #region selectRouteByUserId
        public static DataTable selectRouteByUserId(int userId)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@userId", userId));


                using (DataTable ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spGetRouteByUserId", parameters.ToArray()).Tables[0])
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
