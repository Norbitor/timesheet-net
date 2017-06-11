$(function () {
    getAssignedEmployeesToProject(pageProjID)
        .done(function (response) {
            $("#userList li").remove();
            $.each(response, function (index, value) {
                $("#assignedEmpls").append('<li>' + value.NameAndSurname + '</li>');
            })
        });

    getEmployeeList()
        .done(function (response) {
            $("#userList li").remove();
            $.each(response, function (index, value) {
                $("#userList").append('<li><a href="#">' + value.NameAndSurname + '</a></li>');
            })
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
                })
            });
    }, 300);
});