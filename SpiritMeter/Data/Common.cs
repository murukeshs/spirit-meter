using Microsoft.ApplicationBlocks.Data;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpiritMeter.Data
{
    public class Common
    {
        #region DBCon
        public static IConfiguration configuration
        {
            get;
            private set;
        }
        public Common(IConfiguration iConfig)
        {
            configuration = iConfig;
        }
        public static string GetConnectionString()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));

            IConfigurationRoot configuration = builder.Build();
            var connstring = configuration.GetSection("ConnectionString").GetSection("DefaultConnection").Value;

            return connstring;
        }
        #endregion



        #region Encryption_Decryption
        public static string EncryptData(string textToEncrypt)
        {
            try
            {
                string ToReturn = "";
                //string _key = "ay$a5%&jwrtmnh;lasjdf98787";
                //string _iv = "abc@98797hjkas$&asd(*$%";
                IConfigurationBuilder builder = new ConfigurationBuilder();
                builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));
                IConfigurationRoot configuration = builder.Build();

                string _key = configuration.GetSection("EncryptDecrypt").GetSection("Key").Value;
                string _iv = configuration.GetSection("EncryptDecrypt").GetSection("iv").Value;

                byte[] _ivByte = { };
                _ivByte = System.Text.Encoding.UTF8.GetBytes(_iv.Substring(0, 8));
                byte[] _keybyte = { };
                _keybyte = System.Text.Encoding.UTF8.GetBytes(_key.Substring(0, 8));
                MemoryStream ms = null; CryptoStream cs = null;
                byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateEncryptor(_keybyte, _ivByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    ToReturn = Convert.ToBase64String(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ae)
            {
                throw new Exception(ae.Message, ae.InnerException);
            }
        }

        public static string DecryptData(string textToDecrypt)
        {
            try
            {
                string ToReturn = "";
                //string _key = "ay$a5%&jwrtmnh;lasjdf98787";
                //string _iv = "abc@98797hjkas$&asd(*$%";
                IConfigurationBuilder builder = new ConfigurationBuilder();
                builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));
                IConfigurationRoot configuration = builder.Build();

                string _key = configuration.GetSection("EncryptDecrypt").GetSection("Key").Value;
                string _iv = configuration.GetSection("EncryptDecrypt").GetSection("iv").Value;

                byte[] _ivByte = { };
                _ivByte = System.Text.Encoding.UTF8.GetBytes(_iv.Substring(0, 8));
                byte[] _keybyte = { };
                _keybyte = System.Text.Encoding.UTF8.GetBytes(_key.Substring(0, 8));
                MemoryStream ms = null; CryptoStream cs = null;
                byte[] inputbyteArray = new byte[textToDecrypt.Replace(" ", "+").Length];
                inputbyteArray = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateDecryptor(_keybyte, _ivByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    Encoding encoding = Encoding.UTF8;
                    ToReturn = encoding.GetString(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ae)
            {
                throw new Exception(ae.Message, ae.InnerException);
            }
        }
        #endregion


        #region ErrorLog
        public static string SaveErrorLog(string FunctionName, string ErrorMessage)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@FunctionName", FunctionName));
                parameters.Add(new SqlParameter("@ErrorMessage", ErrorMessage));

                string rowsAffected = SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "spSaveErrorLog", parameters.ToArray()).ToString();
                return rowsAffected;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region GenerateOTP
        public static string GenerateOTP()
        {
            try
            {
                Random generator = new Random();
                string OTPValue = generator.Next(0, 99999).ToString();
                return OTPValue;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion
        #region uploadFile       
        public static string CreateMediaItem(byte[] imageFile, string fileName)
        {
            try
            {

                string Exceptsymbols = Regex.Replace(fileName, @"[^.0-9a-zA-Z]+", "");
                string[] strFilename = Exceptsymbols.Split('.');

                string Filename = strFilename[0] + "_" + DateTime.Now.ToString("dd'-'MM'-'yyyy'-'HH'-'mm'-'ss") + "." + strFilename[1];

                var FileURL = AzureStorage.UploadImage(imageFile, Filename, "holidaydrive").Result; 
                return FileURL;
            }
            catch (Exception e)
            {
                
                throw e;
            }

        }
        #endregion
        #region apiKey
        public static string Apikey() 
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));

            IConfigurationRoot configuration = builder.Build();
            var apiKey = configuration.GetSection("APIKey").GetSection("Key").Value;

            return apiKey;
        }
        #endregion
        #region EventToken
        public static string EventToken()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));

            IConfigurationRoot configuration = builder.Build();
            var apiKey = configuration.GetSection("EventToken").GetSection("Key").Value;

            return apiKey;
        }
        #endregion

        public class UploadModel
        {
            public string file { get; set; }
            public string fileName { get; set; }
        }

        //For file upload
        public class FileUploadOperation : IOperationFilter
        {
            public void Apply(Operation operation, OperationFilterContext context)
            {
                if (operation.OperationId.ToLower() == "apivaluesuploadpost")
                {
                    operation.Parameters.Clear();
                    operation.Parameters.Add(new NonBodyParameter
                    {
                        Name = "uploadedFile",
                        In = "formData",
                        Description = "Upload File",
                        Required = true,
                        Type = "file"
                    });
                    operation.Consumes.Add("multipart/form-data");
                }
            }
        }
    }
}
