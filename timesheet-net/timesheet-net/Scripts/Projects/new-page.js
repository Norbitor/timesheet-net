var assignedEmployeesList = [];

$(function () {
    getEmployeeList()
        .done(function (response) {
            $("#userList li").remove();
            $.each(response, function (index, value) {
                $("#userList").append('<li><a href="#" onclick="addEmployeeToProject(' +
                                           value.EmployeeID + ',\'' + value.NameAndSurname +
                                           '\')">' + value.NameAndSurname + '</a></li>');
            });
        });
    
    var today = new Date();
    $('#start-datepicker').datetimepicker({
        locale: 'pl',
        format: 'YYYY-MM-DD',
        defaultDate: today
    });
    $('#finish-datepicker').datetimepicker({
        locale: 'pl',
        format: 'YYYY-MM-DD'
    });
    $('#finish-datepicker').data("DateTimePicker").minDate(today);
    $("#start-datepicker").on("dp.change", function (e) {
        $('#finish-datepicker').data("DateTimePicker").minDate(e.date);
    });
    $("#finish-datepicker").on("dp.change", function (e) {
        $('#start-datepicker').data("DateTimePicker").maxDate(e.date);
    });

    $('#newProjectForm').submit(function (eventObj) {
        for (var i = 0; i < assignedEmployeesList.length; i++) {
            $('#newProjectForm').append('<input type="hidden" name="ProjectMembers[]" value="' + assignedEmployeesList[i].emplId + '" />');
        }        
        return true;
    });
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

function addEmployeeToProject(emplId, nameAndSurname) {
    for (var i = 0; i < assignedEmployeesList.length; i++) {
        if (assignedEmployeesList[i].emplId == emplId) {
            return;
        }
    }
    assignedEmployeesList.push({ emplId: emplId, nameAndSurname: nameAndSurname });
    renderEmployeeList();
}

function deleteEmployeeFromProject(emplId) {
    for (var i = 0; i < assignedEmployeesList.length; i++)
    {
        if (assignedEmployeesList[i].emplId == emplId) {
            assignedEmployeesList.splice(i, 1);
            break;
        }
    }
    renderEmployeeList();
}

function renderEmployeeList()
{
    $("#assignedEmpls li").remove();
    $.each(assignedEmployeesList, function (index, value) {
        $("#assignedEmpls").append('<li>' + value.nameAndSurname +
            '<div class="delete-me" onclick="deleteEmployeeFromProject(' +
            value.emplId + ');">X</div></li>');
    });
    
}
