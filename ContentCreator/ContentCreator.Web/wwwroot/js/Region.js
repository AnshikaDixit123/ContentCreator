$(document).ready(function () {
    let currentSection = "Country"
    let emptyGuid = "00000000-0000-0000-0000-000000000000";
    let clickedCountryId = emptyGuid;
    let clickedStateId = emptyGuid;
    //$("#btnAddCountry").trigger('click')
    GetCountryList()
    function GetCountryList() {
        $('#tblCountryListBody').html(``)
        
        $.ajax({
            url: "https://localhost:7134/" + "api/Home/GetCountry",
            type: "GET",
            success: function (response) {
                if (response.StatusCode == 200) {
                    var recordCountry = response.Result.length;
                    // for table
                    for (var i = 0; i < recordCountry; i++) {
                        var data = response.Result[i];
                        var tableData = `<tr data-countryid="${data.Id}">
                            <td>${i + 1}</td>
                            <td>${data.CountryName}</td>
                            <td>${data.CountryCode}</td>
                            <td>${data.PhoneCode}</td>
                            <td>${data.StateCount}</td>
                            <td><span class="badge bg-success gotostate">Go to State</span></td>
                        </tr>`
                        $('#tblCountryListBody').append(tableData);
                    }
                    // for dropdowns
                    var optionHtml = `<option value="">Select Country</option>`;
                    for (var i = 0; i < recordCountry; i++) {
                        var data = response.Result[i];
                        optionHtml += `<option value="${data.Id}">${data.CountryName}</option>`;                        
                    }
                    $('#ddlCountryOnState').html(optionHtml);
                    $('#ddlCountryOnCity').html(optionHtml);
                }
                else {
                    console.log("error");
                }
            },
            error: function (error) {
                console.warn(error);
            }
        });
    }
    
    $(document).on("click", ".gotostate", function () {
        currentSection = "State"
        $("#StateSection").removeClass("d-none")
        $("#CountrySection").addClass("d-none")
        clickedCountryId = $(this).closest("tr").data("countryid");
        $("#ddlCountryOnState").val(clickedCountryId)//.trigger('change')
        GetStateList()
    })
    
    $(document).on("change", "#ddlCountryOnState", function () {
        var countryId = $(this).val();
        if (countryId) {
            clickedCountryId = countryId;
            $("#addStateSection").removeClass('d-none');
        }
        else {
            clickedCountryId = emptyGuid;
            $("#addStateSection").addClass('d-none');
        }
        GetStateList();
    })
    
    function GetStateList() {
        $('#tblStateListBody').html(``)
        $.ajax({
            url: "https://localhost:7134/" + "api/Home/GetState?CountryId=" + clickedCountryId,
            type: "GET",
            success: function (response) {
                if (response.StatusCode == 200) {
                    var recordState = response.Result.length;
                    for (var i = 0; i < recordState; i++) {
                        var data = response.Result[i];
                        var tableData = `<tr data-stateid="${data.Id}">
                            <td>${i + 1}</td>
                            <td>${data.StateName}</td>
                            <td>${data.StateCode}</td>
                            <td>${data.CityCount}</td>
                            <td><span class="badge bg-success gotocity">Go to City</span></td>
                        </tr>`
                        $('#tblStateListBody').append(tableData);
                    }
                    //for dropdown

                    var optionHtml = `<option value="">Select State</option>`
                    for (var i = 0; i < recordState; i++) {
                        var data = response.Result[i];
                        optionHtml += `<option value="${data.Id}">${data.StateName}</option>`;
                    }
                    $('#StateforCity').html(optionHtml);
                }
                else {
                    console.log("error");
                }
            },
            error: function (error) {
                console.warn(error);
            }
        });
    }

    $(document).on("click", ".gotocity", function () {
        currentSection = "City"
        clickedStateId = $(this).closest("tr").data("stateid");
        $("#CitySection").removeClass("d-none")
        $("#CountrySection").addClass("d-none")
        $("#StateSection").addClass("d-none")
        $("#ddlCountryOnCity").val(clickedCountryId)
        $("#StateforCity").val(clickedStateId)
        GetCityList()
    })
    $(document).on("change", "#ddlCountryOnCity", function () {
        var countryId = $(this).val();
        clickedStateId = emptyGuid;
        $('#StateforCity').html(`<option value="">Select State</option>`);
        $("#addCitySection").addClass('d-none');
        if (countryId) {
            clickedCountryId = countryId;
             GetStateList();
        }
        else {
            clickedCountryId = emptyGuid;
        }
        GetCityList();

    })

    $(document).on("click", ".backtostate, .backtocountry", function () {
        if (currentSection == "City") {
            $("#CitySection").addClass("d-none")
            $("#CountrySection").addClass("d-none")
            $("#StateSection").removeClass("d-none")
            clickedStateId = emptyGuid;
            currentSection = "State"
        } else if (currentSection == "State") {
            $("#CitySection").addClass("d-none")
            $("#CountrySection").removeClass("d-none")
            $("#StateSection").addClass("d-none")
            clickedCountryId = emptyGuid;
            currentSection = "Country"
        } 
    })
    //$(document).on("click", ".backtostate, .backtocountry", function () {
    //    if ($(this).hasClass("backtostate")) {
    //        $("#CitySection").addClass("d-none")
    //        $("#CountrySection").addClass("d-none")
    //        $("#StateSection").removeClass("d-none")
    //        clickedStateId = emptyGuid;
    //        currentSection = "State"
    //    } else if ($(this).hasClass("backtocountry")) {
    //        $("#CitySection").addClass("d-none")
    //        $("#CountrySection").removeClass("d-none")
    //        $("#StateSection").addClass("d-none")
    //        clickedCountryId = emptyGuid;
    //        currentSection = "Country"
    //    }
    //});
    function GetCityList() {
        $('#tblCityListBody').html(``)
        $.ajax({
            url: "https://localhost:7134/" + "api/Home/GetCity?CountryId=" + clickedCountryId + "&StateId" + clickedStateId,
            type: "GET",
            success: function (response) {
                if (response.StatusCode == 200) {
                    var recordRoles = response.Result.length;
                    for (var i = 0; i < recordRoles; i++) {
                        var data = response.Result[i];
                        var tableData = `<tr data-cityId="${data.Id}">
                            <td>${i + 1}</td>
                            <td>${data.CityName}</td>
                        </tr>`
                        $('#tblCityListBody').append(tableData);
                    }
                }
                else {
                    console.log("error");
                }
            },
            error: function (error) {
                console.warn(error);
            }
        });
    }
    $(document).on("click", "#btnAddCountry", function () {
        var countryName = $("#countryInput").val();
        var countryCode = $("#countryCodeInput").val();
        var phoneCode = $("#phoneCodeInput").val();

        if (countryName == "" || countryCode == "" || phoneCode == "") {
            Swal.fire('Warning', 'Enter all the details', 'warning');
            return;
        }
        var formData = new FormData();
        formData.append('countryName', countryName);
        formData.append('countryCode', countryCode);
        formData.append('phoneCode', phoneCode);

        $.ajax({
            url: "https://localhost:7134/" + "api/Home/AddCountry",
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.StatusCode == 200) {
                    Swal.fire('Successful', response.Message, 'success');
                    GetCountryList();
                }
                else {
                    Swal.fire('Warning', response.Message, 'error');
                }
                
            },
            error: function (error) {
                console.warn(error)
            }
        })
    });
    $(document).on("click", "#btnAddState", function () {
        var stateName = $("#stateInput").val();
        var stateCode = $("#stateCodeInput").val();

        if (stateName == "" || stateCode == "") {
            Swal.fire('Warning', 'Enter all the details', 'warning');
            return;
        }
        var formData = new FormData();
        formData.append('stateName', stateName);
        formData.append('stateCode', stateCode);
        formData.append('countryId', clickedCountryId);

        $.ajax({
            url: "https://localhost:7134/" + "api/Home/AddState",
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.StatusCode == 200) {
                    Swal.fire('Successful', response.Message, 'success');
                    GetStateList();
                }
                else {
                    Swal.fire('Warning', response.Message, 'error');
                }
                
            },
            error: function (error) {
                console.warn(error)
            }
        })
    });
    $(document).on("click", "#btnAddCity", function () {
        var cityName = $("#cityInput").val();

        if (cityName == "") {
            Swal.fire('Warning', 'Enter all the details', 'warning');
            return;
        }
        var formData = new FormData();
        formData.append('cityName', cityName);
        formData.append('stateId', clickedStateId);
        formData.append('countryId', clickedCountryId);

        $.ajax({
            url: "https://localhost:7134/" + "api/Home/AddCity",
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.StatusCode == 200) {
                    Swal.fire('Successful', response.Message, 'success');
                    GetCityList();
                }
                else {
                    Swal.fire('Warning', response.Message, 'error');
                }
                
            },
            error: function (error) {
                console.warn(error)
            }
        })
    });
})