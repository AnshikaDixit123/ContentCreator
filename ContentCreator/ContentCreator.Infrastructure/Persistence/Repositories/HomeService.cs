using ContentCreator.Application.Common.DTOs;
using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;
using ContentCreator.Application.Interfaces;
using ContentCreator.Domain.Entities.Identity;
using ContentCreator.Domain.Enums;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ContentCreator.Infrastructure.Persistence.Repositories
{
    public class HomeService : IHomeService
    {
        private readonly IContentCreatorDBContext _context;
        private readonly IDbConnection _dbConnection;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public HomeService(IContentCreatorDBContext context, UserManager<ApplicationUser> userManager, IDbConnection dbConnection, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _dbConnection = dbConnection;
            _roleManager = roleManager;
        }
        public async Task<ResponseData<bool>> CreateUserAsync(CreateNewUserRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>
            {
                Message = "Invalid username or email address or phone no.",
                Result = false
            };

            // Normalize to match Identity’s internal storage
            var normalizedEmail = _userManager.NormalizeEmail(request.EmailAddress);
            var normalizedUserName = _userManager.NormalizeName(request.UserName);
            string sql = @"
                SELECT 
                    CASE WHEN EXISTS (SELECT 1 FROM AspNetUsers WHERE NormalizedEmail = @Email) THEN 1 ELSE 0 END AS EmailExists,
                    CASE WHEN EXISTS (SELECT 1 FROM AspNetUsers WHERE NormalizedUserName = @UserName) THEN 1 ELSE 0 END AS UserNameExists,
                    CASE WHEN EXISTS (SELECT 1 FROM AspNetUsers WHERE PhoneNumber = @PhoneNumber) THEN 1 ELSE 0 END AS PhoneExists;";

            var exists = await _dbConnection.QuerySingleAsync<UserExistsDto>(sql, new
            {
                Email = normalizedEmail,
                UserName = normalizedUserName,
                PhoneNumber = request.PhoneNumber
            });
            if (exists.EmailExists)
            {
                response.Message = "Email already exists.";
                return response;
            }
            if (exists.UserNameExists)
            {
                response.Message = "Username already exists.";
                return response;
            }
            if (exists.PhoneExists)
            {
                response.Message = "Phone number already exists.";
                return response;
            }

            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.EmailAddress,
                PhoneNumber = request.PhoneNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                var role = await _roleManager.FindByIdAsync(request.RoleID.ToString());
                await _userManager.AddToRoleAsync(user, role.Name); // assign default role
                response.StatusCode = 200;
                response.Message = "User registered successfully";
                response.Result = true;
                response.IsSuccess = true;
            }
            return response;
        }
        public async Task<ResponseData<UserResponseModel>> GetMyProfileAsync(Guid UserId, CancellationToken cancellation)
        {
            var response = new ResponseData<UserResponseModel>();
            response.Message = "User doesn't exist";
            var userDetail = await _userManager.FindByIdAsync(UserId.ToString());

            if (userDetail != null)
            {
                var userResponse = new UserResponseModel();
                userResponse.UserId = userDetail.Id;
                userResponse.UserName = userDetail.UserName ?? string.Empty;
                userResponse.FirstName = userDetail.FirstName;
                userResponse.LastName = userDetail.LastName;
                userResponse.EmailAddress = userDetail.Email ?? string.Empty;
                userResponse.PhoneNumber = userDetail.PhoneNumber ?? string.Empty;
                userResponse.CompleteAddress = userDetail.CompleteAddress ?? string.Empty;
                userResponse.CountryId = userDetail.CountryId;
                userResponse.StateId = userDetail.StateId;
                userResponse.CityId = userDetail.CityId;

                response.StatusCode = 200;
                response.Message = "success";
                response.Result = userResponse;
                response.IsSuccess = true;
            }
            return response;
        }
        public async Task<ResponseData<bool>> SaveChangesAsync(SaveChangesRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>();
            response.Message = "Email or phone number already exist!";

            var normalizedEmail = _userManager.NormalizeEmail(request.Email);
            string sql = @"
            SELECT 
                0 AS UserNameExists, 
                CASE WHEN EXISTS (
                    SELECT 1 FROM AspNetUsers 
                    WHERE NormalizedEmail = @Email 
                      AND Id <> @UserId
                ) THEN 1 ELSE 0 END AS EmailExists,
                CASE WHEN EXISTS (
                    SELECT 1 FROM AspNetUsers 
                    WHERE PhoneNumber = @PhoneNumber 
                      AND Id <> @UserId
                ) THEN 1 ELSE 0 END AS PhoneExists;";
            var exists = await _dbConnection.QuerySingleAsync<UserExistsDto>(sql, new
            {
                Email = normalizedEmail,
                UserId = request.UserId,
                PhoneNumber = request.PhoneNumber
            });
            if (exists.EmailExists)
            {
                response.Message = "Email already exists.";
                return response;
            }
            if (exists.PhoneExists)
            {
                response.Message = "Phone number already exists.";
                return response;
            }
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            response.Message = "User doesn't exist!";
            if(user != null)
            {
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Email = request.Email;
                user.PhoneNumber = request.PhoneNumber;
                user.CountryId = request.CountryId;
                user.StateId = request.StateId;
                user.CityId = request.CityId;
                user.CompleteAddress = request.CompleteAddress;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    response.StatusCode = 200;
                    response.Message = "Profile Updated Successfully!";
                    response.Result = true;
                    response.IsSuccess = true;
                }
            }


            return response;
        }
        public async Task<ResponseData<bool>> CreateRolesAsync(CreateRolesRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>();
            response.Message = "Role already exist";
            


            return response;
        }

        public async Task<ResponseData<List<CountryResponseModel>>> GetCountryAsync(CancellationToken cancellation)
        {
            var response = new ResponseData<List<CountryResponseModel>>();
            response.Message = "Something went wrong";
            List<CountryResponseModel> countryList = new List<CountryResponseModel>();
            countryList = await _context.Country.Select(x => new CountryResponseModel {Id =  x.Id, CountryName = x.CountryName, CountryCode = x.CountryCode ?? string.Empty, PhoneCode = x.PhoneCode ?? string.Empty, StateCount = 5}).ToListAsync();
            if (countryList.Any())
            {
                response.StatusCode = 200;
                response.Message = "success";
                response.Result = countryList;
                response.IsSuccess = true;
            }
            return response;
        }

        public async Task<ResponseData<List<StateResponseModel>>> GetStateAsync(Guid CountryId, CancellationToken cancellation)
        {
            var response = new ResponseData<List<StateResponseModel>>();
            response.Message = "Something went wrong";
            List<StateResponseModel> stateList = new List<StateResponseModel>();
            stateList = await _context.State.Where(x => ( CountryId == Guid.Empty) || (CountryId != Guid.Empty && x.CountryId == CountryId)).Select(x =>
            new StateResponseModel {
                Id = x.Id, StateName = x.StateName,
                CountryId = x.CountryId,
                StateCode = x.StateCode ?? string.Empty,
                CityCount = 5 }).ToListAsync();
            if (stateList.Any())
            {
                response.StatusCode = 200;
                response.Message = "success";
                response.IsSuccess = true;
            }
            response.Result = stateList;
            return response;
        }

        public async Task<ResponseData<List<CityResponseModel>>> GetCityAsync(Guid CountryId, Guid StateId, CancellationToken cancellation)
        {
            var response = new ResponseData<List<CityResponseModel>>();
            response.Message = "Something went wrong";
            List<CityResponseModel> cityList = new List<CityResponseModel>();

            var query = _context.City.AsQueryable();

            if(CountryId == Guid.Empty && StateId == Guid.Empty)
            {
                //query = query;
            }
            else if(StateId == Guid.Empty)
            {
                query = query.Where(x => x.CountryId == CountryId);
            }
            else
            {
                query = query.Where(x => x.CountryId == CountryId && x.StateId == StateId);
            }
            cityList = query.Select(x => new CityResponseModel
            {
                Id = x.Id,
                CityName = x.CityName,
                CountryId = x.CountryId,
                StateId = x.StateId
            }).ToList();

            //if(1 == 1)
            //{
            //    var query1 = _context.City.Select(x => new CityResponseModel
            //    {
            //        Id = x.Id,
            //        CityName = x.CityName,
            //        CountryId = x.CountryId,
            //        StateId = x.StateId
            //    }).ToList();
            //}

            //if(2 == 2)
            //{
            //    var query2 = _context.City.Where(x => x.CountryId == CountryId).Select(x => new CityResponseModel
            //    {
            //        Id = x.Id,
            //        CityName = x.CityName,
            //        CountryId = x.CountryId,
            //        StateId = x.StateId
            //    }).ToList();
            //}

            //if(3 == 3)
            //{
            //    var query3 = _context.City.Where(x => x.CountryId == CountryId && x.StateId == StateId).Select(x => new CityResponseModel
            //    {
            //        Id = x.Id,
            //        CityName = x.CityName,
            //        CountryId = x.CountryId,
            //        StateId = x.StateId
            //    }).ToList();
            //}

            //Select(x => new CityResponseModel {Id =  x.Id, CityName = x.CityName, CountryId = x.CountryId, StateId = x.StateId}).ToListAsync();
            if (cityList.Any())
            {
                response.StatusCode = 200;
                response.Message = "success";
                response.Result = cityList;
                response.IsSuccess = true;
            }
            return response;
        }
        public async Task<ResponseData<List<CountryStateCityNestedResponseModel>>> CountryStateCityNestedAsync(CancellationToken cancellation)
        {
            var response = new ResponseData<List<CountryStateCityNestedResponseModel>>
            {
                Message = "Something went wrong",
                IsSuccess = false,
                StatusCode = 500
            };

            var countryStateCityList = new List<CountryStateCityNestedResponseModel>();
            var getCountryList = await _context.Country.Select(x => new { x.Id, x.CountryName }).ToListAsync(cancellation);

            foreach (var country in getCountryList)
            {
                var countryResponse = new CountryStateCityNestedResponseModel();
                countryResponse.CountryId = country.Id;
                countryResponse.CountryName = country.CountryName;

                var stateList = new List<StateCityNestedResponseModel>();
                var getStateList = await _context.State.Where(x => x.CountryId == country.Id).Select(x => new { x.Id, x.StateName }).ToListAsync(cancellation);

                foreach (var state in getStateList)
                {
                    var stateResponse = new StateCityNestedResponseModel();
                    stateResponse.StateId = state.Id;
                    stateResponse.StateName = state.StateName;

                    var cityList = new List<CityNestedResponseModel>();
                    var getCityList = await _context.City.Where(x => x.StateId == state.Id).Select(x => new { x.Id, x.CityName }).ToListAsync(cancellation);

                    foreach (var city in getCityList)
                    {
                        var cityResponse = new CityNestedResponseModel();
                        cityResponse.CityId = city.Id;
                        cityResponse.CityName = city.CityName;
                        cityList.Add(cityResponse);
                    }

                    stateResponse.CityList = cityList;

                    stateList.Add(stateResponse);
                }
                countryResponse.StateList = stateList;

                countryStateCityList.Add(countryResponse);
            }
            response.StatusCode = 200;
            response.Message = "success";
            response.Result = countryStateCityList;
            response.IsSuccess = true;

            return response;
        }
        public async Task<ResponseData<bool>> AddCountryAsync(AddCountryRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>();
            response.Message = "This Country already exist";

            var addCountry = await _context.Country.Where(x => 
            x.CountryName == request.CountryName
            || x.CountryCode == request.CountryCode 
            || x.PhoneCode == request.PhoneCode).FirstOrDefaultAsync(cancellation);

            if(addCountry != null)
            {
                if(addCountry.CountryName == request.CountryName)
                {
                    response.Message = "Country name already exist";
                }
                else if(addCountry.CountryCode == request.CountryCode)
                {
                    response.Message = "Country code already exist";
                }
                else
                {
                    response.Message = "Phone code already exist";
                }
                return response;
            }
            var country = new Country();
            country.CountryName = request.CountryName;
            country.CountryCode = request.CountryCode;
            country.PhoneCode = request.PhoneCode;
            _context.Country.Add(country);
            await _context.SaveChangesAsync();

            response.StatusCode = 200;
            response.Message = "Country added successfully";
            response.Result = true;
            response.IsSuccess = true;

            return response;
        }
        public async Task<ResponseData<bool>> AddStateAsync(AddStateRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>();
            response.Message = "This State already exist";

            var addState = await _context.State.Where(x => 
            x.StateName == request.StateName
            || x.StateCode == request.StateCode).FirstOrDefaultAsync(cancellation);

            if(addState != null)
            {
                if(addState.StateName == request.StateName)
                {
                    response.Message = "State name already exist";
                }
                else
                {
                    response.Message = "State code already exist";
                }
                return response;
            }
            var state = new State();
            state.StateName = request.StateName;
            state.StateCode = request.StateCode;
            state.CountryId = request.CountryId;
            _context.State.Add(state);
            await _context.SaveChangesAsync();

            response.StatusCode = 200;
            response.Message = "State added successfully";
            response.Result = true;
            response.IsSuccess = true;

            return response;
        }
        public async Task<ResponseData<bool>> AddCityAsync(AddCityRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>();
            response.Message = "This City already exist";

            var addCity = await _context.City.Where(x => 
            x.CityName == request.CityName).FirstOrDefaultAsync(cancellation);

            if(addCity != null)
            {
                if (addCity.CityName == request.CityName)
                {
                    response.Message = "City name already exist";
                }
                return response;
            }
            var city = new City();
            city.CityName = request.CityName;
            city.StateId = request.StateId;
            city.CountryId = request.CountryId;
            _context.City.Add(city);
            await _context.SaveChangesAsync();

            response.StatusCode = 200;
            response.Message = "City added successfully";
            response.Result = true;
            response.IsSuccess = true;

            return response;
        }

    }
}
