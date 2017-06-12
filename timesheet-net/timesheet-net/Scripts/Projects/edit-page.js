$(function () {
    getAssignedEmployeesToProject(pageProjID)
        .done(function (response) {
            $("#assignedEmpls li").remove();
            $.each(response, function (index, value) {
                $("#assignedEmpls").append('<li>' + value.NameAndSurname +
                    '<div class="delete-me" onclick="deleteEmployeeFromProject(' +
                        pageProjID + ',' + value.EmployeeID + ');">X</div></li>');
            });
        });

    getEmployeeList()
        .done(function (response) {
            $("#userList li").remove();
            $.each(response, function (index, value) {
                $("#userList").append('<li><a href="#" onclick="addEmployeeToProject(' + pageProjID +
                                           ',' + value.EmployeeID + ')">' + value.NameAndSurname + '</a></li>');
            });
        });

    datepickersSetup();
});

$("#userToAdd").bind("input", function () {
    delayTimer = setTimeout(function () {
        getEmployeeList($("#userToAdd").val())
            .done(function (response) {
                $("#userList li").remove();
                $.each(response, function (index, value) {
                    $("#userList").append('<li><a href="#">' + value.NameAndSurname + '</a></li>');
                });
            });
    }, 300);
});

function addEmployeeToProject(projId, emplId)
{
    assignEmployeeToProject(projId, emplId)
        .done(function (response) {
            if (response.Error === 1) {
                console.log("Error");
            } else {
                getAssignedEmployeesToProject(pageProjID)
                .done(function (response) {
                    $("#assignedEmpls li").remove();
                    $.each(response, function (index, value) {
                        $("#assignedEmpls").append('<li>' + value.NameAndSurname +
                            '<div class="delete-me" onclick="deleteEmployeeFromProject(' +
                            pageProjID + ',' + value.EmployeeID + ');">X</div></li>');
                    });
                });
            }
        });
}

function deleteEmployeeFromProject(projId, emplId)
{
    unassignEmployeeFromProject(projId, emplId)
        .done(function (response) {
            if (response.Error === 1) {
                console.log("Error");
            } else {
                getAssignedEmployeesToProject(pageProjID)
                .done(function (response) {
                    $("#assignedEmpls li").remove();
                    $.each(response, function (index, value) {
                        $("#assignedEmpls").append('<li>' + value.NameAndSurname +
                            '<div class="delete-me" onclick="deleteEmployeeFromProject(' +
                            pageProjID + ',' + value.EmployeeID + ');">X</div></li>');
                    });
                });
            }
        });
}
