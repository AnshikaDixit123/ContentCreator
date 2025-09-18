$(document).ready(function () {
    constUserName = localStorage.getItem("UserName");
    GetCountryList()
    function GetCountryList() {
        $('#Country').html(`<option selected disabled value="">Select Country</option>`)
        $.ajax({
            url: "https://localhost:7134/" + "api/Home/GetCountry",
            type: "GET",
            success: function (response) {
                if (response.StatusCode == 200) {
                    var countryList = response.Result.length;
                    for (var i = 0; i < countryList; i++) {
                        var data = response.Result[i];
                        var optionHtml = `<option value="${data.Id}">${data.CountryName}</option>`;
                        $('#Country').append(optionHtml);
                    }
                }
                else {
                    Swal.fire('warning', "Something went wrong", 'warning');
                }
            },
            error: function (error) {
                console.warn(error)
                Swal.fire('error', "ERROR", 'error');
            }
        });
    }
    $(document).on('change', '#Country', function () {
        var countryId = $(this).val();
        GetStateList(countryId);
    })
    function GetStateList(countryId) {
        $('#State').html(`<option selected disabled value="">Select State</option>`)
        $('#City').html(`<option selected disabled value="">Select City</option>`)
        $.ajax({
            url: "https://localhost:7134/" + "api/Home/GetState?CountryId=" + countryId,
            type: "GET",
            success: function (response) {
                if (response.StatusCode == 200) {
                    var stateList = response.Result.length;
                    for (var i = 0; i < stateList; i++) {
                        var data = response.Result[i];
                        var optionHtml = `<option value="${data.Id}">${data.StateName}</option>`;
                        $('#State').append(optionHtml);
                    }
                }
                else {
                    Swal.fire('warning', "Something went wrong", 'warning');
                }
            },
            error: function (error) {
                console.warn(error)
                Swal.fire('error', "ERROR", 'error');
            }
        });
    }
    $(document).on('change', '#State', function () {
        var stateId = $(this).val();
        GetCityList(stateId);
    })
    function GetCityList(stateId) {
        $('#City').html(`<option selected disabled value="">Select City</option>`)
        $.ajax({
            url: "https://localhost:7134/" + "api/Home/GetCity?StateId=" + stateId,
            type: "GET",
            success: function (response) {
                if (response.StatusCode == 200) {
                    var cityList = response.Result.length;
                    for (var i = 0; i < cityList; i++) {
                        var data = response.Result[i];
                        var optionHtml = `<option value="${data.Id}">${data.CityName}</option>`;
                        $('#City').append(optionHtml);
                    }
                }
                else {
                    Swal.fire('warning', "Something went wrong", 'warning');
                }
            },
            error: function (error) {
                console.warn(error)
                Swal.fire('error', "ERROR", 'error');
            }
        });
    }
    GetMyProfile()
    function GetMyProfile() {
        $.ajax({
            url: "https://localhost:7134/" + "api/Home/GetMyProfile?UserName=" + constUserName,
            type: "GET",
            success: function (response) {
                console.log(response)
                if (response.StatusCode == 200) {
                    ('#UserName').val(response.Result.UserName);
                    ('#FirstName').val(response.Result.FirstName);
                    ('#LastName').val(response.Result.LastName);
                    ('#EmailAddress').val(response.Result.EmailAddress);
                    ('#PhoneNo').val(response.Result.PhoneNumber);
                }
                else {
                    Swal.fire('warning', "Something went wrong", 'warning');
                }
            },
            error: function (error) {
                console.warn(error);
            }
        })
    }
});