using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockHaus.Core.BaseService;
using StockHaus.Data.Model;
using StockHaus.ModelClass.AdminPage;
using StockHaus.ModelClass.UserPage;

namespace StockHaus.Core.DataService
{
    public class UserService : ServiceBase<Users>
    {
        public override MessageService Insert(Users dto)
        {
            if (context.User.Any(x => x.Mail == dto.Mail))
            {
                result.ResultID = 0;
                if (result.IsSuccess == false)
                {
                    result.Message = "Benzer Kayıt Var !";
                    return result;
                }
            }

            bool isExist = (dto == null) ? false : true;

            if (isExist)
            {
                dto.IsActive = true;
                dbset.Add(dto);

                try
                {
                    context.SaveChanges();
                    result.Message = "True";
                }
                catch (Exception ex)
                {
                    result.Message = ex.Message;
                }
            }
            return result;
        }

        public override MessageService Update(Users dto)
        {
            if (dbset.Any(x => x.ID == dto.ID))
            {
                Users user = FindByID(dto.ID);
                user.IsOnline = dto.IsOnline;
                user.IpAddress = dto.IpAddress;
                user.IsSignableDocument = dto.IsSignableDocument;
                user.Role = dto.Role;

                result.ResultID = user.ID;
                if (result.IsSuccess == true)
                {
                    try
                    {
                        result.Message = "True";
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        result.Message = ex.Message;
                    }
                }
            }
            return result;
        }

        public string BeforeInsert(DefineUser dto)
        {
            Users d = new Users
            {
                ChangedBy = dto._createdBy,
                ChangeDate = DateTime.Now,
                Company = "01",
                CreateDate = DateTime.Now,
                CreatedBy = dto._createdBy,
                IpAddress = dto._ipAddress,
                IsActive = true,
                IsOnline = 0,
                IsSignableDocument = false,
                Mail = dto._email,
                Name = dto._name,
                Password = dto._password,
                Role = 5000
            };

            MessageService msg = Insert(d);

            return msg.Message;
        }

        // ıd den dto yu yakalayıp Update ile güncellemeyi göndereceğiz
        public string UpdateRole(int id, int rol)
        {
            Users _dto = context.User.Where(x => x.ID == id && x.IsActive).SingleOrDefault();
            _dto.Role = rol;

            MessageService msg = Update(_dto);

            return msg.Message;
        }

        public bool UpdateBeforeOnline(StockHaus.ModelClass.LoginService.ViewModel.LoginModel model)
        {
            Users dto = context.User.Where(x => x.IsActive && x.Mail == model.Email && x.Password == model.Password).SingleOrDefault();
            dto.IsOnline = dto.IsOnline + 1;

            try
            {
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string UpdateOnline(int id, int status)
        {
            Users _dto = context.User.Where(x => x.ID == id && x.IsActive).SingleOrDefault();
            _dto.IsOnline = status;

            MessageService msg = Update(_dto);

            return msg.Message;
        }

        public bool UpdateAuth(string _mail)
        {
            Users dto = context.User.Where(x => x.IsActive && x.Mail == _mail).SingleOrDefault();
            dto.IsSignableDocument = true;

            try
            {
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool IsCanSignPersonel(string _mail)
        {
            return context.User.Any(x => x.IsActive && x.Mail == _mail && x.IsSignableDocument);
        }

        // role bilgileri nedir ?
        public int CheckRoleFromMail(string mail)
        {
            return context.User.Where(x => x.IsActive && x.Mail == mail).Select(y => y.Role).SingleOrDefault();
        }

        // hesap aktif mi var mı ?
        public bool AccountIsActive(string mail, string pass)
        {
            return context.User.Any(x => x.IsActive && x.Mail == mail && x.Password == pass);
        }

        public List<UserModel> GetPersonel()
        {
            var list = context.User.Where(x => x.IsActive && (x.Role == 3000)).Select
                (c => new UserModel
                {
                    _id = c.ID,
                    _mail = c.Mail,
                    _name = c.Name,
                    _isSignable = c.IsSignableDocument

                }).ToList();

            return list;
        }

        public int GetID(string _mail)
        {
            return context.User.Where(x => x.IsActive && x.Mail == _mail).Select(c => c.ID).SingleOrDefault();
        }
        public int GetIDFName(string _name)
        {
            return context.User.Where(x => x.IsActive && x.Name == _name).Select(c => c.ID).SingleOrDefault();
        }

        // pre estimated
        public ICollection<UserModel> GetNameFromChar(string charName)
        {
            string _upper = "";
            if (charName != null)
            {
                _upper = charName.ToUpper();
            }
            else
            {
                _upper = null;
            }
            var _estimatedPerNameList = context.User.Where(x => x.Name.Contains(_upper) && x.IsActive && (x.Role == 5000 || x.Role == 3000))
                                                    .Select(c => new UserModel
                                                    {
                                                        _id = c.ID,
                                                        _mail = c.Mail,
                                                        _name = c.Name
                                                    })
                                                      .ToList();

            return _estimatedPerNameList;
        }

        public TeamGroup GetTeamAndGroup(string username)
        {
            var _data = context.User.Join(context.InventoryGroup,
                                          user => user.ID,
                                          ig => ig.UserID,
                                          (user, ig) => new
                                          {
                                              USERX = user,
                                              IGX = ig
                                          })
                                          .Where(c => c.USERX.Mail == username && c.IGX.IsActive == true)
                                          .Select(v => new
                                          {
                                              v.USERX.ID,
                                              v.USERX.Name,
                                              v.IGX.TeamNo,
                                              v.IGX.GroupNo
                                          })
                                            .Distinct().SingleOrDefault();
            TeamGroup _tg = new TeamGroup
            {
                _team = _data.TeamNo,
                _group = _data.GroupNo
            };


            return _tg;
        }

        public bool isOnline(string mail)
        {
            int count = context.User.Where(x => x.IsActive && x.Mail == mail).Select(c => c.IsOnline).SingleOrDefault();

            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public List<UserModel> GetOnlineUser()
        {
            return context.User.Where(x => x.IsActive && (x.Role == 2000 || x.Role == 3000 || x.Role == 4000)).Select(
                   c => new UserModel
                   {
                       _id = c.ID,
                       _mail = c.Mail,
                       _name = c.Name,
                       _status = c.IsOnline
                   }).ToList();
        }

        public string GetMailForName(string name)
        {
            return context.User.Where(x => x.IsActive && x.Name == name).Select(c => c.Mail).SingleOrDefault();
        }
        public string GetNameFromMail(string mail)
        {
            return context.User.Where(x => x.Mail == mail && x.IsActive).Select(c => c.Name).SingleOrDefault();
        }
    }
}
