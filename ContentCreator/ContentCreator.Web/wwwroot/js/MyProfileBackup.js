$(document).ready(function () {
    const constUserId = localStorage.getItem("UserId");
    const emptyGuid = "00000000-0000-0000-0000-000000000000";
    //GetCountryList()
    function GetCountryList(CountryId, StateId, CityId, isSelected) {
        $('#Country').html(`<option selected disabled value="">Select Country</option>`)
        $.ajax({
            url: "https://localhost:7134/" + "api/Home/GetCountry",
            type: "GET",
            success: function (response) {
                if (response.StatusCode == 200) {
                    var countryList = response.Result.length;
                    for (var i = 0; i < countryList; i++) {
                        var data = response.Result[i];
                        var selectStr = "";
                        if (isSelected) {
                            if (CountryId == data.Id) {
                                selectStr = "selected";
                            }
                        }
                        var optionHtml = `<option ${selectStr} value="${data.Id}">${data.CountryName}</option>`;
                        $('#Country').append(optionHtml);
                    }
                    if (isSelected) {
                        GetStateList(CountryId, StateId, CityId, true);
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
        GetStateList(countryId, emptyGuid, emptyGuid, false);
    })
    function GetStateList(countryId, StateId, CityId, isSelected) {
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
                        var selectStr = "";
                        if (isSelected) {
                            if (StateId == data.Id) {
                                selectStr = "selected"
                            }
                        }
                        var optionHtml = `<option ${selectStr} value="${data.Id}">${data.StateName}</option>`;
                        $('#State').append(optionHtml);
                    }
                    if (isSelected) {
                        GetCityList(StateId, CityId, true)
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
        GetCityList(stateId, emptyGuid, false);
    })
    function GetCityList(stateId, cityId, isSelected) {
        $('#City').html(`<option selected disabled value="">Select City</option>`)
        $.ajax({
            url: "https://localhost:7134/" + "api/Home/GetCity?StateId=" + stateId,
            type: "GET",
            success: function (response) {
                if (response.StatusCode == 200) {
                    var cityList = response.Result.length;
                    for (var i = 0; i < cityList; i++) {
                        var data = response.Result[i];
                        var selectStr = "";
                        if (isSelected) {
                            if (cityId == data.Id) {
                                selectStr = "selected";
                            }
                        }
                        var optionHtml = `<option ${selectStr} value="${data.Id}">${data.CityName}</option>`;
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
            url: "https://localhost:7134/" + "api/Home/GetMyProfile?UserId=" + constUserId,
            type: "GET",
            success: function (response) {
                console.log(response)
                if (response.StatusCode == 200) {
                    $('#UserName').val(response.Result.UserName);
                    $('#FirstName').val(response.Result.FirstName);
                    $('#LastName').val(response.Result.LastName);
                    $('#EmailAddress').val(response.Result.EmailAddress);
                    $('#PhoneNo').val(response.Result.PhoneNumber);
                    $('#CompleteAddress').val(response.Result.CompleteAddress);

                    if (response.Result.CountryId) {
                        GetCountryList(response.Result.CountryId, response.Result.StateId, response.Result.CityId, true);
                    }
                    else {
                        GetCountryList(emptyGuid, emptyGuid, emptyGuid, false);
                    }
                }
                else {
                    Swal.fire('warning', response.Message, 'warning');
                }
            },
            error: function (error) {
                console.warn(error);
                Swal.fire('Failed', "Something went wrong", 'error');
            }
        })
    }
    $(document).on('click', '#saveChanges', function () {
        var FirstName = $('#FirstName').val();
        var LastName = $('#LastName').val();
        var EmailAddress = $('#EmailAddress').val();
        var PhoneNumber = $('#PhoneNo').val();
        var Country = $('#Country').val();
        var State = $('#State').val();
        var City = $('#City').val();
        var CompleteAddress = $('#CompleteAddress').val();

        var formData = new FormData();
        formData.append('UserId', constUserId);
        formData.append('FirstName', FirstName);
        formData.append('LastName', LastName);
        formData.append('Email', EmailAddress);
        formData.append('PhoneNumber', PhoneNumber);
        formData.append('CountryId', Country);
        formData.append('StateId', State);
        formData.append('CityId', City);
        formData.append('CompleteAddress', CompleteAddress);

        $.ajax({
            url: "https://localhost:7134/" + "api/Home/SaveChanges",
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.StatusCode == 200) {
                    Swal.fire('success', response.Message, 'success');
                }
                else {
                    Swal.fire('warning', "Something went wrong", 'warning');
                }
            },
            error: function (error) {
                console.warn(error);
                Swal.fire('Failed', "Something went wrong", 'error');
            }
        });
    });
});