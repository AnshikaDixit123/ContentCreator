$(document).ready(function () {
    let currentSection = "Country"
    let emptyGuid = "00000000-0000-0000-0000-000000000000";
    let clickedCountryId = emptyGuid;
    let clickedStateId = emptyGuid;
    GetCountryList()
    function GetCountryList() {
        $('#tblCountryListBody').html(``)
        $.ajax({
            url: "https://localhost:7134/" + "api/Home/GetCountry",
            type: "GET",
            success: function (response) {
                if (response.StatusCode == 200) {
                    var recordRoles = response.Result.length;
                    for (var i = 0; i < recordRoles; i++) {
                        var data = response.Result[i];
                        var tableData = `<tr data-countryId="${data.Id}">
                            <td>${i + 1}</td>
                            <td>${data.CountryName}</td>
                            <td>${data.CountryCode}</td>
                            <td>${data.PhoneCode}</td>
                            <td>${data.StateCount}</td>
                            <td><span class="badge bg-success gotostate">Go to State</span></td>
                        </tr>`
                        $('#tblCountryListBody').append(tableData);
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
    $(document).on("click", ".gotostate", function () {
        $("#StateSection").removeClass("d-none")
        $("#CountrySection").addClass("d-none")
        currentSection = "State"
        clickedCountryId = $(this).data("countryId")
        GetStateList()
    })
    
    function GetStateList() {
        $('#tblStateListBody').html(``)
        $.ajax({
            url: "https://localhost:7134/" + "api/Home/GetState",
            type: "GET",
            success: function (response) {
                if (response.StatusCode == 200) {
                    var recordRoles = response.Result.length;
                    for (var i = 0; i < recordRoles; i++) {
                        var data = response.Result[i];
                        var tableData = `<tr data-stateId="${data.Id}">
                            <td>${i + 1}</td>
                            <td>${data.StateName}</td>
                            <td>${data.StateCode}</td>
                            <td>${data.CityCount}</td>
                            <td><span class="badge bg-success gotocity">Go to City</span></td>
                        </tr>`
                        $('#tblStateListBody').append(tableData);
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

    $(document).on("click", ".gotocity", function () {
        $("#CitySection").removeClass("d-none")
        $("#CountrySection").addClass("d-none")
        $("#StateSection").addClass("d-none")
        currentSection = "City"
        clickedStateId = $(this).closest().data("stateId")
        GetCityList()
    })

    $(document).on("click", ".backtostate, .backtocountry", function () {
        if (currentSection == "City") {
            $("#CitySection").addClass("d-none")
            $("#CountrySection").addClass("d-none")
            $("#StateSection").removeClass("d-none")
            currentSection = "State"
        } else if (currentSection == "State") {
            $("#CitySection").addClass("d-none")
            $("#CountrySection").removeClass("d-none")
            $("#StateSection").addClass("d-none")
            currentSection = "Country"
        } 
    })
    function GetCityList() {
        $('#tblCityListBody').html(``)
        $.ajax({
            url: "https://localhost:7134/" + "api/Home/GetCity",
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
    if (currentSection == "City") {

    }
})