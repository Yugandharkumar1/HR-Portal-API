using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace HRPortalWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HRPortalController : ControllerBase
    {
        DbContext dbContext = new DbContext();
        [HttpGet]
        public IActionResult Test()
        {
            return Ok("hello");
        }



        [HttpGet("region")]
        public IActionResult GetRegions()
        {
            string sql = "GetRegions";
            DataTable dtRegions = dbContext.ReturnTableOnQuery(sql);
            List<Regions> regions = dtRegions.AsEnumerable().Select(row => new Regions
            {
                region_id = row.Field<int>("region_id"),
                region_name = row.Field<string>("region_name")
            }).ToList();
            return Ok(regions);
        }

        [HttpGet("validateUser")]
        public IActionResult ValidateUser(string userName, string password)
        {
            try
            {
                string sql = "validateUser";
                SqlParameter[] parameters = new[]
                {
                    new SqlParameter("@userName", SqlDbType.VarChar){ Value = userName},
                    new SqlParameter("@password", SqlDbType.VarChar){ Value = password}
                };
                DataTable dtUsers = dbContext.ReturnTableOnQuery(sql, parameters);

                if (dtUsers.Rows.Count == 0)
                {
                    return Ok("User not found or invalid credentials.");
                }

                var users = dtUsers.AsEnumerable().Select(row => new Users
                {
                    userID = row.Field<int>("userID"),
                    userName = row.Field<string>("userName"),
                    isActive = row.Field<bool>("isActive"),
                    isAdmin = row.Field<bool>("isAdmin"),
                    fullName = row.Field<string>("fullName"),
                    dateOfJoining = row.Field<DateTime>("dateOfJoining")
                }).FirstOrDefault();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message.Trim());
            }
        }

        [HttpGet("GetUserDetailsByUserID")]
        public IActionResult GetUserDetailsByUserID(string userID)
        {
            try
            {
                string sql = "GetUserDetailsByUserID";

                SqlParameter[] parameters = new[]
                {
                    new SqlParameter("@userID", SqlDbType.Int){Value = userID}
                };

                DataTable dtUserDetails = dbContext.ReturnTableOnQuery(sql, parameters);

                UserDetails userDetails = dtUserDetails.AsEnumerable().Select(row => new UserDetails
                {
                    first_name = row.Field<string>("first_name"),
                    last_name = row.Field<string>("last_name"),
                    fullName = row.Field<string>("fullName"),
                    email = row.Field<string>("email"),
                    phone_number = row.Field<string>("phone_number"),
                    hire_date = row.Field<DateTime>("hire_date"),
                    dateOfJoining = row.Field<DateTime>("dateOfJoining"),
                    userID = row.Field<int>("userID"),
                    userName = row.Field<string>("userName"),
                    isActive = row.Field<bool>("isActive"),
                    isAdmin = row.Field<bool>("isAdmin"),

                    confirmDate = row.Field<DateTime>("confirmDate"),
                    country_id = row.Field<string>("country_id"),
                    country_name = row.Field<string>("country_name"),
                    region_id = row.Field<int>("region_id"),
                    region_name = row.Field<string>("region_name")
                }).FirstOrDefault();

                return Ok(userDetails);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message.ToString());
            }
        }

        [HttpPost("InsertUserDetails")]
        public IActionResult CreateUser(SignUpUserDetails signUpUserDetails)
        {
            string result = "";
            try
            {
                string sql = "InsertUserDetails";
                SqlParameter[] parameters = new[]
                {
                    new SqlParameter("@userName", SqlDbType.VarChar){ Value = signUpUserDetails.userName},
                    new SqlParameter("@password", SqlDbType.VarChar){ Value = signUpUserDetails.password},
                };

                dbContext.ExcuteStoreCommand(sql, parameters);

                return Ok(result);
            }
            catch (Exception ex)
            {
                result = ex.Message.Trim();
                return Ok(result);
            }
        }

        [HttpPost("UpdateOverView")]
        public IActionResult UpdateOverView(EmployeeOverview employeeOverview)
        {
            try
            {
                string sql = "UpdateEmployeeOverview";
                SqlParameter[] parameters = new[]
                {
                    new SqlParameter("@firstName", SqlDbType.VarChar){ Value = employeeOverview.firstName},
                    new SqlParameter("@lastName", SqlDbType.VarChar){ Value = employeeOverview.lastName},
                    new SqlParameter("@userID", SqlDbType.Int){ Value = employeeOverview.userID},
                };

                dbContext.ExcuteStoreCommand(sql, parameters);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return Ok(ex.Message.ToString());
            }
        }

        [HttpPost("UpdateEmployeeAddressAndContact")]
        public IActionResult UpdateEmployeeAddressAndContact(EmployeeAddressAndContact employeeAddressAndContact)
        {
            string result = "";
            try
            {
                string sql = "UpdateEmoplyeeAddressAndContact";
                SqlParameter[] parameters = new[]
                {
                    new SqlParameter("@phoneNumber", SqlDbType.VarChar){ Value = employeeAddressAndContact.phoneNumber},
                    new SqlParameter("@email", SqlDbType.VarChar) {Value = employeeAddressAndContact.email},
                    new SqlParameter("@userID", SqlDbType.Int){Value = employeeAddressAndContact.userID}
                };

                dbContext.ExcuteStoreCommand(sql, parameters);

                result = "Success";
            }
            catch (Exception ex)
            {
                result = ex.Message.ToString();
            }
            return Ok(result);
        }


        [HttpGet("GetleavesDetailsByUsersID")]
        public IActionResult GetleavesDetailsByUsersID(int userID)
        {
            try
            {
                string sql = "GetLeavesByUserID";
                SqlParameter[] parameters = new[]
                {
                    new SqlParameter("@userID", SqlDbType.Int){ Value = userID}
                };

                DataTable dtLeavesDetails = dbContext.ReturnTableOnQuery(sql, parameters);

                var leaveDetails = dtLeavesDetails.AsEnumerable().Select(row => new Leaves
                {
                    userID = row.Field<int?>("userID") ?? 0,
                    leave_type_id = row.Field<int?>("leave_type_id") ?? 0,
                    leave_status_id = row.Field<int?>("leave_status_id") ?? 0,
                    approver_id = row.Field<int?>("approver_id") ?? 0,
                    submissiondate = row.Field<DateTime?>("submissiondate") ?? default(DateTime),
                    approvaldate = row.Field<DateTime?>("approvaldate") ?? default(DateTime),
                    user_comment = row.Field<string>("user_comment") ?? string.Empty,
                    approver_comment = row.Field<string>("approver_comment") ?? string.Empty,
                    leave_from = row.Field<DateTime?>("leave_from") ?? default(DateTime),
                    leave_to = row.Field<DateTime?>("leave_to") ?? default(DateTime),
                    leave_span = row.Field<int?>("leave_span") ?? 0,
                    leavesAllocated = row.Field<int?>("leavesAllocated") ?? 0
                });

                return Ok(leaveDetails);

            }
            catch(Exception ex)
            {
                return Ok(ex.Message.ToString());
            }

        }


        #region Models
        public class Regions
        {
            public int region_id { get; set; }
            public string region_name { get; set; }
        }

        public class Users
        {
            public int userID { get; set; }
            public string userName { get; set; }
            public bool isActive { get; set; }
            public bool isAdmin { get; set; }
            public string fullName { get; set; }
            public DateTime dateOfJoining { get; set; }
        }

        public class UserDetails : Users
        {
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string email { get; set; }
            public string phone_number { get; set; }
            public DateTime hire_date { get; set; }
            public DateTime confirmDate { get; set; }
            public string country_id { get; set; }
            public string country_name { get; set; }
            public int region_id { get; set; }
            public string region_name { get; set; }
            public int leavesAllocated { get; set; }
        }

        public class SignUpUserDetails
        {
            public string userName { get; set; }
            public string password { get; set; }
        }

        public class EmployeeOverview
        {
            public int userID { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
        }

        public class EmployeeAddressAndContact
        {
            public int userID { get; set; }
            public string phoneNumber { get; set; }
            public string email { get; set; }
        }

        public class Leaves : UserDetails
        {
            public int? userID { get; set; }
            public int? leave_type_id { get; set; }
            public int? leave_status_id { get; set; }
            public int? approver_id { get; set; }
            public DateTime? submissiondate { get; set; }
            public DateTime? approvaldate { get; set; }
            public string? approver_comment { get; set; }
            public string? user_comment { get; set; }
            public DateTime? leave_from { get; set; }
            public DateTime? leave_to { get; set; }
            public int? leave_span { get; set; }
        }
        #endregion
    }
}
