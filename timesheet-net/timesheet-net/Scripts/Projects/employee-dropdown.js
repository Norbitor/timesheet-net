function getAssignedEmployeesToProject(projectId) {
    return $.ajax({
        url: '/Ajax/GetAssignedEmplotyeesToProject',
        type: 'post',
        data: {
            projID: projectId
        }
    });
}

function getEmployeeList(needle) {
    return $.ajax({
        url: '/Ajax/GetEmployeeList',
        type: 'post',
        data: {
            pattern: needle
        }
    });
}

function assignEmployeeToProject(projId, emplId) {
    return $.ajax({
        url: '/Ajax/AssignEmployeeToProject',
        type: 'post',
        data: {
            projID: projId,
            emplID: emplId
        }
    });
}
