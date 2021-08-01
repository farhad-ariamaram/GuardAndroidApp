using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GuardAndroidApp.Utilities;
using System.Threading.Tasks;
using System;

namespace GuardAndroidApp.Models
{
    public class DbContext
    {
        private string dbPath = "";
        private string dbName = "GuardianDB";
        private SQLiteConnection _db;

        public DbContext()
        {
            dbPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            _db = new SQLiteConnection(Path.Combine(dbPath, dbName));
            _db.CreateTable<User>();
            _db.CreateTable<Location>();
            _db.CreateTable<SubmittedLocation>();
            _db.CreateTable<Plan>();
            _db.CreateTable<Check>();
            _db.CreateTable<Climate>();
            _db.CreateTable<LocationDetail>();
            _db.CreateTable<SubmittedLocationDtl>();
            _db.CreateTable<Login>();
            _db.CreateTable<AttendanceDetail>();
        }

        #region AttendanceDetail

        public void InsertAttendanceDetail(AttendanceDetail attendanceDetail)
        {
            _db.Insert(attendanceDetail);
        }

        public List<AttendanceDetail> GetAttendanceDetails(DateTime dateTime, long uid)
        {
            return _db.Table<AttendanceDetail>().Where(a => a.GuardId == uid && a.Date == dateTime).ToList();
        }

        #endregion

        #region Login
        public void AddLogin(Login login)
        {
            _db.Insert(login);
        }

        public void ClearLogin()
        {
            var loginList = _db.Table<Login>().ToList();
            foreach (var item in loginList)
            {
                _db.Delete(item);
            }
        }

        public Login GetLogin()
        {
            return _db.Table<Login>().FirstOrDefault();
        }

        #endregion

        #region User
        public void InsertUser(User user)
        {
            var euser = FindUser(user.Id);
            user.Password = Utils.sha512(user.Password + Utils._SALT);
            if (euser == null)
            {
                _db.Insert(user);
            }
        }

        public void UpdateUser(User user)
        {
            user.Password = Utils.sha512(user.Password + Utils._SALT);
            _db.Update(user);
        }

        public User FindUser(int userId)
        {
            return _db.Table<User>().SingleOrDefault(p => p.Id == userId);
        }

        public User LocalLogin(string username, string password)
        {
            var hashPassword = Utils.sha512(password + Utils._SALT);

            var user = _db.Table<User>().FirstOrDefault(p => p.Username == username && p.Password == hashPassword);

            return user;
        }

        public List<User> ListUser()
        {
            return _db.Table<User>().ToList();
        }
        #endregion

        #region Location
        public List<Location> GetLocationsList()
        {
            return _db.Table<Location>().ToList();
        }

        public long GetIdFromQR(string qr)
        {
            return _db.Table<Location>().SingleOrDefault(p => p.Qr == qr).Id;
        }

        public void InsertLocation(Location location)
        {
            var loc = GetLocationsList().Where(a => a.Id == location.Id);
            if (!loc.Any())
            {
                _db.Insert(location);
            }
        }

        public string GetLocationName(int id)
        {
            return _db.Table<Location>().SingleOrDefault(a => a.Id == id).Name;
        }
        #endregion

        #region SubmittedLocation
        public void InsertSubmittedLocation(SubmittedLocation SubmittedLocation)
        {
            _db.Insert(SubmittedLocation);
        }

        public void UpdateSubmittedLocation(SubmittedLocation SubmittedLocation)
        {
            _db.Update(SubmittedLocation);
        }

        public List<SubmittedLocation> SubmittedLocationsList()
        {
            return _db.Table<SubmittedLocation>().ToList();
        }

        public void IsSyncTrue(SubmittedLocation SubmittedLocation)
        {
            SubmittedLocation.IsSync = true;
            _db.Update(SubmittedLocation);
        }

        public bool StatusById(long id)
        {
            return _db.Table<SubmittedLocation>().FirstOrDefault(a => a.Id == id).IsSync;
        }
        #endregion

        #region Plan
        public List<Plan> GetPlansList()
        {
            return _db.Table<Plan>().ToList();
        }

        public void InsertPlan(Plan plan)
        {
            var p = GetPlansList().Where(a => a.Id == plan.Id);
            if (!p.Any())
            {
                _db.Insert(plan);
            }
        }

        public void ClearPlan()
        {
            _db.Execute("DELETE FROM Plan");
        }
        #endregion

        #region Check

        public void InsertCheck(Check check)
        {
            var p = GetCheckList().Where(a => a.Id == check.Id);
            if (!p.Any())
            {
                _db.Insert(check);
            }
        }

        public List<Check> GetCheckList()
        {
            return _db.Table<Check>().ToList();
        }

        public Check GetCheckById(long id)
        {
            return _db.Table<Check>().FirstOrDefault(a => a.Id == id);
        }

        #endregion

        #region Climate

        public void InsertClimate(Climate climate)
        {
            var p = GetClimateList().Where(a => a.Id == climate.Id);
            if (!p.Any())
            {
                _db.Insert(climate);
            }
        }

        public List<Climate> GetClimateList()
        {
            return _db.Table<Climate>().ToList();
        }

        #endregion

        #region LocationDetail

        public void InsertLocationDetail(LocationDetail locationDetail)
        {
            var p = GetLocationDetailList().Where(a => a.Id == locationDetail.Id);
            if (!p.Any())
            {
                _db.Insert(locationDetail);
            }
        }

        public List<LocationDetail> GetLocationDetailList()
        {
            return _db.Table<LocationDetail>().ToList();
        }

        public void CheckLocationDetail(LocationDetail locationDetail)
        {
            locationDetail.Check = true;
            _db.Update(locationDetail);
        }

        public void UnCheckLocationDetail(LocationDetail locDetail)
        {
            locDetail.Check = false;
            _db.Update(locDetail);
        }

        public LocationDetail GetLocationDetailById(long id)
        {
            return _db.Table<LocationDetail>().FirstOrDefault(a => a.Id == id);
        }

        public async Task wait()
        {
            await Task.Delay(2000);
        }

        #endregion

        #region LocationDetailDtl
        public void InsertSubmittedLocationDtl(SubmittedLocationDtl SubmittedLocationDtl)
        {
            _db.Insert(SubmittedLocationDtl);
        }

        public List<SubmittedLocationDtl> SubmittedLocationDtlsList()
        {
            return _db.Table<SubmittedLocationDtl>().ToList();
        }

        public void SubmittedLocationDtlIsSyncTrue(SubmittedLocationDtl SubmittedLocationDtl)
        {
            SubmittedLocationDtl.IsSync = true;
            _db.Update(SubmittedLocationDtl);
        }
        #endregion
    }
}